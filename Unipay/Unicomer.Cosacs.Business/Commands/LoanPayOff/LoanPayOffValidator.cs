/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Commands.LoanPayOff
{
    public class LoanPayOffValidator : AbstractValidator<LoanPayOffCommand>
    {
        public LoanPayOffValidator()
        {
            RuleFor(x => x.CosacsAccountId).NotNull().NotEmpty().Length(12);
        }
    }
}
