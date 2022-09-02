using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace HK_Launcher.Source
{
    internal static class Checksum
    {
        private static readonly ConcurrentDictionary<string, string> filesToUpdate = new();

        public static async Task ChecksumValidation(string filename, string checksum)
        {
            if (!File.Exists(filename)) return;

            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(filename);
            var hash = md5.ComputeHashAsync(stream);
            await hash;
            var result = BitConverter.ToString(hash.Result).Replace("-", "").ToLowerInvariant();
            if (result == checksum)
            {
                filesToUpdate[Path.GetFileName(filename)] = String.Empty;
            }
            else
            {
                return;
            }
        }

        public static async Task<ConcurrentDictionary<string, string>> ReadChecksumManifest(string filename)
        {
            filesToUpdate.Clear();
            foreach (var line in await File.ReadAllLinesAsync(filename))
            {
                var parts = line.Split(' ');
                if (parts.Length < 2)
                {
                    throw new Exception("File checksum is missing");
                }

                var hash = parts.LastOrDefault();
                var file = string.Join(" ", parts[0..^1]);
                filesToUpdate.TryAdd(file, hash);
            }

            var taskList = new List<Task>();

            foreach (var file in filesToUpdate)
            {
                Constants.fileExtensionMap.TryGetValue(Path.GetExtension(file.Key).ToLower(), out var filePath);
                if (filePath != null)
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), filePath));
                    taskList.Add(ChecksumValidation(Path.Combine(filePath, file.Key), file.Value));
                }
                else
                {
                    taskList.Add(ChecksumValidation(file.Key, file.Value));
                }
            }

            await Task.WhenAll(taskList);
            return filesToUpdate;
        }
    }
}