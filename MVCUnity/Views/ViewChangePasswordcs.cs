using TMPro;
using UnityEngine.UI;
using System.ComponentModel;
using Hoard.MVC.Utilities;


namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(ChangePassword))]
    internal class ViewChangePasswordcs : UnityView<ChangePassword>
    {
#pragma warning disable IDE0052, CS0649 // UNITY set
        public TMP_InputField oldPassInput, newPassInput, repeatPassInput;

        public TMP_Text oldPasswordHint, passwordHint, repeatHint;

        private CredentialsObserver passwordObserver;
        private CredentialsObserver oldPasswordObserver;
        private CredentialsObserver repeatObserver;

        private CredentialsTextColors passwordColors;
        private CredentialsTextColors oldPasswordColors;
        private CredentialsTextColors repeatColors;

        public Button changeButton;
#pragma warning restore IDE0052, CS0649 // Remove unread private members

        private bool AllCorrect =>
            ContextControler.oldPassword.IsValid
            && ContextControler.newPassword.IsValid
            && ContextControler.repeatNewPassword.IsValid;

        public override void Enable()
        {
            base.Enable();
            changeButton.interactable = AllCorrect;
        }

        public override void Close()
        {
            base.Close();
            ContextControler.PropertyChanged -= OnErrorMessage;
        }

        public override void Open()
        {
            base.Open();
            oldPassInput.SetTextWithoutNotify("");
            newPassInput.SetTextWithoutNotify("");
            repeatPassInput.SetTextWithoutNotify("");

            oldPasswordObserver = new CredentialsObserver(ContextControler.oldPassword)
            {
                onValueChanged = x =>
                {
                    changeButton.interactable = AllCorrect;
                    if (oldPasswordHint.text != string.Empty)
                    {
                        oldPasswordHint.SetText("HINT_PASSWORD_OLD".Translated());
                    }
                }
            }.Setup();

            ContextControler.PropertyChanged += OnErrorMessage;

            repeatObserver = new CredentialsObserver(ContextControler.repeatNewPassword)
            {
                onEmptyValue = x => repeatHint.SetText("HINT_PASSWORD_REPEAT".Translated()),
                onInvalidValue = x => repeatHint.SetText("HINT_PASSWORD_MISMATCH".Translated()),
                onValidValue = x => repeatHint.SetText("HINT_PASSWORD_MATCH".Translated()),
                onValueChanged = x => changeButton.interactable = AllCorrect,
            }.Setup();

            passwordObserver = new CredentialsObserver(ContextControler.newPassword)
            {
                onEmptyValue = x => passwordHint.SetText("HINT_PASSWORD_NEW".Translated()),
                onInvalidValue = x => passwordHint.SetText("HINT_PASSWORD_RULES".Translated()),
                onValidValue = x => passwordHint.SetText("HINT_PASSWORD_OK".Translated()),
                onValueChanged = x =>{
                    repeatObserver.Observed.Validate();
                    changeButton.interactable = AllCorrect;
                }
            }.Setup();

            changeButton.interactable = AllCorrect;

            //Colors
            repeatColors = new CredentialsTextColors(repeatHint, repeatObserver);
            passwordColors = new CredentialsTextColors(passwordHint, passwordObserver);
            oldPasswordColors = new CredentialsTextColors(oldPasswordHint, oldPasswordObserver);
        }

        private void OnErrorMessage(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ContextControler.ErrorMesage))
            {
                oldPasswordHint.SetText(ContextControler.ErrorMesage);
                oldPasswordColors.ColorText(oldPasswordColors.ColorError);
            }
        }

        public void SetOldPassword(string password)
            => ContextControler.oldPassword.Value = password;

        public void SetNewPassword(string password)
            => ContextControler.newPassword.Value = password;

        public void RepeatNewPassword(string password)
            => ContextControler.repeatNewPassword.Value = password;

        public void TrySetPassword()
            => ContextControler.RequestPasswordChange();
    }
}
