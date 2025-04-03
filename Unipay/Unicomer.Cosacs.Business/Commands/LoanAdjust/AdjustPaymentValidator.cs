/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Commands.LoanAdjust
{
    public class AdjustPaymentValidator : AbstractValidator<LoanAdjustCommand>
    {
        public AdjustPaymentValidator()
        {
            RuleFor(x => x.CosacsAccountId).NotNull().NotEmpty().Length(12);
            RuleFor(x => x.LoanTransactionId).NotNull().NotEmpty();
            RuleFor(x => x.Notes).NotNull().NotEmpty();
        }
    }
}
