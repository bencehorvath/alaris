using System;
using Alaris.Core;
using System.Net.Sockets;
using System.Collections.Generic;
using Alaris.Extras;
using System.Text;
using Alaris.Network;

namespace Alaris.Server
{
	public delegate void ServerPacketHandlerDelegate(AlarisPacket packet, string hst);
	
	
	public class ServerPacketHandler
	{
		public int BackPort = 35221;
		
		public event ServerPacketHandlerDelegate OnAuthRequest;
		public event ServerPacketHandlerDelegate OnAcsRandomRequest;
		private ServerPacketHandler ()
		{
			OnAuthRequest += AuthRequestPacketHandler;
			OnAcsRandomRequest += AcsRandomHandler;
		}
		
		public void Init()
		{
			
		}
		
		public void HandlePacket(AlarisPacket packet, TcpClient client)
		{
			var hst = client.Client.RemoteEndPoint.ToString().Split(':')[0];
			var packetid = packet.Read<int>();
			Log.Debug("PacketHandler", "Got packet with ID: " + packetid + " from: " + client.Client.RemoteEndPoint);
			
			if(packetid == (int)Opcode.CmsgRequestAuth)
				OnAuthRequest(packet,hst);
			else if(packetid == (int)Opcode.CmsgRequestACSRandom)
				OnAcsRandomRequest(packet, hst);
		}
		
		private void AcsRandomHandler(AlarisPacket pck, string hst)
		{
			// read chan
			var chan = pck.Read<string>();
			
			var rand = new Random();
			int random = rand.Next();
			
			Log.Notice("Random", "Sending random value: " + random + " to client. Will send it to: " + chan + ".");
			
			var packet = new AlarisPacket();
			packet.Write<int>((int)Opcode.SmsgSendACSRandom);
			packet.Write<int>(random);
			packet.Write<string>((string)chan);
			
			SendPacketBack(packet, hst, BackPort);
		}
		
		private void AuthRequestPacketHandler(AlarisPacket pck, string hst)
		{
			// opcode is already read, DO _NOT_ READ AGAIN
			
			string guid = pck.Read<string>();
			string hash = pck.Read<string>();
			int bck = pck.Read<int>();
			
			BackPort = bck;
			
			pck.Dispose();
			
			if(hash != Utilities.MD5String("twlbot"))
			{
				Log.Notice("AuthHandler", "Auth unsuccessful. Guid of client: " + guid);
				Log.Debug("Security", "Hash was: " + hash);
				Log.Notice("AuthHandler", "Back port is: " + bck);
				var packet = new AlarisPacket();
				
				packet.Write<int>((int)Opcode.SmsgAuthDenied);
				packet.Write<int>((int)0);
				
				SendPacketBack(packet, hst, BackPort);

			}
			else
			{
				Log.Success("AuthHandler", "Auth successful. Guid of client: " + guid);
				Log.Debug("Security", "Hash was: " + hash);
				Log.Notice("AuthHandler", "Back port is: " + bck);
				var packet = new AlarisPacket();
				packet.Write<int>((int)Opcode.SmsgAuthApproved);
				packet.Write<int>((int)1);
				
				SendPacketBack(packet, hst, BackPort);
			}
		}
		
		public void SendPacketBack(AlarisPacket packet, string hst, int backport)
		{
			
			
			var tcp = new TcpClient();
			Log.Notice("PacketHandler", "SendPacketBack(): host is: " + hst + ", port is: " + backport);
			tcp.Connect(hst, backport);
			
			if(tcp.Connected)
			{
				var stream = tcp.GetStream();
				
				if(stream.CanWrite)
				{
					var buff = new UTF8Encoding().GetBytes(packet.GetNetMessage());
					
					stream.Write(buff, 0, buff.Length);
					stream.Flush();
					
					stream.Close();
					
					tcp.Close();
				}
			}
		}
	}
}

