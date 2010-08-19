using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Alaris.Network;

namespace Alaris.Core
{
    /// <summary>
    ///   Listener used inside the Alaris bot to get and handle ACS responses.
    /// </summary>
    public sealed class ClientListener : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly Thread _lThread;
        private readonly ClientPacketHandler sClientPacketHandler = Singleton<ClientPacketHandler>.Instance;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Alaris.Core.ClientListener" /> class.
        /// </summary>
        /// <param name = 'port'>
        ///   Port to listen on.
        /// </param>
        public ClientListener(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            /*_lThread = new Thread(Listen);
			
			_lThread.Start();*/

            sClientPacketHandler.Init();
        }

        /// <summary>
        ///   Start listening.
        /// </summary>
        public void Listen()
        {
            _listener.Start();
            Log.Notice("AlarisListener", "Started...");

            while (true)
            {
                var client = _listener.AcceptTcpClient();

                Log.Notice("AlarisListener", "Client connection from: " + client.Client.RemoteEndPoint);

                var client_thread = new Thread(ClientHandler);

                client_thread.Start(client);

                Thread.Sleep(50);
            }
        }

        /// <summary>
        ///   Client handler procedure.
        /// </summary>
        /// <param name = 'ob'>
        ///   The object passed with ParameterizedThreadStart (a TcpClient)
        /// </param>
        public void ClientHandler(object ob)
        {
            var client = (ob as TcpClient);

            var stream = client.GetStream();

            var message_buffer = new byte[262144];

            int bytes_read;

            Log.Notice("ClientHandler", "Handling client...");

            while (true)
            {
                bytes_read = 0;

                // read

                if (stream.DataAvailable && stream.CanRead)
                {
                    Log.Debug("ClientHandler", "Stream data available, reading.");
                    bytes_read = stream.Read(message_buffer, 0, message_buffer.Length);

                    if (bytes_read == 0)
                    {
                        Log.Notice("ClientHandler", "Lost connection.");
                        break;
                    }

                    var encoding = new UTF8Encoding();

                    var msg = encoding.GetString(message_buffer, 0, bytes_read);

                    var packet = new AlarisPacket(msg);

                    sClientPacketHandler.HandlePacket(packet, client);
                    packet.Dispose();
                }

                Thread.Sleep(50);
            }
        }

        /// <summary>
        ///   Releases all resource used by the <see cref = "Alaris.Core.ClientListener" /> object.
        /// </summary>
        /// <remarks>
        ///   Call <see cref = "Dispose" /> when you are finished using the <see cref = "Alaris.Core.ClientListener" />. The
        ///   <see cref = "Dispose" /> method leaves the <see cref = "Alaris.Core.ClientListener" /> in an unusable state. After
        ///   calling <see cref = "Dispose" />, you must release all references to the <see cref = "Alaris.Core.ClientListener" /> so
        ///   the garbage collector can reclaim the memory that the <see cref = "Alaris.Core.ClientListener" /> was occupying.
        /// </remarks>
        public void Dispose()
        {
            /*_lThread.Join(1000);
			_lThread.Abort();*/
        }
    }
}