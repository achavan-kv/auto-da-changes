/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Queries.PreQualifications
{
    public class PreQualificationsValidator : AbstractValidator<PreQualificationQuery>
    {
        public PreQualificationsValidator()
        {
            RuleFor(x => x.SalesCustomerId).NotNull().NotEmpty().MaximumLength(20);
        }
    }
}
