using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using Knight.Core;

namespace Knight.Framework.Network
{
    public class NetworkTcpManager : TSingleton<NetworkTcpManager>
    {
        private INetworkMessageHandler mMessageHandler;
        private Dictionary<NetworkType, NetworkTcpClient> mTcpClients;

        private NetworkTcpManager()
        {
        }

        public void Initialize(INetworkMessageHandler rMessageHandler)
        {
            this.mTcpClients = new Dictionary<NetworkType, NetworkTcpClient>();
            this.mMessageHandler = rMessageHandler;
        }

        public async Task ConnectTcpAsync(NetworkType rNetworkType, string rRemoteHost)
        {
            var rTcpClient = new NetworkTcpClient();
            rTcpClient.Initialize(rNetworkType, this.OnTcpReceived);
            await rTcpClient.ConnectAsync(rRemoteHost);
            this.mTcpClients.Add(rNetworkType, rTcpClient);
        }

        public void OnTcpReceived(uint nCMD, uint nACT, int nIndex, in ReadOnlySpan<byte> rBodyBytes)
        {
            this.mMessageHandler?.OnTcpReceived(nCMD, nACT, nIndex, in rBodyBytes);
        }

        public async Task SendAsync(NetworkType rNetworkType, uint nCMD, uint nACT, ushort nIndex, IMessage rMessage)
        {
            if (this.mTcpClients.TryGetValue(rNetworkType, out var rTcpClient))
            {
                await rTcpClient.SendAsync(nCMD, nACT, nIndex, rMessage);
            }
        }
    }
}
