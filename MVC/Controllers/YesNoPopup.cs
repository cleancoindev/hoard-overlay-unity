using System;

namespace Hoard.MVC
{
    /// <summary>
    ///   Popup shown when user have to decide on the following action
    /// </summary>
    public class YesNoPopup : ViewController
    {
        private readonly Action<bool> callBack;

        private string message;

        /// <summary>
        ///   Message to the user
        /// </summary>
        public string Message
        {
            get => message;
            private set
            {
                message = value;
                NotifyChange();
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message to the player</param>
        /// <param name="callback"> Callback invoked on the user decision. Calls with true for Yes and false for No</param>
        public YesNoPopup(string message, Action<bool> callback)
        {
            Message = message;
            this.callBack = callback ?? throw new ArgumentNullException("callback");
        }

        /// <summary>
        ///   Call "yes" decision
        /// </summary>
        public void OnYes()
        {
            callBack(true);
        }

        /// <summary>
        ///   Call "no" decision
        /// </summary>
        public void OnNo()
        {
            callBack(false);
        }
    }
}
