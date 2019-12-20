using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hoard.MVC.Unity
{
    /// <summary>
    /// Helper for enforcing the vertical navigation with the game pads
    /// </summary>
    public class MyVerticalNavigation : MonoBehaviour
    {
        public Selectable elementAbove, elementBelow;
        private EventSystem eSys;
        public void Start()
        {
            eSys = EventSystem.current;
        }
        public void Update()
        {
            var vert = Input.GetAxis("Vertical") != 0 || Input.GetButtonDown("Vertical");
            if (eSys.currentSelectedGameObject != gameObject) return;
            if (!vert) return;
            if (Input.GetAxis("Vertical") > 0.01f)
            {
                if (elementAbove != null)
                  eSys.SetSelectedGameObject(elementAbove.gameObject);
            }
            else if (Input.GetAxis("Vertical") < -0.1f)
            {
                if (elementBelow != null)
                  eSys.SetSelectedGameObject(elementBelow.gameObject);
            }
        }
    }
}
