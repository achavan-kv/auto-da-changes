/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Queries.Store
{
    public class StoreValidator : AbstractValidator<StoreQuery>
    {
        public StoreValidator()
        {
            RuleFor(x => x.CountryIsoCode).NotNull().NotEmpty().MaximumLength(2);
        }
    }
}
