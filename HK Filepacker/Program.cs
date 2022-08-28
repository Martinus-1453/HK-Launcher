using System.Collections.Concurrent;
using System.Security.Cryptography;


const string manifestFilename = "manifest.hk";
var allowedExtensions = new[] {".vdf", ".zen", ".dll"};

//Delete old manifest file
File.Delete(manifestFilename);

ConcurrentDictionary<string,string> fileHashDictionary = new();

var fileList = Directory
    .GetFiles(Directory.GetCurrentDirectory())
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
    fileHashDictionary.TryAdd(Path.GetFileName(file), result);
});

using StreamWriter outputFile = new(manifestFilename, append: false);

foreach (var keyPair in fileHashDictionary)
{
    outputFile.WriteLine($"{keyPair.Key} {keyPair.Value}");
}

outputFile.Close();
Console.WriteLine("manifest.hk is ready!");

Console.Write("Press any key to exit ...");
Console.ReadKey(true);