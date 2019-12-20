using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Hoard.MVC.Utilities
{
    /// <summary>
    ///   Observes the internal string input and shows the result of the validation funcition that was passed
    /// </summary>
    public class CredentialInputValidator : INotifyPropertyChanged
    {
        public static bool StandardLengthValidator(string x)
            => x != null && x.Length > 2 && x.Length <= 32;

        /// <summary>
        ///   INotifyPropertyChanged event exposed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChange([CallerMemberName]string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        ///   Validator function
        /// </summary>
        public Func<string, bool> Validator {get; private set;}

        private string inputValue = "";

        private CredentialInputValidator()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">Validation function</param>
        public CredentialInputValidator(Func<string, bool> validator)
        {
            IsEmpty = true;
            Validator = validator;
        }

        /// <summary>
        ///   Is value empty
        /// </summary>
        public bool IsEmpty { get; private set; }

        /// <summary>
        ///   Is value incorrect
        /// </summary>
        public bool IsIncorrect { get; private set; }

        /// <summary>
        ///   is value correct and not empty
        /// </summary>
        public bool IsValid => !(IsEmpty || IsIncorrect);


        /// <summary>
        /// Call validation function and set the state of object
        /// </summary>
        /// <returns>Is internal value Valid</returns>
        public bool Validate()
        {
            IsEmpty = (string.IsNullOrEmpty(inputValue));
            IsIncorrect = !Validator(inputValue);
            NotifyChange(nameof(Value));
            return IsValid;
        }

        /// <summary>
        ///   Internal string value
        /// </summary>
        public string Value
        {
            get => inputValue;
            set
            {
                if (inputValue == value) return;
                inputValue = value;
                Validate();
            }
        }
    }
}
