/* 
Author: Suresh -IGT
Date: Feb 1st
Description:JM BlueStart-Pre-Qualification API 
 */
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Customers;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        PreQualificationModel PreQualification(string salesCustomerId, IApiRequest apiRequest);
        CustomerResultModel Save(CustomerModel customer, IApiRequest apiRequest);
        QualificationResponseModel Qualification(string countryIsoCode, string salesCustomerId, IApiRequest apiRequest);
    }
}
