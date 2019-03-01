using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Knight.Framework.Net
{
	public sealed class TService: AService
	{
		private TcpListener                         mAcceptor;
		private readonly Dictionary<long, TChannel> mIdChannels = new Dictionary<long, TChannel>();
		
		/// <summary>
		/// 即可做client也可做server
		/// </summary>
		public TService(IPEndPoint rIpEndPoint)
		{
			this.mAcceptor = new TcpListener(rIpEndPoint);
			this.mAcceptor.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			this.mAcceptor.Server.NoDelay = true;
			this.mAcceptor.Start();
		}

		public TService()
		{
		}

		public override void Dispose()
		{
			if (this.mAcceptor == null)
			{
				return;
			}

			foreach (long id in this.mIdChannels.Keys.ToArray())
			{
				TChannel channel = this.mIdChannels[id];
				channel.Dispose();
			}
			this.mAcceptor.Stop();
			this.mAcceptor = null;
		}
		
		public override AChannel GetChannel(long id)
		{
			TChannel rChannel = null;
			this.mIdChannels.TryGetValue(id, out rChannel);
			return rChannel;
		}

		public override async Task<AChannel> AcceptChannel()
		{
			if (this.mAcceptor == null)
			{
				throw new Exception("service construct must use host and port param");
			}
			TcpClient rTcpClient = await this.mAcceptor.AcceptTcpClientAsync();
			TChannel rChannel = new TChannel(rTcpClient, this);
			this.mIdChannels[rChannel.Id] = rChannel;
			return rChannel;
		}

		public override AChannel ConnectChannel(IPEndPoint rIpEndPoint)
		{
			TcpClient tcpClient = new TcpClient();
			TChannel rChannel = new TChannel(tcpClient, rIpEndPoint, this);
			this.mIdChannels[rChannel.Id] = rChannel;

			return rChannel;
		}


		public override void Remove(long id)
		{
			TChannel rChannel;
			if (!this.mIdChannels.TryGetValue(id, out rChannel))
			{
				return;
			}
			if (rChannel == null)
			{
				return;
			}
			this.mIdChannels.Remove(id);
			rChannel.Dispose();
		}

		public override void Update()
		{
		}
	}
}