
/* 
Author:  
Date:  
Description:POST Select Product Service  API 
 */
using System.Collections.Generic;
using Unicomer.Cosacs.Model.Interfaces;
using Unicomer.Cosacs.Model.Models.Products;
using Unicomer.Cosacs.Model.ViewModels;

namespace Unicomer.Cosacs.Repository.Interfaces
{
    public interface ISelectProductRepository
    {
        List<SelectProductResponseModel> Select(ProductServiceModel product, IApiRequest apiRequest);
    }
}
