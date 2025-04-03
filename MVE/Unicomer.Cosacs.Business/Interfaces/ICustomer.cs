using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicomer.Cosacs.Model;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface ICustomer
    {
        JResponse CreateCustomer(UserJson objCustomer);
        JResponse GetParentSKUMaster();
        JResponse GetParentSKUMasterEOD();
        JResponse GetSupplierMaster();
        JResponse SearchCustomer(CustomerRequest objCustomerSearch);
        JResponse UpdateCustomer(UserJson objCustomerUpdate);
    }
}
