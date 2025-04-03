
using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warranty;
using Blue.Cosacs.Repositories;
using STL.Common.Services.Model;
using System.Data;
using Blue.Cosacs.Shared;
using STL.Common;

namespace Blue.Cosacs.Services.Warranty
{
	partial class Server 
    {
        public GetWarrantyRenewalsResponse Call(GetWarrantyRenewalsRequest request)
        {

            var table = new DataTable();
            table.TableName = "Renewal";
            table.Columns.Add(new DataColumn { ColumnName = "AcctNo", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "ItemNo", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "ItemId", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "itemiupc", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "StockLocn", DataType = int.MaxValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "description", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "WarrantyId", DataType = int.MaxValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "WarrantyNo", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "warrantylocn", DataType = int.MaxValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "contractno", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "expires", DataType = DateTime.Now.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "newwarrantyid", DataType = int.MaxValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "renewalwarrantyno", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "warrantydesc1", DataType = string.Empty.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "warrantyprice", DataType = decimal.MinValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "warrantycostprice", DataType = decimal.MinValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "taxRate", DataType = decimal.MinValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "length", DataType = int.MinValue.GetType() });
            table.Columns.Add(new DataColumn { ColumnName = "custid", DataType = string.Empty.GetType() });    //#16237
            table.Columns.Add(new DataColumn { ColumnName = "TypeCode", DataType = string.Empty.GetType() });    //#17313

            var type = STL.Common.Services.Services.ServiceTypes.CosacsWeb;
            var service = STL.Common.Services.Services.GetService(type);
            
            var wr = new WarrantyRepository();
            var availableWarranties = wr.GetRenewableWarranties(request.AccountNumber);

            var taxRate = CountryParameterCache.GetCountryParameter<decimal>(CountryParameterNames.TaxRate); //#17219

            var renewables = service.GetRenewals(availableWarranties);

            var ds = new DataSet();

            if (renewables != null)
            { 
                var renewList = new List<WarrantyRenewal>();
                foreach (var renewable in renewables)
                {
                    renewList.AddRange(renewable.ToFlat());
                }
            
                foreach (var aw in availableWarranties)
                {
                    foreach (var renewable in renewList)
                    {
                        if (renewable.WarrantyNumber == aw.Itemno)
                        {
                        
                            var row = table.NewRow();
                            row["AcctNo"] = aw.acctno;
                            row["ItemNo"] = aw.parentItemNumber;
                            row["ItemId"] = aw.parentItemId;
                            row["itemiupc"] = aw.parentItemNumber;
                            row["StockLocn"] = aw.stocklocn;
                            row["description"] = aw.Description;
                            row["Warrantyid"] = aw.ItemId;
                            row["WarrantyNo"] = aw.Itemno;
                            row["warrantylocn"] = aw.stocklocn;
                            row["contractno"] = aw.contractno;
                            row["expires"] = aw.EffectiveDate.Value.AddMonths(aw.warrantyLength ?? 0);         // was .Value
                            row["newwarrantyid"] = renewable.Id;
                            row["renewalwarrantyno"] = renewable.Number;
                            row["warrantydesc1"] = renewable.Description;
                            row["warrantyprice"] = renewable.Price;
                            row["warrantycostprice"] = renewable.CostPrice;
                            row["taxRate"] = renewable.TaxRate.HasValue == true ? renewable.TaxRate.Value : taxRate; //#17219
                            row["length"] = renewable.Length;
                            row["custid"] = aw.custid;              //#16237
                            row["TypeCode"] = renewable.TypeCode;              //#17313
                            table.Rows.Add(row);
                        }
                    }
                }

                ds.Tables.Add(table);

            }

            return new GetWarrantyRenewalsResponse
            {
                WarrantyRenewal = ds,
            };

        }



    }
}
