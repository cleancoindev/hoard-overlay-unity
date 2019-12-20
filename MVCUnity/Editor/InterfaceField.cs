using UnityEngine;
using UnityEditor;

namespace Hoard.MVC.Unity
{
    [CustomPropertyDrawer(typeof(InterfaceFieldAttribute))]
    public class InterfaceField : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as InterfaceFieldAttribute;
            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(position, property, fieldInfo.FieldType);
            if (EditorGUI.EndChangeCheck())
            {
                if (property.objectReferenceValue == null) return;
                var inType = property.objectReferenceValue.GetType();
                if (attr.iType.IsAssignableFrom(inType))
                {
                    return;
                }

                if (property.objectReferenceValue is GameObject)
                {
                    var gObj = property.objectReferenceValue as GameObject;
                    foreach (var p in gObj.GetComponents<Component>())
                    {
                        inType = p.GetType();
                        if (attr.iType.IsAssignableFrom(inType))
                        {
                            property.objectReferenceValue = p;
                            return;
                        }
                    }
                }
                else if (property.objectReferenceValue is Component)
                {
                    var gObj = (property.objectReferenceValue as Component).gameObject;
                    foreach (var p in gObj.GetComponents<Component>())
                    {
                        inType = p.GetType();
                        if (attr.iType.IsAssignableFrom(inType))
                        {
                            property.objectReferenceValue = p;
                            return;
                        }
                    }
                }
                property.objectReferenceValue = null;
            }
        }
    }

    [CustomPropertyDrawer(typeof(GetComponentAttribute))]
    public class GetComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(position, property);
            if (property.isArray) return;
            {
            }

            if (property.propertyType != SerializedPropertyType.ObjectReference) return;
            if (property.objectReferenceValue == null)
            {
                //if (property.serializedObject.UpdateIfRequiredOrScript())
                //{
                var obj = (property.serializedObject.targetObject as Component).GetComponent(fieldInfo.FieldType);
                if (obj != null)
                {
                    property.objectReferenceValue = obj;
                    //property.serializedObject.ApplyModifiedProperties();
                }
                //}
            }
        }
    }
}
