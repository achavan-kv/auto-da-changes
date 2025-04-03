/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Commands.LoanSchedule
{
    public class LoanScheduleValidator : AbstractValidator<LoanScheduleCommand>
    {
        public LoanScheduleValidator()
        {
            RuleFor(x => x.CosacsAccountId).NotNull().NotEmpty().Length(12);
        }
    }
}
