namespace Model
{
	public interface IMessageDispatcher
	{
		void Dispatch(ASession session, ushort opcode, int offset, byte[] messageBytes, AMessage message);
	}
}
