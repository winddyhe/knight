using System;
using System.Net;
using System.Threading.Tasks;

namespace Knight.Framework.Net
{
	public enum NetworkProtocol
	{
		TCP,
		KCP,
	}

	public abstract class AService : IDisposable
	{
		public abstract AChannel        GetChannel(long nID);
		public abstract Task<AChannel>  AcceptChannel();
		public abstract AChannel        ConnectChannel(IPEndPoint rIpEndPoint);
		public abstract void            Remove(long nChannelId);
		public abstract void            Update();
        public abstract void            Dispose();
    }
}