using UnityEngine;
using UnityEngine.UI;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Button that can be selected in the context of the content lists 
    /// This button resizes when selected
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SelectableButton : MonoBehaviour, ISelectedUser
    {
        public Button button;

        [Range(0, 1f)]
        public float selectedResize = 0.2f;

        private Vector3 originalScale = Vector3.one;

        private Vector3 gotoScale = Vector3.one;

        public Canvas myCanvas;
        private int startSortingOrder;

        public string ElementName;

        private void Update()
        {
            if (gotoScale != transform.localScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, gotoScale, 3f * Time.deltaTime);
            }
        }

        protected void OnEnabled()
        {
            if (button.interactable) OnBeingSelected();
        }

        private void Awake()
        {
            myCanvas = myCanvas ?? GetComponent<Canvas>();
        }

        protected void Start()
        {
            startSortingOrder = myCanvas.sortingOrder;
        }

        public void OnBeingDeselected()
        {
            SetSortinAndLayer(false);
            button.interactable = false;
        }

        private void SetSortinAndLayer(bool selected)
        {
            myCanvas.sortingOrder = selected ? 3 : 2;
            gotoScale = selected ?
                originalScale * (1f + selectedResize) :
                originalScale;
        }

        public void OnBeingSelected()
        {
            SetSortinAndLayer(true);
            button.interactable = true;
            Debug.Log("Selected is :" + name);
            UIUtility.ForceSelection(this);
        }

        // Can't set the the interactable durin deselection event so there is a delay of on frame;
        private System.Collections.IEnumerator DelayInteractableSet(bool val)
        {
            yield return null;
            button.interactable = val;
        }
    }

    public class GetComponentAttribute : PropertyAttribute
    {
    }
}
