/* 
Author: Suresh -IGT
Date: Feb 2st
Description:save Customer API 
 */

using FluentValidation;
using System.Linq;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Model.Models.Customers;

namespace Unicomer.Cosacs.Business.Commands.SaveCustomer
{
    public class SaveCustomerValidator : AbstractValidator<SaveCustomerCommand>
    {
        public SaveCustomerValidator()
        {
            RuleFor(x => x.Customer).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Customer.CustomerDetail).NotNull().DependentRules(() =>
                {
                    RuleFor(x => x.Customer.CustomerDetail).SetValidator(new CustomerDetailValidator());
                });
            });
        }
    }

    public class CustomerDetailValidator : AbstractValidator<CustomerDetail>
    {
        public CustomerDetailValidator()
        {
            RuleFor(x => x.CreditCustomerId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.LineOfCreditId).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.SalesCustomerId).NotNull().MaximumLength(20);
            RuleFor(x => x.Nationality).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.CountryIsoCode).NotNull().NotEmpty().MaximumLength(2);
            RuleFor(x => x.FirstName).NotNull().NotEmpty().MaximumLength(30);
            RuleFor(x => x.LastName).NotNull().NotEmpty().MaximumLength(60);
            RuleFor(x => x.SecondLastName).NotNull().MaximumLength(50);
            RuleFor(x => x.DateOfBirth).NotNull().NotEmpty().MaximumLength(12);
            RuleFor(x => x.StoreId).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.Gender).NotNull().NotEmpty().MaximumLength(1);
            RuleFor(x => x.CountryId).NotNull().MaximumLength(2);
            RuleFor(x => x.NumberOfChildren).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaritalStatus).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.IncomesOther).GreaterThanOrEqualTo(0);
            RuleFor(x => x.NumberOfDependents).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Salutation).NotNull().MaximumLength(25);
            RuleFor(x => x.Email).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Email)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() =>
                {
                    RuleForEach(x => x.Email).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() =>
                    {
                        RuleForEach(x => x.Email).SetValidator(new EmailValidator());
                    });
                });
            });

            RuleFor(x => x.Identification).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Identification)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() =>
                {
                    RuleForEach(x => x.Identification).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() =>
                    {
                        RuleForEach(x => x.Identification).SetValidator(new IdentificationValidator());
                    });
                });
            });

            RuleFor(x => x.Phone).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Phone)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() =>
                {
                    RuleForEach(x => x.Phone).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() =>
                    {
                        RuleForEach(x => x.Phone).SetValidator(new PhoneValidator());
                    });
                });
            });

            RuleFor(x => x.Work).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Work)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() =>
                {
                    RuleForEach(x => x.Work).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() =>
                    {
                        RuleForEach(x => x.Work).SetValidator(new WorkValidator());
                    });
                });
            });

            RuleFor(x => x.PersonalReference).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.PersonalReference)
                .Must(x => x.Any())
                .WithMessage(Constants.CollectionNotNulllorEmptyMessage)
                .DependentRules(() =>
                {
                    RuleForEach(x => x.PersonalReference).NotNull().WithMessage(Constants.CollectionIndexNotNulllorEmptyMessage).DependentRules(() =>
                    {
                        RuleForEach(x => x.PersonalReference).SetValidator(new PersonalReferenceValidator());
                    });
                });
            });
        }
    }

    public class EmailValidator : AbstractValidator<Email>
    {
        public EmailValidator()
        {
            RuleFor(x => x.EmailDetail).NotNull().NotEmpty().MaximumLength(100);
        }
    }

    public class IdentificationValidator : AbstractValidator<Identification>
    {
        public IdentificationValidator()
        {
            RuleFor(x => x.IdentificationType).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.IdentificationId).NotNull().NotEmpty().MaximumLength(30);
        }
    }

    public class PhoneValidator : AbstractValidator<Phone>
    {
        public PhoneValidator()
        {
            RuleFor(x => x.PhoneType).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.PhoneNumber).NotNull().NotEmpty().MaximumLength(20);
        }
    }

    public class WorkValidator : AbstractValidator<Work>
    {
        public WorkValidator()
        {
            RuleFor(x => x.HiringDate).NotNull().NotEmpty().MaximumLength(12);
            RuleFor(x => x.CompanyName).NotNull().NotEmpty().MaximumLength(30);
            RuleFor(x => x.Position).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.Profession).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Salary).GreaterThanOrEqualTo(0);
        }
    }

    public class PersonalReferenceValidator : AbstractValidator<PersonalReference>
    {
        public PersonalReferenceValidator()
        {
            RuleFor(x => x.ReferenceType).NotNull().NotEmpty().MaximumLength(1);
            RuleFor(x => x.ReferenceName).NotNull().NotEmpty().MaximumLength(20);
            RuleFor(x => x.ReferencePhoneNumber).NotNull().NotEmpty().MaximumLength(20);
        }
    }

    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.StateRegion).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.City).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Neighborhood).NotNull().MaximumLength(100);
            RuleFor(x => x.AddressLine).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.AddressType).NotNull().NotEmpty().MaximumLength(1);
            RuleFor(x => x.TimeLivingCurrentAddress).NotNull().NotEmpty();
            RuleFor(x => x.HomeType).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.ZipCode).NotNull().NotEmpty().MaximumLength(10);
        }
    }
}
