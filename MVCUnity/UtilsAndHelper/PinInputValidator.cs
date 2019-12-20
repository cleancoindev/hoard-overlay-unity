using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

namespace Hoard.MVC.Unity
{
    public class PinInputValidator : TMP_InputValidator
    {
        Regex Regex = new Regex("^[0-9a-fA-F]{8}$");
        public override char Validate(ref string text, ref int pos, char ch)
        {
            return text.Length < 8 && Regex.IsMatch(text) ? ch : '\0';
        }

    }
}
