using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.TableNames;
using STL.Common.ServiceRequest;
using STL.Common.Static;
using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for BSchedule.
    /// </summary>
    public class BSchedule : CommonObject
    {
        public BSchedule()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Load Schedules (Delivery Notes) by branch number and buff number.
        /// </summary>
        public DataSet GetByBuffNo(int BranchNo, int BuffNo)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetByBuffNo(BranchNo, BuffNo);
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        /// <summary>
        /// Generate a new Picklist and retrieve its Picklist number.
        /// </summary>
        public void GetNextPicklistNo(int branchNo, int user, string pickListType,
                                            out int pickListNo)
        {
            pickListNo = 0;

            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetNextPicklistNo(branchNo, user, pickListType, out pickListNo);
        }
        /// <summary>
        /// 5.1 UAT 74 rdb 8/11/07 - ResetHoldProp.  See Param
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="ds"></param>
        /// <param name="ResetHoldProp">5.1 UAT 74 rdb 8/11/07 - when a schedule is deleted because 
        /// an item with quatity > 1 has been partly delivered, the remaining quantity is
        /// set to 0, the holdprop does not need to be set to 'Y'
        /// </param>
        public void DeleteDeliverySchedules(SqlConnection conn, SqlTransaction trans, DataSet ds, bool resetHoldProp)
        {
            DSchedule sched = new DSchedule();
            sched.User = this.User;
            BAgreement agreement = new BAgreement();
            foreach (DataRow row in ds.Tables["Schedules"].Rows)
            {
                sched.DeleteDeliverySchedule(conn, trans, (string)row["AccountNo"],
                    (int)row["AgreementNo"],
                    //(string)row["ItemNo"],
                    (int)row[CN.ItemId],                                            //IP - 07/06/11 - CR1212 - RI
                    (short)row["Location"],
                    (short)row["Branch"],
                    (int)row["Number"],
                    (double)row["Quantity"],
                    (double)row["QtyRemoved"]); //IP/JC - 03/03/10 - CR1072 - Malaysia 3PL 

                // UAT 255 - The agreement must be authorised again even if the same items are
                // added back onto the account so that the agreement total has not changed.
                // ALSO: The agreement must be authorised again if Schedule Override has been
                // used to remove an item but then the user did not click 'Save' on the 
                // NewAccount screen.
                agreement.Populate(conn, trans, (string)row["AccountNo"], (int)row["AgreementNo"]);
                agreement.AgreementDate = DateTime.Today;

                if (resetHoldProp)
                {
                    agreement.HoldProp = "Y";
                }
                agreement.Save(conn, trans);
            }
        }

        public void UpdateSchedule(SqlConnection conn, SqlTransaction trans, DataSet ds)
        {
            DSchedule sched = new DSchedule();
            foreach (DataRow row in ds.Tables["Schedules"].Rows)
            {
                sched.OrigBr = 0;
                sched.AccountNumber = (string)row["acctno"];
                sched.AgreementNumber = Convert.ToInt32(row["agrmtno"]);
                sched.DeliveryOrCollection = (string)row["delorcoll"];
                sched.ItemNumber = (string)row["itemno"];
                sched.StockLocation = (short)Convert.ToInt32(row["stocklocn"]);
                sched.Quantity = Convert.ToDouble(row["quantity"]);

                if (sched.DeliveryOrCollection == "C" || sched.DeliveryOrCollection == "R")
                    sched.ReturnItemNumber = "";
                else
                    sched.ReturnItemNumber = (string)row["retitemno"];

                sched.ReturnStockLocation = (short)Convert.ToInt32(row["retstocklocn"]);
                sched.BuffNo = Convert.ToInt32(row["buffno"]);
                sched.BuffBranchNo = (short)Convert.ToInt32(row["buffbranchno"]);
                sched.DateDelPlan = Convert.ToDateTime(row["dateplandel"]);
                sched.RetVal = Convert.ToDouble(row["retval"]);
                sched.User = this.User;
                sched.ParentItemID = Convert.ToInt32(row["ParentItemId"]);
                sched.Write(conn, trans, this.User);
            }
        }


        public void UpdateScheduleForPicklist(SqlConnection conn, SqlTransaction trans,
            DataSet ds, int branchNo, string pickListType)
        {
            DSchedule sched = new DSchedule();
            foreach (DataRow row in ds.Tables["Schedules"].Rows)
            {
                if (Convert.ToBoolean(row["released"]))
                {
                    sched.AccountNumber = ((string)row["acctno"]).Replace("-", "");
                    sched.StockLocation = (short)Convert.ToInt32(row["stocklocn"]);
                    sched.BuffNo = Convert.ToInt32(row["buffno"]);
                    sched.ItemID = Convert.ToInt32(row[CN.ItemId]);
                    sched.PicklistNumber = Convert.ToInt32(row["picklistnumber"]);
                    sched.PicklistBranchNumber = branchNo;
                    sched.UpdateScheduleForPicklist(conn, trans, pickListType);
                }
            }
        }

        public int ScheduleAssignNewBufferNo(SqlConnection conn, SqlTransaction trans,
            string acctno, int itemId,
            short stockLocn, int buffNo, int newBuffNo)
        {
            DSchedule sched = new DSchedule();
            DBranch branch = new DBranch();
            //			int newBuffNo =  branch.GetBuffNo(stockLocn);

            sched.AccountNumber = acctno.Replace("-", "");
            sched.ItemID = itemId;
            sched.StockLocation = stockLocn;
            sched.BuffNo = buffNo;
            sched.ScheduleAssignNewBufferNo(conn, trans, newBuffNo);
            return newBuffNo;
        }

        public void SaveSchedule(SqlConnection conn, SqlTransaction trans, DataSet ds,
            short branchNo, string countryCode, string accountType,
            string accountNo, int agreementNo, bool collectAssociated, int? serviceRequestNo = null)        //#11989 - added serviceRequestNo
        {
            DSchedule sched = new DSchedule();
            DLineItem li = new DLineItem();
            DBranch branch = new DBranch();
            XmlNode lineItems = null;
            BItem lineItem = new BItem();
            //decimal price = 0;
            string itemType = "";
            DateTime dateTrans = DateTime.Now;
            //string transType = "";
            string notes = "";
            bool collection = false;
            decimal collectionAmount = 0;
            decimal taxAmount = 0;
            int lastLocation = 0;
            int buffNo = 0;
            DDelivery del = new DDelivery();
            del.User = this.User;
            sched.User = this.User;
            li.User = this.User;
            bool exchange = false;
            lineItem.PromoBranch = branchNo;
            lineItems = lineItem.GetLineItems(conn, trans, accountNo, accountType, countryCode, agreementNo);

            // Make sure this account is not settled
            DAccount acct = new DAccount(conn, trans, accountNo);
            if (acct.CurrentStatus == "S")
            {
                DStatus stat = new DStatus();
                acct.CurrentStatus = stat.Unsettle(conn, trans, accountNo, DateTime.Now, User);
                acct.Save(conn, trans);
            }

            // need to sort dataset by return stock location, to ensure the same buff no
            // is used for all items that are being returned to the same location
            ds.Tables["Schedules"].DefaultView.Sort = CN.RetStockLocn + " ASC";
            foreach (DataRowView row in ds.Tables["Schedules"].DefaultView)
            {
                if (lastLocation == 0 || lastLocation != Convert.ToInt32(row[CN.RetStockLocn]))
                    buffNo = branch.GetBuffNo(conn, trans, Convert.ToInt16(row[CN.RetStockLocn]));

                sched.OrigBr = 0;
                sched.AccountNumber = (string)row[CN.AcctNo];
                sched.AgreementNumber = Convert.ToInt32(row[CN.AgrmtNo]);
                sched.DeliveryOrCollection = "C";
                sched.ItemNumber = (string)row[CN.ItemNo];
                sched.ItemID = Convert.ToInt32(row[CN.ItemId]);       //RI 
                //sched.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                sched.Quantity = Convert.ToDouble(row[CN.Quantity]);
                sched.ReturnStockLocation = (short)Convert.ToInt32(row[CN.RetStockLocn]);
                sched.CollectReason = (string)row[CN.Reason];
                sched.CollectType = (string)row[CN.CollectionType];
                sched.CreatedBy = this.User;  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                sched.DateCreated = DateTime.Today;  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                sched.GRTnotes = (string)row[CN.Notes];  //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)

                if ((string)row[CN.ItemType] == "S")
                {
                    sched.ReturnItemNumber = "";
                    sched.RetItemID = 0;          //RI 
                    sched.BuffNo = buffNo;
                    sched.BuffBranchNo = (short)Convert.ToInt32(row[CN.RetStockLocn]);
                }
                else
                {
                    sched.ReturnItemNumber = (string)row[CN.RetItemNo];
                    sched.RetItemID = Convert.ToInt32(row[CN.RetItemId]);       //RI 
                    sched.BuffNo = Convert.ToInt32(row[CN.BuffNo]);
                    sched.BuffBranchNo = (short)Convert.ToInt32(row[CN.BuffBranchNo]);
                }

                sched.DateDelPlan = Convert.ToDateTime(row[CN.DateDelPlan]);
                sched.RetVal = Convert.ToDouble(row[CN.RetVal]);
                string user = (string)row[CN.EmployeeNo];

                sched.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                sched.ContractNo = (string)row[CN.ContractNo];

                string contractNo = (string)row[CN.ContractNo];
                string deliveryArea = (string)row[CN.DeliveryArea];
                string deliveryProcess = (string)row[CN.DeliveryProcess];

                string parentItemNo = row[CN.ParentItemNo].ToString();
                int parentItemID = Convert.ToInt32(row[CN.ParentItemId]);               //IP - 17/05/11 - CR1212 - #3627
                itemType = (string)row[CN.ItemType];
                notes = (string)row[CN.Reason];
                //IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
                //if ((string)row[CN.Notes] != "")       // CN.Notes blank when notes textbox not enabled in GRT screen
                //    notes = notes + " [GRT Notes:" + (string)row[CN.Notes] + "]";

                sched.ParentItemID = parentItemID;

                //#10789 - no longer inserting into schedule for immediate //#10522 - Insert into schedule for Immediate
                //if (deliveryProcess == "I" || (deliveryProcess == "S" && itemType!= "S")) //#10522 - write Non Stocks to schedule (used for collection)
                if (itemType != "S") //#10789 //#10522 - write Non Stocks to schedule (used for collection)
                {
                    sched.Write(conn, trans, this.User);
                }
                //Insert into LineItemBookingSchedule for Scheduled
                else
                {
                    if (itemType == "S" && (string)row[CN.DelOrColl] != "I") //#17290
                    {
                        new WarehouseRepository().InsertLineItemBookingSchedule(conn, trans, Convert.ToInt32(row[CN.LineItemId]), "C", Convert.ToInt32(row[CN.RetItemId]),      //#10789
                                                                                            Convert.ToDecimal(row[CN.RetVal]), Convert.ToInt16(row[CN.RetStockLocn]), 0, Convert.ToSingle(row[CN.Quantity]),
                                                                                            Convert.ToInt32(row[CN.ItemId]), Convert.ToInt32(row[CN.StockLocn]), Convert.ToDecimal(row[CN.Price]));  //#12842
                    }
                }
                // write collection reason  CR36
                sched.WriteCollectReason(conn, trans, this.User);

                var bookingId = 0; //#14313

                //if (deliveryProcess == "S" && itemType=="S")         // #10506 #10441 jec GRT screen should generate 'collect' booking
                if (itemType == "S" && (string)row[CN.DelOrColl] != "I") //#17290  //#10789       // #10506 #10441 jec GRT screen should generate 'collect' booking
                {
                    AccountRepository AcctR = new AccountRepository();
                    WarehouseRepository Whr = new WarehouseRepository();
                    AcctR.bookingType = "C";        // collection

                    var lineItemBooking = new DataTable();
                    lineItemBooking.Columns.Add("id");
                    lineItemBooking.Columns.Add("QtyBooked");
                    lineItemBooking.Columns.Add("BookingID");

                    var dr = lineItemBooking.NewRow();
                    dr["id"] = Convert.ToInt32(row[CN.LineItemId]);
                    dr["QtyBooked"] = Math.Abs(Convert.ToDouble(row[CN.Quantity]));     // Qty must be positive in warehouse
                    dr["BookingID"] = Convert.ToInt32(0);

                    lineItemBooking.Rows.Add(dr);

                    AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);

                    Whr.UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(lineItemBooking.Rows[0]["id"]), Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"])); //#13829 - Update before GetBookingData 

                    bookingId = Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]); //#14313

                    var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking,
                        Convert.ToInt16(row[CN.StockLocn]), Convert.ToInt16(row[CN.RetStockLocn]), deliveryArea, (string)row[CN.Notes],
                        deliveryProcess: deliveryProcess, deliveryAdr: Convert.ToString(row[CN.DeliveryAddress]));   // # 14927 #12378 #10481 #10489

                    new Chub().SubmitMany(bookings, conn, trans);



                    //#11989 - This has come through the BERReplacements screen therefore update as actioned.
                    //Also updating BookingId as they may cancel the collection note later, therefore we need to know the BookingId
                    if (serviceRequestNo != null)
                    {
                        new ServiceRepository().UpdateReplacementActioned(conn, trans, accountNo, Convert.ToInt32(serviceRequestNo), Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row[CN.StockLocn]), Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]));
                    }

                }

                if ((string)row[CN.DelOrColl] == "R")
                {
                    /* update the lineitem delqty += quantity */
                    // 69577  jec 03/03/08   Do not reduce DelQty for instant replacement as does not get incremented later
                    //li.UpdateDelQty(conn, trans, sched.AccountNumber, (short)Convert.ToInt32(row[CN.StockLocn]), 
                    //    sched.AgreementNumber, sched.ItemNumber, 
                    //    contractNo,
                    //    sched.Quantity);	
                    double Quantity = Math.Abs(Convert.ToDouble(row[CN.Quantity]));
                    // write schedule record for replacement delivery note
                    if ((string)row[CN.ItemType] == "S")
                    {
                        if (del.IsDotNetWarehouse(branchNo))
                        {
                            sched.BuffNo = branch.GetBuffNo(conn, trans, sched.ReturnStockLocation);
                            sched.Quantity = Quantity;
                            sched.DeliveryOrCollection = "D";
                            sched.ContractNo = "";
                            sched.ReturnItemNumber = GetResource("T_REPLACE");
                            sched.ParentItemID = parentItemID;
                            sched.Write(conn, trans, this.User);

                            //#10789 - Write LineItemBookingSchedule record
                            new WarehouseRepository().InsertLineItemBookingSchedule(conn, trans, Convert.ToInt32(row[CN.LineItemId]), "D", 0,      //#10789
                                0, 0, 0, Convert.ToSingle(row[CN.Quantity]) < 0 ? Math.Abs(Convert.ToSingle(row[CN.Quantity])) : Convert.ToSingle(row[CN.Quantity]),
                                 Convert.ToInt32(row[CN.ItemId]), Convert.ToInt32(row[CN.StockLocn]), Convert.ToDecimal(row[CN.Price])); //#12842

                            // #10411 submit Exchange booking etc
                            AccountRepository AcctR = new AccountRepository();
                            WarehouseRepository Whr = new WarehouseRepository();
                            AcctR.bookingType = "D";        // delivery

                            var lineItemBooking = new DataTable();
                            lineItemBooking.Columns.Add("id");
                            lineItemBooking.Columns.Add("QtyBooked");
                            lineItemBooking.Columns.Add("BookingID");

                            var dr = lineItemBooking.NewRow();
                            dr["id"] = Convert.ToInt32(row[CN.LineItemId]);
                            dr["QtyBooked"] = Math.Abs(Convert.ToDouble(row[CN.Quantity]));     // Qty must be positive in warehouse
                            dr["BookingID"] = Convert.ToInt32(0);

                            lineItemBooking.Rows.Add(dr);

                            AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);

                            Whr.UpdateLineItemBookingScheduleBookingId(conn, trans, Convert.ToInt32(lineItemBooking.Rows[0]["id"]), Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"])); //#13829 - Update before GetBookingData 

                            var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking, deliveryProcess: deliveryProcess);        // #13651
                            new Chub().SubmitMany(bookings, conn, trans);

                            DFACTTrans ftrans = new DFACTTrans();

                            ftrans.AccountNumber = sched.AccountNumber;
                            ftrans.AgreementNumber = sched.AgreementNumber;
                            ftrans.BuffNo = sched.BuffNo;
                            ftrans.ItemNumber = sched.ItemNumber;
                            ftrans.Price = (double)sched.RetVal / sched.Quantity;
                            ftrans.Quantity = sched.Quantity;
                            ftrans.Value = sched.RetVal;
                            ftrans.StockLocation = sched.StockLocation;
                            ftrans.TaxAmt = Convert.ToDouble(row[CN.TaxAmt]);
                            ftrans.TCCode = "61";
                            ftrans.TranDate = DateTime.Now;
                            ftrans.TranType = "01";

                            ftrans.Save(conn, trans);


                            if (serviceRequestNo != null)       //#11989
                            {
                                new ServiceRepository().UpdateReplacementActioned(conn, trans, accountNo, Convert.ToInt32(serviceRequestNo), Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row[CN.StockLocn]), Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]));
                            }

                        }

                        // CR784 - if the warranty has been fulfilled for the item being
                        // replaced, the link needs to broken by updating the parent
                        // itemno to "", and the parent location to 0.
                        //if ((string)row[CN.WarrantyFulfilled] == "Y")             //#17678 - Need Replacement to go through here regardless of warranty being fullfilled. Procedure will check if warranty fullfilled and break link to item if necessary
                        //{
                        del.WarrantyFulFilled(conn, trans, sched.BuffNo, sched.AccountNumber,
                            sched.AgreementNumber, sched.ItemID,        // RI //sched.ItemNumber, 
                            Convert.ToInt16(row[CN.StockLocn]), false, Math.Abs(Convert.ToInt32(Quantity)), (string)row[CN.DelOrColl], (string)row[CN.WarrantyFulfilled]); //#17678
                        //}
                    }

                    li.ProcessReplacement(conn, trans, sched.AccountNumber,
                        //sched.AgreementNumber, sched.ItemNumber, 
                        sched.AgreementNumber, sched.ItemID,                                        //IP - 09/06/11 - CR1212 - RI
                        (short)Convert.ToInt32(row[CN.StockLocn]), Math.Abs(sched.Quantity),
                        user, contractNo, sched.ReturnStockLocation, false);

                    DServiceRequest serviceRequest = new DServiceRequest();
                    serviceRequest.SaveResolutionStatus(conn, trans, sched.AccountNumber, sched.ItemID, //IP - 22/07/11 - RI - sched.ItemNumber, - changed to use ItemID
                                                        sched.StockLocation, ServiceReplacementStatus.Authorised);
                }
                else
                {
                    if ((string)row[CN.ItemType] == "S")
                    {
                        if ((string)row[CN.DelOrColl] != "I")       // #17290 - not for Instant Replacements
                        {
                            string auditSource = string.Empty;
                            if ((string)row[CN.DelOrColl] == "C")
                                auditSource = AS.GRTCancel;
                            else
                                auditSource = AS.GRTExchange;

                            collection = true;

                            //CR615 - Update order quantity for stock item and any associated 
                            //discounts as part of the collection process from the
                            //Goods Return screen.
                            //This is to ensure items are not scheduled and delivery notes are not 
                            //printed, by user entering the Revise Agreement screen.

                            // todo uat363 rdb need to get PArentProductCode
                            li.GetSingleItem(conn, trans, sched.StockLocation, sched.ItemID,                    //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID rather than ItemNumber
                                sched.AccountNumber, sched.AgreementNumber, contractNo, parentItemID);          //IP - 17/05/11 - CR1212 - #3627 - Changed to use parentItemID rather than parentItemNo

                            // 67977 RD 22/02/06 Added bfTaxamt to get old taxamt
                            double bfQty = li.Quantity;
                            double newQty = li.Quantity + sched.Quantity;
                            decimal bfValue = Convert.ToDecimal(bfQty) * li.Price;
                            decimal newValue = Convert.ToDecimal(newQty) * li.Price;
                            double bfTaxamt = li.TaxAmount;
                            decimal bfPrice = li.Price;
                            li.AuditSource = auditSource;

                            collectionAmount += Convert.ToDecimal(sched.Quantity) * li.Price;
                            taxAmount += Convert.ToDecimal(row[CN.TaxAmt]);

                            li.User = this.User;
                            if ((string)row[CN.DelOrColl] == "C")
                                li.AuditSource = AS.GRTCancel;
                            else
                                li.AuditSource = AS.GRTExchange;
                            // todo uat363 need to get the parentItemNo here 
                            li.UpdateItemQuantity(conn, trans, sched.AccountNumber, sched.AgreementNumber,
                                sched.ItemID, sched.StockLocation, contractNo, Convert.ToDecimal(newQty), parentItemID);	//IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNumber and ParentItemNo

                            // uat363 rdb todo get parentitemno
                            //70267 LineItem delqty should not be updated at this stage if collection is an Exchange
                            if ((string)row[CN.DelOrColl] != "E")
                            {
                                li.UpdateDelQty(conn, trans, sched.AccountNumber,                        //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNumber and ParentItemNo
                                  sched.StockLocation, sched.AgreementNumber, sched.ItemID,
                                         contractNo, sched.Quantity, parentItemID);
                            }

                            // Save details to the LineItemAudit table to ensure the
                            // correct information is displayed on the Agreement Audit
                            // tab in the Account Details screen.
                            if (li.Quantity != newQty || li.OrderValue != newValue)
                            {
                                lineItem.AccountNumber = sched.AccountNumber;
                                lineItem.AgreementNumber = sched.AgreementNumber;
                                lineItem.User = this.User;
                                lineItem.ItemNumber = sched.ItemNumber;
                                lineItem.ItemId = sched.ItemID;             //RI
                                lineItem.StockLocation = sched.StockLocation;
                                lineItem.ContractNo = contractNo;
                                lineItem.AuditSource = auditSource;
                                if ((string)row[CN.DelOrColl] == "C")
                                    lineItem.AuditSource = AS.GRTCancel;
                                else
                                    lineItem.AuditSource = AS.GRTExchange;

                                // 67977 RD 22/02/06 Added taxamout  recalling getsingleitem in order to get the new value for taxamt
                                // todo uat363 rdb need to get PArentProductCode
                                li.GetSingleItem(conn, trans, sched.StockLocation, sched.ItemID,         //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNumber and ParentItemNo
                                    sched.AccountNumber, sched.AgreementNumber, contractNo, parentItemID);

                                lineItem.UpdateLineItemAudit(conn, trans, bfQty, newQty, bfValue, newValue, bfTaxamt, li.TaxAmount);
                            }
                            //Always save collection 'before image' details to the 
                            //LineItemBfCollection table in case the Collection gets cancelled and
                            //values need to be reversed. (Looks like LineItem_Amend isn't being utilised for this purpose, may be an oversight).
                            lineItem.CreateLineItemBfCollection(conn, trans, bfQty, bfValue, bfPrice);
                        }

                        // CR784 - if the warranty has been fulfilled for the item being
                        // exchanged, the link needs to broken by updating the parent
                        // itemno to "", and the parent location to 0.
                        //if ((string)row[CN.DelOrColl] == "E")
                        //if ((string)row[CN.DelOrColl] != "C")               //#17678
                        //{
                        //exchange = true;  CR2018-013 Allow for all types of GRT
                        del.WarrantyFulFilled(conn, trans, bookingId, sched.AccountNumber,      //#14313
                                sched.AgreementNumber, //sched.ItemNumber,
                                sched.ItemID,        // RI
                                Convert.ToInt16(row[CN.StockLocn]), true, Math.Abs((Convert.ToInt32(sched.Quantity))), (string)row[CN.DelOrColl], (string)row[CN.WarrantyFulfilled]); //#17678
                        //}
                    }
                    else
                    {
                        decimal refund = Convert.ToDecimal(row[CN.Refund]);
                        string refType = (string)row[CN.RefundType];                           //IP - 28/07/11 - RI - #3939
                        //if (refund >= 0)
                        if (refund >= 0 && refType != string.Empty)                             //IP - 28/07/11 - RI - #3939
                        {
                            //Save details of the warranty adjustment to the 
                            //LineItemBfCollection table in case the Collection 
                            //gets cancelled and the adjustment needs to be reversed.
                            lineItem.AccountNumber = sched.AccountNumber;
                            lineItem.AgreementNumber = sched.AgreementNumber;
                            lineItem.User = this.User;
                            lineItem.ItemNumber = (string)row[CN.ItemNo];
                            lineItem.ItemId = Convert.ToInt32(row[CN.ItemId]);                  //IP - 21/01/13 - #12003 - LW75608
                            lineItem.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                            lineItem.ContractNo = (string)row[CN.ContractNo];
                            lineItem.AccountNumber = sched.AccountNumber;

                            li.GetSingleItem(conn, trans, lineItem.StockLocation, lineItem.ItemId,                  //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNumber and ParentItemNo
                                sched.AccountNumber, sched.AgreementNumber, lineItem.ContractNo, parentItemID);

                            if (refund > 0)
                            {
                                lineItem.CreateLineItemBfCollection(conn, trans, 1, refund, refund);
                            }
                            else
                            {
                                lineItem.CreateLineItemBfCollection(conn, trans, 1, li.Price, li.Price);
                            }
                            // CR784 - the unexpired portion of the warranty will be debited
                            // onto the customer's account
                            string type = (string)row[CN.RefundType] == "E" ? TransType.ElecWarrantyRecovery : TransType.FurnWarrantyRecovery;

                            int transRefNo = branch.GetTransRefNo(conn, trans, (short)branchNo);

                            BTransaction t = new BTransaction(conn, trans, accountNo, branchNo,
                            transRefNo, refund, this.User, type,
                            "", "", "", 0, countryCode, DateTime.Now, "", agreementNo);
                        }

                        if (refund == 0 && sched.ContractNo != string.Empty)
                        {
                            sched.AccountNumber = (string)row[CN.AcctNo];

                            sched.InsertIntoIgnoreCRECRF(conn, trans, sched.AccountNumber, sched.ContractNo, sched.StockLocation); //Insert into table here so financial procedure knows to calculate CRE / CRF
                        }

                        // #18554 - increase agreement total by +ve Discount
                        if (Convert.ToBoolean(Convert.ToInt16(row[CN.Discount])) == true && !(AT.IsCreditType(accountType)))
                        {
                            collectionAmount += Convert.ToDecimal(sched.Quantity) * Convert.ToDecimal(row[CN.Price]);
                        }

                        //CR2018-013 allow if refund is more than 0
                        if (refund > 0)
                        {
                            if (Convert.ToString(row["contractno"]) != string.Empty) //Warranties only
                            {

                                sched.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                                sched.ContractNo = (string)row[CN.ContractNo];
                                new WarrantyRepository().CollectGRTWarrantiesWithStock(conn, trans, accountNo, agreementNo, row.Row, this.User); //CR2018 - 013
                                
                            }
                        }//
                    } 
                    

                    
                    li.UpdateNotes(conn, trans, sched.AccountNumber,
                        sched.AgreementNumber, sched.ItemID,                //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID rather than ItemNumber
                        sched.StockLocation, contractNo, notes, true);



                    //69105 - the delnotebranch field in lineitem needs to be updated to the same branch no as the return stock location
                    //70988 goods return for Mauritius
                    if ((string)row[CN.DelOrColl] != "R" || (bool)Country[CountryParameterNames.IdentRepEqualsDNStockBranch]) //not instant replacement or identical replacement dn should be printed at stock branch
                    {
                        //li.UpdateLineItemDelNoteBranch(conn, trans, sched.AccountNumber, sched.ItemNumber, sched.ReturnStockLocation);
                        li.UpdateLineItemDelNoteBranch(conn, trans, sched.AccountNumber, sched.ItemID, sched.ReturnStockLocation);              //IP - 01/08/11 - RI
                    }


                }

                li.UpdateDeliveryArea(conn, trans, sched.AccountNumber, sched.AgreementNumber,
                    sched.ItemID, sched.StockLocation, contractNo, deliveryArea, deliveryProcess);              //IP - 08/06/11 - CR1212 - RI
                //sched.ItemNumber,sched.StockLocation,contractNo, deliveryArea, deliveryProcess);

                li.UpdateRequiredDelDate(conn, trans, sched.AccountNumber, sched.AgreementNumber,
                     sched.ItemID, sched.StockLocation, contractNo, DateTime.Today);                            //IP - 21/07/11 - RI
                //sched.ItemNumber, sched.StockLocation, contractNo, DateTime.Today);

                lastLocation = Convert.ToInt32(row[CN.RetStockLocn]);

                //Annual Service Contract 
                var annualServiceUsed = Convert.ToDecimal(row["AnnualServiceContractUsed"]);

                if (annualServiceUsed > 0)
                {
                    var transtype = AT.IsCreditType(acct.AccountType) ? TransType.AnnualServiceRecoveryCredit : TransType.AnnualServiceRecoveryCash;
                    var transRefNumber = branch.GetTransRefNo(conn, trans, (short)branchNo);

                    BTransaction transaction = new BTransaction(conn, trans, accountNo, branchNo,
                    transRefNumber, annualServiceUsed, this.User, transtype,
                    "", "", "", 0, countryCode, DateTime.Now, "", agreementNo);
                }

            }

            //CR615 26/05/05 RD/DR Updating agreement for stock item that are authorised 
            // for collection only as the lineitem is udpated to remove the item.
            DAgreement agreement = new DAgreement(conn, trans, accountNo, agreementNo);

            if (collection)
            {

                agreement.EmployeeNumChange = this.User;

                //LiveWire Call 68394
                //Need to save the agreemnt total before it is updated to make sure
                //the correct agreement total is used when the account is cancelled.
                // if (!exchange)      //CR2018-013 Allow for all types of GRT
                agreement.AgrmtTotalBFCollection(conn, trans);

                agreement.AgreementTotal += collectionAmount;
                agreement.CashPrice += collectionAmount;

                if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                {
                    agreement.AgreementTotal -= taxAmount;
                    agreement.CashPrice -= taxAmount;
                }

                agreement.AuditSource = lineItem.AuditSource;
                if (!(AT.IsCreditType(acct.AccountType) && collection))
                {
                    agreement.DateChange = DateTime.Now;                    //#14392
                    agreement.Save(conn, trans);
                }
                // Saving the agreement on the acct
                acct.AgreementTotal = agreement.AgreementTotal;
                acct.Save(conn, trans);
            }

            //#16208 Collection of standalone waranties
            foreach (DataRow dr in ds.Tables[TN.NonStockList].Rows)
            {
                if (Convert.ToString(dr["contractno"]) != string.Empty && Convert.ToBoolean(dr["ReadyAssist"]) == false) //#18607 - CR15594 //Warranties only
                {
                    decimal refund = Convert.ToDecimal(dr[CN.Refund]);
                    sched.StockLocation = (short)Convert.ToInt32(dr[CN.StockLocn]);
                    sched.ContractNo = (string)dr[CN.ContractNo];

                    if (refund == 0)
                    {
                        sched.InsertIntoIgnoreCRECRF(conn, trans, accountNo, sched.ContractNo, sched.StockLocation); //Insert into table here so financial procedure knows to calculate CRE / CRF
                    }

                    new WarrantyRepository().CollectGRTWarranties(conn, trans, accountNo, agreementNo, dr, this.User);          // #17692
                }

            }

            // UAT 3 - COLLECT or REPLACE warranties separate to their stock items
            BDelivery nonStockDelivery = new BDelivery();
            nonStockDelivery.User = this.User;
            // This will also save the agreement and the account
            nonStockDelivery.CollectNonStockOnly(conn, trans, countryCode, branchNo,
                            this.User, accountNo, agreementNo, ds.Tables[TN.NonStockList],
                            collection, lineItem.AuditSource, agreement);



        }

        public void DeleteSchedule(SqlConnection conn, SqlTransaction trans, DataSet ds)
        {
            DSchedule sched = new DSchedule();

            foreach (DataRow row in ds.Tables["Schedules"].Rows)
            {
                sched.AccountNumber = (string)row["acctno"];
                sched.AgreementNumber = Convert.ToInt32(row["agrmtno"]);
                sched.ItemNumber = (string)row["itemno"];
                sched.StockLocation = (short)Convert.ToInt32(row["stocklocn"]);
                sched.BuffNo = 0;
                sched.BuffBranchNo = 0;

                sched.DeleteSchedule(conn, trans, false);
            }
        }

        public void DeleteSingleSchedule(SqlConnection conn, SqlTransaction trans, string acctNo,
            int agrmtNo, int itemID, short stockLocn, int buffNo, int buffBranchNo) //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
        {
            DSchedule sched = new DSchedule();

            sched.AccountNumber = acctNo;
            sched.AgreementNumber = agrmtNo;
            //sched.ItemNumber = itemNo;
            sched.ItemNumber = string.Empty;            //IP/NM - 18/05/11 -CR1212 - #3627
            sched.ItemID = itemID;                      //IP/NM - 18/05/11 -CR1212 - #3627 - use itemID rather than itemNo
            sched.StockLocation = stockLocn;
            sched.BuffNo = buffNo;
            sched.BuffBranchNo = buffBranchNo;
            sched.DeleteSchedule(conn, trans, true);
        }

        public DataSet GetCollections(short stockLocn)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.StockLocation = stockLocn;
            sched.GetCollections();
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public DataSet GetScheduledForAccount(SqlConnection conn, SqlTransaction trans, string acctNo, bool deleteDelLoad)
        {
            DSchedule sched = new DSchedule();
            DDeliveryLoad del = new DDeliveryLoad();
            DataSet ds = new DataSet();

            sched.AccountNumber = acctNo;
            sched.GetScheduledForAccount();

            if (deleteDelLoad)
            {
                foreach (DataRow row in sched.Schedules.Rows)
                {
                    short buffLocn = (short)row[CN.RetStockLocn];
                    if (buffLocn == 0) buffLocn = (short)row[CN.StockLocn];
                    del.DeleteByBuffNo(conn, trans, buffLocn, (int)row[CN.BuffNo]);
                }
            }

            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public void ChangeReqDeliveryDate(SqlConnection conn, SqlTransaction trans,
                                            string acctNo, int agrmtNo, int itemId,
                                            string contractNo, short stockLocn,
                                            DateTime reqDeliveryDate, int buffNo,
                                            string reason)
        {
            DDeliveryLoad dLoad = new DDeliveryLoad();
            dLoad.User = this.User;
            dLoad.ChangeReqDeliveryDate(conn, trans, acctNo, agrmtNo, itemId, contractNo,
                                        stockLocn, reqDeliveryDate, buffNo, reason);
        }

        public void RemoveDeliveryNote(SqlConnection conn, SqlTransaction trans,
            short stockLocn, int buffNo, int empeeNo, string reason)
        {
            DDeliveryLoad dLoad = new DDeliveryLoad();
            //IP - UAT351 - 01/11/07
            BBranch branch = new BBranch();
            DSchedule sched = new DSchedule();

            //IP - UAT351 - 01/11/07
            int newBuffNo = 0;

            dLoad.RemoveDeliveryNote(conn, trans, stockLocn, buffNo, empeeNo, reason);

            //IP - UAT351 - 01/11/07
            //If the country parameter is true then assign a new buffno to each of the items on the
            //delivery note that has been removed.
            if ((bool)Country[CountryParameterNames.CancelDelNoteIfFailed])
            {
                //Retrieve the next buffno
                newBuffNo = branch.GetBuffNo(conn, trans, stockLocn);

                //retrieve the items on the selected delivery note
                sched.GetDeliveryNotes(conn, trans, buffNo, stockLocn);
                DataTable delnote = sched.Schedules;

                foreach (DataRow d in delnote.Rows)
                {
                    ScheduleAssignNewBufferNo(conn, trans, Convert.ToString(d[CN.AcctNo]), Convert.ToInt32(d[CN.ItemId]), stockLocn, buffNo, newBuffNo);
                }
            }
        }


        public void RemoveLineItem(SqlConnection conn, SqlTransaction trans,
            short stockLocn, int buffNo, string acctNo, int agrmtNo,
            int itemId, int empeeNo, string reason)
        {
            DDeliveryLoad dLoad = new DDeliveryLoad();
            dLoad.RemoveDeliveryNoteItem(conn, trans,
                stockLocn, buffNo, acctNo, agrmtNo, itemId, empeeNo, reason, false);
        }

        public void ChangeQuantity(SqlConnection conn, SqlTransaction trans,
            double newQuantity, short stockLocn, int buffNo,
            string acctNo, int agrmtNo, int itemID, short curBranchNo, int parentItemId)                      //IP - 07/06/11 - CR1212 - RI
        {
            DSchedule sched = new DSchedule();

            // Change the quantity on the existing delivery note
            sched.GetScheduleItem(conn, trans,
                stockLocn, buffNo, acctNo, agrmtNo, itemID, stockLocn);                     //IP - 07/06/11 - CR1212 - RI
            double oldQuantity = sched.Quantity;
            sched.Quantity = newQuantity;
            sched.ParentItemID = parentItemId;
            sched.Write(conn, trans, this.User);

            // Add the item again for the remaining quantity
            DBranch branch = new DBranch();
            if (sched.ReturnStockLocation != 0)
            {
                sched.BuffNo = branch.GetBuffNo(conn, trans, sched.ReturnStockLocation);
            }
            else
            {
                sched.BuffNo = branch.GetBuffNo(conn, trans, sched.StockLocation);
            }
            sched.BuffBranchNo = curBranchNo;
            sched.Quantity = oldQuantity - newQuantity;
            sched.DateDelPlan = Date.blankDate;
            sched.VanNo = "";
            sched.LoadNo = 0;
            sched.User = 0;
            sched.ParentItemID = parentItemId;

            if (sched.UndeliveredFlag.Equals(String.Empty))
                sched.UndeliveredFlag = "A";
            else
            {
                char tmpChar = Convert.ToChar(sched.UndeliveredFlag);
                if (!sched.UndeliveredFlag.Equals("Z"))
                    sched.UndeliveredFlag = ((char)Convert.ToInt32(tmpChar + 1)).ToString();
            }

            sched.Write(conn, trans, this.User);
        }

        public DataSet GetPickListSchedule(int filter, int PickListNo, int BuffNo)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetPickListSchedule(filter, PickListNo, BuffNo);
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public DataSet GetTransPickListDetails(short branchNo, int transPickickListNo)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetTransPickListDetails(branchNo, transPickickListNo);
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public DataSet GetRevisedSchedules(short branchNo, int loadNo, int pickNo,
            DateTime reviseFrom, DateTime reviseTo, out DateTime timeLocked)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();
            sched.User = this.User;

            sched.GetRevisedSchedules(branchNo, loadNo, pickNo, reviseFrom, reviseTo,
                                        out timeLocked);
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public DataSet GetRevisedScheduleDetails(string acctNo, int buffNo)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetRevisedScheduleDetails(acctNo, buffNo);
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public DataSet GetRevisedScheduleChanges(string acctNo, int buffNo, int itemID, short locn)     //IP - 18/06/11 - CR1212 - RI - #4042
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetRevisedScheduleChanges(acctNo, buffNo, itemID, locn);                              //IP - 18/06/11 - CR1212 - RI - #4042
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        public void ConfirmScheduleChanges(SqlConnection conn, SqlTransaction trans,
            short loadNo, int pickListNo, int pickListBranch, string acctNo, int agrmtno,
            //string itemNo, short locn, int buffNo, int origBuffNo, string removal, 
            int itemID, short locn, int buffNo, int origBuffNo, string removal,                     //IP - 18/06/11 - CR1212 - RI - #4042
                                                                                                    //string origItemNo, int tranSchedNo, int tranSchedNoBranch)
            int origItemID, int tranSchedNo, int tranSchedNoBranch)                                 //IP - 20/06/11 - CR1212 - RI - #4042
        {
            DSchedule sched = new DSchedule();
            sched.User = this.User;
            sched.ConfirmScheduleChanges(conn, trans, loadNo, pickListNo, pickListBranch,
                            //acctNo, agrmtno, itemNo, locn, buffNo, origBuffNo, removal, 
                            acctNo, agrmtno, itemID, locn, buffNo, origBuffNo, removal,              //IP - 18/06/11 - CR1212 - RI - #4042
                                                                                                     //origItemNo, tranSchedNo, tranSchedNoBranch);
                            origItemID, tranSchedNo, tranSchedNoBranch);                            //IP - 20/06/11 - CR1212 - RI - #4042
        }

        public DataSet GetAdditionalItems(string acctNo, short loadNo, int pickListNo, int pickListBranch)
        {
            DataSet ds = new DataSet();
            DSchedule sched = new DSchedule();

            sched.GetAdditionalItems(acctNo, loadNo, pickListNo, pickListBranch);
            ds.Tables.Add(sched.Schedules);
            return ds;
        }

        //IP - 28/11/2007 - 69360
        //Method returns 'True' or 'False' when checking for scheduled records for an account.
        //Returns true if records are found.
        public bool AccountScheduleExists(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DSchedule sched = new DSchedule();
            bool hasSchedule = sched.AccountScheduleExists(conn, trans, accountNo);
            return hasSchedule;
        }

        //IP - 18/02/09 - CR929 & CR974 - Deliveries - Method to delete items from the account
        //that were on a delivery note that was deleted from Delivery Notification.
        //Recalculates Service charge.
        public void DeleteDeliveryNoteAndItems(SqlConnection conn, SqlTransaction trans,
            short branchNo, string countryCode, DataTable dnItemsToDelete, DataTable dnItemsLinkedNonStocks,
            short stockLocn, int buffNo, int empeeNo, string reason, ref bool allItemsCollected)
        {
            DBranch branch = new DBranch();
            BDelivery delivery = new BDelivery();
            DFinTrans fin = new DFinTrans();
            DSundryCharge sundry = new DSundryCharge();

            allItemsCollected = false;

            string acctNo = string.Empty;
            int agrmtNo = 0;
            int transRefNo = branch.GetTransRefNo(conn, trans, branchNo);

            decimal adminPrice = 0;
            decimal cashPrice = 0;
            decimal charge = 0;
            decimal transValue = 0;

            DDeliveryLoad dLoad = new DDeliveryLoad();

            //Firstly call the method to remove the items on the DN from the account
            RemoveItemsAndNonStock(conn, trans, dnItemsToDelete, reason);

            //If there are any NonStocks (warranties) linked to the items which are being removed
            //then we also need to remove these aswell.

            if (dnItemsLinkedNonStocks.Rows.Count > 0)
            {
                RemoveItemsAndNonStock(conn, trans, dnItemsLinkedNonStocks, reason);
            }

            acctNo = Convert.ToString(dnItemsToDelete.Rows[0][CN.AcctNo]);
            agrmtNo = Convert.ToInt32(dnItemsToDelete.Rows[0][CN.AgrmtNo]);

            DAccount account = new DAccount(conn, trans, acctNo);
            DAgreement agreement = new DAgreement(conn, trans, acctNo, agrmtNo);

            //Saves an audit record for the schedule removal
            //Deletes the DN from the Delivery load
            //Adds the reason for deleting the DN to the BailAction table.
            dLoad.DeleteDeliveryNoteAndItems(conn, trans, stockLocn, buffNo, empeeNo, reason);

            agreement.AuditSource = AS.DeliveryNotification;
            //Recalculate the Service Charge if this is a Credit account.
            if (AT.IsCreditType(account.AccountType))
            {
                // Recalculate Service Charge using new OrdVal amounts
                // This expects the agreement cash price to exclude service charges
                decimal chargeableAdminPrice = 0;
                agreement.CashPrice = account.GetChargeableCashPrice(conn, trans, acctNo, ref chargeableAdminPrice);
                // This will save the agreement with a new total
                delivery.User = empeeNo;
                delivery.RecalculateServiceCharge(conn, trans, account, agreement);
            }
            else
            {
                // Save new Agreement Total
                agreement.EmployeeNumChange = empeeNo;
                agreement.DateChange = DateTime.Now;                //#14392
                agreement.Save(conn, trans);
                account.AgreementTotal = agreement.AgreementTotal;
                account.Save(conn, trans);
            }

            // Delivery
            // Deliver non-stock items on this account
            // This will deliver new DT value, INS, ADM, STAX if there are any differences
            delivery.DeliverNonStocks(conn, trans, acctNo, account.AccountType,
                countryCode, branchNo, transRefNo, ref transValue, agrmtNo);

            //Need this as outstanding balance will be reduced on the 'Acct' table.
            // Fintrans delivery record for all items
            BTransaction t = null;
            if (Math.Abs(transValue) > 0)
                t = new BTransaction(conn, trans, acctNo, branchNo,
                    transRefNo, transValue, empeeNo,
                    TransType.Delivery, "", "", "", 0, countryCode,
                    DateTime.Today, FootNote.NonStockCollection, agrmtNo);

            //If the  accounts 'CashPrice' is 0, or the 'CashPrice' equals the sundrycharges
            //and there are no warrany adjustments on the account then proceed to 
            //cancel the account.
            sundry.GetSundryChargeItem(conn, trans, account.AccountType);
            DataTable sundryTable = sundry.Items;

            if (sundryTable.Rows.Count > 0)
            {
                foreach (DataRow r in sundryTable.Rows)
                    charge = Convert.ToDecimal(r[CN.Amount]);
            }

            cashPrice = account.GetChargeableCashPrice(conn, trans, acctNo, ref adminPrice);


            if ((fin.GetWarrantyAdjustment(conn, trans, acctNo) == 0 &&
                (cashPrice == charge || cashPrice == 0)))
            {
                allItemsCollected = true;
            }
        }

        //IP  -21/02/09 - CR929 & 974 - Deliveries
        //Method to retrieve warranties linked to the items that are being deleted 
        //on the Delivery Note.
        public DataTable GetNonStockLinkedToDNItems(SqlConnection conn, SqlTransaction trans,
            short stockLocn, int buffNo)
        {
            DDeliveryLoad dLoad = new DDeliveryLoad();
            DataTable _dnItemsLinkedNonStocks = new DataTable();

            dLoad.GetNonStockLinkedToDNItems(conn, trans, stockLocn, buffNo);
            _dnItemsLinkedNonStocks = dLoad.DNItemsLinkedNonStocks;

            return _dnItemsLinkedNonStocks;
        }

        //IP - 21/02/09 - CR929 & 974 - Method to remove the stock items and any linked
        //NonStocks which were on a Delivery Note which is to be deleted.
        public void RemoveItemsAndNonStock(SqlConnection conn, SqlTransaction trans,
            DataTable dt, string reason)
        {
            string auditSource = AS.DeliveryNotification;
            decimal collectionAmount = 0;
            decimal taxAmount = 0;
            string acctNo = string.Empty;
            int agrmtNo = 0;
            short stockLocn = 0;
            string itemNo = string.Empty;
            int itemID = 0;                         //IP - 17/05/11 - CR1212 - #3627
            string contractNo = string.Empty;
            string parentItemNo = string.Empty;
            int parentItemID = 0;                   //IP - 17/05/11 - CR1212 - #3627         
            string notes = reason;

            DLineItem li = new DLineItem();
            BItem lineItem = new BItem();

            foreach (DataRow dr in dt.Rows)
            {
                acctNo = Convert.ToString(dr[CN.AcctNo]);
                agrmtNo = Convert.ToInt32(dr[CN.AgrmtNo]);
                stockLocn = Convert.ToInt16(dr[CN.StockLocn]);
                itemNo = Convert.ToString(dr[CN.ItemNo]);
                itemID = Convert.ToInt32(dr[CN.ItemId]);                 //IP - 17/05/11 - CR1212 - #3627
                contractNo = Convert.ToString(dr[CN.ContractNo]);
                parentItemNo = Convert.ToString(dr[CN.ParentItemNo]);
                parentItemID = Convert.ToInt32(dr[CN.ParentItemId]);    //IP - 17/05/11 - CR1212 - #3627

                //Retrieve line details
                li.GetSingleItem(conn, trans, stockLocn, itemID, acctNo, agrmtNo, contractNo, parentItemID);    //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo

                double bfQty = li.Quantity;
                double newQty = 0;
                decimal bfValue = Convert.ToDecimal(bfQty) * li.Price;
                decimal newValue = Convert.ToDecimal(newQty) * li.Price;
                double bfTaxamt = li.TaxAmount;
                decimal bfPrice = li.Price;
                li.AuditSource = auditSource;

                //Store the total of the value of the items and the tax if any which will later be deducted from
                //the agreement total.
                collectionAmount += -bfValue;
                taxAmount += Convert.ToDecimal(bfTaxamt);

                li.User = this.User;

                //Proceed to update the quantity, ordval on the lineitem table for the item.
                li.UpdateItemQuantity(conn, trans, acctNo, agrmtNo, itemID, stockLocn, contractNo, Convert.ToDecimal(newQty), parentItemID); //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo

                if (li.Quantity != newQty || li.OrderValue != newValue)
                {
                    lineItem.AccountNumber = acctNo;
                    lineItem.AgreementNumber = agrmtNo;
                    lineItem.User = this.User;
                    lineItem.ItemNumber = itemNo;
                    lineItem.ItemId = itemID;
                    lineItem.StockLocation = stockLocn;
                    lineItem.ContractNo = contractNo;
                    lineItem.AuditSource = auditSource;

                    li.GetSingleItem(conn, trans, stockLocn, itemID, acctNo, agrmtNo, contractNo, parentItemID); //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID and parentItemID rather than itemNo and parentItemNo

                    //If this is a warranty, then we need to pass the ParentItemNumber and ParentStockLocation
                    //to the LineItemAuditUpdate.
                    if (li.ItemType == "N")
                    {
                        lineItem.ParentItemNumber = li.ParentItemNumber;
                        lineItem.ParentItemId = li.ParentItemID;
                        lineItem.ParentStockLocation = li.ParentStockLocation;
                    }

                    lineItem.UpdateLineItemAudit(conn, trans, bfQty, newQty, bfValue, newValue, bfTaxamt, li.TaxAmount);
                }

                li.UpdateNotes(conn, trans, acctNo, agrmtNo, itemID, stockLocn, contractNo, notes, true); //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID rather than itemNo 
            }

            //Retrieve the agreement details for the account.
            DAccount acct = new DAccount(conn, trans, acctNo);
            DAgreement agreement = new DAgreement(conn, trans, acctNo, agrmtNo);
            agreement.EmployeeNumChange = this.User;

            //Reduce the AgreementTotal and CashPrice by the value of the items we are deleting from the Delivery Note.
            agreement.AgreementTotal += collectionAmount;
            agreement.CashPrice += collectionAmount;

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
            {
                agreement.AgreementTotal -= taxAmount;
                agreement.CashPrice -= taxAmount;
            }

            //If there are Non Stocks linked to the items on the DN which we are deleting then we need to 
            //delete these Non Stocks

            agreement.AuditSource = lineItem.AuditSource;
            agreement.DateChange = DateTime.Now;                //#14392
            agreement.Save(conn, trans);
            // Saving the agreement on the acct
            acct.AgreementTotal = agreement.AgreementTotal;
            acct.Save(conn, trans);

        }

        //BOC Added by Suvidha - CR 2018-13 - 15/04/19 - to update data for GRT.
        public void UpdateInvoiceVersionForGRT(SqlConnection conn, SqlTransaction trans, string accountNo, int agreementNo, DataTable dtReturnItems)
        {
            DSchedule sched = new DSchedule();
            foreach (DataRow dr in dtReturnItems.Rows)
            {               
                string orig_item_no = Convert.ToString(dr["ItemNo"]);
                int returnQuantity = Convert.ToInt32(dr["Quantity"]);
                string retItemNo = Convert.ToString(dr["RetItemNo"]);
                Decimal retVal = Convert.ToDecimal(dr["RetVal"]);
                string contractNo = Convert.ToString(dr["contractno"]);
                int parentitemID = Convert.ToInt32(dr["ParentitemID"]);
                int lineitemID = Convert.ToInt32(dr["lineitemID"]);
                Decimal ordVal = Convert.ToDecimal(dr["OrdVal"]);
                Decimal taxAmt = Convert.ToDecimal(dr["TaxAmt"]);
                sched.UpdateInvoiceVersionForGRT(conn, trans, accountNo, agreementNo, orig_item_no, returnQuantity, retItemNo, retVal, contractNo, parentitemID, lineitemID, ordVal, taxAmt);

            }
        }
        //EOC

    }
}
