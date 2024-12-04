using Pk2ReaderAPI;
using SilkroadLauncher.Utility;

using System.Windows.Media;

namespace SilkroadLauncher
{
    /// <summary>
    /// Contains all assets references from media.pk2 file
    /// </summary>
    public class LauncherAssets
    {
        #region Public Properties
        public ImageBrush Background { get; }

        public ImageBrush ButtonStartGame { get; }
        public ImageBrush ButtonStartGameDisabled { get; }
        public ImageBrush ButtonStartGameHover { get; }
        public ImageBrush ButtonStartGamePressed { get; }
        public ImageBrush ButtonExit { get; }
        public ImageBrush ButtonExitHover { get; }
        public ImageBrush ButtonExitPressed { get; }
        public ImageBrush ButtonWebsite { get; }
        public ImageBrush ButtonWebsiteHover { get; }
        public ImageBrush ButtonWebsitePressed { get; }
        public ImageBrush ButtonDiscord { get; }
        public ImageBrush ButtonDiscordHover { get; }
        public ImageBrush ButtonDiscordPressed { get; }
        public ImageBrush ButtonFacebook { get; }
        public ImageBrush ButtonFacebookHover { get; }
        public ImageBrush ButtonFacebookPressed { get; }

        public ImageBrush TextLanguageBackground { get; }
        public ImageBrush ButtonLanguage { get; }
        public ImageBrush ButtonLanguageHover { get; }
        public ImageBrush ButtonLanguagePressed { get; }

        public ImageBrush PopupLanguageBackground { get; }
        public ImageBrush CheckboxLanguage { get; }
        public ImageBrush CheckboxLanguageChecked { get; }
        public ImageBrush ButtonLanguageOk { get; }
        public ImageBrush ButtonLanguageOkHover { get; }
        public ImageBrush ButtonLanguageOkPressed { get; }
        public ImageBrush ButtonLanguageCancel { get; }
        public ImageBrush ButtonLanguageCancelHover { get; }
        public ImageBrush ButtonLanguageCancelPressed { get; }

        public ImageBrush ProgressbarBackground { get; }
        public ImageBrush ProgressbarFill { get; }

        public string LinkWebsite { get; }
        public string LinkDiscord { get; }
        public string LinkFacebook { get; }
        #endregion

        #region Constructor
        public LauncherAssets(Pk2Reader reader)
        {
            Background = new ImageBrush(reader.GetImage("launcher_wpf_fire/background.dat"));

            ButtonStartGame = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_start.dat"));
            ButtonStartGameDisabled = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_start_disabled.dat"));
            ButtonStartGameHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_start_hover.dat"));
            ButtonStartGamePressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_start_pressed.dat"));
            ButtonExit = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_exit.dat"));
            ButtonExitHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_exit_hover.dat"));
            ButtonExitPressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_exit_pressed.dat"));
            ButtonWebsite = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_website.dat"));
            ButtonWebsiteHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_website_hover.dat"));
            ButtonWebsitePressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_website_pressed.dat"));
            ButtonDiscord = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_discord.dat"));
            ButtonDiscordHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_discord_hover.dat"));
            ButtonDiscordPressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_discord_pressed.dat"));
            ButtonFacebook = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_facebook.dat"));
            ButtonFacebookHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_facebook_hover.dat"));
            ButtonFacebookPressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_facebook_pressed.dat"));

            TextLanguageBackground = new ImageBrush(reader.GetImage("launcher_wpf_fire/text_language_background.dat"));
            ButtonLanguage = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language.dat"));
            ButtonLanguageHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_hover.dat"));
            ButtonLanguagePressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_pressed.dat"));

            PopupLanguageBackground = new ImageBrush(reader.GetImage("launcher_wpf_fire/popup_language_background.dat"));
            CheckboxLanguage = new ImageBrush(reader.GetImage("launcher_wpf_fire/checkbox_language.dat"));
            CheckboxLanguageChecked = new ImageBrush(reader.GetImage("launcher_wpf_fire/checkbox_language_checked.dat"));
            ButtonLanguageOk = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_ok.dat"));
            ButtonLanguageOkHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_ok_hover.dat"));
            ButtonLanguageOkPressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_ok_pressed.dat"));
            ButtonLanguageCancel = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_cancel.dat"));
            ButtonLanguageCancelHover = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_cancel_hover.dat"));
            ButtonLanguageCancelPressed = new ImageBrush(reader.GetImage("launcher_wpf_fire/button_language_cancel_pressed.dat"));

            ProgressbarBackground = new ImageBrush(reader.GetImage("launcher_wpf_fire/progressbar_background.dat"));
            ProgressbarFill = new ImageBrush(reader.GetImage("launcher_wpf_fire/progressbar_fill.dat"));

            LinkWebsite = reader.GetFileText("launcher_wpf_fire/link_website.txt");
            LinkDiscord = reader.GetFileText("launcher_wpf_fire/link_discord.txt");
            LinkFacebook = reader.GetFileText("launcher_wpf_fire/link_facebook.txt");
        }
        #endregion
    }
}