using System.Threading.Tasks;
using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;
using Hoard.Exceptions;

namespace Hoard.MVC
{
    /// <summary>
    ///   Controller for the changing user passwords
    /// </summary>
    public class ChangePassword : ViewController
    {
        private Profile profile;
        private IProfilesManagement profileManager;

        public CredentialInputValidator oldPassword {get; private set;}
        public CredentialInputValidator newPassword {get; private set;}
        public CredentialInputValidator repeatNewPassword {get; private set;}

        private string errorMessage;

        /// <summary>
        ///   If there is an error encountered it will be exposed through this property
        /// </summary>
        public string ErrorMesage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                NotifyChange();
            }
        }

        /// <summary>
        /// Ctor
        /// <param name="profile">Subject for the password change</param>
        /// <param name="profileManager">profile manager that will be used for the operations.
        /// If null passed standard hoard profile manager will be used</param>
        /// </summary>
        public ChangePassword(Profile profile, IProfilesManagement profileManager = null)
        {
            this.profile = profile;
            this.profileManager = profileManager ?? ProfilesManagement.Instance;

            Title = "TITLE_CHANGE_PASS".Translated();

            oldPassword = new CredentialInputValidator(x => !string.IsNullOrEmpty(x));
            newPassword = new CredentialInputValidator(CredentialInputValidator.StandardLengthValidator);
            repeatNewPassword = new CredentialInputValidator(x => x == newPassword.Value);
        }

        /// <summary>
        /// Sends the request for the password change based on the provided data
        /// </summary>
        public void RequestPasswordChange()
        {
            InternalPasswordChange(new ProfileDescription(profile.Name, profile.ID));
        }

        private void AfterLogin(Task t)
        {
            if (t.IsFaulted)
            {
                if (t.Exception.GetBaseException().GetType() == typeof(CredentialsException))
                {
                    ErrorMesage = "ERROR_INVALID_PASSWORD".Translated();
                }
            }
            else
            {
                // Disabling autologin that have wrong password currently
                ErrorMesage = null;
                HoardSettings.AutoLogin = false;
                Navigation.GoBack();
            }
            Navigation.GoBack();
        }

        private void InternalPasswordChange(ProfileDescription userName)
        {
            if (newPassword.IsValid && repeatNewPassword.IsValid)
            {
                var credentials = new DecryptedCredentials(oldPassword.Value, userName.userName);

                profileManager.ChangeProfilePassword(credentials, newPassword.Value)
                    .ContinueGUISynch(AfterLogin);
                Navigation.Open(new WaitForTask());
            }
        }
    }
}
