using System.Collections.ObjectModel;
using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    /// Base for user select and users management list
    /// </summary>
    public class ListUsers : ViewController
    {
        protected IProfilesManagement profilesProvider;

        private ObservableCollection<ProfileDescription> profiles;

        public ObservableCollection<ProfileDescription> Profiles
        {
            get => profiles;
            set
            {
                profiles = value;
                NotifyChange();
            }
        }

        public ListUsers(IProfilesManagement provider)
        {
            Title = "TITLE_USER_LIST".Translated();
            Profiles = provider.AvailableProfileNames;
            profilesProvider = provider;
        }
    }
}
