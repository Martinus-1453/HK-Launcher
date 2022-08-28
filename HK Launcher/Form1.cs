using Extensions;
using HK_Launcher.Source;

namespace HK_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private bool CheckRequiredFilesExist()
        {
            return File.Exists("G2O_Launcher.exe") && 
                   File.Exists("system/Gothic2.exe") &&
                   File.Exists("G2O_Proxy.dll");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            try
            {
                if (CheckRequiredFilesExist())
                {
                    await DownloadLogic();
#if !DEBUG
                LaunchG2O.JoinServer();
#endif
                }
                else
                {
                    label1.Text = "Brakuje plików g2o lub aplikacja jest w z³ym folderze";
                }

            }
            catch (Exception exception)
            {
                label1.Text = exception.Message;
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        private async Task DownloadLogic()
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 0;
            label1.Text = "Pobieranie manifestu plików";
            await Downloader.DownloadFile(Downloader.manifestFilename);
            progressBar1.Style = ProgressBarStyle.Marquee;
            label1.Text = "Sprawdzanie plików...";
            var downloadList = Checksum.ReadChecksumManifest(Downloader.manifestFilename);
            var counter = 0;
            await downloadList;
            string downloadInfo = string.Empty;
            string namename = string.Empty;
            Downloader.SetHandlers((s, e) =>
                {
                    Invoker(() =>
                    {
                        label1.InvokeIfRequired(c => c.Text = $"{downloadInfo}: {namename} 100%");
                        progressBar1.InvokeIfRequired(c => c.Value = 100);
                    });
                },
                (s, e) =>
                {
                    Invoker(() =>
                    {
                        label1.InvokeIfRequired(c => c.Text = $"{downloadInfo}: {namename} {e.ProgressPercentage}%");
                        progressBar1.InvokeIfRequired(c => c.Value = e.ProgressPercentage);
                    });
                });
            progressBar1.Style = ProgressBarStyle.Continuous;
            foreach (var download in downloadList.Result.Keys)
            {
                ++counter;
                downloadInfo = $"Pobieranie ({counter}/{downloadList.Result.Keys.Count})";
                namename = download;
                await Downloader.DownloadFile(download);
            }
            label1.Text = "Gotowe!";
            progressBar1.Value = 100;
        }

        private void Invoker(MethodInvoker methodInvokerDelegate)
        {
            if (InvokeRequired)
                Invoke(methodInvokerDelegate);
            else
                methodInvokerDelegate();
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}