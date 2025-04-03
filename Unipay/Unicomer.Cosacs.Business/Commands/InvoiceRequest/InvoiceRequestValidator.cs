/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Commands.InvoiceRequest
{
    public class InvoiceRequestValidator : AbstractValidator<InvoiceRequestCommand>
    {
        public InvoiceRequestValidator()
        {
            RuleFor(x => x.CosacsAccountId).NotNull().NotEmpty().Length(12);
            RuleFor(x => x.InvoiceNumber).NotNull().NotEmpty();
        }
    }
}
