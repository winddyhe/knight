using Knight.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.Net
{
	public static class KcpProtocalType
	{
		public const uint SYN = 1;
		public const uint ACK = 2;
		public const uint FIN = 3;
	}

	public sealed class KService : AService
	{
		private uint                                mIdGenerater = 1000;

		public  uint                                mTimeNow { get; set; }

		private UdpClient                           mSocket;         
		private readonly Dictionary<long, KChannel> mIdChannels      = new Dictionary<long, KChannel>();
		private TaskCompletionSource<AChannel>      mAcceptTcs;      
		private readonly Queue<long>                mRemovedChannels = new Queue<long>();
		// 下帧要更新的channel                                          
		private readonly HashSet<long>              mUpdateChannels  = new HashSet<long>();
		// 下次时间更新的channel                                        
		private readonly MultiMap<long, long>       mTimerId         = new MultiMap<long, long>();
		private readonly List<long>                 mTimeOutId       = new List<long>();

		public KService(IPEndPoint rIpEndPoint)
		{
			this.mTimeNow = (uint)TimeAssist.Now();
			this.mSocket = new UdpClient(rIpEndPoint);

			this.StartRecv();
		}

		public KService()
		{
			this.mTimeNow = (uint)TimeAssist.Now();
			this.mSocket = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
			this.StartRecv();
		}

		public override void Dispose()
		{
			if (this.mSocket == null)
			{
				return;
			}

			this.mSocket.Close();
			this.mSocket = null;
		}

		public async void StartRecv()
		{
			while (true)
			{
				if (this.mSocket == null)
				{
					return;
				}

				UdpReceiveResult rUdpReceiveResult;
				try
				{
					rUdpReceiveResult = await this.mSocket.ReceiveAsync();
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					continue;
				}

				try
				{
					int nMessageLength = rUdpReceiveResult.Buffer.Length;

					// 长度小于4，不是正常的消息
					if (nMessageLength < 4)
					{
						continue;
					}

					// accept
					uint nConn = BitConverter.ToUInt32(rUdpReceiveResult.Buffer, 0);

					// conn从1000开始，如果为1，2，3则是特殊包
					switch (nConn)
					{
						case KcpProtocalType.SYN:
							// 长度!=8，不是accpet消息
							if (nMessageLength != 8)
							{
								break;
							}
							this.HandleAccept(rUdpReceiveResult);
							break;
						case KcpProtocalType.ACK:
							// 长度!=12，不是connect消息
							if (nMessageLength != 12)
							{
								break;
							}
							this.HandleConnect(rUdpReceiveResult);
							break;
						case KcpProtocalType.FIN:
							// 长度!=12，不是DisConnect消息
							if (nMessageLength != 12)
							{
								break;
							}
							this.HandleDisConnect(rUdpReceiveResult);
							break;
						default:
							this.HandleRecv(rUdpReceiveResult, nConn);
							break;
					}
				}
				catch (Exception e)
				{
                    Debug.LogError(e);
					continue;
				}
			}
		}

		private void HandleConnect(UdpReceiveResult rUdpReceiveResult)
		{
			uint nRequestConn = BitConverter.ToUInt32(rUdpReceiveResult.Buffer, 4);
			uint nResponseConn = BitConverter.ToUInt32(rUdpReceiveResult.Buffer, 8);

			KChannel rKChannel;
			if (!this.mIdChannels.TryGetValue(nRequestConn, out rKChannel))
			{
				return;
			}
			// 处理chanel
			rKChannel.HandleConnnect(nResponseConn);
		}

		private void HandleDisConnect(UdpReceiveResult rUdpReceiveResult)
		{
			uint nRequestConn = BitConverter.ToUInt32(rUdpReceiveResult.Buffer, 8);

			KChannel rKChannel;
			if (!this.mIdChannels.TryGetValue(nRequestConn, out rKChannel))
			{
				return;
			}
			// 处理chanel
			this.mIdChannels.Remove(nRequestConn);
			rKChannel.Dispose();
		}

		private void HandleRecv(UdpReceiveResult rUdpReceiveResult, uint nConn)
		{
			KChannel rKChannel;
			if (!this.mIdChannels.TryGetValue(nConn, out rKChannel))
			{
				return;
			}
			// 处理chanel
			rKChannel.HandleRecv(rUdpReceiveResult.Buffer, this.mTimeNow);
		}

		private void HandleAccept(UdpReceiveResult rUdpReceiveResult)
		{
			if (this.mAcceptTcs == null)
			{
				return;
			}

			uint nRequestConn = BitConverter.ToUInt32(rUdpReceiveResult.Buffer, 4);

			// 如果已经连接上,则重新响应请求
			KChannel rKChannel;
			if (this.mIdChannels.TryGetValue(nRequestConn, out rKChannel))
			{
				rKChannel.HandleAccept(nRequestConn);
				return;
			}

			TaskCompletionSource<AChannel> t = this.mAcceptTcs;
			this.mAcceptTcs = null;
			rKChannel = this.CreateAcceptChannel(rUdpReceiveResult.RemoteEndPoint, nRequestConn);
			rKChannel.HandleAccept(nRequestConn);
			t.SetResult(rKChannel);
		}

		private KChannel CreateAcceptChannel(IPEndPoint rRemoteEndPoint, uint nRemoteConn)
		{
			KChannel rChannel = new KChannel(++this.mIdGenerater, nRemoteConn, this.mSocket, rRemoteEndPoint, this);
			KChannel rOldChannel;
			if (this.mIdChannels.TryGetValue(rChannel.Id, out rOldChannel))
			{
				this.mIdChannels.Remove(rOldChannel.Id);
				rOldChannel.Dispose();
			}
			this.mIdChannels[rChannel.Id] = rChannel;
			return rChannel;
		}

		private KChannel CreateConnectChannel(IPEndPoint rRemoteEndPoint)
		{
			uint nConv = (uint)UnityEngine.Random.Range(1000, int.MaxValue);
			KChannel rChannel = new KChannel(nConv, this.mSocket, rRemoteEndPoint, this);
			KChannel rOldChannel;
			if (this.mIdChannels.TryGetValue(rChannel.Id, out rOldChannel))
			{
				this.mIdChannels.Remove(rOldChannel.Id);
				rOldChannel.Dispose();
			}
			this.mIdChannels[rChannel.Id] = rChannel;
			return rChannel;
		}

		public void AddToUpdate(long nID)
		{
			this.mUpdateChannels.Add(nID);
		}

		public void AddToNextTimeUpdate(long nTime, long nID)
		{
			this.mTimerId.Add(nTime, nID);
		}

		public override AChannel GetChannel(long nID)
		{
			KChannel rChannel;
			this.mIdChannels.TryGetValue(nID, out rChannel);
			return rChannel;
		}

		public override Task<AChannel> AcceptChannel()
		{
			mAcceptTcs = new TaskCompletionSource<AChannel>();
			return this.mAcceptTcs.Task;
		}

		public override AChannel ConnectChannel(IPEndPoint rIpEndPoint)
		{
			KChannel rChannel = this.CreateConnectChannel(rIpEndPoint);
			return rChannel;
		}


		public override void Remove(long nID)
		{
			KChannel rChannel;
			if (!this.mIdChannels.TryGetValue(nID, out rChannel))
			{
				return;
			}
			if (rChannel == null)
			{
				return;
			}
			this.mRemovedChannels.Enqueue(nID);
			rChannel.Dispose();
		}

		public override void Update()
		{
			this.TimerOut();

			foreach (long id in mUpdateChannels)
			{
				KChannel kChannel;
				if (!this.mIdChannels.TryGetValue(id, out kChannel))
				{
					continue;
				}
				if (kChannel.Id == 0)
				{
					continue;
				}
				kChannel.Update(this.mTimeNow);
			}
			this.mUpdateChannels.Clear();

			while (true)
			{
				if (this.mRemovedChannels.Count <= 0)
				{
					break;
				}
				long id = this.mRemovedChannels.Dequeue();
				this.mIdChannels.Remove(id);
			}
		}

		// 计算到期需要update的channel
		private void TimerOut()
		{
			if (this.mTimerId.Count == 0)
			{
				return;
			}

			this.mTimeNow = (uint)TimeAssist.ClientNow();

			mTimeOutId.Clear();

			while (this.mTimerId.Count > 0)
			{
				long k = this.mTimerId.FirstKey();
				if (k > this.mTimeNow)
				{
					break;
				}
				foreach (long ll in this.mTimerId[k])
				{
					this.mTimeOutId.Add(ll);
				}
				this.mTimerId.Remove(k);
			}

			foreach (long k in this.mTimeOutId)
			{
				this.mUpdateChannels.Add(k);
			}
		}
	}
}