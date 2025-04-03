namespace Blue.Cosacs.Merchandising.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Blue.Cosacs.Merchandising.Models;
    using Blue.Cosacs.Merchandising.Helpers;

    public class VendorReturnMapper : IVendorReturnMapper
    {
        public VendorReturnViewModel MapVendorReturnViewModel(VendorReturnViewModel vendorReturn, List<VendorReturnProductView> products)
        {
            vendorReturn.PurchaseOrders = products
                .Select(vrp =>
                    new VendorReturnPurchaseOrderViewModel()
                    {
                        PurchaseOrderId = vrp.PurchaseOrderId,
                        Vendor = vrp.Vendor,
                        VendorId = vrp.VendorId,
                        Products = Mapper.Map<List<VendorReturnProductViewModel>>(products.Where(p => p.PurchaseOrderId == vrp.PurchaseOrderId))
                    }).DistinctBy(vrp => vrp.PurchaseOrderId)
                    .ToList();

            return vendorReturn;
        }

        public VendorReturnDirectViewModel MapVendorReturnViewModel(VendorReturnDirectViewModel vendorReturn, List<VendorReturnDirectProductView> products)
        {
            vendorReturn.Products = Mapper.Map<List<VendorReturnProductViewModel>>(products);
            return vendorReturn;
        }

        public VendorReturnNewModel MapVendorReturnNewModel(VendorReturnNewModel vendorReturn, List<VendorReturnNewView> products)
        {
            vendorReturn.PurchaseOrders = products
                .Select(vrp => new VendorReturnPurchaseOrderNewModel()
                {
                    PurchaseOrderId = vrp.PurchaseOrderId,
                    VendorId = vrp.VendorId,
                    Vendor = vrp.Vendor,
                    Products = Mapper.Map<List<VendorReturnProductNewModel>>(products.Where(p => p.PurchaseOrderId == vrp.PurchaseOrderId))
                })
                .DistinctBy(vrp => vrp.PurchaseOrderId)
                .ToList();

            return vendorReturn;
        }

        public VendorReturnDirectNewModel MapVendorReturnDirectNewModel(VendorReturnDirectNewModel vendorReturn, List<VendorReturnDirectNewView> products)
        {
            vendorReturn.Products = Mapper.Map<List<VendorReturnProductNewModel>>(products);
            return vendorReturn;
        }
    }
}
