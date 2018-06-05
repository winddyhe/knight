using System;

namespace Knight.Framework.Net
{
	public interface IMessagePacker
	{
		byte[] SerializeToByteArray(object rObj);
		string SerializeToText(object rObj);

		object DeserializeFrom(Type rType, byte[] rBytes);
		object DeserializeFrom(Type rType, byte[] rBytes, int nIndex, int nCount);
        T DeserializeFrom<T>(byte[] rBytes);
        T DeserializeFrom<T>(byte[] rBytes, int nIndex, int nCount);

        T DeserializeFrom<T>(string rStr);
        object DeserializeFrom(Type rType, string rStr);
	}
}
