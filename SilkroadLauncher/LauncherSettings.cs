namespace SilkroadLauncher
{
    /// <summary>
    /// Global class for a quick usage setup
    /// </summary>
    public static class LauncherSettings
    {
        public static string
            APP_TITLE = "Silkroad Online Launcher",
            APP_WEBNOTICE_TIME_FORMAT = "dd.MM";

        public static string
            MSG_INSPECTION = "The server is undergoing inspection or updates.\nConnect to https://SilkroadOnline.com/ for more information.",
            MSG_PATCH_UNABLE = "We're sorry, the download server is down.\nPlease, try again later.",
            MSG_PATCH_TOO_OLD = "Your version is too old.\nConnect to https://SilkroadOnline.com/ to download the new client.",
            MSG_PATCH_TOO_NEW = "Your version is too new.\nDo you want to talk us about it? ;)";

        public static string[]
            CLIENT_LANGUAGE_SUPPORTED = new string[]{
                "Korean", // 2
                "Chinese", // 3
                "Japan", // 5
                "English", // 6
                //"Vietnam", // 7
                "Turkey", // 10
                "Spain", // 11
                "Taiwan", // 4
                "Russia", // 9
                "Arabic", // 12
                "Thailand", // 8
            };
        public static string[]
            CLIENT_LANGUAGE_SUPPORTED_MASK = new string[]{
                "한국어‫",
                "中文",
                "日本語‫",
                "English",
                "Türkçe",
                "Español",
                "Deutsch",
                "Русский",
                "العربية",
                "Portuguese",
            };

        public static string
            CLIENT_EXECUTABLE = "sro_client.exe",
            CLIENT_BLOWFISH_KEY = "169841";
        public static string[]
            CLIENT_VERIFY_HOST = new string[] {

            };
        public static ushort CLIENT_VERIFY_PORT = 0;

        public static string
            PATH_PK2_MEDIA = "Media.pk2";
    }
}
