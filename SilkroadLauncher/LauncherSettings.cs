namespace SilkroadLauncher
{
    /// <summary>
    /// Global class for a quick usage setup
    /// </summary>
    public static class LauncherSettings
    {
        public static string
            APP_TITLE = "Silkroad Online Launcher",
            APP_WEBNOTICE_TIME_FORMAT = "dd MMMM yyyy";

        public static string
            MSG_INSPECTION = "The server is undergoing inspection or updates.\nConnect to https://SilkroadOnline.com/ for more information.",
            MSG_PATCH_UNABLE = "We're sorry, the download server is down.\nPlease, try again later.",
            MSG_PATCH_TOO_OLD = "Your version is too old.\nConnect to https://SilkroadOnline.com/ to download the new client.",
            MSG_PATCH_TOO_NEW = "Your version is too new.\nDo you want to talk us about it? ;)";

        public static string[]
            CLIENT_LANGUAGE_SUPPORTED = new string[]{
                //"Korean", // 3
                //"Chinese", // 4
                //"Taiwan", // 5
                //"Japan", // 6
                //"English", // 7
                "Vietnam", // 8
                //"Thailand", // 9
                //"Russia", // 10
                "Turkey", // 11
                //"Spain", // 12
                //"Arabic", // 13
            };
        public static string[]
            CLIENT_LANGUAGE_SUPPORTED_MASK = new string[]{
                "English",
                "Türkçe",
            };

        public static string
            CLIENT_EXECUTABLE = "sro_client.exe",
            CLIENT_BLOWFISH_KEY = "169841";

        public static string
            PATH_PK2_MEDIA = "Media.pk2";
    }
}
