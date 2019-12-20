using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(CurrentUser))]
    public class ViewCurrentUser : UnityView<CurrentUser>
    {
#pragma warning disable IDE0052, CS0649, CS0414
        [SerializeField]
        private TMP_Text userName = null;

        [SerializeField]
        private TMP_Text userID = null;

        [InterfaceField(typeof(IObserver<Profile>))]
        public MonoBehaviour profileBar;
        public Button accountOptions;
#pragma warning restore IDE0052, CS0649

        public IObserver<Profile> profileDisplay;

        private PropertyChangeHandler<CurrentUser> changeHandler;

        override public void Open()
        {
            base.Open();

            profileDisplay = profileDisplay ?? (profileBar as IObserver<Profile>);

            changeHandler = new PropertyChangeHandler<CurrentUser>(ContextControler)
            {
                { nameof(ContextControler.Profile), (x) => profileDisplay.OnNext(x.Profile)},
            };
            changeHandler.Setup();
        }

        public void MoveToAccountOption()
        {
            ContextControler.OpenAccountOptions();
        }
    }
}
