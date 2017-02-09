using System;
using System.Collections.Generic;
using System.IO;
using Core.WindJson;
using Pomelo.Protobuf;

namespace Pomelo.Protobuf.Test
{
    public class ProtobufTest
    {
        public static JsonNode read(string name)
        {
            StreamReader file = new StreamReader(name);

            String str = file.ReadToEnd();

            return JsonParser.Parse(str);
        }

        public static bool equal(JsonNode a, JsonNode b)
        {
            ICollection<string> keys0 = a.Keys;
            //ICollection<string> keys1 = b.Keys;

            foreach (string key in keys0)
            {
                Console.WriteLine(a[key].GetType());
                if (a[key] is JsonClass)
                {
                    if (!equal(a[key], b[key])) return false;
                }
                else if (a[key] is JsonArray)
                {
                    continue;
                }
                else
                {
                    if (!a[key].ToString().Equals(b[key].ToString())) return false;
                }
            }

            return true;
        }

        public static void Run()
        {
            JsonNode protos = read("../../json/rootProtos.json");
            JsonNode msgs = read("../../json/rootMsg.json");

            Protobuf protobuf = new Protobuf(protos, protos);

            ICollection<string> keys = msgs.Keys;

            foreach (string key in keys)
            {
                JsonNode msg = msgs[key];
                byte[] bytes = protobuf.encode(key, msg);
                JsonNode result = protobuf.decode(key, bytes);
                if (!equal(msg, result))
                {
                    Console.WriteLine("protobuf test failed!");
                    return;
                }
            }

            Console.WriteLine("Protobuf test success!");
        }

        private static void print(byte[] bytes, int offset, int length)
        {
            for (int i = offset; i < length; i++)
                Console.Write(Convert.ToString(bytes[i], 16) + " ");
            Console.WriteLine();
        }
    }
}