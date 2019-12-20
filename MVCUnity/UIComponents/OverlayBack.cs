using UnityEngine;
using UnityEngine.UI;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   UI Component that invokes Navigation.GoBackTimes method
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class OverlayBack : MonoBehaviour
    {
        /// <summary>
        ///   How much views Navigation should go back
        /// </summary>
        public int goBackTimes = 1;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() =>
                Navigation.GoBackTimes(goBackTimes));
        }
    }
}
