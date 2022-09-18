using System.ComponentModel;
using System.Net;
using HK_Shared.Source;

namespace HK_Launcher.Source
{
    internal static class Downloader
    {
        static WebClient webClient = new();
#if DEBUG
        public static readonly string remoteUri = "http://localhost:7800/";
#else
        public static readonly string remoteUri = "http://historiakoloniilauncher.hmcloud.pl/";
#endif

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

        public static async Task DownloadFile(string fileName, string filePath = "")
        {
            if (filePath != "." && filePath != string.Empty)
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), filePath));
                await webClient.DownloadFileTaskAsync($"{remoteUri}{Path.Combine(filePath, fileName)}", Path.Combine(filePath, fileName));
            }
            else
            {
                await webClient.DownloadFileTaskAsync($"{remoteUri}{fileName}", fileName);
            }
        }

        public static void OldFilesRemover(Manifest manifest)
        {
            var currentDataFiles = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), manifest.CUSTOM_FOLDER_PATH));

            foreach (var file in currentDataFiles)
            {
                if (manifest.ManifestEntries.All(item => item.GetFullName() != file))
                {
                    File.Delete(file);
                }
            }
        }
    }
}