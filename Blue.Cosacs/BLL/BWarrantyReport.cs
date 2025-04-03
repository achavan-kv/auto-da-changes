using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.DAL;

namespace STL.BLL
{
    public class BWarrantyReport : CommonObject
    {
        public BWarrantyReport()
		{

		}

        public DataSet WarrantySalesReport(string warrantyType, string branch, string salesPerson, string categorySet, 
                                           short includeCash, short includeCredit, short includeSpecial, DateTime dateFrom, 
                                           DateTime dateTo, short branchTotal,short categoryTotal, short salesPersonTotal,
                                           short acctTypeTotal, string datesAre, short includeCanc, short includeRep)
        {
            DataSet ds = new DataSet();

            DWarrantyReport wr = new DWarrantyReport();
            wr.WarrantyType = warrantyType;
            wr.Branch = branch;
            wr.SalesPerson = salesPerson;
            wr.CategorySet = categorySet;
            wr.IncludeCash = includeCash;
            wr.IncludeCredit = includeCredit;
            wr.IncludeSpecial = includeSpecial;
            wr.DateFrom = dateFrom;
            wr.DateTo = dateTo;
            wr.BranchTotal = branchTotal;
            wr.CategoryTotal = categoryTotal;
            wr.SalesPersonTotal = salesPersonTotal;
            wr.AcctTypeTotal = acctTypeTotal;
            wr.DatesAre = datesAre;
            wr.IncludeCanc = includeCanc;
            wr.IncludeRep = includeRep;

            switch (warrantyType)
            {
                case "HR":
                    wr.WarrantyHitRateReport();
                    break;
                case "LC":
                    wr.WarrantyLostSalesReport();
                    break;
                default:
                    wr.WarrantySalesReport();
                    break;
            }
            
            ds.Tables.Add(wr.Warranties);

            return ds;
        }
    }
}
