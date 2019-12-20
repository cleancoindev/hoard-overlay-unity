using TMPro;
using UnityEngine;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Class that automatically traslates the Lockit key in the TMP_Text field
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_Text_Translation : MonoBehaviour
    {
        public TMP_Text text;
        void Start()
        {
            text = text?? GetComponent<TMP_Text>();

            var tText = text.text.Translated();
            if (tText != text.text)
            {
                text.text = tText;
            }
        }
    }
}
