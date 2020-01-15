using UnityEngine;
using Hoard.ProfileUtilities;
using System.Threading.Tasks;
using System.IO;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Example of the script that start whole overlay system.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     If you want to override debug initialization of unity overlay change the @Update() method
    ///   </para>
    ///   <para>
    ///     It's a good practice to set early initialization of the start scripts
    /// You can set them in UnityEditor. Go to Editor/ProjectSettings/ScriptsExceution to do that
    ///   </para>
    /// </remarks>
    public class TestStarter : MonoBehaviour
    {

        public static string UNIVERSAL_UNITY_PATH =>
            Path.Combine(Application.persistentDataPath, "Hoard", "Profiles");
        private void Awake()
        {
            var storagePath =
#if UNITY_STANDALONE_WIN
                ProfilesManagement.WINDOWS_PROFILES_PATH;
#elif UNITY_STANDALONE_LINUX
            ProfilesManagement.LINUX_PROFILES_PATH;
#else
            Path.Combine(Application.persistentDataPath, "Hoard", "Profiles");
#endif
            // Singletons initialization
            new HoardSettings(new UnitySettingsPersistency());
            ProfilesManagement.CreateSingleton(storagePath);
            CredentialsStorage.InitializeStorage(storagePath);

        }

        public void Update()
        {
            if (Navigation.CurrentController == null)
            {
                if (Input.GetKeyDown(KeyCode.F11))
                  Start();
            }
        }

        bool AutologLastUser()
        {
            var id = HoardSettings.LastUser.ID;
            if (CredentialsStorage.Instance.HasPasswordSaved(id))
            {
                Navigation.Open(new WaitForTask());

                CredentialsStorage.Instance.DecryptPasswordFor(id)
                    .ContinueGUISynch(AfterPassword);
                return true;
            }
            return false;
        }

        void AfterPassword(Task<string> passTask)
        {
            if (!passTask.IsFaulted)
            {
                var credentials = new DecryptedCredentials(passTask.Result, HoardSettings.LastUser.userName);

                ProfilesManagement.Instance
                    .SignInProfile(credentials)
                    .ContinueGUISynch(x =>
                    {
                        if (x.IsFaulted || x.IsCanceled)
                            FailedAutologin();
                        else
                          Navigation.OpenControlerWithNewRoot(new CurrentUser(x.Result));
                    });
            }
        }

        private void FailedAutologin()
        {
            HoardSettings.AutoLogin = false;
            Navigation.GoBack();
            Navigation.Open(new Information("HINT_AUTOLOGIN_FAILED".Translated(), OpenUsersOverview));
        }

        private void OpenUsersOverview()
        {
            Debug.Log("Start: List");
            When.True(() => ProfilesManagement.Instance.Initialized,
                      () => Navigation.Open(new UsersOverview(ProfilesManagement.Instance)));
        }

        private void Start()
        {
            Debug.Log("Start");
            if (HoardSettings.AutoLogin && HoardSettings.LastUser != null)
            {
                    if (!AutologLastUser())
                    {
                        OpenUsersOverview();
                    }
                    else
                      Debug.Log("Start: Attempting autologin");
            }
            else
            {
                OpenUsersOverview();
            }
        }
    }
}
