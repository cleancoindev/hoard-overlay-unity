using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hoard.MVC.Utilities
{
    public static class Lockit
    {
        [System.Serializable]
        private class LanguageDictionary
        {
            public string lang = "";
            public Dictionary<string, string> EntriesDictionary { get; private set; } = new Dictionary<string, string>();
        }

        /// <summary>
        ///   Loads the proper dictionary for the language selected.
        /// Return false if it could not be loaded.
        /// Needs to be called after LoadLockitText()
        /// </summary>
        public static bool SetLanguage(string languageCode)
        {
            if (fullLockit == null) return false;

            if (languageDictionary != null && languageDictionary.lang == languageCode) return true;

            if (fullLockit.languages.ContainsKey(languageCode))
            {
                languageDictionary = fullLockit.languages[languageCode];
                return true;
            }
            else return false;
        }

        private static LanguageDictionary languageDictionary;
        private static LockitDict fullLockit;

        [System.Serializable]
        private class LockitDict
        {
            public Dictionary<string, LanguageDictionary> languages = new Dictionary<string, LanguageDictionary>();
        }

        /// <summary>
        ///   Generates the example json string. Use it to create template for the lockit
        /// </summary>
        public static string GenerateExampleText()
        {
            var file = new LockitDict();
            var eng = new LanguageDictionary();
            eng.lang = "en_US";
            eng.EntriesDictionary.Add("OV_EXAMPLE", "This is an example entry in dictionary");
            file.languages.Add("en_US", eng);
            var pol = new LanguageDictionary();
            pol.lang = "pol_PL";
            pol.EntriesDictionary.Add("OV_EXAMPLE", "Przykładowy wpis do lockitu");
            file.languages.Add("pol_PL", pol);
            return JsonConvert.SerializeObject(file, Formatting.Indented);
        }

        /// <summary>
        ///   Loads text to the object and organise it for further use
        /// </summary>
        public static void LoadLockitText(ILockitTextProvider lockitProvider)
        {
            if (lockitProvider == null) throw new System.ArgumentNullException(nameof(lockitProvider));
            var lText = lockitProvider.GetLockitText();
            if (lText == null) throw new System.ArgumentNullException("loaded lockit did not provided text");
            fullLockit = JsonConvert.DeserializeObject<LockitDict>(lText);
        }

        /// <summary>
        ///   Extension method for concise coding 
        /// </summary>
        public static string Translated(this string s, string languageCode = null)
        {
            return GetTranslation(s, languageCode);
        }

        /// <summary>
        ///   Returns the translated text for the Selected language.
        /// </summary>
        public static string GetTranslation(string entry, string languageCode = null)
        {
            if (languageCode == null) languageCode =
                                          languageDictionary == null ?
                                          throw new System.ArgumentException("No standard language selected") :
                                          languageDictionary.lang;
            else if (languageDictionary == null)
            {
                if (fullLockit.languages.ContainsKey(languageCode))
                    languageDictionary = fullLockit.languages[languageCode];
            }

            if (languageDictionary == null)
            {
                ErrorCallbackProvider.ReportError(string.Format("Language of code {0}: not existent in dictionary ", languageCode));
                return entry;
            }

            if (!languageDictionary.EntriesDictionary.ContainsKey(entry))
            {
                return entry;
            }
            return languageDictionary.EntriesDictionary[entry];
        }
    }

    /// <summary>
    ///   Interface for the object that can provided the lockit string to the lockit
    /// </summary>
    public interface ILockitTextProvider
    {
        string GetLockitText();
    }
}
