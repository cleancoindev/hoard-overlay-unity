using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    /// Interrmidiate between the Import procedure and the user view
    /// </summary>
    public class ImportAccount : ViewController
    {
        public ImportProcedure Procedure { get; private set; }

        private string hint;

        /// <summary>
        ///   Information for the user about the current state of procedure and what actions are requested
        /// </summary>
        public string Hint
        {
            get => hint;
            set
            {
                hint = value;
                NotifyChange();
            }
        }

        private string exportPIN;

        /// <summary>
        /// Exposes generated confirmation pin for the procedure
        /// </summary>
        public string ConfimationPIN
        {
            get => exportPIN;
            private set
            {
                exportPIN = value;
                NotifyChange();
            }
        }

        private string actionDescription;

        /// <summary>
        ///   Name for the next action that user should perfom
        /// </summary>
        public string ActionDescription
        {
            get => actionDescription;
            set
            {
                actionDescription = value;
                NotifyChange();
            }
        }

        private readonly
        PropertyChangeHandler<ImportProcedure> procedureHandler;

        /// <summary>
        ///   Ctor
        /// </summary>
        public ImportAccount()
        {
            Title = "TITLE_IMPORT".Translated();
            Procedure = new ImportProcedure();
            procedureHandler = new PropertyChangeHandler<ImportProcedure>(Procedure)
            {
                {nameof(Procedure.State), x=> SetInfo(x.State)},
                {nameof(Procedure.WaitingForUserProceed), x => NotifyChange(nameof(Procedure.WaitingForUserProceed)) },
                {nameof(Procedure.ConfirmationPIN), x=> ConfimationPIN = x.ConfirmationPIN}
            };
            procedureHandler.Setup();
        }



        private bool inputPinInteractive;

        /// <summary>
        ///   Is input pin input box be recieving input
        /// </summary>
        public bool InputPINInteractive
        {
            get => inputPinInteractive;
            private set
            {
                inputPinInteractive = value;
                NotifyChange();
            }
        }
        private bool inputPinVisiable;

        /// <summary>
        ///   Should input field be visiable to the user
        /// </summary>
        public bool InputPINVisiable
        {
            get => inputPinVisiable;
            private set
            {
                inputPinVisiable = value;
                NotifyChange();
            }
        }

        /// <summary>
        /// Move to the next step of the procedure
        /// </summary>
        public void Continue()
        {
            if (Procedure.WaitingForUserProceed)
                Procedure.Proceed();
        }

        public override void Close()
        {
            Procedure.Shutdown().ContinueGUISynch(x => base.Close());
        }

        /// <summary>
        ///   Set the information for the user according to the transfer state
        /// </summary>
        private void SetInfo(ImportProcedure.TransferState state)
        {
            switch (state)
            {

                case ImportProcedure.TransferState.Timeout:
                    ConfimationPIN = null;
                    InputPINInteractive = true;
                    InputPINVisiable = false;
                    Hint =string.Format("HINT_TIMEOUT".Translated() , ExportProcedure.TimeOutMax);
                    ActionDescription = "ACTION_RESET".Translated();
                    break;
                case ImportProcedure.TransferState.Error:
                    ConfimationPIN = null;
                    InputPINInteractive = true;
                    InputPINVisiable = false;
                    Hint = "HINT_ERROR".Translated();
                    ActionDescription = "ACTION_RESET".Translated();
                    break;

                case ImportProcedure.TransferState.Ready:
                    ConfimationPIN = null;
                    InputPINVisiable = false;
                    InputPINInteractive = true;
                    Hint = "HINT_START_EXPORT".Translated();
                    ActionDescription = "ACTION_DONE".Translated();
                    break;

                case ImportProcedure.TransferState.InputPIN:
                    InputPINVisiable = true;
                    Hint = "HINT_ENTER_PIN".Translated();
                    ActionDescription = "ACTION_DONE".Translated();
                    break;

                case ImportProcedure.TransferState.CheckPin:
                    InputPINInteractive = false;
                    Hint = "HINT_CHECKING_PIN".Translated();
                    ActionDescription = "ACTION_WAIT".Translated();
                    break;

                case ImportProcedure.TransferState.ConfirmOtherPIN:
                    Hint = "HINT_CONFIRM_OTHER_PIN".Translated();
                    ActionDescription = "ACTION_WAIT".Translated();
                    InputPINVisiable = false;
                    break;

                case ImportProcedure.TransferState.SendRecieve:
                    ConfimationPIN = null;
                    Hint = "HINT_EXPORT_TRANSFER".Translated();
                    ActionDescription = "ACTION_RECEIVING".Translated();
                    InputPINVisiable = false;
                    break;

                case ImportProcedure.TransferState.Done:
                    Hint = "HINT_SUCCESS".Translated();
                    ActionDescription = "ACTION_DONE".Translated();
                    break;
            }
        }
    }
}
