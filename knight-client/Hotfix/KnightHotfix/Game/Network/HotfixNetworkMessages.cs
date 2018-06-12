using Knight.Framework.Net;
using Knight.Hotfix.Core;
using System.Collections.Generic;

namespace Game
{
    [Message(HotfixNetworkMessageOpcode.C2R_Login)]
    [HotfixSBGroup("Network")]
    public partial class C2R_Login : HotfixSerializerBinary, IHotfixRequest
    {
        public int      RpcId       { get; set; }
        public string   Account;
        public string   Password;
    }

    [Message(HotfixNetworkMessageOpcode.R2C_Login)]
    [HotfixSBGroup("Network")]
    public partial class R2C_Login : HotfixSerializerBinary, IHotfixResponse
    {
        public int      RpcId       { get; set; }
        public int      Error       { get; set; }
        public string   Message     { get; set; }
        public string   Address;
        public long     Key;
    }

    [Message(HotfixNetworkMessageOpcode.C2G_LoginGate)]
    [HotfixSBGroup("Network")]
    public partial class C2G_LoginGate : HotfixSerializerBinary, IHotfixRequest
    {
        public int      RpcId       { get; set; }
        public long     Key;
    }

    [Message(HotfixNetworkMessageOpcode.G2C_LoginGate)]
    [HotfixSBGroup("Network")]
    public partial class G2C_LoginGate : HotfixSerializerBinary, IHotfixResponse
    {
        public int      RpcId       { get; set; }
        public int      Error       { get; set; }
        public string   Message     { get; set; }
        public long     PlayerId;
    }

    [Message(HotfixNetworkMessageOpcode.G2C_Test1)]
    [HotfixSBGroup("Network")]
    public partial class G2C_TestHotfixMessage : HotfixSerializerBinary, IHotfixMessage
    {
        public string   Info;
    }
}
