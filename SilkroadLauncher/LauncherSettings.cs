namespace SilkroadLauncher
{
    /// <summary>
    /// Global class for a quick usage setup
    /// </summary>
    public static class LauncherSettings
    {
        // App
        public static string
            APP_TITLE = "Silkroad Online Launcher",
            APP_WEBNOTICE_TIME_FORMAT = "dd.MM";

        // Messages
        public static string
            MSG_INSPECTION = "The server is undergoing inspection or updates.\nConnect to {0} for more information.",
            MSG_PATCH_UNABLE = "We're sorry, the download server is down.\nPlease, try again later.",
            MSG_PATCH_TOO_OLD = "Your version is too old.\nConnect to {0} to download the new client.",
            MSG_PATCH_TOO_NEW = "Your version is too new.\nDo you want to talk us about it? ;)",
            MSG_ERR_FILE_UPDATE = "Fatal error updating file \"{0}\"!",
            MSG_ERR_FILE_OPEN = "Fatal error opening file \"{0}\"!";

        // Languages availables
        public static string[]
            CLIENT_LANGUAGE_SUPPORTED = new string[]{
                "Korean", // 1
                "Chinese", // 4
                "Japan", // 6
                "English", // 7
                "Turkey", // 11
                "Spain", // 12
                "Taiwan", // 5 (as Deutch)
                "Russia", // 10
                "Arabic", // 13
                "Vietnam", // 8
                "Thailand", // 9 (as Portuguese)
            };
        public static string[]
            CLIENT_LANGUAGE_SUPPORTED_MASK = new string[]{
                "한국어‫",
                //"中文(繁體)", // Chinese (Traditional)
                "中文(简体)", // Chinese (Simplified)
                "日本語‫",
                "English",
                "Türkçe",
                "Español",
                "Deutsch",
                "Русский",
                "العربية",
                "Tiếng Việt",
                "Portuguese",
            };

        // Client settings
        public static string
            CLIENT_EXECUTABLE = "sro_client.exe",
            CLIENT_BLOWFISH_KEY = "169841",
            CLIENT_MEDIA_PK2_PATH = "Media.pk2";

        // Client restriction
        /// Restrict hosts used to connect (empty = disabled)
        public static string[]
            CLIENT_VERIFY_HOST = new string[] {
                // Example:
                //"127.0.0.1", // Launcher will work at local servers only
            };
        /// Restrict port used to connect (0 = disabled)
        public static ushort CLIENT_VERIFY_PORT = 0;
    }
}