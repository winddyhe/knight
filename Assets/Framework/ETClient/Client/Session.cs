using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Model
{
	public sealed class Session : ASession
	{
        private static uint                         STATIC_ID = 1;
		private static uint                         RpcId            { get; set; }

		private NetworkClient                       mNetwork;
		private Dictionary<uint, Action<object>>    mRequestCallback = new Dictionary<uint, Action<object>>();
		private AChannel                            mChannel;
		private List<byte[]>                        mByteses         = new List<byte[]>() {new byte[0], new byte[0]};
        
        public string                               RemoteAddress    => this.mChannel?.RemoteAddress;
        public ChannelType                          ChannelType      => this.mChannel.ChannelType;

		public Session(NetworkClient rNetwork, AChannel rChannel)
		{
            this.Id = STATIC_ID++;

			this.mNetwork = rNetwork;
			this.mChannel = rChannel;

			this.StartRecv();
		}

		private async void StartRecv()
		{
			while (true)
			{
				if (this.Id == 0)
				{
					return;
				}

				byte[] rMessageBytes;
				try
				{
					rMessageBytes = await mChannel.Recv();
					if (this.Id == 0)
					{
						return;
					}
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
					continue;
				}

				if (rMessageBytes.Length < 3)
				{
					continue;
				}

				ushort nOpcode = BitConverter.ToUInt16(rMessageBytes, 0);
				try
				{
					this.Run(nOpcode, rMessageBytes);
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
			}
		}

		private void Run(ushort nOpcode, byte[] rMessageBytes)
		{
			int nOffset = 0;
			bool bIsCompressed = (nOpcode & 0x8000) > 0;    // opcode最高位表示是否压缩

            if (bIsCompressed) // 最高位为1,表示有压缩,需要解压缩
			{
				rMessageBytes = ZipHelper.Decompress(rMessageBytes, 2, rMessageBytes.Length - 2);
				nOffset = 0;
			}
			else
			{
				nOffset = 2;
			}
			nOpcode &= 0x7fff;
			this.RunDecompressedBytes(nOpcode, rMessageBytes, nOffset);
		}

		private void RunDecompressedBytes(ushort nOpcode, byte[] rMessageBytes, int nOffset)
		{
			Type rMessageType =  NetworkOpcodeType.Instance.GetType(nOpcode);
			object message = this.mNetwork.MessagePacker.DeserializeFrom(rMessageType, rMessageBytes, nOffset, rMessageBytes.Length - nOffset);

			//Log.Debug($"recv: {MongoHelper.ToJson(message)}");

			AResponse response = message as AResponse;
			if (response != null)
			{
				// rpcFlag>0 表示这是一个rpc响应消息
				// Rpc回调有找不着的可能，因为client可能取消Rpc调用
				Action<object> rAction;
				if (!this.mRequestCallback.TryGetValue(response.RpcId, out rAction))
				{
					return;
				}
				this.mRequestCallback.Remove(response.RpcId);
				rAction(message);
				return;
			}
			this.mNetwork.MessageDispatcher.Dispatch(this, nOpcode, nOffset, rMessageBytes, (AMessage)message);
		}

		/// <summary>
		/// Rpc调用
		/// </summary>
		public Task<Response> Call<Response>(ARequest rRequest, CancellationToken rCancellationToken) where Response : AResponse
		{
			rRequest.RpcId = ++RpcId;
			this.SendMessage(rRequest);

			var tcs = new TaskCompletionSource<Response>();
			this.mRequestCallback[RpcId] = (message) =>
			{
				try
				{
					Response response = (Response)message;
					if (response.Error > 100)
					{
						tcs.SetException(new RpcException(response.Error, response.Message));
						return;
					}
					//Log.Debug($"recv: {MongoHelper.ToJson(response)}");
					tcs.SetResult(response);
				}
				catch (Exception e)
				{
					tcs.SetException(new Exception($"Rpc Error: {typeof(Response).FullName}", e));
				}
			};
			rCancellationToken.Register(() => { this.mRequestCallback.Remove(RpcId); });
			return tcs.Task;
		}

		/// <summary>
		/// Rpc调用,发送一个消息,等待返回一个消息
		/// </summary>
		public Task<Response> Call<Response>(ARequest rRequest) where Response : AResponse
		{
			rRequest.RpcId = ++RpcId;
			this.SendMessage(rRequest);

			var tcs = new TaskCompletionSource<Response>();
			this.mRequestCallback[RpcId] = (message) =>
			{
				try
				{
					Response response = (Response)message;
					if (response.Error > 100)
					{
						tcs.SetException(new RpcException(response.Error, response.Message));
						return;
					}
                    Log.Debug($"recv: {this.mNetwork.MessagePacker.SerializeToText(response)}");
					tcs.SetResult(response);
				}
				catch (Exception e)
				{
					tcs.SetException(new Exception($"Rpc Error: {typeof(Response).FullName}", e));
				}
			};
			return tcs.Task;
		}

		public void Send(AMessage rMessage)
		{
			if (this.Id == 0)
			{
				throw new Exception("session已经被Dispose了");
			}
			this.SendMessage(rMessage);
		}

		public void Reply<Response>(Response rMessage) where Response : AResponse
		{
			if (this.Id == 0)
			{
				throw new Exception("session已经被Dispose了");
			}
			this.SendMessage(rMessage);
		}

		private void SendMessage(object rMessage)
		{
			//Log.Debug($"send: {MongoHelper.ToJson(message)}");
			ushort nOpcode = NetworkOpcodeType.Instance.GetOpcode(rMessage.GetType());
			byte[] rMessageBytes = this.mNetwork.MessagePacker.SerializeToByteArray(rMessage);
			if (rMessageBytes.Length > 100)
			{
				byte[] rNewMessageBytes = ZipHelper.Compress(rMessageBytes);
				if (rNewMessageBytes.Length < rMessageBytes.Length)
				{
					rMessageBytes = rNewMessageBytes;
                    nOpcode |= 0x8000;
				}
			}

			byte[] rOpcodeBytes = BitConverter.GetBytes(nOpcode);
			
			this.mByteses[0] = rOpcodeBytes;
			this.mByteses[1] = rMessageBytes;

			mChannel.Send(this.mByteses);
		}

		public void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}

			long nId = this.Id;
            
			this.mChannel.Dispose();
			this.mNetwork.Remove(nId);
		}
	}
}