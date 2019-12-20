using TMPro;
using UnityEngine.UI;
using System.Collections;
using Hoard.MVC.Utilities;

namespace Hoard.MVC.Unity
{
    [UnityViewOf(typeof(ExportAccount))]
    public class ViewExportAccount : UnityView<ExportAccount>
    {
        public TMP_Text hint;

        public TMP_InputField inputField;
        public TMP_Text exportPin;

        public TMP_Text buttonInfo;
        public Button continueButton;

        public TMP_Text timeOutCount;

        private PropertyChangeHandler<ExportAccount> propertyHandler;

        public Button backButton;

        public ViewExportAccount(string userName)
        {
            this.name = userName;
        }

        private IEnumerator PoolTimer()
        {
            while (true)
            {
                timeOutCount.enabled = true;
                var proc = ContextControler.Procedure;
                var time = ExportProcedure.TimeOutMax - proc.TimeOutCount;
                timeOutCount.SetText(string.Format("TIMEOUT_COUNT".Translated(), time.ToString()));
                yield return null;
            }
        }

        public override void Open()
        {
            base.Open();
            inputField.SetTextWithoutNotify("");
            propertyHandler = new PropertyChangeHandler<ExportAccount>(ContextControler)
            {
                {nameof(ExportAccount.Hint), x => hint.SetText(x.Hint) },
                {nameof(ExportAccount.ActionDescription), x => buttonInfo.SetText(x.ActionDescription) },
                {nameof(ExportProcedure.WaitingForUserProceed), x =>
                 {
                     var oldInteract = !continueButton.interactable && ContextControler.Procedure.WaitingForUserProceed;
                     continueButton.interactable = ContextControler.Procedure.WaitingForUserProceed;
                     if (oldInteract)
                     {
                         continueButton.Select();
                     }

                     if (!ContextControler.Procedure.WaitingForUserProceed) backButton.Select();
                 }
                },
                {nameof(ExportAccount.InputPIN), x=> inputField.gameObject.SetActive(ContextControler.InputPIN) },
                {nameof(ExportAccount.ExportPIN), x =>
                    {
                        if (string.IsNullOrEmpty(x.ExportPIN))
                        {
                            exportPin.gameObject.SetActive(false);
                        }
                        else
                        {
                            exportPin.gameObject.SetActive(true);
                            exportPin.text = x.ExportPIN;
                        }
                    }
                },
            };
            propertyHandler.Setup();
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
