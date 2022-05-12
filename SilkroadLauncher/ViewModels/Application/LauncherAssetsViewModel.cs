using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SilkroadLauncher
{
    /// <summary>
    /// Contains all assets references from media.pk2 file
    /// </summary>
    public class LauncherAssetsViewModel : BaseViewModel
    {
        #region Public Properties
        public ImageBrush Background { get; set; }

        public ImageBrush HomeIcon { get; set; }
        public ImageBrush HomeIconOnHover { get; set; }

        public ImageBrush OptionButton { get; set; }
        public ImageBrush OptionButtonOnHover { get; set; }
        public ImageBrush OptionButtonOnPressed { get; set; }
        public ImageBrush GuideButton { get; set; }
        public ImageBrush GuideButtonOnHover { get; set; }
        public ImageBrush GuideButtonOnPressed { get; set; }
        public ImageBrush MovieButton { get; set; }
        public ImageBrush MovieButtonOnHover { get; set; }
        public ImageBrush MovieButtonOnPressed { get; set; }
        public ImageBrush ExitButton { get; set; }
        public ImageBrush ExitButtonOnHover { get; set; }
        public ImageBrush ExitButtonOnPressed { get; set; }
        public ImageBrush StartButton { get; set; }
        public ImageBrush StartButtonOnHover { get; set; }
        public ImageBrush StartButtonOnPressed { get; set; }
        public ImageBrush StartButtonUpdating { get; set; }
        public ImageBrush UpdatingBackground { get; set; }
        public ImageBrush UpdatingBar { get; set; }

        public ImageBrush NoticeSelectedIcon { get; set; }
        public ImageBrush NoticeScrollBackground { get; set; }
        public ImageBrush NoticeScrollThumb { get; set; }
        public ImageBrush NoticeScrollArrowUp { get; set; }
        public ImageBrush NoticeScrollArrowUpOnHover { get; set; }
        public ImageBrush NoticeScrollArrowUpOnPressed { get; set; }
        public ImageBrush NoticeScrollArrowDown { get; set; }
        public ImageBrush NoticeScrollArrowDownOnHover { get; set; }
        public ImageBrush NoticeScrollArrowDownOnPressed { get; set; }

        public ImageBrush LanguageDisplayBackground { get; set; }
        public ImageBrush LanguageButton { get; set; }
        public ImageBrush LanguageButtonOnHover { get; set; }
        public ImageBrush LanguageButtonOnPressed { get; set; }

        public ImageBrush LanguagePopupFrameTop { get; set; }
        public ImageBrush LanguagePopupFrameMid { get; set; }
        public ImageBrush LanguagePopupFrameBottom { get; set; }
        public ImageBrush LanguagePopupCheckbox { get; set; }
        public ImageBrush LanguagePopupCheckboxChecked { get; set; }
        public ImageBrush LanguagePopupAcceptButton { get; set; }
        public ImageBrush LanguagePopupAcceptButtonOnHover { get; set; }
        public ImageBrush LanguagePopupAcceptButtonOnPressed { get; set; }
        public ImageBrush LanguagePopupCancelButton { get; set; }
        public ImageBrush LanguagePopupCancelButtonOnHover { get; set; }
        public ImageBrush LanguagePopupCancelButtonOnPressed { get; set; }

        public ImageBrush SettingsFrameTop { get; set; }
        public ImageBrush SettingsComboboxBackground { get; set; }
        public ImageBrush SettingsComboboxArrow { get; set; }
        public ImageBrush SettingsComboboxArrowOnHover { get; set; }
        public ImageBrush SettingsComboboxArrowOnPressed { get; set; }
        public ImageBrush SettingsSaveButton { get; set; }
        public ImageBrush SettingsSaveButtonOnHover { get; set; }
        public ImageBrush SettingsSaveButtonOnPressed { get; set; }
        public ImageBrush SettingsCheckbox { get; set; }
        public ImageBrush SettingsCheckboxChecked { get; set; }
        #endregion
    }
}