using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   Controler that is used to list user and allow to import or create new one
    /// </summary>
    public class UsersOverview : ListUsers
    {
        private bool canCreateNew;

        public bool CanCreateNew
        {
            get => canCreateNew;
            private set
            {
                canCreateNew = value;
                NotifyChange();
            }
        }

        private bool canImport;

        public bool CanImport
        {
            get
            {
                return canImport;
            }
            set
            {
                canImport = value;
                NotifyChange();
            }
        }

        public UsersOverview(IProfilesManagement provider) : base(provider)
        {
        }

        public void StartCreateNew()
        {
            Navigation.Open(new Information("INFO_ACCOUNT".Translated(),
                                            () => Navigation.Open(new CreateUser(ProfilesManagement.Instance))));
        }

        public void OpenUserOptions(Profile profile)
        {
            Navigation.Open(new AccountOptions(profile));
        }
    }
}
