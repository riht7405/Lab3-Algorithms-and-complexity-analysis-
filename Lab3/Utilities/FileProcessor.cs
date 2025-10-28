using System.IO;

namespace Lab3.Utilities
{
    public static class FileProcessor
    {
        public static string ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не найден");

            return File.ReadAllText(filePath).Trim();
        }

        public static void WriteFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
    }
}