using System.ComponentModel;
using System.Net;

namespace HK_Launcher.Source
{
    internal static class Downloader
    {
        static WebClient webClient = new();
        public static readonly string remoteUri = "http://localhost:8080/";
        public static readonly string manifestFilename = "manifest.hk";

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
    }
}
