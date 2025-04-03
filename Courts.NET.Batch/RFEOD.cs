using System;
using STL.Common;
using STL.BLL;
using System.Data;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.IO;
using STL.DAL;
using STL.Common.Constants.EOD;

namespace STL.Batch
{
    /// <summary>
    /// Summary description for ReScore.
    /// </summary>
    public class RFEOD : CommonObject
    {
        private Logging log;
        private new int _user = 0;
        public new int User
        {
            get { return _user; }
            set { _user = value; }
        }

        private string _countrycode = "";
        public string CountryCode
        {
            get { return _countrycode; }
            set { _countrycode = value; }
        }

        private int _runNo = 0;
        public int runNo
        {
            get { return _runNo; }
            set { _runNo = value; }
        }

        private string _interfaceName = "";
        public string interfaceName
        {
            get { return _interfaceName; }
            set { _interfaceName = value; }
        }


        public RFEOD(int curUser, string curCountry,
                        int curRunNo, string curInterfaceName)
        {
            User = curUser;
            CountryCode = curCountry;
            runNo = curRunNo;
            interfaceName = curInterfaceName;
            log = new Logging();
        }

        public RFEOD()
        {
            log = new Logging();
        }

        public string Go()
        {
            // Create CSV for RF card print
            // For non-privilege customers
            string csvResult = this.RFCardPrint(interfaceName, runNo, false);
            // For privilege customers
            string csvResultPriv = this.RFCardPrint(interfaceName, runNo, true);

            // Send a reminder letter where zero balance over 10 months
            // or reset credit limit where zero balance over 12 months
            // For Ready Finance customer
            string checkResult = this.RFCheckExpired();

            // Rescore RF customers
            //string rescoreResult = this.Rescore('A', true); //EODResult.Pass; 
            //Equifax score card 
            char ScoreType = Convert.ToChar(Country[CountryParameterNames.BehaviouralScorecard]);
            string rescoreResult = string.Empty;
            switch (ScoreType.ToString().ToUpper())
            {
                case "A":
                case "B":
                case "C":
                case "D":
                    rescoreResult = this.Rescore(ScoreType, false); //EODResult.Pass; 
                    break;
                case "P":
                case "S":
                default:
                    rescoreResult = this.Rescore('A', false);
                    break;
            }
            string creditResult = EODResult.Pass;
            if ((bool)Country[CountryParameterNames.DoPotentialSpend])
            {
                // Calculate the potential credit limit
                creditResult = this.SetPotentialCreditLimit();
            }

            if (csvResult == EODResult.Fail ||
                csvResultPriv == EODResult.Fail ||
                checkResult == EODResult.Fail ||
                rescoreResult == EODResult.Fail ||
                creditResult == EODResult.Fail)
                return EODResult.Fail;
            else
                return EODResult.Pass;
        }

        private string RFCheckExpired()
        {
            //  Send a reminder letter where zero balance over 10 months
            //  or reset credit limit where zero balance over 12 months
            string eodResult = EODResult.Pass;
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection(Connections.Default);
                retries = 0;
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            DCustomer RFCheck = new DCustomer();
                            RFCheck.RFCheckExpired(conn, trans, User);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                //Stick it in the interface error table?
                BInterfaceError ie = new BInterfaceError(
                    null,
                    null,
                    this.interfaceName,
                    this.runNo,
                    DateTime.Now,
                    ex.Message + Environment.NewLine +
                    "#System Message#" + Environment.NewLine + 
                    ex.StackTrace,
                    "E");
                eodResult = EODResult.Fail;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return eodResult;
        }

        private string RFCardPrint(string interfaceName, int runNo, bool privilege)
        {
            //Creating RF card print csv file
            string eodResult = EODResult.Pass;
            SqlConnection conn = null;


            try
            {
                conn = new SqlConnection(Connections.Default);
                retries = 0;
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            BCustomer RFCard = new BCustomer();
                            RFCard.RFCardPrint(conn, trans,
                                interfaceName,
                                runNo,
                                privilege);
                            trans.Commit();
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == Deadlock && retries < maxRetries)
                        {
                            retries++;
                            if (conn.State != ConnectionState.Closed)
                                conn.Close();
                        }
                        else
                            throw ex;
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                //Stick it in the interface error table?
                BInterfaceError ie = new BInterfaceError(
                    null,
                    null,
                    this.interfaceName,
                    this.runNo,
                    DateTime.Now,
                    ex.Message + Environment.NewLine +
                    "#System Message#" + Environment.NewLine +
                    ex.StackTrace,
                    "E");
                eodResult = EODResult.Fail;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return eodResult;
        }

        //private void SaveConfig(string status)
        //{
        //    try
        //    {
        //        Assembly asm = Assembly.GetExecutingAssembly();
        //        FileInfo fileInfo = new FileInfo(asm.Location + ".config");

        //        if (!fileInfo.Exists)
        //            throw new Exception("Missing config file" + asm.Location + ".config");

        //        //Load the config file into the XML DOM.
        //        XmlDocument xml = new XmlDocument();
        //        xml.Load(fileInfo.FullName);

        //        foreach (XmlNode node in xml["configuration"]["appSettings"])
        //        {
        //            if (node.Name == "add")
        //            {
        //                switch (node.Attributes["key"].Value)
        //                {
        //                    case "Status": node.Attributes["value"].Value = status;
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //        }
        //        //Write out the new config file.
        //        xml.Save(fileInfo.FullName);
        //    }
        //    catch (Exception)
        //    {
        //        Console.WriteLine("Unable to write the config file.");
        //    }
        //}

        private void WriteLetter(SqlConnection conn, SqlTransaction trans, string accountNo, decimal oldLimit, decimal newLimit)
        {
            string letterCode = "";

            if (newLimit == oldLimit)
                letterCode = "RFSL";
            else
                letterCode = newLimit < oldLimit ? "RFLL" : "RFHL";

            BLetter l = new BLetter();
            l.Write(conn, trans, accountNo, DateTime.Now, DateTime.Now, letterCode, 0, "0");
        }

        private string Rescore(char scoretype, bool saveOnlyCreditLimit)
        {
            decimal score = 0;
            decimal limit = 0;
            string refCode = "";
            string Error = "";
            string progress = "";
            // string status = "0";
            string result = EODResult.Pass;
            short branchNo = 0;
            short lastBranchNo = 0;
            string lastRegion = "FirstTimeThrough";
            SqlConnection conn = null;
            DBranch b = new DBranch();
            DataTable Behaviouralrules = null;
            DataTable Applicantrules = null;
            DataTable EquifaxBehaviouralrules = null;  //CR2018-005 Equifax score card
            DataTable EquifaxApplicantrules = null;   //CR2018-005 Equifax score card

            DataSet rules = new DataSet();

            try
            {
                //DataSet resscore = null;

                BProposal prop = new BProposal();
                BMmi mmi = new BMmi();
                DataTable dt = prop.GetProposalsToRescore(scoretype);
                progress = "Beginning re-scoring. " + dt.Rows.Count.ToString() + " proposals to process.";

                Console.WriteLine(progress);

                dt.DefaultView.Sort = CN.AccountNo;

                //BCountry c = new BCountry();
                //c.GetMaintenanceParameters(null, null, CountryCode);

                foreach (DataRowView r in dt.DefaultView)
                {
                    string accountNo = (string)r[CN.AccountNo];
                    string accountType = (string)r[CN.AccountType];
                    string customerID = (string)r[CN.CustomerID];
                    DateTime dateProp = (DateTime)r[CN.DateProp];
                    decimal oldLimit = (decimal)r[CN.RFCreditLimit];
                    string oldband = r[CN.ScoringBand].ToString();
                    score = limit = 0;
                    Error = refCode = "";

                    branchNo = Convert.ToInt16(accountNo.Substring(0, 3));
                    if (branchNo != lastBranchNo)		/* only load the region if the branch has changed */
                    {
                        b.Populate(null, null, branchNo);
                        if (b.Region != lastRegion)	/* only load the rules if the region has changed */
                        {
                            if (rules.IsInitialized)
                                rules.Tables.Clear();
                            BScoring s = new BScoring();
                            Applicantrules = s.GetRulesTable(CountryCode, b.Region, 'A');
                            Applicantrules.TableName = "Applicant";

                            Behaviouralrules = s.GetRulesTable(CountryCode, b.Region, 'B');
                            Behaviouralrules.TableName = "Behavioural";

                            EquifaxApplicantrules = s.GetRulesTable(CountryCode, b.Region, 'C');  //CR2018-005 Equifax score card
                            EquifaxApplicantrules.TableName = "EquifaxApplicant";

                            EquifaxBehaviouralrules = s.GetRulesTable(CountryCode, b.Region, 'D');  //CR2018-005 Equifax score card
                            EquifaxBehaviouralrules.TableName = "EquifaxBehavioural";

                            rules.Tables.Add(Applicantrules);
                            rules.Tables.Add(Behaviouralrules);
                            rules.Tables.Add(EquifaxApplicantrules);  //CR2018-005 Equifax score card
                            rules.Tables.Add(EquifaxBehaviouralrules); //CR2018-005 Equifax score card

                            lastRegion = b.Region;
                        }
                        lastBranchNo = branchNo;
                    }

                    progress = "Re-scoring Customer: " + customerID;
                    Console.WriteLine(progress);

                    try
                    {
                        conn = new SqlConnection(Connections.Default);
                        do
                        {
                            try
                            {
                                string bureauFailure = "";
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    prop = new BProposal();
                                    prop.User = User;
                                    string outcome = "";
                                    string newBand = "";

                                    prop.ReScore(conn, trans, scoretype, CountryCode, accountNo, accountType, customerID, dateProp,
                                        rules, out refCode, out score, out limit, out outcome, out bureauFailure, out newBand);

                                    //if (scoretype == 'A')  //CR2018-005 Equifax, Commented and new condition for Equifax score card 
                                    if (scoretype == 'A' || scoretype == 'C')
                                    {
                                        WriteLetter(conn, trans, accountNo, oldLimit, limit);
                                    }
                                    //else if (scoretype == 'B') //CR2018-005 Equifax, //behavioural, Commented and new condition for Equifax score card
                                    else if (scoretype == 'B' || scoretype == 'D')
                                    {
                                        if ((bool)Country[CountryParameterNames.BehaveApplyEodImmediate])
                                        {
                                            BLetter l = new BLetter();
                                            l.WriteBHLetter(conn, trans, accountNo, oldLimit, limit, newBand, oldband);
                                        }
                                    }
                                    if (saveOnlyCreditLimit)
                                    {
                                        var cmd = new SqlCommand
                                        {
                                            CommandText = "SELECT RFCreditLimit FROM customer WHERE custID = @CustomerId",
                                            Connection = conn,
                                            Transaction = trans
                                        };

                                        cmd.Parameters.AddWithValue("@CustomerId", customerID);
                                        var RFCreditLimit = (decimal)cmd.ExecuteScalar();

                                        trans.Rollback();

                                        cmd = new SqlCommand
                                        {
                                            CommandText = "UPDATE Customer Set RFCreditLimit = @RFCreditLimit WHERE custID = @CustomerId",
                                            Connection = conn
                                        };

                                        cmd.Parameters.AddWithValue("@RFCreditLimit", RFCreditLimit);
                                        cmd.Parameters.AddWithValue("@CustomerId", customerID);
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        trans.Commit();
                                    }

                                    // Calculate and save MMI value for customer.
                                    mmi = new BMmi();
                                    mmi.User = User;
                                    mmi.SaveCustomerMmi(conn, trans, customerID, User, "Score");
                                }
                                break;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == Deadlock && retries < maxRetries)
                                {
                                    retries++;
                                    if (conn.State != ConnectionState.Closed)
                                        conn.Close();
                                }
                                else
                                    throw ex;
                            }
                        } while (retries <= maxRetries);
                    }
                    catch (Exception ex)
                    {
                        BInterfaceError ie = new BInterfaceError(
                            null,
                            null,
                            this.interfaceName,
                            this.runNo,
                            DateTime.Now,
                            customerID + ": " + ex.Message + Environment.NewLine +
                            "#System Message#" + Environment.NewLine +
                            ex.StackTrace,
                            "E");
                        Console.WriteLine("The following error occurred re-scoring this customer:");
                        Console.WriteLine(ex.Message);
                        result = EODResult.Fail;
                        log.logException(ex, this.User.ToString(), "Re-Scoring()");
                    }
                    finally
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            conn.Close();
                        }
                    }
                }
                progress = "Re-scoring finished.";
                Console.WriteLine(progress);
            }
            catch (Exception ex)
            {
                //Stick it in the interface error table?
                BInterfaceError ie = new BInterfaceError(
                    null,
                    null,
                    this.interfaceName,
                    this.runNo,
                    DateTime.Now,
                    ex.Message + Environment.NewLine +
                    "#System Message#" + Environment.NewLine +
                    ex.StackTrace,
                    "E");
                result = EODResult.Fail;
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// This method will find all the proposals that need rescoring
        /// and rescore them generating letters files for each one.
        /// </summary>
        public string Rescore(char scoretype)
        {
            return Rescore(scoretype, false);
        }

        /// <summary>
        /// This method will find all the proposals that need rescoring
        /// and rescore them generating letters files for each one.
        /// </summary>
        public string ApplyBHRescore()
        {
            string result = EODResult.Pass;
            SqlConnection conn = null;

            conn = new SqlConnection(Connections.Default);
            try
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    BProposal prop = new BProposal();
                    prop.ApplyLatestBSRescoreForRun(conn, trans);
                    //prop.Cache = c.Cache;
                    trans.Commit();
                }

            }
            catch (Exception ex)
            {
                BInterfaceError ie = new BInterfaceError(
                    null,
                    null,
                    this.interfaceName,
                    this.runNo,
                    DateTime.Now,
                    ex.Message + Environment.NewLine +
                    "#System Message#" + Environment.NewLine +
                    ex.StackTrace,
                    "E");
                Console.WriteLine("The following error occurred re-scoring this customer:");
                Console.WriteLine(ex.Message);
                result = EODResult.Fail;
                log.logException(ex, this.User.ToString(), "Re-Scoring()");
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// This method will calculate the potential credit limit
        /// for non-RF customers generating letters files for each one.
        /// </summary>
        public string SetPotentialCreditLimit()
        {
            //string Error = "";
            string progress = "";
            string status = "0";
            string result = EODResult.Pass;
            short branchNo = 0;
            short lastBranchNo = 0;
            string lastRegion = "FirstTimeThrough";
            decimal limit = 0;
            SqlConnection conn = null;

            DBranch b = new DBranch();
            DataTable rules = null;

            try
            {
                BProposal prop = new BProposal();
                DataTable dt = prop.GetNonRFProposals();
                progress = "Beginning potential credit limit calculation. " + dt.Rows.Count.ToString() + " proposals to process.";

                Console.WriteLine(progress);

                dt.DefaultView.Sort = CN.AccountNo;

                //BCountry c = new BCountry();
                //c.GetMaintenanceParameters(null, null, CountryCode);

                //this.Cache = c.Cache;

                foreach (DataRowView r in dt.DefaultView)
                {
                    string accountNo = (string)r[CN.AccountNo];
                    string accountType = (string)r[CN.AccountType];
                    string customerID = (string)r[CN.CustomerID];
                    DateTime dateProp = (DateTime)r[CN.DateProp];


                    branchNo = Convert.ToInt16(accountNo.Substring(0, 3));
                    if (branchNo != lastBranchNo)		/* only load the region if the branch has changed */
                    {
                        b.Populate(null, null, branchNo);
                        if (b.Region != lastRegion)	/* only load the rules if the region has changed */
                        {
                            BScoring s = new BScoring();
                            rules = s.GetRulesTable(CountryCode, b.Region, 'A');
                            lastRegion = b.Region;
                        }
                        lastBranchNo = branchNo;
                    }

                    progress = "Calculating Potential Spend For Customer: " + customerID;
                    Console.WriteLine(progress);

                    try
                    {
                        conn = new SqlConnection(Connections.Default);
                        do
                        {
                            try
                            {
                                conn.Open();
                                using (SqlTransaction trans = conn.BeginTransaction())
                                {
                                    prop = new BProposal();
                                    //prop.Cache = c.Cache;
                                    prop.User = User;
                                    prop.SetPotentialCreditLimit(conn, trans, CountryCode, accountNo, customerID, dateProp, rules, out limit);

                                    if (limit >= (decimal)Country[CountryParameterNames.MinSpendAmount])
                                        WritePotentialSpendLetter(conn, trans, accountNo);

                                    trans.Commit();
                                }
                                int i = Convert.ToInt32(status);
                                status = (++i).ToString();
                                //SaveConfig(i.ToString());
                                break;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == Deadlock && retries < maxRetries)
                                {
                                    retries++;
                                    if (conn.State != ConnectionState.Closed)
                                        conn.Close();
                                }
                                else
                                    throw ex;
                            }
                        } while (retries <= maxRetries);
                    }
                    catch (Exception ex)
                    {
                        BInterfaceError ie = new BInterfaceError(
                            null,
                            null,
                            this.interfaceName,
                            this.runNo,
                            DateTime.Now,
                            customerID + ": " + ex.Message + Environment.NewLine +
                            "#System Message#" + Environment.NewLine +
                            ex.StackTrace,
                            "E");
                        Console.WriteLine("The following error occurred calculating potential credit limit for this customer:");
                        Console.WriteLine(ex.Message);
                        result = EODResult.Fail;
                        log.logException(ex, this.User.ToString(), "SetPotentialCreditLimit()");
                    }
                }
                progress = "Potential credit limit calculation finished.";
                Console.WriteLine(progress);
            }
            catch (Exception ex)
            {
                //Stick it in the interface error table?
                BInterfaceError ie = new BInterfaceError(
                    null,
                    null,
                    this.interfaceName,
                    this.runNo,
                    DateTime.Now,
                    ex.Message + Environment.NewLine +
                    "#System Message#" + Environment.NewLine +
                    ex.StackTrace,
                    "E");
                result = EODResult.Fail;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //SaveConfig(result);

                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return result;
        }

        private void WritePotentialSpendLetter(SqlConnection conn, SqlTransaction trans, string acctNo)
        {
            BLetter l = new BLetter();
            l.WritePotentialSpend(conn, trans, acctNo, DateTime.Today, DateTime.Today, "RFPS", 0);
        }

    }
}
