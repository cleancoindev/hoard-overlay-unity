using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Hoard.MVC
{
    /// <summary>
    ///   The utility class that observes the other object and invoke callbacks when changed property fits the description
    /// </summary>
    public class PropertyChangeHandler<T>
        : Dictionary<string, Action<T>>, IDisposable
        where T : class, INotifyPropertyChanged
    {
        private readonly T observed;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="observed"> Object to boserve</param>
        /// <param name="refreshAtStart">Should object call all passed hadlers at start</param>
        public PropertyChangeHandler(T observed) : base()
        {
            this.observed = observed;
            observed.PropertyChanged += HandleChange;
        }

        /// <summary>
        ///   Sets the this object according to the current state of the observed object
        /// </summary>
        public void Setup()
        {
            foreach (var v in Values)
            {
                v?.Invoke(observed);
            }
        }

        public void Dispose()
        {
            if (observed != null)
            {
                observed.PropertyChanged -= HandleChange;
            }
        }

        private void HandleChange(object sender, PropertyChangedEventArgs e)
        {
            var senderT = sender as T;
            if (ContainsKey(e.PropertyName))
            {
                if (TryGetValue(e.PropertyName, out Action<T> action))
                {
                    action?.Invoke(senderT);
                }
            }
        }
    }

    /// <summary>
    ///   Defines basic functionality for the hoard controllers
    /// </summary>
    public interface IHoardViewController
    {
        /// <summary>
        ///   The title of the controller
        /// </summary>
        string Title {get;}

        /// <summary>
        ///   Call when controller is now active ad ready to receive user input
        /// </summary>
        void Enable();

        /// <summary>
        ///   Called when object is temporarily disabled or just before closed
        /// </summary>
        void Disable();

        /// <summary>
        ///   Called when object is closed and removed form the navigation queue
        /// </summary>
        void Close();

        /// <summary>
        ///   Called when controller is opened by navigation. Should perform all initialization tasks that depends on other objects
        /// but can't be done during the construction, or if calling constructor is impossible (Unity);
        /// </summary>
        void Open();
    }
}
