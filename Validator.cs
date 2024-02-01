using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TestApp
{
    public class DoubleValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            //bool validated = true;
            //string doublePattern = @"^[+-]?[0-9]{1,307}(?:\.[0-9]{1,16})?$";
            //Regex regex = new Regex(doublePattern);
            //if (!regex.IsMatch(value as string))
            //{
            //    validated = false;
            //}
            double result = 0.0;
            bool canConvert = double.TryParse(value as string, out result);
            return new ValidationResult(canConvert, "Not a valid double");
        }
    }
}
