using System;
using System.Collections.Generic;
using System.Text;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.BLL
{
    public class BSalesCommission : CommonObject
    {
        public BSalesCommission()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    

    // Get Sales Commission Rates - CR36
        public DataSet GetSalesCommissionRates(string commItemStr, DateTime selectDate)
        {
            DataSet ds = new DataSet();
            DSalesCommission bc = new DSalesCommission();
            ds.Tables.Add(bc.GetSalesCommissionRates(commItemStr, selectDate));
            return ds;
        }

        /// <summary>
        /// SaveCommissionRates
        /// </summary>
        /// <param name="exchangeRateSet">DataSet</param>
        /// <returns>void</returns>
        /// 
        public void SaveCommissionRates(SqlConnection conn, SqlTransaction trans, string commItemStr, DataSet commissionRateSet)
        {
            DSalesCommission commissionRates = new DSalesCommission();
            commissionRates.User = this.User;

            var commItem = commItemStr;     // #9785

            if (commItemStr != "Linked Spiffs")
            {

                foreach (DataTable table in commissionRateSet.Tables)
                    if (table.TableName == TN.SalesCommissionRates)
                        foreach (DataRow row in table.Rows)
                        //if ((string)row[CN.Status] == RateStatus.Edit)
                        {
                            commissionRates.itemText = (string)row[CN.ItemText];
                            //    commissionRates.commissionType = (string)row[CN.CommissionType];
                            commissionRates.percentage = Convert.ToDouble(row[CN.Percentage]);
                            commissionRates.percentageCash = Convert.ToDouble(row[CN.PercentageCash]);      // RI
                            commissionRates.repoPercentage = Convert.ToDouble(row[CN.RepoPercentage]);
                            commissionRates.repoPercentageCash = Convert.ToDouble(row[CN.RepoPercentageCash]);
                            if (commItemStr == "Terms Type" || commItemStr == "Single Spiffs")
                            {
                                commissionRates.value = Convert.ToDecimal(row[CN.Value]);  // was decimal
                                commissionRates.repoValue = Convert.ToDecimal(row["RepoValue"]);
                            }
                            else
                            {
                                commissionRates.value = 0;
                                commissionRates.repoValue = 0;
                            }
                            commissionRates.dateFrom = (DateTime)row[CN.DateFrom];
                            commissionRates.dateTo = (DateTime)row[CN.DateTo];
                            commissionRates.branch = (string)row[CN.Branch];            //CR1035

                            if (commItemStr == "All Products")
                            {
                                commItem = (string)row["CommissionType"];           // #9785
                            }
                            commissionRates.SaveCommissionRates(conn, trans, commItem);             // #9785
                        }
            }
            else
            {
                foreach (DataTable table in commissionRateSet.Tables)
                    if (table.TableName == TN.SalesCommissionRates)
                        foreach (DataRow row in table.Rows)
                        //if ((string)row[CN.Status] == RateStatus.Edit)
                        {
                            commissionRates.description = (string)row[CN.Description];
                            commissionRates.item1 = (string)row[CN.Item1];
                            commissionRates.item2 = (string)row[CN.Item2];
                            commissionRates.item3 = (string)row[CN.Item3];
                            commissionRates.item4 = (string)row[CN.Item4];
                            commissionRates.item5 = (string)row[CN.Item5];
                            commissionRates.percentage = Convert.ToDouble(row[CN.Percentage]);
                            commissionRates.value = Convert.ToDecimal(row[CN.Value]);
                            commissionRates.dateFrom = (DateTime)row[CN.DateFrom];
                            commissionRates.dateTo = (DateTime)row[CN.DateTo];
                            commissionRates.branch = (string)row[CN.Branch];            //CR1035
                            commissionRates.SaveLinkedSpiffRates(conn, trans, commItemStr);
                        }
            }

        }

        // Validate Commission Item 
        public string ValidateCommItem(SqlConnection conn, SqlTransaction trans, string commItemStr, string commItemText, DateTime CommDateFrom, DateTime CommDateTo, string CommSpiffBranch)  //CR1035
         {
             DSalesCommission val = new DSalesCommission();

             string exists = val.ValidateCommItem(conn, trans, commItemStr, commItemText, CommDateFrom, CommDateTo, CommSpiffBranch);   //CR1035
             
             return exists;
         }

         // Validate Product Categories 
         public string ValidateCategory(SqlConnection conn, SqlTransaction trans)
         {
             DSalesCommission valcat = new DSalesCommission();

             string category = valcat.ValidateCategory(conn, trans);

             return category;
         }

         // EOD Commissions Calculation 
         public void EODCommCalc(SqlConnection conn, SqlTransaction trans, int runNo)
         {
             DSalesCommission comm = new DSalesCommission();

             comm.EODCommCalc(conn, trans, runNo);
             
         }
        // Get Basic Commission Details
        public DataSet GetBasicSalesCommission(string branchNo, int employee, DateTime fromDate, DateTime toDate, string accountNo, int agreementNo, string sumDet,string category)
        {
            DataSet ds = new DataSet();
            DSalesCommission basicDetails = new DSalesCommission();
            ds.Tables.Add(basicDetails.GetBasicSalesCommission(branchNo, employee, fromDate, toDate, accountNo, agreementNo, sumDet, category));
            return ds;
        }

        // Get eod csv information
        public DataSet EODCommissionExtract(SqlConnection conn, SqlTransaction trans, int runNo)
        {
            DataSet ds = new DataSet();
            DSalesCommission comm = new DSalesCommission();
            ds.Tables.Add(comm.EODGetCommissionsExtract(conn, trans, runNo));
            return ds;
        }

        public void AddAdditionalSpiff(SqlConnection conn, SqlTransaction trans, string acctNo, int authorisedBy,
                                       string itemNo, short stockLocn, decimal amount, int agrmtNo, int salesPerson)
        {
            DSalesCommission sc = new DSalesCommission();
            sc.AddAdditionalSpiff(conn, trans, acctNo, authorisedBy, itemNo, stockLocn, amount, agrmtNo, salesPerson);
        }

        public void DeleteSpiff(SqlConnection conn, SqlTransaction trans, string acctNo, string itemNo, short stockLocn)
        {
            DSalesCommission sc = new DSalesCommission();
            sc.DeleteSpiff(conn, trans, acctNo, itemNo, stockLocn);
        }

        public DataSet GetSpiffs(string itemNo, short stocklocn, int itemId)
        {
            DataSet ds = new DataSet();
            DSalesCommission comm = new DSalesCommission();
            ds.Tables.Add(comm.GetSpiffs(itemNo, stocklocn, (string)Country[CountryParameterNames.SpiffReselection], itemId));
            ds.Tables.Add(comm.GetLinkedSpiffItems(itemNo, stocklocn));
            return ds;
        }

        public DataSet GetSalesCommissionReportHeader(int branchNo, int employee, DateTime fromDate, DateTime toDate, bool showStandardCommission, bool showSPIFFCommission)
        {
            DataSet ds = new DataSet();
            DSalesCommission comm = new DSalesCommission();
            DataTable dt = comm.GetSalesCommissionReportHeader(branchNo, employee, fromDate, toDate, showStandardCommission, showSPIFFCommission);
            
            dt.TableName = TN.SalesCommission;
            ds.Tables.Add(dt);
            return ds;

        }

        public DataSet GetSalesCommissionReportDetail(int employee, DateTime fromDate, DateTime toDate, bool showStandardCommission, bool showSPIFFCommission)
        {
            DataSet ds = new DataSet();
            DSalesCommission comm = new DSalesCommission();
            DataTable dt =  comm.GetSalesCommissionReportDetail(employee, fromDate, toDate, showStandardCommission, showSPIFFCommission);
            
            dt.TableName = TN.SalesCommission;
            ds.Tables.Add(dt);
            return ds;
        }
    }
}
