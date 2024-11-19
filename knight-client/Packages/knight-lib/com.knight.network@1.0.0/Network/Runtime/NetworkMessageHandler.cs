using System;

namespace Knight.Framework.Network
{
    public interface INetworkMessageHandler
    {
        void OnTcpReceived(uint nCMD, uint nACT, int nIndex, in ReadOnlySpan<byte> rBodyBytes);
    }
}
