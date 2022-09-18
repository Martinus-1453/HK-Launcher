using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using HK_Shared.Source;
using Newtonsoft.Json;

namespace HK_Launcher.Source
{
    internal static class Checksum
    {
        public static async Task<ManifestEntry?> ChecksumValidation(ManifestEntry entry)
        {
            var filename = entry.GetFullName();
            if (!File.Exists(filename)) return null;

            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(filename);
            var hash = md5.ComputeHashAsync(stream);
            await hash;
            var result = BitConverter.ToString(hash.Result).Replace("-", "").ToLowerInvariant();
            if (result == entry.Checksum)
            {
                return entry;
            }

            return null;
        }

        public static async Task<Manifest?> ReadChecksumManifest(string filename)
        {
            return JsonConvert.DeserializeObject<Manifest>(await File.ReadAllTextAsync(filename));
        }

        public static async Task<Manifest?> ProcessChecksumManifest(Manifest? manifest)
        {
            var taskList = new List<Task<ManifestEntry?>>();

            if (manifest != null)
            {
                foreach (var entry in manifest.ManifestEntries)
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), entry.Filepath));
                    taskList.Add(ChecksumValidation(entry));
                }
            }

            var result = await Task.WhenAll(taskList);

            foreach (var task in result)
            {
                if (task != null)
                {
                    manifest?.ManifestEntries.Remove((ManifestEntry)task);
                }
            }
            return manifest;
        }
    }
}