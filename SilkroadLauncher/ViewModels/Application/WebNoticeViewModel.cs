using SRO.Data.Gateway;

namespace SilkroadLauncher
{
    public class WebNoticeViewModel : BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// Notice title
        /// </summary>
        public string Subject { get; }
        /// <summary>
        /// Notice in Html format
        /// </summary>
        public string Article { get; }
        /// <summary>
        /// Date of the notice
        /// </summary>
        public string Date { get; }
        #endregion

        #region Constructor
        public WebNoticeViewModel(WebNotice WebNotice)
        {
            Subject = WebNotice.Subject;

            // Fix if doesn't have html/html5 wrapper (to set correctly the CSS)
            if (!WebNotice.Article.ToUpperInvariant().StartsWith("<!DOCTYPE HTML"))
                Article = "<!DOCTYPE html><html><body>" + WebNotice.Article.Replace("\r\n","<br>") + "</body></html>";
            else
                Article = WebNotice.Article;

            Date = WebNotice.Date.ToString(LauncherSettings.APP_WEBNOTICE_TIME_FORMAT);
        }
        #endregion
    }
}
