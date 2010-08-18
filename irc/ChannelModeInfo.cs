using System;
using System.Collections;


namespace Alaris.Irc
{
	/// <summary>
	/// A simple struct designed to hold al the attributes that 
	/// are contain in a Channel mode. 
	/// </summary>
	public sealed class ChannelModeInfo
	{
		
		private ModeAction action;
		private ChannelMode mode;
		private string parameter;
	
		/// <summary>
		/// Whether the mode is being added or removed. In the case of a Channel mode
		/// request this will always be 'added'.
		/// </summary>
		public ModeAction Action
		{
			get
			{
				return action;
			}
			set
			{
				action = value;
			}
		}

		/// <summary>
		/// What mode is being added or removed.
		/// </summary>
		public ChannelMode Mode
		{
			get
			{
				return mode;
			}
			set
			{
				mode = value;
			}
		}
		/// <summary>
		/// Any additional parameters that belong to the mode. For example
		/// user masks or a maximum numbers of user allowed in a channel.
		/// </summary>
		public string Parameter
		{
			get
			{
				return parameter;
			}
			set
			{
				parameter = value;
			}
		}
	
	
		public override string ToString() 
		{
			return string.Format("Action={0} Mode={1} Parameter={2}", Action, Mode, Parameter ); 
		}

		internal static ChannelModeInfo[] ParseModes( string[] tokens, int start)
		{	
			//This nice piece of code was contributed by Klemen �avs.
			//25 October 2003
			ArrayList modeInfoArray = new ArrayList();
			int i = start;
			while (i < tokens.Length)
			{
				ChannelModeInfo modeInfo = new ChannelModeInfo();
				int parmIndex = i + 1;
				for (int j = 0; j < tokens[i].Length; j++)
				{
						
					while (j < tokens[i].Length && tokens[i][j] == '+')
					{
						modeInfo.Action = ModeAction.Add;
						j++;
					}
					while (j < tokens[i].Length && tokens[i][j] == '-')
					{
						modeInfo.Action = ModeAction.Remove;
						j++;
					}
					if (j == 0)
					{
						throw new Exception();
					}
					else if (j < tokens[i].Length)
					{
						switch (tokens[i][j])
						{	
							case 'o':
							case 'h':
							case 'v':
							case 'b':
							case 'e':
							case 'I':
							case 'k':
							case 'O':
								modeInfo.Mode = Rfc2812Util.CharToChannelMode(tokens[i][j]);
								modeInfo.Parameter = tokens[parmIndex++];
								break;
							case 'l':
								modeInfo.Mode = Rfc2812Util.CharToChannelMode(tokens[i][j]);
								if (modeInfo.Action == ModeAction.Add)
								{
									modeInfo.Parameter = tokens[parmIndex++];
								}
								else
								{
									modeInfo.Parameter = "";
								}
								break;
							default:
								modeInfo.Mode = Rfc2812Util.CharToChannelMode(tokens[i][j]);
								modeInfo.Parameter = "";
								break;
						}

					}
					modeInfoArray.Add( modeInfo.MemberwiseClone() );
				}
				i = parmIndex;
			}
	
			ChannelModeInfo[] modes = new ChannelModeInfo[ modeInfoArray.Count ];
			for (int k = 0; k < modeInfoArray.Count; k++)
			{
				modes[k] = (ChannelModeInfo)modeInfoArray[k];
			}
			return modes;
		}
	}
}
