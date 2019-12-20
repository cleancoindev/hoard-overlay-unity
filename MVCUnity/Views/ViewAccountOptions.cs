using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(AccountOptions))]
    public class ViewAccountOptions : UnityView<AccountOptions>
    {
        public Button changeUser;
        public TMP_Text autoSaveButtonText;
        public Button[] actionButtons;

        [InterfaceField(typeof(IObserver<Profile>))]
        public MonoBehaviour profileBar;

        public IObserver<Profile> profileDisplay;

        private bool ChangeUsersActive
        {
            set => changeUser.gameObject.SetActive(value);
            get => changeUser.gameObject.activeSelf;
        }

        public override void Open()
        {
            profileDisplay = profileDisplay ?? (profileBar as IObserver<Profile>);
            base.Open();
            changeHandler = new PropertyChangeHandler<AccountOptions>(ContextControler)
            {
                {nameof(ContextControler.AccountLogged), x => ChangeUsersActive = !ContextControler.AccountLogged },
                {nameof(ContextControler.Profile),
                    x =>
                    {
                        profileDisplay.OnNext(x.Profile);
                    }
                },
                {nameof(ContextControler.Autologin), x=> autoSaveButtonText.SetText(ContextControler.Autologin? "AUTOLOGIN_ON".Translated(): "AUTOLOGIN_OFF".Translated()) }
            };
            changeHandler.Setup();
            ChangeUsersActive = !ContextControler.AccountLogged;
        }

        private PropertyChangeHandler<AccountOptions> changeHandler;

        public void OnChangeUserClicked()
        {
            Navigation.Open(new UsersOverview(ProfilesManagement.Instance));
        }

        public void OnLoginClicked()
        {
            Navigation.Open(new LoginUser(ContextControler.Profile.Name));
        }

        public void OnExportClicked()
        {
            Navigation.Open(new ExportAccount(new ProfileDescription(ContextControler.Profile)));
        }

        public void OnChangePasswordClicked()
        {
            Navigation.Open(new Information("INFO_ACCOUNT".Translated(),
                                ()=>Navigation.Open(new ChangePassword(ContextControler.Profile))));
        }

        public void OnSavePasswordClicked()
        {
            ContextControler.SetSavePassword(!ContextControler.Autologin);
        }
    }
}
