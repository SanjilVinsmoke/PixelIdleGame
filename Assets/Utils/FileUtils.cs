using System.IO;

namespace Utils
{
    public class FileUtils
    {
        public static void SaveTextToFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public static string LoadTextFromFile(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllText(filePath) : string.Empty;
        }

        public static bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static string[] GetFilesInDirectory(string path)
        {
            return Directory.Exists(path) ? Directory.GetFiles(path) : new string[0];
        }

        public static string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        public static string RemoveFileExtension(string filePath)
        {
            return Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
        }
    }
}