using UnityEngine;
using TMPro;
using System;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Utility class that binds the coloring of the tmp text depending on the state
    /// Of the credentials. Colors are taken from the HoardUnityPrefs if available
    /// </summary>
    public class CredentialsTextColors
    {
        /// <summary>
        ///   Color used when validator is in invalid state
        /// </summary>
        public Color ColorError;

        /// <summary>
        ///   Correct input color
        /// </summary>
        public Color ColorCorrect;

        /// <summary>
        ///   Standard color for the text that awaits input
        /// </summary>
        public Color ColorStandard;

        private TMP_Text text;
        CredentialsObserver observer;

        public CredentialsTextColors(TMP_Text text, CredentialsObserver observer)
        {

            this.text = text ?? throw new ArgumentNullException(nameof(text));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));

            if (HoardUnityPrefs.Instance != null)
            {
                ColorError = HoardUnityPrefs.Instance.CredentialsValidatorError;
                ColorCorrect = HoardUnityPrefs.Instance.CredentialsValidatorCorrect;
                ColorStandard = HoardUnityPrefs.Instance.CredentialsValidatorStandardColor;
            }

            observer.onEmptyValue += x => ColorText(ColorStandard);
            observer.onInvalidValue += x => ColorText(ColorError);
            observer.onValidValue += x => ColorText(ColorCorrect);

            ColorText(ColorStandard);
        }

        ~CredentialsTextColors()
        {
            observer.onEmptyValue += x => ColorText(ColorStandard);
            observer.onInvalidValue += x => ColorText(ColorError);
            observer.onValidValue += x => ColorText(ColorCorrect);
        }

        public void ColorText(Color color)
            => text.color = color;
    }
}
