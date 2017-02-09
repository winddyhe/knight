using System;
using System.Text;
using Core.WindJson;
using System.Net;
using System.Net.Sockets;

namespace Pomelo.DotNetClient
{
    public class HandShakeService
    {
        private Protocol protocol;
        private Action<JsonNode> callback;

        public const string Version = "0.3.0";
        public const string Type = "unity-socket";


        public HandShakeService(Protocol protocol)
        {
            this.protocol = protocol;
        }

        public void request(JsonNode user, Action<JsonNode> callback)
        {
            byte[] body = Encoding.UTF8.GetBytes(buildMsg(user).ToString());

            protocol.send(PackageType.PKG_HANDSHAKE, body);

            this.callback = callback;
        }

        internal void invokeCallback(JsonNode data)
        {
            //Invoke the handshake callback
            if (callback != null) callback.Invoke(data);
        }

        public void ack()
        {
            protocol.send(PackageType.PKG_HANDSHAKE_ACK, new byte[0]);
        }

        private JsonNode buildMsg(JsonNode user)
        {
            if (user == null) user = new JsonClass();

            JsonNode msg = new JsonClass();

            //Build sys option
            JsonNode sys = new JsonClass();
            sys.Add("version", new JsonData(Version));
            sys.Add("type", new JsonData(Type));

            //Build handshake message
            msg.Add("sys", sys);
            msg.Add("user", user);

            UnityEngine.Debug.Log(msg);

            return msg;
        }
    }
}