
#pragma warning disable 1591

namespace Alaris.Network
{
    /// <summary>
    ///   List of possible Opcodes.
    /// </summary>
    public enum Opcode
    {
        ScmsgPacketNull = 0x0,
        CmsgRequestAuth = 0x01,
        SmsgAuthApproved = 0x02,
        SmsgAuthDenied = 0x03,
        CmsgRequestConfig = 0x04,
        SmsgConfigResponse = 0x05,
        CmsgCloseConnection = 0x06,
        SmsgCloseConnection = 0x07,
        CmsgRequestACSRandom = 0x08,
        SmsgSendACSRandom = 0x09
    }
}

#pragma warning restore 1591