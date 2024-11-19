using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Knight.Core
{
    public static class MD5Tool
    {
        public static byte[] GetFileMD5(string rContentFile)
        {
            return GetFilesMD5(new List<string>() { rContentFile }, string.Empty);
        }

        public static byte[] GetFilesMD5(List<string> rContentFiles, string rExtraFilePath)
        {
            rContentFiles.Sort((a1, a2) => { return a1.CompareTo(a2); });
            HashAlgorithm rHasAlgo = HashAlgorithm.Create("MD5");
            byte[] rHashValue = new byte[20];
            byte[] rTempBuffer = new byte[4096];
            int rTempCount = 0;
            for (int i = 0; i < rContentFiles.Count; i++)
            {
                if (File.Exists(rContentFiles[i]))
                {
                    FileStream fs = File.OpenRead(rContentFiles[i]);
                    while (fs.Position != fs.Length)
                    {
                        rTempCount += fs.Read(rTempBuffer, 0, 4096 - rTempCount);
                        if (rTempCount == 4096)
                        {
                            if (rHasAlgo.TransformBlock(rTempBuffer, 0, rTempCount, null, 0) != 4096)
                                UnityEngine.Debug.LogError("TransformBlock error.");
                            rTempCount = 0;
                        }
                    }
                    fs.Close();
                }
            }
            var rLongString = new StringBuilder();
            for (int i = 0; i < rContentFiles.Count; i++)
            {
                rLongString.Append(rContentFiles[i]);
            }
            rLongString.Append(rExtraFilePath);
            var rFileBytes = System.Text.Encoding.Default.GetBytes(rLongString.ToString());
            var ms = new MemoryStream(rFileBytes);
            while (ms.Position != ms.Length)
            {
                rTempCount += ms.Read(rTempBuffer, 0, 4096 - rTempCount);
                if (rTempCount == 4096)
                {
                    if (rHasAlgo.TransformBlock(rTempBuffer, 0, rTempCount, null, 0) != 4096)
                        UnityEngine.Debug.LogError("TransformBlock error.");
                    rTempCount = 0;
                }
            }
            rHasAlgo.TransformFinalBlock(rTempBuffer, 0, rTempCount);
            rHashValue = rHasAlgo.Hash;
            return rHashValue;
        }

        public static byte[] GetBytesMD5(byte[] rDatas)
        {
            var rHashAlgorithm = HashAlgorithm.Create("MD5");
            rHashAlgorithm.TransformFinalBlock(rDatas, 0, rDatas.Length);
            return rHashAlgorithm.Hash;
        }

        public static byte[] GetStringMD5(string rText, Encoding rEncoding)
        {
            return HashAlgorithmByString(rText, "MD5", rEncoding);
        }

        public static byte[] GetStringMD5(string rText)
        {
            return GetStringMD5(rText, Encoding.Default);
        }

        public static string ToHEXString(this byte[] rSelfBytes)
        {
            var rText = new StringBuilder();
            for (int nIndex = 0; nIndex < rSelfBytes.Length; ++nIndex)
                rText.AppendFormat("{0:X2}", rSelfBytes[nIndex]);
            return rText.ToString();
        }

        public static string ToHEXLowerString(this byte[] rSelfBytes)
        {
            var rText = new StringBuilder();
            for (int nIndex = 0; nIndex < rSelfBytes.Length; ++nIndex)
                rText.AppendFormat("{0:x2}", rSelfBytes[nIndex]);
            return rText.ToString();
        }

        public static byte[] HashAlgorithmByString(string rText, string rHashName, Encoding rEncoding)
        {
            var rHashAlgorithm = HashAlgorithm.Create(rHashName);
            var rTextBytes = rEncoding.GetBytes(rText);
            rHashAlgorithm.TransformFinalBlock(rTextBytes, 0, rTextBytes.Length);
            return rHashAlgorithm.Hash;
        }
    }
}