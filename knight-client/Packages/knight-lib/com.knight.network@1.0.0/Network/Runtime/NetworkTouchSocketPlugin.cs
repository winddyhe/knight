using System;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Knight.Framework.Network
{
    public class NetworkTouchSocketPlugin : PluginBase, ITcpReceivedPlugin
    {
        public delegate void OnTcpReceived_PackageProccess(in NetworkPackageHeader rPackageHeader, ByteBlock rByteBlock);

        private OnTcpReceived_PackageProccess mOnTcpReceived_PackageProccess;

        public NetworkTouchSocketPlugin(OnTcpReceived_PackageProccess rOnTcpReceived_PackageProccess)
        {
            this.mOnTcpReceived_PackageProccess = rOnTcpReceived_PackageProccess;
        }

        public async Task OnTcpReceived(ITcpSession rClient, ReceivedDataEventArgs rArgs)
        {
            var rRequestInfo = rArgs.RequestInfo as NetworkPackage_FixedHeaderRequestInfo;
            var rByteBlock = rArgs.ByteBlock;

            this.mOnTcpReceived_PackageProccess(in rRequestInfo.PackageHeader, rByteBlock);

            await rArgs.InvokeNext();
        }
    }
}
