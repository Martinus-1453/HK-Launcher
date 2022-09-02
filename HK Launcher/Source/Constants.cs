namespace HK_Launcher.Source
{
    internal static class Constants
    {
        public static Dictionary<string, string> fileExtensionMap = new()
        {
            { ".zen", @"_work\Data\Worlds\" },
            { ".vdf", @"Data\" },
            { ".dll", @"Multiplayer\modules\" }
        };

        // DO NOT change that if you dont know what it is
        public static string G2O_API_URL = "http://api.gothic-online.com.pl/master/server/{0}/";

        public static string CONFIG_FOLDER_PATH = ".launcher/";
        public static string VERSION_FILE_NAME = "version.xml";
        public static string GAME_CONFIG_PATH = "system/Gothic.ini";
        public static string GAME_EXE = "system/Gothic2.exe";
        public static string G2O_DLL = "G2O_Proxy.dll";
    }
}