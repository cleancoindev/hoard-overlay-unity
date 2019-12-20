using System;
using UnityEngine;
using TMPro;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   User bar keeps track of the current logged user Profile and provides information on current Hoard Token ballance 
    /// </summary>
    public class UserBar : MonoBehaviour, IObserver<Profile>
    {
#pragma warning disable IDE0052, CS0649 // Remove unread private members
        [SerializeField]
        private TMP_Text UserName;

        [SerializeField]
        private TMP_Text Balance;
#pragma warning restore IDE0052, CS0649 

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            UserName.SetText("Disconnected");
        }

        private void SetBalance(Profile profile)
        {
            HoardService.Instance.GetHRDAmount(profile).ContinueGUISynch(x =>
            {
                if (x.IsFaulted)
                {
                    Balance.SetText(string.Format("Disconnected"));
                }
                else
                {
                    Balance.SetText(string.Format("{0} HRD", x.Result.ToString()));
                }
            });
        }

        public void OnNext(Profile value)
        {
            if (value == null)
            {
                UserName.SetText("Disconnected");
                Balance.SetText("Disconnected");
            }
            else
            {
                UserName.SetText(value.Name);
                SetBalance(value);
            }
        }
    }
}
