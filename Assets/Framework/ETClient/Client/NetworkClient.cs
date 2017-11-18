using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;

namespace Model
{
	public class NetworkClient : TSingleton<NetworkClient>
	{
		private AService                            mService;
		private Dictionary<long, Session>    mSessions               = new Dictionary<long, Session>();

		public IMessagePacker                       MessagePacker           { get; set; }
		public IMessageDispatcher                   MessageDispatcher       { get; set; }

        private NetworkClient()
        {
        }

		public void Initialize(NetworkProtocol rProtocol)
        {
            this.MessagePacker = new SerializerPacker();
            this.MessageDispatcher = new ClientDispatcher();

            switch (rProtocol)
			{
				case NetworkProtocol.TCP:
					this.mService = new TService();
					break;
				case NetworkProtocol.UDP:
					this.mService = new UService();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Initialize(NetworkProtocol rProtocol, string rHost, int nPort)
		{
			try
			{
				switch (rProtocol)
				{
					case NetworkProtocol.TCP:
						this.mService = new TService(rHost, nPort);
						break;
					case NetworkProtocol.UDP:
						this.mService = new UService(rHost, nPort);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				this.StartAccept();
			}
			catch (Exception e)
			{
				throw new Exception($"{rHost} {nPort}", e);
			}
		}

		private async void StartAccept()
		{
			while (true)
			{
				await this.Accept();
			}
		}

		public virtual async Task<Session> Accept()
		{
			AChannel rChannel = await this.mService.AcceptChannel();
			Session rSession = new Session(this, rChannel);
			rChannel.ErrorCallback += (c, e) => { this.Remove(rSession.Id); };
			this.mSessions.Add(rSession.Id, rSession);
			return rSession;
		}

		public virtual void Remove(long nId)
		{
			Session rSession;
			if (!this.mSessions.TryGetValue(nId, out rSession))
			{
				return;
			}
			this.mSessions.Remove(nId);
			rSession.Dispose();
		}

		public Session Get(long nId)
		{
			Session rSession;
			this.mSessions.TryGetValue(nId, out rSession);
			return rSession;
		}

		/// <summary>
		/// 创建一个新Session
		/// </summary>
		public virtual Session Create(string rAddress)
		{
			try
			{
				string[] ss = rAddress.Split(':');
				int nPort = int.Parse(ss[1]);
				string rHost = ss[0];
				AChannel rChannel = this.mService.ConnectChannel(rHost, nPort);
				Session rSession = new Session(this, rChannel);
				rChannel.ErrorCallback += (c, e) => { this.Remove(rSession.Id); };
				this.mSessions.Add(rSession.Id, rSession);
				return rSession;
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
				return null;
			}
		}

		public void Update()
		{
			if (this.mService == null)
			{
				return;
			}
			this.mService.Update();
		}

		public void Dispose()
		{
			foreach (Session rSession in this.mSessions.Values.ToArray())
			{
				rSession.Dispose();
			}
			this.mService.Dispose();
		}
	}
}