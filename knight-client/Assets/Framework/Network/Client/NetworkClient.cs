using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Knight.Core;

namespace Knight.Framework.Net
{
    public class NetworkClient : TSingleton<NetworkClient>
    {
        private AService                                    mService;
        private readonly IndexedDict<long, NetworkSession>  mSessions           = new IndexedDict<long, NetworkSession>();
        private bool                                        mIsDisposed         = false;

        public IMessagePacker                               MessagePacker       { get; set; }
        public IMessageDispatcher                           MessageDispatcher   { get; set; }
        public NetworkOpcodeTypes                           OpcodeTypes         { get; set; }

        private NetworkClient()
        {
        }

        public void Initialize(NetworkProtocol rProtocol)
        {
            this.OpcodeTypes = new NetworkOpcodeTypes();
            this.OpcodeTypes.Initialize();

            this.MessageDispatcher = new NetworkClientDispatcher();
            this.MessagePacker = new NetworkMessagePacker();

            switch (rProtocol)
            {
                case NetworkProtocol.TCP:
                    this.mService = new TService();
                    break;
                case NetworkProtocol.KCP:
                    this.mService = new KService();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Initialize(NetworkProtocol rProtocol, IPEndPoint rIpEndPoint)
        {
            try
            {
                switch (rProtocol)
                {
                    case NetworkProtocol.TCP:
                        this.mService = new TService(rIpEndPoint);
                        break;
                    case NetworkProtocol.KCP:
                        this.mService = new KService(rIpEndPoint);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                this.StartAccept();
            }
            catch (Exception e)
            {
                throw new Exception($"{rIpEndPoint}", e);
            }
        }

        private async void StartAccept()
        {
            while (true)
            {
                if (this.mIsDisposed)
                {
                    return;
                }

                try
                {
                    await this.Accept();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        public virtual async Task<NetworkSession> Accept()
        {
            AChannel rChannel = await this.mService.AcceptChannel();
            NetworkSession rSession = new NetworkSession(this, rChannel);
            rSession.Parent = this;
            rChannel.ErrorCallback += (c, e) =>
            {
                rSession.Error = e;
                this.Remove(rSession.Id);
            };
            this.mSessions.Add(rSession.Id, rSession);
            return rSession;
        }

        public virtual void Remove(long nSessionID)
        {
            NetworkSession rSession;
            if (!this.mSessions.TryGetValue(nSessionID, out rSession))
            {
                return;
            }
            this.mSessions.Remove(nSessionID);
            rSession.Dispose();
        }

        public NetworkSession Get(long nSessionID)
        {
            NetworkSession rSession;
            this.mSessions.TryGetValue(nSessionID, out rSession);
            return rSession;
        }

        /// <summary>
        /// 创建一个新Session
        /// </summary>
        public virtual NetworkSession Create(IPEndPoint rIpEndPoint)
        {
            try
            {
                AChannel rChannel = this.mService.ConnectChannel(rIpEndPoint);
                NetworkSession rSession = new NetworkSession(this, rChannel);
                rSession.Parent = this;
                rChannel.ErrorCallback += (c, e) =>
                {
                    rSession.Error = e;
                    this.Remove(rSession.Id);
                };
                this.mSessions.Add(rSession.Id, rSession);
                return rSession;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
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
            if (this.mIsDisposed)
            {
                return;
            }
            this.mIsDisposed = true;
            
            for (int i = 0;i < this.mSessions.Keys.Count; i++)
            {
                this.mSessions[this.mSessions.Keys[i]].Dispose();
            }
            this.mService.Dispose();
        }
    }
}