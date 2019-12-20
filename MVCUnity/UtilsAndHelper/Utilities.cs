using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
namespace Hoard.MVC.Unity
{
    public static class UnityExtensions
    {
        public static IEnumerator DelayByFrames(this IEnumerator ie, int frameCount)
        {
            while (frameCount-- > 0) yield return null;
            yield return ie;
        }
    }
    public static class UIUtility
    {

        /// <summary>
        ///   Forces the selection of the object after one frame using
        /// TaskRunner as a coroutine context. Use when you really need something selected.
        /// Works around the bug that sometimes prevent selectable to be selected on OnEnabled event
        /// </summary>
        public static void ForceSelection(MonoBehaviour toSelect)
        {
            if (toSelect.gameObject.activeInHierarchy)
            {
                toSelect.StartCoroutine(ForceSelectionRoutine(toSelect.gameObject));
            }
            else
            {
                Debug.LogWarning(toSelect.name + " is not active. Attempt to activate the same frame");
                Selection(toSelect.gameObject);
            }
        }

        private static void Selection(GameObject toSelect)
        {
            var eventS = EventSystem.current;
            if (eventS.currentSelectedGameObject == toSelect)
            {
                eventS.SetSelectedGameObject(null);
            }
            eventS.SetSelectedGameObject(toSelect);
        }

        private static IEnumerator ForceSelectionRoutine(GameObject toSelect)
        {
            yield return null;
            Selection(toSelect);
        }
    }
}
