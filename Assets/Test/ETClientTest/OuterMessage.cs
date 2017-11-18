using System.Collections.Generic;
using Core.Serializer;

namespace Model
{
	public abstract partial class AActorMessage : AMessage
	{
	}

    public abstract partial class AActorRequest : ARequest
    {
    }

    public abstract partial class AActorResponse : AResponse
	{
	}

    /// <summary>
    /// 帧消息，继承这个类的消息会经过服务端转发
    /// </summary>
    public abstract partial class AFrameMessage : AActorMessage
    {
    }

    [Message((ushort)Opcode.C2R_Login)]
	public partial class C2R_Login: ARequest
	{
		public string       Account;
		public string       Password;
	}
    
	[Message((ushort)Opcode.R2C_Login)]
    public partial class R2C_Login: AResponse
	{
		public string       Address     { get; set; }
		public long         Key         { get; set; }
	}
    
	[Message((ushort)Opcode.C2G_LoginGate)]
    public partial class C2G_LoginGate: ARequest
	{
		public long         Key;
	}
    
	[Message((ushort)Opcode.G2C_LoginGate)]
    public partial class G2C_LoginGate: AResponse
	{
		public long         PlayerId;
	}


	[Message((ushort)Opcode.Actor_Test)]
    public partial class Actor_Test : AActorMessage
	{
		public string       Info;
	}

	[Message((ushort)Opcode.Actor_TestRequest)]
    public partial class Actor_TestRequest : AActorRequest
	{
		public string       request;
	}

	[Message((ushort)Opcode.Actor_TestResponse)]
    public partial class Actor_TestResponse : AActorResponse
	{
		public string       response;
	}


	[Message((ushort)Opcode.Actor_TransferRequest)]
    public partial class Actor_TransferRequest : AActorRequest
	{
		public int          MapIndex;
	}

	[Message((ushort)Opcode.Actor_TransferResponse)]
    public partial class Actor_TransferResponse : AActorResponse
	{
	}
    
	[Message((ushort)Opcode.C2G_EnterMap)]
    public partial class C2G_EnterMap: ARequest
	{
	}

	[Message((ushort)Opcode.G2C_EnterMap)]
    public partial class G2C_EnterMap: AResponse
	{
		public long         UnitId;
		public int          Count;
	}

    public partial class UnitInfo : SerializerBinary
	{
		public long         UnitId;
		public int          X;
		public int          Z;
	}
    
	[Message((ushort)Opcode.Actor_CreateUnits)]
    public partial class Actor_CreateUnits : AActorMessage
	{
		public List<UnitInfo> Units = new List<UnitInfo>();
	}

    public partial struct FrameMessageInfo
	{
		public long         Id;
		public AMessage     Message;
	}
    
	[Message((ushort)Opcode.FrameMessage)]
    public partial class FrameMessage : AActorMessage
	{
		public int          Frame;
		public List<AFrameMessage> Messages = new List<AFrameMessage>();
	}
    
	[Message((ushort)Opcode.Frame_ClickMap)]
    public partial class Frame_ClickMap: AFrameMessage
	{
		public int          X;
		public int          Z;
	}

	[Message((ushort)Opcode.C2M_Reload)]
    public partial class C2M_Reload: ARequest
	{
		public AppType      AppType;
	}

	[Message((ushort)Opcode.M2C_Reload)]
    public partial class M2C_Reload: AResponse
	{
	}

	[Message((ushort)Opcode.C2R_Ping)]
    public partial class C2R_Ping: ARequest
	{
	}

	[Message((ushort)Opcode.R2C_Ping)]
    public partial class R2C_Ping: AResponse
	{
	}
}