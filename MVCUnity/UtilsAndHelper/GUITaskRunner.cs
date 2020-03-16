using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    /// Gives the task runner an update context
    /// </summary>
    public class GUITaskRunner : MonoBehaviour
    {
        private void Update()
        {
            TaskRunner.Pool();
        }
    }
}
