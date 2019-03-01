using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Knight.Core;

namespace Knight.Framework.Net
{
	public struct WaitSendBuffer
	{
		public byte[]   Bytes;
		public int      Index;
		public int      Length;

		public WaitSendBuffer(byte[] bytes, int index, int length)
		{
			this.Bytes  = bytes;
			this.Index  = index;
			this.Length = length;
		}
	}

	public class KChannel: AChannel
	{
		private UdpClient                       mSocket;
		private Kcp                             mKcp;

		private readonly CircularBuffer         mRecvBuffer = new CircularBuffer();
		private readonly Queue<WaitSendBuffer>  mSendBuffer = new Queue<WaitSendBuffer>();

		private readonly PacketParser           mParser;
		private bool                            mIsConnected;
		private readonly IPEndPoint             mRemoteEndPoint;

		private TaskCompletionSource<Packet>    mRecvTcs;

		private uint                            mLastRecvTime;
		private readonly byte[]                 mCacheBytes = new byte[ushort.MaxValue];

		public uint                             Conn;
		public uint                             RemoteConn;

		// accept
		public KChannel(uint nConn, uint nRemoteConn, UdpClient rSocket, IPEndPoint rRemoteEndPoint, KService rKService): base(rKService, ChannelType.Accept)
		{
			this.Id = nConn;
			this.Conn = nConn;
			this.RemoteConn = nRemoteConn;
			this.mRemoteEndPoint = rRemoteEndPoint;
			this.mSocket = rSocket;
			this.mParser = new PacketParser(this.mRecvBuffer);
			this.mKcp = new Kcp(this.RemoteConn, this.Output);
			this.mKcp.SetMtu(512);
			this.mKcp.NoDelay(1, 10, 2, 1);  //fast
			this.mIsConnected = true;
			this.mLastRecvTime = rKService.mTimeNow;
		}

		// connect
		public KChannel(uint nConn, UdpClient rSocket, IPEndPoint rRemoteEndPoint, KService rKService): base(rKService, ChannelType.Connect)
		{
			this.Id = nConn;
			this.Conn = nConn;
			this.mSocket = rSocket;
			this.mParser = new PacketParser(this.mRecvBuffer);

			this.mRemoteEndPoint = rRemoteEndPoint;
			this.mLastRecvTime = rKService.mTimeNow;
			this.Connect(rKService.mTimeNow);
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			for (int i = 0; i < 4; i++)
			{
				this.DisConnect();
			}

			this.mSocket = null;
		}

		private KService GetService()
		{
			return (KService)this.mService;
		}

		public void HandleConnnect(uint responseConn)
		{
			if (this.mIsConnected)
			{
				return;
			}
			this.mIsConnected = true;

			this.RemoteConn = responseConn;
			this.mKcp = new Kcp(responseConn, this.Output);
            this.mKcp.SetMtu(512);
            this.mKcp.NoDelay(1, 10, 2, 1);  //fast

            this.HandleSend();
		}

		public void HandleAccept(uint nRequestConn)
		{
            this.mCacheBytes.WriteTo(0, KcpProtocalType.ACK);
            this.mCacheBytes.WriteTo(4, nRequestConn);
            this.mCacheBytes.WriteTo(8, this.Conn);
			this.mSocket.Send(mCacheBytes, 12, mRemoteEndPoint);
		}

		/// <summary>
		/// 发送请求连接消息
		/// </summary>
		private void Connect(uint nTimeNow)
		{
            this.mCacheBytes.WriteTo(0, KcpProtocalType.SYN);
            this.mCacheBytes.WriteTo(4, this.Conn);
			//Log.Debug($"client connect: {this.Conn}");
			this.mSocket.Send(mCacheBytes, 8, mRemoteEndPoint);

			// 200毫秒后再次update发送connect请求
			this.GetService().AddToNextTimeUpdate(nTimeNow + 200, this.Id);
		}

		private void DisConnect()
		{
			this.mCacheBytes.WriteTo(0, KcpProtocalType.FIN);
			this.mCacheBytes.WriteTo(4, this.Conn);
			this.mCacheBytes.WriteTo(8, this.RemoteConn);
			//Log.Debug($"client disconnect: {this.Conn}");
			this.mSocket.Send(mCacheBytes, 12, mRemoteEndPoint);
		}

		public void Update(uint nTimeNow)
		{
			// 如果还没连接上，发送连接请求
			if (!this.mIsConnected)
			{
				Connect(nTimeNow);
				return;
			}

			// 超时断开连接
			if (nTimeNow - this.mLastRecvTime > 20 * 1000)
			{
				this.OnError((int)SocketError.Disconnecting);
				return;
			}
			this.mKcp.Update(nTimeNow);
			uint nextUpdateTime = this.mKcp.Check(nTimeNow);
			this.GetService().AddToNextTimeUpdate(nextUpdateTime, this.Id);
		}

		private void HandleSend()
		{
			while (true)
			{
				if (this.mSendBuffer.Count <= 0)
				{
					break;
				}
				WaitSendBuffer rBuffer = this.mSendBuffer.Dequeue();
				this.KcpSend(rBuffer.Bytes, rBuffer.Index, rBuffer.Length);
			}
		}

		public void HandleRecv(byte[] rDate, uint nTimeNow)
		{
			this.mKcp.Input(rDate);
			// 加入update队列
			this.GetService().AddToUpdate(this.Id);

			while (true)
			{
				int n = mKcp.PeekSize();
				if (n == 0)
				{
					this.OnError((int)SocketError.NetworkReset);
					return;
				}
				int count = this.mKcp.Recv(this.mCacheBytes);
				if (count <= 0)
				{
					return;
				}

				mLastRecvTime = nTimeNow;

				// 收到的数据放入缓冲区
				byte[] rSizeBuffer = BitConverter.GetBytes((ushort)count);
				this.mRecvBuffer.Write(rSizeBuffer, 0, rSizeBuffer.Length);
				this.mRecvBuffer.Write(mCacheBytes, 0, count);

				if (this.mRecvTcs == null)
				{
					continue;
				}

				try
				{
					bool bIsOK = this.mParser.Parse();
					if (!bIsOK)
					{
						continue;
					}

					Packet rPacket = this.mParser.GetPacket();
					var rRecvTcs = this.mRecvTcs;
					this.mRecvTcs = null;
					rRecvTcs.SetResult(rPacket);
				}
				catch (Exception e)
				{
					this.OnError(NetworkErrorCode.ERR_PacketParserError);
						
					var rRecvTcs = this.mRecvTcs;
					this.mRecvTcs = null;
					rRecvTcs.SetException(e);
				}
			}
		}

		public void Output(byte[] rBytes, int nCount)
		{
			this.mSocket.Send(rBytes, nCount, this.mRemoteEndPoint);
		}

		private void KcpSend(byte[] rBuffers, int nIndex, int nLength)
		{
			this.mKcp.Send(rBuffers, nIndex, nLength);
			this.GetService().AddToUpdate(this.Id);
		}

		public override void Send(byte[] rBuffer, int nIndex, int nLength)
		{
			if (this.mIsConnected)
			{
				this.KcpSend(rBuffer, nIndex, nLength);
				return;
			}
			this.mSendBuffer.Enqueue(new WaitSendBuffer(rBuffer, nIndex, nLength));
		}

		public override void Send(List<byte[]> rBuffers)
		{
			ushort nSize = (ushort)rBuffers.Select(b => b.Length).Sum();
			byte[] rBytes;
			if (!this.mIsConnected)
			{
				rBytes = this.mCacheBytes;
			}
			else
			{
				rBytes = new byte[nSize];
			}

			int nIndex = 0;
			foreach (byte[] rBuffer in rBuffers)
			{
				Array.Copy(rBuffer, 0, rBytes, nIndex, rBuffer.Length);
				nIndex += rBuffer.Length;
			}

			Send(rBytes, 0, nSize);
		}

		public override Task<Packet> Recv()
		{
			if (this.IsDisposed)
			{
				throw new Exception("TChannel已经被Dispose, 不能接收消息");
			}

			try
			{
				bool bIsOK = this.mParser.Parse();
				if (bIsOK)
				{
					Packet rPacket = this.mParser.GetPacket();
					return Task.FromResult(rPacket);
				}

				this.mRecvTcs = new TaskCompletionSource<Packet>();
				return this.mRecvTcs.Task;
			}
			catch (Exception)
			{
				this.OnError(NetworkErrorCode.ERR_PacketParserError);
				throw;
			}
		}
	}
}
