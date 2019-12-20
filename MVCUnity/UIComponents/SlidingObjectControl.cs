using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Helper object that helps to do a math related to the vertical carousel
    /// </summary>
    public class SlidingObjectControl
    {
        public float ContentElementWidth { get; set; }
        public float spacing = 0;
        public float ContentWidth { get; set; }

        public float ContentOriginPoint;
        public float ContentOffset;

        public float LocalZeroPoint => ContentOriginPoint + ContentOffset;

        public SlidingObjectControl(float startPoint,
float elementWidth,
float contentHolderWidth,
float spacing)
        {
            if (elementWidth == 0)
                throw new System.ArgumentOutOfRangeException(nameof(elementWidth), "Can't be 0");
            if (contentHolderWidth == 0)
                throw new System.ArgumentOutOfRangeException(nameof(contentHolderWidth), "Can't be 0");

            ContentWidth = contentHolderWidth;
            this.spacing = spacing;
            ContentElementWidth = elementWidth + spacing;
            ContentOriginPoint = startPoint;
            ContentOffset = ((contentHolderWidth / 2f) - (ContentElementWidth / 2f));
        }

        public float GetContentPositionOnIndex(int index)
        {
            var width = ContentElementWidth * index;
            return LocalZeroPoint + -1f * width;
        }

        public int GetContentElementIndexAt(float elemX, float contentX)
        {
            var beginning = contentX - (ContentWidth * .5f);
            var distance = elemX - beginning;
            return Mathf.RoundToInt(distance / ContentElementWidth);
        }

        public int GetIndexAtContentPosition(float position)
        {
            if (position > LocalZeroPoint) return -1;

            var distance = Mathf.Abs(position - LocalZeroPoint);

            if (distance > ContentWidth) return (int)(ContentWidth / ContentElementWidth);
            return Mathf.RoundToInt(distance / (ContentElementWidth));
        }

        public void ExpandShrinkByElementsCount(int count)
        {
            var additionalWidth = ContentElementWidth * count;
            ContentWidth += additionalWidth;
            ContentOffset += (additionalWidth / 2);
        }

        public float GetClosestSnapPosition(float position)
        {
            var distance = position - LocalZeroPoint;
            var count = Mathf.RoundToInt(distance / (ContentElementWidth));
            return LocalZeroPoint + (count * ContentElementWidth);
        }
    }
}
