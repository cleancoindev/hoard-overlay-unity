using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hoard.MVC.Unity
{
    /// <summary>
    /// Provides the controlers decorated with MonoViewControler
    /// Automatically searches for the all MonoViewControler in the hierarchy below
    /// </summary>
    public class UnityControlerProvider : MonoBehaviour, IHoardViewModelProvide
    {
        public Dictionary<Type, UnityViewModel> controlers = new Dictionary<Type, UnityViewModel>();

        /// <summary>
        /// Provides the controler decorated with unity view. In this case it will only work with ViewControlerBase
        /// </summary>
        /// <param name="controler"></param>
        /// <returns></returns>
        public IHoardViewController ProvideFor(IHoardViewController controler)
        {
            if (!(controler is ViewController)) return controler;

            var type = controler.GetType();

            if (!controlers.ContainsKey(type)) return controler;

            return controlers[type].BindWithViewModel(controler as ViewController);
        }

        private void Awake()
        {
            var monoViews = GetComponentsInChildren<UnityViewModel>(true);
            foreach (var view in monoViews)
            {
                var attributes = view.GetType()
                    .GetCustomAttributes(typeof(UnityViewOf), false);

                if (attributes.Length == 0) continue;
                var keyT = (attributes[0] as UnityViewOf).ControlerType;
                controlers.Add(keyT, view);
            }
            Navigation.controlerProvider = this;
        }
    }

    /// <summary>
    /// Class that indicates to which generic viewcontroler this class relates
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false, Inherited = false)]
    public class UnityViewOf : Attribute
    {
        public Type ControlerType { get; private set; }
        public UnityViewOf(Type t) => this.ControlerType = t;
    }
}
