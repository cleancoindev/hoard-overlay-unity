using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Hoard.Utils;
using Hoard.ProfileUtilities;
using System.Text;
using Hoard.MVC.Utilities;

namespace Hoard.MVC
{
    /// <summary>
    ///   Encapsulates the export procedure of the user accounts
    /// </summary>
    public class ExportProcedure : Procedure
    {
        Profile profile;

        public override void ProvideInput(string input) =>
            ConfirmPIN.Value = input;

        ProfileDescription transferProfile;

        AccountSynchronizerKeeper SyncKeeper {get => sync as AccountSynchronizerKeeper;}

        AccountSynchronizerKeeper CreateSync(string address)
        {
            return new AccountSynchronizerKeeper(new SystemWebSocketProvider(address));
        }

        private string exportPIN = null;

        /// <summary>
        ///   Pin that will be input in the other device to validate profile transfer
        /// </summary>
        public string PIN
        {
            get => exportPIN;
            private set
            {
                exportPIN = value;
                NotifyChange();
            }
        }

        public ExportProcedure(ProfileDescription profile)
        {
            transferProfile = profile ??
                throw new System.ArgumentNullException("profile for transfer is required");

            // Hex value regex digits + (a::f)
            var hexRegex = new Regex("^[0-9a-fA-F]{8}$");
            ConfirmPIN = new CredentialInputValidator((x) => hexRegex.IsMatch(x) && x == PIN);
            State = TransferState.Ready;
            WaitingForUserProceed = true;
        }

        /// <summary>
        ///   Confirmation pin that will verify that user machine is the one for the transfer
        /// </summary>
        public CredentialInputValidator ConfirmPIN;

        protected async Task<byte[]> GetTransferData(ProfileDescription profile, string accountsDir)
        {
            string accountsData = "{" + SectionName_Profile + ":";

            accountsData += await KeyStoreUtils.LoadEncryptedProfileAsync(profile.ID, accountsDir);
            accountsData += ", " + SectionName_CustomData + ": \"0x0\"}";

            return Encoding.UTF8.GetBytes(accountsData);
        }



        string ParseToPin(byte[] publicKey) => publicKey.ToHex(false).Substring(0, 8).ToUpper();


        //NOTE that could be done as an iterator
        public override void Proceed()
        {
            switch (State)
            {
                case TransferState.Timeout:
                    State = TransferState.Ready;
                    break;
                    // Wait for user set everything on the oteher device
                    // Then connect
                case TransferState.Error:
                    State = TransferState.Ready;
                    break;
                case TransferState.Ready:
                    TimeOutCount = 0f;
                    PingAndConnect();
                    break;

                    // Aquire the confirmation PIN
                case TransferState.InputPIN:
                    // Wait for user to put the pin in the other device
                    break;

                case TransferState.ConfirmOtherPIN:
                    // Check DisplayOther
                    CheckConfirmation();
                    break;

                case TransferState.SendRecieve:
                    SendAccount();
                    break;

                case TransferState.Done:
                    Navigation.GoBackToView<CurrentUser>();
                    break;
            }
        }

        /// <summary>
        ///   Create connection to the whisper server
        /// </summary>
        public override void Connect()
        {
            if (!WaitingForUserProceed) return;
            WaitingForUserProceed = false;
            cancelToken = new System.Threading.CancellationTokenSource();
            string WhisperAddress = "ws://ws.eth-rpc.hoard.exchange";
            sync = CreateSync(WhisperAddress);
            PIN = ParseToPin(sync.PublicKey);
            State = TransferState.InputPIN;
            StartTimer();
            sync.Initialize(PIN, cancelToken.Token)
                .ContinueGUISynch(x =>
                {
                    if (x.IsFaulted || x.IsCanceled) return;
                    ErrorCallbackProvider.ReportInfo("Export: Initialize cofirmed");
                    State = TransferState.InputPIN;
                    GetHashReponse();
                });
        }

        private void GetHashReponse()
        {
            SyncKeeper.AcquireConfirmationHash(cancelToken.Token)
                .ContinueGUISynch(x =>
                {
                    if (x.IsFaulted || x.IsCanceled) return;
                    ErrorCallbackProvider.ReportInfo("Export: Hash Response");
                    SendPublicKey(x);
                });
        }

        private void SendPublicKey(Task<string> hashResponse)
        {
            SyncKeeper.SendPublicKey(cancelToken.Token)
                .ContinueGUISynch(x =>
                {
                    if (x.IsFaulted || x.IsCanceled)
                    {
                        if (State != TransferState.Timeout)
                        {
                            ErrorCallbackProvider.ReportInfo("Export: Reset on send public key");
                            Reset(TransferState.Error);
                        }
                    }
                    else
                    {
                        ErrorCallbackProvider.ReportInfo("Export: Public key sent");
                        State = TransferState.ConfirmOtherPIN;
                        PIN = hashResponse.Result;
                        WaitingForUserProceed = true;
                    }
                });
        }

        private void CheckConfirmation()
        {
            State = TransferState.SendRecieve;
            SendAccount();
        }

        private void SendAccount()
        {
            var path = ProfilesManagement.Instance.ProfilesDir;
            StopTimer();
            GetTransferData(transferProfile, path)
                .ContinueGUISynch(data => SyncKeeper.EncryptAndTransferKeystore(data.Result, cancelToken.Token)
                .ContinueGUISynch(task => State = TransferState.Done));
        }
    }
}
