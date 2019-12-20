using UnityEngine;

/// <summary>
/// Helper class providing consisten Navigation 'go back to previous view' behaviour
/// </summary>
namespace Hoard.MVC.Unity
{
    public class GlobalbackInput : MonoBehaviour
    {
        public KeyCode BackKeyCode;
        public string buttonName;

        private bool cancelThisFrame;
        void Update()
        {
            if (Input.GetKeyDown((BackKeyCode)) ||
                (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName)))
                Back();
        }

        private void Back()
        {
            if (cancelThisFrame) return;
            cancelThisFrame = true;
            Navigation.GoBack();
        }

        private void LateUpdate()
        {
            cancelThisFrame = false;
        }
    }
}
