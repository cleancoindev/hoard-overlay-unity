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

        private void Update()
        {
            if (Input.GetKeyDown((BackKeyCode)) ||
                (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName)))
                Back();
        }

        private void Back()
        {
            // We don't close modal views form Global Back
            if (Navigation.ModalView) return;

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