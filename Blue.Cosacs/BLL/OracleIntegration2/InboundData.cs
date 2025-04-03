using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Cosacs.CSVReader;
using System.Data.SqlClient;
using System.IO;
using STL.DAL;

namespace STL.BLL.OracleIntegration2
{
    public class InboundData
    {
        private DataTable dtStockInfo, dtStockPrice, dtStockQuantity, dtPromoPrice,
                            dtPurchaseOrder, dtWarrantyBand, dtFreightCarrier;

        private DataTable dtCreateStockInfo()
        {
            DataTable dt = new DataTable("StockInfo");
            dt.Columns.Add("itemno", typeof(String));
            dt.Columns.Add("itemdescr1", typeof(String));
            dt.Columns.Add("itemdescr2", typeof(String));
            dt.Columns.Add("category", typeof(Int16));
            dt.Columns.Add("supplier", typeof(String));
            dt.Columns.Add("prodstatus", typeof(String));
            dt.Columns.Add("suppliercode", typeof(String));
            dt.Columns.Add("warrantable", typeof(Int16));
            dt.Columns.Add("itemtype", typeof(String));
            dt.Columns.Add("refcode", typeof(String));
            dt.Columns.Add("warrantyrenewalflag", typeof(String));
            dt.Columns.Add("leadtime", typeof(Int16)); //Not available
            dt.Columns.Add("assemblyrequired", typeof(String));
            dt.Columns.Add("deleted", typeof(String));

            return dt;
        }

        private DataTable dtCreateStockPrice()
        {
            DataTable dt = new DataTable("StockPrice");
            dt.Columns.Add("itemno", typeof(String));
            dt.Columns.Add("branchno", typeof(Int16));
            dt.Columns.Add("CreditPrice", typeof(Decimal));
            dt.Columns.Add("CashPrice", typeof(Decimal));
            dt.Columns.Add("DutyFreePrice", typeof(Decimal));
            dt.Columns.Add("CostPrice", typeof(Decimal));
            dt.Columns.Add("taxrate", typeof(Double));

            return dt;
        }

        private DataTable dtCreateStockQuantity()
        {
            DataTable dt = new DataTable("StockQuantity");
            dt.Columns.Add("itemno", typeof(String));
            dt.Columns.Add("stocklocn", typeof(Int16));
            dt.Columns.Add("qtyAvailable", typeof(Double));
            dt.Columns.Add("stock", typeof(Double)); //Not Available
            dt.Columns.Add("stockonorder", typeof(Double));
            dt.Columns.Add("stockdamage", typeof(Double));
            dt.Columns.Add("leadtime", typeof(Int16));
            dt.Columns.Add("dateupdated", typeof(DateTime));

            return dt;
        }

        private DataTable dtCreatePromoPrice()
        {
            DataTable dt = new DataTable("PromoPrice");
            dt.Columns.Add("origbr", typeof(Int16));
            dt.Columns.Add("itemno", typeof(String));
            dt.Columns.Add("stocklocn", typeof(Int16));
            dt.Columns.Add("hporcash", typeof(String));
            dt.Columns.Add("fromdate", typeof(DateTime));
            dt.Columns.Add("todate", typeof(DateTime));
            dt.Columns.Add("unitprice", typeof(Double));

            return dt;
        }

        private DataTable dtCreatePurchaseOrder()
        {
            DataTable dt = new DataTable("PurchaseOrderOutstanding");
            dt.Columns.Add("warehousenumber", typeof(String));
            dt.Columns.Add("itemno", typeof(String));
            dt.Columns.Add("stocklocn", typeof(Int16)); //Not Available, but a PK
            dt.Columns.Add("supplierno", typeof(String));
            dt.Columns.Add("purchaseordernumber", typeof(String));
            dt.Columns.Add("expectedreceiptdate", typeof(DateTime));
            dt.Columns.Add("quantityonorder", typeof(Int16)); //DB Type not suitable, Not Avaialbe
            dt.Columns.Add("quantityavailable", typeof(Int16)); //DB Type not suitable, Not Avaialbe

            return dt;
        }

        private DataTable dtCreateWarrantyBand()
        {
            DataTable dt = new DataTable("WarrantyBand");
            dt.Columns.Add("maxprice", typeof(Decimal));
            dt.Columns.Add("minprice", typeof(Decimal));
            dt.Columns.Add("refcode", typeof(String));
            dt.Columns.Add("waritemno", typeof(String));
            dt.Columns.Add("warrantylength", typeof(Double));
            dt.Columns.Add("firstYearWarPeriod", typeof(Int16));

            return dt;
        }

        private DataTable dtCreateFreightCarrier()
        {
            DataTable dt = new DataTable("Transport");
            dt.Columns.Add("truckid", typeof(String));
            dt.Columns.Add("drivername", typeof(String));
            dt.Columns.Add("phoneno", typeof(String));
            dt.Columns.Add("carrierNumber", typeof(String));
            dt.Columns.Add("status", typeof(String));
            dt.Columns.Add("createdDate", typeof(DateTime));
            dt.Columns.Add("activeEndDate", typeof(DateTime));
            dt.Columns.Add("lastUpdatedDate", typeof(DateTime));

            return dt;
        }

        public InboundData()
        {
            dtStockInfo = dtCreateStockInfo();
            dtStockPrice = dtCreateStockPrice();
            dtStockQuantity = dtCreateStockQuantity();
            dtPromoPrice = dtCreatePromoPrice();
            dtPurchaseOrder = dtCreatePurchaseOrder();
            dtWarrantyBand = dtCreateWarrantyBand();
            dtFreightCarrier = dtCreateFreightCarrier();
        }


        public int UpdateStockInfo(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // ITEMNO	            SUPPLIERCODE	    SUPPLIERNAME	    ITEMDESC1	 ITEMDESC2	        
            // CATEGORY	            PRODSTATUS	        WARRANTABLE	        ITEMTYPE	 REFCODE
            // WARRANTYRENEWFLAG	ASSEMBLYREQUIRED	DELETEDINDICATOR
            //-----------------------------------------------------------------------------------------

            int updateCount = 0;
            DataRow drNew = dtStockInfo.NewRow();

            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();

            using (CosacsCSVReader csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["itemno"] = strArray[csv.GetFieldIndex("ITEMNO")];
                    drNew["itemdescr1"] = strArray[csv.GetFieldIndex("ITEMDESC1")];
                    drNew["itemdescr2"] = strArray[csv.GetFieldIndex("ITEMDESC2")];
                    if (strArray[csv.GetFieldIndex("CATEGORY")].Trim() == "")
                        drNew["category"] = DBNull.Value;
                    else
                        drNew["category"] = Convert.ToInt16(strArray[csv.GetFieldIndex("CATEGORY")]);
                    drNew["supplier"] = strArray[csv.GetFieldIndex("SUPPLIERNAME")];
                    drNew["prodstatus"] = strArray[csv.GetFieldIndex("PRODSTATUS")];
                    drNew["suppliercode"] = strArray[csv.GetFieldIndex("SUPPLIERCODE")];
                    if (strArray[csv.GetFieldIndex("WARRANTABLE")] == "Y")
                        drNew["warrantable"] = 1;
                    else
                        drNew["warrantable"] = 0;
                    drNew["itemtype"] = strArray[csv.GetFieldIndex("ITEMTYPE")];
                    drNew["refcode"] = strArray[csv.GetFieldIndex("REFCODE")];
                    drNew["warrantyrenewalflag"] = strArray[csv.GetFieldIndex("ITEMNO")];
                    drNew["assemblyrequired"] = strArray[csv.GetFieldIndex("ASSEMBLYREQUIRED")];
                    drNew["deleted"] = strArray[csv.GetFieldIndex("DELETEDINDICATOR")];

                    updateCount += objDOracleInteg.UpdateStockInfo(conn, trans, drNew);
                }
            }

            return updateCount;
        }

        public int UpdateStockPrice(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // ITEMNO	            STOCKLOCN	    UNITPRICEHP	    UNITPRICECASH		
            // UNITPRICEDUTYFREE    COSTPRICE	    TAXRATE
            //-----------------------------------------------------------------------------------------

            int updateCount = 0;
            DataRow drNew = dtStockPrice.NewRow();

            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();

            using (CosacsCSVReader csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["itemno"] = strArray[csv.GetFieldIndex("ITEMNO")];
                    drNew["branchno"] = Convert.ToInt16(strArray[csv.GetFieldIndex("STOCKLOCN")]);
                    drNew["CreditPrice"] = Convert.ToDecimal(strArray[csv.GetFieldIndex("UNITPRICEHP")]);
                    drNew["CashPrice"] = Convert.ToDecimal(strArray[csv.GetFieldIndex("UNITPRICECASH")]);
                    if (strArray[csv.GetFieldIndex("UNITPRICEDUTYFREE")].Trim() == "")
                        drNew["DutyFreePrice"] = DBNull.Value;
                    else
                        drNew["DutyFreePrice"] = Convert.ToDecimal(strArray[csv.GetFieldIndex("UNITPRICEDUTYFREE")]);
                    drNew["CostPrice"] = Convert.ToDecimal(strArray[csv.GetFieldIndex("COSTPRICE")]);
                    drNew["taxrate"] = Convert.ToDouble(strArray[csv.GetFieldIndex("TAXRATE")]);

                    updateCount += objDOracleInteg.UpdateStockPrice(conn, trans, drNew);
                }
            }

            return updateCount;
        }

        public int UpdateStockQuantity(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // ITEM_NO	        STOCKLOCN	    STOCK	        STOCKONORDER	    
            // STOCKDAMAGED	    QTYAVAILABLE	LEAD_TIME	    DATEUPDATED
            //-----------------------------------------------------------------------------------------

            int updateCount = 0;
            DataRow drNew = dtStockQuantity.NewRow();

            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();

            using (CosacsCSVReader csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["itemno"] = strArray[csv.GetFieldIndex("ITEM_NO")];
                    drNew["stocklocn"] = Convert.ToInt16(strArray[csv.GetFieldIndex("STOCKLOCN")]);
                    drNew["qtyAvailable"] = Convert.ToDouble(strArray[csv.GetFieldIndex("QTYAVAILABLE")]);
                    drNew["stock"] = Convert.ToDouble(strArray[csv.GetFieldIndex("STOCK")]);
                    drNew["stockonorder"] = Convert.ToDouble(strArray[csv.GetFieldIndex("STOCKONORDER")]);
                    drNew["stockdamage"] = Convert.ToDouble(strArray[csv.GetFieldIndex("STOCKDAMAGED")]);
                    if (strArray[csv.GetFieldIndex("LEAD_TIME")].Trim() == "")
                        drNew["leadtime"] = 0;
                    else
                        drNew["leadtime"] = Convert.ToInt16(strArray[csv.GetFieldIndex("LEAD_TIME")]);
                    if (strArray[csv.GetFieldIndex("DATEUPDATED")].Trim() == "")
                        drNew["dateupdated"] = DBNull.Value;
                    else
                        drNew["dateupdated"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("DATEUPDATED")]);

                    updateCount += objDOracleInteg.UpdateStockQuantity(conn, trans, drNew);
                }
            }

            return updateCount;
        }

        public int UpdatePromoPrice(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // ITEMNO	STOCKLOCN	HPORCASH	FROMDATE	TODATE	UNITPRICE
            //-----------------------------------------------------------------------------------------

            int updateCount = 0;
            DataRow drNew = dtPromoPrice.NewRow();

            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();

            using (CosacsCSVReader csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["origbr"] = 0;
                    drNew["itemno"] = strArray[csv.GetFieldIndex("ITEMNO")];
                    drNew["stocklocn"] = Convert.ToInt16(strArray[csv.GetFieldIndex("STOCKLOCN")]);
                    drNew["hporcash"] = strArray[csv.GetFieldIndex("HPORCASH")];
                    drNew["fromdate"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("FROMDATE")]);
                    drNew["todate"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("TODATE")]);
                    drNew["unitprice"] = Convert.ToDouble(strArray[csv.GetFieldIndex("UNITPRICE")]);

                    updateCount += objDOracleInteg.UpdatePromoPrice(conn, trans, drNew);
                }
            }

            return updateCount;
        }

        public int UpdatePurchaseOrder(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // PO_NO	        itemNo	                stockLocn	
            // supplierCode     expectedReceiptDate	    qtyOnOrder
            //-----------------------------------------------------------------------------------------

            var updateCount = 0;
            var drNew = dtPurchaseOrder.NewRow();

            var objDOracleInteg = new DOracleIntegration2();

            using (var csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["itemno"] = strArray[csv.GetFieldIndex("itemNo")];
                    drNew["stocklocn"] = Convert.ToInt16(strArray[csv.GetFieldIndex("stockLocn")]);
                    drNew["supplierno"] = strArray[csv.GetFieldIndex("supplierCode")];
                    drNew["purchaseordernumber"] = strArray[csv.GetFieldIndex("PO_NO")];
                    drNew["expectedreceiptdate"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("expectedReceiptDate")]);
                    drNew["quantityonorder"] = Convert.ToInt16(strArray[csv.GetFieldIndex("qtyOnOrder")]);
                    drNew["quantityavailable"] = Convert.ToInt16(strArray[csv.GetFieldIndex("qtyOnOrder")]);

                    updateCount += objDOracleInteg.UpdatePurchaseOrder(conn, trans, drNew);
                }
            }
            objDOracleInteg.UpdateNonStockTaxRate(conn, trans);
            return updateCount;
        }

        public int UpdateWarrantyBand(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // itemNo	refCode	    minPrice	maxPrice	warrantyLength	firstYearWarPeriod
            //-----------------------------------------------------------------------------------------

            int updateCount = 0;
            DataRow drNew = dtWarrantyBand.NewRow();

            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();

            using (CosacsCSVReader csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["waritemno"] = strArray[csv.GetFieldIndex("itemNo")];
                    drNew["maxprice"] = Convert.ToDecimal(strArray[csv.GetFieldIndex("maxPrice")]);
                    drNew["minprice"] = Convert.ToDecimal(strArray[csv.GetFieldIndex("minPrice")]);
                    drNew["refcode"] = strArray[csv.GetFieldIndex("refCode")];
                    drNew["warrantylength"] = Convert.ToDouble(strArray[csv.GetFieldIndex("warrantyLength")]);
                    if (strArray[csv.GetFieldIndex("firstYearWarPeriod")].Trim() == "")
                        drNew["firstYearWarPeriod"] = DBNull.Value;
                    else
                        drNew["firstYearWarPeriod"] = Convert.ToInt16(strArray[csv.GetFieldIndex("firstYearWarPeriod")]);

                    updateCount += objDOracleInteg.UpdateWarrantyBand(conn, trans, drNew);
                }
            }

            return updateCount;
        }

        public int UpdateFreightCarrier(SqlConnection conn, SqlTransaction trans, string strCSV)
        {
            //-----------------------------------------------------------------------------------------
            // CSV File Headers
            // TRUCKID	    CARRIERNUMBER	DRIVERNAME	    STATUS	
            // DATECREATED	DATEUPDATED	    DATEENDACTIVE
            //-----------------------------------------------------------------------------------------

            int updateCount = 0;
            DataRow drNew = dtFreightCarrier.NewRow();

            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();

            using (CosacsCSVReader csv = new CosacsCSVReader(new StringReader(strCSV), true))
            {
                foreach (string[] strArray in csv)
                {
                    drNew["truckid"] = strArray[csv.GetFieldIndex("TRUCKID")];
                    drNew["drivername"] = strArray[csv.GetFieldIndex("DRIVERNAME")];
                    drNew["phoneno"] = "";
                    drNew["carrierNumber"] = strArray[csv.GetFieldIndex("CARRIERNUMBER")];
                    drNew["status"] = strArray[csv.GetFieldIndex("STATUS")];
                    drNew["createdDate"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("DATECREATED")]);
                    if (strArray[csv.GetFieldIndex("DATEENDACTIVE")].Trim() == "")
                        drNew["activeEndDate"] = DBNull.Value;
                    else
                        drNew["activeEndDate"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("DATEENDACTIVE")]);
                    if (strArray[csv.GetFieldIndex("DATEUPDATED")].Trim() == "")
                        drNew["lastUpdatedDate"] = DBNull.Value;
                    else
                        drNew["lastUpdatedDate"] = Convert.ToDateTime(strArray[csv.GetFieldIndex("DATEUPDATED")]);

                    updateCount += objDOracleInteg.UpdateFreightCarrier(conn, trans, drNew);
                }
            }

            return updateCount;
        }

        public int GetNextRunNo(string interfaceName)
        {
            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();
            return objDOracleInteg.GetNextRunNo(interfaceName);
        }

        public void DeleteOrUpdateRunNo(string interfaceName, int runNo, char result, bool delete)
        {
            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();
            objDOracleInteg.DeleteOrUpdateRunNo(interfaceName, runNo, result, delete);
        }
    }

}
