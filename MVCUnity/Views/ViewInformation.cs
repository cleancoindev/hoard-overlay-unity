using TMPro;
using System.ComponentModel;
namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   View for players information
    /// </summary>
    [UnityViewOf(typeof(Information))]
    public class ViewInformation : UnityView<Information>
    {
        public TMP_Text textField;

        public override void Enable()
        {
            base.Enable();
            textField.text = ContextControler.Info;
            ContextControler.PropertyChanged += PropertyChangedEventHandler;
        }

        /// <summary>
        ///   Property change handler
        /// </summary>
        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            textField.text = ContextControler.Info;
        }

        public override void Disable()
        {
            base.Disable();
            ContextControler.PropertyChanged -= PropertyChangedEventHandler;
        }

        /// <summary>
        ///   Call when user confirms information
        /// </summary>
        public void Proceed()
            => ContextControler.Proceed();
    }

}
