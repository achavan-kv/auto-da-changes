using System;
using STL.Common;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace STL.DAL
{
    public class DOracleIntegration2 : DALObject
    {
        #region Outbound Interface Methods ----------------------------------------------------------
        //-------------------------------------------------------------------------------------------

        public DataSet GetOrderAndDeliveries(SqlConnection conn, int runNo, int newRunNo)
        {
            DataSet ds = new DataSet();

            try
            {
                if (runNo == 0) //Only when first time running
                {
                    //--Writing the new runno to the export table-----------------
                    parmArray = new SqlParameter[1];
                    parmArray[0] = new SqlParameter("@newRunNo", SqlDbType.Int);
                    parmArray[0].Value = newRunNo;

                    using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        RunSP(conn, trans, "DN_OracleInteg_UpdateExportRunNo", parmArray);
                        trans.Commit();
                    }
                    //-------------------------------------------------------------
                }

                //--Reading from LineItem export table-------------------------
                DataSet dsOrder = new DataSet();
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = newRunNo;

                this.RunSP("DN_OracleInteg_OutboundExport", parmArray, dsOrder);
                //-------------------------------------------------------------

                //--Retrieving Customer Detail---------------------------------
                DataSet dsCustomer = new DataSet();
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = newRunNo;

                this.RunSP("DN_OracleInteg_GetCustomerDetail", parmArray, dsCustomer);
                //-------------------------------------------------------------

                //--Retrieving AR Detail---------------------------------------
                DataSet dsAR = new DataSet();
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = newRunNo;
                parmArray[1] = new SqlParameter("@isRerun", SqlDbType.Bit);
                parmArray[1].Value = (runNo > 0); //If RunNo > 0 then it's a re-run

                this.RunSP("DN_OracleInteg_GetARDetail", parmArray, dsAR);
                //-------------------------------------------------------------

                //--Reading (Receipt) from FinTrans export table---------------
                DataSet dsReceipt = new DataSet();
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = newRunNo;

                this.RunSP("DN_OracleInteg_GetReceipt", parmArray, dsReceipt);
                //-------------------------------------------------------------

                //-------------------------------------------------------------
                if (dsCustomer.Tables.Count > 0)
                {
                    DataTable dt = dsCustomer.Tables[0].Copy();
                    dt.TableName = "Customer";
                    ds.Tables.Add(dt);
                }

                if (dsAR.Tables.Count > 1)
                {
                    DataTable dt = dsAR.Tables[0].Copy();
                    dt.TableName = "ARHeader";
                    ds.Tables.Add(dt);

                    dt = dsAR.Tables[1].Copy();
                    dt.TableName = "ARDetail";
                    ds.Tables.Add(dt);
                }

                if (dsReceipt.Tables.Count > 0)
                {
                    DataTable dt = dsReceipt.Tables[0].Copy();
                    dt.TableName = "Receipt";
                    ds.Tables.Add(dt);
                }

                if (dsOrder.Tables.Count > 1)
                {
                    DataTable dt = dsOrder.Tables[0].Copy();
                    dt.TableName = "OrderNo";
                    ds.Tables.Add(dt);

                    dt = dsOrder.Tables[1].Copy();
                    dt.TableName = "OrderDetail";
                    ds.Tables.Add(dt);
                }
                //-------------------------------------------------------------            
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return ds;
        }

        public int GetNextRunNo(string interfaceName)
        {
            // Add a new Interface Control entry and return the new run no
            int nextRunNo = 0;
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@interfaceName", SqlDbType.VarChar, 12);
                parmArray[0].Value = interfaceName;
                parmArray[1] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[1].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_OracleInteg_GetNextRunNo", parmArray);

                if (result == 0)
                {
                    if (parmArray[1].Value != DBNull.Value)
                        nextRunNo = (int)parmArray[1].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return nextRunNo;
        }

        public void DeleteOrUpdateRunNo(string interfaceName, int runNo, char result, bool delete)
        {
            try
            {
                if (delete)
                {
                    //--Delete Interface Control------------------------------------------
                    parmArray = new SqlParameter[2];
                    parmArray[0] = new SqlParameter("@interfaceName", SqlDbType.VarChar, 12);
                    parmArray[0].Value = interfaceName;
                    parmArray[1] = new SqlParameter("@runNo", SqlDbType.Int);
                    parmArray[1].Value = runNo;

                    this.RunSP("DN_OracleInteg_DeleteRunNo", parmArray);
                    //---------------------------------------------------------------------  
                }
                else
                {
                    //--Update Interface Control------------------------------------------
                    parmArray = new SqlParameter[3];
                    parmArray[0] = new SqlParameter("@interfaceName", SqlDbType.VarChar, 12);
                    parmArray[0].Value = interfaceName;
                    parmArray[1] = new SqlParameter("@runNo", SqlDbType.Int);
                    parmArray[1].Value = runNo;
                    parmArray[2] = new SqlParameter("@result", SqlDbType.VarChar, 1);
                    parmArray[2].Value = result.ToString();

                    this.RunSP("DN_OracleInteg_UpdateRunNo", parmArray);
                    //---------------------------------------------------------------------  
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void ResetRunNo(SqlConnection conn, int runNo)
        {
            try
            {
                //--Reset Export Table RunNo------------------------------------------
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@runNo", SqlDbType.Int);
                parmArray[0].Value = runNo;

                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    this.RunSP(conn, trans, "DN_OracleInteg_ResetExportRunNo", parmArray);
                    trans.Commit();
                }
                //---------------------------------------------------------------------  
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        //-------------------------------------------------------------------------------------------
        #endregion ----------------------------------------------------------------------------------


        #region Inbound Interface Methods -----------------------------------------------------------
        //-------------------------------------------------------------------------------------------

        public int UpdateStockInfo(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[14];

                parmArray[0] = new SqlParameter("@itemno", SqlDbType.VarChar, 8);
                parmArray[0].Value = dr["itemno"];
                parmArray[1] = new SqlParameter("@itemdescr1", SqlDbType.VarChar, 25);
                parmArray[1].Value = dr["itemdescr1"];
                parmArray[2] = new SqlParameter("@itemdescr2", SqlDbType.VarChar, 40);
                parmArray[2].Value = dr["itemdescr2"];
                parmArray[3] = new SqlParameter("@category", SqlDbType.SmallInt);
                parmArray[3].Value = dr["category"];
                parmArray[4] = new SqlParameter("@supplier", SqlDbType.VarChar, 40);
                parmArray[4].Value = dr["supplier"];
                parmArray[5] = new SqlParameter("@prodstatus", SqlDbType.VarChar, 1);
                parmArray[5].Value = dr["prodstatus"];
                parmArray[6] = new SqlParameter("@suppliercode", SqlDbType.VarChar, 18);
                parmArray[6].Value = dr["suppliercode"];
                parmArray[7] = new SqlParameter("@warrantable", SqlDbType.SmallInt);
                parmArray[7].Value = dr["warrantable"];
                parmArray[8] = new SqlParameter("@itemtype", SqlDbType.VarChar, 1);
                parmArray[8].Value = dr["itemtype"];
                parmArray[9] = new SqlParameter("@refcode", SqlDbType.VarChar, 3);
                parmArray[9].Value = dr["refcode"];
                parmArray[10] = new SqlParameter("@warrantyrenewalflag", SqlDbType.VarChar, 1);
                parmArray[10].Value = dr["warrantyrenewalflag"];
                parmArray[11] = new SqlParameter("@leadtime", SqlDbType.SmallInt);
                parmArray[11].Value = 0; // leadtime
                parmArray[12] = new SqlParameter("@assemblyrequired", SqlDbType.VarChar, 1);
                parmArray[12].Value = dr["assemblyrequired"];
                parmArray[13] = new SqlParameter("@deleted", SqlDbType.VarChar, 1);
                parmArray[13].Value = dr["deleted"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdateStockInfo", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public int UpdateStockPrice(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[7];

                parmArray[0] = new SqlParameter("@itemno", SqlDbType.VarChar, 8);
                parmArray[0].Value = dr["itemno"];
                parmArray[1] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[1].Value = dr["branchno"];
                parmArray[2] = new SqlParameter("@CreditPrice", SqlDbType.Money);
                parmArray[2].Value = dr["CreditPrice"];
                parmArray[3] = new SqlParameter("@CashPrice", SqlDbType.Money);
                parmArray[3].Value = dr["CashPrice"];
                parmArray[4] = new SqlParameter("@DutyFreePrice", SqlDbType.Money);
                parmArray[4].Value = dr["DutyFreePrice"];
                parmArray[5] = new SqlParameter("@CostPrice", SqlDbType.Money);
                parmArray[5].Value = dr["CostPrice"];
                parmArray[6] = new SqlParameter("@taxrate", SqlDbType.Float);
                parmArray[6].Value = dr["taxrate"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdateStockPrice", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public int UpdateStockQuantity(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@itemno", SqlDbType.VarChar, 8);
                parmArray[0].Value = dr["itemno"];
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = dr["stocklocn"];
                parmArray[2] = new SqlParameter("@qtyAvailable", SqlDbType.Float);
                parmArray[2].Value = dr["qtyAvailable"];
                parmArray[3] = new SqlParameter("@stock", SqlDbType.Float);
                parmArray[3].Value = dr["stock"];
                parmArray[4] = new SqlParameter("@stockonorder", SqlDbType.Float);
                parmArray[4].Value = dr["stockonorder"];
                parmArray[5] = new SqlParameter("@stockdamage", SqlDbType.Float);
                parmArray[5].Value = dr["stockdamage"];
                parmArray[6] = new SqlParameter("@leadtime", SqlDbType.SmallInt);
                parmArray[6].Value = dr["leadtime"];
                parmArray[7] = new SqlParameter("@dateupdated", SqlDbType.DateTime);
                parmArray[7].Value = dr["dateupdated"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdateStockQuantity", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public int UpdatePromoPrice(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@origbr", SqlDbType.SmallInt);
                parmArray[0].Value = dr["origbr"];
                parmArray[1] = new SqlParameter("@itemno", SqlDbType.VarChar, 8);
                parmArray[1].Value = dr["itemno"];
                parmArray[2] = new SqlParameter("@stocklocn", SqlDbType.Int);
                parmArray[2].Value = dr["stocklocn"];
                parmArray[3] = new SqlParameter("@hporcash", SqlDbType.Char, 1);
                parmArray[3].Value = dr["hporcash"];
                parmArray[4] = new SqlParameter("@fromdate", SqlDbType.DateTime);
                parmArray[4].Value = dr["fromdate"];
                parmArray[5] = new SqlParameter("@todate", SqlDbType.DateTime);
                parmArray[5].Value = dr["todate"];
                parmArray[6] = new SqlParameter("@unitprice", SqlDbType.Float);
                parmArray[6].Value = dr["unitprice"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdatePromoPrice", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public int UpdatePurchaseOrder(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[7];
                parmArray[0] = new SqlParameter("@itemno", SqlDbType.VarChar, 10);
                parmArray[0].Value = dr["itemno"];
                parmArray[1] = new SqlParameter("@stocklocn", SqlDbType.SmallInt);
                parmArray[1].Value = dr["stocklocn"];
                parmArray[2] = new SqlParameter("@supplierno", SqlDbType.VarChar, 12);
                parmArray[2].Value = dr["supplierno"];
                parmArray[3] = new SqlParameter("@purchaseordernumber", SqlDbType.VarChar, 12);
                parmArray[3].Value = dr["purchaseordernumber"];
                parmArray[4] = new SqlParameter("@expectedreceiptdate", SqlDbType.DateTime);
                parmArray[4].Value = dr["expectedreceiptdate"];
                parmArray[5] = new SqlParameter("@quantityonorder", SqlDbType.SmallInt);
                parmArray[5].Value = dr["quantityonorder"];
                parmArray[6] = new SqlParameter("@quantityavailable", SqlDbType.SmallInt);
                parmArray[6].Value = dr["quantityavailable"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdatePurchaseOrder", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public int UpdateWarrantyBand(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[6];
                parmArray[0] = new SqlParameter("@waritemno", SqlDbType.VarChar, 8);
                parmArray[0].Value = dr["waritemno"];
                parmArray[1] = new SqlParameter("@refcode", SqlDbType.VarChar, 3);
                parmArray[1].Value = dr["refcode"];
                parmArray[2] = new SqlParameter("@minprice", SqlDbType.Money);
                parmArray[2].Value = dr["minprice"];
                parmArray[3] = new SqlParameter("@maxprice", SqlDbType.Money);
                parmArray[3].Value = dr["maxprice"];
                parmArray[4] = new SqlParameter("@warrantylength", SqlDbType.Float);
                parmArray[4].Value = dr["warrantylength"];
                parmArray[5] = new SqlParameter("@firstYearWarPeriod", SqlDbType.SmallInt);
                parmArray[5].Value = dr["firstYearWarPeriod"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdateWarrantyBand", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public int UpdateFreightCarrier(SqlConnection conn, SqlTransaction trans, DataRow dr)
        {
            int updateCount = 0;

            try
            {
                parmArray = new SqlParameter[8];
                parmArray[0] = new SqlParameter("@truckid", SqlDbType.VarChar, 26);
                parmArray[0].Value = dr["truckid"];
                parmArray[1] = new SqlParameter("@drivername", SqlDbType.VarChar, 50);
                parmArray[1].Value = dr["drivername"];
                parmArray[2] = new SqlParameter("@phoneno", SqlDbType.VarChar, 20);
                parmArray[2].Value = dr["phoneno"];
                parmArray[3] = new SqlParameter("@carrierNumber", SqlDbType.VarChar, 20);
                parmArray[3].Value = dr["carrierNumber"];
                parmArray[4] = new SqlParameter("@status", SqlDbType.VarChar, 12);
                parmArray[4].Value = dr["status"];
                parmArray[5] = new SqlParameter("@createdDate", SqlDbType.DateTime);
                parmArray[5].Value = dr["createdDate"];
                parmArray[6] = new SqlParameter("@activeEndDate", SqlDbType.DateTime);
                parmArray[6].Value = dr["activeEndDate"];
                parmArray[7] = new SqlParameter("@lastUpdatedDate", SqlDbType.DateTime);
                parmArray[7].Value = dr["lastUpdatedDate"];

                int SPReturn = this.RunSP(conn, trans, "DN_OracleInteg_UpdateTransport", parmArray);

                if (SPReturn == 0) updateCount++;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return updateCount;
        }

        public void UpdateNonStockTaxRate(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                RunSPNoReturn(conn, trans, "NonStockUpdateTaxRate");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
        }
        //-------------------------------------------------------------------------------------------
        #endregion ----------------------------------------------------------------------------------

    }


}
