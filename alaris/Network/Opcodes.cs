using System;

#pragma warning disable 1591

namespace Alaris.Core
{
	/// <summary>
	/// List of possible Opcodes.
	/// </summary>
	public enum Opcode : int
	{
		SCMSG_PACKET_NULL = 0x0,
		CMSG_REQUEST_AUTH = 0x01,
		SMSG_AUTH_APPROVED = 0x02,
		SMSG_AUTH_DENIED = 0x03,
		CMSG_REQUEST_CONFIG = 0x04,
		SMSG_CONFIG_RESPONSE = 0x05,
		CMSG_CLOSE_CONNECTION = 0x06,
		SMSG_CLOSE_CONNECTION = 0x07,
		CMSG_REQUEST_ACS_RANDOM = 0x08,
		SMSG_SEND_ACS_RANDOM = 0x09
	}
}

#pragma warning restore 1591
