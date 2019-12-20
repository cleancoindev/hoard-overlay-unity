using System;
using System.ComponentModel;
using Hoard.ProfileUtilities;
using Hoard.MVC.Utilities;
using System.Linq;

namespace Hoard.MVC
{
    /// <summary>
    ///   Controller in charge of creating new user profile
    /// </summary>
    public class CreateUser : ViewController
    {
        private IProfilesManagement profilesService;

        //Seal
        private CreateUser() { }

        /// <summary>
        /// Ctor
        /// </summary>
        public CreateUser(IProfilesManagement profilesService)
        {
            Title = "TITLE_CREATE_NEW_USER".Translated();
            Password = new CredentialInputValidator(CredentialInputValidator.StandardLengthValidator);
            RepeatedPassword = new CredentialInputValidator(x => x == Password.Value
                                      && CredentialInputValidator.StandardLengthValidator(x));
            NewUserName = new CredentialInputValidator(x => CredentialInputValidator.StandardLengthValidator(x)
                                                       && !profilesService.AvailableProfileNames.Any(y => y.userName == x));

            this.profilesService = profilesService;
        }

        /// <summary>
        ///   Proposed new user name
        /// </summary>
        public CredentialInputValidator NewUserName { get; private set; }

        /// <summary>
        ///   New User Password
        /// </summary>
        public CredentialInputValidator Password { get; private set; }
            = new CredentialInputValidator(CredentialInputValidator.StandardLengthValidator);

        /// <summary>
        ///   Repeated password. has to mach Password internal value
        /// </summary>
        public CredentialInputValidator RepeatedPassword { get; private set; }
        // Initialization moved to constuctor as validator not compile time const

        /// <summary>
        ///   Are all fields filled correctly
        /// </summary>
        public bool AllCorrect => NewUserName.IsValid && Password.IsValid && RepeatedPassword.IsValid;

        /// <summary>
        ///   Start the user profile creation process
        /// </summary>
        public void RequestCreateAccount()
        {
            if (!AllCorrect) return;
            else
            {
                var nCredentials = new DecryptedCredentials(Password.Value, NewUserName.Value);
                profilesService.CreateProfile(nCredentials)
                   .ContinueGUISynch(x =>
                   {
                       if (x.IsFaulted)
                       {
                           Navigation.GoBack();
                       }
                       else
                       {
                           if (HoardSettings.AutoLogin)
                           {
                               if (HoardSettings.LastUser.userName != x.Result.Name)
                               {
                                   HoardSettings.AutoLogin = false;
                               }
                           }
                           HoardSettings.LastUser = new ProfileDescription(x.Result);
                           Navigation.OpenControlerWithNewRoot(new CurrentUser(x.Result));
                           ProfilesManagement.Instance.AddProfile(x.Result);
                       }
                   });

                Navigation.Open(new WaitForTask());
            }
        }
    }

    /// <summary>
    ///   This is a helper class that helps encapsulate the input verification from the user and expose the state of said verification
    /// </summary>
    public class CredentialsObserver : IDisposable
    {
        private CredentialInputValidator observed;

        /// <summary>
        /// Currently observe credentials
        /// </summary>
        public CredentialInputValidator Observed { get => observed; }

        /// <summary>
        ///   Ctor
        /// <param name="observed">Subject for verification</param>
        /// </summary>
        public CredentialsObserver(CredentialInputValidator observed)
        {
            this.observed = observed;
            this.observed.PropertyChanged += HandleChange;
            HandleChange(this.observed, null);
        }

        /// <summary>
        ///   Sets the observer internal state according to the current @Observed status
        /// </summary>
        public CredentialsObserver Setup()
        {
            HandleChange(observed, null);
            return this;
        }

        private void HandleChange(object sender, PropertyChangedEventArgs e)
        {
            if (observed.IsEmpty)
                onEmptyValue?.Invoke(observed.Value);
            else if (observed.IsValid)
                onValidValue?.Invoke(observed.Value);
            else if (observed.IsIncorrect)
                onInvalidValue?.Invoke(observed.Value);
            onValueChanged?.Invoke(observed.Value);
        }

        /// <summary>
        ///   Called if value correct
        /// </summary>
        public Action<string> onValidValue;

        /// <summary>
        ///   Called if value incorrect
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Reverse of the @onValidValue
        ///   </para>
        /// </remarks>
        public Action<string> onInvalidValue;

        /// <summary>
        /// Called if observed value become empty or null string
        /// </summary>
        public Action<string> onEmptyValue;

        /// <summary>
        ///   Callback when value changes in any way
        /// </summary>
        public Action<string> onValueChanged;

        public void Dispose()
        {
            this.observed.PropertyChanged -= HandleChange;
        }
    }
}
