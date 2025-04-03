using System;

namespace Blue.Cosacs.Merchandising.Validators
{
    using System.ComponentModel.DataAnnotations;

    public class FutureDateAttribute : ValidationAttribute
    {
        private readonly string message;
        public FutureDateAttribute(string message = null)
        {
            this.message = message;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            var dt = (DateTime)value;
            if (dt.Date >= DateTime.Now.Date)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(message ?? "Please select a future date");
        }
    }
}
