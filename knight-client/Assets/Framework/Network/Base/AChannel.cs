using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Knight.Framework.Net
{
	[Flags]
	public enum PacketFlags
	{
		None        = 0,
		Reliable    = 1 << 0,
		Unsequenced = 1 << 1,
		NoAllocate  = 1 << 2
	}

	public enum ChannelType
	{
		Connect,
		Accept,
	}

	public abstract class AChannel: IDisposable
	{
        public    uint                      Id;
        public    bool                      IsDisposed;
		public    ChannelType               ChannelType     { get; }
		public    IPEndPoint                RemoteAddress   { get; protected set; }
                  
        protected AService                  mService;
		private   Action<AChannel, int>     mErrorCallback;
        
		public event Action<AChannel, int>  ErrorCallback
		{
			add
			{
				this.mErrorCallback += value;
			}
			remove
			{
				this.mErrorCallback -= value;
			}
		}

		protected void OnError(int nError)
		{
			if (this.IsDisposed)
			{
				return;
			}
			this.mErrorCallback?.Invoke(this, nError);
		}


		protected AChannel(AService rService, ChannelType rChannelType)
		{
			this.ChannelType = rChannelType;
			this.mService = rService;
		}
		
		/// <summary>
		/// 发送消息
		/// </summary>
		public abstract void Send(byte[] rBuffer, int nIndex, int nLength);

		public abstract void Send(List<byte[]> buffers);

		/// <summary>
		/// 接收消息
		/// </summary>
		public abstract Task<Packet> Recv();

		public virtual void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
			this.mService.Remove(this.Id);
		}
	}
}