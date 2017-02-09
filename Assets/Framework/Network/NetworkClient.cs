//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Pomelo.DotNetClient;
using System;
using Core.WindJson;
using Core;
using System.Collections.Generic;

namespace Framework
{
    public class NetworkClient : MonoBehaviour
    {
        public class NetRequest
        {
            public string                   GUID;
            public string                   Route;
            public JsonNode                 Message;
            public JsonNode                 ResultMessage;
            public bool                     IsReceived;

            public Action<JsonNode, JsonNode> OnResponseEvent;
        }

        public class NetNotify
        {
            public string                   GUID;
            public string                   Route;
            public Action<JsonNode>         OnNotifyEvent;
            public JsonNode                 ResultMessage;
            public bool                     IsReceived;
        }

        private static NetworkClient        __instance;
        public  static NetworkClient        Instance { get { return __instance; } }

        private PomeloClient                mClient;
        
        private Dict<string, NetRequest>    mNetRequestDict;
        private List<string>                mRemoveRequestList;

        private Dict<string, NetNotify>     mNetNotifyDict;
        
        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
                mNetRequestDict = new Dict<string, NetRequest>();
                mRemoveRequestList = new List<string>();
                mNetNotifyDict = new Dict<string, NetNotify>();
            }
        }

        void Update()
        {
            if (mRemoveRequestList == null || mNetRequestDict == null) return;
            if (mNetNotifyDict == null) return;

            mRemoveRequestList.Clear();
            foreach (var rPair in mNetRequestDict)
            {
                if (!rPair.Value.IsReceived) continue;

                try {
                    UtilTool.SafeExecute(rPair.Value.OnResponseEvent, rPair.Value.Message, rPair.Value.ResultMessage);
                    mRemoveRequestList.Add(rPair.Key);
                }
                catch (Exception e) {
                    Debug.LogError(e.Message + e.StackTrace);
                    mRemoveRequestList.Add(rPair.Key);
                    rPair.Value.IsReceived = true;
                }
            }
            // 删除不用的网络消息
            for (int i = 0; i < mRemoveRequestList.Count; i++)
            {
                mNetRequestDict.Remove(mRemoveRequestList[i]);
            }

            // 处理服务器自己通知过来的消息
            foreach (var rPair in mNetNotifyDict)
            {
                if (!rPair.Value.IsReceived) continue;

                try {
                    UtilTool.SafeExecute(rPair.Value.OnNotifyEvent, rPair.Value.ResultMessage);
                }
                catch (Exception e) {
                    Debug.LogError(e.StackTrace);
                }
                finally {
                    rPair.Value.IsReceived = false;
                }
            }
        }

        public void Connect(string rHost, int rPort, Action OnConnectCompleted = null)
        {
            mClient = new PomeloClient();
            mClient.initClient(rHost, rPort, ()=> 
            {
                Debug.Log("Pomelo client init success!");

                bool rResult = this.ConnectPomeloServer(OnConnectCompleted);

                if (rResult)
                    Debug.Log("Connect success!");
                else
                    Debug.Log("Connect failed!");
            });
        }

        public void Disconnect()
        {
            if (mClient != null)
            {
                mClient.disconnect();
                mClient = null;
                Debug.Log("Disconnected!");
            }
        }

        private bool ConnectPomeloServer(Action OnConnectCompleted)
        {
            return mClient.connect(null, (rProtocolMsg)=> {
                Debug.Log("Connection Established: " + rProtocolMsg.ToString());
                UtilTool.SafeExecute(OnConnectCompleted);
            });
        }
        
        /// <summary>
        /// 客户端的请求
        /// </summary>
        public string ClientRequest(string rRoute, JsonNode rMsg, Action<JsonNode, JsonNode> rOnResponse)
        {
            string rRequestGUID = Guid.NewGuid().ToString();
            var rNetRequest = new NetRequest()
            {
                GUID = rRequestGUID,
                Route = rRoute,
                Message = rMsg,
                OnResponseEvent = rOnResponse,
                IsReceived = false
            };
            this.mNetRequestDict.Add(rRequestGUID, rNetRequest);
            this.mClient.request(rRoute, rMsg, (rResultMsg) =>
            {
                Debug.Log("NetworkMsg -- " + rRoute + ": " + rResultMsg);
                rNetRequest.ResultMessage = rResultMsg;
                rNetRequest.IsReceived = true;
            });
            return rRequestGUID;
        }

        /// <summary>
        /// 服务器的通知
        /// </summary>
        public string OnServerNotify(string rRoute, Action<JsonNode> rOnServerNotify)
        {
            string rRequestGUID = Guid.NewGuid().ToString();
            var rNetRequest = new NetNotify()
            {
                GUID = rRequestGUID,
                Route = rRoute,
                OnNotifyEvent = rOnServerNotify,
                IsReceived = false
            };
            mNetNotifyDict.Add(rRequestGUID, rNetRequest);

            this.mClient.on(rRoute, (rResultMsg)=> {
                Debug.Log("NetworkMsg -- " + rRoute + ": " + rResultMsg);
                rNetRequest.ResultMessage = rResultMsg;
                rNetRequest.IsReceived = true;
            });
            return rRequestGUID;
        }
        
        public NetworkMsgCode GetMessageCode(JsonNode rProtocolMsg)
        {
            NetworkMsgCode rCode = NetworkMsgCode.Unknown;
            rCode = (NetworkMsgCode)rProtocolMsg["code"].AsInt;
            return rCode;
        }
    }
}