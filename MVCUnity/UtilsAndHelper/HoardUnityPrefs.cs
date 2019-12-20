using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Class that uses unity preferences to save the aesthetic settings
    /// </summary>
    public class HoardUnityPrefs : MonoBehaviour
    {
        public static HoardUnityPrefs Instance {get; private set;}

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning("Second Instance of global prefs, there should be only one available at same time");
                Destroy(this);
            }
        }

        public Color CredentialsValidatorError;
        public Color CredentialsValidatorCorrect;
        public Color CredentialsValidatorStandardColor;
    }
}
