using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Implements ISettingsProvider for the nees of the unity.
    /// </summary>
    public class UnitySettingsPersistency : ISettingsProvider
    {
        public bool GetBool(string paramName) => PlayerPrefs.GetInt(paramName) > 0;
        public void SetBool(string paramName, bool paramValue)
            => PlayerPrefs.SetInt(paramName , paramValue ? 1 : 0);

        public string GetString(string paramName) => PlayerPrefs.GetString(paramName);
        public void SetString(string paramName, string paramValue)
            => PlayerPrefs.SetString(paramName, paramValue);

    }
}

