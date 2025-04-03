/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Queries.Qualification
{
    public class QualificationValidator : AbstractValidator<QualificationQuery>
    {
        public QualificationValidator()
        {
            RuleFor(x => x.CountryIsoCode).NotNull().NotEmpty().MaximumLength(2);
            RuleFor(x => x.SalesCustomerId).NotNull().NotEmpty().MaximumLength(20);
        }
    }
}
