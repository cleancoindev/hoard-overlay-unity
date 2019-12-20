using Hoard.ProfileUtilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   Opens and exposes information about the currently logged user
    /// </summary>
    public class CurrentUser : ViewController
    {
        private Profile profile;

        /// <summary>
        ///   Currently observed profile
        /// </summary>
        public Profile Profile
        {
            get => profile;
            set
            {
                profile = value;
                NotifyChange();
            }
        }

        /// <summary>
        ///   Ctor
        /// </summary>
        public CurrentUser(Profile profile)
        {
            this.profile = profile;
            ProfilesManagement.Instance.PropertyChanged += HandleProfilesChanged;
        }

        /// <summary>
        ///   Handles the change of the profile when the controller is still open
        /// </summary>
        private void HandleProfilesChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IProfilesManagement.ActiveProfile))
            {
                Profile = ProfilesManagement.Instance.ActiveProfile;
            }
        }

        /// <summary>
        ///   Open the options related to the current user
        /// </summary>
        public void OpenAccountOptions()
        {
            Navigation.Open(new AccountOptions(profile));
        }
    }
}
