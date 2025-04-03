
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Repositories;
using STL.Common.Services;

namespace Blue.Cosacs.Services.Warranty
{
	partial class Server 
    {
        public GetWarrantyReturnResponse Call(GetWarrantyReturnRequest request)
        {
            var w = new WarrantyRepository().GetWarrantyElapsedMonthsDel(request.warrantyItemID, request.contractNo, request.stocklocn);
            var elapsed = w == null? null : w.Item1; //#15993
            if (elapsed.HasValue)
            {
                return new GetWarrantyReturnResponse
                {
                    warrantyReturn = STL.Common.Services.Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarrantyReturn(request.warrantyNumber, (int)request.stocklocn, elapsed.Value, w.Item4),
                    DateDel = w.Item2
                };
            }
            else
                return null; 

        }
    }
}
