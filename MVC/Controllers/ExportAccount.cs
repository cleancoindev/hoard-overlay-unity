using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   Controller that handles the communication between the user and the export procedure
    /// </summary>
    public class ExportAccount : ViewController
    {
        /// <summary>
        ///   Current export procedure object
        /// </summary>
        public ExportProcedure Procedure { get; private set; }

        private string hint;

        /// <summary>
        ///   Hint to the player on what to do next
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
        ///   PIN that will validate the account transfer
        /// </summary>
        public string ExportPIN
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
        /// Human readable information on what action is available to the user
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

        private ProfileDescription userName;

        /// <summary>
        /// Information of the user that will be transfered
        /// </summary>
        public ProfileDescription UserName
        {
            get => userName;
            private set
            {
                userName = value;
                NotifyChange();
            }
        }

        private readonly
        PropertyChangeHandler<ExportProcedure> procedureHandler;

        /// <summary>
        ///   Ctor. 
        /// </summary>
        public ExportAccount(ProfileDescription name)
        {
            UserName = name;
            Title = "TITLE_EXPORT_ACCOUNT".Translated() + UserName.userName;
            Procedure = new ExportProcedure(UserName);
            procedureHandler = new PropertyChangeHandler<ExportProcedure>(Procedure)
            {
                {nameof(ExportProcedure.State), x=> SetInfo(x.State)},
                {nameof(ExportProcedure.WaitingForUserProceed), x => NotifyChange(nameof(ExportProcedure.WaitingForUserProceed)) },
                {nameof(ExportProcedure.PIN), x => ExportPIN = x.PIN}
            };
            procedureHandler.Setup();
            //Procedure.Proceed();
        }

        private bool inputPin;

        /// <summary>
        /// Should verification pin box be filled by user
        /// </summary>
        public bool InputPIN
        {
            get => inputPin;
            private set
            {
                inputPin = value;
                NotifyChange();
            }
        }

        public override void Close()
        {
            Procedure.Shutdown().ContinueGUISynch(x => base.Close());
            base.Close();
        }

        /// <summary>
        /// Perfom next step of the procedure if possible
        /// </summary>
        public void Continue()
        {
            if (Procedure.WaitingForUserProceed)
                Procedure.Proceed();
        }

        /// <summary>
        ///   Sets information fields and state of this object according to the current transfer state
        /// </summary>
        private void SetInfo(ExportProcedure.TransferState state)
        {
            switch (state)
            {
                case ExportProcedure.TransferState.Timeout:
                    ExportPIN = null;
                    InputPIN = false;
                    Hint = string.Format("HINT_TIMEOUT".Translated(), ExportProcedure.TimeOutMax);
                    ActionDescription = "ACTION_RESET".Translated();
                    break;

                case ExportProcedure.TransferState.Error:
                    ExportPIN = null;
                    InputPIN = false;
                    Hint = "HINT_ERROR".Translated();
                    ActionDescription = "ACTION_RESET".Translated();
                    break;

                case ExportProcedure.TransferState.Ready:
                    ExportPIN = null;
                    InputPIN = false;
                    Hint = "HINT_EXPORT_READY".Translated();
                    ActionDescription = "ACTION_DONE".Translated();
                    break;


                case ExportProcedure.TransferState.InputPIN:
                    ExportPIN = Procedure.PIN;
                    Hint = "HINT_INPUT_PIN".Translated();
                    ActionDescription = "ACTION_WAIT".Translated();
                    break;

                case ExportProcedure.TransferState.ConfirmOtherPIN:
                    ExportPIN = null;
                    Hint = "HINT_CONFIRM_OTHER_PIN".Translated();
                    ActionDescription = "ACTION_CONFIRM".Translated();
                    break;

                case ExportProcedure.TransferState.SendRecieve:
                    Hint = "HINT_EXPORT_TRANSFER".Translated();
                    ActionDescription = "ACTION_SENDING".Translated();
                    break;

                case ExportProcedure.TransferState.Done:
                    Hint = "HINT_SUCCESS".Translated();
                    ActionDescription = "ACTION_DONE".Translated();
                    break;
            }
        }
    }
}
