
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Repositories;
using System.Data;
using STL.Common.Services.Model;
using STL.Common.Constants.ColumnNames;
using System.Linq;

namespace Blue.Cosacs.Services.Warranty
{
	partial class Server 
    {
        public GetAvailableWarrantiesResponse Call(GetAvailableWarrantiesRequest request)
        {
            var type = STL.Common.Services.Services.ServiceTypes.CosacsWeb;
            var service = STL.Common.Services.Services.GetService(type);

            DataTable dt = new DataTable();
            dt.Columns.Add(CN.AccountNumber);
            dt.Columns.Add("agrmtno");                          //#16992
            dt.Columns.Add(CN.ItemNo);
            dt.Columns.Add("ItemID");                           //#16992
            dt.Columns.Add("stocklocn");                        //#16992
            dt.Columns.Add("NoOfPrompts");

            var itemsWithoutWarranties = new WarrantyRepository().GetItemsWithoutWarranties(request.CustomerId);

            WarrantyResult[] warrantyResult = null;

            if (itemsWithoutWarranties.Count > 0)
            {
                warrantyResult = service.GetWarranties(itemsWithoutWarranties);

                for(var i = 0; i < itemsWithoutWarranties.Count; i++)
                {

                    foreach (var a in warrantyResult)
                    {
                        if (a.ProductSearch.Product == itemsWithoutWarranties[i].itemno
                             && dt.Select("acctno = " + itemsWithoutWarranties[i].acctno +                          //#16992 - only add if row doesn't exist
                             " and agrmtno = " + itemsWithoutWarranties[i].agrmtno +
                             " and itemID = " + itemsWithoutWarranties[i].ItemID +
                             " and stocklocn = " + itemsWithoutWarranties[i].stocklocn).Length == 0)
                        {
                            DataRow dr = dt.NewRow();

                            dr[0] = itemsWithoutWarranties[i].acctno;
                            dr[1] = itemsWithoutWarranties[i].agrmtno;       //#16992
                            dr[2] = itemsWithoutWarranties[i].itemno;
                            dr[3] = itemsWithoutWarranties[i].ItemID;        //#16992
                            dr[4] = itemsWithoutWarranties[i].stocklocn;     //#16992
                            dr[5] = itemsWithoutWarranties[i].NoOfPrompts;

                            dt.Rows.Add(dr);
                        }
                    }
              
                }
            }

            return new GetAvailableWarrantiesResponse
            {
                WarrantyResult = warrantyResult,
                items = dt
            };
        }
    }
}
