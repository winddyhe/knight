using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindHotfix.Core;
using WindHotfix.Net;

namespace Game.Knight
{
    [Message((ushort)NetworkOpcode.C2R_Login)]
    public partial class C2R_Login : ARequest
    {
        public string Account;
        public string Password;
    }

    [Message((ushort)NetworkOpcode.R2C_Login)]
    public partial class R2C_Login : AResponse
    {
        public string Address { get; set; }
        public long Key { get; set; }
    }

    [Message((ushort)NetworkOpcode.C2G_LoginGate)]
    public partial class C2G_LoginGate : ARequest
    {
        public long Key;
    }

    [Message((ushort)NetworkOpcode.G2C_LoginGate)]
    public partial class G2C_LoginGate : AResponse
    {
        public long PlayerId;
    }


    [Message((ushort)NetworkOpcode.Actor_Test)]
    public partial class Actor_Test : AActorMessage
    {
        public string Info;
    }

    [Message((ushort)NetworkOpcode.Actor_TestRequest)]
    public partial class Actor_TestRequest : AActorRequest
    {
        public string request;
    }

    [Message((ushort)NetworkOpcode.Actor_TestResponse)]
    public partial class Actor_TestResponse : AActorResponse
    {
        public string response;
    }


    [Message((ushort)NetworkOpcode.Actor_TransferRequest)]
    public partial class Actor_TransferRequest : AActorRequest
    {
        public int MapIndex;
    }

    [Message((ushort)NetworkOpcode.Actor_TransferResponse)]
    public partial class Actor_TransferResponse : AActorResponse
    {
    }

    [Message((ushort)NetworkOpcode.C2G_EnterMap)]
    public partial class C2G_EnterMap : ARequest
    {
    }

    [Message((ushort)NetworkOpcode.G2C_EnterMap)]
    public partial class G2C_EnterMap : AResponse
    {
        public long UnitId;
        public int Count;
    }

    [HotfixSBGroupInerited("Protocol")]
    public partial class UnitInfo : HotfixSerializerBinary
    {
        public long UnitId;
        public int X;
        public int Z;
    }

    [Message((ushort)NetworkOpcode.Actor_CreateUnits)]
    public partial class Actor_CreateUnits : AActorMessage
    {
        public List<UnitInfo> Units = new List<UnitInfo>();
    }

    public partial struct FrameMessageInfo
    {
        public long Id;
        public AMessage Message;
    }

    [Message((ushort)NetworkOpcode.FrameMessage)]
    public partial class FrameMessage : AActorMessage
    {
        public int Frame;
        public List<AFrameMessage> Messages = new List<AFrameMessage>();
    }

    [Message((ushort)NetworkOpcode.Frame_ClickMap)]
    public partial class Frame_ClickMap : AFrameMessage
    {
        public int X;
        public int Z;
    }

    [Message((ushort)NetworkOpcode.C2M_Reload)]
    public partial class C2M_Reload : ARequest
    {
        public Model.AppType AppType;
    }

    [Message((ushort)NetworkOpcode.M2C_Reload)]
    public partial class M2C_Reload : AResponse
    {
    }

    [Message((ushort)NetworkOpcode.C2R_Ping)]
    public partial class C2R_Ping : ARequest
    {
    }

    [Message((ushort)NetworkOpcode.R2C_Ping)]
    public partial class R2C_Ping : AResponse
    {
    }
}
