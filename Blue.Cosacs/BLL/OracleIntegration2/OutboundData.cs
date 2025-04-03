using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using STL.BLL.OracleIntegration;
using STL.DAL;

namespace STL.BLL.OracleIntegration2
{
    public class OutboundData
    {
        public OutboundDataContainer GetOrderAndDeliveries(SqlConnection conn, int runNo, int newRunNo)
        {
            // This variable will hold the detail of possible excpetion taking place area in the coding
            string exceptionMessage = "";
            OutboundDataContainer objDataContainer = new OutboundDataContainer();

            try
            {
                DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();
                DataSet ds = objDOracleInteg.GetOrderAndDeliveries(conn, runNo, newRunNo);

                #region -- Populate Customer ----------------------------------------------------
                //-------------------------------------------------------------------------------
                exceptionMessage = "At Populate Customer Region";
                if (ds.Tables.Contains("Customer"))
                {
                    foreach (DataRow drCustomer in ds.Tables["Customer"].Rows)
                    {
                        Customer objCustomer = new Customer();
                        objCustomer.CustomerName = DBValueToString(drCustomer["CustomerName"], true);
                        objCustomer.CustType1 = DBValueToString(drCustomer["CustType1"], true);
                        objCustomer.Title = DBValueToString(drCustomer["Title"], true);
                        objCustomer.Custid = DBValueToString(drCustomer["CustId"], true);
                        objCustomer.Acctno = DBValueToString(drCustomer["AcctNo"], true);
                        objCustomer.FirstName = DBValueToString(drCustomer["FirstName"], true);
                        objCustomer.Name = DBValueToString(drCustomer["Name"], true);
                        objCustomer.CustType2 = DBValueToString(drCustomer["CustType2"], true);
                        objCustomer.CustClass = DBValueToString(drCustomer["CustClass"], true);
                        objCustomer.CustCat = DBValueToString(drCustomer["CustCat"], true);
                        objCustomer.HomeTelno = DBValueToString(drCustomer["HomeTelNo"], true);
                        objCustomer.Email = DBValueToString(drCustomer["Email"], true);
                        objCustomer.Passport = DBValueToString(drCustomer["Passport"], true);
                        objCustomer.empeeno = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drCustomer["EmpeeNo"]);
                        objCustomer.BillAddr1 = DBValueToString(drCustomer["BillAddr1"], true);
                        objCustomer.BillAddr2 = DBValueToString(drCustomer["BillAddr2"], true);
                        objCustomer.BillAddr3 = DBValueToString(drCustomer["BillAddr3"], true);
                        objCustomer.BillCity = DBValueToString(drCustomer["BillCity"], true);
                        objCustomer.BillPostCode = DBValueToString(drCustomer["BillPostCode"], true);
                        objCustomer.BillCountry = DBValueToString(drCustomer["BillCountry"], true);
                        objCustomer.BillAdrRef = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drCustomer["BillAdrRef"]);
                        objCustomer.ShipAddr1 = DBValueToString(drCustomer["ShipAddr1"], true);
                        objCustomer.ShipAddr2 = DBValueToString(drCustomer["ShipAddr2"], true);
                        objCustomer.ShipAddr3 = DBValueToString(drCustomer["ShipAddr3"], true);
                        objCustomer.ShipCity = DBValueToString(drCustomer["ShipCity"], true);
                        objCustomer.ShipPostCode = DBValueToString(drCustomer["ShipPostCode"], true);
                        objCustomer.ShipCountry = DBValueToString(drCustomer["ShipCountry"], true);
                        objCustomer.ShipAdrRef = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drCustomer["ShipAdrRef"]);
                        objCustomer.MobileTelno = DBValueToString(drCustomer["MobileTelNo"], true);
                        objCustomer.WorkTelNo = DBValueToString(drCustomer["WorkTelNo"], true);
                        objCustomer.RunNo = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drCustomer["RunNo"]);

                        objDataContainer.CustomerList.Add(objCustomer);
                    }
                }
                exceptionMessage = "";
                //-------------------------------------------------------------------------------
                #endregion ----------------------------------------------------------------------


                #region -- Populate Receipt -----------------------------------------------------
                //-------------------------------------------------------------------------------
                exceptionMessage = "At Populate Receipt Region";
                if (ds.Tables.Contains("Receipt"))
                {
                    foreach (DataRow drReceipt in ds.Tables["Receipt"].Rows)
                    {
                        Receipt objReceipt = new Receipt();
                        objReceipt.ReceiptNo = DBValueToString(drReceipt["ReceiptNo"], true);
                        objReceipt.ReceiptDate = DBValueToDateTime(drReceipt["ReceiptDate"]);
                        objReceipt.CurrencyCode = DBValueToString(drReceipt["CurrencyCode"], true);
                        objReceipt.Custid = DBValueToString(drReceipt["CustId"], true);
                        objReceipt.Acctno = DBValueToString(drReceipt["AcctNo"], true);
                        objReceipt.ReceiptAmount = (Decimal)DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drReceipt["ReceiptAmount"]);
                        objReceipt.InvoiceReference = DBValueToString(drReceipt["InvoiceReference"], true);
                        objReceipt.AppliedAmount = (Decimal)DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drReceipt["AppliedAmount"]);
                        objReceipt.CosacsUser = DBValueToString(drReceipt["CosacsUser"], true);
                        objReceipt.PayMethod = DBValueToString(drReceipt["PayMethod"], true);
                        objReceipt.TranType = DBValueToString(drReceipt["TranType"], true);
                        objReceipt.Chq_CredCard = DBValueToString(drReceipt["Chq_CredCard"], true);
                        objReceipt.Bankname = DBValueToString(drReceipt["BankName"], true);
                        objReceipt.RunNo = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drReceipt["RunNo"]);
                        objReceipt.OracleReceiptNo = DBValueToString(drReceipt["OracleReceiptNo"], true);

                        objDataContainer.ReceiptList.Add(objReceipt);
                    }
                }
                exceptionMessage = "";
                //-------------------------------------------------------------------------------
                #endregion ----------------------------------------------------------------------


                #region -- Populate ARInvoice ---------------------------------------------------
                //-------------------------------------------------------------------------------
                exceptionMessage = "At Populate ARInvoice Region";
                if (ds.Tables.Contains("ARHeader") && ds.Tables.Contains("ARDetail"))
                {
                    string acctNo = "", invReference = "";
                    foreach (DataRow drARHeader in ds.Tables["ARHeader"].Rows)
                    {
                        acctNo = DBValueToString(drARHeader["AcctNo"], false);
                        invReference = DBValueToString(drARHeader["InvoiceReference"], false);

                        if (acctNo.Trim() == "" && invReference.Trim() == "")
                        {
                            continue;
                        }

                        ARInvoice objARinvoice = new ARInvoice();

                        DataRow[] drArrayQueried = ds.Tables["ARDetail"].Select("AcctNo = '" + acctNo + "' and InvoiceReference = '" + invReference + "'");

                        objARinvoice.InvoiceHeader = new ARInvoiceHeader();
                        objARinvoice.InvoiceHeader.TranType = DBValueToString(drArrayQueried[0]["TranType"], true);
                        objARinvoice.InvoiceHeader.TranClass = DBValueToString(drArrayQueried[0]["TranClass"], true);
                        objARinvoice.InvoiceHeader.TranDate = DBValueToDateTime(drArrayQueried[0]["TranDate"]);
                        objARinvoice.InvoiceHeader.GLDate = DBValueToDateTime(drArrayQueried[0]["GLDate"]);
                        objARinvoice.InvoiceHeader.DelDate = DBValueToDateTime(drArrayQueried[0]["DelDate"]);
                        objARinvoice.InvoiceHeader.empeenosale = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["EmpeeNoSale"]);
                        objARinvoice.InvoiceHeader.invoicereference = DBValueToString(drArrayQueried[0]["InvoiceReference"], true);
                        objARinvoice.InvoiceHeader.CredInvRef = DBValueToString(drArrayQueried[0]["CredInvRef"], true);
                        objARinvoice.InvoiceHeader.PayTerm = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["PayTerm"]);
                        objARinvoice.InvoiceHeader.SalesPerson = DBValueToString(drArrayQueried[0]["SalesPerson"], true);
                        objARinvoice.InvoiceHeader.CustomerId = DBValueToString(drArrayQueried[0]["CustomerID"], true);
                        objARinvoice.InvoiceHeader.AcctNo = DBValueToString(drArrayQueried[0]["AcctNo"], true);
                        objARinvoice.InvoiceHeader.BillAdrRef = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["BillAdrRef"]);
                        objARinvoice.InvoiceHeader.ShipAdrRef = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["ShipAdrRef"]);
                        objARinvoice.InvoiceHeader.BranchNo = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["BranchNo"]);
                        objARinvoice.InvoiceHeader.RunNo = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["RunNo"]);

                        int i = 0;
                        objARinvoice.InvoiceLine = new ARInvoiceLine[drArrayQueried.Length];

                        foreach (DataRow drQueried in drArrayQueried)
                        {
                            objARinvoice.InvoiceLine[i] = new ARInvoiceLine();
                            objARinvoice.InvoiceLine[i].itemno = DBValueToString(drQueried["ItemNo"], true);
                            objARinvoice.InvoiceLine[i].lineDescription = DBValueToString(drQueried["LineDescription"], true);
                            objARinvoice.InvoiceLine[i].UOM = DBValueToString(drQueried["UOM"], true);
                            objARinvoice.InvoiceLine[i].Quantity = (Double)DBValueToNullableValue<Object, Double>(Convert.ToDouble, drQueried["Quantity"]);
                            objARinvoice.InvoiceLine[i].UnitPrice = (Decimal)DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["UnitPrice"]);
                            objARinvoice.InvoiceLine[i].LineValue = (Decimal)DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["LineValue"]);
                            objARinvoice.InvoiceLine[i].TaxFlag = DBValueToString(drQueried["TaxFlag"], true);
                            objARinvoice.InvoiceLine[i].TaxCode = DBValueToString(drQueried["TaxCode"], true);
                            objARinvoice.InvoiceLine[i].TaxRate = (Decimal)DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["TaxRate"]);
                            objARinvoice.InvoiceLine[i].RetItemNo = DBValueToString(drQueried["RetItemNo"], true);
                            objARinvoice.InvoiceLine[i].LineRef = (Int32)DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drQueried["LineRef"]);
                            objARinvoice.InvoiceLine[i].AccountCode = DBValueToString(drQueried["AccountCode"], true);

                            i++;
                        }

                        objDataContainer.ARInvoiceList.Add(objARinvoice);
                    }
                }
                exceptionMessage = "";
                //-------------------------------------------------------------------------------
                #endregion ----------------------------------------------------------------------


                #region -- Populate Order -------------------------------------------------------
                //-------------------------------------------------------------------------------
                exceptionMessage = "At Populate Order Region";
                if (ds.Tables.Contains("OrderNo") && ds.Tables.Contains("OrderDetail"))
                {
                    int? salesOrderNo;
                    foreach (DataRow drSalesOrder in ds.Tables["OrderNo"].Rows)
                    {
                        salesOrderNo = DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drSalesOrder["OrderNo"]);

                        if (salesOrderNo.HasValue == false)
                        {
                            continue;
                        }

                        DataRow[] drArrayQueried = ds.Tables["OrderDetail"].Select("OrderNo = '" + salesOrderNo + "'");

                        SalesOrder objSalesOrder = new SalesOrder();
                        objSalesOrder.AcctNo = DBValueToString(drArrayQueried[0]["AcctNo"], true);
                        objSalesOrder.OrderNo = salesOrderNo;
                        objSalesOrder.CustId = DBValueToString(drArrayQueried[0]["CustId"], true);
                        objSalesOrder.PayTerm = DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drArrayQueried[0]["PayTerm"]);
                        objSalesOrder.OrderType = DBValueToString(drArrayQueried[0]["OrderType"], true);
                        objSalesOrder.PayMethod = DBValueToString(drArrayQueried[0]["PayMethod"], true);
                        objSalesOrder.BranchNo = DBValueToString(drArrayQueried[0]["BranchNo"], true);
                        objSalesOrder.BillToAddr = DBValueToString(drArrayQueried[0]["BillToAddr"], true);
                        objSalesOrder.ShipToAddr = DBValueToString(drArrayQueried[0]["ShipToAddr"], true);
                        objSalesOrder.SalesChannel = DBValueToString(drArrayQueried[0]["SalesChannel"], true);
                        objSalesOrder.InterfacedDate = DBValueToNullableValue<Object, DateTime>(Convert.ToDateTime, drArrayQueried[0]["InterfacedDate"]);

                        foreach (DataRow drQueried in drArrayQueried)
                        {
                            OrderLineDetail objOrderLine = new OrderLineDetail();
                            objOrderLine.AcctNo = DBValueToString(drQueried["AcctNo"], true);
                            objOrderLine.OrderNo = salesOrderNo;
                            objOrderLine.OrderedDate = DBValueToDateTime(drQueried["OrderedDate"]);
                            objOrderLine.TranClass = DBValueToString(drQueried["TranClass"], true);
                            objOrderLine.SalesPersonId = DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drQueried["SalesPersonId"]);
                            objOrderLine.SalesPersonName = DBValueToString(drQueried["SalesPersonName"], true);
                            objOrderLine.LineNumber = DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drQueried["LineNumber"]);
                            objOrderLine.ItemNo = DBValueToString(drQueried["ItemNo"], true); //They require int
                            objOrderLine.ItemDesc = DBValueToString(drQueried["ItemDesc"], true);
                            objOrderLine.OrderedQty = DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["OrderedQty"]);
                            objOrderLine.UnitPrice = DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["UnitPrice"]);
                            objOrderLine.UOM = DBValueToString(drQueried["UOM"], true);
                            objOrderLine.LineAmount = DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["LineAmount"]);
                            objOrderLine.TaxFlag = DBValueToString(drQueried["TaxFlag"], true);
                            objOrderLine.TaxCode = DBValueToString(drQueried["TaxCode"], true);
                            objOrderLine.TaxRate = DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["TaxRate"]);
                            objOrderLine.StatusFlag = DBValueToString(drQueried["StatusFlag"], true); //OE.Type 
                            objOrderLine.CancelReason = DBValueToString(drQueried["CancelReason"], true);
                            objOrderLine.ReturnReason = DBValueToString(drQueried["ReturnReason"], true);
                            objOrderLine.DeliveryNumber = DBValueToNullableValue<Object, Int32>(Convert.ToInt32, drQueried["DeliveryNumber"]); //DEL.BuffNo
                            objOrderLine.DeliveredDate = DBValueToDateTime(drQueried["DeliveredDate"]);
                            objOrderLine.DeliveredQty = DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["DeliveredQty"]);
                            objOrderLine.DeliveredFromLocn = DBValueToString(drQueried["DeliveredFromLocn"], true);
                            objOrderLine.FreightCarrier = DBValueToString(drQueried["FreightCarrier"], true);
                            objOrderLine.DropOffTime = DBValueToDateTime(drQueried["DropOffTime"]);
                            objOrderLine.PickUpTime = DBValueToDateTime(drQueried["PickUpTime"]);
                            objOrderLine.FreightCharge = DBValueToNullableValue<Object, Decimal>(Convert.ToDecimal, drQueried["FreightCharge"]);
                            objOrderLine.DeliveryComments = DBValueToString(drQueried["DeliveryComments"], true);
                            objOrderLine.ScheduledDelDate = DBValueToDateTime(drQueried["ScheduledDelDate"]);

                            objSalesOrder.OrderLineList.Add(objOrderLine);
                        }

                        objDataContainer.SalesOrderList.Add(objSalesOrder);
                    }
                }
                exceptionMessage = "";
                //-------------------------------------------------------------------------------
                #endregion ----------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                throw new Exception(exceptionMessage, ex);
            }

            objDataContainer.RunNo = newRunNo;
            return objDataContainer;
        }

        public DataSet dsGetOrderAndDeliveries(SqlConnection conn, int runNo, int newRunNo)
        {
            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();
            DataSet ds = objDOracleInteg.GetOrderAndDeliveries(conn, runNo, newRunNo);
            return ds;
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

        public void ResetRunNo(SqlConnection conn, int runNo)
        {
            DOracleIntegration2 objDOracleInteg = new DOracleIntegration2();
            objDOracleInteg.ResetRunNo(conn, runNo);
        }


        private Nullable<TOutput> DBValueToNullableValue<TInput, TOutput>(Converter<TInput, TOutput> converter, TInput dbValue)
            where TOutput : struct
            where TInput : class
        {
            if (dbValue == null || dbValue == DBNull.Value)
                return default(TOutput);

            return (Nullable<TOutput>)(converter(dbValue));
        }

        private String DBValueToString(Object dbValue, bool returnDefaultChar)
        {
            if (dbValue == null || dbValue == DBNull.Value || dbValue.ToString().Length == 0)
                return returnDefaultChar ? "-" : "";

            return dbValue.ToString();
        }

        private DateTime DBValueToDateTime(Object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value)
                return new DateTime(1900, 1, 1);

            return Convert.ToDateTime(dbValue);
        }
    }
}
