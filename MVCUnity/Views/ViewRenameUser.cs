using TMPro;
using UnityEngine.UI;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(RenameUser))]
    public class ViewRenameUser : UnityView<RenameUser>
    {
#pragma warning disable IDE0052
        private CredentialsObserver userNameObserver;
#pragma warning restore IDE0052

        private CredentialsTextColors userNameColors;

        public TMP_Text oldUserName;
        public TMP_Text hint;
        public Button changeButton;
        public TMP_InputField userNameInput;

        public void SetNewUserName(string newUserName) =>
            ContextControler.NewUserName.Value = newUserName;

        public override void Open()
        {
            base.Open();
            oldUserName.SetText(ContextControler.CurrentUserName);

            changeButton.interactable = false;

            userNameObserver = new CredentialsObserver(ContextControler.NewUserName)
            {
                onValueChanged = x =>
                {
                    if (x != userNameInput.text) userNameInput.SetTextWithoutNotify(x);
                    changeButton.interactable = ContextControler.NewUserName.IsValid;
                },
                onEmptyValue = x => hint.SetText("HINT_RENAME_USER_NEW".Translated()),
                onInvalidValue = x => hint.SetText("HINT_RENAME_RULES".Translated() + ContextControler.CurrentUserName),
                onValidValue = x => hint.SetText("HINT_RENAME_OK".Translated())
            };

            userNameColors = new CredentialsTextColors(hint, userNameObserver);
        }

        public void RequestChange() =>
            ContextControler.RequestNameChange();
    }
}
