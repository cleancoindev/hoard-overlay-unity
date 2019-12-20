using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Text;
using System.Timers;
using System.Net.NetworkInformation;
using System.Threading;

namespace Hoard.MVC
{
    /// <summary>
    ///   Base object for the import and export procedure
    /// </summary>
    public abstract class Procedure : INotifyPropertyChanged
    {
        static public readonly string SectionName_Profile = "profile";
        static public readonly string SectionName_Accounts = "accounts";
        static public readonly string SectionName_CustomData = "customdata";

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChange([CallerMemberName]string name = default)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected string WhisperAddress = "ws://ws.eth-rpc.hoard.exchange";

        protected AccountSynchronizer sync;

        protected System.Threading.CancellationTokenSource cancelToken;

        private bool canProceed;

        /// <summary>
        ///   Is porcedure waiting for user to move forward
        /// </summary>
        public bool WaitingForUserProceed
        {
            get
            {
                return canProceed;
            }
            protected set
            {
                canProceed = value;
                NotifyChange();
            }
        }

        private TransferState state;

        /// <summary>
        ///   Current transfer state of the procedure
        /// </summary>
        public TransferState State
        {
            get => state;
            protected set
            {
                state = value;
                NotifyChange();
            }
        }

        private System.Timers.Timer t;
        public float TimeOutCount { get; protected set; }

        /// <summary>
        ///  Time before procedure will shutdown and reset. 
        /// </summary>
        public const float TimeOutMax = 90f;

        /// <summary>
        ///   Begin countdown
        /// </summary>
        protected void StartTimer()
        {
            TimeOutCount = 0f;
            t = new System.Timers.Timer(1000d);
            t.Elapsed += ElapsedEventHandler;
            t.Start();
        }

        protected void Reset(TransferState followState)
        {
            Shutdown().ContinueGUISynch(x =>
            {
                State = followState;
                WaitingForUserProceed = true;
            });
        }

        public abstract void ProvideInput(string input);

        public async Task Shutdown()
        {
            if (cancelToken != null)
            {
                try
                {
                    cancelToken.Cancel();
                    cancelToken.Dispose();
                }
                catch (ObjectDisposedException e)
                {
                    ErrorCallbackProvider.ReportError("Exception during shutdown: " + e.ToString());
                }
                // We move forward even with exceptions
                cancelToken = null;
            }

            if (sync != null)
            {
                StopTimer();
                await sync.Shutdown(CancellationToken.None);
                sync = null;
            }
        }

        private string ParseToPin(byte[] publicKey) => publicKey.ToHex(false).Substring(0, 8).ToUpper();

        /// <summary>
        ///   Countdown towared the reset. Callback to the timeout Timer
        /// </summary>
        public void ElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            TimeOutCount += 1f;
            if (TimeOutMax <= TimeOutCount)
            {
                TaskRunner.ExecuteDuringNextPool(() => Reset(TransferState.Timeout));
            }
        }

        protected void PingAndConnect()
        {
            var whisper = WhisperAddress.Replace("ws://", "");
            var waiter = new AutoResetEvent(true);
            Ping ping = new Ping();
            ping.PingCompleted += PingCallback;
            PingOptions opt = new PingOptions();
            byte[] data = Encoding.ASCII.GetBytes("TestPing");
            try
            {
                ping.SendAsync(whisper, 400, data, opt, waiter);
            }
            catch (System.Exception e)
            {
                State = TransferState.Error;
                ErrorCallbackProvider.ReportError("Server is not responding. Probably network issue: \n" + e.ToString());
            }
        }

        protected void StopTimer()
        {
            TimeOutCount = 0f;
            t.Elapsed -= ElapsedEventHandler;
            t.Stop();
        }

        private void PingCallback(object sender, PingCompletedEventArgs a)
        {
            if (a.Error != null)
            {
                TaskRunner.ExecuteDuringNextPool(() => State = TransferState.Error);
            }
            else
            {
                TaskRunner.ExecuteDuringNextPool(Connect);
            }
        }

        public abstract void Connect();

        public abstract void Proceed();

        /// <summary>
        ///   Describes current state of Profile Synchronisation
        /// </summary>
        public enum TransferState
        {
            /// <summary>
            ///   Error has been encountered
            /// </summary>
            Error,

            /// <summary>
            ///   Procedure has reached timeout and has reset aumotaticaly itself
            /// </summary>
            Timeout,

            /// <summary>
            ///   Ready to start synch procedure
            /// </summary>
            Ready,

            /// <summary>
            ///   Waiting for the user to input PIN in the import client
            /// </summary>
            InputPIN,

            /// <summary>
            ///   Devices check if pin is correct
            /// </summary>
            CheckPin,

            /// <summary>
            ///   Waiting for user to confirm that export machine displays correct confirmation PIN
            /// </summary>
            ConfirmOtherPIN,

            /// <summary>
            ///   Transfer of data in progress
            /// </summary>
            SendRecieve,

            /// <summary>
            ///   Procedure finished succesfully
            /// </summary>
            Done
        }
    }
}
