using Hoard.Exceptions;
using System;
using System.Threading.Tasks;
using Hoard.MVC.Utilities;
using Hoard.ProfileUtilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   Controller that recives and validates users profile decoding credentials 
    /// </summary>
    public class ProvidePassword : ViewController
    {
        /// <summary>
        ///   Validator for the user password
        /// </summary>
        public CredentialInputValidator PasswordValidator { get; private set; }
        public string UserName { get; private set; }

        private string errorMessage;

        /// <summary>
        ///   Expose the error messages from the system
        /// </summary>
        public string ErrorMessage
        {
            get => errorMessage;
            protected set
            {
                errorMessage = value;
                NotifyChange();
            }
        }

        /// <summary>
        /// ProvidePassword
        /// </summary>
        public ProvidePassword(string name, Action<bool, string> onLogin)
        {
            OnLogin = onLogin;

            UserName = name;

            Title = "TITLE_PROVIDE_PASSWORD".Translated();
            PasswordValidator =
                new CredentialInputValidator(CredentialInputValidator.StandardLengthValidator);
        }

        /// <summary>
        /// Callback invoked after HoardService finish login porcedure
        /// </summary>
        public Action<bool, string> OnLogin { get; }

        public void RequestLogin()
        {
            if (PasswordValidator.IsEmpty)
            {
                ErrorMessage = "ERROR_INVALID_PASSWORD".Translated();
                return;
            }

            if (PasswordValidator.IsIncorrect)
            {
                ErrorMessage = "ERROR_INCORRECT_PASS_FORMAT".Translated() ;
                return;
            }
            var wait = new WaitForTask();
            var task = Task.Run<Profile>(
                                         () => ProfilesManagement.Instance.SignInProfile(
                                                                                         new DecryptedCredentials(PasswordValidator.Value, UserName))
                                         );

            TaskRunner.RunWithCallback(task, ResolveLogin);
            Navigation.Open(wait);
        }

        virtual protected void ResolveLogin(Task<Profile> task)
        {
            Navigation.GoBack();
            // Go back from wait screen
            if (task.IsFaulted)
            {
                var mainExcept = task.Exception.GetBaseException();

                if (mainExcept is ProfileNotFoundException)
                {
                    ErrorMessage = "ERROR_LOGIN_PROFILE_NOT_FOUND".Translated();
                }
                else if (mainExcept is CredentialsException)
                {
                    ErrorMessage = "ERROR_INVALID_PASSWORD".Translated();
                    PasswordValidator.Value = string.Empty;
                }
            }
            else
            {
                Navigation.GoBack();
                if (HoardSettings.AutoLogin)
                {
                    if (HoardSettings.LastUser.userName != task.Result.Name)
                    {
                        HoardSettings.AutoLogin = false;
                    }
                }
                HoardSettings.LastUser = new ProfileDescription(task.Result);
                OnLogin?.Invoke(true, PasswordValidator.Value);
            }
        }
    }
}
