using Google.Protobuf;
using Knight.Core;
using Knight.Framework.Network;
using Knight.Framework.UI;
using System.Collections.Generic;

namespace Game
{
    public interface IModel
    {
        void Initialize();
        void Destroy();
        void Login();
        void Logout();
    }

    public class ModelBase<T> : IModel where T : ViewModel
    {
        protected List<NetworkMsgProto> mNetworkMsgs;
        protected List<T> mBindViewModels;

        public ModelBase()
        {
        }

        public void Initialize()
        {
            // 初始化网络消息列表 事件绑定的
            this.mNetworkMsgs = new List<NetworkMsgProto>();

            var rType = this.GetType();
            var rMethodInfos = rType.GetMethods(ReflectTool.flags_method_inst);
            for (int i = 0; i < rMethodInfos.Length; i++)
            {
                var rMethodInfo = rMethodInfos[i];

                // 注册网络消息
                var rAttributes = rMethodInfo.GetCustomAttributes(typeof(NetworkMessageHandlerAttribute), false);
                if (rAttributes.Length > 0)
                {
                    var rAttribute = rAttributes[0] as NetworkMessageHandlerAttribute;
                    var rActionDelegate = (NetworkMsgHandleDelegate)rMethodInfo.CreateDelegate(typeof(NetworkMsgHandleDelegate), this);
                    var rMsgProto = NetworkManager.Instance.RegisterMessageHandler(rAttribute.CMD, rAttribute.ACT, rAttribute.ResponseType, rActionDelegate);
                    this.mNetworkMsgs.Add(rMsgProto);
                }
            }

            this.mBindViewModels = new List<T>();

            this.OnInitialize();
        }

        public void Destroy()
        {
            // 注销网络消息
            for (int i = 0; i < this.mNetworkMsgs.Count; i++)
            {
                NetworkManager.Instance.UnregisterMessageHandler(this.mNetworkMsgs[i]);
            }
            this.mNetworkMsgs.Clear();

            this.mBindViewModels.Clear();

            this.OnDestroy();
        }

        public void Login()
        {
            this.OnLogin();
        }

        public void Logout()
        {
            this.OnLogout();
        }

        public void BindViewModel(T rViewModel)
        {
            this.mBindViewModels.Add(rViewModel);
        }

        public void UnbindViewModel(T rViewModel)
        {
            this.mBindViewModels.Remove(rViewModel);
        }

        protected void SyncViewModels(IMessage rMessage)
        {
            foreach (var rViewModel in this.mBindViewModels)
            {
                rViewModel.SyncData(rMessage);
            }
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnLogin()
        {
        }

        protected virtual void OnLogout()
        {
        }
    }
}
