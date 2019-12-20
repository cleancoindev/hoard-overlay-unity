using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Element that will be forced selected when component become enabled
    /// </summary>
    public class StartElement : MonoBehaviour
    {

        public void OnEnable()
        {
            UIUtility.ForceSelection(this);
        }
    }
}
