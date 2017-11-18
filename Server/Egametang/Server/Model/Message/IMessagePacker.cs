using Core.Serializer;
using System;

namespace Model
{
	public interface IMessagePacker
	{
		byte[] SerializeToByteArray(object obj);
		string SerializeToText(object obj);

		object DeserializeFrom(Type type, byte[] bytes);
		object DeserializeFrom(Type type, byte[] bytes, int index, int count);
		T DeserializeFrom<T>(byte[] bytes) where T : SerializerBinary;
		T DeserializeFrom<T>(byte[] bytes, int index, int count) where T : SerializerBinary;

		T DeserializeFrom<T>(string str) where T : SerializerBinary;
		object DeserializeFrom(Type type, string str);
	}
}
