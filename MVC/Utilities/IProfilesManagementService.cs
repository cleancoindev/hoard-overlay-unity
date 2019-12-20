using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hoard.MVC;
using System;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Hoard.ProfileUtilities
{
    /// <summary>
    /// Implementation of this interface is responsible of tracking the currently signed user
    /// and decoding the private key of user's profile returning Profile that is ready to sign transactions
    /// Implementor of this interface should notify about the change on the active profile,
    /// or available usernames using INotifyProperyChanged interface
    /// </summary>
    public interface IProfilesManagement : INotifyPropertyChanged
    {
        Profile ActiveProfile { get; }

        bool Initialized {get;}

        ObservableCollection<ProfileDescription> AvailableProfileNames { get; }

        Task<Profile> SignInProfile(DecryptedCredentials credentials);

        Task ChangeProfilePassword(DecryptedCredentials oldCredentials, string newPassword);

        Task DeleteProfile(DecryptedCredentials credentials);

        Task<Profile> CreateProfile(DecryptedCredentials credentials);

        void ReceiveProfile(string profileName, string profileID, string profileData);
    }


    /// <summary>
    /// This class holds the credentials in plain form ready to be used for profile decoding etc.
    /// Do not store the information in this class localy.
    /// </summary>
    public class DecryptedCredentials : IUserInputProvider
    {
        public DecryptedCredentials(string encryptedPassword, string userName)
        {
            EncryptedPassword = encryptedPassword ?? throw new ArgumentNullException(nameof(encryptedPassword));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }

        public string EncryptedPassword { get; private set; }
        public string UserName { get; private set; }

        public Task<string> RequestInput(string name, HoardID id, eUserInputType type, string description)
        {
            switch (type)
            {
                case eUserInputType.kLogin:
                    return Task.Run(() => UserName);

                case eUserInputType.kPassword:
                    return Task.Run(() => EncryptedPassword);

                default:
                    throw new System.NotImplementedException("Decrypted credentials can't provide information about " + type.ToString());
            }
        }
    }

    /// <summary>
    /// Stores localy encrypted passwords for profiles
    /// </summary>
    [Serializable]
    public struct EncryptedCredentials
    {
        public readonly string encryptedPassword;
        public readonly string userName;

        public EncryptedCredentials(string encryptedPassword, string userName)
        {
            this.encryptedPassword = encryptedPassword ?? throw new ArgumentNullException(nameof(encryptedPassword));
            this.userName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }

    /// <summary>
    /// Helper base class that simplifies using the INotifyPropertyChanged interface
    /// </summary>
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///   Automatized PropertyChanged event call
        /// </summary>
        public void NotifyChange([CallerMemberName] string name = default)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Creates and stores password encrypted profiles
    /// </summary>
    public class ProfilesManagement : PropertyChangedNotifier, IProfilesManagement
    {
        static private ProfilesManagement instance;

        /// <summary>
        ///   Current instance of singleton
        /// </summary>
        static public ProfilesManagement Instance
        {
            get
            {
                if (instance == null)
                    throw new System.NullReferenceException("Profiles manager sigleton is not instantiated");
                return instance;
            }
        }

        /// <summary>
        ///   Standard path for storing profiles on Windows
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This is a standard path used by Hoard games on the platform. Using it allows user to easly use the hoard profiles in all games across the system
        ///   </para>
        /// </remarks>
        public static readonly string WINDOWS_PROFILES_PATH =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Hoard", "Profiles");

        /// <summary>
        ///   Standard path for storing profiles on Linux
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This is a standard path used by Hoard games on the platform. Using it allows user to easly use the hoard profiles in all games across the system
        ///   </para>
        /// </remarks>
        public static readonly string LINUX_PROFILES_PATH =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".hoard", "Profiles");


        /// <summary>
        /// Hidden so user have to go through the Instance getter that guarantees singleton instatiation
        /// </summary>
        private ProfilesManagement(string ProfilesDir)
        {
            this.ProfilesDir = ProfilesDir;
            if (!Directory.Exists(ProfilesDir))
            {
                Directory.CreateDirectory(ProfilesDir);
            }
        }

        /// <summary>
        ///   Is singleton instance correctly initialized
        /// </summary>
        public bool Initialized {get; private set;}

        public static ProfilesManagement CreateSingleton(string ProfilesDir)
        {
            instance = new ProfilesManagement(ProfilesDir);
            instance.GetProfiles();
            return instance;
        }

        /// <summary>
        ///   Reads the profiles from the storage and store them in AvailableProfileNames property
        /// </summary>
        public void GetProfiles()
        {
            // Enumerate all *.keystore files located in ProfilesDir directory.
            /// NOTE That is hardly easy to use
            ErrorCallbackProvider.ReportWarning("Reading from : " + ProfilesDir);
            Hoard.Utils.KeyStoreUtils.EnumerateProfiles(ProfilesDir, (Func<string, Task>)(async (fileName) =>
            {
                await Task.Yield();
                StreamReader jsonReader = new StreamReader(Path.Combine(ProfilesDir, fileName));
                JObject jobj = JObject.Parse(jsonReader.ReadToEnd());
                jsonReader.Close();
                JToken valueAddress;
                JToken valueName;

                // Valid keystore file should contain address and name.
                if (jobj.TryGetValue("address", out valueAddress) && jobj.TryGetValue("name", out valueName))
                {
                    AvailableProfileNames.Add(new ProfileDescription((string)valueName.Value<string>(), (HoardID)new HoardID((string)valueAddress.Value<string>())));
                }
            })).ContinueGUISynch(x => Initialized = true);
        }

        private Profile activeProfile;

        /// <summary>
        /// Last decoded profile
        /// </summary>
        public Profile ActiveProfile
        {
            get => activeProfile;
            private set
            {
                if (activeProfile != value)
                {
                    activeProfile = value;
                    NotifyChange();
                }
            }
        }

        /// <summary>
        /// The names of the profiles found on the storage
        /// </summary>
        public ObservableCollection<ProfileDescription> AvailableProfileNames { get; private set; }
            = new ObservableCollection<ProfileDescription>();

        /// <summary>
        /// Profiles' Storage path
        /// </summary>
        public string ProfilesDir { get; private set; }

        /// <summary>
        ///   Create new User Profile
        /// </summary>
        public async Task<Profile> CreateProfile(DecryptedCredentials credentials)
        {
            var service = new KeyStoreProfileService(credentials, ProfilesDir);
            return await service.CreateProfile(credentials.UserName);
        }

        /// <summary>
        /// Manually add profile to the available profiles
        /// </summary>
        public void AddProfile(Profile p)
        {
            AvailableProfileNames.Add(new ProfileDescription(p.Name, p.ID));
        }

        /// <summary>
        ///   Change password of the stored profile
        /// </summary>
        public async Task ChangeProfilePassword(DecryptedCredentials oldCredentials, string newPassword)
        {
            var service = new KeyStoreProfileService(oldCredentials, ProfilesDir);
            var user = await RequestProfile(oldCredentials, service);
            await service.ChangePassword(user.ID, oldCredentials.EncryptedPassword, newPassword);
        }

        /// <summary>
        ///   Save the profile provided by external source.
        /// </summary>
        public void ReceiveProfile(string profileName, string profileID, string profileData)
        {
            KeyStoreProfileService.SaveProfileAsync(ProfilesDir, profileData)
                .ContinueGUISynch(x =>
                      AvailableProfileNames.Add(
                            new ProfileDescription(profileName,
                            new HoardID(profileID))));
        }

        /// <summary>
        ///   Deletes profile from the storage
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Remember that profiles belongs to the user and are stored localy only. We discurage recless exposing
        /// This api to the user without proper warning.
        ///   </para>
        /// </remarks>
        public async Task DeleteProfile(DecryptedCredentials credentials)
        {
            var service = new KeyStoreProfileService(credentials, ProfilesDir);
            var user = await RequestProfile(credentials, service);
            await service.DeleteProfile(user.ID);
        }

        /// <summary>
        ///   Loads and decodes the stored profile
        /// </summary>
        private async Task<Profile> RequestProfile(DecryptedCredentials credentials, KeyStoreProfileService service)
        {
            service = service ?? new KeyStoreProfileService(credentials, ProfilesDir);
            var name = await credentials.RequestInput("Name", null, eUserInputType.kLogin, "Login Request");
            var password = await credentials.RequestInput("Password", null, eUserInputType.kPassword, "Login Request");
            return await service.RequestProfile(name, password);
        }

        /// <summary>
        ///   Signs in the user profile
        /// </summary>
        public async Task<Profile> SignInProfile(DecryptedCredentials credentials)
        {
            Profile profile = null;
            try
            {
                profile = await RequestProfile(credentials, null);
            }
            catch (Exception e)
            {
                throw e;
            }

            return profile;
        }
    }

    /// <summary>
    ///   Striped down Profile. Just for the purpose of
    /// basic user identification
    /// Can't sign transaciotn;
    /// </summary>
    public class ProfileDescription
    {
        public readonly string userName;
        public readonly HoardID ID;
        public string IDString => ID.ToString();

        public ProfileDescription(string Name, string hoardID)
        {
            this.userName = Name;
            this.ID = new HoardID(hoardID);
        }

        public ProfileDescription(string Name, HoardID hoardID)
        {
            this.userName = Name;
            this.ID = hoardID;
        }

        public ProfileDescription(Profile profile)
        {
            this.userName = profile.Name;
            this.ID = profile.ID;
        }
    }

    /// <summary>
    /// Thrown when sign in process fails
    /// </summary>
    [Serializable]
    public class SignInFailedException : Exception
    {
        public SignInFailedException()
        {
        }

        public SignInFailedException(string message) : base(message)
        {
        }

        public SignInFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SignInFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
