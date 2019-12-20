using TMPro;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(LoginUser))]
    public class ViewLoginUser : UnityView<LoginUser>
    {
        public TMP_Text userName;
        public TMP_InputField password;
        public TMP_Text errorMessage;

        public override void Enable()
        {
            base.Enable();
            errorMessage.SetText(string.Empty);
            userName.SetText(ContextControler.UserName);
            password.SetTextWithoutNotify("");
            ContextControler.PropertyChanged += OnPropertyChanged;
        }

        public override void Disable()
        {
            base.Disable();
            ContextControler.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginUser.ErrorMessage))
            {
                errorMessage.SetText("PASSWORD_INCORRECT".Translated());
                password.onFocusSelectAll = true;
                password.Select();
            }
        }

        public override void Open()
        {
            base.Open();
            password.Select();
        }

        public void SetPassword(string pass)
            => ContextControler.PasswordValidator.Value = pass;

        public void RequestLogin() => ContextControler.RequestLogin();
    }
}
