using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface IAshleySetUpRepository
    {
        List<ProductAttribute> GetAll();
        ProductAttribute Get(int id);
        ProductAttribute Update(ProductAttribute model);
        ProductAttribute Save(ProductAttribute model);
    }
    /// <summary>
    /// Author : Rahul Dubey
    /// Date   : 15/02/2019
    /// CR     : #Ashley
    /// Details: Repository for Ashley Product Attributes.
    /// </summary>
    public class AshleySetUpRepository : IAshleySetUpRepository
    {
        ProductAttribute IAshleySetUpRepository.Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductAttributes.ToList().Where(s => s.ProductId == id).FirstOrDefault();
            }
        }

        List<ProductAttribute> IAshleySetUpRepository.GetAll()
        {
            using (var scope = Context.Read())
            {
                var productAttributes = scope.Context.ProductAttributes.ToList();
                return productAttributes.Any() ? Mapper.Map<List<ProductAttribute>>(productAttributes) : new List<ProductAttribute>();
            }
        }

        ProductAttribute IAshleySetUpRepository.Save(ProductAttribute model)
        {
            ProductAttribute ash;
            using (var scope = Context.Write())
            {
                scope.Context.ProductAttributes.Add(model);
                scope.Context.SaveChanges();
                ash = model;
                scope.Complete();
                return ash;
            }
        }

        ProductAttribute IAshleySetUpRepository.Update(ProductAttribute model)
        {
            using(var scope = Context.Write())
            {
                ProductAttribute ash = scope.Context.ProductAttributes.FirstOrDefault(p => p.ProductId == model.ProductId);
           
                ash.IsAshleyProduct = model.IsAshleyProduct;
                ash.IsSpecialProduct  = model.IsSpecialProduct;
                ash.ISOnlineAvailable = model.ISOnlineAvailable;
                ash.ISAutoPO = model.ISAutoPO;
                ash.SKU = model.SKU;
                scope.Context.SaveChanges();
                ash = model;
                scope.Complete();
                return ash;
            }
        }

        int CastNullable(int? val)
        {
            return val ?? 0;
        }
    }
}
