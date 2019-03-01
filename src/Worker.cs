using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Unprotecc
{
    internal class Worker
    {
        private const string Worksheet_Pattern = @"xl/worksheets.*\.xml$";
        private const string Protection_Pattern = @"<sheetProtection.+?>";
        private const string Output_Suffix = "_unprotecc";

        internal static void Run(string[] args)
        {
            foreach (var oneItem in args)
            {
                try
                {
                    var filePath = Path.GetFullPath(oneItem);

                    Logger.Write($"Processing {filePath}");

                    Checc(filePath);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex.Message);
                }
            }
        }

        private static void Checc(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            using (var zipArchive = System.IO.Compression.ZipFile.OpenRead(filePath))
            {
                if (zipArchive.Entries.Count == 0)
                    return;
            }

            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);

            var newFileName = Path.ChangeExtension(fileName + Output_Suffix, extension);
            var newFilePath = Path.Combine(directory, newFileName);

            File.Copy(filePath, newFilePath, true);

            Unprotecc(newFilePath);
        }

        private static void Unprotecc(string filePath)
        {
            using (var zipArchive = System.IO.Compression.ZipFile.Open(filePath, System.IO.Compression.ZipArchiveMode.Update))
            {
                foreach (var oneEntry in zipArchive.Entries)
                {
                    if (!Regex.IsMatch(oneEntry.FullName, Worksheet_Pattern))
                        continue;

                    string contents;

                    using (var stream = oneEntry.Open())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        contents = reader.ReadToEnd();
                    }

                    var newContents = Regex.Replace(contents, Protection_Pattern, string.Empty);

                    if (newContents.Length == contents.Length)
                        continue;

                    Logger.Write(oneEntry.FullName);

                    using (var stream = oneEntry.Open())
                    {
                        stream.SetLength(0);
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(newContents);
                        }
                    }
                }
            }
        }
    }
}