using System.IO;

namespace Knight.Core
{
    public static class PathTool
    {
        public static string Combine(params string[] rPaths)
        {
            var rPath = Path.Combine(rPaths);
            return rPath.Replace("\\", "/");
        }

        public static string GetParentPath(string rPath)
        {
            var rParentPath = Path.GetDirectoryName(rPath);
            return rParentPath.Replace("\\", "/");
        }

        public static string ReadFileText(string rFilePath)
        {
            if (!File.Exists(rFilePath)) 
            {
                return string.Empty;
            }
            return File.ReadAllText(rFilePath);
        }

        public static byte[] ReadFileBytes(string rFilePath)
        {
            if (!File.Exists(rFilePath))
            {
                return null;
            }
            return File.ReadAllBytes(rFilePath);
        }

        public static void CreateDirectory(string rFilePath)
        {
            var rFileDirectory = Path.GetDirectoryName(rFilePath);
            if (!Directory.Exists(rFileDirectory))
            {
                Directory.CreateDirectory(rFileDirectory);
            }
        }

        public static void WriteFile(string rFilePath, byte[] rBytes)
        {
            var rFileDirectory = Path.GetDirectoryName(rFilePath);
            if (!Directory.Exists(rFileDirectory))
            {
                Directory.CreateDirectory(rFileDirectory);
            }
            File.WriteAllBytes(rFilePath, rBytes);
        }

        public static void WriteFile(string rFilePath, string rText)
        {
            var rFileDirectory = Path.GetDirectoryName(rFilePath);
            if (!Directory.Exists(rFileDirectory))
            {
                Directory.CreateDirectory(rFileDirectory);
            }
            File.WriteAllText(rFilePath, rText);
        }

        public static void CopyFile(string rSrcFilePath, string rDestFilePath)
        {
            var rFileDirectory = Path.GetDirectoryName(rDestFilePath);
            if (!Directory.Exists(rFileDirectory))
            {
                Directory.CreateDirectory(rFileDirectory);
            }
            File.Copy(rSrcFilePath, rDestFilePath, true);
        }

        public static long GetFileSize(string rFilePath)
        {
            if (!File.Exists(rFilePath))
            {
                return 0;
            }
            var rFileInfo = new FileInfo(rFilePath);
            return rFileInfo.Length;
        }

        public static void DeleteFile(string rFilePath)
        {
            if (!File.Exists(rFilePath))
            {
                return;
            }
            File.Delete(rFilePath);
        }

        public static string ReplaceSlash(this string rPath)
        {
            if (string.IsNullOrEmpty(rPath))
            {
                return string.Empty;
            }
            return rPath.Replace('\\', '/');
        }
    }
}
