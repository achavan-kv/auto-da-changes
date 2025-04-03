/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;
using System;
using System.Globalization;
using System.Linq;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Models.Orders;

namespace Unicomer.Cosacs.Business.Commands.SaveSalesOrder
{
    public class SaveSalesOrderValidator : AbstractValidator<SaveSalesOrderCommand>
    {
        public SaveSalesOrderValidator()
        {
            RuleFor(x => x.SalesOrder).NotNull().DependentRules(() => {
                RuleFor(x => x.SalesOrder).SetValidator(new SalesOrderModelValidator());
            });
        }
    }

    public class SalesOrderModelValidator : AbstractValidator<SalesOrderModel>
    {
        public SalesOrderModelValidator()
        {
            RuleFor(x => x.SalesOrderId).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.StoreId).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.SalePerson).NotNull().NotEmpty().MaximumLength(100);
            RuleFor(x => x.InvoiceDate).NotNull().NotEmpty().DependentRules(() => {
                RuleFor(x => x.InvoiceDate).Must(invoiceDate => ValidateDate(invoiceDate))
                    .WithMessage("InvoiceDate is not correct format(dd-MM-yyyy HH:mm:ss)");
            });
                
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SalesCustomerId).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.DocumentNumber).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.FullName).NotNull().NotEmpty().MaximumLength(100);
            RuleFor(x => x.Phone).NotNull().MaximumLength(20);
            RuleFor(x => x.Delivery).NotNull();
            RuleFor(x => x.CellPhone).NotNull().MaximumLength(20);
            RuleFor(x => x.CreditCustomerId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.LineOfCreditId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.ReceiptData).NotNull().DependentRules(() => {
                RuleFor(x => x.ReceiptData.CreditDetail).NotNull().DependentRules(() => {
                    RuleFor(x => x.ReceiptData.CreditDetail).SetValidator(t=>new CreditDetailValidator(t));
                });
            });
        }

        public bool ValidateDate(string date)
        {
            DateTime invoiceDate;

            if (string.IsNullOrWhiteSpace(date) || !DateTime.TryParseExact(date.Trim(), new string[] { "dd-MM-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out invoiceDate))
            {
                return false;
            }
            return true;
        }
    }

    public class CreditDetailValidator : AbstractValidator<CreditDetail>
    {
        public CreditDetailValidator(SalesOrderModel salesOrder)
        {
            RuleFor(x => x.CreditAccountId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.TypeCreditProduct).NotNull().NotEmpty().MaximumLength(50);//cashloan or HP
            RuleFor(x => x.TotalLoan).GreaterThan(0);
            RuleFor(x => x.NumberInstallments).GreaterThan(0);
            RuleFor(x => x.InstallmentValue).GreaterThan(0);
            RuleFor(x => x.LastInstallmentValue).GreaterThan(0);
            RuleFor(x => x.PaymentStartDate).GreaterThan(DateTime.Now.AddYears(-50)).WithMessage("PaymentStartDate should not be less than 50 years old");
            RuleFor(x => x.AnnualRate).GreaterThanOrEqualTo(0);

            RuleFor(x => x.Fee).NotNull().DependentRules(() => {
                RuleFor(x => x.Fee).SetValidator(new CreditFeeValidator());
            });

            RuleFor(x => x.DeliveryAddress).NotNull().DependentRules(() => {
                RuleFor(x => x.DeliveryAddress).SetValidator(new DeliveryAddressValidator(salesOrder));
            });

            RuleFor(x => x.ProductDetail).NotNull().DependentRules(() => {
                RuleFor(x => x.ProductDetail)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() => {
                    RuleForEach(x => x.ProductDetail).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() => {
                        RuleForEach(x => x.ProductDetail).SetValidator(new ProductDetailValidator());
                    });
                });
            });
        }
    }

    public class ProductDetailValidator : AbstractValidator<ProductDetail>
    {
        public ProductDetailValidator()
        {
            RuleFor(x => x.ProductData).NotNull().DependentRules(() => {
                RuleFor(x => x.ProductData).SetValidator(new ProductDataValidator());
            });
        }
    }

    public class ProductDataValidator : AbstractValidator<ProductData>
    {
        public ProductDataValidator()
        {
            RuleFor(x => x.UPC).NotNull().MaximumLength(50);
            RuleFor(x => x.SKU).NotNull().MaximumLength(20);
            RuleFor(x => x.UPCVendor).NotNull().MaximumLength(50);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        }
    }

    public class CreditFeeValidator : AbstractValidator<CreditFee>
    {
        public CreditFeeValidator()
        {
            RuleFor(x => x.DetailFee).NotNull().DependentRules(() => {
                RuleFor(x => x.DetailFee)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() => {
                    RuleForEach(x => x.DetailFee).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() => {
                        RuleForEach(x => x.DetailFee).SetValidator(new DetailFeeValidator());
                    });
                });
            });

            RuleFor(x => x.DetailTaxFee).NotNull().DependentRules(() => {
                RuleFor(x => x.DetailTaxFee)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() => {
                    RuleForEach(x => x.DetailTaxFee).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() => {
                        RuleForEach(x => x.DetailTaxFee).SetValidator(new DetailFeeValidator());
                    });
                });
            });
        }
    }

    public class DeliveryAddressValidator : AbstractValidator<DeliveryAddress>
    {
        public DeliveryAddressValidator(SalesOrderModel salesOrder)
        {
            if(salesOrder.Delivery??false)
            {
                RuleFor(x => x.Region).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.State).NotNull().NotEmpty().MaximumLength(20);
                RuleFor(x => x.City).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.DeliveryArea).NotNull().NotEmpty().MaximumLength(35);
                RuleFor(x => x.AddressLine).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.ZipCode).NotNull().NotEmpty().MaximumLength(20);
                RuleFor(x => x.DeliveryContact).NotNull();
            }
            else
            {
                RuleFor(x => x.Region).NotNull().MaximumLength(50);
                RuleFor(x => x.State).NotNull().MaximumLength(20);
                RuleFor(x => x.City).NotNull().MaximumLength(50);
                RuleFor(x => x.DeliveryArea).NotNull().MaximumLength(35);
                RuleFor(x => x.AddressLine).NotNull().MaximumLength(50);
                RuleFor(x => x.ZipCode).NotNull().MaximumLength(20);
                RuleFor(x => x.DeliveryContact).NotNull();
            }
           
        }
    }
    public class DeliveryContactValidator : AbstractValidator<DeliveryContact>
    {
        public DeliveryContactValidator(bool isDelivery)
        {
            if (isDelivery)
            {
                RuleFor(x => x.TitleDelivery).NotNull().NotEmpty().MaximumLength(20);
                RuleFor(x => x.FirstName).NotNull().NotEmpty().MaximumLength(20);
                RuleFor(x => x.LastName).NotNull().NotEmpty().MaximumLength(20);
            }
            else
            {
                RuleFor(x => x.TitleDelivery).NotNull().MaximumLength(20);
                RuleFor(x => x.FirstName).NotNull().MaximumLength(20);
                RuleFor(x => x.LastName).NotNull().MaximumLength(20);
            }

            RuleFor(x => x.Cellphone).NotNull().MaximumLength(20);
        }
    }

    public class DetailFeeValidator : AbstractValidator<DetailFee>
    {
        public DetailFeeValidator()
        {
            RuleFor(x => x.CodeFee).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.ValueFee).GreaterThanOrEqualTo(0);
        }
    }
}
