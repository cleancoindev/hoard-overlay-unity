using UnityEngine;
using TMPro;
using System.ComponentModel;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   UI Component that displays viewcontroller name
    /// </summary>
    public class ViewTitle : MonoBehaviour
    {
        public TMP_Text text;
        public UnityViewModel unityView;

        public void OnEnable()
        {
            unityView = unityView ?? GetComponentInParent<UnityViewModel>();
            text.text = unityView.Title;
            unityView.contextObject.PropertyChanged += PropertyChangedEventHandler;
        }

        public void OnDisable()
        {
            unityView.contextObject.PropertyChanged -= PropertyChangedEventHandler;
        }


        public void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UnityViewModel.Title))
            {
                text.text = unityView.Title;
            }
        }
    }
}
