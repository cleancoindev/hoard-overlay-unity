using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hoard.ProfileUtilities;
using Newtonsoft.Json.Linq;
using Hoard.MVC.Utilities;
using System.Linq;

namespace Hoard.MVC
{
    /// <summary>
    ///   Step by step procedure to user profile to the other user machine
    /// </summary>
    public class ImportProcedure : Procedure
    {
        /// <summary>
        ///   Provide the input pin values 
        /// </summary>
        public override void ProvideInput(string input)
            => ImportPIN.Value = input;

        public ImportProcedure()
        {
 // Hex value regex digits + (a::f)
            var hexRegex = new Regex("^[0-9a-fA-F]{8}$");
            ImportPIN = new CredentialInputValidator((x) => hexRegex.IsMatch(x));
            State = TransferState.Ready;
            WaitingForUserProceed = true;
        }

        public string confirmationPIN;

        /// <summary>
        ///   The confirmation pin generated so you can verify that users machines are indeed connected securely
        /// </summary>
        public string ConfirmationPIN
        {
            get => confirmationPIN;
            private set
            {
                confirmationPIN = value;
                NotifyChange();
            }
        }

        /// <summary>
        ///   Pin field for the value privided by export porcedure
        /// </summary>
        public CredentialInputValidator ImportPIN;

        /// <summary>
        ///   Take another action in the procedure
        /// </summary>
        public override void Proceed()
        {
            switch (State)
            {
                case TransferState.Timeout:
                    State = TransferState.Ready;
                    break;

                case TransferState.Error:
                    State = TransferState.Ready;
                    break;

                case TransferState.Ready:
                    TimeOutCount = 0f;
                    PingAndConnect();
                    break;

                case TransferState.InputPIN:
                    ExchangeKeys();
                    break;

                case TransferState.CheckPin:
                    break;

                case TransferState.ConfirmOtherPIN:
                    break;

                case TransferState.Done:
                    Navigation.GoBackToView<UsersOverview>();
                    break;
            }
        }

        private AccountSynchronizerApplicant SyncApplicant
        {
            get => sync as AccountSynchronizerApplicant;
        }

        public override void Connect()
        {
            cancelToken = new System.Threading.CancellationTokenSource();
            sync = new AccountSynchronizerApplicant(new SystemWebSocketProvider(WhisperAddress));
            State = TransferState.InputPIN;
        }

        private void ExchangeKeys()
        {
            if (!ImportPIN.IsValid) return;
            StartTimer();
            ImportPIN.Value = ImportPIN.Value.ToUpper();
            WaitingForUserProceed = false;
            State = TransferState.CheckPin;
            sync.Initialize(ImportPIN.Value, cancelToken.Token)
                .ContinueGUISynch(FollowInitialization);
        }

        // Add the general timeout
        private void FollowInitialization(Task t)
        {
            if (t.IsFaulted)
            {
                Reset(TransferState.Error);
            }
            else
            {
                ErrorCallbackProvider.ReportInfo("Export: Initialized");
                SendPublicKey();
            }
        }

        private void SendPublicKey()
        {
            SyncApplicant.SendPublicKey(cancelToken.Token)
                .ContinueGUISynch(x =>
                {
                    if (x.IsFaulted || x.IsCanceled) return;
                    ErrorCallbackProvider.ReportInfo("Export: Public key sent");
                    GetConfirmationHash();
                });
        }

        private void GetConfirmationHash()
        {
            SyncApplicant.AcquireConfirmationHash(cancelToken.Token)
                .ContinueGUISynch(CheckConfirmation);
        }

        private void CheckConfirmation(Task<string> confirmationHash)
        {
            if (confirmationHash.IsCanceled || confirmationHash.IsFaulted)
            {
                return;
            }
            ErrorCallbackProvider.ReportInfo("Export: Confirmation hash acquired");
            ConfirmationPIN = confirmationHash.Result;
            State = TransferState.ConfirmOtherPIN;
            RecieveAccount();
        }

        private void RecieveAccount()
        {
            SyncApplicant.AcquireKeystoreData(cancelToken.Token)
                .ContinueGUISynch(x =>
                {
                    if (!x.IsFaulted && !x.IsCanceled)
                        DecodeProfile(x);
                });
        }

        private void FinishImport()
        {
            State = TransferState.Done;
            Shutdown();
            WaitingForUserProceed = true;
        }

        private void DecodeProfile(Task<string> task)
        {
            if (!task.IsFaulted)
            {
                var accountData = task.Result;
                JObject accountJson = JObject.Parse(accountData);
                JObject profile = (JObject)accountJson[SectionName_Profile];
                string name = profile["name"].ToString();
                string adress = profile["address"].ToString();
                var sameAccount = ProfilesManagement.Instance.AvailableProfileNames.FirstOrDefault(x => x.ID.ToString() == adress);
                if (sameAccount != null)
                {
                    Navigation.Open(new Information(string.Format("INFO_SAME_KEY".Translated(),sameAccount.userName) , () => {Navigation.GoBack();}));
                }
                else if (ProfilesManagement.Instance.AvailableProfileNames.Any(x => x.userName == name))
                {
                    Navigation.Open(new RenameUser(name, x =>
                    {
                        profile["name"] = x;
                        ProfilesManagement.Instance.ReceiveProfile(x, adress, profile.ToString());
                        FinishImport();
                    }));
                }
                else
                {
                    ProfilesManagement.Instance.ReceiveProfile(name, adress, profile.ToString());
                    FinishImport();
                }
            }
        }
    }
}
