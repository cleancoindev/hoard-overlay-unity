using System;
using Hoard.ProfileUtilities;

namespace Hoard.MVC
{
    /// <summary>
    /// Singletong where we store overlay important user settings
    /// </summary>
    public class HoardSettings
    {
        /// <summary>
        ///   Key for User name serialization
        /// </summary>
        public const string PREF_LAST_USER_NAME = "HOARD_PREF_LAST_USER_NAME";

        /// <summary>
        ///   Key for User ID serialization
        /// </summary>
        public const string PREF_LAST_USER_ID = "HOARD_PREF_LAST_USER_ID";

        /// <summary>
        ///   Key for autologin setting serialization
        /// </summary>
        public const string PREF_AUTOLOGIN = "HOARD_PREF_AUTOLOGIN";

        private static HoardSettings instance;

        /// <summary>
        ///   Access current singleton instance
        /// </summary>
        public static HoardSettings Instance
        {
            get => instance ?? throw new ArgumentNullException("Hoard settings are not initialiezed, call constructor first" ) ;
        }


        private readonly ISettingsProvider SettingsProvider;

        public HoardSettings(ISettingsProvider settingsProvider, bool reinitializeSingleton = false)
        {
            if (instance != null && !reinitializeSingleton)
                throw new Exception("Attempt to create second instance of singleton HoardPersistentSettings" +
                                    " You can use reinitializeSingleton to force the call if you find it necessary");

            SettingsProvider = settingsProvider;
            instance = this;
        }

        /// <summary>
        ///   Should Hoard automaticaly log user on initialization
        /// </summary>
        public static bool AutoLogin
        {
            get => Instance.SettingsProvider.GetBool(PREF_AUTOLOGIN);
            set => Instance.SettingsProvider.SetBool(PREF_AUTOLOGIN, value);
        }

        /// <summary>
        ///   What was last succesfull loged user profile
        /// </summary>
        public static ProfileDescription LastUser
        {
            get {
                  var id = Instance.SettingsProvider.GetString(PREF_LAST_USER_ID);
                  if (string.IsNullOrEmpty(id)) return null;

                  var name = Instance.SettingsProvider.GetString(PREF_LAST_USER_NAME);
                  return new ProfileDescription(name, id);
                }

            set {
                Instance.SettingsProvider.SetString(PREF_LAST_USER_NAME, value.userName);
                Instance.SettingsProvider.SetString(PREF_LAST_USER_ID, value.ID);
            }
        }
    }

    /// <summary>
    ///   Provides engine / platform specific implementation of data storage
    /// </summary>
    public interface ISettingsProvider
    {
        bool GetBool(string paramName);
        void SetBool(string paramName, bool paramValue);

        void SetString(string paramName, string paramValue);
        string GetString(string paramName);
    }
}
