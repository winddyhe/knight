using System;

namespace Model
{
	public interface IMHandler
	{
		void Handle(ASession session, AMessage message);
		Type GetMessageType();
	}
}