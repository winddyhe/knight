using Core.Serializer;
using Core.WindJson;

// 不要在这个文件加[ProtoInclude]跟[BsonKnowType]标签,加到InnerMessage.cs或者OuterMessage.cs里面去
namespace Model
{
    [SBGroupInerited("Protocol")]
	public abstract partial class AMessage : SerializerBinary
	{
		public override string ToString()
        {
            var rJsonNode = JsonParser.ToJsonNode(this);
            return rJsonNode?.ToString();
        }
    }
    
	public abstract partial class ARequest : AMessage
	{
		public uint         RpcId;
	}

	/// <summary>
	/// 服务端回的RPC消息需要继承这个抽象类
	/// </summary>
	public abstract partial class AResponse : AMessage
	{
		public uint         RpcId;
		public int          Error   = 0;
		public string       Message = "";
	}
}