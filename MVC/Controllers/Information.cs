using System;

namespace Hoard.MVC
{
    /// <summary>
    ///   View controller that expose information to the player
    /// </summary>
    public class Information : ViewController
    {
        private string info;

        /// <summary>
        ///   Information to display user
        /// </summary>
        public string Info
        {
            get => info;
            set
            {
                info = value;
                NotifyChange();
            }
        }

        /// <summary>
        ///   Constructor
        /// </summary>
        /// <param name="information">Information to display</param>
        /// <param name="onClose">Callback for closing view event</param>
        public Information(string information, Action onClose)
        {
            if (onClose == null) throw new System.ArgumentNullException(nameof(onClose));
            if (string.IsNullOrEmpty(information)) throw new System.ArgumentNullException(nameof(information));

            info = information;
            this.onClose = onClose;
        }

        /// <summary>
        ///   Action performed when view is closed by the  user.
        /// </summary>
        private readonly Action onClose;

        public void Proceed()
        {
            Navigation.CloseTopView();
            onClose?.Invoke();
        }
    }
}
