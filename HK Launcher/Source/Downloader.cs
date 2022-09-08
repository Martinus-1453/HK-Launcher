using System.ComponentModel;
using System.Net;

namespace HK_Launcher.Source
{
    internal static class Downloader
    {
        static WebClient webClient = new();
        public static readonly string remoteUri = "http://historiakoloniilauncher.hmcloud.pl/";
        public static readonly string manifestFilename = "manifest.hk";

        static readonly List<string> nativeDataFileList = new()
        {
            "Anims.vdf",
            "Anims_Addon.vdf",
            "Meshes.vdf",
            "Meshes_Addon.vdf",
            "Sounds.vdf",
            "Sounds_Addon.vdf",
            "Sounds_bird_01.vdf",
            "Speech_Addon.vdf",
            "Speech1.vdf",
            "Speech2.vdf",
            "SystemPack.vdf",
            "Textures.vdf",
            "Textures_Addon.vdf",
            "Worlds.vdf",
            "Worlds_Addon.vdf"
        };

        public static void SetHandlers(AsyncCompletedEventHandler? downloadFileCompletedHandler = null,
            DownloadProgressChangedEventHandler? downloadFileProgressHandler = null)
        {
            webClient.DownloadFileCompleted += downloadFileCompletedHandler;
            webClient.DownloadProgressChanged += downloadFileProgressHandler;
        }

        public static async Task DownloadFile(string filename)
        {
            Constants.fileExtensionMap.TryGetValue(Path.GetExtension(filename).ToLower(), out var filePath);
            if (filePath != null)
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), filePath));
                await webClient.DownloadFileTaskAsync($"{remoteUri}{filename}", Path.Combine(filePath, filename));
            }
            else
            {
                await webClient.DownloadFileTaskAsync($"{remoteUri}{filename}", filename);
            }
        }

        public static void OldFilesRemover(ICollection<string> fileCollection)
        {
            var currentDataFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Data/"));

            foreach (var file in currentDataFiles)
            {
                var fileName = Path.GetFileName(file);
                if (!nativeDataFileList.Contains(fileName, StringComparer.OrdinalIgnoreCase) &&
                    !fileCollection.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                {
                    File.Delete(file);
                }
            }
        }
    }
}