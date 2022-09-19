using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace HK_Launcher.Source
{
    internal enum Error
    {
        Ok,
        MissingVersion,
        GothicNotFound,
        Unknown
    }

    internal struct RunResult
    {
        public Error Result { get; }
        public int Error { get; }
    }

    internal struct Version
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Build { get; }
        public override string ToString() => $"{Major}.{Minor}.{Patch}.{Build}";
    }

    internal static class LaunchG2O
    {
        // G2O_Proxy.dll methods
        [DllImport("G2O_Proxy.dll", EntryPoint = "G2O_Version", CallingConvention = CallingConvention.Cdecl,
            SetLastError = false)]
        private static extern Version G2O_Version();

        [DllImport("G2O_Proxy.dll", EntryPoint = "G2O_Run", CallingConvention = CallingConvention.Cdecl,
            SetLastError = false)]
        private static extern RunResult G2O_Run(int major, int minor, int patch);

        private static void UpdateRegistry()
        {
            RegistryKey? key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
            key = key?.CreateSubKey("G2O", true);
            key?.SetValue("ip_port", "51.68.138.141:5000");
            key?.Close();
        }

        public static void JoinServer()
        {
            UpdateRegistry();
            var version = G2O_Version();
            var result = G2O_Run(0, 2, 0);
            switch (result.Result)
            {
                case Error.Ok:
                    break;
                case Error.MissingVersion:
                    throw new Exception("Missing G2O version");
                case Error.GothicNotFound:
                    throw new Exception("Gothic2.exe not found");
                case Error.Unknown:
                    throw new Exception("Unknown error");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}