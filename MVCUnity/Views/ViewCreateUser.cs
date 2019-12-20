using UnityEngine.UI;
using TMPro;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(CreateUser))]
    public class ViewCreateUser : UnityView<CreateUser>
    {
        public TMP_InputField userNameInput, passwordInput, repeatInput;
        public TMP_Text userNameHint, passwordHint, repeatHint;
        public Button createButton;

#pragma warning disable IDE0052 // Remove unread private members
        private CredentialsObserver passwordObserver;
        private CredentialsObserver userNameObserver;
        private CredentialsObserver repeatObserver;

        private CredentialsTextColors passwordColors;
        private CredentialsTextColors userNameColors;
        private CredentialsTextColors repeatColors;

#pragma warning restore IDE0052 // Remove unread private members

        public override void Open()
        {
            base.Open();
            userNameInput.SetTextWithoutNotify("");
            passwordInput.SetTextWithoutNotify("");
            repeatInput.SetTextWithoutNotify("");

            userNameObserver = new CredentialsObserver(ContextControler.NewUserName)
            {
                onEmptyValue = x => userNameHint.SetText("HINT_USER_DESC".Translated()),
                onInvalidValue = x => userNameHint.SetText("HINT_USER_RULES".Translated()),
                onValidValue = x => userNameHint.SetText("HINT_USER_CORRECT".Translated()),
                onValueChanged = x => createButton.interactable = AllCorrect,
            }.Setup();


            repeatObserver = new CredentialsObserver(ContextControler.RepeatedPassword)
            {
                onEmptyValue = x => repeatHint.SetText("HINT_PASSWORD_REPEAT".Translated()),
                onInvalidValue = x => repeatHint.SetText("HINT_PASSWORD_MISMATCH".Translated()),
                onValidValue = x => repeatHint.SetText("HINT_PASSWORD_MATCH".Translated()),
                onValueChanged = x => createButton.interactable = AllCorrect,
            }.Setup();

            passwordObserver = new CredentialsObserver(ContextControler.Password)
                {
                    onEmptyValue = x => passwordHint.SetText("HINT_PASSWORD".Translated()),
                    onInvalidValue = x => passwordHint.SetText("HINT_PASSWORD_RULES".Translated()),
                    onValidValue = x => passwordHint.SetText("HINT_PASSWORD_OK".Translated()),
                    onValueChanged = x =>
                    {
                        createButton.interactable = AllCorrect;
                        repeatObserver.Observed.Validate();
                    }
                }.Setup();
            createButton.interactable = AllCorrect;

            passwordColors = new CredentialsTextColors(passwordHint, passwordObserver);
            userNameColors = new CredentialsTextColors(userNameHint, userNameObserver);
            repeatColors = new CredentialsTextColors(repeatHint, repeatObserver);
        }

        private bool AllCorrect =>
            ContextControler.NewUserName.IsValid
            && ContextControler.Password.IsValid
            && ContextControler.RepeatedPassword.IsValid;

        public void SetUserName(string name) =>
            ContextControler.NewUserName.Value = name;

        public void SetUserPassword(string pass) =>
            ContextControler.Password.Value = pass;

        public void RepeatPassword(string pass) =>
            ContextControler.RepeatedPassword.Value = pass;

        public void RequestCreate()
        {
            ContextControler.RequestCreateAccount();
        }
    }
}
