using System;
using Alaris.Core;
using System.Net.Sockets;
using System.Collections.Generic;
using Alaris.Extras;
using System.Text;
using System.Threading;

namespace Alaris.Client
{
	/// <summary>
	/// Delegate for handling client packets.
	/// </summary>
	public delegate void ClientPacketHandlerDelegate(AlarisPacket packet, string hst);
	
	/// <summary>
	/// Packet handler used by the client.
	/// </summary>
	public class ClientPacketHandler
	{
		/// <summary>
		/// Occurs when auth is denied.
		/// </summary>
		public event ClientPacketHandlerDelegate OnAuthDenied;
		/// <summary>
		/// Occurs when auth is approved.
		/// </summary>
		public event ClientPacketHandlerDelegate OnAuthApproved;
		/// <summary>
		/// Occurs when ACS sends a random number packet back.
		/// </summary>
		public event ClientPacketHandlerDelegate OnAcsRandom;
		
		private ClientPacketHandler ()
		{
			OnAuthApproved += AuthApprovedHandler;
			OnAuthDenied += AuthDeniedHandler;
			OnAcsRandom += AcsRandHandler;
		}
		
		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			
		}
		
		/// <summary>
		/// Handles the packet.
		/// </summary>
		/// <param name='packet'>
		/// Packet.
		/// </param>
		/// <param name='client'>
		/// Client.
		/// </param>
		public void HandlePacket(AlarisPacket packet, TcpClient client)
		{
			var hst = client.Client.RemoteEndPoint.ToString().Split(':')[0];
			var packetid = packet.Read<int>();
			Log.Debug("PacketHandler", "Got packet with ID: " + packetid + " from: " + client.Client.RemoteEndPoint);
			
			if(packetid == (int)Opcode.SMSG_AUTH_DENIED)
				OnAuthDenied(packet, hst);
			else if(packetid == (int)Opcode.SMSG_AUTH_APPROVED)
				OnAuthApproved(packet, hst);
			else if(packetid == (int)Opcode.SMSG_SEND_ACS_RANDOM)
				OnAcsRandom(packet, hst);
		}
		
		/// <summary>
		/// The auth denied packet handler. (SMSG_AUTH_DENIED)
		/// </summary>
		/// <param name='pck'>
		/// Packet.
		/// </param>
		/// <param name='hst'>
		/// Host.
		/// </param>
		public void AuthDeniedHandler(AlarisPacket pck, string hst)
		{
			Log.Error("AlarisServer", "Authentication denied to ACS server.");
			Thread.Sleep(1000);
			var sBot = Singleton<AlarisBot>.Instance;
			
			sBot.Disconnect("Auth denied.");
		}
		
		/// <summary>
		/// The auth approved packet handler. (SMSG_AUTH_APPROVED)
		/// </summary>
		/// <param name='pck'>
		/// Packet.
		/// </param>
		/// <param name='hst'>
		/// Host.
		/// </param>
		public void AuthApprovedHandler(AlarisPacket pck, string hst)
		{
			Log.Success("Alaris", "Successfully authed to ACS server.");
		}
		
		/// <summary>
		/// The ACS Random number handler. (SMSG_SEND_ACS_RANDOM)
		/// </summary>
		/// <param name='pck'>
		/// Packet.
		/// </param>
		/// <param name='hst'>
		/// Host.
		/// </param>
		public void AcsRandHandler(AlarisPacket pck, string hst)
		{
			// read random value.
			var rand = pck.Read<int>();
			// read channel
			var chan = pck.Read<string>();
			
			var sBot = Singleton<AlarisBot>.Instance;
			
			if(string.IsNullOrEmpty(chan) || chan == "0")
				chan = sBot.acs_rand_request_channel;
			
			sBot.SendMsg(chan, "ACS sent random: " + rand.ToString());
		}
		
		
	}
}

