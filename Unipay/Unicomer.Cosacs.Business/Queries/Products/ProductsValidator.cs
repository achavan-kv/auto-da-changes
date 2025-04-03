/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;

namespace Unicomer.Cosacs.Business.Queries.Products
{
    public class ProductsValidator : AbstractValidator<ProductsQuery>
    {
        public ProductsValidator()
        {
            RuleFor(x => x.CountryIsoCode).NotNull().NotEmpty().MaximumLength(2);
        }
    }
}
