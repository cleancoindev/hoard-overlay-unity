using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Specialized;
using System.Linq;
using Hoard.ProfileUtilities;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(UsersOverview))]
    public class ViewUsersOverview : UnityView<UsersOverview>
    {
#pragma warning disable IDE0052, CS0649
        public Object userButtonPrefab;
        public GameObject listObject;
        public Button createButton, importButton;
        private Dictionary<ProfileDescription, UserListButton> currentButtons = new Dictionary<ProfileDescription, UserListButton>();
#pragma warning restore IDE0052, CS0649

        private PropertyChangeHandler<UsersOverview> HandleControlerChanges;

        private bool setToUser = true;

        public override void Open()
        {
            if (!contentSelector.Initialized)
            {
                ContextControler.Profiles.CollectionChanged += HandleProfilesChanged;
                contentSelector.Initialize();
            }
            RefreshProfiles(ContextControler.Profiles);
            base.Open();
        }

        public override void Enable()
        {
            base.Enable();
            // This is done so when controler is opened fresh it points to the last user. Else it will select last option
            if (setToUser)
            {
                contentSelector.ResetToContentIndex(HoardSettings.LastUser?.userName);
                setToUser = false;
            }
            else
            {
                contentSelector.ResetToCurrentIndex();
            }
        }

        private void HandleProfilesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.Cast<ProfileDescription>())
                    {
                        CreateUserPanel(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.Cast<ProfileDescription>())
                    {
                        Destroy(currentButtons[item]);
                        currentButtons.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    RefreshProfiles(e.NewItems.Cast<ProfileDescription>());
                    break;

                default:
                    break;
            }
        }

        public Carousel contentSelector;

        private void CreateUserPanel(ProfileDescription p)
        {
            var obj = (Instantiate(userButtonPrefab) as GameObject).GetComponent<UserListButton>();
            var scale = obj.transform.localScale;
            obj.SetForProfile(p);
            obj.transform.SetParent(listObject.transform);
            obj.transform.localScale = Vector3.one;
            currentButtons.Add(p, obj);
            var sButton = obj.GetComponent<SelectableButton>();
            contentSelector.AddContent(sButton);
            sButton.ElementName = p.userName;
        }

        public override void Close()
        {
            setToUser = true;
            contentSelector.DeselectActive();
            base.Close();
        }

        public void OpenImport()
        {
            Navigation.Open(new ImportAccount(HoardServiceInitializer.Instance.HoardConfig.WhisperAddress));
        }

        public void OpenCreateUser() =>
            ContextControler.StartCreateNew();

        private void RefreshProfiles(IEnumerable<ProfileDescription> list)
        {
            var buttonProfiles = currentButtons.Keys;
            var missing = list.Except(buttonProfiles);
            var toRemove = buttonProfiles.Except(list);

            foreach (var m in missing)
            {
                CreateUserPanel(m);
            }
            contentSelector.ResetToContentIndex(HoardSettings.LastUser?.userName);
        }
    }
}