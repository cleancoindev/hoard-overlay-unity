using System.Collections.Generic;
using System;
#if REFLECTION_BIND
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using Hoard.MVC.Binding.Reflection;
using System.Linq;
using System.Collections;
#endif

namespace Hoard.MVC.Binding
{
    /// <summary>
    ///   Automatic binding through the reflection
    /// </summary>
    namespace Reflection
    {
        /// <summary>
        ///   Bindable atribute. This atributes will be binded to the properties in the oberver
        /// </summary>
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class AutoBindAttribute : System.Attribute
        {
            public string PropName { get; set; }
            public AutoBindAttribute(string PropName = null)
            {
                this.PropName = PropName;
            }
        }

    public static class CustomBinder
    {
#if REFLECTION_BIND
        public static void Bind<C, V>(C controler, V view) 
        {
            var tCont = typeof(C).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var tView = typeof(V).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var tContBindable = tCont.Where(x => x.GetCustomAttribute<AutoBindAttribute>() != null);
            var tViewBindable = tView.Where(x => x.GetCustomAttribute<AutoBindAttribute>() != null);

            foreach (var bind in tViewBindable)
            {
                // TODO Check the types
                var toBind = tContBindable.FirstOrDefault(x => x.Name == bind.Name);
                if (toBind == default) continue;

                var toVal = toBind.GetValue(controler);
                var fromVal = bind.GetValue(view);
                var genericBaseType = bind.FieldType;

                MethodInfo method = genericBaseType.GetMethod("SubscribeTo");
                method.Invoke(fromVal, new object[] { toVal });
            }
        }
#else
        public static void Bind<C, V>(C controler, V view)
        {
            throw new System.NotImplementedException();
        }

#endif
        public static void PropertyChanged(object caller, object value)
        {
        }
    }

    /// <summary>
    ///   Property that can be observed be the binder
    /// </summary>
    class BindedProperty
    {
        object prop;
        Guid objct;
        string propname;
    }

    /// <summary>
    ///   Property that will be updated to the observed value
    /// </summary>
    public class ObservatorProperty<T> : IObserver<T>
        where T : struct
    {
        Guid guid;
        T internalValue = default;
        IDisposable unsubscriber;
        Action<T> onPropertyChanged;

        public static implicit operator T(ObservatorProperty<T> val) => val.internalValue;

        public ObservatorProperty(IObservable<T> observable = null, Action<T> onPropertyChanged = null) 
        {
            SubscribeTo(observable);
            this.onPropertyChanged = onPropertyChanged;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void SubscribeTo(IObservable<T> observable)
        {
            unsubscriber = observable?.Subscribe(this);
        }

        public void OnNext(T value)
        {
            internalValue = value;
            onPropertyChanged?.Invoke(value);
        }

        ~ObservatorProperty()
        {
            if (unsubscriber != null) unsubscriber.Dispose();
        }
    }

    public class PropertyUnsubscriber<T> : IDisposable
    {
        private readonly List<IObserver<T>> subscribers;
        private readonly IObserver<T> observer;

        public PropertyUnsubscriber(List<IObserver<T>> subscribers, IObserver<T> observer)
        {
            this.subscribers = subscribers;
            this.observer = observer;
        }
        public void Dispose()
        {
            if (subscribers.Contains(observer)) subscribers.Remove(observer);
        }
    }

    public class ObservableProperty<T> : IObservable<T>
        where T : struct
    {
        T internalValue;
        public T Value
        {
            get
            {
                return internalValue;
            }
            set
            {
                internalValue = value;
                Notify(value);
            }
        }

        public ObservableProperty(T initialValue = default)
        {
            internalValue = initialValue;
        }

        void Notify(T y) => myObservers.ForEach(x => x.OnNext(y));

        List<IObserver<T>> myObservers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!myObservers.Contains(observer))
            {
                myObservers.Add(observer);
            }
            observer.OnNext(internalValue);
            return new PropertyUnsubscriber<T>(myObservers, observer);
        }

        public static implicit operator T(ObservableProperty<T> val)
        {
            return val.Value;
        }
    }
}
} 
