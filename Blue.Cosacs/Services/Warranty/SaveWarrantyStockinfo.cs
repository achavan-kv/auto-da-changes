
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Warranty
{
	partial class Server 
    {
        public SaveWarrantyStockinfoResponse Call(SaveWarrantyStockinfoRequest request)
        {
            new WarrantyRepository().CreateWarranty(request);
            return null;
        }
    }
}
