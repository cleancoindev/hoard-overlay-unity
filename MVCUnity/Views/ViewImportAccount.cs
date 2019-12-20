using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Hoard.MVC.Utilities;

using UnityEngine;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(ImportAccount))]
    public class ViewImportAccount : UnityView<ImportAccount>
    {
#pragma warning disable IDE0052, CS0649 // UNITY set
        public TMP_Text hint;
        public TMP_Text timeOutCount;

        public TMP_InputField inputField;
        public TMP_Text exportPin;

        public TMP_Text buttonInfo;
        public Button continueButton;
        public Button backButton;

        private PropertyChangeHandler<ImportAccount> propertyHandler;

        public ViewImportAccount(string userName)
        {
            this.name = userName;
        }

#pragma warning restore IDE0052, CS0649 // UNITY set

        private IEnumerator PoolTimer()
        {
            while (true)
            {
                var proc = ContextControler.Procedure;
                timeOutCount.enabled = true;
                var time = ImportProcedure.TimeOutMax - proc.TimeOutCount;
                timeOutCount.SetText(string.Format("TIMEOUT_COUNT".Translated(), time.ToString()));
                yield return null;
            }
        }

        public override void Enable()
        {
            base.Enable();
            StartCoroutine(PoolTimer());
        }

        public override void Disable()
        {
            base.Disable();
            StopAllCoroutines();
        }

        public char Validate(string text, int pos, char ch)
        {
            Regex Regex = new Regex("^[0-9a-fA-F]{0,8}$");
            var testText = text + ch;
            var match = Regex.IsMatch(testText);

            return  match ? ch : '\0';
        }

        private void SetContinueButton(string text)
        {
            Regex Regex = new Regex("^[0-9a-fA-F]{8}$");
            continueButton.interactable = Regex.IsMatch(text);
        }

        public override void Close()
        {
            base.Close();
            inputField.onValidateInput -= Validate;
            inputField.onValueChanged.RemoveListener(SetContinueButton);
        }

        public override void Open()
        {
            base.Open();
            inputField.onValidateInput += Validate;
            inputField.onValueChanged.AddListener(SetContinueButton);
            inputField.SetTextWithoutNotify("");
            propertyHandler = new PropertyChangeHandler<ImportAccount>(ContextControler)
            {
                {nameof(ImportAccount.Hint), x => hint.SetText(x.Hint) },
                {nameof(ImportAccount.ActionDescription), x => buttonInfo.SetText(x.ActionDescription) },
                {nameof(ImportProcedure.WaitingForUserProceed), x =>
                 {
                     var waitingForUser = ContextControler.Procedure.WaitingForUserProceed;
                     var oldInteract = !continueButton.interactable && waitingForUser;
                     continueButton.interactable = ContextControler.Procedure.WaitingForUserProceed;
                     if (oldInteract)
                     {
                         continueButton.Select();
                     }
                     if (!waitingForUser)
                     {
                         backButton.Select();
                     }
                 }},
                {nameof(ImportAccount.InputPINVisiable),
                x=>
                  {
                      inputField.gameObject.SetActive(ContextControler.InputPINVisiable);
                      if (ContextControler.InputPINVisiable)
                      {
                        continueButton.interactable = false;
                        SelectWhenAvailabe(inputField);
                      }
                  }
                },
                {nameof(ImportAccount.InputPINInteractive),
                x=>
                  {
                      inputField.interactable = ContextControler.InputPINInteractive;
                      if (ContextControler.InputPINInteractive)
                      {
                          SelectWhenAvailabe(inputField);
                      }
                  }
                },
                {nameof(ImportAccount.ConfimationPIN), x =>
                    {
                        if (string.IsNullOrEmpty(x.ConfimationPIN))
                        {
                            exportPin.gameObject.SetActive(false);
                        }
                        else
                        {
                            exportPin.gameObject.SetActive(true);
                            exportPin.text = x.ConfimationPIN;
                        }
                    }
                },
            };
            propertyHandler.Setup();
        }

        private void SelectWhenAvailabe(TMP_InputField pinField)
        {
            if (pinField.gameObject.activeInHierarchy && pinField.interactable)
            {
                UIUtility.ForceSelection(pinField);
            }
        }

        public void Continue()
        {
            ContextControler.Continue();
        }

        public void ProvideInput(string input)
        {
            ContextControler.Procedure.ProvideInput(input);
        }
    }
}
