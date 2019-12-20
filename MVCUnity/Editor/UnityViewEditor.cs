using UnityEditor;

namespace Hoard.MVC.Unity.Editors
{
    [CustomEditor(typeof(UnityViewModel))]
    public class UnityViewEditor : Editor
    {
        UnityViewModel targetObject;

        private void OnEnable()
        {
            targetObject = target as UnityViewModel;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
