using SilkroadLauncher.Utility;
using SRO.PK2API;

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

        public ImageBrush HomeIcon { get; }
        public ImageBrush HomeIconOnHover { get; }

        public ImageBrush OptionButton { get; }
        public ImageBrush OptionButtonOnHover { get; }
        public ImageBrush OptionButtonOnPressed { get; }
        public ImageBrush GuideButton { get; }
        public ImageBrush GuideButtonOnHover { get; }
        public ImageBrush GuideButtonOnPressed { get; }
        public ImageBrush MovieButton { get; }
        public ImageBrush MovieButtonOnHover { get; }
        public ImageBrush MovieButtonOnPressed { get; }
        public ImageBrush ExitButton { get; }
        public ImageBrush ExitButtonOnHover { get; }
        public ImageBrush ExitButtonOnPressed { get; }
        public ImageBrush StartButton { get; }
        public ImageBrush StartButtonOnHover { get; }
        public ImageBrush StartButtonOnPressed { get; }
        public ImageBrush StartButtonUpdating { get; }
        public ImageBrush UpdatingBackground { get; }
        public ImageBrush UpdatingBar { get; }

        public ImageBrush NoticeSelectedIcon { get; }
        public ImageBrush NoticeScrollBackground { get; }
        public ImageBrush NoticeScrollThumb { get; }
        public ImageBrush NoticeScrollArrowUp { get; }
        public ImageBrush NoticeScrollArrowUpOnHover { get; }
        public ImageBrush NoticeScrollArrowUpOnPressed { get; }
        public ImageBrush NoticeScrollArrowDown { get; }
        public ImageBrush NoticeScrollArrowDownOnHover { get; }
        public ImageBrush NoticeScrollArrowDownOnPressed { get; }

        public ImageBrush LanguageDisplayBackground { get; }
        public ImageBrush LanguageButton { get; }
        public ImageBrush LanguageButtonOnHover { get; }
        public ImageBrush LanguageButtonOnPressed { get; }

        public ImageBrush LanguagePopupFrameTop { get; }
        public ImageBrush LanguagePopupFrameMid { get; }
        public ImageBrush LanguagePopupFrameBottom { get; }
        public ImageBrush LanguagePopupCheckbox { get; }
        public ImageBrush LanguagePopupCheckboxChecked { get; }
        public ImageBrush LanguagePopupAcceptButton { get; }
        public ImageBrush LanguagePopupAcceptButtonOnHover { get; }
        public ImageBrush LanguagePopupAcceptButtonOnPressed { get; }
        public ImageBrush LanguagePopupCancelButton { get; }
        public ImageBrush LanguagePopupCancelButtonOnHover { get; }
        public ImageBrush LanguagePopupCancelButtonOnPressed { get; }

        public ImageBrush SettingsFrameTop { get; }
        public ImageBrush SettingsComboboxBackground { get; }
        public ImageBrush SettingsComboboxArrow { get; }
        public ImageBrush SettingsComboboxArrowOnHover { get; }
        public ImageBrush SettingsComboboxArrowOnPressed { get; }
        public ImageBrush SettingsSaveButton { get; }
        public ImageBrush SettingsSaveButtonOnHover { get; }
        public ImageBrush SettingsSaveButtonOnPressed { get; }
        public ImageBrush SettingsCheckbox { get; }
        public ImageBrush SettingsCheckboxChecked { get; }

        public string LinkWebsite { get; }
        #endregion

        #region Constructor
        public LauncherAssets(Pk2Stream reader)
        {
            Background = new ImageBrush(reader.GetImage("launcher_wpf/background.dat"));
            HomeIcon = new ImageBrush(reader.GetImage("launcher_wpf/home_icon.dat"));
            HomeIconOnHover = new ImageBrush(reader.GetImage("launcher_wpf/home_icon_onhover.dat"));

            OptionButton = new ImageBrush(reader.GetImage("launcher_wpf/option_button.dat"));
            OptionButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/option_button_onhover.dat"));
            OptionButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/option_button_onpressed.dat"));
            GuideButton = new ImageBrush(reader.GetImage("launcher_wpf/guide_button.dat"));
            GuideButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/guide_button_onhover.dat"));
            GuideButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/guide_button_onpressed.dat"));
            MovieButton = new ImageBrush(reader.GetImage("launcher_wpf/movie_button.dat"));
            MovieButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/movie_button_onhover.dat"));
            MovieButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/movie_button_onpressed.dat"));
            ExitButton = new ImageBrush(reader.GetImage("launcher_wpf/exit_button.dat"));
            ExitButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/exit_button_onhover.dat"));
            ExitButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/exit_button_onpressed.dat"));
            StartButton = new ImageBrush(reader.GetImage("launcher_wpf/start_button.dat"));
            StartButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/start_button_onhover.dat"));
            StartButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/start_button_onpressed.dat"));
            StartButtonUpdating = new ImageBrush(reader.GetImage("launcher_wpf/start_button_updating.dat"));

            UpdatingBackground = new ImageBrush(reader.GetImage("launcher_wpf/updating_background.dat"));
            UpdatingBar = new ImageBrush(reader.GetImage("launcher_wpf/updating_bar.dat"));

            NoticeSelectedIcon = new ImageBrush(reader.GetImage("launcher_wpf/notice/selected_icon.dat"));
            NoticeScrollBackground = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_background.dat"));
            NoticeScrollThumb = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_thumb.dat"));
            NoticeScrollArrowUp = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_arrow_up.dat"));
            NoticeScrollArrowUpOnHover = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_arrow_up_onhover.dat"));
            NoticeScrollArrowUpOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_arrow_up_onpressed.dat"));
            NoticeScrollArrowDown = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_arrow_down.dat"));
            NoticeScrollArrowDownOnHover = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_arrow_down_onhover.dat"));
            NoticeScrollArrowDownOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/notice/scroll_arrow_down_onpressed.dat"));

            LanguageDisplayBackground = new ImageBrush(reader.GetImage("launcher_wpf/language_display_background.dat"));
            LanguageButton = new ImageBrush(reader.GetImage("launcher_wpf/language_button.dat"));
            LanguageButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/language_button_onhover.dat"));
            LanguageButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/language_button_onpressed.dat"));

            LanguagePopupFrameTop = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/frame_top.dat"));
            LanguagePopupFrameMid = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/frame_mid.dat"));
            LanguagePopupFrameBottom = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/frame_bottom.dat"));
            LanguagePopupCheckbox = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/checkbox.dat"));
            LanguagePopupCheckboxChecked = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/checkbox_checked.dat"));
            LanguagePopupAcceptButton = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/accept_button.dat"));
            LanguagePopupAcceptButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/accept_button_onhover.dat"));
            LanguagePopupAcceptButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/accept_button_onpressed.dat"));
            LanguagePopupCancelButton = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/cancel_button.dat"));
            LanguagePopupCancelButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/cancel_button_onhover.dat"));
            LanguagePopupCancelButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/language_popup/cancel_button_onpressed.dat"));

            SettingsFrameTop = new ImageBrush(reader.GetImage("launcher_wpf/settings/frame_top.dat"));
            SettingsComboboxBackground = new ImageBrush(reader.GetImage("launcher_wpf/settings/combobox_background.dat"));
            SettingsComboboxArrow = new ImageBrush(reader.GetImage("launcher_wpf/settings/combobox_arrow.dat"));
            SettingsComboboxArrowOnHover = new ImageBrush(reader.GetImage("launcher_wpf/settings/combobox_arrow_onhover.dat"));
            SettingsComboboxArrowOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/settings/combobox_arrow_onpressed.dat"));
            SettingsSaveButton = new ImageBrush(reader.GetImage("launcher_wpf/settings/save_button.dat"));
            SettingsSaveButtonOnHover = new ImageBrush(reader.GetImage("launcher_wpf/settings/save_button_onhover.dat"));
            SettingsSaveButtonOnPressed = new ImageBrush(reader.GetImage("launcher_wpf/settings/save_button_onpressed.dat"));
            SettingsCheckbox = new ImageBrush(reader.GetImage("launcher_wpf/settings/checkbox.dat"));
            SettingsCheckboxChecked = new ImageBrush(reader.GetImage("launcher_wpf/settings/checkbox_checked.dat"));

            LinkWebsite = reader.GetFileText("launcher_wpf/link_website.txt");
        }
        #endregion
    }
}