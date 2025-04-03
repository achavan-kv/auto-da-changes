/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;
using System;
using System.Linq;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Models.Payments;

namespace Unicomer.Cosacs.Business.Commands.UpdateInvoice
{
    public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceCommand>
    {
        public UpdateInvoiceValidator()
        {
            RuleFor(x => x.UpdatePaymentRequest).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.UpdatePaymentRequest.CountryIsoCode).NotNull().NotEmpty().MaximumLength(2);
                RuleFor(x => x.UpdatePaymentRequest.InvoiceNumber).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.UpdatePaymentRequest.CreditCustomerId).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.UpdatePaymentRequest.LineOfCreditId).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.UpdatePaymentRequest.CreditAccountId).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(x => x.UpdatePaymentRequest.SalesCustomerId).NotNull().NotEmpty().MaximumLength(20);
                RuleFor(x => x.UpdatePaymentRequest.TotalLoan).GreaterThan(0);
                RuleFor(x => x.UpdatePaymentRequest.NumberInstallments).GreaterThan(0);
                RuleFor(x => x.UpdatePaymentRequest.InstallmentValue).GreaterThan(0);
                RuleFor(x => x.UpdatePaymentRequest.LastInstallmentValue).GreaterThan(0);
                RuleFor(x => x.UpdatePaymentRequest.PaymentStartDate).GreaterThan(DateTime.Now.AddYears(-50)).WithMessage("PaymentStartDate should not be less than 50 years old");
                RuleFor(x => x.UpdatePaymentRequest.AnnualRate).GreaterThanOrEqualTo(0);
                RuleFor(x => x.UpdatePaymentRequest.Fee).NotNull().DependentRules(() => {
                    RuleFor(x => x.UpdatePaymentRequest.Fee).SetValidator(new InvoiceFeeValidator());
                });
            });
        }
    }

    public class InvoiceFeeValidator : AbstractValidator<InvoiceFee>
    {
        public InvoiceFeeValidator()
        {
            RuleFor(x => x.DetailFee).NotNull().DependentRules(() => {
                RuleFor(x => x.DetailFee)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() => {
                    RuleForEach(x => x.DetailFee).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() => {
                        RuleForEach(x => x.DetailFee).SetValidator(new InvoiceDetailFeeValidator());
                    });
                });
            });

            RuleFor(x => x.DetailTaxFee).NotNull().DependentRules(() => {
                RuleFor(x => x.DetailTaxFee)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() => {
                    RuleForEach(x => x.DetailTaxFee).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() => {
                        RuleForEach(x => x.DetailTaxFee).SetValidator(new InvoiceDetailFeeValidator());
                    });
                });
            });
        }
    }

    public class InvoiceDetailFeeValidator : AbstractValidator<InvoiceDetailFee>
    {
        public InvoiceDetailFeeValidator()
        {
            RuleFor(x => x.CodeFee).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.ValueFee).GreaterThanOrEqualTo(0);
        }
    }
}
