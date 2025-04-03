using System;
using System.Collections.Generic;
using System.Text;
using STL.Common;
using STL.DAL;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.Common.Constants.StoreInfo;
using STL.Common.Constants.Tags;
using System.Xml;
using System.Globalization;
using STL.Common.Printing;
using Blue.Cosacs.Repositories;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for BBranch.
    /// </summary>
    public class BPrinting : CommonObject
    {
        public void Verifyfile(ref DateTime modified, ref double size, ref string path, string filename, string serverdir, string countrycode)
        {
            Dprinting DP = new Dprinting();
            DP.VerifyFile(ref modified, ref size, ref path, filename, serverdir, countrycode);
        }

        //public string FindPath(string homepath, string filename)
        //{
        //    Dprinting DP = new Dprinting();
        //    return DP.FindPath(homepath, filename);
        //}

        public List<PrintingDN> GetDNPrintInfo(DNparameters[] DNinput)
        {

            List<PrintingDN> output = new List<PrintingDN>();
            bool reprint = false;

            try
            {
                List<DNparameters> inputlist = new List<DNparameters>();
                inputlist.AddRange(DNinput);

                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

                    foreach (DNparameters input in inputlist)
                    {
                        BAccount acct = new BAccount();
                        BLogin login = new BLogin();
                        BDelivery deliveryAudit = new BDelivery();
                        DCustomer cust = new DCustomer();
                        DSchedule sched = new DSchedule();

                        PrintingDN DNinfo = new PrintingDN();

                        cust.GetCustomerAddresses(conn, trans, input.custID);
                        DNinfo.Customer.Tables.Add(cust.Addresses);
                        DNinfo.Customer.Tables[DNinfo.Customer.Tables.Count - 1].TableName = "Address";


                        //login = new BLogin();
                        DNinfo.empname = login.GetEmployeeName(conn, trans, input.userSale);
                        DNinfo.printedby = login.GetEmployeeName(conn, trans, input.user);

                        //retrieve the rest of the required data
                        //acct = new BAccount();

                        decimal amount;
                        decimal charges;
                        bool cod;

                        //if (Convert.ToString(input.printText) == GetResource("T_REPRINT"))
                        if (Convert.ToString(input.printText) == "REPRINT")
                        {
                            DNinfo.Customer.Tables.Add(acct.GetReprintDetails(conn, trans, input.acctno.Replace("-", ""), input.buffLocn, input.delnotenum,
                                input.dateReqDel, input.delAddressType, input.timeReqDel, out amount, out charges, out cod));
                            DNinfo.Customer.Tables[DNinfo.Customer.Tables.Count - 1].TableName = "Lineitem";
                            reprint = true;

                        }
                        else
                        {
                            DNinfo.Customer.Tables.Add(acct.GetDeliveryNotes(input.acctno.Replace("-", ""), conn, trans, input.buffLocn, input.delnotenum,
                                                              input.dateReqDel, input.delAddressType, input.timeReqDel,
                                                              out amount, out charges, out cod));
                            DNinfo.Customer.Tables[DNinfo.Customer.Tables.Count - 1].TableName = "Lineitem";

                        }
                        DNinfo.amountPayable = amount;
                        DNinfo.charges = charges;
                        DNinfo.cod = cod;

                        foreach (DataRow row in DNinfo.Customer.Tables["Lineitem"].Rows)
                        {
                            // Add the next line item to the Delivery Note

                            //IP - 12/03/08 - (69461) added 'lineItems.Rows.Count' to hold a count of the items to be printed on the delivery note.

                            deliveryAudit.AuditDeliveryReprint(conn, trans, (string)row[CN.AcctNo], Convert.ToInt32(row[CN.AgrmtNo]),
                                Convert.ToInt32(row[CN.ItemId]), Convert.ToInt16(row[CN.StockLocn]), input.delnotenum, input.user);

                            sched.User = input.user;
                            sched.SetDeliveryNotesPrinted(conn, trans, input.acctno.Replace("-", ""), input.delnotenum, input.buffLocn);


                        }
                        output.Add(DNinfo);
                    }

                    bool found = false;
                    if (!reprint)
                    {
                        foreach (PrintingDN DN in output)
                        {
                            foreach (DataRow item in DN.Customer.Tables["Lineitem"].Rows)
                            {
                                if (item["scheduledateprinted"] != DBNull.Value)
                                {
                                    found = true;
                                }
                            }
                        }
                    }

                    if (found && !reprint)
                    {
                        trans.Rollback();
                        conn.Close();
                        output = null;

                    }
                    else
                    {
                        trans.Commit();
                        conn.Close();

                    }
                }
            }
            catch
            {
                throw;
            }
            return output;
        }

        public PrintingAgreementResult GetAgreeementPrint(PrintingAgreementRequest input, string countryName)
        {
            PrintingAgreementResult output = new PrintingAgreementResult();

            try
            {

                DataTable joint;

                output.JointID = "";
                output.JointName = "";
                output.relationship = "";

                XmlNode[] addToItems;

                output.itemsPerPage = 10;

                try
                {
                    output.itemsPerPage = Convert.ToDouble(ConfigurationSettings.AppSettings["agreementLineItems"]);
                    output.noCopies = Convert.ToInt32(Country[CountryParameterNames.NoAgrCopies]);
                    output.custCopies = Convert.ToInt32(Country[CountryParameterNames.AgreementCustomerCopies]);
                    output.AgrTimePrint = Convert.ToBoolean(Country[CountryParameterNames.AgrTimePrint]);
                    output.localformat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                    output.percenttopay = Convert.ToDecimal(Country[CountryParameterNames.PercentToPay]);
                    output.IncInsinServAgrPrint = Convert.ToBoolean(Country[CountryParameterNames.IncInsinServAgrPrint]);
                    output.Print90 = Convert.ToInt16(Country[CountryParameterNames.Print90]);
                    output.CountryName = countryName;
                }
                catch { }



                BCustomer bCustomer = new BCustomer();
                output.customer = bCustomer.GetBasicCustomerDetails(null, null, input.customerID, input.accountNo, "H");

                /* retrieve the service percentage */
                BAccount bAccount = new BAccount();
                bAccount.Populate(input.accountNo);

                /* if there is a joint applicant for credit, include their name */
                if (AT.IsCreditType(input.accountType))

                //if (AccountType != AT.Cash && AccountType != AT.Special) //Acct Type Translation DSR 29/9/03
                {
                    output.JointName = bAccount.GetApplicantTwoName(input.customerID, input.accountNo, out output.JointID, out output.relationship);
                    if (output.JointName.Length > 0)
                    {
                        joint = bCustomer.GetDetails(output.JointID);
                        joint.TableName = "joint";
                        output.customer.Tables.Add(joint);
                    }
                }

                DTermsType dTermsType = new DTermsType();
                dTermsType.GetTermsTypeDetail(null, null, input.countrycode, bAccount.TermsType, bAccount.AccountNumber, "", bAccount.DateAccountOpen);

                output.ServicePCent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0]["servpcent"]);
                output.TermsDescription = (string)dTermsType.TermsTypeDetails.Rows[0][CN.Description];
                output.PaymentHolidays = Convert.ToBoolean(dTermsType.TermsTypeDetails.Rows[0][CN.PaymentHoliday]);
                output.PaymentHolidaysMin = (short)dTermsType.TermsTypeDetails.Rows[0][CN.PaymentHolidayMin];
                output.AgreementText = (string)dTermsType.TermsTypeDetails.Rows[0][CN.AgreementText];
                output.insPcent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0][CN.InsPcent]);
                output.insIncluded = Convert.ToBoolean(dTermsType.TermsTypeDetails.Rows[0][CN.InsIncluded]);

                DBranch branch = new DBranch();

                BranchRepository br = new BranchRepository();
                var nonCourtsStoreType = br.GetNonCourtsStoreType(Convert.ToInt16(input.accountNo.Substring(0, 3)));

                string countryCode = (string)Country[CountryParameterNames.CountryCode];

                if (branch.GetStoreType(Convert.ToInt16(input.accountNo.Substring(0, 3))) == StoreType.NonCourts && (nonCourtsStoreType.LuckyDollarStore != null && nonCourtsStoreType.LuckyDollarStore == true))
                {
                    output.filename = "Agreement_LD.xslt";
                }
                else if (branch.GetStoreType(Convert.ToInt16(input.accountNo.Substring(0, 3))) == StoreType.NonCourts && (nonCourtsStoreType.AshleyStore != null && nonCourtsStoreType.AshleyStore == true))
                {
                    output.filename = "Agreement_Ashley.xslt";
                }
                else if (branch.GetStoreType(Convert.ToInt16(input.accountNo.Substring(0, 3))) == StoreType.NonCourts &&
                         (nonCourtsStoreType.RadioShackStore != null && nonCourtsStoreType.RadioShackStore == true))
                {
                    output.filename = "Agreement_Radioshack.xslt";
                }
                else if (branch.GetStoreType(Convert.ToInt16(input.accountNo.Substring(0, 3))) == StoreType.NonCourts && (nonCourtsStoreType.DisplayType == "Omni"))
                {
                    output.filename = "Agreement_Omni.xslt";
                }
                else if (countryCode.Trim() == "Q" || countryCode.Trim() == "B" || countryCode.Trim() == "X")
                {
                    output.filename = "Agreement_CUR.xslt";
                }
                else
                {
                    output.filename = (string)dTermsType.TermsTypeDetails.Rows[0][CN.AgreementPrintDesc];
                }

                /* load the agreement table stuff */
                BAgreement bAgreement = new BAgreement();
                output.agreement = bAgreement.GetAgreement(null, null,input.accountNo, 1, false); //IP - 11/02/11 - Sprint 5.10 - #2978 - Added null, null for conn, trans
                output.PaymentHolidaysMax = (short)output.agreement.Tables[TN.Agreements].Rows[0][CN.PaymentHoliday];

                /* load the line items for the account */
                BItem bItem = new BItem();
                DAccount dAccount = new DAccount();

                if (dAccount.IsWarrantyRenewal(input.accountNo))
                    output.lineitems = bItem.GetWarrantyRenewalItems(input.accountNo, input.accountType, input.countrycode, 1);
                else
                    output.lineitems = bItem.GetLineItems(null, null, input.accountNo, input.accountType, input.countrycode, 1);

                /* flag all lineitems as not belonging to an added on account */
                foreach (XmlNode i in output.lineitems.ChildNodes)
                    FlagItems(i, Boolean.FalseString, input.accountNo);

                /* if this account has been added to */
                if (output.lineitems.SelectSingleNode("//Item[@Quantity != '0' and @Code = 'ADDDR']") != null)
                {
                    DataTable addedToAccts = bAccount.GetAccountsAddedTo(null, null, input.accountNo);
                    addToItems = new XmlNode[addedToAccts.Rows.Count];
                    for (int x = 0; x < addedToAccts.Rows.Count; x++)
                        addToItems[x] = bItem.GetLineItems(null, null, (string)addedToAccts.Rows[x][CN.AccountNumber], input.accountNo, input.countrycode, 1);

                    /* need to try and combine the lineitem nodes */
                    XmlNode addcr = output.lineitems.SelectSingleNode("//Item[@Quantity != '0' and @Code = 'ADDDR']");
                    addcr.Attributes[STL.Common.Constants.Tags.Tags.Description1].Value = GetResource("M_BALANCEFROMPREV");
                    for (int i = 0; i < addToItems.Length; i++)
                    {
                        for (int a = 0; a < addToItems[i].ChildNodes.Count; a++)
                        {
                            XmlNode addNode = addToItems[i].ChildNodes[a];
                            if (Convert.ToInt32(addNode.Attributes[Tags.ItemId].Value) != StockItemCache.Get(StockItemKeys.ADDCR))
                            {
                                addNode = output.lineitems.OwnerDocument.ImportNode(addNode, true);
                                FlagItems(addNode, Boolean.TrueString, (string)addedToAccts.Rows[i][CN.AccountNumber]);	/* flag as add tos */
                                output.lineitems.InsertAfter(addNode, addcr);
                            }
                        }
                    }
                }


                int monthsintfree;
                int monthsdeferred;

                dAccount = new DAccount();
                dAccount.SelectType(null, null, input.accountType, input.countrycode, out monthsintfree, out monthsdeferred);

                bItem = new BItem();

                DInstalPlan ip = new DInstalPlan();
                ip.Populate(null, null, input.accountNo, 1);
                output.months = ip.NumberOfInstalments += monthsdeferred - monthsintfree;

                output.chargeablePrice = bAccount.GetChargeableCashPrice(null, null, input.accountNo, ref output.chargeableAdminPrice);

                BInstalPlan bInstalPlan = new BInstalPlan();
                DataSet variableInstalPlanSet = bInstalPlan.GetVariableInstalmentsByAcctNo(input.accountNo);
                if (variableInstalPlanSet.Tables[TN.VariableInstal] != null)
                {
                    output.customer.Tables.Add(variableInstalPlanSet.Tables[TN.VariableInstal]);
                }



            }
            catch
            {
                throw;
            }

            return output;
        }

        private void FlagItems(XmlNode item, string flag, string accountNo)
        {
            item.Attributes.Append(item.OwnerDocument.CreateAttribute(Tags.AddTo));
            item.Attributes[Tags.AddTo].Value = flag;
            item.Attributes.Append(item.OwnerDocument.CreateAttribute("ACCTNO"));
            item.Attributes["ACCTNO"].Value = accountNo;
            foreach (XmlNode child in item.SelectSingleNode(STL.Common.Constants.Elements.Elements.RelatedItem).ChildNodes)
                FlagItems(child, flag, accountNo);
        }

        public List<PrintingDN> GetDNPrintScheduleItems(short loadNo, short branchNo, DateTime dateDel, int printedby)
        {

            SqlTransaction trans;
            List<PrintingDN> Printlist = new List<PrintingDN>();
            DCustomer cust = new DCustomer();
            DSchedule sched = new DSchedule();
            DDelivery del = new DDelivery();
            BDelivery deliveryAudit = new BDelivery();
            BLogin login = new BLogin();

            try
            {

                using (SqlConnection conn = new SqlConnection(Connections.Default))
                {
                    conn.Open();
                    using (trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        DataTable loadschedule = del.GetDeliveryScheduleCustomerDetails(conn, trans, loadNo, branchNo, dateDel);

                        foreach (DataRow order in loadschedule.Rows)
                        {
                            PrintingDN Dnote = new PrintingDN();

                            Dnote.alias = order[CN.Alias].ToString();
                            Dnote.customername = order[CN.CustomerName].ToString();
                            Dnote.acctno = order[CN.AcctNo].ToString();
                            Dnote.buffno = order[CN.BuffNo].ToString();
                            Dnote.locn = order[CN.BuffBranchNo].ToString();

                            cust.GetCustomerAddresses(conn, trans, order[CN.CustID].ToString());
                            Dnote.Customer.Tables.Add(cust.Addresses);
                            Dnote.Customer.Tables[Dnote.Customer.Tables.Count - 1].TableName = "Address";
                            sched.GetDeliveryNotes(conn, trans, Convert.ToInt32(order[CN.BuffNo]), Convert.ToInt32(order[CN.BuffBranchNo]));
                            Dnote.Customer.Tables.Add(sched.Schedules);
                            Dnote.Customer.Tables[Dnote.Customer.Tables.Count - 1].TableName = "Lineitem";

                            decimal amount;
                            decimal charges;
                            bool cod;

                            if (sched.Schedules.Rows.Count > 0)
                            {
                                del.GetCODCharges(conn, trans, (string)order[CN.AcctNo], Convert.ToInt32(order[CN.BuffNo]),
                                                  1, out amount, out charges, out  cod,
                                                  Convert.ToDateTime(sched.Schedules.Rows[0][CN.DateReqDel]),
                                                  (string)sched.Schedules.Rows[0][CN.TimeReqDel],
                                                  (string)sched.Schedules.Rows[0][CN.DeliveryAddress],
                                                  Convert.ToInt32(order[CN.BuffBranchNo]));

                                Dnote.amountPayable = amount;
                                Dnote.charges = charges;
                                Dnote.cod = cod;
                            }

                            foreach (DataRow row in Dnote.Customer.Tables["Lineitem"].Rows)
                            {
                                // Add the next line item to the Delivery Note

                                //IP - 12/03/08 - (69461) added 'lineItems.Rows.Count' to hold a count of the items to be printed on the delivery note.
                                deliveryAudit.AuditDeliveryReprint(conn, trans,
                                                               (string)row[CN.AcctNo],
                                                               Convert.ToInt32(row[CN.AgrmtNo]),
                                                               Convert.ToInt32(row[CN.ItemId]),
                                                               Convert.ToInt16(row[CN.StockLocn]),
                                                               Convert.ToInt32(order[CN.BuffNo]),
                                                               printedby);

                                sched.User = printedby;
                                sched.SetDeliveryNotesPrinted(conn, trans, (string)row[CN.AcctNo],
                                               Convert.ToInt32(order[CN.BuffNo]),
                                               Convert.ToInt32(order[CN.BuffBranchNo]));

                                Dnote.user = Convert.ToInt32(row[CN.EmpeeNoSale]);

                            }
                            Dnote.empname = login.GetEmployeeName(conn, trans, Dnote.user);
                            Dnote.printedby = login.GetEmployeeName(conn, trans, printedby);
                            Printlist.Add(Dnote);
                        }

                        bool found = false;


                        foreach (PrintingDN DN in Printlist)
                        {
                            foreach (DataRow item in DN.Customer.Tables["Lineitem"].Rows)
                            {
                                if (item["scheduledateprinted"] != DBNull.Value &&
                                    item["delorcoll"].ToString().Trim().ToUpper() == "D" &&
                                    Convert.ToInt32(item["quantity"]) > 0)
                                {
                                    found = true;
                                }
                            }
                        }

                        if (found)
                        {
                            trans.Rollback();
                            conn.Close();
                            if (Printlist != null)
                                Printlist.Clear();
                        }
                        else
                        {
                            trans.Commit();
                            conn.Close();

                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return Printlist;
        }


        public string GetDecimalPlaces()
        {
            return Country[CountryParameterNames.DecimalPlaces].ToString();
        }
        /// <summary>
        /// Gets printing information for Action Sheets.
        /// </summary>
        /// <param name="acctno"></param>
        /// <returns></returns>
        public PrintingAction GetASPrintInfo(string acctNo, int empeeNo, SqlConnection conn, SqlTransaction trans)
        {
            BAccount acct = new BAccount();
            BLogin login = new BLogin();
            DDelivery del = new DDelivery();
            DCustomer cust = new DCustomer();
            DEmployment emp = null;
            PrintingAction Actioninfo = new PrintingAction();
            Actioninfo.ActionSet = new DataSet();
            cust = new DCustomer();
            cust.GetDetailsForDebtCollector(acctNo);
            Actioninfo.ActionSet.Tables.Add(cust.Customer);
            Actioninfo.ActionSet.Tables[0].TableName = "Customer";

            if (Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].Rows.Count > 0)
            {
                DataRow Row = Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].Rows[0];
                Actioninfo.Arrears = (decimal)Row[CN.Arrears];
                Actioninfo.Currstatus = Row[CN.CurrStatus].ToString();
                Actioninfo.custID = Row[CN.CustID].ToString();
                Actioninfo.Outstbal = (decimal)Row[CN.OutstBal];
                if (Row[CN.Instalment] != DBNull.Value)
                    Actioninfo.instAmount = (decimal)Row[CN.Instalment];
                if (Row[CN.DateFirst] != DBNull.Value)
                    Actioninfo.day = ((DateTime)Row[CN.DateFirst]).Day;

                if (Row[CN.DateLastPaid] != DBNull.Value)
                    Actioninfo.DateLastPaid = ((DateTime)(Row[CN.DateLastPaid]));






                Actioninfo.Title = Row[CN.Title].ToString();
                Actioninfo.Name = Row[CN.Name].ToString();
                Actioninfo.Firstname = Row[CN.FirstName].ToString();
                Actioninfo.Alias = Row[CN.Alias].ToString();
            }

            string photoFileName = String.Empty;
            string serverPath = String.Empty;
            string signatureFileName = String.Empty;
            string serverSignaturePath = String.Empty;

            photoFileName = cust.GetCustomerPhoto(conn, trans, Actioninfo.custID);
            Actioninfo.Photo = photoFileName;
            serverPath = (string)Country[CountryParameterNames.PhotoDirectory];

            //Get the customer's signature and the virtual path to this signature
            signatureFileName = cust.GetCustomerSignature(conn, trans, Actioninfo.custID);
            Actioninfo.SignatureFile = signatureFileName;

            emp = new DEmployment();
            emp.CustomerID = Actioninfo.custID;
            emp.GetEmployerDetails(acctNo);





            Actioninfo.ActionSet.Tables.Add(emp.EmpDetails);
            Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].TableName = "Employer";


            DataTable otherCustomers = null;


            BCollectionsModule collection = new BCollectionsModule();
            DataSet dsFollupAlloc = collection.GetFollupAllocForActionSheet(acctNo, empeeNo);
            int Collector = 0;
            if (dsFollupAlloc.Tables.Count > 0 && dsFollupAlloc.Tables[0].Rows.Count > 0)
            {
                if (dsFollupAlloc.Tables[0].Rows[0]["empeeno"] != null) //if allocated get employee number of collector /bailiff
                    Collector = Convert.ToInt32(dsFollupAlloc.Tables[0].Rows[0]["empeeno"]);

                if (dsFollupAlloc.Tables[0].Rows[0]["DeadLinedate"] == null || dsFollupAlloc.Tables[0].Rows[0]["DeadLinedate"] == DBNull.Value)
                    Actioninfo.DeadLineDate = STL.Common.Static.Date.blankDate;
                else
                    Actioninfo.DeadLineDate = Convert.ToDateTime(dsFollupAlloc.Tables[0].Rows[0]["DeadLinedate"]);
            }

            otherCustomers = cust.GetOtherCustomers(acctNo);

            Actioninfo.ActionSet.Tables.Add(otherCustomers);
            Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].TableName = "otherCustomers";

            del = new DDelivery();
            del.GetItemsForDebtCollector(acctNo);

            Actioninfo.ActionSet.Tables.Add(del.Deliveries);
            Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].TableName = "Deliveries";

            DFinTrans fintrans = new DFinTrans();
            int privilegeCount = 0;
            fintrans.GetDetailsForDebtCollector(acctNo, out privilegeCount);
            Actioninfo.privilegeCount = privilegeCount;

            Actioninfo.ActionSet.Tables.Add(fintrans.FinTrans);
            Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].TableName = "Fintrans";

            int debitAccount = 0;
            int segmentId = 0;
            BPayment pay = new BPayment();
            string countryCode = String.Empty;
            DCountry country = new DCountry();
            countryCode = (string)Country[CountryParameterNames.CountryCode];

            decimal paymentAmount = 0; decimal collectionFee = 0; decimal bailiffFee = 0;
            pay.CalculateCreditFee(
                conn,
                trans,
                countryCode,
                acctNo,
                ref paymentAmount,
                TransType.Payment,
                ref Collector,
                Actioninfo.Arrears,
                out collectionFee,
                out bailiffFee,
                out debitAccount,
                out segmentId);

            Actioninfo.paymentAmount = paymentAmount;
            Actioninfo.collectionFee = collectionFee;
            Actioninfo.bailiffFee = bailiffFee;

            cust.GetCustomerAddresses(null, null, Actioninfo.custID);

            Actioninfo.ActionSet.Tables.Add(cust.Addresses);
            Actioninfo.ActionSet.Tables[Actioninfo.ActionSet.Tables.Count - 1].TableName = "Address";


            return Actioninfo;
        }

        //public PrintingDN GetDNSchedulePrintInfo(PrintDNScheduleInput.parameters input)
        //{

        //    PrintingDN DNinfo = new PrintingDN();
        //    DCustomer cust = new DCustomer();
        //    DSchedule sched = new DSchedule();
        //    DDelivery del = new DDelivery();

        //    SqlTransaction trans;
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(Connections.Default))
        //        {
        //            conn.Open();
        //            trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);

        //            cust.GetCustomerAddresses(conn, trans, input.custID);
        //            DNinfo.Customer.Tables.Add(cust.Addresses);
        //            DNinfo.Customer.Tables[DNinfo.Customer.Tables.Count - 1].TableName = "Address";
        //            sched.GetDeliveryNotes(conn, trans, input.buffno, input.buffLocn);
        //            DNinfo.DNnotes.Tables.Add(sched.Schedules);

        //            decimal amount;
        //            decimal charges;
        //            bool cod;

        //            del.GetCODCharges(conn, trans, input.custID, input.buffno,
        //                                     1, out amount, out charges, out  cod,
        //                                     Convert.ToDateTime(sched.Schedules.Rows[0][CN.DateReqDel]),
        //                                     (string)sched.Schedules.Rows[0][CN.TimeReqDel],
        //                                     (string)sched.Schedules.Rows[0][CN.DeliveryAddress],
        //                                     Convert.ToInt32(input.buffLocn));

        //            DNinfo.amountPayable = amount;
        //            DNinfo.charges = charges;
        //            DNinfo.cod = cod;

        //            int items = 0;

        //            foreach (DataRow delRow in sched.Schedules.Rows)
        //            {
        //                // Add the next line item to the Delivery Note
        //                items++;
        //                //IP - 12/03/08 - (69461) added 'dtSchedules.Rows.Count' to hold a count of the items to be printed on the delivery note.
        //                DNinfo. = (string)delRow[CN.DeliveryAddress];

        //                // Audit this line item if it has already been printed
        //                deliveryAudit.AuditDeliveryReprint(conn, trans,
        //                    (string)delRow[CN.AcctNo],
        //                    Convert.ToInt32(delRow[CN.AgrmtNo]),
        //                    (string)delRow[CN.ItemNo],
        //                    Convert.ToInt16(delRow[CN.StockLocn]),
        //                    Convert.ToInt32(row[CN.BuffNo]),
        //                    printedBy);

        //                qty += Convert.ToDouble(delRow[CN.Quantity]);
        //                delDate = ((DateTime)delRow[CN.DateReqDel]).ToLongDateString() + " " + (string)delRow[CN.TimeReqDel];
        //                user = Convert.ToInt32(delRow[CN.EmpeeNoSale]);
        //                locn = Convert.ToInt32(delRow[CN.StockLocn]);
        //                delOrColl = (string)delRow[CN.DelOrColl];
        //            }


        //            trans.Commit();
        //            conn.Close();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}


        //                           //cust = new DCustomer();
        //                         

        //                           //retrieve the rest of the required data
        //                           //sched = new DSchedule();

        //                           //get lineitems to be printed
        //                           sched.GetDeliveryNotes(conn, trans, Convert.ToInt32(row[CN.BuffNo]),
        //                                                  Convert.ToInt32(row[CN.BuffBranchNo]));

        //                           DataTable dtSchedules = sched.Schedules;
        //                           if (dtSchedules.Rows.Count > 0)
        //                           {
        //                              del.GetCODCharges(conn, trans, (string)row[CN.AcctNo], Convert.ToInt32(row[CN.BuffNo]),
        //                                                1, out amountPayable, out charges, out  cod,
        //                                                Convert.ToDateTime(dtSchedules.Rows[0][CN.DateReqDel]),
        //                                                (string)dtSchedules.Rows[0][CN.TimeReqDel],
        //                                                (string)dtSchedules.Rows[0][CN.DeliveryAddress],
        //                                                Convert.ToInt32(row[CN.BuffBranchNo]));
        //                           }

        //                           delNote.SetNode("DELIVERYNOTE/FOOTER/ADDCHARGES", charges.ToString("F2"));
        //                           delNote.SetNode("DELIVERYNOTE/FOOTER/PAYABLE", amountPayable.ToString("F2"));
        //                           delNote.SetNode("DELIVERYNOTE/FOOTER/COD", cod ? "Y" : "N");

        //                           string timeStamp = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();

        //                           //delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", branchNo.ToString());
        //                           delNote.SetNode("DELIVERYNOTE/HEADER/BUFFNO", row[CN.BuffNo].ToString());
        //                           delNote.SetNode("DELIVERYNOTE/HEADER/PRINTED", timeStamp);

        //                           int items = 0;
        //                           bool found = false;
        //                           string delOrColl = "";
        //                           //deliveryAudit = new BDelivery();

        //                           //add line items to the xml document
        //                           DataRow[] allItemRows = dtSchedules.Select();
        //                           foreach (DataRow delRow in allItemRows)
        //                           {
        //                              // Add the next line item to the Delivery Note
        //                              items++;
        //                              //IP - 12/03/08 - (69461) added 'dtSchedules.Rows.Count' to hold a count of the items to be printed on the delivery note.
        //                              delAddressType = (string)delRow[CN.DeliveryAddress];
        //                              delNote.AddLineItem(delRow, dtSchedules.Rows.Count, ref items);
        //                              // Audit this line item if it has already been printed
        //                              deliveryAudit.AuditDeliveryReprint(conn, trans,
        //                                  (string)delRow[CN.AcctNo],
        //                                  Convert.ToInt32(delRow[CN.AgrmtNo]),
        //                                  (string)delRow[CN.ItemNo],
        //                                  Convert.ToInt16(delRow[CN.StockLocn]),
        //                                  Convert.ToInt32(row[CN.BuffNo]),
        //                                  printedBy);

        //                              qty += Convert.ToDouble(delRow[CN.Quantity]);
        //                              delDate = ((DateTime)delRow[CN.DateReqDel]).ToLongDateString() + " " + (string)delRow[CN.TimeReqDel];
        //                              user = Convert.ToInt32(delRow[CN.EmpeeNoSale]);
        //                              locn = Convert.ToInt32(delRow[CN.StockLocn]);
        //                              delOrColl = (string)delRow[CN.DelOrColl];
        //                           }
        //                           // LW 69370 Use delAddressType to load tel numbers
        //                           LoadTelNumbers(cust.Addresses, delNote, delAddressType.Trim());
        //                           delNote.SetNode("DELIVERYNOTE/HEADER/DELDATE", delDate);

        //                           if (qty < 0)
        //                              delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_COLLTEXT"));
        //                           else
        //                           {
        //                              if (delOrColl == "D")
        //                                 delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_DELTEXT"));
        //                              else
        //                                 delNote.SetNode("DELIVERYNOTE/HEADER/DELTEXT", GetResource("T_REDELIVERY"));
        //                           }

        //                           delAddressType = delAddressType.Trim();
        //                           if (delAddressType != "H")
        //                              LoadAddressDetails(addrTable, delNote, delAddressType);

        //                           //login = new BLogin();
        //                           userName = login.GetEmployeeName(conn, trans, user);
        //                           printedByName = login.GetEmployeeName(conn, trans, printedBy);

        //                           delNote.SetNode("DELIVERYNOTE/HEADER/BRANCH", locn.ToString());
        //                           delNote.SetNode("DELIVERYNOTE/HEADER/LOCATION", locn.ToString());
        //                           delNote.SetNode("DELIVERYNOTE/HEADER/PRINTEDBY", printedByName);
        //                           delNote.SetNode("DELIVERYNOTE/FOOTER/USER", user.ToString() + " ");
        //                           delNote.SetNode("DELIVERYNOTE/FOOTER/USERNAME", userName);

        //                           //if (index == accounts.Rows.Count) //-- IP - 04/08/08 - UAT5.1 - UAT(508)
        //                           if (index == allAccountRows.Length) //IP - 04/08/08 - UAT5.1 - UAT(508)
        //                              delNote.SetNode("DELIVERYNOTE/LAST", "TRUE");
        //                           else
        //                           {
        //                              index++; 
        //                              delNote.SetNode("DELIVERYNOTE/LAST", "FALSE");
        //                           }

        //                           sched.User = printedBy;
        //                           sched.SetDeliveryNotesPrinted(conn, trans, (string)row[CN.AcctNo],
        //                                             Convert.ToInt32(row[CN.BuffNo]),
        //                                             Convert.ToInt32(row[CN.BuffBranchNo]));

        //                           /* import this delivery note into the main document */
        //                           dxml.ImportNode(delNote.DocumentElement, true);
        //                           trans.Commit();

        //                           allItemRows = null;
        //                           delNote = null;
        //                        }
        //                        accounts = null;
        //                        allAccountRows = null;
        //                        System.GC.Collect();
        //                     }

        //                     Response.Write(dxml.TransformXml());
        //                     break;
        //                  }
        //                  catch (SqlException ex)
        //                  {
        //                     CatchDeadlock(ex, conn);
        //                  }
        //               }
        //                }while (retries <= maxRetries);				
        //            }
        //            catch(Exception ex)
        //            {
        //                logException(ex, Function);
        //                Response.Write(ex.Message);
        //            }
        //            finally
        //            {
        //            //if(conn.State != ConnectionState.Closed)
        //            //   conn.Close();
        //            deliveryAudit = null;
        //            cust = null;
        //            sched = null;
        //            login = null;
        //            del = null;
        //            }
        //        }

        //        private void LoadCustDetails(DataTable dt, DeliveryNoteXML dxml)
        //        {
        //            foreach(DataRow row in dt.Rows)
        //            {
        //                if(((string)row[CN.Alias]).Length > 0)
        //                    dxml.SetNode("DELIVERYNOTE/HEADER/ALIAS", "ALIAS: " + (string)row[CN.Alias]);

        //                dxml.SetNode("DELIVERYNOTE/HEADER/FIRSTNAME", (string)row[CN.FirstName] + " ");
        //                dxml.SetNode("DELIVERYNOTE/HEADER/LASTNAME", (string)row[CN.LastName]);
        //            }
        //        }

        //        private void LoadAddressDetails(DataTable dt, DeliveryNoteXML dxml, string adrressType)
        //        {
        //            string addType = "";

        //            DataRow[] allAddressRows = dt.Select();
        //            foreach (DataRow row in allAddressRows)
        //            {
        //                addType = ((string)row["AddressType"]).Trim();
        //                if(addType == adrressType)
        //                {
        //                    address1 = (string)row[CN.Address1];

        //                    dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS1", address1);
        //                    dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS2", (string)row[CN.Address2]);
        //                    dxml.SetNode("DELIVERYNOTE/HEADER/ADDRESS3", (string)row[CN.Address3]);
        //                    dxml.SetNode("DELIVERYNOTE/HEADER/POSTCODE", (string)row[CN.PostCode]);
        //                    dxml.SetNode("DELIVERYNOTE/FOOTER/CUSTNOTES", (string)row["Notes"]);
        //                }
        //            }

        //            allAddressRows = null;
        //        }

        //      private void LoadTelNumbers(DataTable dt, DeliveryNoteXML dxml, string addressType)
        //        {
        //            string addType = String.Empty;

        //            DataRow[] allTelNumberRows = dt.Select();
        //            foreach (DataRow row in allTelNumberRows)
        //            {
        //                addType = ((string)row["AddressType"]).Trim();
        //            string address = addType.Substring(0, 1); // 69370 Delivery telephone number to be included
        //            switch (address)
        //                {
        //                    case "H":	
        //                        dxml.SetNode("DELIVERYNOTE/HEADER/HOMETEL", GetResource("L_HOME") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
        //                        break;
        //                    case "W":	dxml.SetNode("DELIVERYNOTE/HEADER/WORKTEL", GetResource("L_WORK") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
        //                        break;
        //                    case "M":	dxml.SetNode("DELIVERYNOTE/HEADER/MOBILE", GetResource("L_MOBILE") + ": " + (string)row["Phone"],false);
        //                        break;
        //               case "D":
        //                  if (addType == addressType)
        //                  {
        //                     dxml.SetNode("DELIVERYNOTE/HEADER/DELTEL", GetResource("L_DEL") + ": " + ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"], false);
        //                  }
        //                  break;
        //                    default:
        //                        break;
        //                }
        //            }

        //            allTelNumberRows = null;
        //        }

        //        private void LoadCancelledDelNotes(SqlConnection conn, SqlTransaction trans,
        //            DeliveryNoteXML dxml, DataRow r, ref bool found)
        //        {
        //            int buffNo = 0;
        //            DateTime datePrinted = DateTime.MinValue.AddYears(1899);

        //            BItem item = new BItem();
        //            item.GetCancelledDelNote(conn, trans,
        //                (string)r[CN.AcctNo],
        //                (int)r[CN.AgrmtNo],
        //                (string)r[CN.ItemNo],
        //                Convert.ToInt16(r[CN.StockLocn]),
        //                out buffNo,
        //                out datePrinted);
        //            if(buffNo > 0)
        //            {
        //                dxml.SetNode("DELIVERYNOTE/HEADER/CANCELTEXT", GetResource("M_DELIVERYNOTECANCEL", new object[]{buffNo.ToString(),datePrinted.ToShortDateString()}));
        //                found = true;
        //            }
        //        }
        //        #region Web Form Designer generated code
        //        override protected void OnInit(EventArgs e)
        //        {
        //            //
        //            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
        //            //
        //            InitializeComponent();
        //            base.OnInit(e);
        //        }

        //        /// <summary>
        //        /// Required method for Designer support - do not modify
        //        /// the contents of this method with the code editor.
        //        /// </summary>
        //        private void InitializeComponent()
        //        {    
        //        }
        //        #endregion
        //    }
        //}

    }
}


