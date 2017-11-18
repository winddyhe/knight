using System;

namespace Model
{
	public abstract class AMHandler<Message> : IMHandler where Message : AMessage
	{
		protected abstract void Run(ASession session, Message message);

		public void Handle(ASession session, AMessage msg)
		{
			Message message = msg as Message;
			if (message == null)
			{
				UnityEngine.Debug.LogError($"消息类型转换错误: {msg.GetType().Name} to {typeof(Message).Name}");
				return;
			}
			if (session.Id == 0)
			{
                UnityEngine.Debug.LogError($"session disconnect {msg}");
				return;
			}
			this.Run(session, message);
		}

		public Type GetMessageType()
		{
			return typeof(Message);
		}
	}
}