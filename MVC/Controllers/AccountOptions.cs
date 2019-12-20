using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   This controller is in charge of redirecting or setting the account options.
    /// </summary>
    public class AccountOptions : ViewController
    {
        private Profile profile;
        private CredentialsStorage storage = CredentialsStorage.Instance;

        /// <summary>
        ///   Current user profile
        /// </summary>
        public Profile Profile
        {
            get => profile;
            private set
            {
                profile = value;
                NotifyChange();
            }
        }

        private bool accountLogged;

        public bool AccountLogged
        {
            get => accountLogged;
            set
            {
                accountLogged = value;
                NotifyChange();
            }
        }

        /// <summary>
        ///   Ctor
        /// </summary>
        public AccountOptions(Profile profile)
        {
            Title = "TITLE_ACCOUNT_OPTIONS".Translated();
            this.Profile = profile;
            AccountLogged = false;
            Autologin = storage.HasPasswordSaved(profile.ID);
        }

        /// <summary>
        ///   Sets the system to save the current user credentials and sets autologin to the val
        /// </summary>
        public void SetSavePassword(bool val)
        {
            if (val == Autologin) return;
            if (val)
            {
                Navigation.Open(new ProvidePassword(Profile.Name,
                (x, y) =>
                {
                    if (x)
                    {
#pragma warning disable CS4014
                        storage.EncryptAndSaveCredentials(Profile.ID, new DecryptedCredentials(y, Profile.Name));

#pragma warning restore CS4014
                        Autologin = true;
                    }
                }));
            }
            else
            {
                storage.DeleteSavedPasswordFor(profile.ID);
                Autologin = false;
            }
        }

        /// <summary>
        /// Should overlay autologin user
        /// </summary>
        public bool Autologin
        {
            get => HoardSettings.AutoLogin;
            private set
            {
                // For test now
                HoardSettings.AutoLogin = value;
                NotifyChange();
            }
        }
    }
}
