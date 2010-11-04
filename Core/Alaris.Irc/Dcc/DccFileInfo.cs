using System;
using System.IO;

namespace Alaris.Irc.Dcc
{
	/// <summary>
	/// Manages the information about the file being
	/// transfered. 
	/// </summary>
	public sealed class DccFileInfo
	{
		private readonly FileInfo _fileInfo;
		private FileStream _fileStream;
		//Where in the file to start reading or writing
		private long _fileStartingPosition;
		//Number of bytes sent or received so far in this
		//session
		private long _bytesTransfered;
		//Total number of bytes to send or receive
		private readonly long _completeFileSize;
		//The last position ack value
		private long _lastAckValue;

		/// <summary>
		/// Create a new instance using information sent from the remote user
		/// in his DCC Send message.
		/// </summary>
		/// <param name="fileInfo">The file being received</param>
		/// <param name="completeFileSize">The size of the file being received as specified in the DCC Send
		/// request.</param>
		public DccFileInfo( FileInfo fileInfo, long completeFileSize)
		{	
			_fileInfo = fileInfo;
			_completeFileSize = completeFileSize;
			_fileStartingPosition = 0;
			_bytesTransfered = 0;
		}
		/// <summary>
		/// Create a new instance using the file information from a local file
		/// to be sent to a remote user.
		/// </summary>
		/// <param name="fileInfo">The local file being sent</param>
		/// <exception cref="ArgumentException">If the file does not already exist.</exception>
		public DccFileInfo( FileInfo fileInfo) 
		{
		    if (fileInfo == null) throw new ArgumentNullException("fileInfo");
		    _fileInfo = fileInfo;
			if( !fileInfo.Exists ) 
			{
				throw new ArgumentException( fileInfo.Name + " does not exist.");
			}
			_completeFileSize = fileInfo.Length;
			_fileStartingPosition = 0;
			_bytesTransfered = 0;
		}
		/// <summary>
		/// Create a new instance using the file information from a local file
		/// to be sent to a remote user.
		/// </summary>
		/// <param name="fileName">The full pathname of local file being sent</param>
		/// <exception cref="ArgumentException">If the file does not already exist.</exception>
		public DccFileInfo( string fileName ) 
		{
			_fileInfo = new FileInfo(fileName);
			if( !_fileInfo.Exists ) 
			{
				throw new ArgumentException( fileName + " does not exist.");
			}
			_completeFileSize = _fileInfo.Length;
			_fileStartingPosition = 0;
			_bytesTransfered = 0;
		}

		/// <summary>
		/// Where to start reading or writing a file. Used during DCC Resume actions.
		/// </summary>
		/// <value>A read-only long indicating the location within the file.</value>
		public long FileStartingPosition 
		{
			get 
			{
				return _fileStartingPosition;
			}
		}
		/// <summary>
		/// The number of bytes sent or received so far. This Property
		/// is thread safe.
		/// </summary>
		/// <value>A read-only long.</value>
		public long BytesTransfered
		{
			get 
			{
				lock (this ) 
				{
					return _bytesTransfered;
				}
			}
		}
		/// <summary>
		/// The length of the file. This number is either the actual size
		/// of a file being sent or the number sent in the DCC SEND request.
		/// </summary>
		/// <value>A read-only long.</value>
		public long CompleteFileSize 
		{
			get 
			{
				return _completeFileSize;
			}
		}
		/// <summary>
		/// The file's name with all spaces converted to underscores and
		/// without the path.
		/// </summary>
		/// <value>A read-only string.</value>
		public string DccFileName 
		{
			get 
			{
				return DccUtil.SpacesToUnderscores(_fileInfo.Name);
			}
		}

		internal FileStream TransferStream 
		{
			get 
			{
				return _fileStream;
			}
		}

		/// <summary>
		/// Add the most recent number of bytes received
		/// to the total count.
		/// </summary>
		/// <param name="additionalBytes"></param>
		internal void AddBytesTransfered( int additionalBytes ) 
		{
			lock( this ) 
			{
				_bytesTransfered += additionalBytes;
			}
		}
		/// <summary>
		/// Does the position sent in the DCC Accept message
		/// match what we expect?
		/// </summary>
		internal bool AcceptPositionMatches( long position ) 
		{
			return position == _fileStartingPosition;
		}
		/// <summary>
		/// Our Resume request was accepted so start
		/// writing at the current position + 1.
		/// </summary>
		internal void GotoWritePosition() 
		{
			_fileStream.Seek( _fileStartingPosition +1, SeekOrigin.Begin );
		}
		/// <summary>
		/// Advance to the correct reading start
		/// position.
		/// </summary>
		internal void GotoReadPosition() 
		{
			_fileStream.Seek( _fileStartingPosition, SeekOrigin.Begin );
		}
		/// <summary>
		/// Is the position where the remote user would to to resume
		/// valid?
		/// </summary>
		internal bool ResumePositionValid( long position ) 
		{
			return position > 1 && position < _fileInfo.Length;
		}
		/// <summary>
		/// Can this file be resumed, i.e. does it
		/// support random access?
		/// </summary>
		internal bool CanResume() 
		{
			return _fileStream.CanSeek;
		}
		/// <summary>
		/// Start a Resume where the file last left off.
		/// </summary>
		internal void SetResumeToFileSize() 
		{
			_fileStartingPosition = _fileInfo.Length;
		}
		/// <summary>
		/// Set the point at which the transfer will begin
		/// </summary>
		internal void SetResumePosition( long resumePosition ) 
		{
			_fileStartingPosition = resumePosition;
			_bytesTransfered = _fileStartingPosition;
		}
		/// <summary>
		/// Where in the file is the transfer currently at?
		/// </summary>
		internal long CurrentFilePosition() 
		{
			return BytesTransfered + _fileStartingPosition;
		}
		/// <summary>
		/// Have all the file's bytes been sent/received?
		/// </summary>
		internal Boolean AllBytesTransfered()
		{
		    return _completeFileSize != 0 && (_fileStartingPosition + BytesTransfered) == _completeFileSize;
		}

	    /// <summary>
		/// Close the file stream.
		/// </summary>
		internal void CloseFile() 
		{
			if( _fileStream != null ) 
			{
				_fileStream.Close();
			}
		}
		/// <summary>
		/// Set this file stream to a read only one.
		/// </summary>
		internal void OpenForRead() 
		{
			_fileStream = _fileInfo.OpenRead();
		}
		/// <summary>
		/// Set this file stream to a write only one.
		/// </summary>
		internal void OpenForWrite() 
		{
			_fileStream = _fileInfo.OpenWrite();
		}
		/// <summary>
		/// Should we try to resume this file download?
		/// </summary>
		internal bool ShouldResume() 
		{
			return _fileInfo.Length > 0 && CanResume();
		}
		/// <summary>
		/// Determine whether the acks sent during an upload
		/// signal that all bytes have been sent.
		/// 
		/// BitchX sends bad acks after a resume but we can
		/// catch that by testing for the same ack sent twice.
		/// I sure hope others behave better since I don't
		/// want to write special code for every IRC client.
		/// </summary>
		/// <param name="ack"></param>
		/// <returns>True if the acks are done</returns>
		internal bool AcksFinished( long ack ) 
		{
			bool done = (ack == BytesTransfered || ack == _lastAckValue);
			_lastAckValue = ack;
			return done;
		}
	
	}
}
