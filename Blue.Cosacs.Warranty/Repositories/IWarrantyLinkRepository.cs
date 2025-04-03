using System;
namespace Blue.Cosacs.Warranty.Repositories
{
    public interface IWarrantyLinkRepository
    {
        void Delete(int id);
        Blue.Data.IPagedSearchResults<Blue.Cosacs.Warranty.Model.WarrantyLink> Get(Blue.Cosacs.Warranty.Model.WarrantyLinkSearch search);
        System.Collections.Generic.List<Blue.Cosacs.Warranty.Model.WarrantyRenewal> GetRenewals(Blue.Cosacs.Warranty.Model.WarrantyLocation[] warrantyLocation);
        string GetWarrantyType(int warrantyId);
        int Save(Blue.Cosacs.Warranty.Model.WarrantyLink warrantyLink);
        Blue.Cosacs.Warranty.Model.WarrantySearchResult Search(Blue.Cosacs.Warranty.Model.WarrantySearchByProduct search);
        Blue.Cosacs.Warranty.Model.WarrantySearchByProductResult SearchByProduct(Blue.Cosacs.Warranty.Model.WarrantySearchByProduct search, string typeCode = "");
        string ValidateNewWarrantyLink(int warrantyId, Blue.Cosacs.Warranty.Model.WarrantyLink warrantyLink);
    }
}
