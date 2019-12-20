using UnityEngine;
using TMPro;
using Hoard.ProfileUtilities;

namespace Hoard.MVC.Unity
{
    public class UserListButton : MonoBehaviour
    {
        public TMP_Text userName;
        public ProfileDescription Profile { get; private set; }

        public void SetForProfile(ProfileDescription profile)
        {
            this.Profile = profile;
            userName.SetText(profile.userName);
            gameObject.SetActive(true);
        }

        public void OpenOptions()
        {
            Navigation.Open(new LoginUser(Profile.userName));
        }
    }
}
