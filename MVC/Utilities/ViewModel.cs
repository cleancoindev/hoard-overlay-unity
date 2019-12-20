using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hoard.MVC
{
    /// <summary>
    /// Basic IViewModel class.
    /// </summary>
    public class ViewController : IHoardViewController, INotifyPropertyChanged
    {
        private IHoardViewController attachedView;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChange([CallerMemberName]string name = default)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected string title;

        /// <summary>
        ///   Title of the ViewController
        /// </summary>
        public virtual string Title {
            get => title;
            protected set{
                title = value;
                NotifyChange();
            }
        }

        private bool active;

        /// <summary>
        ///   Is Controller activated 
        /// </summary>
        public bool Active
        {
            get => active;
            private set
            {
                active = value;
                NotifyChange();
            }
        }

        private bool visible;

        /// <summary>
        ///   Is Controller visiable to the user
        /// </summary>
        public bool Visible
        {
            get => visible;
            private set
            {
                visible = value;
                NotifyChange();
            }
        }

        /// <summary>
        ///   Connect the ViewController to the view
        /// </summary>
        public ViewController BindView(IHoardViewController controler)
        {
            attachedView = controler;
            return this;
        }

        /// <summary>
        ///   Override to perform tasks when controller closes
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Remember to call base.Close() in the override method for correct behaviour
        ///   </para>
        /// </remarks>
        virtual public void Close()
        {
            Active = false;
            attachedView?.Close();
        }

        /// <summary>
        ///   Override to perform tasks when controller is disabled
        /// Called when controller is no longer visiable to the user
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Remember to call base.Disable() in the override method for correct behaviour
        ///   </para>
        /// </remarks>
        virtual public void Disable()
        {
            Visible = false;
            attachedView?.Disable();
        }

        /// <summary>
        ///   Override to perform tasks when controller is Enabled
        /// Called when controller becomes visiable 
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Remember to call base.Enable() in the override method for correct behaviour
        ///   </para>
        /// </remarks>
        virtual public void Enable()
        {
            Visible = true;
            attachedView?.Enable();
        }

        /// <summary>
        ///   Override to perform tasks when controller Opens
        /// Called when controller becomes visiable 
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Remember to call base.Open() in the override method for correct behaviour
        ///   </para>
        /// </remarks>
        virtual public void Open()
        {
            Active = true;
            attachedView?.Open();
        }
    }
}
