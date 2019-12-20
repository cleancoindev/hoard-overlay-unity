using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Unity component that creates the navigable and extendable vertical carousel UI element
    /// </summary>
    public class Carousel : Selectable
    {
#pragma warning disable IDE0052, CS0649 // Remove unread private members
        public RectTransform content;
        public RectTransform contentElement;

        private float contentElementWidth;

        private Vector3 toPosition;

        public bool snap = true;
        private float snapDistance;

        [InterfaceField(typeof(IPositionChanger))]
        public MonoBehaviour additionalMover;

        private IPositionChanger positionChanger;

        [SerializeField]
        private HorizontalLayoutGroup layoutGroup;

        private SlidingObjectControl contentControl;
        private List<SelectableButton> buttons = new List<SelectableButton>();
        public int indexPosition = 0;

        [Range(0.01f, 2f)]
        [SerializeField]
        private float timeOfMovement;

#pragma warning restore IDE0052, CS0649 // Remove unread private members

        public bool Initialized { get; private set; }

        public bool autoInitialize = true;

        protected override void OnEnable()
        {
            if (autoInitialize)
            {
                if (!Initialized) Initialize();
                ResetToCurrentIndex();
            }
        }

        /// <summary>
        ///   Initilize the component
        /// </summary>
        public void Initialize()
        {
            buttons.AddRange(GetComponentsInChildren<SelectableButton>());
            layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
            var rTransform = transform as RectTransform;
            rTransform.ForceUpdateRectTransforms();

            move = false;
            positionChanger = additionalMover as IPositionChanger;
            positionChanger.OnPositionChangeFinished += OnPositionChangeFinished;
            positionChanger.OnPositionChanged += PositionChanger_OnPositionChanged;

            var elementScaleFactor = contentElement.lossyScale.x;
            contentElementWidth = contentElement.sizeDelta.x * elementScaleFactor;

            var contentScaleFactor = content.lossyScale.x;
            var contentWidth = contentElementWidth * buttons.Count;// content.rect.width * contentScaleFactor;

            contentControl = new SlidingObjectControl(transform.position.x, contentElementWidth, contentWidth, layoutGroup.spacing * contentScaleFactor);
            //ResetToCurrentIndex();
            Initialized = true;
        }

        /// <summary>
        ///   Forces selection to given content object. 
        /// </summary>
        /// <param name="contentName">Contents game object name as it apperas when you call .name on it</param>
        public void ResetToContentIndex(string contentName)
        {
            var idx = buttons.FindIndex(0, x => x.ElementName == contentName);
            if (idx > -1)
            {
                indexPosition = idx;
            }
            ResetToCurrentIndex();
        }

        /// <summary>
        ///   Move and selects the current index object
        /// </summary>
        public void ResetToCurrentIndex()
        {
            MoveToIdx(indexPosition, true);
        }

        private void MoveToIdx(int idx, bool immidiateMove = false)
        {
            if (buttons.Count == 0) return;

            if (indexPosition > buttons.Count - 1)
                indexPosition = buttons.Count - 1;

            buttons[indexPosition].OnBeingDeselected();
            indexPosition = Mathf.Clamp(idx, 0, buttons.Count - 1);
            Debug.Log("Standard Move idx Position: " + indexPosition);
            buttons[indexPosition].OnBeingSelected();

            var pos = new Vector3(contentControl.GetContentPositionOnIndex(indexPosition), content.position.y);
            if (Vector3.Distance(content.position, pos) < 0.001f) return;
            if (immidiateMove)
            {
                content.position = pos;
            }
            else
            {
                toPosition = pos;
                move = true;
            }
        }

        /// <summary>
        ///   Adds new selectable button to the carousel and recalculate the positions
        /// </summary>
        public void AddContent(SelectableButton button)
        {
            buttons.Add(button);
            contentControl.ExpandShrinkByElementsCount(1);
        }

        /// <summary>
        ///   Removes content and recalculate the carousel math
        /// </summary>
        public void RemoveContent(SelectableButton button)
        {
            buttons.Remove(button);
            contentControl.ExpandShrinkByElementsCount(-1);
        }

        private void PositionChanger_OnPositionChanged(Vector3 currentPosition)
        {
            contentDragged = true;
            UpdateIndexToContentPosition(currentPosition);
        }

        private void UpdateIndexToContentPosition(Vector3 contentPosition)
        {
            var idx = contentControl.GetIndexAtContentPosition(contentPosition.x);
            //Debug.Log("Index Position: " + idx);
            if (idx >= 0 && idx != indexPosition && idx < buttons.Count)
            {
                buttons[indexPosition].OnBeingDeselected();
                buttons[idx].OnBeingSelected();
                indexPosition = idx;
            }
        }

        private bool contentDragged;

        private void OnPositionChangeFinished(Vector3 startposition, Vector3 endPosition)
        {
            var propPosition = contentControl.GetClosestSnapPosition(endPosition.x);

            var idx = contentControl.GetIndexAtContentPosition(propPosition);

            if (idx < 0)
            {
                idx = 0;
            }
            else if (idx > buttons.Count)
            {
                idx = buttons.Count;
            }

            Debug.Log("Index Selected: " + buttons.Count);

            var toPositionX = contentControl.GetContentPositionOnIndex(idx);

            toPosition.x = toPositionX;
            toPosition.y = content.position.y;
            UpdateIndexToContentPosition(toPosition);

            contentDragged = false;
            move = true;
        }

        public void OnDrawGizmos()
        {
            var mainCamPos = transform.parent.position;
            Gizmos.DrawLine(new Vector3(mainCamPos.x, 1000f), new Vector3(mainCamPos.x, -1000f));
        }

        private void Move(int dir)
        {
            MoveToIdx(dir + indexPosition);
        }

        /// <summary>
        ///   Overide so the select event can be passed to the current button
        /// </summary>
        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnSelect(eventData);
            buttons[indexPosition].OnBeingSelected();
        }

        /// <summary>
        ///   Deselects active content object
        /// </summary>
        public void DeselectActive()
        {
            buttons[indexPosition]?.OnBeingDeselected();
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            base.OnDeselect(eventData);
        }

        /// <summary>
        ///   Turns the carouse so the given button is selected
        /// </summary>
        public void MoveToButtonPosition(CarouselButton btn)
        {
            if (contentDragged) return;
            var idx = contentControl.GetContentElementIndexAt(btn.transform.position.x, content.position.x);
            if (indexPosition == idx)
            {
                buttons[indexPosition].OnBeingSelected();
                return;
            }

            buttons[indexPosition].OnBeingDeselected();
            indexPosition = idx;
            buttons[indexPosition].OnBeingSelected();

            toPosition = new Vector3(contentControl.GetContentPositionOnIndex(idx), content.position.y);
            move = true;
        }

        private bool move = false;
        private bool buttonReset;

        private void Update()
        {
            var buttonHorizontal = Input.GetButtonDown("Horizontal");
            var horiz = Input.GetAxis("Horizontal") != 0.0f
                || buttonHorizontal;
            if (horiz)
            {
                if (!buttonReset)
                {
                    buttonReset = true;
                    var horizontal = Input.GetAxis("Horizontal");
                    if (horizontal < -.0f)
                    {
                        Move(-1);
                    }
                    else if (horizontal > .0f)
                    {
                        Move(1);
                    }
                    if (buttonHorizontal) Input.ResetInputAxes();
                }
            }
            else
            {
                buttonReset = false;
            }

            if (move)
            {
                UpdateMove();
            }
        }

        private void UpdateMove()
        {
            if (Vector3.Distance(content.transform.position, toPosition) > 0.001f)
            {
                content.position = Vector3.MoveTowards(content.position, toPosition, contentElementWidth * Time.deltaTime / timeOfMovement);
            }
            else
            {
                content.position = toPosition;
                move = false;
            }
        }
    }

    public delegate void PositionChangeFinishedDelegate(Vector3 startposition, Vector3 endPosition);

    public delegate void PositionChangeDelegate(Vector3 currentPosition);

    /// <summary>
    ///   Interface for the object that can change the position of the content
    /// </summary>
    public interface IPositionChanger
    {
        event PositionChangeFinishedDelegate OnPositionChangeFinished;

        event PositionChangeDelegate OnPositionChanged;
    }

    /// <summary>
    ///   Internal selection interface for carousel
    /// </summary>
    public interface ISelectedUser
    {
        void OnBeingSelected();

        void OnBeingDeselected();
    }
}
