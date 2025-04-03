using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared.Extensions;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Elements;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Tags;
using STL.DAL;
using STL.Common.Services;

namespace STL.BLL
{
    /// <summary>
    /// Takes care of business logic associated with items
    /// </summary>
    public class BItem : CommonObject
    {
        //private int itemNodeCounter = 0;
        private DDelivery _deliveryDA = null;
        public DDelivery Delivery
        {
            get { return _deliveryDA; }
            set { _deliveryDA = value; }
        }
        private DFinTrans _fintransDA = null;
        public DFinTrans FinTrans
        {
            get { return _fintransDA; }
            set { _fintransDA = value; }
        }
        private DLineWarranty _warrantyDA = null;
        public DLineWarranty Warranty
        {
            get { return _warrantyDA; }
            set { _warrantyDA = value; }
        }
        private short _origbr = 0;
        public short OrigBr
        {
            get { return _origbr; }
            set { _origbr = value; }
        }
        private string _acctNo = "";
        public string AccountNumber
        {
            get { return _acctNo; }
            set { _acctNo = value; }
        }
        private int _agreementNo = 0;
        public int AgreementNumber
        {
            get { return _agreementNo; }
            set { _agreementNo = value; }
        }
        private string _itemNo = "";
        public string ItemNumber
        {
            get { return _itemNo; }
            set { _itemNo = value; }
        }

        //private int _itemID = 0;        //RI jec 28/04/11
        //public int ItemId
        //{
        //    get { return _itemID; }
        //    set { _itemID = value; }
        //}

        public int ItemId { get; set; }

        private string _itemSuppText = "";
        public string ItemSuppText
        {
            get { return _itemSuppText; }
            set { _itemSuppText = value; }
        }
        private double _quantity = 0;
        public double Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        private double _delqty = 0;
        public double DeliveredQuantity
        {
            get { return _delqty; }
            set { _delqty = value; }
        }
        private double _scheduledQty = 0;
        public double ScheduledQuantity
        {
            get { return _scheduledQty; }
            set { _scheduledQty = value; }
        }
        private short _stockLocn = 0;
        public short StockLocation
        {
            get { return _stockLocn; }
            set { _stockLocn = value; }
        }
        private decimal _price = 0;
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }
        private decimal _orderVal = 0;
        public decimal OrderValue
        {
            get { return _orderVal; }
            set { _orderVal = value; }
        }
        private DateTime _dateReqDel = DateTime.MinValue.AddYears(1899);
        public DateTime DateRequiredDelivery
        {
            get { return _dateReqDel; }
            set { _dateReqDel = value; }
        }
        private string _timeReqDel = "";
        public string TimeRequiredDelivery
        {
            get { return _timeReqDel; }
            set { _timeReqDel = value; }
        }
        private DateTime _datePlanDel = DateTime.MinValue.AddYears(1899);
        public DateTime DatePlannedDelivery
        {
            get { return _datePlanDel; }
            set { _datePlanDel = value; }
        }
        private short _delNoteBranch = 0;
        public short DeliveryNoteBranch
        {
            get { return _delNoteBranch; }
            set { _delNoteBranch = value; }
        }
        private string _qtyDiff = "";
        public string QuantityDiff
        {
            get { return _qtyDiff; }
            set { _qtyDiff = value; }
        }
        private string _itemType = "";
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }
        private short _hasString = 0;
        public short HasString
        {
            get { return _hasString; }
            set { _hasString = value; }
        }
        private string _notes = "";
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        private double _taxAmount = 0;
        public double TaxAmount
        {
            get { return _taxAmount; }
            set { _taxAmount = value; }
        }
        private string _parentItemNo = "";
        public string ParentItemNumber
        {
            get { return _parentItemNo; }
            set { _parentItemNo = value; }
        }

        public int ParentItemId { get; set; }

        private short _parentStockLocn = 0;
        public short ParentStockLocation
        {
            get { return _parentStockLocn; }
            set { _parentStockLocn = value; }
        }
        private short _isKit = 0;
        public short IsKit
        {
            get { return _isKit; }
            set { _isKit = value; }
        }
        private string _deliveryAddress = "";
        public string DeliveryAddress
        {
            get { return _deliveryAddress; }
            set { _deliveryAddress = value; }
        }

        private string _deliveryArea = "";
        public string DeliveryArea
        {
            get { return _deliveryArea; }
            set { _deliveryArea = value; }
        }

        private string _deliveryProcess = "";
        public string DeliveryProcess
        {
            get { return _deliveryProcess; }
            set { _deliveryProcess = value; }
        }

        private decimal _realDiscount = 0;
        public decimal realDiscount
        {
            get { return _realDiscount; }
        }

        private string _contractNo = "";
        public string ContractNo
        {
            get { return _contractNo; }
            set { _contractNo = value; }
        }

        private DateTime _expectedreturn = DateTime.MinValue.AddYears(1899);
        public DateTime ExpectedReturnDate
        {
            get { return _expectedreturn; }
            set { _expectedreturn = value; }
        }

        private short _promobranch = 0;
        public short PromoBranch
        {
            get { return _promobranch; }
            set { _promobranch = value; }
        }

        private string _damaged = "";
        public string Damaged
        {
            get { return _damaged; }
            set { _damaged = value; }
        }

        private string _assembly = "";
        public string Assembly
        {
            get { return _assembly; }
            set { _assembly = value; }
        }

        private string _auditsource = "";
        public string AuditSource
        {
            get { return _auditsource; }
            set { _auditsource = value; }
        }

        private short _accountbranch = 0;
        public short AccountBranch
        {
            get { return _accountbranch; }
            set { _accountbranch = value; }
        }

        private double _taxrate = 0;
        public double Taxrate
        {
            get { return _taxrate; }
            set { _taxrate = value; }
        }

        //IP - 23/05/11 - CR1212 - RI - #3651
        private short? _salesBrnNo = 0;
        public short? SalesBrnNo
        {
            get { return _salesBrnNo; }
            set { _salesBrnNo = value; }
        }

        //IP - 07/06/12 - #10229 - Warehouse & Deliveries
        private string _express = "";
        public string Express
        {
            get { return _express; }
            set { _express = value; }
        }

       

        public void DeliverItem(SqlConnection conn, SqlTransaction trans)
        {
            Delivery.Write(conn, trans);
        }

        public void WriteFinTrans(SqlConnection conn, SqlTransaction trans)
        {
            DBranch dbranch = new DBranch();
            FinTrans.TransRefNo = dbranch.BranchTransrefnoCheckUpdate(FinTrans.AccountNumber, FinTrans.BranchNumber, FinTrans.TransRefNo);
            FinTrans.Write(conn, trans);
        }

        /// <summary>
        /// this method will save the current line item details to the database
        /// </summary>
        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            DFACTTrans fact = new DFACTTrans();
            DLineItem line = new DLineItem();
            line.OrigBr = this.OrigBr;
            line.AccountNumber = this.AccountNumber;
            line.AgreementNumber = this.AgreementNumber;
            line.ItemNumber = this.ItemNumber;
            line.ItemID = this.ItemId;
            line.ItemSuppText = this.ItemSuppText;
            line.Quantity = this.Quantity;
            //JPJ line.DeliveredQuantity = this.DeliveredQuantity;
            //line.DeliveredQuantity = this.ScheduledQuantity;
            line.StockLocation = this.StockLocation;
            line.Price = this.Price;
            line.OrderValue = this.OrderValue;
            line.DateRequiredDelivery = this.DateRequiredDelivery;
            line.TimeRequiredDelivery = this.TimeRequiredDelivery;
            line.DatePlannedDelivery = this.DatePlannedDelivery;
            line.DeliveryNoteBranch = this.DeliveryNoteBranch;
            line.QuantityDiff = this.QuantityDiff;
            line.ItemType = this.ItemType;
            line.HasString = this.HasString;
            line.Notes = this.Notes;
            line.TaxAmount = this.TaxAmount;
            line.Taxrate = this.Taxrate;
            line.ParentItemNumber = this.ParentItemNumber;
            line.ParentItemID = this.ParentItemId;
            line.ParentStockLocation = this.ParentStockLocation;
            line.IsKit = this.IsKit;
            line.DeliveryAddress = this.DeliveryAddress;
            line.DeliveryArea = this.DeliveryArea;
            line.DeliveryProcess = this.DeliveryProcess;
            line.BuffNo = fact.GetOrderNo(conn, trans, this.AccountNumber, this.AgreementNumber);
            if (line.BuffNo == 0)
            {
                DBranch b = new DBranch();
                line.BuffNo = b.GetBuffNo(conn, trans, Convert.ToInt16(this.AccountNumber.Substring(0, 3)));            // #16339
            }
            line.ContractNo = this.ContractNo;
            line.ExpectedReturnDate = this.ExpectedReturnDate;
            line.DeliveryArea = this.DeliveryArea;
            line.DeliveryProcess = this.DeliveryProcess;
            line.Damaged = this.Damaged;
            line.Assembly = this.Assembly;
            line.User = this.User;
            line.AuditSource = this.AuditSource;
            line.SalesBrnNo = this.SalesBrnNo;              //IP - 23/05/11 - CR1212 - RI - #3651
            line.Express = this.Express;                    //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            line.Save(conn, trans);
        }

        /// <summary>
        /// this method will save the current line item details to the database
        /// </summary>
        public void CreateLineItemBfCollection(SqlConnection conn, SqlTransaction trans, double bfQty, decimal bfValue, decimal bfPrice)
        {
            DLineItemBfCollection bfline = new DLineItemBfCollection();
            bfline.AccountNumber = this.AccountNumber;
            bfline.AgreementNumber = this.AgreementNumber;
            bfline.ItemNumber = this.ItemNumber;
            bfline.ItemID = this.ItemId;                    //IP - 06/06/11 - CR1212 - RI - #3806
            bfline.Quantity = bfQty;
            bfline.Price = bfPrice;
            bfline.OrderValue = bfValue;
            bfline.ContractNo = this.ContractNo;
            bfline.User = this.User;
            bfline.Save(conn, trans);
        }

        /// <summary>
        /// Restore the LineItem's Quantity and OrdVal values to what they were before the
        /// goods were returned. Update the totals on Agreement and Acct accordingly. Also,
        /// remove the schedule row that was created for the Goods Return.
        /// </summary>
        public void CancelCollectionNote(SqlConnection conn, SqlTransaction trans, string acctNo, int agrmtNo,
            int itemID, string contractNo, short stockLocn, int buffNo, int buffBranchNo,                       //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
            short branchNo, int lineItemId)     //#10544
        {
            DSchedule dSched = new DSchedule();

            dSched.AccountNumber = acctNo;
            dSched.AgreementNumber = agrmtNo;
            //dSched.ItemNumber = itemNo;
            dSched.ItemNumber = string.Empty;     //IP/NM - 18/05/11 -CR1212 - #3627
            dSched.ItemID = itemID;   //IP/NM - 18/05/11 -CR1212 - #3627
            dSched.StockLocation = stockLocn;

            //Check if any warranty adjustments were debited to the
            //account during GRT and reverse them.
            dSched.GetScheduledAssociatedItems(conn, trans);

            foreach (DataRow r in dSched.Schedules.Rows)
            {
                if (r["contractno"].ToString() != string.Empty)
                {
                    dSched.DeleteFromCalculateCRECRF(conn, trans, acctNo, r["contractno"].ToString(), Convert.ToInt16(r["stocklocn"]));
                }
            }

           
            // UAT160 Removed as crediting account (reducing outstbal) when GRT reversed which is incorrect
            //if (dSched.Schedules.Rows.Count > 0)
            //{                
            //    for (int i = 0; i < dSched.Schedules.Rows.Count; i++)   //UAT160 may be more than 1
            //    {
            //        ReverseWarrantyAdjustment(conn, trans, acctNo, agrmtNo, branchNo,
            //                                  dSched.Schedules.Rows[i]);        //UAT160 may be more than 1
            //    }
            //}
            var bookingSchedule = new WarehouseRepository().GetLineItemBookingSchedule(conn, trans, lineItemId);

            if (new WarehouseRepository().GetLineItemBooking(conn, trans, lineItemId) != null)      // #10593 check if scheduled booking 
            {
                var cancelations = new AccountRepository().GetCancelCollectionNoteData(conn, trans, lineItemId, this.User, "");     // #16525
                //new Chub().CancelMany(cancelations);

                //#11989
                var bookingId = Convert.ToInt32(cancelations.ToDataTable().Rows[0]["Id"]);                

                new ServiceRepository().ReverseReplacementActioned(conn, trans, acctNo, itemID, stockLocn, bookingId);                   
            }

            bool isCancellation = false;
            string auditSource = "";

            //1. Retrieve the LineItemBfCollection row for the specified key
            DLineItemBfCollection lbf = new DLineItemBfCollection();
            lbf.GetLineItemBfCollection(conn, trans, acctNo, agrmtNo, itemID, contractNo, out isCancellation);  //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo

            // Need to know the source of the original action so
            // the corrcet source is recored in the audit tables
            if (isCancellation)
                auditSource = AS.GRTCancelRev;
            else
                auditSource = AS.GRTExchangeRev;


            //Note : There won't be a LineItemBfCollection row for old orders!
            foreach (DataRow row in lbf.ItemDetails.Rows)       //UAT160
            //if (lbf.ItemDetails.Rows.Count > 0) 
            {
                if (Convert.ToInt32(row[CN.ItemID]) == itemID)                      //IP - 11/01/12 - #9411 - Only process for the ItemID passed into this method.
                {
                    //2. Update the corresponding LineItem, restoring the old values
                    decimal taxAmount = 0;
                    //ProcessLineItems(conn, trans, acctNo, agrmtNo, itemNo, stockLocn,
                    //                 contractNo, Convert.ToDecimal(lbf.Quantity),
                    //                 Convert.ToDecimal(lbf.OrderValue), auditSource,
                    //                 lbf.WarrantyNo, lbf.WarrantyLocn, lbf.ExchangedWarrantyContractNo, ref taxAmount);

                    //UAT160
                    lbf.ItemNumber = Convert.ToString(row[CN.ItemNo]);
                    lbf.ContractNo = Convert.ToString(row[CN.ContractNo]);
                    lbf.ItemID = Convert.ToInt32(row[CN.ItemId]);               //IP - 06/06/11 - CR1212 - RI - #3806

                    ProcessLineItems(conn, trans, acctNo, agrmtNo, itemID, stockLocn,                               //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo   
                                     Convert.ToString(row[CN.ContractNo]), Convert.ToDecimal(row[CN.Quantity]),
                                     Convert.ToDecimal(row[CN.OrdVal]), auditSource,
                                     Convert.ToInt32(row[CN.WarrantyId]), Convert.ToInt16(row[CN.WarrantyLocn]), Convert.ToString(row[CN.ExchangeContractNo]), ref taxAmount);  //IP - CR1212 - RI - #3806 

                    //3. Update agreement using bf 'value' (subract it from AgreementTotal and CashPrice)#
                    DAgreement agreement = new DAgreement(conn, trans, acctNo, agrmtNo);
                    if (Convert.ToString(row[CN.ContractNo]) == string.Empty)       //UAT160  only update Agreement total if not warranty
                    {
                        agreement.AgreementTotal += Convert.ToDecimal(row[CN.OrdVal]);      //UAT160 
                        agreement.CashPrice += Convert.ToDecimal(row[CN.OrdVal]);       //UAT160 
                    }

                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        agreement.AgreementTotal += taxAmount;
                        agreement.CashPrice += taxAmount;
                    }

                    //IP - 16/03/12 - #9797 - Get the sum OrdVal from Lineitem to give the agreement total.
                    AccountRepository ar = new AccountRepository();
                    decimal? sumOrdVal = ar.SumOrdValForAcct(conn, trans, acctNo, agrmtNo);

                    if (sumOrdVal.HasValue)
                    {
                        agreement.AgreementTotal = Convert.ToDecimal(sumOrdVal);
                    }


                    agreement.AuditSource = auditSource;
                    agreement.DateChange = DateTime.Now;            //#14392
                    agreement.Save(conn, trans);

                    //4. Save the agreement value on the acct
                    DAccount acct = new DAccount(conn, trans, acctNo);
                    acct.AgreementTotal = agreement.AgreementTotal;
                    acct.Save(conn, trans);

                    BDelivery del = new BDelivery();
                    del.User = this.User;

                    //5. Recalculate DT
                    if (AT.IsCreditType(acct.AccountType))
                    {
                        // Recalculate Service Charge using new OrdVal amounts
                        // This expects the agreement cash price to exclude service charges
                        decimal chargeableAdminPrice = 0;
                        agreement.CashPrice = acct.GetChargeableCashPrice(conn, trans, acctNo, ref chargeableAdminPrice);
                        // This will save the agreement with a new total
                        del.RecalculateServiceCharge(conn, trans, acct, agreement);
                    }

                    DBranch branch = new DBranch();
                    int transRefNo = branch.GetTransRefNo(conn, trans, branchNo);
                    decimal transValue = 0;

                    del.DeliverNonStocks(conn, trans, acctNo, acct.AccountType,
                        Country[CountryParameterNames.CountryCode].ToString(), branchNo,
                        transRefNo, ref transValue, agrmtNo);

                    // 6. Write fintrans record for revised DT value
                    BTransaction t = null;
                    if (Math.Abs(transValue) > 0)
                        t = new BTransaction(conn, trans, acctNo, branchNo,
                            transRefNo, transValue, this.User,
                            TransType.Delivery, "", "", "", 0, Country[CountryParameterNames.CountryCode].ToString(),
                            DateTime.Today, FootNote.CancelCollectionNote, agrmtNo);


                    //7. remove the bf row
                    lbf.Delete(conn, trans);
                }
            }

            //8. Remove the corresponding Schedule row & any scheduled 
            //   associated warranties/discounts
            BSchedule sched = new BSchedule();
            sched.DeleteSingleSchedule(conn, trans, acctNo, agrmtNo, itemID, stockLocn, buffNo, buffBranchNo); //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo

            // #18038 - not required - cancellation messages are not sent from Win to Web -- #14313 - IDentical Replacement - Cancellation for Collection
            ////if (Convert.ToString(bookingSchedule.DelOrColl) == "D")
            ////{
            ////    var cancelations = new AccountRepository().GetCancelDataIR(conn, trans, lineItemId,Convert.ToInt32(bookingSchedule.BookingId), this.User, "Cancellation - collection cancelled.");
            ////    new Chub().CancelMany(cancelations, conn, trans);
            ////}

            // Audit this cancellation
            dSched.ScheduleCollectionAudit(conn, trans,
                acctNo, agrmtNo, itemID, stockLocn, stockLocn, lbf.Quantity, 0, this.User); //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo

            // Delete and Audit the associated items
            dSched.DeleteAssociatedItems(conn, trans, this.User);

            //#10544 - Delete LineItemBookingSchedule
            //new WarehouseRepository().DeleteLineItemBookingSchedule(conn, trans, buffNo);

            //#13696 - Update LineItemBookingFailure
            new WarehouseRepository().UpdateBookingFailureActioned(conn, trans, buffNo,0);
        }

        /// <summary>
        /// returns a dataset containing stock items
        /// by branch location
        /// </summary>
        /// <param name="branchCode">branch number</param>
        /// <param name="showDeleted">whether or not to show deleted stock items</param>
        /// <param name="showAvailable">whether or not to show unavailable stock</param>
        /// <param name="prodDesc">prodcut description search term</param>
        /// <returns></returns>
        public DataSet GetStockByLocation(short stocklocn, short branchCode,
            int showDeleted,
            int showAvailable,
            string prodDesc,
            bool limit)
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.GetStockByLocation(stocklocn, branchCode, showDeleted, showAvailable, prodDesc, limit ? 250 : 4000);
            ds.Tables.Add(stock.ByLocation);
            return ds;
        }

        /// <summary>
        /// returns a dataset returning locations for a particular
        /// item of stock.
        /// </summary>
        /// <param name="productCode">Product code</param>
        /// <returns></returns>
        public DataSet GetStockByCode(string productCode)
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.GetStockByCode(productCode);
            ds.Tables.Add(stock.ByCode);
            return ds;
        }

        /// <summary>
        /// RD 10/12/2002
        /// Method to return a Auto Warranty Contract Number 
        /// </summary>
        /// <param name="productCode">Product code</param>
        /// <returns></returns>
        public void AutoWarranty(string branchno, out string contractno)
        {

            DLineWarranty warrantycontractno = new DLineWarranty();
            warrantycontractno.AutoWarranty(branchno, out contractno);

        }
        /// <summary>
        /// returns a dataset containing all warranties which can be
        /// sold with a particular product
        /// </summary>
        /// <param name="productCode">product code</param>
        /// <param name="branchCode">branch code</param>
        /// <param name="unitPrice">product unit price</param>
        /// <returns></returns>
        public DataSet GetProductWarranties(SqlConnection conn, SqlTransaction trans, int itemId, short branchCode, double unitPrice, string refCode, bool paidAndTaken)
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.GetProductWarranties(conn, trans, itemId, branchCode, unitPrice, refCode, paidAndTaken);
            ds.Tables.Add(stock.Warranties);
            return ds;
        }


        /// <summary>
        /// Overload method
        /// </summary>
        /// <param name="productCode">Product code</param>
        /// <param name="branchCode">branch number</param>
        /// <param name="accountType">account type (C,H,S,B)</param>
        /// <returns></returns>
        public XmlNode GetItemDetails(int itemId, short stocklocn, string accountType, string countryCode, bool dutyFree, bool taxExempt, string accountNo = null, int agrmtNo = 1, SqlConnection conn = null, SqlTransaction trans = null)
        {
            return GetItemDetails(itemId, stocklocn, stocklocn, accountType, countryCode, dutyFree, taxExempt, accountNo, agrmtNo, conn, trans);
        }

        /// <summary>
        /// Returns the item details for an item selected from
        /// the stock enquiry by location screen
        /// </summary>
        /// <param name="productCode">Product code</param>
        /// <param name="branchCode">branch number</param>
        /// <param name="accountType">account type (C,H,S,B)</param>
        /// <returns></returns>
        public XmlNode GetItemDetails(int itemID, short stocklocn, short branchCode, string accountType, string countryCode, bool dutyFree, bool taxExempt, string accountNo = null, int agrmtNo = 1, SqlConnection conn = null, SqlTransaction trans = null)
        {
            XmlDocument doc = new XmlDocument();
            XmlUtilities xml = new XmlUtilities();
            XmlNode itemNode = this.CreateItemNode(doc);
            DStockItem stock = new DStockItem();
            bool canAddWarranty = false;
            string type = IT.Unknown;
            // short stockLocation = 0;

            stock.PromoBranch = this.PromoBranch;
            // jeh - Correcting incorrect change
            //stockLocation = branchCode;

            //Changing incorrect code issue 69861
            //stockLocation = AccountBranch == 0 ? branchCode : AccountBranch;
            //stockLocation = branchCode;

            stock.GetItemDetails(conn, trans, itemID, stocklocn, branchCode, accountType, countryCode, dutyFree, taxExempt, accountNo, agrmtNo);

            itemNode.Attributes[Tags.Key].Value = itemID + "|" + stocklocn.ToString();

            //IP - 19/08/08 - (69962) - Added a check to ensure that the item being added
            //can have a warranty added to it.

            DLineItem li = new DLineItem()
            {
                StockLocation = stocklocn,
                Price = Convert.ToDecimal(stock.UnitPrice),
                ItemID = itemID
            };

            //Check if this item can have a warranty added to it
            switch (li.CanAddWarrantyOnNewItem(null, null))
            {
                case 0: canAddWarranty = true;
                    break;
                //IP - 19/08/08 - Commented out case 3, as in the stored procedure
                //'DN_WarrantyCanAddSP', if the result returned was '3' then you should
                //not be able to add a warranty to the item.
                //case 3:	canAddWarranty = true;	//2 means the item already has a 
                //break;						//warranty attached but it can 
                //still potentially be amended
                default: canAddWarranty = false;
                    break;
            }

            if (stock.IsStock)
            {
                type = IT.Stock;
                //canAddWarranty = true; --IP - 19/08/08 - (69962) - Now set by the above
            }
            if (stock.IsKit)
            {
                type = IT.Kit;
                //canAddWarranty = true;  --IP - 19/08/08 - (69962) - Now set by the above
            }
            if (stock.IsDiscount)
                type = IT.Discount;
            if (stock.IsWarranty)
                type = IT.Warranty;
            if (stock.IsAffinity)
                type = IT.Affinity;
            if (stock.IsStampDuty)
                type = IT.SundryCharge;
            if (stock.IsInstallation)       //IP - 24/02/11 - #3130
                type = IT.Installation;
            if (stock.IsAssemblyCost)
                type = IT.AssemblyCost;
            if (stock.IsAnnualServiceContract)
                type = IT.AnnualService;
            if (stock.IsGenericService)
                type = IT.GenericService;

            //if(stock.ReadyAssist)
            //    type = IT.ReadyAssist;      //#18604 - CR15594

            itemNode.Attributes[Tags.Type].Value = type;
            itemNode.Attributes[Tags.Code].Value = stock.ItemNo;
            itemNode.Attributes[Tags.Location].Value = stocklocn.ToString();
            itemNode.Attributes[Tags.AvailableStock].Value = stock.AvailableStock.ToString();
            itemNode.Attributes[Tags.DamagedStock].Value = stock.DamagedStock.ToString();
            itemNode.Attributes[Tags.Description1].Value = stock.ProductDesc1;
            itemNode.Attributes[Tags.Description2].Value = stock.ProductDesc2;
            itemNode.Attributes[Tags.SupplierCode].Value = stock.SupplierCode;
            itemNode.Attributes[Tags.UnitPrice].Value = StripCurrency(stock.UnitPrice.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.CashPrice].Value = StripCurrency(stock.CashPrice.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.CostPrice].Value = StripCurrency(stock.CostPrice.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.HPPrice].Value = StripCurrency(stock.HPPrice.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.DutyFreePrice].Value = StripCurrency(stock.DutyFreePrice.ToString(DecimalPlaces));
            itemNode.Attributes[Tags.ValueControlled].Value = stock.ValueControlled.ToString();
            itemNode.Attributes[Tags.Quantity].Value = "0";
            itemNode.Attributes[Tags.Value].Value = "0";
            itemNode.Attributes[Tags.DeliveryDate].Value = "";
            itemNode.Attributes[Tags.DeliveryTime].Value = "";
            itemNode.Attributes[Tags.BranchForDeliveryNote].Value = stock.DelNoteBranch.ToString();
            itemNode.Attributes[Tags.ColourTrim].Value = stock.Style + Environment.NewLine + stock.ColourName;
            itemNode.Attributes[Tags.TaxRate].Value = stock.TaxRate.ToString();
            itemNode.Attributes[Tags.DeliveredQuantity].Value = "0";
            itemNode.Attributes[Tags.PlannedDeliveryDate].Value = "";
            itemNode.Attributes[Tags.CanAddWarranty].Value = canAddWarranty.ToString();
            itemNode.Attributes[Tags.DeliveryAddress].Value = "";
            itemNode.Attributes[Tags.DeliveryArea].Value = "";
            itemNode.Attributes[Tags.DeliveryProcess].Value = "";
            itemNode.Attributes[Tags.QuantityDiff].Value = "Y";
            itemNode.Attributes[Tags.ScheduledQuantity].Value = "0";
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            itemNode.Attributes[Tags.ContractNumber].Value = "";
            itemNode.Attributes[Tags.ReturnItemNo].Value = stock.ItemNo; ;
            itemNode.Attributes[Tags.ReturnLocation].Value = branchCode.ToString();
            itemNode.Attributes[Tags.FreeGift].Value = stock.IsFreeGift.ToString();
            itemNode.Attributes[Tags.ExpectedReturnDate].Value = "";
            itemNode.Attributes[Tags.QtyOnOrder].Value = stock.QtyOnOrder.ToString();
            itemNode.Attributes[Tags.PurchaseOrder].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.LeadTime].Value = stock.LeadTime.ToString();
            itemNode.Attributes[Tags.Assembly].Value = stock.AssemblyRequired;
            itemNode.Attributes[Tags.Damaged].Value = "";
            itemNode.Attributes[Tags.ProductCategory].Value = stock.ProductCategory;
            itemNode.Attributes[Tags.SparePartsCategory].Value = stock.SparePartsCategory; // Required for selecting a spare part for a Service Request JH 08/11/2007
            itemNode.Attributes[Tags.Deleted].Value = stock.Deleted;
            itemNode.Attributes[Tags.PurchaseOrderNumber].Value = "";
            itemNode.Attributes[Tags.ReplacementItem].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.SPIFFItem].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.RefCode].Value = stock.RefCode; //IP - 28/01/10 - LW 72136
            itemNode.Attributes[Tags.ItemRejected].Value = "False";  //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            itemNode.Attributes[Tags.Category].Value = stock.Category.ToString();
            itemNode.Attributes[Tags.ColourName].Value = stock.ColourName ?? "";   //CR1212 jec 21/04/11
            itemNode.Attributes[Tags.Style].Value = stock.Style ?? "";   //CR1212 jec 21/04/11
            itemNode.Attributes[Tags.ItemId].Value = stock.ItemID.ToString();
            itemNode.Attributes[Tags.SalesBrnNo].Value = "";      //IP - 23/05/11 - CR1212 - RI - #3651
            itemNode.Attributes[Tags.RepoItem].Value = stock.RepoItem.ToString();      // RI jec 17/06/11
            itemNode.Attributes[Tags.Class].Value = stock.Class.ToString();            //IP - 27/07/11 - RI - #4415
            itemNode.Attributes[Tags.SubClass].Value = stock.SubClass.ToString();      //IP - 27/07/11 - RI - #4415
            itemNode.Attributes[Tags.Brand].Value = stock.Brand.ToString();            //IP - 19/09/11 - RI - #8218 - CR8201
            itemNode.Attributes[Tags.ReadyAssist].Value = stock.ReadyAssist.ToString();           //#18604 - CR15594

            itemNode.AppendChild(doc.CreateElement(Elements.RelatedItem));

            return itemNode;
        }

        public XmlNode GetItemDetails(SqlConnection conn, SqlTransaction trans, int itemId, short branchCode, string accountType, string countryCode, bool dutyFree, bool taxExempt, string accountNo = null, int agrmtNo = 1)
        {
            XmlDocument doc = new XmlDocument();
            XmlUtilities xml = new XmlUtilities();
            XmlNode itemNode = this.CreateItemNode(doc);
            DStockItem stock = new DStockItem();
            bool canAddWarranty = false;
            string type = "";

            stock.GetItemDetails(conn, trans, itemId, branchCode, accountType, countryCode, dutyFree, taxExempt, accountNo, agrmtNo);

            itemNode.Attributes[Tags.Key].Value = itemId + "|" + branchCode.ToString();
            if (stock.IsStock)
            {
                type = IT.Stock;
                canAddWarranty = true;
            }
            if (stock.IsKit)
            {
                type = IT.Kit;
                canAddWarranty = true;
            }
            if (stock.IsDiscount)
                type = IT.Discount;
            if (stock.IsWarranty)
                type = IT.Warranty;
            if (stock.IsAffinity)
                type = IT.Affinity;
            if (stock.IsStampDuty)
                type = IT.SundryCharge;

            itemNode.Attributes[Tags.Type].Value = type;
            itemNode.Attributes[Tags.Code].Value = stock.ItemNo;
            itemNode.Attributes[Tags.ItemId].Value = stock.ItemID.ToString();
            itemNode.Attributes[Tags.Location].Value = branchCode.ToString();
            itemNode.Attributes[Tags.AvailableStock].Value = stock.AvailableStock.ToString();
            itemNode.Attributes[Tags.DamagedStock].Value = stock.DamagedStock.ToString();
            itemNode.Attributes[Tags.Description1].Value = stock.ProductDesc1;
            itemNode.Attributes[Tags.Description2].Value = stock.ProductDesc2;
            itemNode.Attributes[Tags.SupplierCode].Value = stock.SupplierCode;
            itemNode.Attributes[Tags.UnitPrice].Value = stock.UnitPrice.ToString();
            itemNode.Attributes[Tags.CashPrice].Value = stock.CashPrice.ToString();
            itemNode.Attributes[Tags.HPPrice].Value = stock.HPPrice.ToString();
            itemNode.Attributes[Tags.DutyFreePrice].Value = stock.DutyFreePrice.ToString();
            itemNode.Attributes[Tags.ValueControlled].Value = stock.ValueControlled.ToString();
            itemNode.Attributes[Tags.Quantity].Value = "0";
            itemNode.Attributes[Tags.Value].Value = "0";
            itemNode.Attributes[Tags.DeliveryDate].Value = "";
            itemNode.Attributes[Tags.DeliveryTime].Value = "";
            itemNode.Attributes[Tags.BranchForDeliveryNote].Value = "";
            itemNode.Attributes[Tags.ColourTrim].Value = "";
            itemNode.Attributes[Tags.TaxRate].Value = stock.TaxRate.ToString();
            itemNode.Attributes[Tags.DeliveredQuantity].Value = "0";
            itemNode.Attributes[Tags.PlannedDeliveryDate].Value = "";
            itemNode.Attributes[Tags.CanAddWarranty].Value = canAddWarranty.ToString();
            itemNode.Attributes[Tags.DeliveryAddress].Value = "";
            itemNode.Attributes[Tags.DeliveryArea].Value = "";
            itemNode.Attributes[Tags.DeliveryProcess].Value = "";
            itemNode.Attributes[Tags.QuantityDiff].Value = "Y";
            itemNode.Attributes[Tags.ScheduledQuantity].Value = "0";
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            itemNode.Attributes[Tags.ContractNumber].Value = "";
            itemNode.Attributes[Tags.ReturnItemNo].Value = stock.ItemNo;
            itemNode.Attributes[Tags.ReturnLocation].Value = branchCode.ToString();
            itemNode.Attributes[Tags.FreeGift].Value = stock.IsFreeGift.ToString();
            itemNode.Attributes[Tags.ExpectedReturnDate].Value = "";
            itemNode.Attributes[Tags.QtyOnOrder].Value = stock.QtyOnOrder.ToString();
            itemNode.Attributes[Tags.PurchaseOrder].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.LeadTime].Value = stock.LeadTime.ToString();
            itemNode.Attributes[Tags.Assembly].Value = "";
            itemNode.Attributes[Tags.Damaged].Value = "";
            itemNode.Attributes[Tags.ProductCategory].Value = stock.ProductCategory;
            itemNode.Attributes[Tags.SparePartsCategory].Value = stock.SparePartsCategory;
            itemNode.Attributes[Tags.Deleted].Value = stock.Deleted;
            itemNode.Attributes[Tags.PurchaseOrderNumber].Value = "";
            itemNode.Attributes[Tags.ReplacementItem].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.SalesBrnNo].Value = "";             //IP - 23/05/11 - CR1212 - RI - #3651
            itemNode.Attributes[Tags.RepoItem].Value = stock.RepoItem.ToString();      // RI jec 17/06/11

            itemNode.Attributes[Tags.Class].Value = stock.Class.ToString();            //IP - 27/07/11 - RI - #4415 ?
            itemNode.Attributes[Tags.SubClass].Value = stock.SubClass.ToString();      //IP - 27/07/11 - RI - #4415 ?
            itemNode.Attributes[Tags.Brand].Value = stock.Brand.ToString();            //IP - 19/09/11 - RI - #8218 - CR8201
            itemNode.Attributes[Tags.Style].Value = stock.Style.ToString();            //IP - 20/09/11 - RI - #8218 - CR8201
            itemNode.Attributes[Tags.Express].Value = "";                              //IP - 07/06/12 - #10229 - Warehouse & Deliveries

            itemNode.AppendChild(doc.CreateElement(Elements.RelatedItem));

            return itemNode;
        }

        public XmlNode CreateItemNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(Elements.Item);
            node.Attributes.Append(doc.CreateAttribute(Tags.Key));
            node.Attributes.Append(doc.CreateAttribute(Tags.Type));
            node.Attributes.Append(doc.CreateAttribute(Tags.Code));
            node.Attributes.Append(doc.CreateAttribute(Tags.Location));
            node.Attributes.Append(doc.CreateAttribute(Tags.AvailableStock));
            node.Attributes.Append(doc.CreateAttribute(Tags.DamagedStock));
            node.Attributes.Append(doc.CreateAttribute(Tags.Description1));
            node.Attributes.Append(doc.CreateAttribute(Tags.Description2));
            node.Attributes.Append(doc.CreateAttribute(Tags.SupplierCode));
            node.Attributes.Append(doc.CreateAttribute(Tags.UnitPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.CashPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.CostPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.HPPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.DutyFreePrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.ValueControlled));
            node.Attributes.Append(doc.CreateAttribute(Tags.Quantity));
            node.Attributes.Append(doc.CreateAttribute(Tags.Value));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryDate));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryTime));
            node.Attributes.Append(doc.CreateAttribute(Tags.BranchForDeliveryNote));
            node.Attributes.Append(doc.CreateAttribute(Tags.ColourTrim));
            node.Attributes.Append(doc.CreateAttribute(Tags.TaxRate));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveredQuantity));
            node.Attributes.Append(doc.CreateAttribute(Tags.PlannedDeliveryDate));
            node.Attributes.Append(doc.CreateAttribute(Tags.CanAddWarranty));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryAddress));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryArea));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryProcess));
            node.Attributes.Append(doc.CreateAttribute(Tags.DateDelivered));
            node.Attributes.Append(doc.CreateAttribute(Tags.QuantityDiff));
            node.Attributes.Append(doc.CreateAttribute(Tags.ScheduledQuantity));
            node.Attributes.Append(doc.CreateAttribute(Tags.TaxAmount));
            node.Attributes.Append(doc.CreateAttribute(Tags.ContractNumber));
            node.Attributes.Append(doc.CreateAttribute(Tags.ReturnItemNo));
            node.Attributes.Append(doc.CreateAttribute(Tags.ReturnLocation));
            node.Attributes.Append(doc.CreateAttribute(Tags.FreeGift));
            node.Attributes.Append(doc.CreateAttribute(Tags.ExpectedReturnDate));
            node.Attributes.Append(doc.CreateAttribute(Tags.QtyOnOrder));
            node.Attributes.Append(doc.CreateAttribute(Tags.PurchaseOrder));
            node.Attributes.Append(doc.CreateAttribute(Tags.LeadTime));
            node.Attributes.Append(doc.CreateAttribute(Tags.Assembly));
            node.Attributes.Append(doc.CreateAttribute(Tags.Damaged));
            node.Attributes.Append(doc.CreateAttribute(Tags.ProductCategory));
            node.Attributes.Append(doc.CreateAttribute(Tags.SparePartsCategory)); // Required for selecting a spare part for a Service Request JH 08/11/2007
            node.Attributes.Append(doc.CreateAttribute(Tags.Deleted));
            node.Attributes.Append(doc.CreateAttribute(Tags.PurchaseOrderNumber));
            node.Attributes.Append(doc.CreateAttribute(Tags.ReplacementItem));
            node.Attributes.Append(doc.CreateAttribute(Tags.SPIFFItem));
            node.Attributes.Append(doc.CreateAttribute(Tags.RefCode)); //IP - 28/01/10 - LW 72136
            node.Attributes.Append(doc.CreateAttribute(Tags.ItemRejected)); //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            node.Attributes.Append(doc.CreateAttribute(Tags.Category));
            node.Attributes.Append(doc.CreateAttribute(Tags.ItemId));         //CR1212
            node.Attributes.Append(doc.CreateAttribute(Tags.ColourName));     //CR1212 jec 21/04/11
            node.Attributes.Append(doc.CreateAttribute(Tags.Style));          //CR1212 jec 21/04/11    
            node.Attributes.Append(doc.CreateAttribute(Tags.SalesBrnNo));   //IP - 23/05/11 - CR1212 - RI - #3651
            node.Attributes.Append(doc.CreateAttribute(Tags.RepoItem));     // RI
            node.Attributes.Append(doc.CreateAttribute(Tags.Class));        // 27/07/11 - RI - #4415
            node.Attributes.Append(doc.CreateAttribute(Tags.SubClass));     // 27/07/11 - RI - #4415
            node.Attributes.Append(doc.CreateAttribute(Tags.Brand));        //IP - 19/09/11 - RI - #8218 - CR8201
            node.Attributes.Append(doc.CreateAttribute(Tags.Express));      //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            node.Attributes.Append(doc.CreateAttribute(Tags.WarrantyType));  //#17883    // #16607 
            node.Attributes.Append(doc.CreateAttribute(Tags.ReadyAssist));  //#18604 - CR15594
            node.Attributes.Append(doc.CreateAttribute(Tags.IsAmortized));
            return node;
        }

        private XmlNode CreateRelatedItemsNode(XmlDocument doc)
        {
            return doc.CreateElement(Elements.RelatedItem);
        }

        public XmlNode GetLineItems(SqlConnection conn, SqlTransaction trans,
                                    string accountNumber, string accountType, string country,
                                    int agreementNo, int version = 0, bool reprint = false)
        {
            LineItemDocument items = new LineItemDocument();
            int versionno = version;
            DLineItem line = new DLineItem();
            DataTable dt = line.GetRootLineItemCodes(conn, trans, accountNumber, agreementNo, version);
            foreach (DataRow row in dt.Rows)
            {
                var itemId = Convert.ToInt32(row[CN.ItemId]);
                short location = (short)row["stocklocn"];
                string contractNo = (string)row[CN.ContractNo];                

                LineItemNode item = items.CreateLineItemNode();
                //if (itemNodeCounter < 100)
                if (reprint == true)
                {
                    PopulateItemNode_ReprintWin(conn, trans, accountNumber, agreementNo,
                                itemId, location, accountType, country, false,
                                contractNo, item, items, ParentItemId, versionno);
                }
                else
                {
                    PopulateItemNode(conn, trans, accountNumber, agreementNo,
                                itemId, location, accountType, country, false,
                                contractNo, item, items, ParentItemId, versionno);
                }

                //PopulateItemNode(conn, trans, accountNumber, agreementNo, 
                //			code, location, accountType, country, false, contractNo, item, items,null);				
            }
            if (!items.HasLineItems)
                return null;
            else
                return items.Document.RemoveChild(items.Document.DocumentElement);
        }


        //Suvidha CR 2018-13 to 
        public XmlNode GetSalesOrderLineItems(SqlConnection conn, SqlTransaction trans, string accountNumber,
                                    string accountType, string country,
                                    int agreementNo, string agreementInvNo)
        {
            LineItemDocument items = new LineItemDocument();
            DLineItem line = new DLineItem();
            DataTable dt = line.GetSalesOrderLineItemCodes(conn, trans, agreementNo, agreementInvNo);
            
            foreach (DataRow dr in dt.Rows)
            {
                short location = (short)dr["stocklocn"];
                string contractNo = Convert.ToString(dr[CN.ContractNo]);
                Int32 parentItemID = string.IsNullOrEmpty(Convert.ToString(dr[CN.ParentID])) == true ? 0 : Convert.ToInt32(dr[CN.ParentID]);
                LineItemNode item = items.CreateLineItemNode();
                PopulateItemNode_SalesOrder(conn, trans, accountNumber, agreementNo,
                            accountType, country, item, items, agreementInvNo, dr, parentItemID);                
            }
            
            if (!items.HasLineItems)
                return null;
            else
                return items.Document.RemoveChild(items.Document.DocumentElement);
        }

        //IP - 11/10/11 - #3921 - CR1232
        public XmlNode GetCashLoanItem(SqlConnection conn, SqlTransaction trans,
                                    string accountNumber, string accountType, string country,
                                    int agreementNo)
        {
            LineItemDocument items = new LineItemDocument();

            DLineItem line = new DLineItem();
            DataTable dt = line.GetCashLoanItem(conn, trans, accountNumber, agreementNo);
            foreach (DataRow row in dt.Rows)
            {
                var itemId = Convert.ToInt32(row[CN.ItemId]);
                short location = (short)row["stocklocn"];
                string contractNo = (string)row[CN.ContractNo];

                LineItemNode item = items.CreateLineItemNode();
                //if (itemNodeCounter < 100)
                    PopulateItemNode(conn, trans, accountNumber, agreementNo,
                                itemId, location, accountType, country, false,
                                contractNo, item, items, 0);
            }
            if (!items.HasLineItems)
                return null;
            else
                return items.Document.RemoveChild(items.Document.DocumentElement);
        }

        private void PopulateItemNode(SqlConnection conn, SqlTransaction trans,
                                        string accountNumber, int agreementNo,
                                        int itemId, short branch,
                                        string accountType, string country,
                                        bool parentIsKit, string contractNo,
                                        LineItemNode item,
                                        LineItemDocument items, int parentItemId, int version = 0)
        {
            string type = IT.Unknown;
            bool canAddWarranty = false;
            string lastDelivery = "";
            //itemNodeCounter++;
            //Get the stock item details for this item
            int versionno = version;
            DStockItem stock = new DStockItem();
            stock.PromoBranch = this.PromoBranch;
            stock.GetItemDetails(conn, trans, itemId, branch, accountType, country, false, false, accountNumber, agreementNo);
            DAccount account = new DAccount(null, null, accountNumber);
            account.GetAccount(accountNumber);
            
            //Get the line item details for this item
            DLineItem line = new DLineItem();
            line.AccountNumber = accountNumber;
            line.AgreementNumber = agreementNo;
            line.StockLocation = branch;
            line.ItemID = itemId;
            line.ContractNo = contractNo;
            line.ParentItemID = parentItemId;
            line.InvoiceVersion = version;
            line.IsAmortized = account.IsAmortized;    
            if (version > 0)
            {
                line.GetItemDetailsForReprint(conn, trans);
            }
            else
            {
                line.GetItemDetails(conn, trans);
            }
            

            //Check if this item can have a warranty added to it
            switch (line.CanAddWarranty(conn, trans))
            {
                case 0: canAddWarranty = true;
                    break;
                //IP - 19/08/08 - Commented out case 3, as in the stored procedure
                //'DN_WarrantyCanAddSP', if the result returned was '3' then you should
                //not be able to add a warranty to the item.
                //case 3:	canAddWarranty = true;	//2 means the item already has a 
                //break;						//warranty attached but it can 
                //still potentially be amended
                default: canAddWarranty = false;
                    break;
            }

            item.Key = line.ItemID + "|" + branch.ToString();

            if (stock.IsStock)
            {
                if (parentIsKit && line.IsComponent) // to do add option to check if free gift. Either we do this by checking whether exists in kit product 
                    //-- where the item is actually check against the parent or we say if discount category then potentially not component
                    // Question is it easier to get this from the lineitm or from the stock table.... 
                    // Well look at the lineitem table well we can checkk the parent item number and if parent is kit and item is componennt 
                    //then IT.component
                    type = IT.Component;
                else
                    type = IT.Stock;
            }
            if (stock.IsKit)
                type = IT.Kit;

            if (stock.IsDiscount)
            {
                if (parentIsKit)
                    type = IT.KitDiscount;
                else
                    type = IT.Discount;
                canAddWarranty = false;
            }
            if (stock.IsWarranty)
            {
                if (parentIsKit)
                    type = IT.KitWarranty;
                else
                    type = IT.Warranty;
                canAddWarranty = false;
            }
            if (stock.IsAffinity)
            {
                type = IT.Affinity;
                canAddWarranty = false;
            }

            if (stock.IsStampDuty)
            {
                type = IT.SundryCharge;
                canAddWarranty = false;
            }

            //IP - 24/02/11 - #3130
            if (stock.IsInstallation)
            {
                type = IT.Installation;
                canAddWarranty = false;
            }

            if (stock.IsAssemblyCost)
            {
                type = IT.AssemblyCost;
                canAddWarranty = false;
            }

            if (stock.IsAnnualServiceContract)
            {
                type = IT.AnnualService;
                canAddWarranty = false;
            }

            if (stock.IsGenericService)
            {
                type = IT.GenericService;
                canAddWarranty = false;
            }

            item.Type = type;
            item.Code = line.ItemNumber;
            item.ItemId = itemId;
            item.Location = branch;
            item.AvailableStock = Convert.ToDecimal(stock.AvailableStock);
            item.DamagedStock = Convert.ToDecimal(stock.DamagedStock);
            item.Description1 = stock.ProductDesc1;
            item.Description2 = stock.ProductDesc2;
            item.SupplierCode = stock.SupplierCode;
            item.UnitPrice = line.Price;
            item.CashPrice = Convert.ToDecimal(stock.CashPrice);
            item.CostPrice = Convert.ToDecimal(stock.CostPrice);
            item.HPPrice = Convert.ToDecimal(stock.HPPrice);
            item.DutyFreePrice = Convert.ToDecimal(stock.DutyFreePrice);
            item.ValueControlled = stock.ValueControlled;
            item.Quantity = Convert.ToDecimal(line.Quantity);
            item.Value = line.OrderValue;
            item.DeliveryDate = line.DateRequiredDelivery.ToString();
            item.DeliveryTime = line.TimeRequiredDelivery;
            item.BranchForDeliveryNote = line.DeliveryNoteBranch;
            item.ColourTrim = line.Notes;
            item.QtyOnOrder = Convert.ToDecimal(stock.QtyOnOrder);
            //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
            item.VanNo = line.VanNo;
            item.DhlPickingDate = line.DhlPickingDate;
            item.DhlInterfaceDate = line.DhlInterfaceDate;
            item.DhlDNNo = line.DhlDNNo;
            //item.OrigQty = line.OrigQty;
            item.ShipQty = line.ShipQty;
            item.ItemRejected = line.ItemRejected;          //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            item.ParentItemNo = line.ParentItemNumber;
            item.ParentItemId = parentItemId;
            item.SalesBrnNo = !line.SalesBrnNo.HasValue && line.ItemType.Trim() == "S" ? Convert.ToInt16(accountNumber.Substring(0, 3)) : line.SalesBrnNo; //IP - 23/05/11 - CR1212 - RI - #3651 - Either return existing Sales Branch if populated or first 3 digits of the account number ONLY for stockitems.
            item.RepoItem = line.RepoItem;              //RI jec 16/06/11
            item.Class = stock.Class;        //IP - 28/07/11 - RI - #4415  
            item.SubClass = stock.SubClass;  //IP - 28/07/11 - RI - #4415
            item.Brand = stock.Brand;        //IP - 19/09/11 - RI - #8218 - CR8201
            item.Style = stock.Style;        //IP - 20/09/11 - RI - #8218 - CR8201
            item.IsAmortized = line.IsAmortized;        
            //If it's a discount then it should take the tax rate of the thing it discounts
            //i.e. it's parent (unless it doesn't have one
            if (type == IT.KitDiscount)	//if it's a KitDiscount
            {
                try
                {
                    //TO DO item.TaxRate = item.Parent.TaxRate;
                    item.TaxRate = Convert.ToDecimal(item.Node.ParentNode.Attributes[Tags.TaxRate].Value);
                }
                catch (Exception)
                {
                    item.TaxRate = Convert.ToDecimal(stock.TaxRate);
                }
            }
            else
                item.TaxRate = Convert.ToDecimal(stock.TaxRate);

            item.AdditionalTaxRate = Convert.ToDecimal(stock.Additionaltaxrate); //BCX Change for Set Additional Tax
            item.DeliveredQuantity = Convert.ToDecimal(line.DeliveredQuantity);
            item.PlannedDeliveryDate = line.DatePlannedDelivery.ToString();
            item.CanAddWarranty = canAddWarranty;
            item.DeliveryAddress = line.DeliveryAddress;
            item.DeliveryArea = line.DeliveryArea;
            item.DeliveryProcess = line.DeliveryProcess;
            item.QuantityDiff = line.QuantityDiff;
            item.ScheduledQuantity = Convert.ToDecimal(line.ScheduledQuantity);
            item.TaxAmount = Convert.ToDecimal(line.TaxAmount);
            item.ContractNo = line.ContractNo;
            item.ReturnItemNo = line.ItemNumber;
            item.ReturnLocation = branch;
            item.FreeGift = stock.IsFreeGift;
            item.ExpectedReturnDate = line.ExpectedReturnDate.ToString();
            item.PurchaseOrder = false;
            item.LeadTime = stock.LeadTime;
            item.Damaged = line.Damaged;
            item.Assembly = line.Assembly;
            item.ProductCategory = stock.ProductCategory;
            item.Deleted = stock.Deleted;
            item.PurchaseOrderNumber = "";
            item.ReplacementItem = false;
            item.SPIFFItem = line.SPIFFItem;
            item.IsInsurance = (line.ItemNumber == (string)Country[CountryParameterNames.InsuranceChargeItem]);
            item.RefCode = stock.RefCode; //IP - 29/01/10 - LW 72136

            item.Category = stock.Category;
            item.LineItemId = line.LineItemId;    //#13716 - CR12949
            item.ReadyAssist = stock.ReadyAssist; //#13716 - CR12949
            item.WarrantyType = stock.WarrantyType;     //#17883 //#15888
             

            if (!Convert.IsDBNull(line.DateOfLastDelivery))
                lastDelivery = line.DateOfLastDelivery.ToString();
            item.DateDelivered = lastDelivery;

            item.Express = line.Express;        //IP - 07/06/12 - #10229 - Warehouse & Deliveries


            //Get any child items for this item
            DataTable dt = line.GetChildLineItemCodes(conn, trans, accountNumber, agreementNo, line.ItemID, branch);
            foreach (DataRow row in dt.Rows)
            {
                LineItemNode li = new LineItemNode(item.Document);
                item.AddRelatedItem(li);
                //itemNodeCounter++;

                //if (itemNodeCounter < 100)
                    PopulateItemNode(conn, trans, accountNumber, agreementNo, Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row["stocklocn"]),
                                accountType, country, stock.IsKit, (string)row[CN.ContractNo],
                                li, items, line.ItemID, version);
            }

            if (line.ItemType == "S")   //#17407
            {
                GetServiceInfo(accountNumber, line.ItemNumber, line.StockLocation.ToString(), item);
            }
           
        }

        private void PopulateItemNode_SalesOrder(SqlConnection conn, SqlTransaction trans
                                        , string accountNumber, int agreementNo,                                        
                                        string accountType, string country,                                        
                                        LineItemNode item,
                                        LineItemDocument items, string agreementInvNo, DataRow drSalesOrderItemRow,int parentItemId)
        {
            string type = IT.Unknown;
            bool canAddWarranty = false;
            string lastDelivery = "";
            //itemNodeCounter++;
            //Get the stock item details for this item
            //int versionno = version;

            //DStockItem stock = new DStockItem();
            //stock.PromoBranch = this.PromoBranch;
            //stock.GetItemDetails(conn, trans, itemId, branch, accountType, country, false, false, accountNumber, agreementNo);

            //Get the line item details for this item
            DLineItem line = new DLineItem();
            DAccount account = new DAccount(null, null, accountNumber);
            //line.AccountNumber = accountNumber;
            line.AgreementNumber = agreementNo;
         
            //line.StockLocation = 761;
            //line.ItemID = Convert.ToInt32(drSalesOrderItemRow["ItemID"]);
            line.ContractNo = Convert.ToString(drSalesOrderItemRow["ContractNo"]);
            line.IsAmortized = account.IsAmortized;   // Added Amortized value for laser printer
            //line.ParentItemID = parentItemId;

            //line.InvoiceVersion = version;

            //if (version > 0)
            //{
            //    line.GetItemDetailsForReprint(conn, trans);
            //}
            //else
            //{
            //    line.GetItemDetails(conn, trans);
            //}


            //Check if this item can have a warranty added to it
            switch (line.CanAddWarranty(conn, trans))
            {
                case 0:
                    canAddWarranty = true;
                    break;
                //IP - 19/08/08 - Commented out case 3, as in the stored procedure
                //'DN_WarrantyCanAddSP', if the result returned was '3' then you should
                //not be able to add a warranty to the item.
                //case 3:	canAddWarranty = true;	//2 means the item already has a 
                //break;						//warranty attached but it can 
                //still potentially be amended
                default:
                    canAddWarranty = false;
                    break;
            }

            //item.Key = line.ItemID + "|" + branch.ToString();

            //if (stock.IsStock)
            //{
            //    if (parentIsKit && line.IsComponent) // to do add option to check if free gift. Either we do this by checking whether exists in kit product 
            //        //-- where the item is actually check against the parent or we say if discount category then potentially not component
            //        // Question is it easier to get this from the lineitm or from the stock table.... 
            //        // Well look at the lineitem table well we can checkk the parent item number and if parent is kit and item is componennt 
            //        //then IT.component
            //        type = IT.Component;
            //    else
            //        type = IT.Stock;
            //}
            //if (stock.IsKit)
            //    type = IT.Kit;

            //if (stock.IsDiscount)
            //{
            //    if (parentIsKit)
            //        type = IT.KitDiscount;
            //    else
            //        type = IT.Discount;
            //    canAddWarranty = false;
            //}
            //if (stock.IsWarranty)
            //{
            //    if (parentIsKit)
            //        type = IT.KitWarranty;
            //    else
            //        type = IT.Warranty;
            //    canAddWarranty = false;
            //}
            //if (stock.IsAffinity)
            //{
            //    type = IT.Affinity;
            //    canAddWarranty = false;
            //}

            //if (stock.IsStampDuty)
            //{
            //    type = IT.SundryCharge;
            //    canAddWarranty = false;
            //}

            ////IP - 24/02/11 - #3130
            //if (stock.IsInstallation)
            //{
            //    type = IT.Installation;
            //    canAddWarranty = false;
            //}

            //if (stock.IsAssemblyCost)
            //{
            //    type = IT.AssemblyCost;
            //    canAddWarranty = false;
            //}

            //if (stock.IsAnnualServiceContract)
            //{
            //    type = IT.AnnualService;
            //    canAddWarranty = false;
            //}

            //if (stock.IsGenericService)
            //{
            //    type = IT.GenericService;
            //    canAddWarranty = false;
            //}
            item.Type = type;
            item.Code = Convert.ToString(drSalesOrderItemRow["ItemNo"]);// line.ItemNumber;
            item.ItemId = Convert.ToInt32(drSalesOrderItemRow["ItemID"]);
            item.Location = Convert.ToInt16(drSalesOrderItemRow["stocklocn"]);
            //item.AvailableStock = Convert.ToDecimal(stock.AvailableStock);
            //item.DamagedStock = Convert.ToDecimal(stock.DamagedStock);
            item.Description1 = Convert.ToString(drSalesOrderItemRow["Description"]);
            item.Description2 = "";// stock.ProductDesc2;
            item.SupplierCode = "";// stock.SupplierCode;
            item.UnitPrice = Convert.ToDecimal(drSalesOrderItemRow["Price"]);
            item.CashPrice = Convert.ToDecimal(drSalesOrderItemRow["Price"]);
            item.CostPrice = Convert.ToDecimal(drSalesOrderItemRow["Price"]);
            item.HPPrice = Convert.ToDecimal(drSalesOrderItemRow["Price"]);
            item.DutyFreePrice = 0;
            //item.ValueControlled = stock.ValueControlled;
            //item.Quantity = Convert.ToDecimal(line.Quantity);
            item.Quantity = Convert.ToDecimal(drSalesOrderItemRow["Quantity"]);
            item.Value = Convert.ToDecimal(drSalesOrderItemRow["OrderVal"]); //line.OrderValue;
            item.DeliveryDate = line.DateRequiredDelivery.ToString();
            item.DeliveryTime = line.TimeRequiredDelivery;
            item.BranchForDeliveryNote = line.DeliveryNoteBranch;
            item.ColourTrim = line.Notes;
            item.QtyOnOrder = Convert.ToDecimal(drSalesOrderItemRow["Quantity"]);
            //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
            item.VanNo = line.VanNo;
            item.DhlPickingDate = line.DhlPickingDate;
            item.DhlInterfaceDate = line.DhlInterfaceDate;
            item.DhlDNNo = line.DhlDNNo;
            //item.OrigQty = line.OrigQty;
            item.ShipQty = line.ShipQty;
            item.ItemRejected = line.ItemRejected;          //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            item.ParentItemNo = line.ParentItemNumber;
            item.ParentItemId = parentItemId;
            //item.SalesBrnNo = !line.SalesBrnNo.HasValue && line.ItemType.Trim() == "S" ? Convert.ToInt16(accountNumber.Substring(0, 3)) : line.SalesBrnNo; //IP - 23/05/11 - CR1212 - RI - #3651 - Either return existing Sales Branch if populated or first 3 digits of the account number ONLY for stockitems.
            item.RepoItem = line.RepoItem;              //RI jec 16/06/11
            //item.Class = stock.Class;        //IP - 28/07/11 - RI - #4415  
            //item.SubClass = stock.SubClass;  //IP - 28/07/11 - RI - #4415
            //item.Brand = stock.Brand;        //IP - 19/09/11 - RI - #8218 - CR8201
            //item.Style = stock.Style;        //IP - 20/09/11 - RI - #8218 - CR8201

            //If it's a discount then it should take the tax rate of the thing it discounts
            //i.e. it's parent (unless it doesn't have one
            

            item.TaxRate = Convert.ToDecimal(drSalesOrderItemRow["TaxRate"]);
            
            item.AdditionalTaxRate = Convert.ToDecimal(drSalesOrderItemRow["AdditionalTaxRate"]); //BCX Change for Set Additional Tax
            item.DeliveredQuantity = Convert.ToDecimal(line.DeliveredQuantity);
            item.PlannedDeliveryDate = line.DatePlannedDelivery.ToString();
            item.CanAddWarranty = canAddWarranty;
            //item.DeliveryAddress = line.DeliveryAddress;
            //item.DeliveryArea = line.DeliveryArea;
            //item.DeliveryProcess = line.DeliveryProcess;
            //item.QuantityDiff = line.QuantityDiff;
            item.ScheduledQuantity = Convert.ToDecimal(line.ScheduledQuantity);
            item.TaxAmount = Convert.ToDecimal(line.TaxAmount);
            item.ContractNo = line.ContractNo;
            item.ReturnItemNo = line.ItemNumber;
            //item.ReturnLocation = branch;
            item.FreeGift = false;// stock.IsFreeGift;
            item.ExpectedReturnDate = line.ExpectedReturnDate.ToString();
            item.PurchaseOrder = false;
            //item.LeadTime = stock.LeadTime;
            item.Damaged = line.Damaged;
            item.Assembly = line.Assembly;
            //item.ProductCategory = stock.ProductCategory;
            //item.Deleted = stock.Deleted;
            item.PurchaseOrderNumber = "";
            item.ReplacementItem = false;
            item.SPIFFItem = line.SPIFFItem;
            item.IsInsurance = (line.ItemNumber == (string)Country[CountryParameterNames.InsuranceChargeItem]);
            //item.RefCode = stock.RefCode; //IP - 29/01/10 - LW 72136

            //item.Category = stock.Category;
            item.LineItemId = line.LineItemId;    //#13716 - CR12949
            //item.ReadyAssist = stock.ReadyAssist; //#13716 - CR12949
            //item.WarrantyType = stock.WarrantyType;     //#17883 //#15888


            if (!Convert.IsDBNull(line.DateOfLastDelivery))
                lastDelivery = line.DateOfLastDelivery.ToString();
            item.DateDelivered = lastDelivery;

            item.Express = line.Express;        //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            item.IsAmortized = line.IsAmortized;  // Added Amortized value for laser printer case.


            ////Get any child items for this item
            //DataTable dt = line.GetChildLineItemCodes(conn, trans, accountNumber, agreementNo, line.ItemID, branch);
            //foreach (DataRow row in dt.Rows)
            //{
            //    LineItemNode li = new LineItemNode(item.Document);
            //    item.AddRelatedItem(li);
            //    //itemNodeCounter++;

            //    //if (itemNodeCounter < 100)
            //    PopulateItemNode(conn, trans, accountNumber, agreementNo, Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row["stocklocn"]),
            //                accountType, country, stock.IsKit, (string)row[CN.ContractNo],
            //                li, items, line.ItemID, version);
            //}

            //if (line.ItemType == "S")   //#17407
            //{
            //    GetServiceInfo(accountNumber, line.ItemNumber, line.StockLocation.ToString(), item);
            //}

        }

        private void GetServiceInfo(string acctno, string itemcode, string stocklocn, LineItemNode item)
        {
            var dservice = new DServiceRequest();
            var serviceinfo = dservice.ServiceGetItemPrintInfo(acctno, itemcode, stocklocn);



            foreach (var serviceitem in serviceinfo)
            {
                var node = item.Document.CreateElement("SERVICE");
                var serialattrib = item.Document.CreateAttribute("SERIALNO");
                var modelattrib = item.Document.CreateAttribute("MODELNO");
                serialattrib.Value = serviceitem.serialno;
                modelattrib.Value = serviceitem.modelno;
                node.Attributes.Append(serialattrib);
                node.Attributes.Append(modelattrib);
                item.Document.SelectSingleNode(("//Item[@Code = '" + itemcode + "' and @Location = '" + stocklocn + "']")).AppendChild(node);
            }

        }
        public void GetItemsDeliveredAndScheduled(string accountNo,
            int agreementNo,
            int itemID,                                                     //IP - 17/05/11 - CR1212 - #3627 - changed to use itemID rather than itemNo
            short location,
            string contractNo,
            out double delivered,
            out double scheduled, int parentItemID, out bool repo)          //IP - 26/06/12 - #10516 //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID rather than parentItemNo
        {
            DSchedule schedule = new DSchedule();
            schedule.GetScheduledQuantity(accountNo, agreementNo, itemID, location);
            scheduled = schedule.ScheduledQuantity;

            DDelivery delivery = new DDelivery();
            delivery.GetDeliveredQuantity(null, null, accountNo, agreementNo, itemID, location, contractNo, parentItemID);
            delivered = delivery.DeliveredQuantity;

            repo = new AccountRepository().CheckForRepo(null, null, accountNo, agreementNo, location, itemID);
        }

        public DataSet GetScheduledDeliveriesForItem(string accountNo,
            int agreementNo,
            //string itemNo,   
            int itemID,                                                       //IP/NM - 18/05/11 -CR1212 - #3627 
            short location)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();
            sched.GetScheduledDeliveriesForItem(accountNo, agreementNo, itemID, location);      //IP/NM - 18/05/11 -CR1212 - #3627 
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public void DeleteLineItem(SqlConnection conn, SqlTransaction trans,
                                    string accountNo, int agreementNo,
                                    int itemID, short branchNo)             //IP/NM - 18/05/11 -CR1212 - #3627 
        {
            DLineItem item = new DLineItem();
            item.DeleteLineItem(conn, trans, accountNo,
                agreementNo, itemID, branchNo);
        }

        public void UpdateItemValue(SqlConnection conn, SqlTransaction trans,
            string accountNo, int agreementNo, int itemID, short branchNo, decimal newValue)    //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID rather than itemNo
        {
            DLineItem item = new DLineItem();
            item.UpdateItemValue(conn, trans, accountNo,
                                agreementNo, itemID, branchNo, newValue);
        }

        public void UpdateItemQuantity(SqlConnection conn, SqlTransaction trans,
            string accountNo, int agreementNo, int itemID,                                  //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo
            short branchNo, string contractNo, decimal newValue, int parentItemID)
        {
            DLineItem item = new DLineItem();
            item.UpdateItemQuantity(conn, trans, accountNo,
                agreementNo, itemID, branchNo, contractNo, newValue, parentItemID);
        }

        public void DeleteLineItem(SqlConnection conn, SqlTransaction trans,
            string accountNo,
            int agreementNo,
            XmlNode toDelete)
        {
            DLineItem item = new DLineItem();
            item.DeleteLineItem(conn, trans, accountNo,
                agreementNo, Convert.ToInt32(toDelete.Attributes[Tags.ItemId].Value),                        //IP/NM - 18/05/11 -CR1212 - #3627       
                Convert.ToInt16(toDelete.Attributes[Tags.Location].Value));
            foreach (XmlNode child in toDelete.ChildNodes)
            {
                //find the related items tag
                if (child.NodeType == XmlNodeType.Element && child.Name == Elements.RelatedItem)
                {
                    //loop through related items
                    foreach (XmlNode related in child.ChildNodes)
                    {
                        if (related.NodeType == XmlNodeType.Element && related.Name == Tags.Item)
                        {
                            //Delete each one
                            DeleteLineItem(conn, trans, accountNo, agreementNo, related);
                        }
                    }
                }
            }
        }

        public void DeleteAllLineItems(SqlConnection conn, SqlTransaction trans,
            string accountNo,
            int agreementNo)
        {
            DLineItem item = new DLineItem();
            item.DeleteAllLineItems(conn, trans, accountNo, agreementNo);
        }

        public double GetItemQuantity(SqlConnection conn, SqlTransaction trans,
                                        string accountNo, int agreementNo, short branchNo,
                                        int itemId, int parentItemId, string contractNo, bool current)      // #9854 jec 30/03/12
        {
            DLineItem l = new DLineItem();
            l.AccountNumber = accountNo;
            l.StockLocation = branchNo;
            l.ItemID = itemId;
            l.ParentItemID = parentItemId;              // #9854 jec 30/03/12
            l.ContractNo = contractNo;
            l.AgreementNumber = agreementNo;
            l.GetItemQuantity(conn, trans, current);
            return l.Quantity;
        }

        public decimal GetItemValue(SqlConnection conn, SqlTransaction trans,
                                    string accountNo, int agreementNo, short branchNo,
                                    int itemID, string contractNo,                      //IP/NM - 18/05/11 -CR1212 - #3627 
                                    bool current)
        {
            DLineItem l = new DLineItem();
            l.AccountNumber = accountNo;
            l.StockLocation = branchNo;
            //l.ItemNumber = itemNo;
            l.ItemID = itemID;                  //IP/NM - 18/05/11 -CR1212 - #3627 
            l.ContractNo = contractNo;
            l.AgreementNumber = agreementNo;
            l.ParentItemID = this.ParentItemId;  //#18087
            l.GetItemValue(conn, trans, current);
            return l.Value;
        }
        /// <summary>
        /// removes details from the lineitem_amend table
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="accountNumber"></param>
        /// <param name="agreementNo"></param>
        public void DropTempLineItems(SqlConnection conn, SqlTransaction trans,
                                    string accountNumber, int agreementNo)
        {
            DLineItem l = new DLineItem();
            l.DropTempLineItems(conn, trans, accountNumber, agreementNo);
        }

        public void SaveKit(SqlConnection conn, SqlTransaction trans, XmlNode kitNode)
        {
            DKitLineItem kit = new DKitLineItem();
            DComponentLineItem comp = new DComponentLineItem();

            comp.AccountNo = kit.AccountNo = this.AccountNumber;
            comp.KitNo = kit.KitNo = this.ItemNumber;
            comp.StockLocation = kit.StockLocation = this.StockLocation;
            kit.Quantity = this.Quantity;

            XmlNode related = kitNode.SelectSingleNode(Elements.RelatedItem);
            foreach (XmlNode component in related.ChildNodes)
            {
                comp.ComponentNo = component.Attributes[Tags.Code].Value;

                comp.Price = Convert.ToDouble(component.Attributes[Tags.UnitPrice].Value);
                //IP/JC - 04/12/09 - UAT5.2 (932)
                //comp.Quantity = Convert.ToDouble(component.Attributes[Tags.Quantity].Value) / kit.Quantity;
                if (kit.Quantity != 0)
                {
                    comp.Quantity = Convert.ToDouble(component.Attributes[Tags.Quantity].Value) / kit.Quantity;
                }
                else
                {
                    comp.Quantity = 0;
                }
                comp.OrderValue = comp.Price * comp.Quantity;
                comp.Save(conn, trans);
                if (component.Attributes[Tags.Code].Value == "DS")
                {
                    kit.DiscountPrice = Convert.ToDouble(component.Attributes[Tags.UnitPrice].Value);
                    kit.DiscountOrderValue = kit.DiscountPrice * kit.Quantity;
                    kit.Save(conn, trans);
                }
            }
        }

        /// <summary>
        /// returns a dataset containing stock item translations
        /// </summary>
        /// <returns></returns>
        public DataSet GetStockItemTranslations(SqlConnection conn, SqlTransaction trans,
                                                string itemno, string descr1_en, string descr1,
                                                string descr2_en, string descr2)
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.GetStockItemTranslations(conn, trans, itemno, descr1_en, descr1, descr2_en, descr2);      // TO DO use itemid
            ds.Tables.Add(stock.Translations);
            return ds;
        }

        public void SaveStockItemTranslations(SqlConnection conn, SqlTransaction trans, DataSet changes)
        {
            DStockItem si = new DStockItem();
            //string itemno = "";
            string descr1 = "";
            string descr2 = "";

            foreach (DataRow row in changes.Tables[0].Rows)
            {
                descr1 = descr2 = "";

                if (row[CN.descr1] != DBNull.Value)
                    descr1 = (string)row[CN.descr1];

                if (row[CN.descr2] != DBNull.Value)
                    descr2 = (string)row[CN.descr2];

                si.SetStockItemTranslation(conn, trans, (string)row[CN.ItemNo], descr1, descr2);
            }
        }

        public void SaveKitPT(SqlConnection conn, SqlTransaction trans, XmlNode kit)
        {
            DKitLineItem k = new DKitLineItem();
            DComponentLineItem c = new DComponentLineItem();

            c.AccountNo = k.AccountNo = this.AccountNumber;
            c.KitNo = k.KitNo = this.ItemNumber;
            c.StockLocation = k.StockLocation = this.StockLocation;
            k.Quantity = this.Quantity;

            XmlNode related = kit.SelectSingleNode(Elements.RelatedItem);
            foreach (XmlNode component in related.ChildNodes)
            {
                c.ComponentNo = component.Attributes[Tags.Code].Value;
                c.Price = Convert.ToDouble(component.Attributes[Tags.UnitPrice].Value);
                c.Quantity = Convert.ToDouble(component.Attributes[Tags.Quantity].Value) / k.Quantity;
                c.OrderValue = c.Price * c.Quantity;
                c.SavePT(conn, trans);
                if (component.Attributes[Tags.Code].Value == "DS")
                {
                    k.DiscountPrice = Convert.ToDouble(component.Attributes[Tags.UnitPrice].Value);
                    k.DiscountOrderValue = k.DiscountPrice * k.Quantity;
                    k.SavePT(conn, trans);
                }
            }
        }

        public DataSet GetSingleItem(int locn, int itemID, string acctno, int agreementNo, string contractNo, int parentItemID) //IP - 17/05/11 - CR1212 - #3627 - Changed itemNo and parentItemNo to itemID and parentItemID
        {
            DataSet ds = new DataSet();
            DLineItem item = new DLineItem();
            item.GetSingleItem(null, null, locn, itemID, acctno, agreementNo, contractNo, parentItemID);        //IP - 17/05/11 - CR1212 - #3627
            ds.Tables.Add(item.ItemDetails);
            return ds;
        }

        public DataSet GetItemsForAccount(string acctno)
        {
            DataSet ds = new DataSet();
            DLineItem item = new DLineItem();
            item.AccountNumber = acctno;
            item.GetItemsForAccount();
            ds.Tables.Add(item.ItemDetails);
            return ds;
        }

        public void SaveItems(SqlConnection conn, SqlTransaction trans, DataSet ds, string type, double delQty)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DLineItem line = new DLineItem();
                line.OrigBr = 0;
                line.AccountNumber = (string)row["AcctNo"];
                line.AgreementNumber = Convert.ToInt32(row["AgrmtNo"]);
                line.ItemNumber = (string)row["ItemNo"];
                line.ItemSuppText = (string)row["ItemSuppText"];
                line.Quantity = Convert.ToDouble(row["Quantity"]);
                line.DeliveredQuantity = delQty;
                line.StockLocation = (short)Convert.ToInt32(row["StockLocn"]);
                line.Price = Convert.ToDecimal(row["Price"]);
                line.OrderValue = Convert.ToDecimal(row["OrdVal"]);
                line.DateRequiredDelivery = (DateTime)row["DateReqDel"];
                line.TimeRequiredDelivery = (string)row["TimeReqDel"];
                line.DatePlannedDelivery = (DateTime)row["DatePlanDel"];
                line.QuantityDiff = (string)row["Qtydiff"];
                line.ItemType = (string)row["ItemType"];
                line.TaxAmount = Convert.ToDouble(row["TaxAmt"]);
                line.Save(conn, trans);
            }
        }

        public int GetItemCount(int itemId, short location)             // RI jec 19/05/11
        {
            DStockItem stock = new DStockItem();
            int rowCount = stock.GetItemCount(itemId, location);        // RI jec 19/05/11
            return rowCount;
        }

        public void GetRealDiscount(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DAgreement agreement = new DAgreement(conn, trans, accountNo, 1);
            DLineItem lineItem = new DLineItem();

            lineItem.GetRealDiscount(conn, trans, accountNo);

            // Calc discount
            this._realDiscount = Math.Abs(lineItem.realDiscount);

            if (Math.Abs(agreement.Discount) > this._realDiscount)
                this._realDiscount = Math.Abs(agreement.Discount);
        }

        public decimal CalculateTaxAmount(decimal unitPrice,
                                            bool discount, decimal taxrate,
                                            decimal quantity, int itemId,
                                            bool taxExempt, decimal oldTaxamt)
        {
            decimal taxamt = 0;

            //IP - 21/08/08 - UAT5.1 - UAT(526)- I am excluding tax being calculated on 
            // the kit item (parent)as this was incorrectly calculating tax on the whole 
            // kit and posting this as a delivery when a Goods Return was processed.
            //if (!taxExempt &&
            //    itemNo != "STAX")
            if (!taxExempt &&
                itemId != StockItemCache.Get(StockItemKeys.STAX) &&
                IsKit != 1)
            {
                decimal ordval = quantity * unitPrice;

                if (discount && itemId != StockItemCache.Get(StockItemKeys.Tier2DiscountItemNumber))
                {
                    if (quantity > 0)
                    {
                        /* if we're saving the account when this is called
                         * discount items will already have had their tax
                         * calculated and so it mustn't be done again */
                        if (oldTaxamt == 0)
                            taxamt = (ordval * taxrate) / (100 + taxrate);
                        else
                            taxamt = oldTaxamt;
                    }
                    else
                        taxamt = 0;
                }
                else
                {
                    //order values held tax exclusive
                    //if(country.Parms.TaxType == "I" &&
                    //	country.Parms.AgreementTaxType == "E")
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        taxamt = (ordval * taxrate) / 100;
                    }

                    //order values held tax inclusive
                    //if(country.Parms.TaxType == "E" &&
                    //	country.Parms.AgreementTaxType == "I")
                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                    {
                        taxamt = (ordval * taxrate) / (100 + taxrate);
                    }
                }
            }

            CountryRound(ref taxamt);
            this.TaxAmount = Convert.ToDouble(taxamt);
            return taxamt;
        }

        public decimal CalculateTaxAmount(XmlNode item, bool taxExempt)
        {
            decimal taxamt = 0;
            decimal unitPrice = Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value);
            bool type = Convert.ToBoolean(item.Attributes[Tags.Type].Value == "Discount");
            decimal taxRate = Convert.ToDecimal(item.Attributes[Tags.TaxRate].Value);
            decimal quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
            int itemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
            decimal taxAmount = Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);

            taxamt = CalculateTaxAmount(unitPrice, type, taxRate,
                quantity, itemId, taxExempt, taxAmount);

            return taxamt;
        }

        public DataSet GetStockLocations(ref int itemId, ref string deletedItem, bool includeWarranties)        // RI jec 30/06/11
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.GetStockLocations(ref itemId, includeWarranties);     // RI
            deletedItem = stock.Deleted;
            ds.Tables.Add(stock.StockLocations);
            return ds;
        }

        public DataSet GetWarrantyContractDetails(string accountNo, int agreementNo, string contractNo)
        {
            DataSet ds = new DataSet();
            DLineWarranty lw = new DLineWarranty();
            ds.Tables.Add(lw.GetWarrantyContractDetails(accountNo, agreementNo, contractNo));
            return ds;
        }

        public void MaintainStockLevel(SqlConnection conn, SqlTransaction trans)
        {
            DStockItem stock = new DStockItem();
            stock.MaintainStockLevel(conn, trans, this.AccountNumber, this.ItemId, this.StockLocation, this.Quantity, this.AgreementNumber); //IP - 20/05/11 - CR1212 - RI - #3664
        }

        public bool ContractNoUnique(SqlConnection conn, SqlTransaction trans,
                                    string accountNo, int agreementNo, string contractNo)
        {
            DLineItem lw = new DLineItem();
            return lw.ContractNoUnique(conn, trans, accountNo, agreementNo, contractNo);
        }

        public bool AffinityContractNoUnique(SqlConnection conn, SqlTransaction trans,
            string accountNo, int agreementNo, string contractNo)
        {
            DLineItem lw = new DLineItem();
            return lw.AffinityContractNoUnique(conn, trans, accountNo, agreementNo, contractNo);
        }

        public BItem()
        {

        }

        public DataSet GetAssociatedLineItemWarranties(string accountNo, int itemId, short stockLocn)       // RI
        {
            DataSet ds = new DataSet();
            DLineWarranty lw = new DLineWarranty();
            DLineItem li = new DLineItem();

            ds.Tables.AddRange(new DataTable[] {lw.GetAssociatedWarranties(null, null, accountNo, itemId, stockLocn),       // RI
												li.GetAssociatedDiscounts(accountNo, itemId, stockLocn)});

            return ds;
        }

        public int SaveProductFaults(SqlConnection conn, SqlTransaction trans,
                                        string accountNo, int agreementNo,
                                        string itemNo, string returnItemNo, string notes,
                                        string reason, DateTime dateCollection,
                                        short elapsedMonths, short branchNo)
        {
            int newBuffNo = 0;
            DBranch branch = new DBranch();
            newBuffNo = branch.GetBuffNo(conn, trans, branchNo);

            DProductFault pf = new DProductFault();
            pf.Save(conn, trans, accountNo, agreementNo,
                    itemNo, returnItemNo, notes, reason,
                    dateCollection, elapsedMonths, newBuffNo);

            return newBuffNo;
        }

        public string GetWarrantyReturnItem(SqlConnection conn, SqlTransaction trans, int elapsedMonths)
        {
            return Services.GetService(STL.Common.Services.Services.ServiceTypes.CosacsWeb).GetWarrantyReturn(this.ItemNumber, (int)this.SalesBrnNo, elapsedMonths).Warranty.Number;
            //DWarrantyReturnCode w = new DWarrantyReturnCode();
            //return w.GetWarrantyReturnItem(conn, trans, elapsedMonths);

        }

        public void UpdateDelQty(SqlConnection conn, SqlTransaction trans,
                                    string accountNo, int agreementNo,
                                    short branchNo, int itemID,                                 //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than  //IP - 17/05/11 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo
                                    string contractNo, decimal quantity, int parentItemID)
        {
            DLineItem li = new DLineItem();
            li.UpdateDelQty(conn, trans, accountNo, branchNo, agreementNo, itemID, contractNo, Convert.ToDouble(quantity), parentItemID);   //IP - 17/05/11 - CR1212 - #3627
        }

        /// <summary>
        /// IsItemInstantReplacement
        /// </summary>
        /// <param name="itemno">string</param>
        /// <param name="branchno">int</param>
        /// <param name="instant">int</param>
        /// <returns>bool</returns>
        /// 
        public bool IsItemInstantReplacement(SqlConnection conn, SqlTransaction trans, int itemId, int branchno)
        {
            DStockItem da = new DStockItem();
            return da.IsItemInstantReplacement(conn, trans, itemId, branchno);
        }

        public string[] GetContractNos(short branchNo, int number)
        {
            string[] contracts = new string[number];
            DLineWarranty lw = new DLineWarranty();

            int last = lw.GetContractNos(branchNo, number);

            for (int i = number - 1; i >= 0; i--)
            {
                contracts[i] = branchNo.ToString() + (last--).ToString().PadLeft(5, '0');
            }

            return contracts;
        }

        public void UpdateTaxAmount(SqlConnection conn, SqlTransaction trans,
            string acctno, int itemID,//string itemno,                              //IP - 18/05/11 -CR1212 - #3627 - use itemID rather than itemno
            short branchno, decimal taxamount)
        {
            DLineItem li = new DLineItem();
            li.AgreementNumber = this.AgreementNumber;
            li.AuditSource = this.AuditSource;
            li.User = this.User;
            li.UpdateTaxAmount(conn, trans, acctno, itemID, branchno, taxamount);           //IP/NM - 18/05/11 -CR1212 - #3627
        }

        public void UpdateItemLocation(SqlConnection conn, SqlTransaction trans, DataSet ds, bool newDelNote)
        {
            DLineItem li = new DLineItem();
            DSchedule sched = new DSchedule();
            DBranch b = new DBranch();
            this.AuditSource = AS.ChangeOrder; //RI

            int newBuffNo = 0;

            foreach (DataRow row in ds.Tables["TN.Items"].Rows)
            {
                if (newDelNote)
                    newBuffNo = b.GetBuffNo(conn, trans, Convert.ToInt16((short)row[CN.StockLocn]));

                li.AccountNumber = (string)row[CN.AcctNo];
                li.AgreementNumber = (int)row[CN.AgrmtNo];
                li.ItemNumber = (string)row[CN.ItemNo];
                li.ItemID = Convert.ToInt32(row[CN.ItemId]);
                li.StockLocation = (short)row[CN.StockLocn];
                li.DeliveryNoteBranch = (short)row[CN.DelNoteBranch];
                li.DeliveryAddress = (string)row[CN.DeliveryAddress];
                li.DateRequiredDelivery = (DateTime)row[CN.DateReqDel];
                li.TimeRequiredDelivery = (string)row[CN.TimeReqDel];
                li.DeliveryArea = (string)row[CN.DeliveryArea];
                li.DeliveryProcess = (string)row[CN.DeliveryProcess];
                li.Notes = (string)row[CN.Notes];
                li.Damaged = (string)row[CN.Damaged];
                li.BuffNo = (int)row[CN.BuffNo];
                li.Assembly = (string)row[CN.AssemblyRequired];
                li.Express = (string)row[CN.Express];                       //IP - 07/06/12 - #10229 - Warehouse & Deliveries
                short origLocation = (short)row[CN.BranchNo];
                li.User = this.User;
                li.ParentItemID = (int)row[CN.ParentItemId];

                li.UpdateItemLocation(conn, trans, newBuffNo, origLocation);

                this.AccountNumber = li.AccountNumber;
                this.AgreementNumber = li.AgreementNumber;
                this.ItemNumber = li.ItemNumber;
                this.ItemId = li.ItemID;
                this.StockLocation = li.StockLocation;
                // Mauritius error where cancellations not going through 
                // to FACT
                UpdateLineItemAuditLocation(conn, trans, li.Quantity,
                                            li.OrderValue, TaxAmount, origLocation);

                // #10230 Cancelation to Warehouse only for Scheduled
                if ((string)row["AwaitDA"] != "Y")  //not waitingDA
                {
                    var AcctR = new AccountRepository();
                    var cancelations = AcctR.GetCancelData(conn, trans, (int)row[CN.ID], this.User, "Cancellation - details changed");

                    var dt = cancelations.ToDataTable(); 
                    var cancelationId = Convert.ToInt32(dt.Rows[0]["Id"]);

                    new Chub().CancelMany(cancelations, conn, trans);

                    var lineItemBooking = AcctR.GetLineitem(conn, trans, this.AccountNumber, this.ItemId, this.StockLocation);

                    //we need to insert a new LineItemBookingSchedule record for the new booking
                    new WarehouseRepository().InsertLineItemBookingSchedule(conn, trans, Convert.ToInt32(lineItemBooking.Rows[0][CN.ID]), "D", 0,
                                                                                           0, 0, 0, Convert.ToSingle(lineItemBooking.Rows[0][CN.QtyBooked]),
                                                                                           Convert.ToInt32(row[CN.ItemId]), Convert.ToInt32(row[CN.StockLocn]), Convert.ToDecimal(row[CN.Price])); //#12842

                    AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);

                    //update the LineItemBookingSchedule record just added with the BookinId
                    new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(lineItemBooking.Rows[0][CN.ID]), Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]));

                    AcctR.bookingType = "D";

                    var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking);

                    new Chub().SubmitMany(bookings, conn, trans);

                    new WarehouseRepository().UpdateBookingFailureActioned(conn, trans, cancelationId, Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]));      // #12379
                    //}
                }
            }
        }

        //IP - 28/04/09 - CR929 & 974 Deliveries - boolean to determine whether order details can be changed
        //prior to being DA'ed.
        public DataSet GetItemsForLocationChange(string acctno, bool loadBeforeDA)
        {
            return new DLineItem() { AccountNumber = acctno }.GetItemsForLocationChange(loadBeforeDA);

        }

        public void CancelDeliveryNote(SqlConnection conn, SqlTransaction trans, string acctNo, DataSet ds, bool isDotNetWarehouse)
        {
            DLineItem li = new DLineItem();
            DSchedule sched = new DSchedule();
            DBranch b = new DBranch();

            int lastBuffNo = 0;
            int buffNo = 0;

            foreach (DataRow r in ds.Tables["Schedules"].Rows)
            {
                if (lastBuffNo == 0 || lastBuffNo != Convert.ToInt32(r[CN.BuffNo]))
                    buffNo = b.GetBuffNo(conn, trans, Convert.ToInt16(r[CN.StockLocn]));

                sched.AccountNumber = acctNo;
                sched.AgreementNumber = (int)r[CN.AgrmtNo];
                sched.ItemID = Convert.ToInt32(r[CN.ItemId]);
                sched.StockLocation = Convert.ToInt16(r[CN.StockLocn]);
                sched.BuffNo = (int)r[CN.BuffNo];
                sched.BuffBranchNo = Convert.ToInt32(r[CN.BuffBranchNo]);
                sched.User = this.User;
                sched.CancelSchedule(conn, trans, buffNo, isDotNetWarehouse);

                li.Quantity = 0 - Convert.ToDouble(r[CN.Quantity]);
                li.AccountNumber = acctNo;
                li.AgreementNumber = (int)r[CN.AgrmtNo];
                li.ItemID = Convert.ToInt32(r[CN.ItemId]);
                li.StockLocation = Convert.ToInt16(r[CN.StockLocn]);
                li.ContractNo = "";
                li.UpdateDeliveryNote(conn, trans);

                lastBuffNo = Convert.ToInt32(r[CN.BuffNo]);
            }
        }

        public void GetScheduledDelNote(string accountNo,
            int agreementNo,
            int itemID,                                     //IP/NM - 18/05/11 -CR1212 - #3627 
            short location,
            out bool onPickList,
            out bool delNotePrinted,
            out bool onLoad)
        {
            DSchedule schedule = new DSchedule();
            schedule.AccountNumber = accountNo;
            schedule.AgreementNumber = agreementNo;
            //schedule.ItemNumber = itemNo;
            schedule.ItemID = itemID;                       //IP/NM - 18/05/11 -CR1212 - #3627 
            schedule.StockLocation = location;

            schedule.GetScheduledDelNote(out onPickList, out delNotePrinted, out onLoad);
        }

        public void GetCancelledDelNote(SqlConnection conn, SqlTransaction trans,
                            string accountNo, int agreementNo, int itemId,
                            short location, out int buffNo, out DateTime datePrinted)
        {
            DSchedule schedule = new DSchedule();
            schedule.AccountNumber = accountNo;
            schedule.AgreementNumber = agreementNo;
            schedule.ItemID = itemId;
            schedule.StockLocation = location;

            schedule.GetCancelledDelNote(conn, trans, out buffNo, out datePrinted);
        }

        // 67977 RD 22/02/06 Added TaxAmtBefore and TaxAmtAfter
        public void UpdateLineItemAudit(SqlConnection conn, SqlTransaction trans, double quantityBefore,
                                        double quantityAfter, decimal valueBefore, decimal valueAfter,
                                        double taxamtBefore, double taxamtAfter)
        {
            DLineItem l = new DLineItem();
            l.AccountNumber = this.AccountNumber;
            l.AgreementNumber = this.AgreementNumber;
            l.User = this.User;
            l.ItemNumber = this.ItemNumber;
            l.ItemID = this.ItemId;             //RI
            l.StockLocation = this.StockLocation;
            l.ContractNo = this.ContractNo;
            l.AuditSource = this.AuditSource;
            l.ParentItemNumber = this.ParentItemNumber; //IP - CR929 & 974 - 23/04/09 
            l.ParentStockLocation = this.ParentStockLocation; //IP - CR929 & 974 - 23/04/09
            l.ParentItemID = this.ParentItemId;
            l.UpdateLineItemAudit(conn, trans, quantityBefore, quantityAfter, valueBefore, valueAfter, taxamtBefore, taxamtAfter);
        }

        // sets the old location to 0 and the new location to the quantity
        public void UpdateLineItemAuditLocation(SqlConnection conn, SqlTransaction trans,
                                        double quantityAfter, decimal valueAfter,
                                         double taxamtAfter, short origLocation)
        {
            DLineItem l = new DLineItem();
            l.AccountNumber = this.AccountNumber;
            l.AgreementNumber = this.AgreementNumber;
            l.User = this.User;
            l.ItemNumber = this.ItemNumber;
            l.ItemID = this.ItemId;
            l.StockLocation = origLocation;
            l.ContractNo = this.ContractNo;
            l.AuditSource = this.AuditSource;
            // first update the old value
            l.UpdateLineItemAudit(conn, trans, quantityAfter, 0, valueAfter, 0, taxamtAfter, 0);
            // then the new value after changing branch
            l.StockLocation = this.StockLocation;
            l.UpdateLineItemAudit(conn, trans, 0, quantityAfter, 0, valueAfter, 0, taxamtAfter);
        }

        public DataSet GetRelatedItems(int itemId, short location)
        {
            DataSet ds = new DataSet();
            DLineItem item = new DLineItem();
            item.ItemID = itemId;
            item.StockLocation = location;
            item.GetRelatedItems();
            ds.Tables.Add(item.ItemDetails);
            return ds;
        }

        public XmlNode GetWarrantyRenewalItems(string acctNo, string acctType, string country,
                                                int agreementNo)
        {
            LineItemDocument items = new LineItemDocument();

            DLineItem line = new DLineItem();
            line.AccountNumber = acctNo;
            line.GetItemsForAccount();
            foreach (DataRow row in line.ItemDetails.Rows)
            {
                var itemId = Convert.ToInt32(row[CN.ItemId]);
                short location = (short)row["stocklocn"];
                string contractNo = (string)row[CN.ContractNo];

                LineItemNode item = items.CreateLineItemNode();
                //if (itemNodeCounter < 100)
                    PopulateItemNode(null, null, acctNo, agreementNo,
                                     itemId, location, acctType, country, false,
                                     contractNo, item, items, 0);
            }
            if (!items.HasLineItems)
                return null;
            else
                return items.Document.RemoveChild(items.Document.DocumentElement);
        }

        public DataSet GetItemsInRegion(int itemId, short branchNo)
        {
            DataSet ds = new DataSet();
            DStockItem item = new DStockItem();
            item.GetItemsInRegion(itemId, branchNo);
            ds.Tables.Add(item.StockLocations);
            return ds;
        }

        public DataSet GetPurchaseOrders(int itemId, short branchNo)
        {
            DataSet ds = new DataSet();
            DStockItem item = new DStockItem();
            item.GetPurchaseOrders(itemId, branchNo);
            ds.Tables.Add(item.ByCode);
            return ds;
        }

        public void MaintainPurchaseOrderStockLevel(SqlConnection conn, SqlTransaction trans,
                                                    string purchaseOrderNumber)
        {
            DStockItem stock = new DStockItem();
            stock.MaintainPurchaseOrderStockLevel(conn, trans, this.AccountNumber, this.ItemId,
                this.StockLocation, this.Quantity, this.AgreementNumber, purchaseOrderNumber);
        }

        public void LockItem(SqlConnection conn, SqlTransaction trans, string itemNo,           // TO DO use itemid??
                            short stockLocn, ref string lockString)
        {
            try
            {
                lockString = "";
                DStockItem stock = new DStockItem();
                stock.User = this.User;
                stock.LockItem(conn, trans, itemNo, stockLocn);
            }
            catch (Exception ex)
            {
                lockString = ex.Message;
            }
        }

        public void UnlockItem(SqlConnection conn, SqlTransaction trans)
        {
            DStockItem stock = new DStockItem();
            stock.User = this.User;
            stock.UnlockItem(conn, trans);
        }

        public DataSet GetAssociatedDiscounts(string accountNo, int itemId, short stockLocn)            // RI
        {
            DLineItem li = new DLineItem();
            DataSet ds = new DataSet();
            DataTable dt = li.GetAssociatedDiscounts(accountNo, itemId, stockLocn);                     // RI

            ds.Tables.Add(dt);

            return ds;
        }

        public void ReverseWarrantyAdjustment(SqlConnection conn, SqlTransaction trans,
                                              string acctNo, int agrmtNo, short branchNo,
                                              DataRow row)
        {
            string itemNo = row[CN.ItemNo].ToString();
            int itemID = Convert.ToInt32(row[CN.ItemId]);       //IP/NM - 18/05/11 -CR1212 - #3627 
            bool isCancellation = false;

            //Retrieve the LineItemBfCollection row for the specified key.
            //Here we are looking for warranties that have been collected
            //and an adjustment debited to the account.
            DLineItemBfCollection lbf = new DLineItemBfCollection();
            lbf.GetLineItemBfCollection(conn, trans, acctNo, agrmtNo, itemID,                       //IP/NM - 18/05/11 -CR1212 - #3627 
                                        row[CN.ContractNo].ToString(), out isCancellation);

            if (lbf.ItemDetails.Rows.Count > 0)
            {
                //Get price and adjustment type to reverse the
                //original warranty adjustment
                decimal reversal = lbf.Price * -1;
                string type = itemNo.Substring(0, 2) == "XW" ? TransType.FurnWarrantyRecovery : TransType.ElecWarrantyRecovery;

                DBranch branch = new DBranch();
                int transRefNo = branch.GetTransRefNo(conn, trans, branchNo);

                BTransaction t = new BTransaction(conn, trans, acctNo, branchNo,
                transRefNo, reversal, this.User, type, "", "", "", 0,
                (string)Country[CountryParameterNames.CountryCode], DateTime.Now, "", agrmtNo);

                lbf.Delete(conn, trans);
            }
        }

        public void ProcessLineItems(SqlConnection conn, SqlTransaction trans, string acctNo,
                             int agrmtNo, int itemID, short stockLocn, string contractNo,  //IP - 18/05/11 -CR1212 - #3627  - replaced itemNo with itemID              
                             decimal qty, decimal ordVal, string auditSource, int warrantyID,   //IP - 18/05/11 -CR1212 - #3627 
                             short warrantyLocn, string exchangedWarrantyContractNo, ref decimal taxAmount)
        {
            DLineItem li = new DLineItem();
            DAccount acct = new DAccount(conn, trans, acctNo);


            // todo uat363 rdb need to get PArentProductCode 
            li.GetSingleItem(conn, trans, stockLocn, itemID,
                                 acctNo, agrmtNo, contractNo, 0);       //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID
            //UAT160 
            double bfQty = 0;
            double newQty = 0;
            string parentItemNo = "";
            int parentItemID = 0;           //IP - 17/05/11 - CR1212 - #3627

            if (contractNo == string.Empty)
            {
                bfQty = li.Quantity;
                newQty = li.Quantity + Convert.ToDouble(qty);
                parentItemNo = li.ParentItemNumber;
            }
            else
            {
                bfQty = Convert.ToDouble(qty);
                newQty = Convert.ToDouble(qty);
                parentItemNo = li.ParentItemNumber;
            }
            //UAT160 end

            decimal bfValue = Convert.ToDecimal(bfQty) * li.Price;
            decimal newValue = Convert.ToDecimal(newQty) * li.Price;
            double bfTaxamt = li.TaxAmount;
            decimal bfPrice = li.Price;


            // todo uat363 need to get the parentItemNo here 
            UpdateItemQuantity(conn, trans, acctNo, agrmtNo, itemID, stockLocn, contractNo, qty, parentItemID); //UAT160        //IP - 17/05/11 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo
            // todo uat363 rdb need to get PArentProductCode
            if (contractNo == string.Empty)     //UAT160
            {
                UpdateDelQty(conn, trans, acctNo, agrmtNo, stockLocn, itemID, contractNo, qty, parentItemID);       //UAT160     //IP - 17/05/11 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo
            }
            UpdateItemValue(conn, trans, acctNo, agrmtNo, itemID, stockLocn, ordVal);               //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID 

            //XmlNode itemNode = GetItemDetails(itemNo, stockLocn, acct.AccountType, (string)Country[CountryParameterNames.CountryCode], false, false, 0, acctNo, agrmtNo);   // CR1212 jec need to supply itemID not zero
            XmlNode itemNode = GetItemDetails(itemID, stockLocn, acct.AccountType, (string)Country[CountryParameterNames.CountryCode], false, false, acctNo, agrmtNo, conn, trans);   // CR1212 jec need to supply itemID not zero //IP - 18/05/11 - CR1212 - CR1212 - #3627 - changed to use itemID rather than 0
            itemNode.Attributes[Tags.Quantity].Value = newQty.ToString();
            itemNode.Attributes[Tags.UnitPrice].Value = li.Price.ToString();
            itemNode.Attributes[Tags.TaxAmount].Value = li.TaxAmount.ToString();
            taxAmount += CalculateTaxAmount(itemNode, acct.IsTaxExempt(conn, trans, acctNo, ""));

            if (taxAmount > 0)
                UpdateTaxAmount(conn, trans, acctNo, itemID, stockLocn, taxAmount); //IP - 18/05/11 -CR1212 - #3627 - replaced itemNo with itemID

            // Save details to the LineItemAudit table to ensure the
            // correct information is displayed on the Agreement Audit
            // tab in the Account Details screen.
            if (li.Quantity != newQty || li.OrderValue != newValue)
            {
                this.AccountNumber = acctNo;
                this.AgreementNumber = agrmtNo;
                this.User = this.User;
                //this.ItemNumber = itemNo;
                this.ItemId = itemID;                       //IP - 18/05/11 - CR1212 - #3627 - Added itemID
                this.StockLocation = stockLocn;
                this.ContractNo = contractNo;
                this.AuditSource = auditSource;

                // Recalling getsingleitem in order to get the new value for taxamt
                // todo uat363 rdb need to get PArentProductCode
                li.GetSingleItem(conn, trans, stockLocn, itemID,                        //IP - 18/05/11 - CR1212 - #3627 - changed to use itemID rather than itemNo
                    acctNo, agrmtNo, contractNo, parentItemID);                         //IP - 18/05/11 - CR1212 - #3627 - changed to use parentItemID rather than ""

                UpdateLineItemAudit(conn, trans, bfQty, newQty, bfValue, newValue, bfTaxamt, li.TaxAmount);
            }

            // if (warrantyNo.Length > 0)
            if (warrantyID != 0)                                //IP - 18/05/11 - CR1212 - #3627 
            {
                li.AccountNumber = acctNo;
                //li.ItemNumber = itemNo;                      //IP - 18/05/11 - CR1212 - #3627 - Added itemID
                li.ItemID = itemID;
                li.StockLocation = stockLocn;
                li.ContractNo = exchangedWarrantyContractNo;

                li.LinkWarrantyToItem(conn, trans, warrantyID, warrantyLocn);
            }
        }

        public void UpdateAuditItem(SqlConnection conn, SqlTransaction trans)
        {
            DLineItem line = new DLineItem();
            line.AccountNumber = this.AccountNumber;
            line.AgreementNumber = this.AgreementNumber;
            line.ItemNumber = this.ItemNumber;
            line.StockLocation = this.StockLocation;
            line.AuditSource = this.AuditSource;
            line.UpdateAuditItem(conn, trans);
        }

        public bool IsDiscountItem(string itemNo)       // TO DO use itemid??
        {
            DStockItem stock = new DStockItem();
            return stock.IsDiscountItem(itemNo);
        }

        public DataSet GetTopSellingCashandGo(short branchCode
            )
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.GetTopSellingCashandGo(branchCode);
            ds.Tables.Add(stock.TopSelling);
            return ds;
        }

        //CR1094 jec 09/12/10 - get Non stock items
        public DataSet GetNonStockByCode(string itemNo)         // TO DO use itemid??
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            ds = stock.GetNonStockByCode(itemNo);

            return ds;
        }

        //CR1094 jec 09/12/10 - get all categories
        public DataTable GetCategories()
        {
            DataTable dt = new DataTable();
            DStockItem stock = new DStockItem();
            dt = stock.GetCategories();

            return dt;
        }

        //CR1094 jec 10/12/10 - save Non stock item
        public void SaveNonStockItem(SqlConnection conn, SqlTransaction trans, DataTable nonstock, DataTable prices)
        {
            DStockItem stock = new DStockItem();
            DStockItem price = new DStockItem();
            foreach (DataRow row in nonstock.Rows)
            {
                stock.ItemNo = (string)row[CN.ItemNo];
                stock.ItemDescr1 = (string)row[CN.ItemDescr1];
                stock.ItemDescr2 = (string)row[CN.ItemDescr2];
                stock.ItemSupplierName = (string)row[CN.Supplier];
                stock.ItemSupplierCode = (string)row[CN.SupplierCode];
                stock.ItemCategory = Convert.ToInt16(row[CN.Category]);
                stock.ItemTaxRate = Convert.ToDouble(row[CN.TaxRate]);
                stock.Deleted = (string)row[CN.Deleted];
                stock.DeletionDate = Convert.ToDateTime(row[CN.EndDate]);
                stock.ItemId = Convert.ToString(row[CN.ItemId]).TryParseInt32(0).Value;      // RI

                stock.SaveNonStockItem();
            }

            foreach (DataRow row in prices.Rows)
            {
                price.ItemNo = (string)row[CN.ItemNo];
                price.BranchNo = Convert.ToInt16(row[CN.BranchNo]);
                price.UnitHPPrice = Convert.ToDecimal(row[CN.UnitPriceHP]);
                price.UnitCashPrice = Convert.ToDecimal(row[CN.UnitPriceCash]);
                price.UnitDutyFreePrice = Convert.ToDecimal(row[CN.UnitPriceDutyFree]);
                price.UnitCostPrice = Convert.ToDecimal(row[CN.CostPrice]);
                price.ItemId = Convert.ToString(row[CN.ItemId]).TryParseInt32(0).Value;      // RI

                //Start date is in the Stock table
                foreach (DataRow rowA in nonstock.Rows)
                {
                    if ((string)row[CN.ItemNo] == (string)rowA[CN.ItemNo])
                    {
                        price.DeletionDate = Convert.ToDateTime(rowA[CN.StartDate]);
                    }
                }

                price.SaveNonStockPrice();
            }
        }

        //CR1094 jec 15/12/10 - get Warranty return Codes
        public DataTable GetWarrantyReturnCodes()
        {
            DataTable dt = new DataTable();
            DStockItem stock = new DStockItem();
            dt = stock.GetWarrantyReturnCodes();

            return dt;
        }

        //CR1094 jec 10/12/10 - save Non stock item
        public void SaveWarrantyReturnCodes(SqlConnection conn, SqlTransaction trans, DataTable returnCodes, DateTime dateNow)
        {
            DStockItem stock = new DStockItem();
            foreach (DataRow row in returnCodes.Rows)
            {
                stock.ProductType = (string)row[CN.ProductType];
                stock.WarrCategory = (string)row[CN.Category];
                stock.ReturnCode = (string)row[CN.ReturnCode];
                stock.WarrantyMonths = Convert.ToInt16(row[CN.WarrantyMonths]);
                stock.ManufactMonths = Convert.ToInt16(row[CN.ManuFacturerMonths]);
                stock.ExpiredMonths = Convert.ToInt16(row[CN.ExpiredPortion]);
                stock.RefundPct = Convert.ToDouble(row[CN.RefundPercentage]);
                stock.DateNow = dateNow;

                stock.SaveWarrantyReturnCodes();
            }
        }

        //CR1094 jec 23/12/10 - get All Warranty Items
        public DataTable GetAllWarrantyItems()
        {
            DataTable dt = new DataTable();
            DStockItem stock = new DStockItem();
            dt = stock.GetAllWarrantyItems();

            return dt;
        }

        // CR1212 RI
        public DataSet ProductAssociationGetDetails()
        {
            DataSet ds = new DataSet();

            DStockItem pa = new DStockItem();
            ds = pa.ProductAssociationGetDetails();

            return ds;
        }
        // CR1212 RI
        public void ProductAssociationSaveDetails(DataTable associated)
        {
            DStockItem pa = new DStockItem();

            foreach (DataRow row in associated.Rows)
            {
                pa.ProductGroup = (string)row[CN.ProductGroup];
                pa.Category = Convert.ToInt16(row[CN.Category]);
                pa.Class = (string)row[CN.Class];
                pa.SubClass = (string)row[CN.SubClass];
                pa.ItemId = Convert.ToInt32(row[CN.ItemId]);
                pa.Deleted = (string)row[CN.Deleted];        // RI

                pa.ProductAssociationSaveDetails();
            }
        }

        public DataSet OnlineProductSearch(string location, short category, string online,
            DateTime dateAdded,
            DateTime dateRemoved,
            string prodDesc,
            bool limit)
        {
            DataSet ds = new DataSet();
            DStockItem stock = new DStockItem();
            stock.OnlineProductSearch(location, category, online, dateAdded, dateRemoved, prodDesc, limit ? 250 : 4000);
            ds.Tables.Add(stock.ByLocation);
            return ds;
        }

        //BOC Added by Suvidha - CR 2018-13 - 05/01/19 - to update invoice version in table InvoiceDetails.
        public void UpdateInvoiceVersion(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo)
        {
            DLineItem li = new DLineItem();
            li.UpdateInvoiceVersion(conn, trans, accountNo, agreementNo);
        }
        //EOC

        //to create LineItem Node to be displayed on Reprint Screen
        private void PopulateItemNode_ReprintWin(SqlConnection conn, SqlTransaction trans,
                                        string accountNumber, int agreementNo,
                                        int itemId, short branch,
                                        string accountType, string country,
                                        bool parentIsKit, string contractNo,
                                        LineItemNode item,
                                        LineItemDocument items, int parentItemId, int version = 0)
        {
            string type = IT.Unknown;
            bool canAddWarranty = false;
            string lastDelivery = "";
            //itemNodeCounter++;
            //Get the stock item details for this item
            int versionno = version;
            DStockItem stock = new DStockItem();
            stock.PromoBranch = this.PromoBranch;
            DAccount account = new DAccount(null, null, accountNumber);
            account.GetAccount(accountNumber);
            stock.GetItemDetails(conn, trans, itemId, branch, accountType, country, false, false, accountNumber, agreementNo);

            //Get the line item details for this item
            DLineItem line = new DLineItem();
            line.AccountNumber = accountNumber;
            line.AgreementNumber = agreementNo;
            line.StockLocation = branch;
            line.ItemID = itemId;
            line.ContractNo = contractNo;
            line.ParentItemID = parentItemId;
            line.InvoiceVersion = version;
            line.IsAmortized = account.IsAmortized;
            line.GetItemDetailsForReprint(conn, trans);        

            //Check if this item can have a warranty added to it
            switch (line.CanAddWarranty(conn, trans))
            {
                case 0:
                    canAddWarranty = true;
                    break;
                //IP - 19/08/08 - Commented out case 3, as in the stored procedure
                //'DN_WarrantyCanAddSP', if the result returned was '3' then you should
                //not be able to add a warranty to the item.
                //case 3:	canAddWarranty = true;	//2 means the item already has a 
                //break;						//warranty attached but it can 
                //still potentially be amended
                default:
                    canAddWarranty = false;
                    break;
            }

            item.Key = line.ItemID + "|" + branch.ToString();

            if (stock.IsStock)
            {
                if (parentIsKit && line.IsComponent) // to do add option to check if free gift. Either we do this by checking whether exists in kit product 
                    //-- where the item is actually check against the parent or we say if discount category then potentially not component
                    // Question is it easier to get this from the lineitm or from the stock table.... 
                    // Well look at the lineitem table well we can checkk the parent item number and if parent is kit and item is componennt 
                    //then IT.component
                    type = IT.Component;
                else
                    type = IT.Stock;
            }
            if (stock.IsKit)
                type = IT.Kit;

            if (stock.IsDiscount)
            {
                if (parentIsKit)
                    type = IT.KitDiscount;
                else
                    type = IT.Discount;
                canAddWarranty = false;
            }
            if (stock.IsWarranty)
            {
                if (parentIsKit)
                    type = IT.KitWarranty;
                else
                    type = IT.Warranty;
                canAddWarranty = false;
            }
            if (stock.IsAffinity)
            {
                type = IT.Affinity;
                canAddWarranty = false;
            }

            if (stock.IsStampDuty)
            {
                type = IT.SundryCharge;
                canAddWarranty = false;
            }

            //IP - 24/02/11 - #3130
            if (stock.IsInstallation)
            {
                type = IT.Installation;
                canAddWarranty = false;
            }

            if (stock.IsAssemblyCost)
            {
                type = IT.AssemblyCost;
                canAddWarranty = false;
            }

            if (stock.IsAnnualServiceContract)
            {
                type = IT.AnnualService;
                canAddWarranty = false;
            }

            if (stock.IsGenericService)
            {
                type = IT.GenericService;
                canAddWarranty = false;
            }

            item.Type = type;
            item.Code = line.ItemNumber;
            item.ItemId = itemId;
            item.Location = branch;
            item.AvailableStock = Convert.ToDecimal(stock.AvailableStock);
            item.DamagedStock = Convert.ToDecimal(stock.DamagedStock);
            item.Description1 = stock.ProductDesc1;
            item.Description2 = stock.ProductDesc2;
            item.SupplierCode = stock.SupplierCode;
            item.UnitPrice = line.Price;
            item.CashPrice = Convert.ToDecimal(stock.CashPrice);
            item.CostPrice = Convert.ToDecimal(stock.CostPrice);
            item.HPPrice = Convert.ToDecimal(stock.HPPrice);
            item.DutyFreePrice = Convert.ToDecimal(stock.DutyFreePrice);
            item.ValueControlled = stock.ValueControlled;
            item.Quantity = Convert.ToDecimal(line.Quantity);
            item.Value = line.OrderValue;
            item.DeliveryDate = line.DateRequiredDelivery.ToString();
            item.DeliveryTime = line.TimeRequiredDelivery;
            item.BranchForDeliveryNote = line.DeliveryNoteBranch;
            item.ColourTrim = line.Notes;
            item.QtyOnOrder = Convert.ToDecimal(stock.QtyOnOrder);
            //IP/JC - 02/03/10 - CR1072 - Malaysia 3PL 
            item.VanNo = line.VanNo;
            item.DhlPickingDate = line.DhlPickingDate;
            item.DhlInterfaceDate = line.DhlInterfaceDate;
            item.DhlDNNo = line.DhlDNNo;
            //item.OrigQty = line.OrigQty;
            item.ShipQty = line.ShipQty;
            item.ItemRejected = line.ItemRejected;          //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            item.ParentItemNo = line.ParentItemNumber;
            item.ParentItemId = parentItemId;
            item.SalesBrnNo = !line.SalesBrnNo.HasValue && line.ItemType.Trim() == "S" ? Convert.ToInt16(accountNumber.Substring(0, 3)) : line.SalesBrnNo; //IP - 23/05/11 - CR1212 - RI - #3651 - Either return existing Sales Branch if populated or first 3 digits of the account number ONLY for stockitems.
            item.RepoItem = line.RepoItem;              //RI jec 16/06/11
            item.Class = stock.Class;        //IP - 28/07/11 - RI - #4415  
            item.SubClass = stock.SubClass;  //IP - 28/07/11 - RI - #4415
            item.Brand = stock.Brand;        //IP - 19/09/11 - RI - #8218 - CR8201
            item.Style = stock.Style;        //IP - 20/09/11 - RI - #8218 - CR8201

            //If it's a discount then it should take the tax rate of the thing it discounts
            //i.e. it's parent (unless it doesn't have one
            if (type == IT.KitDiscount)	//if it's a KitDiscount
            {
                try
                {
                    //TO DO item.TaxRate = item.Parent.TaxRate;
                    item.TaxRate = Convert.ToDecimal(item.Node.ParentNode.Attributes[Tags.TaxRate].Value);
                }
                catch (Exception)
                {
                    item.TaxRate = Convert.ToDecimal(stock.TaxRate);
                }
            }
            else
                item.TaxRate = Convert.ToDecimal(stock.TaxRate);

            item.AdditionalTaxRate = Convert.ToDecimal(stock.Additionaltaxrate); //BCX Change for Set Additional Tax
            item.DeliveredQuantity = Convert.ToDecimal(line.DeliveredQuantity);
            item.PlannedDeliveryDate = line.DatePlannedDelivery.ToString();
            item.CanAddWarranty = canAddWarranty;
            item.DeliveryAddress = line.DeliveryAddress;
            item.DeliveryArea = line.DeliveryArea;
            item.DeliveryProcess = line.DeliveryProcess;
            item.QuantityDiff = line.QuantityDiff;
            item.ScheduledQuantity = Convert.ToDecimal(line.ScheduledQuantity);
            item.TaxAmount = Convert.ToDecimal(line.TaxAmount);
            item.ContractNo = line.ContractNo;
            item.ReturnItemNo = line.ItemNumber;
            item.ReturnLocation = branch;
            item.FreeGift = stock.IsFreeGift;
            item.ExpectedReturnDate = line.ExpectedReturnDate.ToString();
            item.PurchaseOrder = false;
            item.LeadTime = stock.LeadTime;
            item.Damaged = line.Damaged;
            item.Assembly = line.Assembly;
            item.ProductCategory = stock.ProductCategory;
            item.Deleted = stock.Deleted;
            item.PurchaseOrderNumber = "";
            item.ReplacementItem = false;
            item.SPIFFItem = line.SPIFFItem;
            item.IsInsurance = (line.ItemNumber == (string)Country[CountryParameterNames.InsuranceChargeItem]);
            item.RefCode = stock.RefCode; //IP - 29/01/10 - LW 72136

            item.Category = stock.Category;
            item.LineItemId = line.LineItemId;    //#13716 - CR12949
            item.ReadyAssist = stock.ReadyAssist; //#13716 - CR12949
            item.WarrantyType = stock.WarrantyType;     //#17883 //#15888
            item.IsAmortized = line.IsAmortized;

            if (!Convert.IsDBNull(line.DateOfLastDelivery))
                lastDelivery = line.DateOfLastDelivery.ToString();
            item.DateDelivered = lastDelivery;

            item.Express = line.Express;        //IP - 07/06/12 - #10229 - Warehouse & Deliveries


            //Get any child items for this item
            DataTable dt = line.GetChildLineItemCodes_Reprint(conn, trans, accountNumber, agreementNo, line.ItemID, branch, versionno);
            foreach (DataRow row in dt.Rows)
            {
                LineItemNode li = new LineItemNode(item.Document);
                item.AddRelatedItem(li);
                //itemNodeCounter++;

                //if (itemNodeCounter < 100)
                PopulateItemNode_ReprintWin(conn, trans, accountNumber, agreementNo, Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row["stocklocn"]),
                            accountType, country, stock.IsKit, (string)row[CN.ContractNo],
                            li, items, line.ItemID, version);
            }

            if (line.ItemType == "S")   //#17407
            {
                GetServiceInfo(accountNumber, line.ItemNumber, line.StockLocation.ToString(), item);
            }

        }
    }
}
