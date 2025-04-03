/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Commands.LoanRepayment
{
    public class LoanRepaymentValidator : AbstractValidator<LoanRepaymentCommand>
    {
        public LoanRepaymentValidator()
        {
            RuleFor(x => x.CosacsAccountId).NotNull().NotEmpty().Length(12);
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }
}
