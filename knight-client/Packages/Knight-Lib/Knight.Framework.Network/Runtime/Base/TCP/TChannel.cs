using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using Knight.Core;

namespace Knight.Framework.Net
{
	public class TChannel : AChannel
	{
		private readonly TcpClient              mTcpClient;

		private readonly CircularBuffer         mRecvBuffer = new CircularBuffer();
		private readonly CircularBuffer         mSendBuffer = new CircularBuffer();

		private bool                            mIsSending;
		private readonly PacketParser           mParser;
		private bool                            mIsConnected;
		private TaskCompletionSource<Packet>    mRecvTcs;

		/// <summary>
		/// connect
		/// </summary>
		public TChannel(TcpClient rTcpClient, IPEndPoint rIpEndPoint, TService rService) : base(rService, ChannelType.Connect)
		{
			this.mTcpClient = rTcpClient;
			this.mParser = new PacketParser(this.mRecvBuffer);
			this.RemoteAddress = rIpEndPoint;

			this.ConnectAsync(rIpEndPoint);
		}

		/// <summary>
		/// accept
		/// </summary>
		public TChannel(TcpClient rTcpClient, TService rService) : base(rService, ChannelType.Accept)
		{
			this.mTcpClient = rTcpClient;
			this.mParser = new PacketParser(this.mRecvBuffer);

			IPEndPoint ipEndPoint = (IPEndPoint)this.mTcpClient.Client.RemoteEndPoint;
			this.RemoteAddress = ipEndPoint;
			this.OnAccepted();
		}

		private async void ConnectAsync(IPEndPoint rIpEndPoint)
		{
			try
			{
				await this.mTcpClient.ConnectAsync(rIpEndPoint.Address, rIpEndPoint.Port);
				
				this.mIsConnected = true;
				this.StartSend();
				this.StartRecv();
			}
			catch (SocketException e)
			{
				Debug.LogError($"connect error: {e.SocketErrorCode}");
				this.OnError((int)e.SocketErrorCode);
			}
			catch (Exception e)
			{
                Debug.LogError($"connect error: {rIpEndPoint} {e}");
				this.OnError((int)SocketError.SocketError);
			}
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			
			base.Dispose();

			this.mRecvTcs = null;
			this.mTcpClient.Close();
		}

		private void OnAccepted()
		{
			this.mIsConnected = true;
			this.StartSend();
			this.StartRecv();
		}

		public override void Send(byte[] rBuffer, int nIndex, int nLength)
		{
			if (this.IsDisposed)
			{
				throw new Exception("TChannel已经被Dispose, 不能发送消息");
			}
			byte[] nSize = BitConverter.GetBytes((ushort)rBuffer.Length);
			this.mSendBuffer.Write(nSize, 0, nSize.Length);
			this.mSendBuffer.Write(rBuffer, nIndex, nLength);
			if (this.mIsConnected)
			{
				this.StartSend();
			}
		}

		public override void Send(List<byte[]> rBuffers)
		{
			if (this.IsDisposed)
			{
				throw new Exception("TChannel已经被Dispose, 不能发送消息");
			}
			ushort nSize = (ushort)rBuffers.Select(b => b.Length).Sum();
			byte[] rSizeBuffer = BitConverter.GetBytes(nSize);
			this.mSendBuffer.Write(rSizeBuffer, 0, rSizeBuffer.Length);
			foreach (byte[] rBuffer in rBuffers)
			{
				this.mSendBuffer.Write(rBuffer, 0, rBuffer.Length);
			}
			if (this.mIsConnected)
			{
				this.StartSend();
			}
		}

		private async void StartSend()
		{
			try
			{
				// 如果正在发送中,不需要再次发送
				if (this.mIsSending)
				{
					return;
				}

				while (true)
				{
					// 没有数据需要发送
					long nBuffLength = this.mSendBuffer.Length;
					if (nBuffLength == 0)
					{
						this.mIsSending = false;
						return;
					}

					this.mIsSending = true;
					
					NetworkStream rStream = this.mTcpClient.GetStream();
					if (!rStream.CanWrite)
					{
						return;
					}

					await this.mSendBuffer.WriteToAsync(rStream);
				}
			}
			catch (IOException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (ObjectDisposedException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (Exception e)
			{
                Debug.LogError(e);
				this.OnError((int)SocketError.SocketError);
			}
		}

		private async void StartRecv()
		{
			try
			{
				while (true)
				{
					NetworkStream rStream = this.mTcpClient.GetStream();
					if (!rStream.CanRead)
					{
						return;
					}

					int n = await this.mRecvBuffer.ReadFromAsync(rStream);

					if (n == 0)
					{
						this.OnError((int)SocketError.NetworkReset);
						return;
					}

					// 如果没有recv调用
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

						var tcs = this.mRecvTcs;
						this.mRecvTcs = null;
						tcs.SetResult(rPacket);
					}
					catch (Exception e)
					{
						this.OnError(NetworkErrorCode.ERR_PacketParserError);
						
						var tcs = this.mRecvTcs;
						this.mRecvTcs = null;
						tcs.SetException(e);
					}
				}
			}
			catch (IOException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (ObjectDisposedException)
			{
				this.OnError((int)SocketError.SocketError);
			}
			catch (Exception e)
			{
                Debug.LogError(e);
				this.OnError((int)SocketError.SocketError);
			}
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
					Packet packet = this.mParser.GetPacket();
					return Task.FromResult(packet);
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