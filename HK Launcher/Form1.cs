using Extensions;
using HK_Launcher.Source;
using Version = HK_Shared.Source.Version;

namespace HK_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MouseDown += Form1_MouseDown;
            button1.Click += button1_Click;
            //button2.Click += button2_Click;
            //button3.Click += button3_Click;
            progressBar1.ForeColor = Color.FromArgb(112, 98, 53);
            progressBar1.BackColor = Color.FromArgb(66,56,23);
        }

        private async void button3_Click(object? sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
        private async void button2_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private bool CheckRequiredFilesExist()
        {
            return File.Exists("G2O_Launcher.exe") &&
                   File.Exists("system/Gothic2.exe") &&
                   File.Exists("G2O_Proxy.dll");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button1.Visible = false;
            progressBar1.Visible = true;
            try
            {
                if (CheckRequiredFilesExist())
                {
                    var version = await DownloadLogic();
#if !DEBUG
                    LaunchG2O.JoinServer(version);
#endif
                }
                else
                {
                    label1.Text = "Brakuje plik?w g2o lub aplikacja jest w z?ym folderze";
                }
            }
            catch (Exception exception)
            {
                label1.Text = exception.Message;
            }
            finally
            {
                button1.Enabled = true;
                button1.Visible = true;
                progressBar1.Visible = false;
            }
        }

        private async Task<Version> DownloadLogic()
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Value = 0;
            label1.Text = "Pobieranie manifestu plik?w";
            await Downloader.DownloadFile(Downloader.manifestFilename);
            progressBar1.Style = ProgressBarStyle.Marquee;
            label1.Text = "Sprawdzanie plik?w...";
            var manifest = Checksum.ReadChecksumManifest(Downloader.manifestFilename);
            await manifest;
            var manifestResult = manifest.Result;
            Downloader.OldFilesRemover(manifestResult);
            manifest = Checksum.ProcessChecksumManifest(manifestResult);
            var counter = 0;

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
            await manifest;
            manifestResult = manifest.Result;
            foreach (var download in manifestResult.ManifestEntries)
            {
                ++counter;
                downloadInfo = $"Pobieranie ({counter}/{manifestResult.ManifestEntries.Count})";
                namename = download.Filename;
                await Downloader.DownloadFile(download.Filename, download.Filepath);
            }

            label1.Text = "Gotowe!";
            progressBar1.Value = 100;
            return manifestResult.G2O_VERSION;
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

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}