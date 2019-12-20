using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Switches the active UI element on user action
    /// Example would be 'tab' around the elements or move forward when player presses enter
    /// </summary>
    public class GoToElement : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Selectable previousElement;
        public Selectable nextElement;

        /// <summary>
        ///   React to the InputManager input name
        /// </summary>
        public string unityInputName = string.Empty;

        /// <summary>
        ///   React to this keyCode
        /// </summary>
        public KeyCode unityInput = KeyCode.Return;

        private bool poolEvent;
        private Coroutine poolRoutine;
        private bool isSelected;

        public void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
            if (poolRoutine != null)
            {
                StopCoroutine(poolRoutine);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            isSelected = true;
            poolRoutine = StartCoroutine(PoolInput());
        }

        /// <summary>
        ///   Checks if the button or input is pressed so we should navigate to the next element
        /// </summary>
        public bool ActionPressed =>
                Input.GetButtonDown(unityInputName) || Input.GetKeyDown(unityInput);

        public bool ShiftDown =>
            Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift);

        private IEnumerator PoolInput()
        {
            while (isSelected)
            {
                if (ActionPressed)
                {
                    // Parameters for alternate movement
                    if (ShiftDown)
                    {
                        previousElement?.Select();
                    }
                    else
                    {
                        nextElement?.Select();
                    }
                }
                yield return null;
            }
        }
    }

    /// <summary>
    ///   Flags enum that should be treated as Flag
    /// </summary>
    public class EnumFlagAttribute : PropertyAttribute
    {
        public string name;

        public EnumFlagAttribute()
        {
        }

        public EnumFlagAttribute(string name)
        {
            this.name = name;
        }
    }
}
