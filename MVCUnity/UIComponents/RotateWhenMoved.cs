using System;
using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Attribute that allows editor time checkup if the reference is indeed to the correct interface 
    /// </summary>
    public sealed class InterfaceFieldAttribute : PropertyAttribute
    {
        public readonly Type iType;

        public InterfaceFieldAttribute(Type interfaceType)
        {
            iType = interfaceType;
        }
    }

    /// <summary>
    ///   Additional flare for the object movement. It rotates the object a bit when it's dragged
    /// </summary>
    public class RotateWhenMoved : MonoBehaviour
    {
        public float maxRotation;
        public float lerpIn = 1f;

        [InterfaceField(typeof(IVelocityProvider))]
        public UnityEngine.Object dragableElement;

        private IVelocityProvider iDragableElement;

        /// <summary>
        ///   What is the speed when it should rotate to maximal degree
        /// </summary>
        public float atSpeed;

        public void Awake()
        {
            if (dragableElement == null) return;
            iDragableElement = dragableElement as IVelocityProvider;
            iDragableElement.OnVelocityChanged += OnVelocityChanged;
        }

        public void Update()
        {
            var toRot = Quaternion.Euler(0f, nextRot, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRot, Time.deltaTime * lerpIn);
        }

        /// <summary>
        ///   Should rotate
        /// </summary>
        public bool rotate;
        private float nextRot;

        private void OnVelocityChanged(Vector2 obj)
        {
            var normVelo = obj.x / atSpeed;
            nextRot = maxRotation * normVelo;
        }
    }
}
