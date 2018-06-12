using Knight.Framework.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Hotfix.Core
{
    public class HotfixNetworkClientDispatcher
    {
        public void Dispatch(NetworkSession rSession, Packet rPacket)
        {
            // @TODO: 非rpc消息的Dsipacth处理
            HotfixNetworkClient.Instance.Run(rSession, rPacket);
        }
    }
}
