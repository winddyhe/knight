using System;
using System.Collections.Generic;
using Knight.Core.Serializer;

namespace Knight.Framework.Net
{
    [Message(NetworkMessageOpcode.C2R_Login)]
    [SBGroup("Network")]
    public partial class C2R_Login : SerializerBinary, IRequest
    {
        public int      RpcId       { get; set; }
        public string   Account;
        public string   Password;
    }

    [Message(NetworkMessageOpcode.R2C_Login)]
    [SBGroup("Network")]
    public partial class R2C_Login : SerializerBinary, IResponse
    {
        public int      RpcId       { get; set; }
        public int      Error       { get; set; }
        public string   Message     { get; set; }
        public string   Address;
        public long     Key;
    }

    [Message(NetworkMessageOpcode.C2G_LoginGate)]
    [SBGroup("Network")]
    public partial class C2G_LoginGate : SerializerBinary, IRequest
    {
        public int      RpcId       { get; set; }
        public long     Key;
    }

    [Message(NetworkMessageOpcode.G2C_LoginGate)]
    [SBGroup("Network")]
    public partial class G2C_LoginGate : SerializerBinary, IResponse
    {
        public int      RpcId       { get; set; }
        public int      Error       { get; set; }
        public string   Message     { get; set; }
        public long     PlayerId;
    }

    [Message(NetworkMessageOpcode.G2C_Test1)]
    [SBGroup("Network")]
    public partial class G2C_TestHotfixMessage : SerializerBinary, IMessage
    {
        public string   Info;
    }
}
