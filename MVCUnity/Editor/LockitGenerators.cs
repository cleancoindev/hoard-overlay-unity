using UnityEditor;
using UnityEngine;
using Hoard.MVC.Utilities;
using System.IO;
namespace Hoard.MVC.Unity.Editors
{
    public static class LockitExampleGenerator
    {
        /// <summary>
        ///   Creates template for the lockit so the json format will always fit serialization
        /// </summary>
        [MenuItem("Assets/Create/Hoard/GenerateExampleLockitJson")]
        public static void GenerateExampleLockit()
        {
            var text = Lockit.GenerateExampleText();
            Debug.Log(Selection.activeContext);
            Debug.Log(Selection.activeObject);
            Debug.Log(Selection.activeInstanceID);
            if (Selection.activeObject == null) return;
            var path = Path.Combine(AssetDatabase.GetAssetPath(Selection.activeInstanceID), "new_lockit.json");

            // Removing 'Assets from the path'
            var abspath = Path.Combine(Application.dataPath, path.Replace(@"Assets/", ""));
            using (var vs = File.CreateText(abspath))
            {
                vs.Write(text);
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

    }
}

