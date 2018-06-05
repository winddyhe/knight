namespace Knight.Framework.Net
{
	public interface IMessageDispatcher
	{
		void Dispatch(NetworkSession rSession, Packet rPacket);
	}
}
