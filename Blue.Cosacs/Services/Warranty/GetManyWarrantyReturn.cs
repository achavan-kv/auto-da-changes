
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared.CosacsWeb.Models;

namespace Blue.Cosacs.Services.Warranty
{
	partial class Server 
    {
        public GetManyWarrantyReturnResponse Call(GetManyWarrantyReturnRequest request)
        {
            List<WarrantyReturnDetails> returnDetails = new List<WarrantyReturnDetails>();

            foreach (var item in request.returnInputs)
            {
                var w = new WarrantyRepository().GetWarrantyElapsedMonthsDel(item.warrantyItemID, item.contractNo, item.stocklocn);
                var elapsed = w == null ? null : w.Item1; //#15993

                if (elapsed.HasValue)
                {
                    returnDetails.Add(new WarrantyReturnDetails { warrantyReturn = STL.Common.Services.Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarrantyReturn(item.warrantyNumber, 
                                    (int)item.stocklocn, elapsed.Value, w.Item4), EffectiveDate = w.Item2, WarrantyDeliveredOn = w.Item3, WarrantyContractNo = w.Item5});       // #17506
                }
        
            }

            if (returnDetails.Count > 0)
            {
                return new GetManyWarrantyReturnResponse
                {
                    returnDetails = returnDetails
                };

            }
            else
            {
                return null;
            }

           
        }
    }
}
