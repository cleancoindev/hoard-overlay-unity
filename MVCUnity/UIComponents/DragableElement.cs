using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Class that can be dragged and pass the drag information to other objects that needs it 
    /// </summary>
    public class DragableElement : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IVelocityProvider,
        IPositionChanger
    {
        private Vector2 startDragPosition;

        private Vector2 velocity;

        public Vector2 Velocity
        {
            get => velocity;
            private set
            {
                if (value != velocity)
                {
                    velocity = value;
                    OnVelocityChanged?.Invoke(value);
                }
            }
        }

        /// <summary>
        ///   Invoked when velocity of dragged object changes
        /// </summary>
        public event Action<Vector2> OnVelocityChanged;

        /// <summary>
        ///   Callback that passes the new position
        /// </summary>
        public event PositionChangeDelegate OnPositionChanged;

        /// <summary>
        ///   Called when user finishes the drag of the object
        /// </summary>
        public event PositionChangeFinishedDelegate OnPositionChangeFinished;

        public bool BeingDragged { get; private set;}

        public void OnBeginDrag(PointerEventData eventData)
        {
            BeingDragged = true;
            startDragPosition = eventData.position;
            eventData.Use();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.IsPointerMoving())
            {
                Vector3 delta = eventData.position - startDragPosition;
                delta.y = 0;

                transform.position = delta + transform.position;
                OnPositionChanged?.Invoke(transform.position);

                Velocity = eventData.delta;
                startDragPosition = eventData.position;
            }
            else
            {
                velocity = Vector2.zero;
            }
            eventData.Use();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Velocity = Vector2.zero;
            OnPositionChangeFinished?.Invoke(startDragPosition, transform.position);
            BeingDragged = false;
            eventData.Use();
        }
    }

    /// <summary>
    ///   Interface for the object that have a velocity attribute 
    /// </summary>
    public interface IVelocityProvider
    {
        Vector2 Velocity { get; }

        event System.Action<Vector2> OnVelocityChanged;
    }
}
