using System;
using System.Windows.Input;

namespace SilkroadLauncher
{
    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    public class RelayParameterizedCommand : ICommand
    {
        #region Private Members
        /// <summary>
        /// The action to run
        /// </summary>
        private Action<object> m_Action;
        #endregion

        #region Public Events
        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayParameterizedCommand(Action<object> Action)
        {
            m_Action = Action;
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            m_Action(parameter);
        }
        #endregion
    }
}
