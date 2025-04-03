using System;
using System.IO;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using STL.Common.Constants.Enums;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Collections.Specialized;
using STL.Common.PrivilegeClub;
using STL.Common.Static;
using System.Reflection;
using Blue.Cosacs.StoreCardUtil;
using STL.BLL.WebApi;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for BCustomer.
    /// </summary>
    public partial class BCustomer : CommonObject
    {
        private string _custID = String.Empty;
        private short _origbr = 0;
        private string _otherid = String.Empty;
        private short _branchnohdle = 0;
        private string _name = String.Empty;
        private string _firstname = String.Empty;
        private string _title = String.Empty;
        private string _alias = String.Empty;
        private string _addrsort = String.Empty;
        private string _namesort = String.Empty;
        private string _sex = String.Empty;
        private string _ethnicity = String.Empty;
        private string _morerewardsno = String.Empty;
        private DateTime _effectivedate;
        private string _idtype = String.Empty;
        private string _idnumber = String.Empty;
        private DateTime _dateborn;

        public string CustID
        {
            get { return _custID; }
            set { _custID = value; }
        }
        public short OrigBr
        {
            get { return _origbr; }
            set { _origbr = value; }
        }

        public string OtherId
        {
            get { return _otherid; }
            set { _otherid = value; }
        }

        public short BranchNoHandle
        {
            get { return _branchnohdle; }
            set { _branchnohdle = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string FirstName
        {
            get { return _firstname; }
            set { _firstname = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        public string AddressSort
        {
            get { return _addrsort; }
            set { _addrsort = value; }
        }

        public string NameSort
        {
            get { return _namesort; }
            set { _namesort = value; }
        }

        public string Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        public string Ethnicity
        {
            get { return _ethnicity; }
            set { _ethnicity = value; }
        }

        public string MoreRewardsNo
        {
            get { return _morerewardsno; }
            set { _morerewardsno = value; }
        }

        public DateTime EffectiveDate
        {
            get { return _effectivedate; }
            set { _effectivedate = value; }
        }

        public string IDType
        {
            get { return _idtype; }
            set { _idtype = value; }
        }

        public string IDNumber
        {
            get { return _idnumber; }
            set { _idnumber = value; }
        }

        public DateTime DateBorn
        {
            get { return _dateborn; }
            set { _dateborn = value; }
        }

        /// <summary>
        /// Returns a dataset containing the codes currently on a customer
        /// </summary>
        /// <param name="customerID">customerID</param>
        /// <returns></returns>
        public DataSet GetCodesForCustomer(string customerID, out bool noSuchCust)
        {
            DataSet ds = new DataSet();
            DCustomer codes = new DCustomer();
            codes.GetCodesForCustomer(customerID, out noSuchCust);
            ds.Tables.Add(codes.CustomerCodes);
            return ds;
        }

        public DataSet GetCustomerAddresses(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            DataSet ds = null;
            DCustomer addr = new DCustomer();
            if ((int)Return.Success == addr.GetCustomerAddresses(conn, trans, customerID))
            {
                ds = new DataSet();
                ds.Tables.Add(addr.Addresses);
            }
            return ds;
        }

        /// <summary>
        /// This method will update the custcatcodes table with the current selection
        /// </summary>
        /// <param name="con">SqlConnection object</param>
        /// <param name="tran">SqlTransaction object</param>
        /// <param name="customerID">customer ID</param>
        /// <param name="ds">dataset containing updates</param>
        public void AddCodesToAccount(SqlConnection con, SqlTransaction tran,
            string customerID, DataSet ds, string custCode) //IP - 01/09/09 - 5.2 UAT(823) - Added custCode
        {
            DCustomer cust = new DCustomer();

            //First delete all the codes on the customer
            cust.DeleteCodesFromCustomer(con, tran, customerID, custCode);

            //Then add all codes in the new dataset
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cust.AddCodeToCustomer(con, tran, customerID, (string)row["Code"], (DateTime)row["Date Added"], User, (string)row["Reference"]);
            }
        }


        /// <summary>
        /// Returns a dataset containing the customer details and list of accounts for a customer.
        /// </summary>
        /// <param name="customerID">customerID</param>
        /// <returns></returns>
        public DataSet GetCustomerAccountsAndDetails(SqlConnection conn, SqlTransaction trans, string accountNo)
        {
            //call web api
            bool isMambuAccount = false;
            PaymentWebApi api = new PaymentWebApi();
            DataTable dt = api.GetMambuAccountsForPayment(null, accountNo);
            if (dt.Rows.Count > 0)
            {
                isMambuAccount = true;
                string json = api.GetLoanSchedule(accountNo);//(loanAccountId);
                if (json != "Success")
                {
                    throw new STLException(json);
                }
            }

            //end api
            
            DataSet ds = new DataSet();
            DCustomer accountsAndDetails = new DCustomer();
            ds.Tables.Add(accountsAndDetails.GetCustomerAccountsAndDetails(accountNo));
            DataTable mambu = new DataTable("Mambu");
            mambu.Columns.Add("isMambuAccount", typeof(bool));
            DataRow row = mambu.NewRow();
            row["isMambuAccount"] = isMambuAccount;
            mambu.Rows.Add(row);
            ds.Tables.Add(mambu);


            return ds;
        }
        /// <summary>
        /// Returns a dataset containing the customer details .
        ///  ///          // new address pop add hear by tosif ali 27/12/2018*@
        /// </summary>
        /// <param name="customerID">customerID</param>
        /// <returns></returns>
        public DataSet GetCustomerDetails(SqlConnection conn, SqlTransaction trans, string RequestID)
        {
            DataSet ds = new DataSet();

            DCustomer cust = new DCustomer();
            if (0 == cust.GetCustomerDetailsD(conn, trans, RequestID))
            {
                DataTable details = new DataTable("BasicDetails");
                details.Columns.AddRange(new DataColumn[]   {
                                                                new DataColumn("DELTitleC"),
                                                                new DataColumn("DELFirstName"),
                                                                new DataColumn("DELLastName"),
                                                                 new DataColumn("addtype")

                                                                });

                DataRow row = details.NewRow();

                // new address pop add hear by tosif ali 16/10/2018*@
                row["DELTitleC"] = cust.DELTitleC;
                row["DELFirstName"] = cust.DELFirstName;
                row["DELLastName"] = cust.DELLastName;
                row["addtype"] = cust.addtype;
                // End Hear

                details.Rows.Add(row);
                ds.Tables.Add(details);

            }
            return ds;

        }
        public ArrayList GetCustomerAccountsDetails(string accountNo)
        {
            ArrayList customerDetails = new ArrayList();
            DCustomer accountsAndDetails = new DCustomer();
            customerDetails = accountsAndDetails.GetCustomerAccountsDetails(accountNo);

            return customerDetails;
        }

        /// <summary>
        /// Returns a dataset containing the account number, instalment and expiry date for accounts in spa which have not yet expired
        /// </summary>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public DataSet GetSPADetails(string acctNo)       //CR1084 jec
        {
            DataSet ds = new DataSet();
            DCustomer accountsSPA = new DCustomer();
            ds = accountsSPA.GetSPADetails(acctNo);        //CR1084 jec
            return ds;
        }


        /// <summary>
        /// Returns a table of customer accounts for a customer.
        /// </summary>
        /// <param name="customerID">customerID, firstName, lastName, limit, settled</param>
        /// <returns></returns>
        public DataSet CustomerSearch(string customerID,
            string firstName,
            string lastName,
            string address,     //CR1084
            string phoneNumber,     //CR1084
            int limit,
            int settled,
            bool exactMatch,
            string storeType, string source)
        {
            DataSet ds = new DataSet();
            DCustomer accounts = new DCustomer();
            accounts.CustomerSearch(customerID, firstName, lastName, address, phoneNumber, limit, settled, exactMatch, storeType, source);      //CR1084
            ds.Tables.Add(accounts.CustSearch);

            return ds;
        }

        public void SaveBasicDetails(SqlConnection conn,
                                    SqlTransaction trans,
                                    string custid,
                                    string title,
                                    string firstName,
                                    string lastName,
                                    string alias,
                                    string accountNo,
                                    string relationship,
                                    string user,
                                    DateTime dob,
                                    string accountType,
                                    string maidenName,
                                    string loyaltyCardNo,
                                    string countryCode,
                                    DataSet otherTabs,
                                    string maritalStat,
                                    int dependants,
                                    string nationality,
                                    string storeType,
                                    bool resieveSms)
        {
            DCustomer cust = new DCustomer();

            //If this customer is to be tied to an account - record the entry in the
            //custacct table.
            if (accountNo != null)
            {
                bool rescore;
                BAccount acct = new BAccount();
                acct.AddCustomerToAccount(conn, trans, accountNo, custid, relationship, accountType, out rescore);
            }
            else
                accountNo = String.Empty;

            //Call getCustomerDetails to populate the internal 
            //member variables with any existing details
            cust.GetCustomerDetails(conn, trans, custid);

            cust.CustID = custid;
            cust.Title = title;
            cust.FirstName = firstName;
            cust.Name = lastName;
            cust.Alias = alias;
            cust.DateBorn = dob;
            cust.User = Convert.ToInt32(user);
            cust.DateChanged = DateTime.Now;
            cust.MaidenName = maidenName;
            cust.Nationality = nationality;
            cust.Dependants = dependants;
            cust.MaritalStatus = maritalStat;
            cust.StoreType = storeType;
            cust.ResieveSms = resieveSms;
            cust.Save(conn, trans, custid);

            if ((bool)Country[CountryParameterNames.LoyaltyCard])
            {
                cust.UpdateLoyaltyCardNo(conn, trans, custid, loyaltyCardNo);
            }

            if (otherTabs != null)
            {

                SaveBankDetails(conn, trans, custid, accountNo, otherTabs);

                SaveEmploymentDetails(conn, trans, custid, otherTabs);
                SaveCustomerAdditionalDetailsFinancial(conn, trans, custid, accountNo, otherTabs);
                SaveCustomerAdditionalDetailsResidential(conn, trans, otherTabs);
            }

            SetAvailableSpend(conn, trans, custid);
        }
        public void SaveBankDetails(SqlConnection conn,
                                    SqlTransaction trans, string custId, string accountNo, DataSet bankDS)
        {
            if (bankDS.Tables.IndexOf(TN.Bank) < 0)
                return;

            DataRow drBank = bankDS.Tables[TN.Bank].Rows[0];
            DBank b = new DBank();
            b.GetAccountDetails(custId, accountNo);

            b.CustomerID = custId;
            b.BankAccountNo = (string)drBank[CN.BankAccountNo];
            b.BankCode = (string)drBank[CN.BankCode];
            b.DateOpened = (DateTime)drBank[CN.BankAccountOpened];
            b.Code = (string)drBank[CN.Code];

            b.Save(conn, trans, custId, accountNo);
        }

        public void SaveEmploymentDetails(SqlConnection conn,
                                    SqlTransaction trans, string custId, DataSet empDS)
        {
            if (empDS.Tables.IndexOf(TN.Employment) < 0)
                return;

            DataRow drEmp = empDS.Tables[TN.Employment].Rows[0];
            DEmployment e = new DEmployment();
            e.GetEmployment(custId);

            e.CustomerID = custId;
            e.DateEmployed = (DateTime)drEmp[CN.DateEmployed];

            //CR 866 Changed CN.Occupation to CN.WorkType 
            e.WorkType = (string)drEmp[CN.WorkType];
            e.EmploymentStatus = (string)drEmp[CN.EmploymentStatus];
            e.PayFrequency = (string)drEmp[CN.PayFrequency];
            e.AnnualGross = drEmp[CN.AnnualGross];
            e.PersDialCode = (string)drEmp[CN.PersDialCode];
            e.PersTel = (string)drEmp[CN.PersTel];
            e.PrevDateEmployed = (DateTime)drEmp[CN.PrevDateEmployed];

            //CR 866 - Thailand scoring [PC]
            e.JobTitle = drEmp[CN.JobTitle].ToString();
            e.Organisation = drEmp[CN.Organisation].ToString();
            e.Industry = drEmp[CN.Industry].ToString();
            e.EducationLevel = drEmp[CN.EducationLevel].ToString();
            //End CR 866

            e.Save(conn, trans, custId);
        }

        public void SaveCustomerAdditionalDetailsResidential(SqlConnection conn,
                                    SqlTransaction trans, DataSet additionalDetails)
        {
            if (additionalDetails.Tables.IndexOf(TN.CustomerAdditionalDetailsResidential) < 0)
                return;

            if (additionalDetails.Tables[TN.CustomerAdditionalDetailsResidential].Rows.Count < 1)
                return;

            DCustomer cust = new DCustomer();
            DataRow dr;

            dr = additionalDetails.Tables[TN.CustomerAdditionalDetailsResidential].Rows[0];

            cust.CustID = (string)dr[CN.CustID];
            cust.PrevDateIn = (DateTime)dr[CN.PrevDateIn];
            cust.DateIn = (DateTime)dr[CN.DateIn];
            cust.ResidentialStatus = (string)dr[CN.ResidentialStatus];
            cust.PrevResidentialStatus = (string)dr[CN.PrevResidentialStatus];
            cust.MonthlyRent = dr[CN.MonthlyRent];
            cust.PropertyType = (string)dr[CN.PropertyType];
            cust.SaveCustomerAdditionalDetailsResidential(conn, trans);
            //cust.SaveHomeAddress(conn, trans, cust.CustID);
        }

        public void SaveCustomerAdditionalDetailsFinancial(SqlConnection conn,
                                    SqlTransaction trans, string custId, string accountNo, DataSet additionalDetails)
        {
            if (additionalDetails.Tables.IndexOf(TN.CustomerAdditionalDetailsFinancial) < 0)
                return;

            if (additionalDetails.Tables[TN.CustomerAdditionalDetailsFinancial].Rows.Count < 1)
                return;

            DProposal prop = new DProposal();
            DataRow dr;

            dr = additionalDetails.Tables[TN.CustomerAdditionalDetailsFinancial].Rows[0];

            prop.CustomerID = (string)dr[CN.CustID];
            prop.MonthlyIncome = dr[CN.MonthlyIncome];
            prop.AdditionalIncome = dr[CN.AdditionalIncome];
            prop.Commitments1 = dr[CN.Commitments1];
            prop.Commitments2 = dr[CN.Commitments2];
            prop.Commitments3 = dr[CN.Commitments3];
            prop.OtherPayments = dr[CN.OtherPayments];
            prop.AdditionalExpenditure1 = dr[CN.AdditionalExpenditure1];
            prop.AdditionalExpenditure2 = dr[CN.AdditionalExpenditure2];
            prop.CreditCardNo1 = (string)dr[CN.CCardNo1];
            prop.CreditCardNo2 = dr[CN.CCardNo2].ToString();
            prop.CreditCardNo3 = (string)dr[CN.CCardNo3];
            prop.CreditCardNo4 = (string)dr[CN.CCardNo4];
            prop.DueDayId = (int)dr[CN.DueDayId];
            prop.BankAccountName = (string)dr[CN.BankAccountName];
            prop.PaymentMethod = (string)dr[CN.Paymentmethod];

            prop.SaveCustomerAdditionalDetailsFinancial(conn, trans);

        }

        //public void SaveCustomerAddresses(SqlConnection conn, SqlTransaction trans, string customerID, DataSet addresses, string user)
        //{
        //    DCustomer cust = new DCustomer();
        //    DataTable newAddresses = addresses.Tables["Addresses"];
        //    string ErrorString = "The following errors have been found trying to save addresses:\n\n";

        //    /* get the current list of address types */
        //    DataTable currentAddresses = (this.GetCustomerAddresses(conn, trans, customerID)).Tables[TN.CustomerAddresses];

        //    /* derive the list of address types which have been removed */
        //    foreach (DataRow r in currentAddresses.Rows)
        //    {
        //        newAddresses.DefaultView.RowFilter = "AddressType = '"+(string)r["AddressType"]+"'";
        //        if(newAddresses.DefaultView.Count == 0)
        //        {
        //            /* set the date-moved date for all removed address types to today and update */
        //            cust.DeleteAddress(conn, trans, customerID, (string)r["AddressType"], (string)r[CN.Category]);
        //        }
        //    }
        //    newAddresses.DefaultView.RowFilter = String.Empty;	

        //    foreach(DataRow row in newAddresses.Rows)
        //    {
        //        try
        //        {
        //            cust.SaveAddress(conn, trans, customerID, Convert.ToInt32(user),  row);
        //        }
        //        catch(STLException e)
        //        {
        //            row.RowError = e.Message;
        //            ErrorString += e.Message + "\n";
        //        }
        //    }

        //    if(addresses.HasErrors)
        //    {
        //        throw new STLException(ErrorString);
        //    }
        //}

        public void SaveCustomerAddresses(SqlConnection conn, SqlTransaction trans, string customerID, DataSet addresses, string user)
        {
            DCustomer cust = new DCustomer();
            DataTable newAddresses = addresses.Tables["Addresses"];
            string ErrorString = "The following errors have been found trying to save addresses:\n\n";

            /* get the current list of address types */
            DataTable currentAddresses = (this.GetCustomerAddresses(conn, trans, customerID)).Tables[TN.CustomerAddresses];

            /* derive the list of address types which have been removed */
            foreach (DataRow r in currentAddresses.Rows)
            {
                newAddresses.DefaultView.RowFilter = "AddressType = '" + (string)r["AddressType"] + "'";
                if (newAddresses.DefaultView.Count == 0)
                {
                    /* set the date-moved date for all removed address types to today and update */
                    cust.DeleteAddress(conn, trans, customerID, (string)r["AddressType"], (string)r[CN.Category]);
                }
            }
            newAddresses.DefaultView.RowFilter = "";

            foreach (DataRow row in newAddresses.Rows)
            {
                // CR950 RDB 04/07/08 if we have a matching record dont save
                // and avoid adding unwanted audit history
                bool addrSaveRequired = (row["address1"].ToString().Trim() != string.Empty ||
                                            row["address2"].ToString().Trim() != string.Empty ||
                                            row["address3"].ToString().Trim() != string.Empty ||
                                            row["postcode"].ToString().Trim() != string.Empty ||
                                            row["Email"].ToString().Trim() != string.Empty ||
                                            row["notes"].ToString().Trim() != string.Empty || //SC 69979 18/09/08
                                            row["Latitude"].ToString().Trim() != string.Empty || // Address Standardization CR2019-025
                                            row["Longitude"].ToString().Trim() != string.Empty); // Address Standardization CR2019-025

                bool telSaveRequired = (row["phoneno"].ToString().Trim() != string.Empty ||
                                        row["ext"].ToString().Trim() != string.Empty ||
                                        row["dialcode"].ToString().Trim() != string.Empty);

                //CR950 RDB 04/07/08 for audit purposes only save addresses that are new or have changed
                if (!Convert.ToBoolean(row["NewRecord"]))
                {

                    string addType = row["addresstype"].ToString().Trim();
                    DateTime dateIn = Convert.ToDateTime(row["datein"]);
                    //DateTime? dateMoved = Convert.ToDateTime(row["dateMoved"]);
                    string cusaddr1 = row["address1"].ToString().Trim();
                    string cusaddr2 = row["address2"].ToString().Trim();
                    string cusaddr3 = row["address3"].ToString().Trim();
                    string cuspocode = row["postcode"].ToString().Trim();
                    string email = row["Email"].ToString().Trim();
                    string notes = row["notes"].ToString().Trim();//SC 69979 18/09/08
                    string zone = row["zone"].ToString().Trim();    //CR1084 jec
                    string deliveryArea = row["deliveryarea"].ToString().Trim();           //IP - 25/05/12 - #10225 - DeliveryArea was previously not being saved.
                    string DELfirstname = row["DELFirstname"].ToString().Trim();
                    string DELlastname = row["DELLastname"].ToString().Trim();
                    string latitude = row["Latitude"].ToString().Trim(); // Address Standardization CR2019-025
                    string longitude = row["Longitude"].ToString().Trim(); // Address Standardization CR2019 - 025

                    //telephone
                    string telNo = row["phoneno"].ToString().Trim();
                    string extnNo = row["ext"].ToString().Trim();
                    string dialCode = row["dialcode"].ToString().Trim();

                    if (addrSaveRequired)
                    {
                        foreach (DataRow currRow in currentAddresses.Rows)
                        {
                            if (addType == currRow["addresstype"].ToString().Trim() &&
                                dateIn == Convert.ToDateTime(currRow["date in"]) &&
                                //dateMoved == Convert.ToDateTime(currRow["dateMoved"]) &&
                                cusaddr1 == currRow["address1"].ToString().Trim() &&
                                cusaddr2 == currRow["address2"].ToString().Trim() &&
                                cusaddr3 == currRow["address3"].ToString().Trim() &&
                                cuspocode == currRow["postcode"].ToString().Trim() &&
                                email == currRow["Email"].ToString().Trim() &&
                                notes == currRow["notes"].ToString().Trim() &&  //SC 69979 18/09/08
                                zone == currRow["zone"].ToString().Trim() &&      //CR1084 jec                                
                                deliveryArea == currRow["deliveryarea"].ToString().Trim() &&          //IP - 25/05/12 - #10225
                                DELfirstname == currRow["DELFirstname"].ToString().Trim() &&
                                DELlastname == currRow["DELLastname"].ToString().Trim() &&
                                latitude == currRow["Latitude"].ToString().Trim() && // Address Standardization CR2019-025
                                longitude == currRow["Longitude"].ToString().Trim()) // Address Standardization CR2019-025
                            {
                                addrSaveRequired = false;
                                break;
                            }
                        }
                    }

                    // telephone
                    if (telSaveRequired)
                    {
                        foreach (DataRow currRow in currentAddresses.Rows)
                        {
                            if (addType == currRow["addresstype"].ToString().Trim() &&
                                telNo == currRow["Phone"].ToString().Trim() &&
                                extnNo == currRow["ext"].ToString().Trim() &&
                                dialCode == currRow["dialcode"].ToString().Trim())
                            {
                                telSaveRequired = false;
                                break;
                            }
                        }
                    }
                }

                try
                {
                    if (telSaveRequired)
                        cust.SaveTelephone(conn, trans, customerID, Convert.ToInt32(user), row);
                    if (addrSaveRequired)
                        cust.SaveAddress(conn, trans, customerID, Convert.ToInt32(user), row);
                }
                catch (STLException e)
                {
                    row.RowError = e.Message;
                    ErrorString += e.Message + "\n";
                }
            }
            if (addresses.HasErrors)
            {
                throw new STLException(ErrorString);
            }
        }


        public DataSet GetBasicCustomerDetails(SqlConnection conn, SqlTransaction trans, string customerID, string accountNo, string relationship)
        {
            DataSet ds = new DataSet();
            DCustomer cust = new DCustomer();
            if (0 == cust.GetBasicCustomerDetails(conn, trans, customerID, accountNo, relationship))
            {
                DataTable details = new DataTable("BasicDetails");
                details.Columns.AddRange(new DataColumn[]   {   new DataColumn("CustomerID"),
                                                                new DataColumn("Title"),
                                                                new DataColumn("FirstName"),
                                                                new DataColumn("LastName"),
                                                                 // new address pop add hear by tosif ali 16/10/2018*@
                                                                new DataColumn("DELTitleC"),
                                                                new DataColumn("DELFirstName"),
                                                                new DataColumn("DELLastName"),

                                                                new DataColumn("Alias"),
                                                                new DataColumn("BudgetCard", System.Type.GetType("System.Int32")),
                                                                new DataColumn(CN.RFCreditLimit, System.Type.GetType("System.Decimal")),
                                                                new DataColumn("IDNumber"),
                                                                new DataColumn(CN.DOB,Type.GetType("System.DateTime")),
                                                                new DataColumn(CN.AvailableCredit, System.Type.GetType("System.Decimal")),
                                                                new DataColumn(CN.MaidenName),
                                                                new DataColumn(CN.Sex),
                                                                new DataColumn(CN.MoreRewardsNo),
                                                                new DataColumn(CN.RFCardSeqNo, Type.GetType("System.Byte")),
                                                                new DataColumn(CN.LimitType),
                                                                new DataColumn(CN.MaritalStatus),
                                                                new DataColumn(CN.Nationality),
                                                                new DataColumn(CN.Dependants),
                                                                new DataColumn(CN.Band),
                                                                new DataColumn(CN.StoreType),
                                                                new DataColumn("LoanQualified"),
                                                                new DataColumn("StoreCardApproved"),
                                                                new DataColumn("StoreCardAvailable"),
                                                                new DataColumn("StoreCardLimit"),
                                                                new DataColumn("Blacklisted"),
                                                                new DataColumn(CN.CashLoanBlocked),
                                                                new DataColumn("ResieveSms", typeof(bool)),
                                                                new DataColumn(CN.IsSpouseWorking, typeof(bool)),
                                                                new DataColumn(CN.DependantsFromProposal, typeof(int))});

                DataRow row = details.NewRow();
                row["CustomerID"] = cust.CustID;
                row["Title"] = cust.Title;
                row["FirstName"] = cust.FirstName;
                row["LastName"] = cust.Name;
                // new address pop add hear by tosif ali 16/10/2018*@
                row["DELTitleC"] = cust.DELTitleC;
                row["DELFirstName"] = cust.DELFirstName;
                row["DELLastName"] = cust.DELLastName;
                // End Hear
                row["Alias"] = cust.Alias;
                row["BudgetCard"] = cust.BudgetCard;
                row[CN.RFCreditLimit] = cust.RFLimit;
                row["IDNumber"] = cust.IDNumber;
                row[CN.DOB] = cust.DateBorn;
                row[CN.AvailableCredit] = cust.AvailableCredit;
                row[CN.MaidenName] = cust.MaidenName;
                row[CN.Sex] = cust.Sex;
                row[CN.MoreRewardsNo] = cust.MoreRewardsNo;
                row[CN.RFCardSeqNo] = cust.RFCardSeqNo;
                row[CN.LimitType] = cust.LimitType;
                row[CN.Band] = cust.scoringBand;
                row[CN.MaritalStatus] = cust.MaritalStatus;
                row[CN.Nationality] = cust.Nationality;
                row[CN.Dependants] = cust.Dependants;
                row[CN.StoreType] = cust.StoreType;
                row["LoanQualified"] = cust.LoanQualified;
                row["Blacklisted"] = cust.Blacklisted;
                row["StoreCardApproved"] = cust.StoreCardApproved;
                row["StoreCardAvailable"] = cust.StoreCardAvailable;
                row["StoreCardLimit"] = cust.StoreCardLimit;
                row[CN.CashLoanBlocked] = cust.CashLoanBlocked;
                row["ResieveSms"] = cust.ResieveSms;
                row[CN.IsSpouseWorking] = cust.IsSpouseWorking;
                row[CN.DependantsFromProposal] = cust.DependantsFromProposal;
                details.Rows.Add(row);
                ds.Tables.Add(details);

                cust.GetCustomerAddresses(conn, trans, cust.CustID);
                ds.Tables.Add(cust.Addresses);

                DEmployment emp = new DEmployment();
                emp.GetEmployment(cust.CustID);
                ds.Tables.Add(emp.GetRow(TN.Employment));

                DBank bank = new DBank();
                bank.GetAccountDetails(cust.CustID, accountNo);
                ds.Tables.Add(bank.GetRow(TN.Bank));

                ds.Tables.Add(cust.GetCustomerAdditionalDetailsFinancial(cust.CustID));
                ds.Tables.Add(cust.GetCustomerAdditionalDetailsResidential(cust.CustID));

            }

            return ds;
        }

        public DataSet GetBasicCustomerDetailsForReprint(SqlConnection conn, SqlTransaction trans, string agrmtno)
        {
            DataSet ds = new DataSet();
            DCustomer cust = new DCustomer();
            DataTable dt = cust.GetBasicCustomerDetailsForReprint(conn, trans, agrmtno);
            ds.Tables.Add(dt);
            return ds;
        }

        public void GetRFLimit(string customerID, string acctList, string accountType, out decimal limit, out decimal available, out bool wrongType)
        {
            // The acctList parameter is used to exclude certain accounts from the calculation.
            // Used by the Add-To calculation to exclude the accounts to be settled and the new account.

            limit = 0;
            available = 0;
            wrongType = false;
            if (accountType != AT.ReadyFinance)
                wrongType = true;

            //customer's available credit is:
            /*	sum of agreement totals where
				*  accttype = 'R' and
				*	(deliveredqty != agreementtotal || oustbal > 0)
				*	where deliveredqty = 
				*	sum of transvalue from fintrans 
				*	where transtype = DEL or GRT
				*/
            DCustomer cust = new DCustomer();
            cust.GetRFLimit(null, null, customerID, acctList);
            limit = cust.RFLimit;
            available = cust.RFAvailable;
        }

        public void GetExistCashLoan(string customerID, out decimal ExistCashLoan)
        {
            ExistCashLoan = 0;
            DCustomer cust = new DCustomer();
            cust.GetExistCashLoan(null, null, customerID);
            ExistCashLoan = cust.ExistCashLoan;

        }

        public DataSet GetWarrantySecondEffortSolicitationItems(string customerId, int numberOfPrompts, int warrantyAfterDeliveryDays)
        {
            DataSet ds = new DataSet();
            DCustomer cust = new DCustomer();

            DataTable dt = cust.GetWarrantySecondEffortSolicitationItems(customerId, numberOfPrompts, warrantyAfterDeliveryDays);
            dt.TableName = TN.WarrantyList;

            ds.Tables.Add(dt);
            return ds;
        }


        public void SaveWarrantySESConfirmation(SqlConnection conn, SqlTransaction trans, DataSet dsWarrantableItems, int Empno)
        {
            DAccount acct = new DAccount();
            DateTime dateNow = DateTime.Now;
            int noPrompts = Convert.ToInt32(Country[CountryParameterNames.WarrantySESPrompts]);

            if (dsWarrantableItems.Tables.IndexOf(TN.WarrantyList) >= 0)
                foreach (DataRow dr in dsWarrantableItems.Tables[TN.WarrantyList].Rows)
                {
                    int itemNoPrompts = Convert.ToInt32(dr[CN.NoOfPrompts]);
                    //Only update an account code if the item has been prompted less times than the allowable no of prompts
                    if (itemNoPrompts < noPrompts)
                    {
                        string accNo = dr[CN.AccountNumber].ToString();
                        string code = "SSP" + (itemNoPrompts + 1).ToString();
                        string itemno = dr[CN.ItemNo].ToString();

                        dateNow = dateNow.AddSeconds(1); // prevent primary key violations
                        acct.AddCodeToAccount(conn, trans, accNo, code, dateNow, Empno, itemno);
                    }
                }

        }

        public DataSet GetRFCombinedDetails(string customerID)
        {
            DataSet ds = null;
            DCustomer cust = new DCustomer();
            if (0 == cust.GetRFCombinedDetails(customerID))
            {
                ds = new DataSet();

                DataTable details = new DataTable(TN.RFDetails);
                details.Columns.AddRange(new DataColumn[]{
                                                new DataColumn(CN.AvailableCredit),
                                                new DataColumn(CN.CardPrinted),
                                                new DataColumn(CN.TotalAgreements),
                                                new DataColumn(CN.TotalArrears),
                                                new DataColumn(CN.TotalBalances),
                                                new DataColumn(CN.TotalCredit),
                                                new DataColumn(CN.TotalDeliveredInstalments),
                                                new DataColumn(CN.TotalAllInstalments),
                                                new DataColumn(CN.RFCardSeqNo, Type.GetType("System.Byte")),
                                                new DataColumn(CN.DateNextPaymentDue, Type.GetType("System.DateTime"))});

                DataRow row = details.NewRow();

                row[CN.AvailableCredit] = cust.AvailableCredit;
                row[CN.CardPrinted] = cust.CardPrinted;
                row[CN.TotalAgreements] = cust.TotalAgreements;
                row[CN.TotalArrears] = cust.TotalArrears;
                row[CN.TotalBalances] = cust.TotalBalances;
                row[CN.TotalCredit] = cust.TotalCredit;
                row[CN.TotalDeliveredInstalments] = cust.TotalDeliveredInstalments;
                row[CN.TotalAllInstalments] = cust.TotalAllInstalments;
                row[CN.RFCardSeqNo] = cust.RFCardSeqNo;
                row[CN.DateNextPaymentDue] = cust.DateNextPaymentDue;

                details.Rows.Add(row);
                ds.Tables.Add(details);
            }

            return ds;
        }


        /// <summary>
        /// Return the transactions for the customer set of RF accounts 
        /// grouped by date and transaction type.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public DataSet GetRFCombinedTransactions(bool groupSum, string customerID)
        {
            DFinTrans transactions = new DFinTrans();
            if (groupSum)
                return transactions.GetRFGroupedTransactions(customerID);
            else
                return transactions.GetRFCombinedTransactions(customerID);
        }


        public string GetMoreRewardsNo(string customerID)
        {
            DCustomer cust = new DCustomer();
            return cust.GetMoreRewardsNo(customerID);
        }

        public DataSet GetDetailsForDebtCollector(string acctNo, out int privilegeCount)
        {
            privilegeCount = 0;

            DataSet ds = new DataSet();
            DCustomer cust = new DCustomer();
            cust.GetDetailsForDebtCollector(acctNo);
            ds.Tables.Add(cust.Customer);

            DDelivery del = new DDelivery();
            del.GetItemsForDebtCollector(acctNo);
            ds.Tables.Add(del.Deliveries);

            DFinTrans trans = new DFinTrans();
            trans.GetDetailsForDebtCollector(acctNo, out privilegeCount);
            ds.Tables.Add(trans.FinTrans);

            DServiceRequest ser = new DServiceRequest();

            DataTable dt = ser.GetServiceRequestSummaryForAccount(acctNo);
            dt.TableName = TN.ServiceRequest;
            if (dt.Rows.Count > 0)
                ds.Tables.Add(dt);

            return ds;
        }

        public DataSet GetEmployerDetails(string customerID, string acctNo)
        {
            DataSet ds = null;
            DEmployment emp = new DEmployment();
            emp.CustomerID = customerID;
            if ((int)Return.Success == emp.GetEmployerDetails(acctNo))
            {
                ds = new DataSet();
                ds.Tables.Add(emp.EmpDetails);
            }
            return ds;
        }

        public StringCollection GetRequiredAddressTypes(string customerID)
        {
            StringCollection addressTypes = new StringCollection();
            DCustomer cust = new DCustomer();
            DataTable dt = cust.GetRequiredAddressTypes(customerID);
            foreach (DataRow r in dt.Rows)
                addressTypes.Add(((string)r[CN.AddressType]).Trim());
            return addressTypes;
        }

        public StringCollection GetDistinctAddressTypes(string customerID)
        {
            StringCollection addressTypes = new StringCollection();
            DCustomer cust = new DCustomer();
            DataTable dt = cust.GetDistinctAddressTypes(customerID);
            foreach (DataRow r in dt.Rows)
                addressTypes.Add(((string)r[CN.AddressType]).Trim());
            return addressTypes;
        }

        public void UpdateLoyaltyCardNo(SqlConnection conn, SqlTransaction trans,
                                        string customerID, string loyaltyCardNo)
        {
            DCustomer cust = new DCustomer();
            cust.UpdateLoyaltyCardNo(conn, trans, customerID, loyaltyCardNo);
        }

        public DataTable GetDetails(string customerID)
        {
            DCustomer cust = new DCustomer();
            cust.GetCustomerDetails(null, null, customerID);
            cust.GetCustomerHomeAddress(customerID);
            return cust.GetRow(TN.Customer);
        }

        public void UnblockCredit(SqlConnection conn, SqlTransaction trans,
                                  string customerID)
        {
            DCustomer cust = new DCustomer();
            cust.UnblockCredit(conn, trans, customerID);
        }

        // 5.1 uat253 rdb 10/12/07 Check if privilege TermsType existed when account was created
        // if not PL.NewAccount needs to treat account an not privilege member
        public bool ValidatePrivilegeTermsType(string termsType, string privilegeClubCode, DateTime dateAccountOpened)
        {
            // look for record in intrate history for this  TermsType, privilegCubCode and valid for this date
            DTermsType dTerms = new DTermsType();
            DataSet ds = dTerms.LoadTermsTypeDetails(termsType);
            DataTable intRateHis = ds.Tables[TN.IntRateHistory];
            intRateHis.DefaultView.RowFilter = "datefrom < '" + dateAccountOpened.ToString() + "' AND dateto > '" + dateAccountOpened.ToString() + "' AND termsType = '" + termsType + "' AND Band = '" + privilegeClubCode + "'";
            return intRateHis.DefaultView.Count > 0;
        }


        public bool IsPrivilegeMember(string customerID, out string privilegeClubCode, out string privilegeClubDesc, out bool hasDiscount)
        {
            bool isMember = false;
            hasDiscount = false;
            DCustomer cust = new DCustomer();
            isMember = cust.IsPrivilegeMember(customerID, out privilegeClubCode, out privilegeClubDesc);
            if (isMember)
            {
                hasDiscount = (privilegeClubCode == PCCustCodes.Tier2);
            }
            else
            {
                // Default description when not qualified
                if ((bool)Country[CountryParameterNames.TierPCEnabled])
                    privilegeClubDesc = GetResource("T_LOYALTYCLUB");
                else
                    privilegeClubDesc = GetResource("T_PRIVILEGECLUB");
            }
            return isMember;
        }

        public void QualifyPrivilegeClub(SqlConnection conn, SqlTransaction trans)
        {
            DCustomer cust = new DCustomer();
            // Work out which customers to promote / demote
            cust.PCCustomerTiers(conn, trans);
            // Update the customers and generate their letters
            cust.PCCustomerTiersUpdate(conn, trans);
        }

        public DataTable GetOtherCustomers(string acctNo)
        {
            DCustomer cust = new DCustomer();
            DataTable dt = cust.GetOtherCustomers(acctNo);
            return dt;
        }

        public DataSet GetRFCombinedDetailsForPrint(string customerID)
        {
            DataSet ds = null;
            DCustomer cust = new DCustomer();
            if (0 == cust.GetRFCombinedDetailsForPrint(customerID))
            {
                ds = new DataSet();

                DataTable details = new DataTable(TN.RFDetails);
                details.Columns.AddRange(new DataColumn[]{
                                                             new DataColumn(CN.AvailableCredit),
                                                             new DataColumn(CN.CardPrinted),
                                                             new DataColumn(CN.TotalAgreements),
                                                             new DataColumn(CN.TotalArrears),
                                                             new DataColumn(CN.TotalBalances),
                                                             new DataColumn(CN.TotalCredit),
                                                             new DataColumn(CN.TotalDeliveredInstalments),
                                                             new DataColumn(CN.TotalAllInstalments)});

                DataRow row = details.NewRow();

                row[CN.AvailableCredit] = cust.AvailableCredit;
                row[CN.CardPrinted] = cust.CardPrinted;
                row[CN.TotalAgreements] = cust.TotalAgreements;
                row[CN.TotalArrears] = cust.TotalArrears;
                row[CN.TotalBalances] = cust.TotalBalances;
                row[CN.TotalCredit] = cust.TotalCredit;
                row[CN.TotalDeliveredInstalments] = cust.TotalDeliveredInstalments;
                row[CN.TotalAllInstalments] = cust.TotalAllInstalments;

                details.Rows.Add(row);
                ds.Tables.Add(details);
            }

            return ds;
        }

        public void UpdateCustomerID(SqlConnection conn, SqlTransaction trans,
                                     string newCustID, string oldCustID)
        {
            DCustomer cust = new DCustomer();

            cust.GetCustomerDetails(conn, trans, oldCustID);

            cust.CustID = newCustID;
            cust.User = this.User;
            cust.DateChanged = DateTime.Now;
            cust.Save(conn, trans, newCustID);

            cust.UpdateCustomerID(conn, trans, newCustID, oldCustID);

            cust.DeleteCustomer(conn, trans, oldCustID);
        }

        public void SetCreditLimit(SqlConnection conn, SqlTransaction trans,
            string custID, decimal creditLimit)
        {
            DCustomer cust = new DCustomer();
            cust.SetCreditLimit(conn, trans, custID, creditLimit, "A");
        }

        public void SetAvailableSpend(SqlConnection conn, SqlTransaction trans,
                                        string custID)
        {
            DCustomer cust = new DCustomer();
            cust.GetRFLimit(conn, trans, custID, String.Empty);

            if (cust.RFLimit > 0)
                cust.SetAvailableSpend(conn, trans, custID, cust.RFAvailable);
        }

        public DataSet GetCustomerAudit(string customerId)
        {
            DataSet ds = new DataSet();
            DCustomer customerAudit = new DCustomer();
            ds.Tables.Add(customerAudit.GetCustomerAudit(customerId));
            return ds;
        }

        public DataSet GetAddressHistory(string customerId)
        {
            DataSet ds = new DataSet();
            DCustomer addressHistory = new DCustomer();
            ds.Tables.Add(addressHistory.GetAddressHistory(customerId));
            ds.Tables.Add(addressHistory.GetTelephoneHistory(customerId));
            return ds;
        }

        public DataSet GetAddressAuditHistory(string customerId)
        {
            DataSet ds = new DataSet();
            DCustomer addressHistory = new DCustomer();
            ds.Tables.Add(addressHistory.GetAddressAuditHistory(customerId));
            ds.Tables.Add(addressHistory.GetTelephoneAuditHistory(customerId));
            return ds;
        }

        public DataTable RFCardPrintList(SqlConnection conn, SqlTransaction trans, bool privilege)
        {
            DataTable customerList = new DataTable();
            DCustomer cardPrint = new DCustomer();
            customerList = cardPrint.RFCardPrintList(conn, trans, privilege);
            return customerList;
        }

        public void RFCardPrinted(SqlConnection conn, SqlTransaction trans)
        {
            DCustomer cardPrint = new DCustomer();
            cardPrint.RFCardPrinted(conn, trans);
        }

        public void RFCardPrint(
            SqlConnection conn,
            SqlTransaction trans,
            string interfaceName,
            int runNo,
            bool privilege)
        {
            //Creating RF card print csv file
            StreamWriter sw = null;
            string newFileName = String.Empty;
            bool fileExists = false;

            try
            {
                if (privilege)
                {
                    /* Construct the filename using the current date and open the file */
                    newFileName = (string)Country[CountryParameterNames.SystemDrive] + "\\ORFCardPrintPriv(" + DateTime.Now.ToLongDateString() + ").CSV";
                }
                else
                    /* Construct the filename using the current date and open the file */
                    newFileName = (string)Country[CountryParameterNames.SystemDrive] + "\\ORFCardPrint(" + DateTime.Now.ToLongDateString() + ").CSV";

                if (File.Exists(newFileName))
                {
                    // The file already exists
                    fileExists = true;
                    throw new STLException(GetResource("M_RFCSVFILEEXISTS", new object[] { newFileName }));
                }

                // Load a datatable of customers from sproc DN_RFCardPrintListSP
                DataTable pendingRFCardPrintList = this.RFCardPrintList(conn, trans, privilege);

                sw = new StreamWriter(newFileName);
                int recordCount = 0;
                foreach (DataRow cardprintRow in pendingRFCardPrintList.Rows)
                {
                    // Process each line to add to the file
                    recordCount++;
                    sw.WriteLine((string)cardprintRow[CN.RecordLine]);

                }
                sw.Flush();
                sw.Close();

                if (recordCount == 0)
                {
                    // The file was not created because there was no data
                    File.Delete(newFileName);
                    string clubName = String.Empty;

                    if (privilege)
                    {
                        if ((bool)Country[CountryParameterNames.TierPCEnabled])
                            clubName = GetResource("T_LOYALTYCLUB");
                        else
                            clubName = GetResource("T_PRIVILEGECLUB");
                    }

                    string msg = GetResource("M_RFCSVFILENOTCREATED", new object[] { clubName, newFileName });

                    BInterfaceError ie = new BInterfaceError(
                        null,
                        null,
                        interfaceName,
                        runNo,
                        DateTime.Now,
                        msg,
                        "W");

                    Console.WriteLine(msg);
                }
                else
                {
                    //Update
                    this.RFCardPrinted(conn, trans);
                }

            }
            catch (Exception ex)
            {
                if (sw != null) sw.Close();
                /* Delete an incomplete csv file */
                if (!fileExists) File.Delete(newFileName);
                throw ex;
            }
        }

        public void IssuePrizeVouchers(SqlConnection conn, SqlTransaction trans,
                                       string acctNo, decimal cashPrice, int buffNo,
                                       out DateTime issueDate)
        {
            DateTime dateIssued = DateTime.Now;
            issueDate = dateIssued;

            decimal numVouchers = cashPrice / (decimal)Country[CountryParameterNames.ValuePerVoucher];
            numVouchers = Convert.ToDecimal(Math.Floor(Convert.ToDouble(numVouchers)));

            if (numVouchers > 0)
            {
                int voucherID = 0;
                DCustomer cust = new DCustomer();
                cust.User = this.User;
                cust.IssuePrizeVouchers(conn, trans, acctNo, cashPrice, dateIssued,
                                        buffNo, out voucherID);

                for (int i = 1; i <= numVouchers; i++)
                {
                    cust.SavePrizeVouchers(conn, trans, voucherID);
                }
            }
            else
                issueDate = Date.blankDate;
        }

        public void IssueAdditionalPrizeVouchers(SqlConnection conn, SqlTransaction trans,
                                               string acctNo, decimal cashPrice, int buffNo)
        {
            DCustomer cust = new DCustomer();
            DateTime dateIssued = DateTime.Now;
            decimal existingCashPrice = 0;

            cust.GetCashPriceForPrizeVouchers(conn, trans, acctNo, out existingCashPrice);

            decimal diff = cashPrice - existingCashPrice;
            decimal numVouchers = diff / (decimal)Country[CountryParameterNames.ValuePerVoucher];
            numVouchers = Convert.ToDecimal(Math.Floor(Convert.ToDouble(numVouchers)));

            if (numVouchers > 0)
            {
                int voucherID = 0;
                cust.User = this.User;
                cust.IssuePrizeVouchers(conn, trans, acctNo, cashPrice, dateIssued,
                                        buffNo, out voucherID);

                for (int i = 1; i <= numVouchers; i++)
                {
                    cust.SavePrizeVouchers(conn, trans, voucherID);
                }
            }
        }

        public DataSet GetPrizeVoucherDetails(string acctNo, string custID,
            string branchFilter, DateTime dateFrom, DateTime dateTo, int buffNo)
        {
            DCustomer cust = new DCustomer();
            DataSet ds = new DataSet();

            DataTable dt = cust.GetPrizeVoucherDetails(acctNo, custID, branchFilter, dateFrom, dateTo, buffNo);
            ds.Tables.Add(dt);

            return ds;
        }

        public void DeletePrizeVouchers(SqlConnection conn, SqlTransaction trans, DateTime endDate,
                                        string acctNo, bool isCancellation)
        {
            DCustomer cust = new DCustomer();
            cust.DeletePrizeVouchers(conn, trans, endDate, acctNo, isCancellation);
        }

        public bool SaveCustomerPhoto(SqlConnection conn, SqlTransaction trans, string custID, string fileName, int takenBy)
        {
            DCustomer cust = new DCustomer();
            return cust.SaveCustomerPhoto(conn, trans, custID, fileName, takenBy);
        }

        public bool SaveCustomerSignature(SqlConnection conn, SqlTransaction trans, string custID, string fileName)
        {
            DCustomer cust = new DCustomer();
            return cust.SaveCustomerSignature(conn, trans, custID, fileName);
        }

        public string GetCustomerPhoto(SqlConnection conn, SqlTransaction trans, string custID)
        {
            DCustomer cust = new DCustomer();
            return cust.GetCustomerPhoto(conn, trans, custID);
        }

        public DataSet GetAllCustomerPhotos(SqlConnection conn, SqlTransaction trans, string custID)
        {
            DCustomer cust = new DCustomer();
            DataSet ds = new DataSet();

            DataTable dt = cust.GetAllCustomerPhotos(conn, trans, custID);
            ds.Tables.Add(dt);

            return ds;
        }

        public string GetCustomerSignature(SqlConnection conn, SqlTransaction trans, string custID)
        {
            DCustomer cust = new DCustomer();
            return cust.GetCustomerSignature(conn, trans, custID);
        }


        public string CustomerGetIdByAcctno(string acctno)
        {
            DCustomer cust = new DCustomer();
            return cust.CustomerGetIdByAcctno(acctno);

        }

        public BCustomer()
        {

        }

        public bool GetCashLoanQualified(string custID, string acctNo)
        {
            DCustomer cust = new DCustomer();

            string qualified = cust.GetCashLoanQualified(custID, acctNo);
            return (qualified.ToLower() == "y");
        }

        //IP - 18/11/08 -  UAT5.1 - UAT(580) - Method that will generate a new Customer ID for a
        //Cash & Go Service Request and populate the Customer ID field on the Service Request screen.
        public string GenerateSRCashAndGoCustid(SqlConnection conn, SqlTransaction trans, int branchNo)
        {
            DCustomer cust = new DCustomer();
            return cust.GenerateSRCashAndGoCustid(conn, trans, branchNo);

        }

        public string CustomerGetBand(string acctno)
        {
            DCustomer cust = new DCustomer();
            return cust.CustomerGetBand(acctno);
        }

        public void CustomerSaveBand(string custid, char band)
        {
            DCustomer cust = new DCustomer();
            cust.CustomerSaveBand(custid, band);
        }

        public void ScoreCardAccountGet(SqlConnection conn, SqlTransaction trans, string custid)
        {
            DCustomer custd = new DCustomer();
            custd.ScorecardAccountGet(conn, trans, custid);
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to generate the AgreementInvoiceNumber.
        public string GenerateAgreementInvNo(string custid)//SqlConnection conn, SqlTransaction trans, 
        {
            DCustomer custd = new DCustomer();
            string agr_inv_no = custd.GenerateAgreementInvNo(custid);
            return agr_inv_no;
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceDetails.
        public DataSet GetInvoiceDetails(string accountNo, string agrmtno, string agreementInvoiceNumber)
        {
            DataSet ds = new DataSet();
            DCustomer cust = new DCustomer();

            DataTable dtInvoiceDetails = new DataTable("InvoiceDetails");
            dtInvoiceDetails = cust.GetInvoiceDetails(null, null, accountNo, agrmtno, agreementInvoiceNumber);
            ds.Tables.Add(dtInvoiceDetails);

            DataTable dtInvoicePaymentDetails = new DataTable("InvoicePaymentDetails");
            dtInvoicePaymentDetails = cust.GetInvoicePaymentDetails(null, null, accountNo, agrmtno, agreementInvoiceNumber);
            ds.Tables.Add(dtInvoicePaymentDetails);
            return ds;
        }

        // CR - MMI (Maximum Monthly Instalment)
        // This method is used to get MMI limit of customer.
        public decimal GetCustomerMmiLimit(string custId, out bool isMmiCalculated)
        {
            DCustomer dCust = new DCustomer();
            var custMMI = dCust.GetCustomerMmiLimit(custId);

            isMmiCalculated = custMMI.HasValue;
            return custMMI.HasValue ? custMMI.Value : 0;
        }

    }
}
