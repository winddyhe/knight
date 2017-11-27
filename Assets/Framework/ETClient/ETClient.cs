using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Framework.Hotfix;
using Model;

namespace Framework.Network
{
	public class ETClient : TSingleton<ETClient>
	{
		private AService            mService;
		private Dict<long, Session> mSessions;
        
        public  HotfixObject        MessageDispatcher;
                
        private ETClient()
        {
        }

		public void Initialize(NetworkProtocol rProtocol)
        {
            this.mSessions = new Dict<long, Session>();

            // 热更新端的初始化
            OpcodeTypes.Instance.Initialize();
            HotfixManager.Instance.InvokeStatic("WindHotfix.Net.NetworkClient", "Initialize");
            this.MessageDispatcher = HotfixManager.Instance.Instantiate("WindHotfix.Net.MessageDispatcher");

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
			foreach (var rPair in this.mSessions)
			{
				rPair.Value.Dispose();
			}
			this.mService.Dispose();
		}
	}
}