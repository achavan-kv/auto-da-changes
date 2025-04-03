using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Blue.Cosacs;
using Blue.Cosacs.Messages.Warehouse;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared.Extensions;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.AuditSource;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Delivery;
using STL.Common.Constants.Elements;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;
using STL.Common.ServiceRequest;
using STL.Common.Static;
using STL.DAL;
using STL.BLL.WebApi;
namespace STL.BLL
{
    /// <summary>
    /// Summary description for BDelivery.
    /// </summary>
    public class BDelivery : CommonObject
    {
        public BDelivery()
        {
        }

        public int BuffNo
        {
            get { return _buffno; }
        }
        private int _buffno = 0;

        private bool _redelivery = false;
        public bool Redelivery
        {
            get { return _redelivery; }
            set { _redelivery = value; }
        }

        private bool _useExistingTransaction = false;
        public bool UseExistingTransaction
        {
            get { return _useExistingTransaction; }
            set { _useExistingTransaction = value; }
        }


        private bool _warrantyremoved = false;
        public bool WarrantyRemoved
        {
            get { return _warrantyremoved; }
            set { _warrantyremoved = value; }
        }

        //#14772
        private DateTime _datedel = DateTime.Today;
        public DateTime DateDel
        {
            get {return _datedel;}
            set{ _datedel = value;}
        }

        public bool isReplacement = false;


        private DataTable associatedItems = new DataTable();

        public int GetBuffNo(SqlConnection conn, SqlTransaction trans, short branchNo)
        {
            DBranch branch = new DBranch();
            return branch.GetBuffNo(conn, trans, branchNo);
        }

        public DataSet DeliveryNotesByLoadNo(short branchNo, int loadNo, DateTime dateDel, out DateTime minDelDate, out string[] acctExceedsAgr)
        {
            DDeliveryLoad dLoad = new DDeliveryLoad();

            DataSet deliveryNoteSet = dLoad.LoadByLoadNo(branchNo, loadNo, dateDel, out minDelDate);

            //IP - 04/02/08 - Livewire: (69454)
            //Pull out the datatable 'Schedules' and pass to 'ValidateDeliveryAccounts'
            DataTable schedules = deliveryNoteSet.Tables[TN.Schedules];

            //Pass the data table 'Schedules' to validate the accounts for delivery, to check 
            //if any accounts delivery total exceeds the agreement total.
            string[] acctDelTotExceedsAgr = ValidateDeliveryAccounts(schedules);

            //Assign the array that holds the failed accounts to the out parameter 'out string[] acctExceedsAgr' 
            acctExceedsAgr = acctDelTotExceedsAgr;

            return deliveryNoteSet;
        }

        //IP - 04/02/08 - Livewire (69454) Method that checks if accounts delivery totals exceeds their
        //agreement totals and stores the account numbers in an array.
        private string[] ValidateDeliveryAccounts(DataTable schedules)
        {
            SqlConnection conn = null;
            SqlTransaction trans = null;

            //Pass the datatable to 'GetDistinctRows'
            DataRow[] disAccts = GetDistinctRows(schedules, CN.acctno);

            // Call the 'DelExceedsAgrTotal' method for each datarow returned, 
            //which holds the account number and store the accounts where the
            //delivery total is greater than the agreement total in an array.
            string[] objArray = null;
            List<string> distinctRows = new List<string>();

            foreach (DataRow row in disAccts)
            {
                string acctno = Convert.ToString(row[CN.acctno]);
                string buffno = Convert.ToString(row[CN.BuffNo]);
                int qty = Convert.ToInt16(row[CN.Quantity]);    // jec 09/07/08 UAT474
                // Do not check if Delivery exceeds Agreement if qty < 0 i.e collection
                // Agreement total has already been reduced by Goods Return
                if (qty > 0)        // jec 09/07/08 UAT474
                {
                    if (this.DelExceedsAgrTotal(conn, trans, acctno))
                    {
                        distinctRows.Add(acctno + " " + "Del Note:" + " " + buffno);
                    }
                }

            }

            // Poplate a new datarow array with any accounts that fail and return
            objArray = new string[distinctRows.Count];
            distinctRows.CopyTo(objArray);

            return objArray;
        }


        //IP - 04/02/08 - Livewire (69454)
        //Method that loops through the data table and selects the 'distinct'
        //accounts.
        public DataRow[] GetDistinctRows(DataTable drows, string colName)
        {
            Hashtable hTable = new Hashtable();
            List<DataRow> distinctRows = new List<DataRow>();

            DataRow[] objArray = null;
            if (drows != null && colName != null && colName != string.Empty)
            {
                //Hashtable hTable = new Hashtable();

                foreach (DataRow drow in drows.Rows)
                    //if (!hTable.ContainsKey(drow[colName].ToString()))
                    if (distinctRows.Find(delegate(DataRow tempRow) { return drow[colName].ToString() == tempRow[colName].ToString(); }) == null)
                    {
                        //hTable.Add(drow[colName].ToString(), string.Empty);
                        distinctRows.Add(drow);
                    }
            }

            objArray = new DataRow[distinctRows.Count];
            distinctRows.CopyTo(objArray);

            return objArray;
        }

        public DataSet GetDeliveryOrders(DateTime fromDate,
            DateTime toDate,
            string deliveryArea,
            int includeDeliveries,
            int includeCollections,
            int includeAddresses,
            int includeLinkedItems,
            string deliveryProcess,
            string majorProductCategory,
            string minorProductCategory,
            string acctNo,
            int user,
            int branchNo,
            int delNotBranchNo,
            bool reqDelSearch,
            bool includeAssembly,
            bool includeNonAssembly,
            out DateTime timeLocked)
        {
            DDelivery delivery = new DDelivery();
            DataSet ordersSet = delivery.GetDeliveryOrders(fromDate,
                toDate,
                deliveryArea,
                includeDeliveries,
                includeCollections,
                includeAddresses,
                includeLinkedItems,
                deliveryProcess,
                majorProductCategory,
                minorProductCategory,
                acctNo,
                user,
                branchNo,
                delNotBranchNo,
                reqDelSearch,
                includeAssembly,
                includeNonAssembly,
                out timeLocked);
            if (ordersSet.Tables.Count > 0)
            {
                ordersSet.Tables[0].TableName = TN.Deliveries;
            }
            return ordersSet;
        }

        public DataSet GetDeliveryNotes(string acctNo,
            int user,
            bool collectionsOnly,
            out DateTime timeLocked)
        {
            DDelivery delivery = new DDelivery();
            DataSet ordersSet = delivery.GetDeliveryNotes(
                acctNo,
                user,
                collectionsOnly,
                out timeLocked);
            if (ordersSet.Tables.Count > 0)
            {
                ordersSet.Tables[0].TableName = TN.Deliveries;
            }
            return ordersSet;
        }

        public decimal ReplacementDelivery(SqlConnection conn, SqlTransaction trans,
            string accountNo, int agreementNo,
            decimal quantity, short branchNo, int user,
            int refNo, int buffNo, string contractNo,
            string itemNo,
            int itemId, short stockLocation,
            decimal unitPrice, int returnItemId,
            short returnStockLocn, int parentItemId = 0)  //#18575
        {
            DDelivery del = new DDelivery();

            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = agreementNo;
            del.DateDelivered = DateTime.Today;
            del.DeliveryOrCollection = quantity < 0 ? DelType.Collection : DelType.Normal;
            del.ItemNumber = itemNo;
            del.ItemId = itemId;
            del.StockLocation = stockLocation;
            del.Quantity = Convert.ToDouble(quantity);
            del.BuffNo = buffNo;
            del.BuffBranchNumber = branchNo;
            del.DateTrans = DateTime.Today;
            del.BranchNumber = branchNo;
            del.TransRefNo = refNo;
            del.TransValue = Math.Round(unitPrice * quantity, 2);
            del.RunNumber = 0;
            del.ContractNo = contractNo;
            if (del.DeliveryOrCollection == DelType.Collection)
            {
                del.ReturnItemId = returnItemId == 0 ? del.ItemId : returnItemId;
                del.ReturnStockLocation = returnStockLocn == 0 ? del.StockLocation : returnStockLocn;
                del.ReturnValue = (double)del.TransValue;
            }
            del.ftNotes = "DNRE";
            del.NotifiedBy = user;
            del.ParentItemId = parentItemId;   //#18575
            del.Write(conn, trans);

            return unitPrice * quantity;
        }

        public void CashAndGoDelivery(SqlConnection conn, SqlTransaction trans,
            string accountNo, double quantity,
            decimal lineValue, short branchNo,
            int user, XmlNode item, int refNo,
            int buffNo, int agreementNo, string parentItemNo, int parentItemId)         // RI jec 21/06/11
        {
            DDelivery del = new DDelivery();
            DLineItem dLineItem = new DLineItem();  //#17290
            BAccount bAccount = new BAccount();

            bAccount.Populate(conn, trans, accountNo);

            if (Convert.ToBoolean(item.Attributes[Tags.ReplacementItem].Value) == true) //#17290
            {
                dLineItem.UpdateDelQty(conn, trans, accountNo, branchNo, agreementNo, Convert.ToInt32(item.Attributes[Tags.ItemId].Value), item.Attributes[Tags.ContractNumber].Value, quantity, parentItemId);  //#17290
            }

            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = agreementNo;
            del.DateDelivered = DateTime.Today;
            del.DeliveryOrCollection = quantity < 0 ? DelType.Collection : DelType.Normal;
            del.ItemNumber = item.Attributes[Tags.Code].Value;
            del.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
            del.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
            del.Quantity = quantity;
            del.BuffNo = buffNo;
            del.BuffBranchNumber = branchNo;
            del.DateTrans = DateTime.Today;
            del.BranchNumber = branchNo;
            del.TransRefNo = refNo;
            // Cannot calculate the value for some special accounts with aggregated STAX lines
            //del.TransValue = Math.Round(Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value) * (decimal)quantity, 2);
            del.TransValue = lineValue;
            del.RunNumber = 0;
            del.ContractNo = item.Attributes[Tags.ContractNumber].Value;
            if (del.DeliveryOrCollection == DelType.Collection)
            {
                del.ReturnItemNumber = item.Attributes[Tags.ReturnItemNo].Value;
                del.ReturnStockLocation = Convert.ToInt16(item.Attributes[Tags.ReturnLocation].Value);
                del.ReturnValue = (double)del.TransValue;
            }
            del.ftNotes = "DNCG";
            del.NotifiedBy = user;
            // uat363 parentItemNo
            del.ParentItemNo = parentItemNo;
            del.ParentItemId = parentItemId;
            del.Write(conn, trans);
        }

        public BDelivery(SqlConnection conn, SqlTransaction trans, string accountNo, double quantity, short branchNo, int user, XmlNode item, int refNo,
            int buffNo, int agreementNo)
            : this(conn, trans, accountNo, quantity, branchNo, user, item, refNo,
                buffNo, agreementNo, DateTime.Today)
        {

        }

        public BDelivery(SqlConnection conn, SqlTransaction trans, string accountNo, double quantity, short branchNo, int user, XmlNode item, int refNo,
            int buffNo, int agreementNo, DateTime dateDelivered)
        {
            DBranch branch = new DBranch();
            DDelivery del = new DDelivery();

            short locn = (short)Convert.ToInt16(item.Attributes[Tags.ReturnLocation].Value);
            if (locn == 0) locn = (short)Convert.ToInt16(item.Attributes[Tags.Location].Value);

            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = agreementNo;
            del.DateDelivered = dateDelivered;
            del.DeliveryOrCollection = quantity < 0 ? DelType.Collection : DelType.Normal;
            del.ItemNumber = item.Attributes[Tags.Code].Value;
            del.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
            del.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
            del.Quantity = quantity;
            if (buffNo == 0)
                _buffno = del.BuffNo = branch.GetBuffNo(conn, trans, locn);
            else
                _buffno = del.BuffNo = buffNo;
            del.BuffBranchNumber = branchNo;
            del.DateTrans = DateTime.Now;
            del.BranchNumber = branchNo;
            del.TransRefNo = refNo;
            del.TransValue = Math.Round(Convert.ToDecimal(item.Attributes[Tags.UnitPrice].Value) * (decimal)quantity, 2);
            del.RunNumber = 0;
            del.ContractNo = item.Attributes[Tags.ContractNumber].Value;
            if (del.DeliveryOrCollection == DelType.Collection)
            {
                del.ReturnItemNumber = item.Attributes[Tags.ReturnItemNo].Value;
                del.ReturnStockLocation = Convert.ToInt16(item.Attributes[Tags.ReturnLocation].Value);
                del.ReturnValue = (double)del.TransValue;
            }
            del.ftNotes = "DND1";
            del.NotifiedBy = user;
            //uat363 get parentITemNo if any
            string parentItemNo = string.Empty;
            int parentItemId = 0;                       //IP - 20/06/11 - CR1212 - RI - #4046

            //IP - 17/07/09 - 5.2 UAT(736) - Added check to ensure that the item.ParentNode.ParentNode is not null.
            if (item.ParentNode.ParentNode != null && item.ParentNode.ParentNode.Name == "Item")
            {
                parentItemNo = item.ParentNode.ParentNode.Attributes["Code"].Value;
                parentItemId = Convert.ToInt32(item.ParentNode.ParentNode.Attributes["ItemId"].Value);           //IP - 20/06/11 - CR1212 - RI - #4046
            }

            //if (item.ParentNode.ParentNode.Name == "Item")
            //    parentItemNo = item.ParentNode.ParentNode.Attributes["Code"].Value;
            del.ParentItemNo = parentItemNo;
            del.ParentItemId = parentItemId;

            del.Write(conn, trans);

            // If Affinity then create a TC58 record on FactTrans
            this.AffinityFactTC58(conn, trans, del);
        }

        public BDelivery(SqlConnection conn, SqlTransaction trans,
            string accountNo, double quantity,
            short branchNo, int user,
            XmlNode item, int refNo,
            decimal transvalue, int buffNo,
            int agreementNo)
            : this(conn, trans, accountNo, quantity, branchNo, user, item, refNo, transvalue, buffNo, agreementNo, DateTime.Today)
        {
        }

        public BDelivery(SqlConnection conn, SqlTransaction trans,
            string accountNo, double quantity,
            short branchNo, int user,
            XmlNode item, int refNo,
            decimal transvalue, int buffNo,
            int agreementNo, DateTime dateDelivered) //CR 478 added date delivered parameter to constructor
        {
            //uat363 rdb parentItemNo, code untested in all situations
            string parentItemNo = string.Empty;
            int parentItemID = 0;
            if (item.ParentNode.ParentNode != null && item.ParentNode.ParentNode.Name == "Item")
            {
                parentItemNo = item.ParentNode.ParentNode.Attributes["Code"].Value;
                parentItemID = Convert.ToInt32(item.ParentNode.ParentNode.Attributes["ItemId"].Value);   //IP - 13/06/11 - CR1212 - RI
            }

            DBranch branch = new DBranch();
            DDelivery del = new DDelivery();

            short locn = (short)Convert.ToInt16(item.Attributes[Tags.ReturnLocation].Value);
            if (locn == 0) locn = (short)Convert.ToInt16(item.Attributes[Tags.Location].Value);


            del.OrigBr = 0;
            del.AccountNumber = accountNo;
            del.AgreementNumber = agreementNo;
            del.DateDelivered = dateDelivered;// DateTime.Today; 
            del.DeliveryOrCollection = quantity < 0 ? DelType.Collection : DelType.Normal;
            del.ItemNumber = item.Attributes[Tags.Code].Value;
            del.ItemId = Convert.ToInt32(item.Attributes[Tags.ItemId].Value);
            del.StockLocation = Convert.ToInt16(item.Attributes[Tags.Location].Value);
            del.Quantity = quantity;
            if (buffNo == 0)
                _buffno = del.BuffNo = branch.GetBuffNo(conn, trans, locn);     //#15993
            else
                _buffno = del.BuffNo = buffNo;
            del.BuffBranchNumber = branchNo;
            del.DateTrans = DateTime.Now;
            del.BranchNumber = branchNo;
            del.TransRefNo = refNo;
            del.TransValue = transvalue;
            del.RunNumber = 0;
            if (del.DeliveryOrCollection == DelType.Collection)
            {
                del.ReturnItemNumber = item.Attributes[Tags.ReturnItemNo].Value;
                del.ReturnStockLocation = Convert.ToInt16(item.Attributes[Tags.ReturnLocation].Value);
                del.ReturnValue = (double)del.TransValue;
            }
            del.ContractNo = item.Attributes[Tags.ContractNumber].Value;
            del.ftNotes = "DND2";
            del.NotifiedBy = user;
            //uat363 rdb ParentItemNo now added to pk on Delivery table
            del.ParentItemNo = parentItemNo;
            //del.ParentItemId = Convert.ToInt32(item.Attributes[Tags.ParentItemId].Value);
            del.ParentItemId = parentItemID;                                                    //IP - 13/06/11 - CR1212 - RI

            del.Write(conn, trans);

            // If Affinity then create a TC58 record on FactTrans
            this.AffinityFactTC58(conn, trans, del);
        }

        private void AffinityFactTC58(SqlConnection conn, SqlTransaction trans, DDelivery del)
        {
            //
            // AFFINITY TC58 for FACT 2000
            //

            // Check this is an Affinity account
            DAccount account = new DAccount(conn, trans, del.AccountNumber);
            DTermsType terms = new DTermsType();
            // 69442 parameters were being passed in incorrect order JH 28/01/2008
            terms.GetTermsTypeDetail(conn, trans, Country[CountryParameterNames.CountryCode].ToString(), account.TermsType, account.AccountNumber, "", account.DateAccountOpen);
            string affinity = "X";
            foreach (DataRow r in terms.TermsTypeDetails.Rows)
            {
                if (r[CN.Affinity] != DBNull.Value)
                {
                    affinity = (string)r[CN.Affinity];
                }
            }

            if (affinity == "Y")
            {
                DFACTTrans fact = new DFACTTrans();
                // Need some LineItem details
                DLineItem lineItem = new DLineItem();
                //lineItem.GetSingleItem(conn, trans, del.StockLocation, del.ItemNumber, del.AccountNumber, del.AgreementNumber, del.ContractNo, del.ParentItemNo);
                lineItem.GetSingleItem(conn, trans, del.StockLocation, del.ItemId, del.AccountNumber, del.AgreementNumber, del.ContractNo, del.ParentItemId);       //IP - 17/05/11 - CR1212 - #3627 - use ItemID and ParentItemID

                // Create FactTrans TC58 record
                fact.AccountNumber = del.AccountNumber;
                fact.AgreementNumber = del.AgreementNumber;
                fact.BuffNo = del.BuffNo;
                fact.ItemNumber = del.ItemNumber;
                fact.StockLocation = del.StockLocation;
                fact.TranType = "01";
                fact.TCCode = "58";
                fact.TranDate = del.DateTrans;

                // Calculate quantity to allow for revisions
                fact.Quantity = del.Quantity;
                if (lineItem.Quantity != 0)
                {
                    // Get the quantity that might have been delivered already
                    //del.GetDeliveredQuantity(conn, trans, del.AccountNumber, 
                    //    del.AgreementNumber, 
                    //    del.ItemNumber, 
                    //    del.StockLocation,
                    //    "",del.ParentItemNo);

                    del.GetDeliveredQuantity(conn, trans, del.AccountNumber,
                        del.AgreementNumber,
                        del.ItemId,
                        del.StockLocation,
                        "", del.ParentItemId);      //IP - 17/05/11 - CR1212 - #3627 - use ItemID and ParentItemID

                    if (fact.Quantity > 0)
                        // Positive delivery
                        // The remaining order will be the initial quantity minus
                        // quantity already delivered minus quantity delivering now.
                        fact.Quantity = lineItem.Quantity - del.DeliveredQuantity - fact.Quantity;
                    else
                        // Negative delivery / collection
                        // Re-state the remaining order outstanding. A separate record
                        // will be sent to FACT with the quantity being collected.
                        fact.Quantity = lineItem.Quantity - del.DeliveredQuantity;
                }
                else
                    fact.Quantity = 0;

                fact.Price = (double)lineItem.Price;
                // Calculate the order value being delivered now
                fact.Value = fact.Quantity * fact.Price;
                fact.TaxAmt = lineItem.TaxAmount; // ?? Perhaps should also be calculated
                fact.Save(conn, trans);
            }
        }

        public void Save(SqlConnection conn, SqlTransaction trans,
            DataSet ds, int transRefNo)
        {
            DDelivery del = new DDelivery();
            DBranch branch = new DBranch();
            string notes = "";

            foreach (DataRow row in ds.Tables["Deliveries"].Rows)
            {
                short locn = (short)Convert.ToInt32(row["retstocklocn"]);
                if (locn == 0) locn = (short)Convert.ToInt32(row["stocklocn"]);

                del.OrigBr = 0;
                del.AccountNumber = (string)row["acctno"];
                del.AgreementNumber = Convert.ToInt32(row["agrmtno"]);
                del.DateDelivered = Convert.ToDateTime(row["datedel"]);
                del.DeliveryOrCollection = (string)row["delorcoll"];
                del.ItemNumber = (string)row["itemno"];
                del.ItemId = Convert.ToInt32(row[CN.ItemId]);
                del.StockLocation = (short)Convert.ToInt32(row["stocklocn"]);
                del.Quantity = Convert.ToDouble(row["quantity"]);
                _buffno = del.BuffNo = branch.GetBuffNo(conn, trans, locn);
                del.BuffBranchNumber = (short)Convert.ToInt32(row["buffbchno"]);
                del.DateTrans = Convert.ToDateTime(row["datetrans"]);
                del.BranchNumber = (short)Convert.ToInt32(row["buffbchno"]);
                del.TransRefNo = transRefNo;
                del.TransValue = Convert.ToDecimal(row["transvalue"]);
                del.ReturnValue = Convert.ToDouble(row["retval"]);
                del.ReturnItemNumber = (string)row["retitemno"];
                del.ReturnItemId = Convert.ToInt32(row[CN.RetItemId]);                      //IP - 02/10/12 - #10501 - LW75161
                del.ReturnStockLocation = (short)Convert.ToInt32(row["retstocklocn"]);
                del.RunNumber = 0;
                del.ContractNo = (string)row[CN.ContractNo];
                notes = (string)row[CN.Reason];
                del.ftNotes = "DNDS";
                del.NotifiedBy = this.User;
                del.ParentItemNo = row[CN.ParentItemNo].ToString();
                del.ParentItemId = Convert.ToInt32(row[CN.ParentItemId]);
                del.Write(conn, trans);

                if (del.DeliveryOrCollection == "R" && notes.Length > 0)
                {
                    DLineItem li = new DLineItem();
                    li.UpdateNotes(conn, trans, del.AccountNumber,
                        del.AgreementNumber, del.ItemId,                        //IP - 17/05/11  - CR1212 - #3627 - Changed from ItemNo to ItemId
                        del.StockLocation, del.ContractNo, notes, false);
                }
            }
        }

        public void SaveInsuranceReturns(SqlConnection conn, SqlTransaction trans, DataSet ds)
        //CR 822 created to cancel warranty on insurance returns [Peter Chong] 09-Sep-2006
        {
            DDelivery del = new DDelivery();
            BBranch branch = new BBranch();

            foreach (DataRow row in ds.Tables[TN.Deliveries].Rows)
            {

                del.WriteInsuranceReturn(conn, trans, row[CN.AccountNumber].ToString(), Convert.ToInt32(row[CN.AgrmtNo]),
                    // row[CN.ItemNo].ToString(), Convert.ToInt16(row[CN.StockLocn]), Convert.ToInt32(row[CN.BuffNo]),
                      Convert.ToInt32(row[CN.WarrantyId]), Convert.ToInt16(row[CN.StockLocn]), Convert.ToInt32(row[CN.BuffNo]),             //IP - 08/06/11 - CR1212 - RI
                      row[CN.ContractNo].ToString(), Convert.ToInt32(row[CN.EmployeeNo]), row[CN.ReturnCode].ToString(), branch.GetBuffNo(conn, trans, Convert.ToInt16(row[CN.StockLocn])));

            }
        }


        public DataSet GetForRepo(string accountNumber, out string accountType)     // #14927 
        {
            DataSet ds = new DataSet();
            DDelivery del = new DDelivery();
            //del.GetForRepo(accountNumber, out accountType);
            //ds.Tables.Add(del.Deliveries);          

            ds = del.GetForRepo(accountNumber, out accountType);          // #14927 

            return ds;
        }

        public DataSet GetDeliveries(string accountNumber, int agreementNo)
        {
            DataSet ds = new DataSet();
            DDelivery del = new DDelivery();
            del.AccountNumber = accountNumber;
            del.AgreementNumber = agreementNo;
            del.GetDeliveries(null, null);
            ds.Tables.Add(del.Deliveries);
            return ds;
        }

        public DataSet GetCashAndGoAccts(long BuffNo, int BranchNo, DateTime From, DateTime To,
            bool searchWarrantyReturns)
        {
            DataSet ds = new DataSet();
            DDelivery del = new DDelivery();
            del.GetCashAndGoAccts(BuffNo, BranchNo, From, To, searchWarrantyReturns);
            ds = del.CashAndGoDels;         //IP - 08/05/12 - #9608 - CR8520
            //ds.Tables.Add(del.Deliveries); 
            return ds;
        }

        // Check Delivery amount does not exceed Agreement Total
        public bool DelExceedsAgrTotal(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            DAccount account = new DAccount(conn, trans, accountNo);
            BItem lineItem = new BItem();
            BTransaction finTrans = new BTransaction();

            lineItem.GetRealDiscount(conn, trans, accountNo);
            finTrans.GetByAcctNo(conn, trans, accountNo);

            if (finTrans.DelTot - lineItem.realDiscount > account.AgreementTotal)
                return true;
            else
                return false;
        }

        public short AccountFullyDelivered(SqlConnection conn, SqlTransaction trans, string accountNo, decimal agreementTotal, out decimal delTot)
        {
            //DAccount account = new DAccount(conn, trans, accountNo);
            BItem lineItem = new BItem();
            BTransaction finTrans = new BTransaction();

            lineItem.GetRealDiscount(conn, trans, accountNo);
            finTrans.GetByAcctNo(conn, trans, accountNo);
            delTot = finTrans.DelTot;

            //Comment out IF statement below as not calculating rebate.
            //if (finTrans.DelTot - lineItem.realDiscount >= account.AgreementTotal)
            if (finTrans.DelTot >= agreementTotal)
                return 1;
            else
                return 0;
        }

        // Check Return does not exceed the amount delivered
        public bool ReturnExceedsDel(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            decimal delTotal = 0;                    //IP - 28/06/11 - 5.13 - LW73619 - #3751 - Changed the return type
            BItem lineItem = new BItem();
            BTransaction finTrans = new BTransaction();

            lineItem.GetRealDiscount(conn, trans, accountNo);
            //finTrans.GetByAcctNo(conn, trans, accountNo);
            //RM Getting delivery value including scheduled
            delTotal = DeliveryGetTotal(conn, trans, accountNo);


            if (delTotal + lineItem.realDiscount < 0)
                return true;
            else
                return false;
        }

        public decimal DeliveryGetTotal(SqlConnection conn, SqlTransaction trans, string acctno)  //IP - 28/06/11 - 5.13 - LW73619 - #3751 - Changed the return type
        {
            decimal deliveryTotal = 0;      //IP - 28/06/11 - 5.13 - LW73619 - #3751

            DDelivery delivery = new DDelivery();

            deliveryTotal = delivery.GetDeliveryTotal(conn, trans, acctno);

            return deliveryTotal;
        }


        public void DeliverImmediately(SqlConnection conn, SqlTransaction trans, string accountNo, string EmpeeNo,
            DateTime DateDel, DataSet DeliveryItems,
            string countryCode, short branchNo, ref bool allItemsCollected, out string error)
        {
            DAccount account = new DAccount();
            DDeliveryLoad delLoad = new DDeliveryLoad();
            DDelivery del = new DDelivery();
            DFinTrans fin = new DFinTrans();
            DSchedule sched = new DSchedule();
            DBranch branch = new DBranch();
            DLineItem lineItem = new DLineItem();
            DFACTTrans fact = new DFACTTrans();
            BItem item = new BItem();
            DAgreement agree = null;
            DataRow initRow = null;
            short buffLocn;
            int buffNo;
            bool CollectionFound = false;
            decimal transValue = 0;
            string transType = "";
            double delivered = 0;
            int transRefNo = 0;
            decimal delValue = 0;

            allItemsCollected = false;

            error = "";

            // Check there is an item list to deliver
            foreach (DataTable DeliveryTable in DeliveryItems.Tables)
            {
                if (DeliveryTable.TableName == "Schedules" && DeliveryTable.Rows.Count > 0)
                    initRow = DeliveryTable.Rows[0];
            }
            if (initRow == null)
            {
                error = "No items to deliver - Delivery not processed";
                return;
            }

            // Stock Location (used for buff nos)
            buffLocn = (short)(initRow[CN.RetStockLocn]);
            if (buffLocn == 0) buffLocn = (short)(initRow[CN.StockLocn]);
            buffNo = (int)(initRow[CN.BuffNo]);

            // Branch where we are now (used for trans ref nos)
            transRefNo = branch.GetTransRefNo(conn, trans, branchNo);

            //eeff

            // Load the account
            if (!account.Populate(conn, trans, accountNo))
            {
                error = GetResource("M_NOACCOUNTFORDEL");
                //error = "The Account Details could not be loaded - Delivery not processed";
                return;
            }

            // Validate the date of delivery (ignore time)
            if (DateDel.Date < account.DateAccountOpen.Date)
            {
                error = GetResource("M_INVALIDDATEDEL");
                //error = "The date of delivery cannot be before the date the account opened";
                return;
            }

            // Delete any existing transport schedule for this delivery
            delLoad.DeleteByBuffNo(conn, trans, buffLocn, buffNo);

            // Create a Delivery and FinTrans record for each item
            foreach (DataRow row in DeliveryItems.Tables["Schedules"].Rows)
            {
                if ((string)row[CN.DelType] == "R")
                    isReplacement = true;
                else
                    isReplacement = false;

                //
                // Delivery record
                //
                del.OrigBr = 0;
                del.AccountNumber = accountNo;
                del.AgreementNumber = Convert.ToInt32(row[CN.AgrmtNo]);
                del.BranchNumber = branchNo;
                del.BuffBranchNumber = (short)Convert.ToInt32(row[CN.BuffBranchNo]);
                del.BuffNo = buffNo;
                del.ItemNumber = (string)row[CN.ItemNo];
                del.ItemId = Convert.ToInt32(row[CN.ItemID]);

                del.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                del.Quantity = Convert.ToDouble(row[CN.Quantity]);
                del.DateDelivered = DateDel;
                del.DateTrans = DateTime.Now;
                del.RunNumber = 0;
                del.TransRefNo = transRefNo;

                del.ReturnItemNumber = (string)row[CN.RetItemNo];
                del.ReturnItemId = Convert.ToInt32(row[CN.RetItemId]);
                del.ReturnValue = 0;
               // del.ReturnStockLocation = 0;
                del.ReturnStockLocation = Convert.ToInt16(row[CN.RetStockLocn]) != 0 ? Convert.ToInt16(row[CN.RetStockLocn]) : Convert.ToInt16(0);      //IP - 03/10/12 - #10422 - LW75134
                del.NotifiedBy = Convert.ToInt32(EmpeeNo);
                del.ContractNo = (string)row[CN.ContractNo];        //
                //uat363 rdb parentItemN required as part of pk
                del.ParentItemNo = row[CN.ParentItemNo].ToString();
                del.ParentItemId = Convert.ToInt32(row[CN.ParentItemId]);

                if (del.ReturnItemId > 0 && del.ReturnItemNumber.Length != 0 && del.ReturnItemNumber.Trim() != GetResource("T_REPLACE").Trim() && del.Quantity > 0)
                {
                    // Redelivery / Repossession - the value and the stock
                    // location can change. SC added del quant > 0 to prevent collections being assinged as R.
                    del.DeliveryOrCollection = DelType.Return;
                    del.ReturnValue = Convert.ToDouble(row[CN.RetVal]);
                    del.ReturnStockLocation = (short)Convert.ToInt32(row[CN.RetStockLocn]);
                }
                else if (del.Quantity < 0)
                {
                    // Collection - the value is the same but can be
                    // returned to a different location.
                    del.DeliveryOrCollection = DelType.Collection;
                    del.ReturnStockLocation = (short)Convert.ToInt32(row[CN.RetStockLocn]);
                }
                else
                {
                    // Normal Delivery
                    del.DeliveryOrCollection = DelType.Normal;
                }

                if (del.ReturnItemNumber.Trim() == GetResource("T_REPLACE").Trim())
                {
                    del.ReturnStockLocation = (short)Convert.ToInt32(row[CN.RetStockLocn]);
                    del.ReturnItemNumber = String.Empty;
                }

                lineItem.GetSingleItem(conn, trans, System.Convert.ToInt16(del.StockLocation), del.ItemId, del.AccountNumber, del.AgreementNumber, del.ContractNo, del.ParentItemId); //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and ParentItemID rather than ItemNumber and ParentItemNo
                //
                // Fintrans record
                //
                //JJ - Must write the fintrans record using the BTransaction constructor
                //so that the AddTransaction method is run
                if (del.DeliveryOrCollection == DelType.Return)
                {
                    // Trans value is based on the return value
                    //transValue += delValue = (decimal)del.ReturnValue;

                    if (del.Quantity < 0)
                    {
                        // Repossesion - the value must be negative (credit)
                        transType = DelTrans.Repossession;
                        transValue += delValue = -Math.Abs((decimal)del.ReturnValue);
                    }
                    else
                    {
                        // Redelivery - the value must be positive (debit)
                        transType = DelTrans.Redelivery;
                        transValue += delValue = Math.Abs((decimal)del.ReturnValue);
                        Redelivery = true;
                    }
                }
                else
                {
                    //IP - 19/09/11 - #3478 - LW71346 - If collection then use Return value
                    if (del.DeliveryOrCollection == DelType.Collection)
                    {
                        //transValue += delValue = (decimal)del.Quantity * Convert.ToDecimal(row[CN.RetVal]);
                        transValue += delValue = -1 * Convert.ToDecimal(row[CN.RetVal]);     // #10000 jec 01/05/12 retval already caters for quantity
                    }
                    else
                    {
                        // Trans value is based on the line item
                        if (lineItem.Price != 0)
                            transValue += delValue = (decimal)del.Quantity * lineItem.Price;
                        else
                            transValue += delValue = (decimal)del.Quantity * lineItem.OrderValue;
                    }

                    if (del.Quantity < 0)
                    {
                        // Collection
                        transType = DelTrans.Collection;
                        CollectionFound = true;
                    }
                    else
                    {
                        // Normal delivery
                        transType = DelTrans.Normal;
                    }
                }

                //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID and ParentItemID rather than ItemNumber and ParentItemNo
                del.GetDeliveredQuantity(conn, trans, accountNo,
                    del.AgreementNumber,
                    del.ItemId,
                    del.StockLocation,
                    "", del.ParentItemId);	/* contract no now needed as warranties
														 * have to be collected at the same time
														 * as the parent item */
                delivered = del.DeliveredQuantity;

                del.TransValue = delValue;
                del.ftNotes = "DNIM";
                del.NotifiedBy = Convert.ToInt32(EmpeeNo);
                del.Write(conn, trans);

                //IP - 13/01/12 - #9443 - Update Lineitem.delqty
                if (transType == DelTrans.Normal)
                {
                    lineItem.UpdateDelQty(conn, trans, accountNo, del.StockLocation, del.AgreementNumber, del.ItemId, string.Empty, del.Quantity, del.ParentItemId);
                }

                sched.AccountNumber = del.AccountNumber;
                sched.AgreementNumber = del.AgreementNumber;
                sched.ItemNumber = del.ItemNumber;
                sched.ItemID = del.ItemId;
                sched.StockLocation = del.StockLocation;
                sched.BuffBranchNo = del.BuffBranchNumber;
                sched.BuffNo = del.BuffNo;
                // Save an audit record for this schedule item
                sched.AuditItem(conn, trans);
                // Delete the schedule item
                sched.DeleteSchedule(conn, trans, false);

                if (account.AccountType != AT.Special) //Acct Type Translation DSR 29/9/03
                {
                    //Sending a record to FACT cancelling the order unless 
                    //there are outstanding items on it

                    lineItem.GetSingleItem(conn, trans, System.Convert.ToInt16(del.StockLocation), del.ItemId, del.AccountNumber, del.AgreementNumber, del.ContractNo, del.ParentItemId); //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and ParentItemID rather than ItemNumber and ParentItemNo

                    fact.AccountNumber = accountNo;
                    fact.AgreementNumber = Convert.ToInt32(row[CN.AgrmtNo]);
                    fact.BuffNo = buffNo;
                    fact.TaxAmt = lineItem.TaxAmount; // ?? Perhaps should also be calculated
                    fact.Quantity = Convert.ToDouble(row[CN.Quantity]);
                    fact.ItemNumber = (string)row[CN.ItemNo];
                    fact.StockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);


                    if (Convert.ToInt32(row[CN.RetItemId]) == 0)
                    {
                        fact.TranType = "01";
                        fact.TCCode = "58";

                        if (lineItem.Quantity != 0)
                        {
                            if (fact.Quantity > 0)
                                // Positive delivery
                                // The remaining order will be the initial quantity minus
                                // quantity already delivered minus quantity delivering now.
                                fact.Quantity = lineItem.Quantity - delivered - fact.Quantity;
                            else
                                // Negative delivery / collection
                                // Re-state the remaining order outstanding. A separate record
                                // will be sent to FACT with the quantity being collected.
                                fact.Quantity = lineItem.Quantity - delivered;
                        }
                        else
                            fact.Quantity = 0;

                        fact.Value = (double)lineItem.Price * fact.Quantity;
                        fact.TranDate = DateTime.Now;
                        fact.Price = (double)lineItem.Price;
                        fact.Save(conn, trans);
                    }
                }

                //IP - 14/12/11 - #8884 - Moved to here from below.
                if (CollectionFound)
                {
                    if (this.ReturnExceedsDel(conn, trans, account.AccountNumber))
                    {
                        PaymentWebApi apis = new PaymentWebApi();
                        DataTable data = apis.GetMambuAccountsForPayment(null, account.AccountNumber);

                        if (data == null || data.Rows == null || data.Rows.Count == 0)
                        {
                            error = GetResource("M_RETURNEXCEEDSDEL", new object[] { account.AccountNumber });
                            //error = "Return amount for account " + account.AccountNumber +
                            //	"\nexceeds Delivery Total - Return not processed";
                            return;
                        }
                    }
                }

                agree = new DAgreement(conn, trans, accountNo, del.AgreementNumber);

                if (del.DeliveryOrCollection == DelType.Collection)
                {
                    /* if we update the quantity of associated warranties/discounts here
                     * the difference between delivered qty and qty will be 
                     * dealt with in DeliverNonStocks */
                    CollectAssociatedItems(conn, trans, del.AccountNumber, del.ItemId,              //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID rather than ItemNumber
                        del.StockLocation, del.AgreementNumber, -del.Quantity, ref agree);
                }

                if (del.DeliveryOrCollection == DelType.Normal)
                {
                    if (countryCode == "M" || countryCode == "C")
                    {
                        SetDelDateForExchange(conn, trans, del.AccountNumber, del.ItemId, del.StockLocation,
                            del.AgreementNumber);
                    }
                }

                ///write a bailaction record if this is a goods on loan account
                if (account.AccountType == AT.GoodsOnLoan)
                {
                    DBailAction ba = new DBailAction();
                    ba.AccountNo = accountNo;
                    ba.EmployeeNo = this.User;
                    ba.DateAdded = DateTime.Today;
                    ba.Code = del.DeliveryOrCollection == DelType.Collection ? "GOLC" : "GOLD";
                    ba.DateDue = lineItem.ExpectedReturnDate;
                    ba.ActionValue = Convert.ToDouble(del.TransValue);
                    ba.AddedBy = this.User;
                    ba.Save(conn, trans);
                }

                string replacementStatus = del.DeliveryOrCollection == DelType.Collection ? ServiceReplacementStatus.GoodsReturned : ServiceReplacementStatus.Delivered;
                DServiceRequest serviceRequest = new DServiceRequest();
                serviceRequest.SaveResolutionStatus(conn, trans, del.AccountNumber, del.ItemId, // IP - 22/07/11 - RI -  del.ItemNumber, - changed to use ItemId
                                                    del.StockLocation, replacementStatus);

            } // End of foreach delivery item

            //IP - 14/12/11 - #8884 - Moved to before collecting associated items as ordval for discounts updated to 0 at this point
            //if (CollectionFound)
            //{
            //    if (this.ReturnExceedsDel(conn, trans, account.AccountNumber))
            //    {
            //        error = GetResource("M_RETURNEXCEEDSDEL", new object [] {account.AccountNumber});
            //        //error = "Return amount for account " + account.AccountNumber +
            //        //	"\nexceeds Delivery Total - Return not processed";
            //        return;
            //    }
            //}
            //else
            //{
            if (!CollectionFound)
            {
                if (this.DelExceedsAgrTotal(conn, trans, account.AccountNumber))
                {
                    PaymentWebApi apis = new PaymentWebApi();
                    DataTable data = apis.GetMambuAccountsForPayment(null, account.AccountNumber);

                    if (data == null || data.Rows == null || data.Rows.Count == 0)
                    {
                        error = GetResource("M_DELEXCEEDSAGREEMENT", new object[] { account.AccountNumber });
                        //error = "Delivery amount for account " + account.AccountNumber +
                        //	"\nexceeds Agreement Total - Delivery not processed";
                        return;
                    }
                }
            }

            if (WarrantyRemoved && AT.IsCreditType(account.AccountType))
            {
                /* if this is a collection we may have removed one or more warranties 
                 * from the account. If so we must recalculate the service charge */

                decimal chargeableAdminPrice = 0;
                agree.CashPrice = account.GetChargeableCashPrice(conn, trans, accountNo, ref chargeableAdminPrice);

                RecalculateServiceCharge(conn, trans, account, agree);

                /* any reduction in delivered DT or other charges will be collected
                 * when we run DeliverNonStocks below */
            }

            /* Perform the delivery of non-stock items on this account */
            DeliverNonStocks(conn, trans, accountNo, account.AccountType, countryCode, branchNo, transRefNo, ref transValue, del.AgreementNumber);

            //IP - 15/03/12 - #9797 - Get the sum OrdVal from Lineitem to give the agreement total.
            AccountRepository ar = new AccountRepository();
            decimal? sumOrdVal = ar.SumOrdValForAcct(conn, trans, accountNo, agree.AgreementNumber);

            if (sumOrdVal.HasValue)
            {
                agree.AgreementTotal = Convert.ToDecimal(sumOrdVal);
            }

            agree.DeliveryFlag = "Y";
            agree.DateChange = DateTime.Now;        //#14392
            PaymentWebApi api = new PaymentWebApi();
            DataTable dt = api.GetMambuAccountsForPayment(null, accountNo);
            if (dt.Rows.Count > 0)
            {
                agree.DateDel = DateTime.Now;   
            }
            agree.Save(conn, trans);
            account.AgreementTotal = agree.AgreementTotal;
            account.Save(conn, trans);

            /* write the fintrans delivery record for all items (stock and non-stock) */
            BTransaction t = new BTransaction(conn, trans, accountNo, branchNo,
                transRefNo, transValue, Convert.ToInt32(EmpeeNo),
                transType, "", "", "", 0, countryCode,
                DateDel, FootNote.ImmediateDelivery, agree.AgreementNumber);

            decimal charge = 0;
            DSundryCharge sundry = new DSundryCharge();
            sundry.GetSundryChargeItem(conn, trans, account.AccountType);
            DataTable sundryTable = sundry.Items;
            if (sundryTable.Rows.Count > 0)
            {
                foreach (DataRow r in sundryTable.Rows)
                    charge = Convert.ToDecimal(r[CN.Amount]);
            }

            decimal adminPrice = 0;
            decimal cashPrice = account.GetChargeableCashPrice(conn, trans, accountNo, ref adminPrice);

            // UAT 25 for v4 RD 31/01/06 Added check to see if collection and cashprice zero
            // UAT 37 for v4 PN 20/02/06 Added additional check any warranty adjustments
            if ((del.DeliveryOrCollection == DelType.Collection &&
                fin.GetWarrantyAdjustment(conn, trans, accountNo) == 0 &&
                (cashPrice == charge || cashPrice == 0)))
            {
                allItemsCollected = true;
            }
        }




        // CR914 rdb 16/10/07 -Immediate Deivery Failed
        public void DeliverImmediatelyFailed(SqlConnection conn, SqlTransaction trans,
            string accountNo, string EmpeeNo, DateTime DateDel, DataSet DeliveryItems,
            string countryCode, short branchNo, out string error)
        {
            DAccount account = new DAccount();
            DBranch branch = new DBranch();
            DataRow initRow = null;
            short buffLocn;
            int buffNo;
            int transRefNo = 0;


            error = "";

            // Check there is an item list to fail delivery
            foreach (DataTable DeliveryTable in DeliveryItems.Tables)
            {
                if (DeliveryTable.TableName == "Schedules" && DeliveryTable.Rows.Count > 0)
                    initRow = DeliveryTable.Rows[0];
            }
            if (initRow == null)
            {
                error = "No items to deliver - Failed delivery not processed";
                return;
            }

            // Stock Location (used for buff nos)
            buffLocn = (short)(initRow[CN.RetStockLocn]);
            if (buffLocn == 0) buffLocn = (short)(initRow[CN.StockLocn]);
            buffNo = (int)(initRow[CN.BuffNo]);

            // Branch where we are now (used for trans ref nos)
            transRefNo = branch.GetTransRefNo(conn, trans, branchNo);

            // Load the account
            if (!account.Populate(conn, trans, accountNo))
            {
                //error = GetResource("M_NOACCOUNTFORDEL");
                error = "The Account Details could not be loaded - Failed delivery not processed";
                return;
            }

            // Validate the date of delivery (ignore time)
            if (DateDel.Date < account.DateAccountOpen.Date)
            {
                //error = GetResource("M_INVALIDDATEDEL");
                error = "The date of delivery cannot be before the date the account opened - Failed delivery not processed";
                return;
            }


            // rdb we need to add a record to scheduleremoval table and delete the record in
            // schedule table for each item

            //69557 if more than one item in the table then inform database of this fact
            bool moreThanOne = false;
            DDeliveryLoad deliveryLoad = new DDeliveryLoad();
            foreach (DataRow row in DeliveryItems.Tables["Schedules"].Rows)
            {
                short stockLocation = (short)Convert.ToInt32(row[CN.StockLocn]);
                int agrmtNo = Convert.ToInt32(row[CN.AgrmtNo]);
                int itemId = Convert.ToInt32(row[CN.ItemId]);
                // rdb im going to try calling [DN_RemoveDeliveryNoteItemSP]RemoveDeliveryNoteItem
                // for each row
                deliveryLoad.RemoveDeliveryNoteItem(conn, trans, stockLocation, buffNo,
                    accountNo, agrmtNo, itemId, Convert.ToInt32(EmpeeNo), "", moreThanOne);
                moreThanOne = true;
            } // End of foreach delivery item


        }

















































        public void RecalculateServiceCharge(SqlConnection conn, SqlTransaction trans,
            DAccount acct, DAgreement agree)
        {
            decimal chargeableAdminPrice = 0;
            decimal chargeablePrice = 0;
            decimal deferredTerms = 0;
            decimal insuranceCharge = 0;
            decimal adminCharge = 0;
            decimal dtTax = 0;
            decimal insTax = 0;
            decimal adminTax = 0;
            decimal monthly = 0;
            decimal final = 0;
            decimal origDeferredTerms = agree.ServiceCharge;
            decimal origInsuranceValue = 0;
            decimal origAdminValue = 0;
            double origDTTaxValue = 0;
            double origInsuranceTaxValue = 0;
            double origAdminTaxValue = 0;

            // LW 69505 The values of country parameters InsuranceChargeItem & AdminChargeItem should not be set to 'DT'
            int insuranceChargeItem = (string)Country[CountryParameterNames.InsuranceChargeItem] == "DT" ? 0 : StockItemCache.Get(StockItemKeys.InsuranceChargeItem); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item - changed to int
            int adminChargeItem = (string)Country[CountryParameterNames.AdminChargeItem] == "DT" ? 0 : StockItemCache.Get(StockItemKeys.AdminChargeItem); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item - changed to int

            BAgreement bAgree = new BAgreement();
            BItem item = new BItem();

            DInstalPlan ip = new DInstalPlan();
            ip.Populate(conn, trans, acct.AccountNumber, agree.AgreementNumber);

            DLineItem li = new DLineItem();
            // todo uat363 rdb untested code, I don't think parentItemNo is applicable here
            li.GetSingleItem(conn, trans, acct.BranchNo, StockItemCache.Get(StockItemKeys.DT),          //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
                acct.AccountNumber, agree.AgreementNumber, "", 0);                                       //IP/NM - 18/05/11 -CR1212 - #3627 - 0 for ParentItemID
            origDTTaxValue = li.TaxAmount;

            li.OrderValue = 0;
            li.TaxAmount = 0;
            li.GetSingleItem(conn, trans, acct.BranchNo, insuranceChargeItem,  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
                                acct.AccountNumber, agree.AgreementNumber, "", 0);           //IP/NM - 18/05/11 -CR1212 - #3627 - 0 for ParentItemID
            origInsuranceValue = li.OrderValue;
            origInsuranceTaxValue = li.TaxAmount;

            li.OrderValue = 0;
            li.TaxAmount = 0;
            li.GetSingleItem(conn, trans, acct.BranchNo, adminChargeItem,     //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
                                acct.AccountNumber, agree.AgreementNumber, "", 0);        //IP/NM - 18/05/11 -CR1212 - #3627 - 0 for ParentItemID
            origAdminValue = li.OrderValue;
            origAdminTaxValue = li.TaxAmount;

            /* retrieve the chargeable cash price */
            chargeablePrice = acct.GetChargeableCashPrice(conn, trans, acct.AccountNumber, ref chargeableAdminPrice);

            DataSet variableRatesSet = new DataSet();
            /* recalculate the deferred terms and other charges */
            deferredTerms = bAgree.CalculateServiceCharge(conn, trans,
                (string)Country[CountryParameterNames.CountryCode],
                acct.TermsType,
                acct.AccountNumber,
                "",
                agree.Deposit,
                ip.NumberOfInstalments,
                chargeablePrice,
                acct.DateAccountOpen,
                acct.AccountType,
                chargeableAdminPrice,
                ref insuranceCharge,
                ref adminCharge,
                ref variableRatesSet);

            if (insuranceCharge < 0)
                insuranceCharge = 0;

            if (adminCharge < 0)
                adminCharge = 0;


            /* need to add tax onto deferred terms and admin charge and insurance charge
             * and then pass the total of all of that into CreateInstalPlan */
            XmlNode itemNode = item.GetItemDetails(StockItemCache.Get(StockItemKeys.DT), acct.BranchNo, acct.AccountType, (string)Country[CountryParameterNames.CountryCode], false, false);  // CR1212 jec need to supply itemID not zero //IP/NM - 18/05/11 -CR1212 - #3627 
            itemNode.Attributes[Tags.Quantity].Value = "1";
            itemNode.Attributes[Tags.UnitPrice].Value = deferredTerms.ToString();
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            dtTax = item.CalculateTaxAmount(itemNode, acct.IsTaxExempt(conn, trans, acct.AccountNumber, ""));

            if (insuranceCharge > 0)
            {
                itemNode = item.GetItemDetails(insuranceChargeItem, acct.BranchNo, acct.AccountType, (string)Country[CountryParameterNames.CountryCode], false, false);   // CR1212 jec need to supply itemID not zero //IP/NM - 18/05/11 -CR1212 - #3627
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = insuranceCharge.ToString();
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                insTax = item.CalculateTaxAmount(itemNode, acct.IsTaxExempt(conn, trans, acct.AccountNumber, ""));
            }

            if (adminCharge > 0)
            {
                itemNode = item.GetItemDetails(adminChargeItem, acct.BranchNo, acct.AccountType, (string)Country[CountryParameterNames.CountryCode], false, false);   // CR1212 jec need to supply itemID not zero //IP/NM - 18/05/11 -CR1212 - #3627 
                itemNode.Attributes[Tags.Quantity].Value = "1";
                itemNode.Attributes[Tags.UnitPrice].Value = adminCharge.ToString();
                itemNode.Attributes[Tags.TaxAmount].Value = "0";
                adminTax = item.CalculateTaxAmount(itemNode, acct.IsTaxExempt(conn, trans, acct.AccountNumber, ""));
            }

            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I")
                dtTax = adminTax = insTax = 0;

            /* calculate the new instalment plan */
            bAgree.CalculateInstalPlan(agree.CashPrice,
                agree.Deposit,
                deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax,
                ip.NumberOfInstalments,
                out monthly,
                out final);

            /* update the relevant tables with the new agreement information */
            agree.AgreementTotal = deferredTerms + dtTax + insuranceCharge + insTax + adminCharge + adminTax + agree.CashPrice;
            acct.AgreementTotal = agree.AgreementTotal;
            ip.InstalmentAmount = monthly;
            ip.FinalInstalment = final;
            agree.ServiceCharge = deferredTerms;
            agree.EmployeeNumChange = this.User;

            agree.DateChange = DateTime.Now;            //#14392
            agree.Save(conn, trans);
            acct.Save(conn, trans);
            ip.User = this.User;
            ip.Save(conn, trans);

            /* need to update the value STAX items too if dtTax || insTax || adminTax > 0 */
            item.UpdateItemValue(conn, trans, acct.AccountNumber, 1, StockItemCache.Get(StockItemKeys.DT), acct.BranchNo, deferredTerms); //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, acct.AccountNumber, 1, insuranceChargeItem, acct.BranchNo, insuranceCharge);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item
            item.UpdateItemValue(conn, trans, acct.AccountNumber, 1, adminChargeItem, acct.BranchNo, adminCharge);  //IP/NM - 18/05/11 -CR1212 - #3627 - Use cached item

            item.AgreementNumber = agree.AgreementNumber;
            item.AuditSource = agree.AuditSource;
            item.AccountNumber = acct.AccountNumber;
            item.StockLocation = acct.BranchNo;
            item.User = this.User;
            if (dtTax > 0)
            {
                item.UpdateTaxAmount(conn, trans, acct.AccountNumber, StockItemCache.Get(StockItemKeys.DT), acct.BranchNo, dtTax);
            }

            if (insTax > 0 || (origInsuranceTaxValue > 0 && origInsuranceTaxValue != Convert.ToDouble(insTax)))
            {
                item.UpdateTaxAmount(conn, trans, acct.AccountNumber, insuranceChargeItem, acct.BranchNo, insTax);
            }

            if (adminTax > 0 || (origAdminTaxValue > 0 && origAdminTaxValue != Convert.ToDouble(adminTax)))
            {
                item.UpdateTaxAmount(conn, trans, acct.AccountNumber, adminChargeItem, acct.BranchNo, adminTax);
            }

            // write a record to lineitem audit if DT value has changed
            if (origDeferredTerms != deferredTerms)
            {
                item.ItemNumber = "DT";
                item.ItemId = StockItemCache.Get(StockItemKeys.DT);

                item.UpdateLineItemAudit(conn, trans, 1, 1, origDeferredTerms, deferredTerms,
                                          origDTTaxValue, Convert.ToDouble(dtTax));
            }

            if (origInsuranceValue > 0 && origInsuranceValue != insuranceCharge)
            {
                //item.ItemNumber = insuranceChargeItem;
                item.ItemId = insuranceChargeItem;                   //IP/NM - 18/05/11 -CR1212 - #3627
                item.UpdateLineItemAudit(conn, trans, 1, 1, origInsuranceValue, insuranceCharge,
                                          origInsuranceTaxValue, Convert.ToDouble(insTax));
            }

            if (origAdminValue > 0 && origAdminValue != adminCharge)
            {
                //item.ItemNumber = adminChargeItem;
                item.ItemId = adminChargeItem;                       //IP/NM - 18/05/11 -CR1212 - #3627
                item.UpdateLineItemAudit(conn, trans, 1, 1, origAdminValue, adminCharge,
                                          origAdminTaxValue, Convert.ToDouble(adminTax));
            }
        }

        public void DeliverNonStocks(SqlConnection conn, SqlTransaction trans,
            string accountNo, string accountType,
            string countryCode, short branchNo,
            int refNo, ref decimal transValue,
            int agreementNo)
        {
            var DutyFree =false;
            var TaxExempt = false;

            BItem i = new BItem();
            XmlNode XlineItems = i.GetLineItems(conn, trans, accountNo, accountType, countryCode, agreementNo);
            int loanID = StockItemCache.Get(StockItemKeys.LOAN);
            //aabbcc
            string xPath = "//Item[@ItemId='" + loanID + "']";
            XmlNode loanItem = XlineItems.SelectSingleNode(xPath);

            int? staxID = StockItemCache.Get(StockItemKeys.STAX);                           //IP - 05/12/11 - #8778 
            XmlNode staxItem = null;

            if (staxID.HasValue)
            {
                string xPathSTAX = "//Item[@ItemId='" + staxID + "']";
                staxItem = XlineItems.SelectSingleNode(xPathSTAX);
            }

            // #13075 - get taxexempt flag
            DataTable dt = null;
            var acct = new BAccount();
            dt = acct.GetAccountForRevision(conn, trans, accountNo, agreementNo);
         
            foreach (DataRow row in dt.Rows)
            {
                DutyFree = Convert.ToBoolean(row["dutyfree"]);
                TaxExempt = Convert.ToBoolean(row["taxexempt"]);
            }              

            //if exclusive then update the tax rate 
            if ((string)Country[CountryParameterNames.AgreementTaxType] == "E" && staxItem != null)   //IP - 05/12/11 - #8778
            {
                Context.ExecuteTx((ctx, connection, transaction) =>
               {
                   var lineitems = (ctx.LineItem
                          .Where(c => c.acctno == accountNo
                            && c.agrmtno == agreementNo)
                                  ).AnsiToList(ctx);
                   decimal taxamt = 0.0m;
                   foreach (var item in lineitems)
                   {
                       var bitem = new BItem();
                       string xPath2 = "//Item[@ItemId='" + item.ItemID + "']";
                       XmlNode ItemX = XlineItems.SelectSingleNode(xPath2);
                       string type = ItemX.Attributes[Tags.Type].Value;

                       bitem.IsKit = Convert.ToInt16(ItemX.Attributes[Tags.Type].Value == IT.Kit ? 1 : 0);      //#15492
                       taxamt += bitem.CalculateTaxAmount(Convert.ToDecimal(item.price), type == IT.Discount,
                        Convert.ToDecimal(item.taxrate),
                       Convert.ToDecimal(item.quantity), item.ItemID, TaxExempt, Convert.ToDecimal(item.taxamt));
                   }
                   int Taxitem = StockItemCache.Get(StockItemKeys.STAX);
                   xPath = "//Item[@ItemId='" + Taxitem + "']";
                   XmlNode XTaxItem = XlineItems.SelectSingleNode(xPath);

                   XTaxItem.Attributes[Tags.Value].Value = Convert.ToString(taxamt);
                   XTaxItem.Attributes[Tags.UnitPrice].Value = Convert.ToString(taxamt);

                   var taxitem = (ctx.LineItem
                          .Where(c => c.acctno == accountNo
                            && c.agrmtno == agreementNo && c.ItemID == Taxitem)
                                  ).AnsiFirstOrDefault(ctx);
                   taxitem.price = taxamt;
                   taxitem.ordval = taxamt;
                   ctx.SubmitChanges();
               }, null, conn, trans);

            }

            if (loanItem != null)
            {
                XlineItems.RemoveChild(loanItem);
            }

            if (XlineItems != null)  //IP - 27/05/11 - #3749 - merged fix from 5.4.4 update
            {

                DeliverNonStocks(conn, trans, accountNo, accountType, countryCode, XlineItems, branchNo, refNo, ref transValue, agreementNo);
            }
        }

        //IP - 11/10/11 - #3921 - CR1232
        //This method will only deliver the Cash Loan item on an account. Done seperatly from all other non stocks on the account 
        //as the transaction will not go through as DEL but as CLD for just the Cash Loan item.
        public void DeliverCashLoan(SqlConnection conn, SqlTransaction trans,
            string accountNo, string accountType,
            string countryCode, short branchNo,
            int refNo, ref decimal transValue,
            int agreementNo, Blue.Cosacs.Shared.CashLoanDisbursementDetails cashLoanDisbursementDet)
        {
            BItem i = new BItem();
            XmlNode lineItems = i.GetCashLoanItem(conn, trans, accountNo, accountType, countryCode, agreementNo);

            if (lineItems != null)  //IP - 27/05/11 - #3749 - merged fix from 5.4.4 update
            {
                //Write CashLoanDisbursement record
                new AccountRepository().CashLoanDisbursementWrite(conn, trans, cashLoanDisbursementDet,agreementNo);
                DeliverNonStocks(conn, trans, accountNo, accountType, countryCode, lineItems, branchNo, refNo, ref transValue, agreementNo);
            }
        }

        /* this method will recurse through all lineitems in the document */
        private void DeliverNonStocks(SqlConnection conn, SqlTransaction trans,
                   string accountNo, string accountType,
                   string countryCode, XmlNode related,
                   short branchNo, int refNo, ref decimal transValue,
                   int agreementNo)
        {
            DBranch b = new DBranch();

            foreach (XmlNode item in related.ChildNodes)
            {
                XmlNode parent = null;
                if (item.ParentNode.ParentNode != null)
                    parent = item.ParentNode.ParentNode;

                if (item.Attributes[Tags.Type].Value != IT.Stock &&
                   item.Attributes[Tags.Type].Value != IT.Component &&
                   item.Attributes[Tags.Type].Value != IT.Kit)
                {
                    if (StockItemCache.Get(StockItemKeys.REFINCR) != 0 &&
                        Convert.ToInt32(item.Attributes[Tags.ItemId].Value) == StockItemCache.Get(StockItemKeys.REFINCR))
                        refNo = b.GetTransRefNo(conn, trans, branchNo);
                    // #414889 only deliver Warranty if Cash&Go sale - Warranties delivered in New WarrantyRepository for Deliveries from Warehouse 
                    if (item.Attributes[Tags.Type].Value != IT.Warranty || 
                        (item.Attributes[Tags.Type].Value == IT.Warranty && accountType==AT.Special))
                        {
                            BuildNonStock(conn, trans, parent, item, accountNo,
                                  branchNo, refNo, countryCode, ref transValue, agreementNo);
                        }
                }

                DeliverNonStocks(conn, trans, accountNo, accountType,
                    countryCode, item.SelectSingleNode(Elements.RelatedItem),
                    branchNo, refNo, ref transValue, agreementNo);
            }
        }

        /// <summary>
        /// This routine is very similar to a routine with the same name in
        /// BAccount. This routine is used when making deliveries and collections
        /// (DeliverNonStocks, DeliverImmediately, CollectWarrantyOnly, DeliverAffinity).
        /// The BAccount routine is used when creating a new account or saving a revised
        /// account. The BDelivery routine will deliver all value control and DT items
        /// and any other orphan non-stock items. An item with a parent must have the
        /// parent already delivered.
        /// </summary>
        public void BuildNonStock(SqlConnection conn, SqlTransaction trans,
            XmlNode parent, XmlNode child,
            string accountNo, short branchNo,
            int refNo, string countryCode,
            ref decimal transValue, int agreementNo)
        {
            decimal parentQty = 0;
            decimal parentDel = 0;
            decimal itemValue = 0;
            decimal itemQty = 0;
            decimal delValue = 0;
            decimal delQty = 0;
            decimal difference = 0;
            decimal qtyToDeliver = 0;
            BItem lineItem = new BItem();
            bool valueControlled = false;
            bool parentDelivered = false;
            DateTime dateTrans = DateTime.Now;
            int parentsParentID = 0;                    //IP - 13/06/11 - CR1212 - RI

            int Buffno = 0;
            DBranch branch = new DBranch();
            if (this.UseExistingTransaction == true)
                Buffno = branch.GetBuffNo(conn, trans,branchNo);

            string itemType = child.Attributes[Tags.Type].Value;
            string parentType = "";
            if (parent != null)
                parentType = parent.Attributes[Tags.Type].Value;

            lineItem.Delivery = new DDelivery();

            if (parent != null && parentType != IT.Kit)
            {
                if (parentType == IT.Component && parent.ParentNode.ParentNode.Attributes[Tags.ItemId] != null)     //IP - 13/06/11 - CR1212 - RI
                {
                    parentsParentID = Convert.ToInt32(parent.ParentNode.ParentNode.Attributes[Tags.ItemId].Value);
                }
                //IP/JC - 26/06/12 - #10520 - get the parentid of a related item
                else if (parentType == IT.Stock && parent.Attributes[Tags.ParentItemId] != null)
                {
                    parentsParentID = Convert.ToInt32(parent.Attributes[Tags.ParentItemId].Value);
                }

                parentQty = Convert.ToDecimal(parent.Attributes[Tags.Quantity].Value);
                // uat363 rdb, would need to get parent of parent item here,	I think this will always be ""
                lineItem.Delivery.GetDeliveredQuantity(conn, trans, accountNo, agreementNo, Convert.ToInt32(parent.Attributes[Tags.ItemId].Value), //IP - 16/05/11 - CR1212 - CR1212 - #3627 - Use ItemID
                    Convert.ToInt16(parent.Attributes[Tags.Location].Value),
                    parent.Attributes[Tags.ContractNumber].Value, parentsParentID); //IP - 16/05/11 - CR1212 - CR1212 - #3627 - Use 0 for ParentItemID //IP - 13/06/11 - CR1212 - RI - Pass in ParentItemID of the parent as it may be a kit which is the parent of a component
                parentDel = Convert.ToDecimal(lineItem.Delivery.DeliveredQuantity);
                parentDelivered = (parentDel == parentQty);
            }

            valueControlled = Convert.ToBoolean(child.Attributes[Tags.ValueControlled].Value);
            //ffgg
            //what is the delivered qty of the child
            //string parentItemNo = string.Empty;
            int parentItemID = 0;   //IP - 16/05/11 - CR1212 - #3627

            if (parent != null)
                //parentItemNo = parent.Attributes[Tags.Code].Value;
                parentItemID = Convert.ToInt32(parent.Attributes[Tags.ItemId].Value);  //IP - 16/05/11 - CR1212 - #3627

            lineItem.Delivery.GetDeliveredQuantity(conn, trans, accountNo, agreementNo, Convert.ToInt32(child.Attributes[Tags.ItemId].Value),   //IP - 16/05/11 - CR1212 - #3627 - Use ItemID
                Convert.ToInt16(child.Attributes[Tags.Location].Value),
                child.Attributes[Tags.ContractNumber].Value, parentItemID);
            delQty = Convert.ToDecimal(lineItem.Delivery.DeliveredQuantity);

            if (parent != null && parentType != IT.Kit)
            {
                /* if valueControlled deliver the difference between the new value and 
                    * the delivered value */
                if (valueControlled || Convert.ToString(child.Attributes[Tags.Type].Value) == "Installation" 
                        || Convert.ToString(child.Attributes[Tags.Type].Value) == IT.AssemblyCost
                        || Convert.ToString(child.Attributes[Tags.Type].Value) == IT.AnnualService
                        || Convert.ToString(child.Attributes[Tags.Type].Value) == IT.GenericService)  //IP/JC - 12/01/12 - #9440
                {
                    decimal qty = 1;                            //IP - 16/11/11 - #8625 - LW74243

                    delValue = lineItem.Delivery.GetDeliveredValue(conn, trans,
                        accountNo, agreementNo,
                        Convert.ToInt32(child.Attributes[Tags.ItemId].Value),
                        Convert.ToInt16(child.Attributes[Tags.Location].Value),
                        child.Attributes[Tags.ContractNumber].Value, Convert.ToInt32(parent.Attributes[Tags.ItemId].Value));   //IP - 16/05/11 - CR1212 - #3627 - Use ItemID and ItemID of the parent

                    itemValue = Convert.ToDecimal(child.Attributes[Tags.Value].Value);

                    difference = itemValue - delValue;

                    //IP - 16/11/11 - #8625 - LW74243 - If discount is being collected qty is negative. This will mark the discount as DelorColl = C
                    if ((Convert.ToString(child.Attributes[Tags.Type].Value) == "Discount" || Convert.ToString(child.Attributes[Tags.Type].Value) == "Installation"
                            || Convert.ToString(child.Attributes[Tags.Type].Value) == IT.AssemblyCost
                            || Convert.ToString(child.Attributes[Tags.Type].Value) == IT.AnnualService
                            || Convert.ToString(child.Attributes[Tags.Type].Value) == IT.GenericService) && itemValue == 0 && difference != 0) //IP/JC - 12/01/12 - #9440
                    {
                        qty = -1;
                    }

                    //IP - 29/04/08 - UAT(333) - Added 'parentDel > 0' so that discount is collected in GRT when processing a 'Cancellation'.
                    if (Math.Abs(difference) > (decimal).01 && (parentDelivered || parentDel > 0))
                    {
                        /* always deliver the difference (even if it's negative) */
                        //qtyToDeliver = 1;
                        qtyToDeliver = qty;                     //IP - 16/11/11 - #8625 - LW74243 

                        BDelivery d = new BDelivery(conn, trans, accountNo, Convert.ToDouble(qty), branchNo, User, child, refNo, Math.Round(difference, 2), Buffno, agreementNo, this.DateDel); //#14772 //IP - 16/11/11 - #8625 - LW74243 - previously passed in quantity of 1
                        transValue += difference;
                    }
                }
                else	/* if not valueControlled deliver the difference between the new qty and 
				* the delivered qty */
                {
                    itemQty = Convert.ToDecimal(child.Attributes[Tags.Quantity].Value);
                    difference = itemQty - delQty;

                    if (difference > 0 && parentDelivered && !Redelivery)
                    {
                        qtyToDeliver = difference;
                        BDelivery d = new BDelivery(conn, trans,
                            accountNo, (double)difference,
                            branchNo, this.User, child, refNo, Buffno, agreementNo, this.DateDel);  //#14772
                        transValue += Convert.ToDecimal(child.Attributes[Tags.UnitPrice].Value) * difference;
                    }

                    if (difference < 0 && Buffno == 0) // Preventing locking issue - but Buffno should never be >0 for collectwarranty.
                    {
                        CollectWarranty(conn, trans, Convert.ToInt32(child.Attributes[Tags.ItemId].Value),                   //IP - 16/05/11 - CR1212 - #3627 - Changed to use ItemID
                            accountNo, agreementNo,
                            (short)Convert.ToInt16(child.Attributes[Tags.Location].Value),
                            (double)difference, (string)child.Attributes[Tags.ContractNumber].Value,
                            refNo, branchNo, Convert.ToInt32(parent.Attributes[Tags.ItemId].Value), ref transValue);         //IP - 16/05/11 - CR1212 - #3627 - Changed to use ItemID of the parent
                    }
                }
            }
            else
            {
                /* here we handle orphaned non-stocks (DT/SD, account level discounts) 
                    * NB - these are valueControlled */
                if (valueControlled ||
                    Convert.ToInt32(child.Attributes[Tags.ItemId].Value).In
                    (
                        StockItemCache.Get(StockItemKeys.DT),
                        StockItemCache.Get(StockItemKeys.InsuranceChargeItem),
                        StockItemCache.Get(StockItemKeys.AdminChargeItem)
                    ))
                {

                    delValue = lineItem.Delivery.GetDeliveredValue(conn, trans,
                        accountNo, agreementNo,
                        Convert.ToInt32(child.Attributes[Tags.ItemId].Value),                           //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID                
                        Convert.ToInt16(child.Attributes[Tags.Location].Value),
                        child.Attributes[Tags.ContractNumber].Value, parentItemID);                     //IP - 17/05/11 - CR1212 - #3627 - Changed to use parentItemID rather than parentItemNo
                    itemValue = Convert.ToDecimal(child.Attributes[Tags.Value].Value);
                    difference = itemValue - delValue;

                    if (Math.Abs(difference) > (decimal).01)
                    {
                        qtyToDeliver = 1;
                        BDelivery d = new BDelivery(conn, trans, accountNo, 1, branchNo,
                            User, child, refNo, Math.Round(difference, 2), Buffno, agreementNo, this.DateDel        //#14772
                           );
                        transValue += difference;
                    }
                }
                else
                {
                    itemQty = Convert.ToDecimal(child.Attributes[Tags.Quantity].Value);
                    difference = itemQty - delQty;

                    if (Math.Abs(difference) > (decimal).01)
                    {
                        qtyToDeliver = difference;
                        BDelivery d = new BDelivery(conn, trans,
                            accountNo, (double)difference,
                            branchNo, this.User, child, refNo, Buffno, agreementNo, this.DateDel);          //#14772
                        transValue += Convert.ToDecimal(child.Attributes[Tags.UnitPrice].Value) * difference;
                    }
                }
            }
            DLineItem li = new DLineItem();
            li.UpdateDelQty(conn, trans, accountNo,
                Convert.ToInt16(child.Attributes[Tags.Location].Value),
                agreementNo, Convert.ToInt32(child.Attributes[Tags.ItemId].Value),               //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID
                child.Attributes[Tags.ContractNumber].Value,
                (double)qtyToDeliver, parentItemID);                             //IP - 17/05/11 - CR1212 - #3627 - changed to use parentItemID
        }

        /// <summary>
        /// GetCODCharges
        /// </summary>
        /// <param name="acctno">string</param>
        /// <param name="buffno">int</param>
        /// <param name="agrmtno">int</param>
        /// <param name="totalamountdue">double</param>
        /// <param name="nonstocktotal">double</param>
        /// <returns>void</returns>
        /// 
        public void GetCODCharges(SqlConnection conn, SqlTransaction trans, string acctno, int buffno, int agrmtno, out decimal totalamountdue,
            out decimal nonstocktotal, out bool cod, System.DateTime dateReqDel, string timeReqDel, string addtype)
        {

            DDelivery da = new DDelivery();
            da.GetCODCharges(conn, trans, acctno, buffno, agrmtno, out totalamountdue, out nonstocktotal, out cod, dateReqDel, timeReqDel, addtype, 0);

        }

        public short PartialDeliveryCheck(SqlConnection conn, SqlTransaction trans, string accountNo, decimal agreementTotal, double delPCent)
        {
            //DAccount account = new DAccount(conn, trans, accountNo);
            BTransaction finTrans = new BTransaction();

            finTrans.GetByAcctNo(conn, trans, accountNo);

            /* need to add a check to pre-empt divide by zero error */
            decimal delPercentage = 0;
            if (agreementTotal != 0)
                delPercentage = (finTrans.DelTot / agreementTotal) * 100;

            if ((double)delPercentage > delPCent)
                return 1;
            else
                return 0;
        }

        public void CollectAssociatedItems(SqlConnection conn, SqlTransaction trans,
            string accountNo, int itemID,                           //IP - 17/05/11 - CR1212 - #3627 - Changed itemNo to itemID
            short stockLocn, int agreementNo,
            double quantity, ref DAgreement agree)
        {
            //bool warrantiesRemoved = false;

            DSchedule sched = new DSchedule();
            sched.AccountNumber = accountNo;
            sched.AgreementNumber = agreementNo;
            //sched.ItemNumber = itemNo;
            sched.ItemID = itemID;                   //IP - 17/05/11 - CR1212 - #3627
            sched.StockLocation = stockLocn;
            sched.GetScheduledAssociatedItems(conn, trans);

            associatedItems.Clear();
            associatedItems = sched.Schedules.Copy();

            DLineItem item = new DLineItem();
            foreach (DataRow row in sched.Schedules.Rows)
            {
                //  uat363 need to get the parentItemNo here , the itemNo above is actually the parentItemNo
                item.GetSingleItem(conn, trans, Convert.ToInt32(row[CN.StockLocn]), Convert.ToInt32(row[CN.ItemId]), accountNo, agreementNo, (string)row[CN.ContractNo], itemID);   //IP - 17/05/11 - CR1212 - #3627 - changed to use ItemID rather than ItemNo
                foreach (DataRow r in item.ItemDetails.Rows)
                {
                    //if((string)r[CN.ParentItemNo] == itemNo &&
                    //    (short)r[CN.ParentLocation] == stockLocn )
                    if (Convert.ToInt32(r[CN.ParentItemId]) == itemID &&                //IP - 17/05/11 - CR1212 - #3627 - Changed to use ParentItemID
                        (short)r[CN.ParentLocation] == stockLocn && Convert.ToString(r[CN.ContractNo]) == "")       //#15073 - exclude warranty
                    {
                        /* must reduce the agreement.cashprice by the amount we're
                            * removing so the agreement total comes out right */
                        agree.AuditSource = AS.GRTCancel;
                        agree.CashPrice -= Convert.ToDecimal(r[CN.Price]) * Convert.ToDecimal(r[CN.Quantity]);
                        agree.AgreementTotal -= Convert.ToDecimal(r[CN.Price]) * Convert.ToDecimal(r[CN.Quantity]);

                        if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                        {
                            agree.AgreementTotal -= Convert.ToDecimal(r[CN.TaxAmt]);
                            agree.CashPrice -= Convert.ToDecimal(r[CN.TaxAmt]);
                        }

                        item.User = this.User;
                        item.AuditSource = AS.GRTCancel;

                        //  uat363 need to get the parentItemNo here , the itemNo above is actually the parentItemNo
                        item.UpdateItemQuantity(conn, trans, accountNo,
                            agreementNo, Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row[CN.StockLocn]),
                            Convert.ToString(r[CN.ContractNo]), 0, itemID);                              //IP - 17/05/11 - CR1212 - #3627 - Changed to use itemID rather than itemNo

                        // write a record to lineitem audit to record
                        // the warranty collection
                        item.AccountNumber = accountNo;
                        item.AgreementNumber = agreementNo;
                        item.ItemNumber = (string)row[CN.ItemNo];
                        item.ItemID = Convert.ToInt32(row[CN.ItemId]);                  //IP - 17/05/11 - CR1212 - #3627
                        item.StockLocation = Convert.ToInt16(row[CN.StockLocn]);
                        item.ContractNo = Convert.ToString(row[CN.ContractNo]);

                        item.UpdateLineItemAudit(conn, trans, item.Quantity, 0, item.OrderValue, 0, item.TaxAmount, 0);

                        WarrantyRemoved = true;
                    }
                }

                sched.AccountNumber = accountNo;
                sched.AgreementNumber = agreementNo;
                sched.ItemNumber = (string)row[CN.ItemNo];
                sched.ItemID = Convert.ToInt32(row[CN.ItemId]);             //IP - 17/05/11 - CR1212 - #3627
                sched.StockLocation = Convert.ToInt16(row[CN.StockLocn]);
                sched.BuffBranchNo = 0;
                sched.BuffNo = 0;
                sched.DeleteSchedule(conn, trans, false);
            }
        }

        public void CollectNonStockOnly(SqlConnection conn, SqlTransaction trans,
            string countryCode, short branchNo, int empeeNo,
            string accountNo, int agreementNo, DataTable nonStockList,
            bool collection, string auditSource, DAgreement agreement)
        {
            // COLLECT or REPLACE non-stock

            DBranch branch = new DBranch();
            int transRefNo = branch.GetTransRefNo(conn, trans,branchNo);  //#15993
            decimal transValue = 0;
            DAccount account = new DAccount(conn, trans, accountNo);
            //DAgreement agreement = new DAgreement(conn, trans, accountNo, agreementNo);
            DLineItem item = new DLineItem();
            this.User = empeeNo;

            foreach (DataRow nonStockRow in nonStockList.Rows)
            {
                string parentItemNo = nonStockRow["ParentItemNo"].ToString();
                int parentItemID = Convert.ToInt32(nonStockRow[CN.ParentItemId]);       //IP - 17/05/11 - CR1212 - #3627

                if ((string)nonStockRow[CN.DelOrColl] == "C")
                {
                    //
                    // COLLECT non-stock
                    //
                    // LineItem - reduce the Quantity (but not the DelQty)
                    // The difference between delivered qty and qty will be 
                    // dealt with in DeliverNonStocks.

                    // Reduce the Agreement Total
                    agreement.AgreementTotal -= Convert.ToDecimal(nonStockRow[CN.OrdVal]);
                    agreement.CashPrice -= Convert.ToDecimal(nonStockRow[CN.OrdVal]);

                    // Set the deposit value to 0 as we are reducing the cash
                    // price, and if the deposit is greater than the cash price 
                    // the installment amounts will not be calculated.
                    // 68296 RD 12/06/06No longer setting deposit to zero as causing error on only collecting warranty
                    //agreement.Deposit = 0;

                    if ((string)Country[CountryParameterNames.AgreementTaxType] == "E")
                    {
                        agreement.AgreementTotal -= Convert.ToDecimal(nonStockRow[CN.TaxAmt]);
                        agreement.CashPrice -= Convert.ToDecimal(nonStockRow[CN.TaxAmt]);
                    }

                    // Reduce the Quantity
                    // uat363 rdb need to get the parentItemNo here
                    item.GetSingleItem(conn, trans,
                        Convert.ToInt32(nonStockRow[CN.StockLocn]), Convert.ToInt32(nonStockRow[CN.ItemId]),
                        accountNo, agreementNo, (string)nonStockRow[CN.ContractNo], parentItemID); //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNo and parentItemNo

                    item.User = empeeNo;
                    item.AuditSource = AS.GRTCancel;

                    // Save zero quantity (also updates OrdVal and TaxAmt)

                    item.UpdateItemQuantity(conn, trans, accountNo, agreementNo,
                        Convert.ToInt32(nonStockRow[CN.ItemId]),
                        Convert.ToInt16(nonStockRow[CN.StockLocn]),
                        Convert.ToString(nonStockRow[CN.ContractNo]), 0, parentItemID);  //IP - 17/05/11 - CR1212 - #3627 - Changed to use ItemID and parentItemID rather than ItemNo and parentItemNo

                    // Save details to the LineItemAudit table to ensure the
                    // correct information is displayed on the Agreement Audit
                    // tab in the Account Details screen.
                    item.AccountNumber = accountNo;
                    item.AgreementNumber = agreementNo;
                    item.ItemID = Convert.ToInt32(nonStockRow[CN.ItemId]);              //IP - 09/06/11 - CR1212 - RI
                    item.ItemNumber = (string)nonStockRow[CN.ItemNo];
                    item.StockLocation = Convert.ToInt16(nonStockRow[CN.StockLocn]);
                    item.ContractNo = Convert.ToString(nonStockRow[CN.ContractNo]);

                    // 67977 RD 22/02/06 Added taxamout
                    item.UpdateLineItemAudit(conn, trans, item.Quantity, 0, item.OrderValue, 0, item.TaxAmount, 0);

                    collection = true;
                    auditSource = AS.GRTCancel;

                    if (Convert.ToDecimal(nonStockRow[CN.Refund]) > 0)
                    {
                        string type = (string)nonStockRow[CN.RefundType] == "E" ? TransType.ElecWarrantyRecovery : TransType.FurnWarrantyRecovery;
                        int refNo = branch.GetTransRefNo(conn,trans,(short)branchNo); //#15993

                        BTransaction tr = new BTransaction(conn, trans, accountNo, branchNo,
                            refNo, Convert.ToDecimal(nonStockRow[CN.Refund]), empeeNo, type,
                            "", "", "", 0, countryCode, DateTime.Now, "", agreementNo);
                    }

                   //#18605 - CR15594
                   if (Convert.ToBoolean(nonStockRow["ReadyAssist"]))
                   {
                        var acctRepos = new AccountRepository();

                        var status = ReadyAssistStatus.Cancelled;
                        decimal unusedPortion = 0;

                        if(Convert.ToDecimal(nonStockRow["ReadyAssistUsed"]) > 0)
                        {
                            unusedPortion = Convert.ToDecimal(Convert.ToDecimal(nonStockRow["OrdVal"]) - Convert.ToDecimal(nonStockRow["ReadyAssistUsed"]));
                        }
                        else
                        {
                            unusedPortion = Convert.ToDecimal(nonStockRow["OrdVal"]);
                        }


                        acctRepos.UpdateReadyAssistUnusedPortion(conn, trans, accountNo, agreementNo, unusedPortion);
                        acctRepos.UpdateReadyAssistStatus(conn, trans, accountNo, agreementNo, status);
                   }

                    //#18607 - CR15594 - Post debit to account for used portion of Ready Assist
                    if (Convert.ToDecimal(nonStockRow["ReadyAssistUsed"]) > 0)
                    {
                        string type = AT.IsCreditType(account.AccountType) ? TransType.ReadyAssistRecoveryCredit : TransType.ReadyAssistRecoveryCash;       //#19267
                        int refNo = branch.GetTransRefNo(conn, trans, (short)branchNo);

                        BTransaction tr = new BTransaction(conn, trans, accountNo, branchNo,
                            refNo, Convert.ToDecimal(nonStockRow["ReadyAssistUsed"]), empeeNo, type,
                            "", "", "", 0, countryCode, DateTime.Now, "", agreementNo);
                    }

                    if (Convert.ToDecimal(nonStockRow["AnnualServiceContractUsed"]) > 0)
                    {
                        string type = AT.IsCreditType(account.AccountType) ? TransType.AnnualServiceRecoveryCredit : TransType.AnnualServiceRecoveryCash;
                        int refNo = branch.GetTransRefNo(conn, trans, (short)branchNo);

                        BTransaction tr = new BTransaction(conn, trans, accountNo, branchNo,
                            refNo, Convert.ToDecimal(nonStockRow["AnnualServiceContractUsed"]), empeeNo, type,
                            "", "", "", 0, countryCode, DateTime.Now, "", agreementNo);
                    }

                }
                else
                {
                    // REPLACE non-stock
                    //
                    // LineItem - just comment the line item as replaced.
                    item.ProcessReplacement(conn, trans, accountNo, agreementNo,
                        //(string)nonStockRow[CN.ItemNo], 
                        Convert.ToInt32(nonStockRow[CN.ItemId]),                        //IP - 09/06/11 - CR1212 - RI
                        Convert.ToInt16(nonStockRow[CN.StockLocn]),
                        -1,
                        (string)nonStockRow[CN.EmployeeNo],
                        (string)nonStockRow[CN.ContractNo],
                        Convert.ToInt16(nonStockRow[CN.RetStockLocn]), true);
                }
            }

            agreement.AuditSource = auditSource;

            if (AT.IsCreditType(account.AccountType) && collection)
            {
                // Recalculate Service Charge using new OrdVal amounts
                // This expects the agreement cash price to exclude service charges
                decimal chargeableAdminPrice = 0;
                agreement.CashPrice = account.GetChargeableCashPrice(conn, trans, accountNo, ref chargeableAdminPrice);
                // This will save the agreement with a new total
                RecalculateServiceCharge(conn, trans, account, agreement);
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
            // This will call 'CollectWarranty' and will deliver new DT etc
            DeliverNonStocks(conn, trans, accountNo, account.AccountType,
                countryCode, branchNo, transRefNo, ref transValue, agreementNo);


            // FinTrans
            // Fintrans delivery record for all items
            BTransaction t = null;
            if (Math.Abs(transValue) > 0)
                t = new BTransaction(conn, trans, accountNo, branchNo,
                    transRefNo, transValue, empeeNo,
                    TransType.GoodsReturn, "", "", "", 0, countryCode,
                    DateTime.Today, FootNote.NonStockCollection, agreementNo);

            // Update Return Item No.
            foreach (DataRow nonStockRow in nonStockList.Rows)
            {
                item.AccountNumber = accountNo;
                item.AgreementNumber = agreementNo;
                item.ItemNumber = (string)nonStockRow[CN.ItemNo];
                item.StockLocation = Convert.ToInt16(nonStockRow[CN.StockLocn]);
                item.ContractNo = (string)nonStockRow[CN.ContractNo];
                item.ReturnItemNumber = (string)nonStockRow[CN.RetItemNo];
                item.ReturnStockLocn = Convert.ToInt16(nonStockRow[CN.RetStockLocn]);
                //uat363 rdb add parentItemNo
                item.ParentItemNumber = nonStockRow[CN.ParentItemNo].ToString();

                item.ItemID = Convert.ToInt32(nonStockRow[CN.ItemId]);
                item.ParentItemID = Convert.ToInt32(nonStockRow[CN.ParentItemId]);
                item.ReturnItemId = Convert.ToInt32(nonStockRow[CN.RetItemId]);

                item.UpdateItemReturnNo(conn, trans);
            }
        }

        private void CollectWarranty(SqlConnection conn, SqlTransaction trans,
            int itemID, string accountNo, int agreementNo,
            short locn, double qty, string contractNo, int refNo,
            short branchNo, int parentItemID, ref decimal transValue)   //IP - 16/05/11 - CR1212 - #3627 - Changed to pass in itemID and parentItemID
        {
            //string retCode = "";
            decimal transVal = 0;

            DDelivery del = new DDelivery();
            DataTable delTab = new DataTable("Deliveries");
            DataSet ds = new DataSet();

            // RD/PN 27/07/06 68129 Moved out of the loop as causing error on collecting warranty only
            delTab.Columns.Add("acctno");
            delTab.Columns.Add("agrmtno");
            delTab.Columns.Add("buffbchno");
            delTab.Columns.Add("delorcoll");
            delTab.Columns.Add("itemno");
            delTab.Columns.Add(CN.ItemId);       //IP - 16/05/11 - CR1212 - #3627
            delTab.Columns.Add("stocklocn");
            delTab.Columns.Add("datedel");
            delTab.Columns.Add("datetrans");
            delTab.Columns.Add("transvalue");
            delTab.Columns.Add("quantity");
            delTab.Columns.Add("retval");
            delTab.Columns.Add("retitemno");
            delTab.Columns.Add(CN.RetItemId);   //IP - 17/05/11 - CR1212 - #3627
            delTab.Columns.Add("retstocklocn");
            delTab.Columns.Add(CN.ContractNo);
            delTab.Columns.Add(CN.Reason);
            // uat363 rdb add parentitemno
            delTab.Columns.Add(CN.ParentItemNo);
            delTab.Columns.Add(CN.ParentItemId); //IP - 16/05/11 - CR1212 - #3627  

            del.GetDeliveriesForAccount(conn, trans, accountNo, agreementNo);

            var lastContractNo = string.Empty;                                              //IP - 09/01/12 - #3482 - LW73223

            foreach (DataRow r in del.Deliveries.Rows)
            {
                //if (itemNo == (string)r[CN.ItemNo] && (string)r[CN.DelOrColl] == "D"
                //    && (string)r[CN.ContractNo] == contractNo && (string)r[CN.ParentItemNo] == paremtItemNo)
                if (itemID == Convert.ToInt32(r[CN.ItemId]) && (string)r[CN.DelOrColl] == "D"
                   && (string)r[CN.ContractNo] == contractNo && Convert.ToInt32(r[CN.ParentItemId]) == parentItemID               //IP - 16/05/11 - CR1212 - #3627
                    && (string)r[CN.ContractNo] != lastContractNo)                          //IP - 09/01/12 - #3482 - LW73223 - prevent duplicate rows where > 1 delivery of warranty with same contract number
                {
                    if (isReplacement)
                        transVal = 0;
                    else
                        transVal = 0 - Convert.ToDecimal(r[CN.Value]);

                    DataRow newRow;
                    newRow = delTab.NewRow();

                    newRow["acctno"] = accountNo;
                    newRow["agrmtno"] = agreementNo;
                    newRow["buffbchno"] = Convert.ToInt32(branchNo);
                    newRow["delorcoll"] = "C";
                    //newRow["itemno"] = itemNo;   
                    newRow["itemno"] = Convert.ToString(r[CN.ItemNo]);   //IP - 17/05/11 - CR1212 - #3627
                    newRow[CN.ItemId] = itemID;                          //IP - 17/05/11 - CR1212 - #3627
                    newRow["stocklocn"] = locn;
                    newRow["datedel"] = DateTime.Today;
                    newRow["datetrans"] = DateTime.Now; //IP - 18/09/09 - UAT5.2 UAT(877) Was previously DateTime.Today, changed to DateTime.Now;
                    newRow["transvalue"] = transVal;
                    newRow["quantity"] = qty;
                    newRow["retval"] = transVal;
                    //newRow["retitemno"] = Convert.ToString(GetReturnDetails(itemNo,locn,contractNo,true));
                    newRow["retitemno"] = string.Empty;     //IP - 17/05/11 - CR1212 - #3627 - Now RetItemId is used, therefore just setting retitemno to string.empty
                    newRow[CN.RetItemId] = GetReturnDetails(itemID, locn, contractNo, true);       //IP - 17/05/11 - CR1212 - #3627
                    newRow["retstocklocn"] = GetReturnDetails(itemID, locn, contractNo, false);       //IP - 17/05/11 - CR1212 - #3627 - Changed itemNo to itemID
                    newRow[CN.ContractNo] = contractNo;
                    newRow[CN.Reason] = "";
                    //newRow[CN.ParentItemNo] = paremtItemNo;
                    newRow[CN.ParentItemNo] = Convert.ToString(r[CN.ParentItemNo]);
                    newRow[CN.ParentItemId] = parentItemID;         //IP - 17/05/11 - CR1212 - #3627

                    delTab.Rows.Add(newRow);

                    transValue += transVal;

                    lastContractNo = contractNo;                        //IP - 09/01/12 - #3482 - LW73223
                }
            }
            // RD/PN 68129 27/06/07 Taken out of loop as was causing error
            ds.Tables.Add(delTab);
            Save(conn, trans, ds, refNo);
        }

        public string FormatWarrantyReturnCode(string itemNo, DateTime deliveryDate)
        {
            string warrantyReturnCode = "";
            string expire = "";

            //use regular expressions to determine which warranty category key is being collected

            //3 year electrical - 19xxx3
            Regex threeYrElec = new Regex("^19.*3$"); /* ^ (starts with) 19 (literal) .(any character) *(zero or more) 3$ (ends with 3)	*/

            //5 year electrical - 19xxx5
            Regex fiveYrElec = new Regex("^19.*5$"); /* ^ (starts with) 19 (literal) .(any character) *(zero or more) 5$ (ends with 5)	*/

            //3 year furniture - XWxxx3
            Regex threeYrFurn = new Regex("^XW.*3$"); /* ^ (starts with) XW (literal) .(any character) *(zero or more) 3$ (ends with 3)	*/

            //5 year furniture - XWxxx5
            Regex fiveYrFurn = new Regex("^XW.*5$"); /* ^ (starts with) XW (literal) .(any character) *(zero or more) 5$ (ends with 5)	*/

            //2 year mobile phone warranty codes - 19xxx2
            Regex twoYrMobile = new Regex("^19.*2$"); /* ^ (starts with) 19 (literal) .(any character) *(zero or more) 2$ (ends with 2)	*/

            //declare warranty return code format for each category key

            //3 year electrical - 1980xx
            string retThreeYrElec = "1980";

            //5 year electrical - 1986xx
            string retFiveYrElec = "1986";

            //3 year furniture - 1985xx
            string retThreeYrFurn = "1985";

            //5 year furniture - 1987xx
            string retFiveYrFurn = "1987";

            if (Config.CountryCode == "F")
                retFiveYrElec = "1982";

            //2 year mobile phone return warranty codes - 1981xx
            string retTwoYrMobile = "1981";

            // warranty being collected so calculate expired portion of the warranty
            if (!isReplacement)
            {
                int y = DateTime.Now.Year - deliveryDate.Year;
                int m = DateTime.Now.Month - deliveryDate.Month;
                int d = DateTime.Now.Day - deliveryDate.Day;

                if (d < 0)
                    m--;

                if (m < 0)
                {
                    y--;
                    m += 12;
                }

                int elapsedMonths = y * 12 + m;

                if (elapsedMonths < 2)
                    // UAT 9 - Round up to one month but round down over one month
                    expire = "01";
                else if (elapsedMonths < 10)
                    expire = "0" + elapsedMonths.ToString();
                else
                    expire = elapsedMonths.ToString();

                if (threeYrElec.IsMatch(itemNo))
                {
                    if (elapsedMonths > 36) expire = "36";
                    warrantyReturnCode = retThreeYrElec + expire;
                }
                if (fiveYrElec.IsMatch(itemNo))
                {
                    if (elapsedMonths > 60) expire = "60";
                    warrantyReturnCode = retFiveYrElec + expire;
                }
                if (threeYrFurn.IsMatch(itemNo))
                {
                    if (elapsedMonths > 36) expire = "36";
                    warrantyReturnCode = retThreeYrFurn + expire;
                }
                if (fiveYrFurn.IsMatch(itemNo))
                {
                    if (elapsedMonths > 60) expire = "60";
                    warrantyReturnCode = retFiveYrFurn + expire;
                }
                if (twoYrMobile.IsMatch(itemNo))
                {
                    if (elapsedMonths > 24) expire = "24";
                    warrantyReturnCode = retTwoYrMobile + expire;
                }
            }
            //replacing item so must use last warranty return code
            else
            {
                if (threeYrElec.IsMatch(itemNo))
                    warrantyReturnCode = retThreeYrElec + "36";

                if (fiveYrElec.IsMatch(itemNo))
                    warrantyReturnCode = retFiveYrElec + "60";

                if (threeYrFurn.IsMatch(itemNo))
                    warrantyReturnCode = retThreeYrFurn + "36";

                if (fiveYrFurn.IsMatch(itemNo))
                    warrantyReturnCode = retFiveYrFurn + "60";

                if (twoYrMobile.IsMatch(itemNo))
                    warrantyReturnCode = retTwoYrMobile + "24";
            }
            return warrantyReturnCode;
        }

        public string GetWarrantyReturnCode(string acctno, short stocklocn, short replacement,
            //string parentItemNo, string WarrantyNo, string contractNo,
            int parentItemID, int warrantyItemID, string contractNo,                              //IP - 21/06/11 - CR1212 - RI - #3939
         out string returnWarranty, out decimal refund, out DateTime deliveryDate, out int warrantyRetCodeItemID)   //IP - 13/09/11 - RI - #8112 - added warrantyRetCodeItemID
        {
            DDelivery del = new DDelivery();
            returnWarranty = "";
            refund = 0;
            warrantyRetCodeItemID = 0;      //IP - 13/09/11 - RI - #8112

            //del.GetWarrantyReturnCode(null, null, acctno, stocklocn, replacement, parentItemNo, 
            //    WarrantyNo, contractNo, out returnWarranty, out refund,out deliveryDate);

            del.GetWarrantyReturnCode(null, null, acctno, stocklocn, replacement, parentItemID,      //IP - 21/06/11 - CR1212 - RI - #3939
                warrantyItemID, contractNo, out returnWarranty, out refund, out deliveryDate, out warrantyRetCodeItemID);   //IP - 13/09/11 - RI - #8112 - added warrantyRetCodeItemID

            return returnWarranty;
        }

        public DataSet GetRepossessedItemDetails(string acctno)
        {

            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            //ds.Tables.Add(del.GetRepossessedItemDetails(acctno));
            ds=del.GetRepossessedItemDetails(acctno);       // #14927

            ds.Tables[0].TableName = TN.Accounts;
            ds.Tables[1].TableName = "AddrTypes";

            return ds;
        }
        public void ScheduleRedelRepo(SqlConnection conn, SqlTransaction trans,
                            short origbr, string acctno, DateTime datedelplan, char delorcoll,
                            int itemID, short stocklocn, short quantity, short retstocklocn,
                            int retItemID, decimal retval, int buffbranchno, int buffno,
                            string delArea, int agrmtNo, string contractNo, int parentItemID, int lineItemId, string deliveryAddress)  // #14927
        {
            DDelivery da = new DDelivery();
            da.ScheduleRedelRepo(conn, trans, origbr, acctno, datedelplan, delorcoll, itemID,
                                 stocklocn, (short)-quantity, retstocklocn, retItemID, -(retval),
                                 buffbranchno, buffno, delArea, agrmtNo, contractNo, parentItemID, lineItemId); //IP - 15/06/12 - added lineItemId //IP - 26/05/11 - CR1212 - RI - #3636

            if (delorcoll == 'S' || delorcoll == 'I')               // #10686 jec  IP - 12/06/12 - #10357 - Warehouse & Deliveries
            {
                var lineItemBooking = new DataTable();
                lineItemBooking.Columns.Add("id");
                lineItemBooking.Columns.Add("QtyBooked");
                lineItemBooking.Columns.Add("BookingID");

                var dr = lineItemBooking.NewRow();
                dr["id"] = lineItemId;
                dr["QtyBooked"] = Math.Abs(quantity);
                dr["BookingID"] = 0;

                lineItemBooking.Rows.Add(dr);

                var AcctR = new AccountRepository();
                AcctR.bookingType = "R";
                AcctR.InsertLineItemBooking(conn, trans, ref lineItemBooking);

                new WarehouseRepository().UpdateLineItemBookingScheduleBookingId(conn, trans, lineItemId, Convert.ToInt32(lineItemBooking.Rows[0]["BookingID"]));   //IP - 15/06/12 - #10387 - Update LineItemBookingSchedule with the BookingId

                var bookings = (IEnumerable<BookingSubmit>)AcctR.GetBookingData(conn, trans, lineItemBooking, user: User, deliveryAdr : deliveryAddress);   // #14927

                //#19638
                if (bookings.First().PickUp == false)
                {
                    bookings.First().RequestedDate = datedelplan;
                }
         
                new Chub().SubmitMany(bookings, conn, trans);
            }
        }
        public DataSet GetTransportList()
        {

            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            ds.Tables.Add(del.GetTransportList());

            return ds;
        }
        public DataSet GetScheduledLoads(short branchNo, DateTime dateFrom, DateTime dateTo,
            short printed, short loadNo, short withSchedules)
        {

            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            ds.Tables.Add(del.GetScheduledLoads(branchNo, dateFrom, dateTo, printed, loadNo, withSchedules));

            return ds;
        }
        public DataSet GetLoadContents(short loadNo, DateTime dateDel, short branchNo)
        {

            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            ds.Tables.Add(del.GetLoadContents(loadNo, dateDel, branchNo));

            return ds;
        }
        public int TransportSchedAdd(SqlConnection conn, SqlTransaction trans, short branchNo, DateTime dateDel, short loadNo, string TruckId)
        {
            DDelivery del = new DDelivery();
            int result = del.TransportSchedAdd(conn, trans, branchNo, dateDel, loadNo, TruckId, 0);

            return result;
        }
        public int DeliveryScheduleUpdate(SqlConnection conn, SqlTransaction trans, int loadNo, int buffNo, int filter, int branchNo, int pickListNo, DateTime dateDel)
        {
            DDelivery del = new DDelivery();
            int result = del.DeliveryScheduleUpdate(conn, trans, loadNo, buffNo, filter, branchNo, pickListNo, dateDel);

            return result;
        }
        public DataSet LoadAvailablePicklists(short branchNo, char type)
        {
            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            ds.Tables.Add(del.LoadAvailablePicklists(branchNo, type));

            return ds;
        }
        public DataSet GetDeliveryScheduleDetails(short branchNo, DateTime dateDel, short loadNo)
        {
            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            ds.Tables.Add(del.GetDeliveryScheduleDetails(branchNo, dateDel, loadNo));

            return ds;
        }
        public int RemoveLoadFromContents(SqlConnection conn, SqlTransaction trans,
            DateTime dateDel, short stockLocn, int buffNo, short loadNo,
            string accountNo, string notes, int empeeNo)
        {
            DSchedule sched = new DSchedule();
            sched.AccountNumber = accountNo;
            sched.StockLocation = stockLocn;
            sched.BuffNo = buffNo;
            sched.ItemID = 0;
            sched.PicklistNumber = 0;
            sched.PicklistBranchNumber = 0;
            sched.UpdateScheduleForPicklist(conn, trans, "O");
            int result = sched.RemoveLoadFromContents(conn, trans, dateDel, stockLocn, buffNo, loadNo, empeeNo);

            DBailAction ba = new DBailAction();
            ba.AccountNo = accountNo;
            ba.EmployeeNo = empeeNo;
            ba.DateAdded = DateTime.Today;
            ba.Code = "DNR";
            ba.DateDue = dateDel;
            //ba.ActionValue = Convert.ToDouble(del.TransValue);
            ba.AddedBy = empeeNo;
            ba.Notes = notes;
            ba.Save(conn, trans);
            return result;
        }

        private int GetReturnDetails(int itemID, short locn, string contractNo, bool isItem) //IP - 17/05/11 - CR1212 - #3627 - Changed itemNo to itemID
        {
            int? returnVal = null;
            foreach (DataRow r in associatedItems.Rows)
            {
                //if((string)r[CN.ItemNo] == itemNo &&
                if (Convert.ToInt32(r[CN.ItemId]) == itemID &&          //IP - 17/05/11 - CR1212 - #3627
                    Convert.ToInt32(r[CN.StockLocn]) == locn &&
                    (string)r[CN.ContractNo] == contractNo)
                {
                    if (isItem)
                        //returnObject = r[CN.RetItemNo];
                        returnVal = Convert.ToString(r[CN.RetItemId]).TryParseInt32();                 //IP - 17/05/11 - CR1212 - #3627
                    else
                        returnVal = Convert.ToString(r[CN.RetStockLocn]).TryParseInt32();
                    break;
                }
            }

            return returnVal.HasValue ? returnVal.Value : 0;
        }

        public void AuditDeliveryReprint(SqlConnection conn, SqlTransaction trans,
            string accountNo, int agreementNo, int itemId,
            short stockLocn, int buffNo, int printedBy)
        {
            DDelivery deliveryAudit = new DDelivery();
            deliveryAudit.AuditDeliveryReprint(conn, trans, accountNo, agreementNo, itemId, stockLocn, buffNo, printedBy);
        }

        public DataSet SUCBGetDelTotals(int runno, SqlConnection conn, SqlTransaction trans, out decimal delTotal)
        {
            DataSet ds = new DataSet();
            DDelivery del = new DDelivery();
            del.SUCBGetDelTotals(runno, conn, trans, out delTotal);
            ds.Tables.Add(del.Deliveries);
            return ds;
        }

        //public DataSet SUCBGetDelDetails(int runno, string branchNo)
        public DataSet SUCBGetDelDetails(string datetrans, string branchNo)                           //IP - 20/02/12 - #9423 - CR8262
        {
            DataSet ds = new DataSet();
            DDelivery del = new DDelivery();
            //del.SUCBGetDelDetails(runno, branchNo);
            del.SUCBGetDelDetails(datetrans, branchNo);                                                 //IP - 20/02/12 - #9423 - CR8262
            ds.Tables.Add(del.Deliveries);
            return ds;
        }

        public DataSet GetExchangeDetails(string acctNo, int agrmtNo)
        {
            DataSet ds = new DataSet();
            DDelivery del = new DDelivery();
            del.GetExchangeDetails(acctNo, agrmtNo);
            ds.Tables.Add(del.Deliveries);
            return ds;
        }

        public bool IsDotNetWarehouse(short branchNo)
        {
            DDelivery del = new DDelivery();
            return del.IsDotNetWarehouse(branchNo);
        }

        public DataSet GetDeliverySchedules(DateTime fromDate,
            DateTime toDate,
            string deliveryArea,
            int includeDeliveries,
            int includeCollections,
            string majorProductCategory,
            string minorProductCategory,
            string acctNo,
            int user,
            int branchNo,
            int delNotBranchNo,
            string truckID,
            bool includeAssembly,
            bool includeNonAssembly,
            out DateTime timeLocked)
        {
            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            del.GetDeliverySchedules(fromDate,
                toDate,
                deliveryArea,
                includeDeliveries,
                includeCollections,
                majorProductCategory,
                minorProductCategory,
                acctNo,
                user,
                branchNo,
                delNotBranchNo,
                truckID,
                includeAssembly,
                includeNonAssembly,
                out timeLocked);

            ds.Tables.Add(del.Deliveries);
            return ds;
        }

        public DataSet LoadAvailableTransPicklists(short branchNo)
        {
            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            ds.Tables.Add(del.LoadAvailableTransPicklists(branchNo));

            return ds;
        }

        public void DeleteDeliveryLoad(SqlConnection conn, SqlTransaction trans, short stockLocn, int buffNo)
        {
            DDeliveryLoad del = new DDeliveryLoad();
            del.DeleteByBuffNo(conn, trans, stockLocn, buffNo);
        }

        public DataSet GetIRItems(string acctNo, string custID, int buffNo,
                        DateTime dateFrom, DateTime dateTo, string acctType)
        {
            DataSet ds = new DataSet();

            DDelivery del = new DDelivery();
            del.GetIRItems(acctNo, custID, buffNo, dateFrom, dateTo, acctType);
            ds.Tables.Add(del.Deliveries);

            return ds;
        }

        public void SetDelDateForExchange(SqlConnection conn, SqlTransaction trans,
                                            string acctNo, int itemId, short locn, int agrmtNo)
        {
            DDelivery del = new DDelivery();
            del.GetExchangeWarranty(conn, trans, acctNo, agrmtNo);

            if (del.Deliveries.Rows.Count > 0)
            {
                DLineItem line = new DLineItem();
                DataTable dtLineDetails = line.GetChildLineItemCodes(conn, trans, acctNo, agrmtNo, itemId, locn);

                foreach (DataRow lineRow in dtLineDetails.Rows)
                {
                    foreach (DataRow exchangeRow in del.Deliveries.Rows)
                    {
                        if (Convert.ToInt32(lineRow[CN.ItemId]) == Convert.ToInt32(exchangeRow[CN.WarrantyId]) &&
                            (short)lineRow[CN.StockLocn] == (short)exchangeRow[CN.WarrantyLocation] &&
                            (string)lineRow[CN.ContractNo] == (string)exchangeRow[CN.ContractNo])
                        {
                            del.AccountNumber = acctNo;
                            del.AgreementNumber = agrmtNo;
                            del.ItemNumber = (string)lineRow[CN.ItemNo];
                            del.ItemId = Convert.ToInt32(lineRow[CN.ItemId]);
                            del.StockLocation = (short)lineRow[CN.StockLocn];
                            del.ContractNo = (string)lineRow[CN.ContractNo];
                            del.UpdateDeliveryDate(conn, trans);
                        }
                    }
                }
            }
        }
    }
}
