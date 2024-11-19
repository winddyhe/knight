using Google.Protobuf;
using Knight.Core;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Knight.Framework.Network
{
    public enum NetworkType
    {
        MainTcp,
        BattleTcp,
    }

    public class NetworkTcpClient
    {
        public delegate void OnTcpReceived(uint nCMD, uint nACT, int nIndex, in ReadOnlySpan<byte> rBodyBytes);

        private NetworkType mNetworkType;
        private TouchSocketConfig mSocketConfig;
        private TcpClient mTcpClient;

        private IReceiver<IReceiverResult> mReceiver;
        private OnTcpReceived mOnTcpReceived;

        private ArrayPool<byte> mSendArrayPool;

        public NetworkType NetworkType => this.mNetworkType;

        public void Initialize(NetworkType rNetworkType, OnTcpReceived rOnTcpReceived)
        {
            this.mNetworkType = rNetworkType;
            this.mOnTcpReceived = rOnTcpReceived;
            this.mSendArrayPool = new ArrayPool<byte>(5, 1024 * 1024);
        }

        public async Task ConnectAsync(string rRemoteHost)
        {
            // 设置Socket配置
            this.mSocketConfig = new TouchSocketConfig();
            this.mSocketConfig.SetRemoteIPHost(rRemoteHost);
            this.mSocketConfig.ConfigureContainer(a =>
            {
                a.AddLogger(new NetworkLogger());
            });
            this.mSocketConfig.ConfigurePlugins(a =>
            {
                a.Add(new NetworkTouchSocketPlugin(this.OnTcpReceived_PackageProccess));
            });

            // 创建TcpClient
            this.mTcpClient = new TcpClient();
            await this.mTcpClient.SetupAsync(this.mSocketConfig);

            // 开始连接
            await this.mTcpClient.ConnectAsync();
            this.mTcpClient.Logger.Info($"TcpClient successfully connected to {rRemoteHost}.");
        }

        private void OnTcpReceived_PackageProccess(in NetworkPackageHeader rPackageHeader, ByteBlock rByteBlock)
        {
            // @TODO: 这里处理数据解密，解压缩，Header标记，心跳消息等

            // 外层接收消息
            var rBodyBytes = rByteBlock.Span.Slice(NetworkPackageHeader.BodyPos, (int)rPackageHeader.DataLen);
            this.mOnTcpReceived?.Invoke(rPackageHeader.Cmd, rPackageHeader.Act, rPackageHeader.Index, in rBodyBytes);
        }

        public async Task SendAsync(uint nCMD, uint nACT, ushort nIndex, IMessage rRequestMsg)
        {
            byte[] rMsgBytes = null;
            // 序列化消息
            try
            {
                var nMessageSize = this.PackMessage(nCMD, nACT, nIndex, rRequestMsg, out rMsgBytes);
                // 发送
                var rMemorySpan = rMsgBytes.AsMemory(0, nMessageSize);
                await this.mTcpClient.SendAsync(rMsgBytes);
            }
            catch (Exception e)
            {
                this.mTcpClient.Logger.Error(e.ToString());
            }
            finally
            {
                // 释放消息字节数组
                if (rMsgBytes!= null)
                {
                    this.mSendArrayPool.Return(rMsgBytes);
                }
            }
        }

        public async Task SendAsync(uint nCMD, uint nACT, ushort nIndex, byte[] rBodyBytes)
        {
            byte[] rMsgBytes = null;
            // 序列化消息
            try
            {
                var nMessageSize = this.PackMessage(nCMD, nACT, nIndex, rBodyBytes, out rMsgBytes);
                // 发送
                var rMemorySpan = rMsgBytes.AsMemory(0, nMessageSize);
                await this.mTcpClient.SendAsync(rMsgBytes);
            }
            catch (Exception e)
            {
                this.mTcpClient.Logger.Error(e.ToString());
            }
            finally
            {
                // 释放消息字节数组
                if (rMsgBytes != null)
                {
                    this.mSendArrayPool.Return(rMsgBytes);
                }
            }
        }

        private int PackMessage(uint nCMD, uint nACT, ushort nIndex, IMessage rRequestMsg, out byte[] rMsgBytes)
        {
            int nMessageBodySize = rRequestMsg.CalculateSize();
            short nErrorCode = 0;
            byte nFlags = 0;
            byte nBcc = 0;
            int nMessageSize = NetworkPackageHeader.HeaderSize + nMessageBodySize;

            rMsgBytes = this.mSendArrayPool.Rent(nMessageSize);
            var rByteSpan = rMsgBytes.AsSpan();

            // 填充Header
            MemoryMarshal.Write(rByteSpan, ref nMessageBodySize);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.ErrorCodePos), ref nErrorCode);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.CmdPos), ref nCMD);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.ActPos), ref nACT);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.IndexPos), ref nIndex);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.FlagsPos), ref nFlags);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.BccPos), ref nBcc);
            // 填充Body
            rRequestMsg.WriteTo(rByteSpan.Slice(NetworkPackageHeader.BodyPos));

            return nMessageSize;
        }

        private int PackMessage(uint nCMD, uint nACT, ushort nIndex, byte[] rBodyBytes, out byte[] rMsgBytes)
        {
            int nMessageBodySize = rBodyBytes.Length;
            short nErrorCode = 0;
            byte nFlags = 0;
            byte nBcc = 0;
            int nMessageSize = NetworkPackageHeader.HeaderSize + nMessageBodySize;

            rMsgBytes = this.mSendArrayPool.Rent(nMessageSize);
            var rByteSpan = rMsgBytes.AsSpan();

            // 填充Header
            MemoryMarshal.Write(rByteSpan, ref nMessageBodySize);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.ErrorCodePos), ref nErrorCode);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.CmdPos), ref nCMD);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.ActPos), ref nACT);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.IndexPos), ref nIndex);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.FlagsPos), ref nFlags);
            MemoryMarshal.Write(rByteSpan.Slice(NetworkPackageHeader.BccPos), ref nBcc);
            // 填充Body
            rBodyBytes.AsSpan().CopyTo(rByteSpan.Slice(NetworkPackageHeader.BodyPos));

            return nMessageSize;
        }
    }
}
