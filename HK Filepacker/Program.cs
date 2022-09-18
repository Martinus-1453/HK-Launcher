using System.Collections.Concurrent;
using System.Security.Cryptography;
using HK_Shared.Source;
using Newtonsoft.Json;


const string manifestFilename = "manifest.hk";
var allowedExtensions = new[] { ".vdf", ".zen", ".dll" };

//Delete old manifest file
File.Delete(manifestFilename);

ConcurrentBag<ManifestEntry> fileHashBag = new();

var fileList = Directory
    .GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories)
    .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
    .ToList();


Console.WriteLine($"Processing {fileList.Count} files:");
Console.WriteLine(string.Join('\n', fileList));

Parallel.ForEach(fileList, file =>
{
    using var md5 = MD5.Create();
    using var stream = File.OpenRead(file);
    var hash = md5.ComputeHash(stream);
    var result = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    var entry = new ManifestEntry
        { Checksum = result, Filename = Path.GetFileName(file), Filepath = Path.GetRelativePath(Directory.GetCurrentDirectory(), Path.GetDirectoryName(file)) };
    fileHashBag.Add(entry);
});

using StreamWriter outputFile = new(manifestFilename, append: false);
var manifest = new Manifest { ManifestEntries = fileHashBag.ToList() };
outputFile.Write(JsonConvert.SerializeObject(manifest, Formatting.Indented));

outputFile.Close();
Console.WriteLine("manifest.hk is ready!");

Console.Write("Press any key to exit ...");
Console.ReadKey(true);