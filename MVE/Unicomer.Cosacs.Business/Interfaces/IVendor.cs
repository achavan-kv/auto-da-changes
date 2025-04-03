using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;
namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface IVendor
    {
        JResponse GetSupplierMaster();
        dynamic UpdateSupplierMaster(UpdateSupplier objJSON);
        dynamic GetSupplierEOD(int spanInMinutes);
        dynamic GetSupplierRTS(string vendorId);
    }
}
