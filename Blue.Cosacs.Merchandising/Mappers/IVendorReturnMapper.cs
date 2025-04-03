namespace Blue.Cosacs.Merchandising.Mappers
{
    using System.Collections.Generic;
    using Blue.Cosacs.Merchandising.Models;

    public interface IVendorReturnMapper
    {
        VendorReturnViewModel MapVendorReturnViewModel(VendorReturnViewModel vendorReturn, List<VendorReturnProductView> products);

        VendorReturnDirectViewModel MapVendorReturnViewModel(VendorReturnDirectViewModel vendorReturn, List<VendorReturnDirectProductView> products);

        VendorReturnNewModel MapVendorReturnNewModel(VendorReturnNewModel vendorReturn, List<VendorReturnNewView> products);

        VendorReturnDirectNewModel MapVendorReturnDirectNewModel(VendorReturnDirectNewModel vendorReturn, List<VendorReturnDirectNewView> products);
    }
}