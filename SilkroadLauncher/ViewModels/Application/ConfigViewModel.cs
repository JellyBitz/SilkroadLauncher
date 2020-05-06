using SilkroadLauncher.SilkroadCommon;
using System.Collections.Generic;

namespace SilkroadLauncher
{
    public class ConfigViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// The basic config used to start the client
        /// </summary>
        private Config m_Config = new Config();
        #endregion

        #region Public Properties
        /// <summary>
        /// Game resolution
        /// </summary>
        public Config.WindowResoltuion WindowResolution
        {
            get { return m_Config.Resoltuion; }
            set
            {
                m_Config.Resoltuion = value;
                OnPropertyChanged(nameof(WindowResolution));
            }
        }
        /// <summary>
        /// Game resolutions supported
        /// </summary>
        public List<Config.WindowResoltuion> SupportedResolutions { get; } = new List<Config.WindowResoltuion>()
        {
            new Config.WindowResoltuion(800,600),
            new Config.WindowResoltuion(1024,768)
        };
        #endregion


        #region Public Methods
        /// <summary>
        /// Try to load the config from path specified
        /// </summary>
        public bool Load(string Path)
        {


            return true;
        }
        /// <summary>
        /// Save the current config to path specified
        /// </summary>
        public void Save(string Path)
        {
        
        }
        #endregion
    }
}
