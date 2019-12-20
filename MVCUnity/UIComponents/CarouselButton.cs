using UnityEngine.EventSystems;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Special case for the selectable button used with carousel
    /// </summary>
    public class CarouselButton : SelectableButton, IPointerClickHandler
    {
        private Carousel carousel;

        public void Awake()
        {
            carousel = GetComponentInParent<Carousel>();
        }

        /// <summary>
        ///   Pass the click on object information to carousel so it can select this object
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            carousel = carousel ?? GetComponentInParent<Carousel>();
            carousel?.MoveToButtonPosition(this);
            eventData.Use();
        }
    }
}
