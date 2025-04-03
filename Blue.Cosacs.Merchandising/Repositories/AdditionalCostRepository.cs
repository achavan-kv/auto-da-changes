using Blue.Cosacs.Merchandising.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Repositories
{

    public interface IAdditionalCostRepository
    {
        AdditionalCostPrice Get(int id);

        List<AdditionalCostPrice> GetByProduct(int productId);

        List<AdditionalCostPrice> GetByVendorDetail(string vendorId);

        bool Delete(int productId);
        bool Save(List<AdditionalCostPrice> model, string user);
    }
    /// <summary>
    /// Author : Rahul Dubey
    /// Date   : 15/02/2019
    /// CR     : #Ashley
    /// Details: Repository for Mutliple cost Prices
    /// </summary>

    public class AdditionalCostRepository : IAdditionalCostRepository
    {
        bool IAdditionalCostRepository.Delete(int productId)
        {
            using (var scope = Context.Write())
            {
                var costsToRemove = scope.Context.AdditionalCostPrices.Where(cp => cp.ProductId == productId).ToList();
                if (costsToRemove != null) {
                    foreach (AdditionalCostPrice cp in costsToRemove)
                    {
                        var CpAudit = new AdditionalCostPriceAudit();
                        CpAudit.AdditionalCostId = cp.Id;
                        CpAudit.AddedBy = cp.AddedBy;
                        CpAudit.AverageWeightedCost = cp.AverageWeightedCost;
                        CpAudit.DateAdded = cp.DateAdded;
                        CpAudit.LastLandedCost = cp.LastLandedCost;
                        CpAudit.ProductId = cp.ProductId;
                        CpAudit.SupplierCost = cp.SupplierCost;
                        CpAudit.SupplierCurrency = cp.SupplierCurrency;
                        CpAudit.VendorId = cp.VendorId;
                        scope.Context.AdditionalCostPriceAudits.Add(CpAudit);
                        scope.Context.AdditionalCostPrices.Remove(cp);
                    }
                    try
                    {
                        scope.Context.SaveChanges();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                
                return true;
            }
        }

        AdditionalCostPrice IAdditionalCostRepository.Get(int id)
        {
            using (var scope = Context.Read())
            {
                var cost = scope.Context.AdditionalCostPrices.FirstOrDefault(cp => cp.Id == id);

                return cost;
            }
        }

        List<AdditionalCostPrice> IAdditionalCostRepository.GetByProduct(int productId)
        {
            using (var scope = Context.Read())
            {
                var costs = scope.Context.AdditionalCostPrices.Where(cp => cp.ProductId == productId).ToList();
                return costs;
            }
        }

        List<AdditionalCostPrice> IAdditionalCostRepository.GetByVendorDetail(string vendorId)
        {
            using (var scope = Context.Read())
            {
                var vendordetail = scope.Context.AdditionalCostPrices.Where(cp => cp.VendorId == vendorId).ToList();

                return vendordetail;
            }
        }

        bool IAdditionalCostRepository.Save(List<AdditionalCostPrice> model, string user)
        {
            
            using (var scope = Context.Write())
            {
                foreach (AdditionalCostPrice cp in model)
                {
                    try
                    {
                        cp.DateAdded = DateTime.UtcNow;
                        cp.AddedBy = user;
                        scope.Context.AdditionalCostPrices.Add(cp);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                try
                {
                    scope.Context.SaveChanges();
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }


    }
}
