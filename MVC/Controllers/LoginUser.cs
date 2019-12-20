using System.Threading.Tasks;
using Hoard.Exceptions;
using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   Login user controller
    /// </summary>
    public class LoginUser : ProvidePassword
    {

        public LoginUser(string name) : base(name , null)
        {
        }

        override protected void ResolveLogin(Task<Profile> task)
        {
            if (task.IsFaulted)
            {
                Navigation.GoBack();
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
                if (HoardSettings.AutoLogin)
                {
                    if (HoardSettings.LastUser.userName != task.Result.Name)
                    {
                        HoardSettings.AutoLogin = false;
                    }
                }
                HoardSettings.LastUser = new ProfileDescription(task.Result);
                Navigation.OpenControlerWithNewRoot(new CurrentUser(task.Result));
            }
        }
    }
}
