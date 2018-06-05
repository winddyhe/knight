using System;

namespace Knight.Framework.Net
{
	public interface IMHandler
	{
		void Handle(NetworkSession rSession, object rMessage);
		Type GetMessageType();
	}
}