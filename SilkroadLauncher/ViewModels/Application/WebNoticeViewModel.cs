using SilkroadCommon;
using System;
namespace SilkroadLauncher
{
    public class WebNoticeViewModel: BaseViewModel
    {
        #region Private Members
        // <summary>
        // The silkroad notice message structure
        // </summary>
        // private WebNotice m_WebNotice;
        #endregion

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
            if (WebNotice.Article.Length < 14 || !WebNotice.Article.ToUpper().StartsWith("<!DOCTYPE HTML"))
                Article = "<!DOCTYPE html><html><body>" + WebNotice.Article + "</body></html>";
            else
                Article = WebNotice.Article;

            Date = new DateTime(WebNotice.Year, WebNotice.Month, WebNotice.Day).ToString("dd MMMM yyyy");
        }
        #endregion
    }
}
