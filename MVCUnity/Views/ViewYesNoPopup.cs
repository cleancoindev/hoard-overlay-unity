using TMPro;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(YesNoPopup))]
    public class ViewYesNoPopup : UnityView<YesNoPopup>
    {
        public TMP_Text message;

        public override void Open()
        {
            base.Open();
            message.text = ContextControler.Message;
        }

        public void Yes() => ContextControler.OnYes();

        public void No() => ContextControler.OnNo();
    }
}
