using Pomelo.DotNetClient;
using System;
using System.Collections.Generic;
using Core.WindJson;

namespace Pomelo.DotNetClient.Test
{
    public class TransportTest
    {
        static List<byte[]> result = new List<byte[]>();

        public static byte[] genBuffer(int count)
        {
            int size = count + 6;
            byte[] buffer = new byte[count + 10];

            //Transporter tp = new Transporter(new Object(), protocol);
            buffer[0] = (byte)PackageType.PKG_DATA;
            buffer[1] = Convert.ToByte(size >> 16 & 0xFF);
            buffer[2] = Convert.ToByte(size >> 8 & 0xFF);
            buffer[3] = Convert.ToByte(size & 0xFF);

            //Generate message head
            //Response
            buffer[4] = 4;
            buffer[5] = 128;
            buffer[6] = 1;

            //Route length
            buffer[7] = 2;

            //Route
            buffer[8] = 62;
            buffer[9] = 72;

            Random random = new Random();
            for (var i = 0; i < count; i++) buffer[10 + i] = (byte)random.Next(255);

            return buffer;
        }

        public static byte[] generateBuffers(int num, out List<byte[]> list)
        {
            int length = 100;
            int index = 0;
            byte[] result = new byte[(length + 10) * num];
            list = new List<byte[]>();

            for (int i = 0; i < num; i++)
            {
                byte[] bytes = genBuffer(length);
                list.Add(bytes);
                for (int j = 0; j < bytes.Length; j++) result[index++] = bytes[j];
            }

            return result;
        }

        public static void Run()
        {
            int num = 10;
            //int limit = 1000;

            Transporter tc = new Transporter(null, process);

            List<byte[]> list;

            byte[] buffer = generateBuffers(num, out list);

            int offset = 0;
            while (offset < buffer.Length)
            {
                int length = 200;
                length = (offset + length) > buffer.Length ? buffer.Length - offset : length;

                tc.processBytes(buffer, offset, offset + length);
                offset += length;
            }

            if (!check(list))
            {
                Console.WriteLine("Transport test failed!");
            }
            else
            {
                Console.WriteLine("Transport test success!");
            }
        }

        public static void process(byte[] bytes)
        {
            result.Add(bytes);
            //Console.WriteLine("add bytes : {0}", result.Count);
        }

        public static void protocolProcess(byte[] bytes)
        {
            JsonNode dict = new JsonClass();
            JsonNode serverProtos = new JsonClass();
            JsonNode clientProtos = new JsonClass();

            MessageProtocol messageProtocol = new MessageProtocol(dict, serverProtos, clientProtos);
            Package pkg = PackageProtocol.decode(bytes);

            messageProtocol.decode(pkg.body);
        }

        public static bool check(List<byte[]> list)
        {
            byte[][] origin = list.ToArray();
            byte[][] target = result.ToArray();

            if (origin.Length != target.Length) return false;

            for (int i = 0; i < origin.Length; i++)
            {
                byte[] o = origin[i];
                byte[] t = result[i];

                if (o.Length != t.Length) return false;
                for (int j = 0; j < o.Length; j++)
                {
                    if (o[j] != t[j]) return false;
                }
            }

            return true;
        }
    }
}