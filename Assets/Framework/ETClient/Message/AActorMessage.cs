
// 不要在这个文件加[ProtoInclude]跟[BsonKnowType]标签,加到InnerMessage.cs或者OuterMessage.cs里面去
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
    
	public abstract partial class AFrameMessage : AActorMessage
	{
		public long Id;
	}
}