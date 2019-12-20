using UnityEngine;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Reads json lockit from TextAsset and load it to lockit
    /// </summary>
    public class HoardLockitUnity : MonoBehaviour, ILockitTextProvider
    {
        public TextAsset lockitFile;

        public string GetLockitText()
        {
            return lockitFile?.text;
        }

        /// <summary>
        ///   Override or rewrite to match your languages needs
        /// </summary>
        public virtual string ToLangCode(SystemLanguage lang)
        {
            switch (lang)
            {
                case SystemLanguage.Polish:
                    return "pl_PL";

                default:
                    return "en_US";
            }
        }

        public void Awake()
        {
            Lockit.LoadLockitText(this);
            var language = Application.systemLanguage;
            Lockit.SetLanguage(ToLangCode(language));
        }
    }
}