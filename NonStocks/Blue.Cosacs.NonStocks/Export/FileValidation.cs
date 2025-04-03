using System;

namespace Blue.Cosacs.NonStocks.Export
{
    public class FileValidation
    {
        protected void ValidateInt16Value(short value, string propName, short maxValue)
        {
            if (value < -maxValue || value > maxValue)
            {
                var strError = "The number {0} cannot have more than {1} digits.";
                if (maxValue == 999)
                {
                    throw new ArgumentException(string.Format(strError, propName, "three"));
                }
                else throw new Exception("NOT SUPPORTED");
            }
        }

        /// <summary>
        /// why do you create a constant if you gonna use it only once?
        /// </summary>
        private const decimal MaxPriceValue = 99999999m;
        protected void ValidateDecimalPriceValue(decimal value, string propName)
        {
            if (value < -MaxPriceValue || value > 99999999m)
            {
                var strError = "The price {0} cannot have more than {1} digits.";
                throw new ArgumentException(string.Format(strError, propName, "eight"));
            }
        }

        protected void ValidateStringLength(string value, string propName, int maxLength)
        {
            if (value.Length > maxLength)
            {
                throw new ArgumentException("The " + propName + " max length is " + maxLength + ".");
            }
        }

        protected void ValidateCharYesNo(char value, string propName, bool ignoreSpace = false)
        {
            if (ignoreSpace && value == ' ')
            {
                return;
            }

            value = value.ToString().ToUpper()[0];
            if (value != 'Y' && value != 'N')
            {
                throw new ArgumentException("Invalid " +
                    propName + ", only 'Y' and 'N' are valid values.");
            }
        }

        protected void ValidateDate(DateTime? date, string propName)
        {
            if (!date.HasValue)
            {
                return;
            }
            else
            {
                if (date.Value < new DateTime(1900, 1, 1) ||
                    date.Value >= new DateTime(2074, 12, 31))
                {
                    throw new ArgumentException("Invalid " +
                        propName + ", date out of range (1900/01/01 - 2074/12/31");
                }
            
            }

        }
    }
}
