/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;
using System.Linq;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Models.Products;

namespace Unicomer.Cosacs.Business.Commands.SelectProduct
{
    public class SelectProductValidator : AbstractValidator<SelectProductCommand>
    {
        public SelectProductValidator()
        {
            RuleFor(x => x.ItemsAvailable).NotNull().DependentRules(() => {
                RuleFor(x => x.ItemsAvailable).SetValidator(new ProductServiceModelValidator());
            });
        }
    }

    public class ProductServiceModelValidator : AbstractValidator<ProductServiceModel>
    {
        public ProductServiceModelValidator()
        {
            RuleFor(x => x.CountryIsoCode).NotNull().Length(2);
            RuleFor(x => x.Items).NotNull().DependentRules(() => {
                RuleFor(x => x.Items)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() => {
                    RuleForEach(x => x.Items).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() => {
                        RuleForEach(x => x.Items).SetValidator(new ProductServiceItemValidator());
                    });
                });
            });

        }
    }

    public class ProductServiceItemValidator : AbstractValidator<ProductServiceItem>
    {
        public ProductServiceItemValidator()
        {
            RuleFor(x => x.StoreId).NotNull().NotEmpty();
            RuleFor(x => x.Sku).NotNull().NotEmpty();
        }
    }
}
