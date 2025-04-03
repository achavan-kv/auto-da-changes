/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Commands.ProcessDelivery
{
    public class ProcessDeliveryValidator : AbstractValidator<ProcessDeliveryCommand>
    {
        public ProcessDeliveryValidator()
        {
            RuleFor(x => x.CosacsAccountId).NotNull().NotEmpty().Length(12);
            RuleFor(x => x.InvoiceNumber).NotNull().NotEmpty();
        }
    }
}
