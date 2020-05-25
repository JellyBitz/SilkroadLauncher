using System.Collections.Generic;

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
                "Vietnam",
                "English",
            };

        public static string
            CLIENT_EXECUTABLE = "sro_client.exe",
            CLIENT_BLOWFISH_KEY = "169841";

        public static string
            PATH_SILKCFG = "SilkCfg.dat",
            PATH_SROPTIONSET = "Setting\\SROptionSet.dat",
            PATH_PK2_MEDIA = "Media.pk2";
    }
}
