using UnityEngine;

namespace Hoard.MVC.Unity
{
    /// <summary>
    /// Unity Attachable class. Ability to decorate with hoard view controlers;
    /// Additional responsibility of this class is to interact with monobehaviour context of Unity
    /// <typeparamref name="T"/>IHoardViewControler that should be binded with this class</typeparamref>
    /// </summary>
    public class UnityViewModel : MonoBehaviour, IHoardViewController
    {
        public ViewController contextObject {get; protected set;}

        public virtual string Title {get => contextObject.Title;}

        /// <summary>
        /// Adds the decoration to the monobehaviour so it can be used
        /// </summary>
        /// <param name="controler"></param>
        /// <returns></returns>
        public T BindWithViewModel<T>(T controler) where T : ViewController
        {
            contextObject = controler;
            return controler.BindView(this) as T;
        }

        virtual public void Close()
        {
        }

        virtual public void Disable()
        {
            gameObject.SetActive(false);
        }

        virtual public void Enable()
        {
            gameObject.SetActive(true);
        }

        virtual public void Open()
        {
        }
    }

    /// <summary>
    ///   Intermidiate class for the proper viewcontroller
    /// </summary>
    public class UnityView<T> : UnityViewModel where T : ViewController
    {
        public T ContextControler
        {
            get => contextObject as T;
        }
    }
}
