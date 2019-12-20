using Hoard.Utils;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hoard.ProfileUtilities
{
    /// <summary>
    /// General interface for saving and recovering user credentials
    /// when decide to use autologin or save password features
    /// </summary>
    public interface ICredentialStorage
    {
        Task EncryptAndSaveCredentials(HoardID userID, IUserInputProvider credentials);

        Task<string> DecryptPasswordFor(HoardID userID);

        void DeleteSavedPasswordFor(HoardID userID);
    }

    /// <summary>
    /// This object allows to store the encrypted passwords on local drive
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Follows Singleton pattern. Accesible by @Instance static property
    ///   </para>
    ///   <para>
    ///     Please be reminded that storing the users password on the local storage can be dangerous.
    ///   </para>
    /// </remarks>
    public class CredentialsStorage : PasswordProvider, ICredentialStorage
    {
        public string StoragePath { get; private set; }

        /// <summary>
        ///   Private. User static @InitializeStorage for construction
        /// </summary>
        private CredentialsStorage(string storagePath)
        {
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            StoragePath = storagePath;
        }

        private static CredentialsStorage instance;

        /// <summary>
        ///   Singleton instance of the storage
        /// </summary>
        public static CredentialsStorage Instance
        {
            get
            {
                //Default configuration;
                if (instance == null) InitializeStorage(null);
                return instance;
            }
        }

        /// <summary>
        /// Create and initialize singleton.
        /// </summary>
        /// <param name="storagePath">Path to storage if null passed the standard windows path will be used</param>
        public static CredentialsStorage InitializeStorage(string storagePath = null)
        {
            storagePath = storagePath ?? ProfilesManagement.WINDOWS_PROFILES_PATH;
            return instance = new CredentialsStorage(storagePath);
        }

        private readonly byte[] OverlayIV =
            { 0xac, 0x7f, 0xb2,
            0xcc, 0xce, 0xe7,
            0x67, 0xea, 0xdb,
            0xc9, 0xc0, 0x52,
            0x1a, 0x3d, 0xdd, 0xee };

        protected override byte[] ProvideIV()
        {
            return OverlayIV;
        }

        protected override byte[] ProvideEncryptionPhrase()
        {
            return Encoding.UTF8.GetBytes("Beam me up scotty to the Hoard Overlay");
        }

        public async Task EncryptAndSaveCredentials(HoardID userID, IUserInputProvider credentials)
        {
            var pathToFile = PathToID(userID);
            var pass = await credentials.RequestInput("Password", userID, eUserInputType.kPassword, "");
            var encrypted = EncryptPassword(userID, pass);
            using (var file = new StreamWriter(pathToFile, true))
            {
                var converted = await Task.Run(() => JsonConvert.SerializeObject(encrypted));
                await file.WriteAsync(converted);
            }
        }

        private string PathToID(HoardID userID)
            => Path.Combine(StoragePath, userID.ToString()) + ".hse";

        /// <summary>
        ///   Decrypts the password
        /// </summary>
        public async Task<string> DecryptPasswordFor(HoardID userID)
        {
            var pathToFile = PathToID(userID);
            if (!File.Exists(pathToFile)) throw new FileNotFoundException("Storage file not found for " + userID);

            using (var file = new StreamReader(pathToFile, true))
            {
                var buffer = await file.ReadToEndAsync();
                var encrypted = JsonConvert.DeserializeObject<EncryptedData>(buffer);
                var decrypted = DecryptPassword(userID, encrypted);

                return decrypted;
            }
        }

        /// <summary>
        ///   Has the user it's password saved?
        /// </summary>
        public bool HasPasswordSaved(HoardID userID)
            => File.Exists(PathToID(userID));

        /// <summary>
        ///   Clear the password from the local storage
        /// </summary>
        public void DeleteSavedPasswordFor(HoardID userID)
        {
            var pathToFile = PathToID(userID);
            if (!File.Exists(pathToFile)) throw new FileNotFoundException("Storage file not found for " + userID);
            File.Delete(pathToFile);
        }
    }
}
