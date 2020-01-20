using System;
using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;
using System.Linq;

namespace Hoard.MVC
{
    /// <summary>
    ///   Rename User controller
    /// </summary>
    public class RenameUser : ViewController
    {
        private Action<string> callback;

        /// <summary>
        ///   Contructor
        /// </summary>
        /// <param name="userName">Name of profile which name will be changed</param>
        /// <param name="callback">Invoked when the name of the profile changed</param>
        public RenameUser(string userName, Action<string> callback)
        {
            Title = "TITLE_RENAME_USER".Translated();
            CurrentUserName = userName ?? throw new ArgumentNullException(nameof(userName));
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public CredentialInputValidator NewUserName { get; private set; }

        public override void Open()
        {
            NewUserName = new CredentialInputValidator(x =>
            CredentialInputValidator.StandardLengthValidator(x)
                && profileManagement.AvailableProfileNames.Any(y => x == y.userName)
            );
            base.Open();
        }

        public string CurrentUserName { get; }

        private readonly IProfilesManagement profileManagement;

        public void RequestNameChange()
        {
            if (!NewUserName.IsValid) return;
            else
            {
                Navigation.GoBack();
                callback.Invoke(NewUserName.Value);
            }
        }
    }
}