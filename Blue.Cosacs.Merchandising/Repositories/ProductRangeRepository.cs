using AutoMapper;
using Blue.Cosacs.Merchandising.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Repositories
{
    public interface IProductRangeRepository
    {
        List<ProductStockRange> GetAll();
        ProductStockRange Get(int id);
        ProductStockRange Update(ProductStockRange model);
        ProductStockRange Save(ProductStockRange model);
    }
    /// <summary>
    /// Author : Rahul Dubey
    /// Date   : 15/02/2019
    /// CR     : #Ashley
    /// Details: Repository for Product stock Range.
    /// </summary>
    public class ProductRangeRepository : IProductRangeRepository
    {
        ProductStockRange IProductRangeRepository.Get(int id)
        {
            using (var scope = Context.Read())
            {
                return scope.Context.ProductStockRanges.ToList().Where(s => s.ProductId == id).FirstOrDefault();
                //return AshleySetups.Any() ? Mapper.Map<IEnumerable<AshleySetUp>>(AshleySetups) : new List<AshleySetUp>();
            }
        }

        List<ProductStockRange> IProductRangeRepository.GetAll()
        {
            using (var scope = Context.Read())
            {
                var ProductStockRanges = scope.Context.ProductStockRanges.ToList();
                return ProductStockRanges.Any() ? Mapper.Map<List<ProductStockRange>>(ProductStockRanges) : new List<ProductStockRange>();
            }
            //throw new NotImplementedException();
        }

        ProductStockRange IProductRangeRepository.Save(ProductStockRange model)
        {
            ProductStockRange ash;
            using (var scope = Context.Write())
            {
                ///
                //Check all validation for confirmation that its Ashley attribute has been set.
                //ProductAttribute ashDB = scope.Context.ProductAttributes.FirstOrDefault(p => p.ProductId == model.ProductId);
                //if (ashDB == null || !ashDB.IsAshleyProduct || ashDB.IsSpecialProduct)
                //    return null;
                scope.Context.ProductStockRanges.Add(model);
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
        ProductStockRange IProductRangeRepository.Update(ProductStockRange model)
        {
            using (var scope = Context.Write())
            {
                //Check all validation for confirmation that its Ashley attribute has been set.
                ProductStockRange ashPSR = scope.Context.ProductStockRanges.FirstOrDefault(p => p.ProductId == model.ProductId);
                ProductAttribute ashPA = scope.Context.ProductAttributes.FirstOrDefault(p => p.ProductId == model.ProductId);
                //if (ashPA==null||!ashPA.IsAshleyProduct || ashPA.IsSpecialProduct)
                //    return null;
                //if (ash.AshleyProduct != model.AshleyProduct && !ash.AshleyProduct)
                //    return null;
                ashPSR.MaxVal = model.MaxVal;
                ashPSR.MinVal = model.MinVal;
                ashPSR.SKU = model.SKU;
                scope.Context.SaveChanges();
                ashPSR = model;
                scope.Complete();
                return ashPSR;
            }
        }


    }
}
