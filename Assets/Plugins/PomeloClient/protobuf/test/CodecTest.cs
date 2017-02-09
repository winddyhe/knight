using System;
using System.IO;
using Pomelo.Protobuf;

namespace Pomelo.Protobuf.Test
{
    public class CodecTest
    {
        public static bool EncodeSInt32Test(int count)
        {
            Random random = new Random();

            int flag = -1;
            for (int i = 0; i < count; i++)
            {
                flag *= -1;
                int num = random.Next(0, 0x7fffffff) * flag;
                byte[] bytes = Encoder.encodeSInt32(num);
                int result = Decoder.decodeSInt32(bytes);
                if (num != result) return false;
            }

            return true;
        }

        public static bool EncodeUInt32Test(int count)
        {
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                uint num = (uint)random.Next(0, 0x7fffffff);
                byte[] bytes = Encoder.encodeUInt32(num);
                uint result = Decoder.decodeUInt32(bytes);
                if (num != result) return false;
            }

            return true;
        }

        public static bool Run()
        {
            bool success = true, flag = false;
            DateTime start, end;

            start = DateTime.Now;
            flag = EncodeSInt32Test(10000);
            end = DateTime.Now;
            Console.WriteLine("Encode sint32 test finished , result is : {1}, cost time : {0}", end - start, flag);
            if (!flag) success = false;

            start = DateTime.Now;
            flag = EncodeUInt32Test(10000);
            end = DateTime.Now;
            Console.WriteLine("Encode uint32 test finished , result is : {1}, cost time : {0}", end - start, flag);
            if (!flag) success = false;

            return success;
        }
    }
}