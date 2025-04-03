using System;
namespace Blue.Cosacs.Warranty.Repositories
{
    public interface IWarrantyPriceRepository
    {
        Blue.Cosacs.Warranty.Model.WarrantyLocationPrice Delete(int id);
        bool DeleteBulkEdit(int bulkEditId);
        void DeletePriceCalcViewCache();
        string GetBulkEditInfo(int[] filteredIds, Blue.Cosacs.Warranty.Model.WarrantyEditRequest editRequest);
        System.Collections.Generic.IEnumerable<Blue.Cosacs.Warranty.Model.WarrantyPrice> GetWarrantyPrices(System.Collections.Generic.IEnumerable<Blue.Cosacs.Warranty.Model.WarrantyLocation> warrantyLocation);
        System.Collections.Generic.IEnumerable<Blue.Cosacs.Warranty.Model.WarrantyCalculatedPrice> GetWarrantyPrices(System.Collections.Generic.IEnumerable<int> warrantyIds, short branch, DateTime? date);
        System.Collections.Generic.IEnumerable<Blue.Cosacs.Warranty.Model.WarrantyLocationPrice> GetWarrantyPrices(int warrantyId);
        void InsertBulkEdit(int[] filteredIds, Blue.Cosacs.Warranty.Model.WarrantyEditRequest editRequest);
        Blue.Cosacs.Warranty.Model.WarrantyLocationPrice Save(Blue.Cosacs.Warranty.Model.WarrantyLocationPrice warrantyPrice);
    }
}
