using STL.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Blue.Cosacs.BLL.Equifax;
using STL.Common.Constants.Tags;
using STL.Common;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;

namespace Blue.Cosacs.BLL.EquifaxRule
{
    public class EquifaxRule
    {
        public decimal EquifaxgetScore(string customerID, string accountNo, string acctType, decimal interceptvalue, DataSet ds,char scoretype, string country,string region,XmlNodeList scoringRules)
        {
            try
            {
                int score = 0;
                DataTable objoutRowData = null;
                DataTable objoutCustHist = null;
                DataTable objoutCustHistFunValue = null;
                string IsExistingCustomer = Convert.ToString(ds.Tables[1].Rows[0]["flag_customerstatus_his_woe"]);
                if(Convert.ToString(scoretype) == "C")
                {
                    IsExistingCustomer = "N";
                }

                if (Convert.ToString(scoretype) == "A" || Convert.ToString(scoretype) == "B" || Convert.ToString(scoretype) == "P")//code for parallel run
                {
                    //Below we are fatching the rule
                    DScoring eds = new DScoring();
                    eds.Country = country;
                    eds.Region = region;

                    if (IsExistingCustomer == "E")
                        eds.scoretype = 'D';
                    else
                        eds.scoretype = 'C';
                    eds.GetRules();
                    XmlDocument doc = new XmlDocument();
                    foreach (DataRow row in eds.RulesTable.Rows)
                    {
                        doc.LoadXml((string)row[CN.RulesXML]);
                    }
                    XmlNodeList escoringRules = doc.SelectNodes("//Rule[@Type = 'S']");
                    interceptvalue = doc.DocumentElement.Attributes[Tags.InterceptScore].Value == null ? 0 : Convert.ToDecimal(doc.DocumentElement.Attributes[Tags.InterceptScore].Value);
                    scoringRules = escoringRules;
                }
                objoutRowData = CustRowData(acctType, ds.Tables[0], scoringRules);
                objoutCustHist = CustHistData(acctType, ds.Tables[1], scoringRules);
                objoutCustHistFunValue = CustHistFuncData(acctType, objoutRowData, objoutCustHist, scoringRules);

                score = equifaxScoreCal(customerID,accountNo, interceptvalue, IsExistingCustomer, ds.Tables[0],objoutRowData, objoutCustHist, objoutCustHistFunValue);

                return Convert.ToDecimal(score);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable CustRowData(string acctType, DataTable crdt, XmlNodeList scoringRules)
        {
            try
            {
                EquifaxScoreCard objEquifaxScoreCard = new EquifaxScoreCard();
                EquifaxParameters parms = new EquifaxParameters();

                DataTable InputOptiondt = GetCustomerRowDataTable();
                DataRow InputOptionRow = InputOptiondt.NewRow();

                foreach (DataRow row in crdt.Rows)
                { InputOptionRow = row; }
                XmlDocument doc = new XmlDocument();

                DataTable RuleOutputdt = GetCustomerRowDataTable();
                DataRow RuleOutputRow = RuleOutputdt.NewRow();
                RuleOutputRow["employmentstatus_woe"] = 0;
                RuleOutputRow["gender_woe"] = 0;
                RuleOutputRow["maritalstatus_woe"] = 0;
                RuleOutputRow["mobilenumber_woe"] = 0;
                RuleOutputRow["occupation_woe"] = 0;
                RuleOutputRow["residentialstatus_woe"] = 0;
                RuleOutputRow["age"] = Convert.ToInt32(InputOptionRow["age"]);
                RuleOutputRow["numberdependents"] = Convert.ToInt32(InputOptionRow["numberdependents"]);
                RuleOutputRow["timecurrentaddress"] = Convert.ToInt32(InputOptionRow["timecurrentaddress"]);
                RuleOutputRow["timecurrentemployment"] = Convert.ToInt32(InputOptionRow["timecurrentemployment"]);
                RuleOutputRow["postcode_woe"] = 0;
                RuleOutputdt.Rows.Add(RuleOutputRow);
                

                foreach (XmlNode rule in scoringRules)
                {
                    if (((acctType == AT.ReadyFinance || acctType == AT.StoreCard) && rule.Attributes[Tags.ApplyRF].Value == Boolean.TrueString) ||
                        (acctType != AT.Cash && acctType != AT.Special && acctType != AT.ReadyFinance && acctType != AT.StoreCard && rule.Attributes[Tags.ApplyHP].Value == Boolean.TrueString))
                    {
                        objEquifaxScoreCard.EachEvaluateRule(InputOptionRow, rule, parms);
                    }
                    string colName = parms.RuleName;
                    parms.RuleName = "";
                   
                   if (rule.Attributes[Tags.State].Value == Boolean.TrueString)
                    {
                        RuleOutputRow[colName] = Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
                    }
                    else if (rule.Attributes[Tags.State].Value == Boolean.FalseString && parms.IsRuleNumeric)
                    {
                        RuleOutputRow[colName] = parms.RuleValue;
                        parms.RuleValue = 0;
                        parms.IsRuleNumeric = false;
                    }
                    else if (rule.Attributes[Tags.State].Value == Boolean.FalseString && parms.IsOtherText)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(RuleOutputRow[colName])) || Convert.ToString(RuleOutputRow[colName])== "0")
                        {
                            RuleOutputRow[colName] = Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
                        }
                        parms.IsOtherText = false;
                    }
                }
                return RuleOutputdt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable CustHistData(string acctType, DataTable chdt, XmlNodeList scoringRules)
        {
            try
            {
                EquifaxScoreCard objEquifaxScoreCard = new EquifaxScoreCard();
                EquifaxParameters parms1 = new EquifaxParameters();

                DataRow InputOptionRow = null;
                foreach (DataRow row in chdt.Rows)
                { InputOptionRow = row; }

                DataTable RuleOutputdt = GetCustomerHistDataTable();
                DataRow RuleOutputRow = RuleOutputdt.NewRow();
                RuleOutputRow["avg_agreement_total_1m"]         = Convert.ToString(InputOptionRow["avg_agreement_total_1m"]);
                RuleOutputRow["avg_balance_arrears_1m"]         = Convert.ToString(InputOptionRow["avg_balance_arrears_1m"]);
                RuleOutputRow["avg_balance_arrears_12m"]        = Convert.ToString(InputOptionRow["avg_balance_arrears_12m"]);
                RuleOutputRow["balancearrears_pound_6m"]        = Convert.ToString(InputOptionRow["balancearrears_pound_6m"]);
                RuleOutputRow["count_daysarrear_30more_17m"]    = Convert.ToString(InputOptionRow["count_daysarrear_30more_17m"]);
                RuleOutputRow["count_daysarrear_60more_17m"]    = Convert.ToString(InputOptionRow["count_daysarrear_60more_17m"]);
                RuleOutputRow["daysarrears_pound_6m"]           = Convert.ToString(InputOptionRow["daysarrears_pound_6m"]);
                RuleOutputRow["flag_customerstatus_his_woe"]    = Convert.ToString(InputOptionRow["flag_customerstatus_his_woe"]);
                RuleOutputRow["max_perc_outs_3m"]               = Convert.ToString(InputOptionRow["max_perc_outs_3m"]);
                RuleOutputRow["max_perc_outsarrears_6m"]        = Convert.ToString(InputOptionRow["max_perc_outsarrears_6m"]);
                RuleOutputRow["newest_credit"]                  = Convert.ToString(InputOptionRow["newest_credit"]);
                RuleOutputRow["number_account_17m"]             = Convert.ToString(InputOptionRow["number_account_17m"]);
                RuleOutputRow["number_account_opened_3m"]       = Convert.ToString(InputOptionRow["number_account_opened_3m"]);
                RuleOutputRow["oldest_credit"]                  = Convert.ToString(InputOptionRow["oldest_credit"]);
                RuleOutputdt.Rows.Add(RuleOutputRow);

                foreach (XmlNode rule in scoringRules)
                {
                    if (((acctType == AT.ReadyFinance || acctType == AT.StoreCard) && rule.Attributes[Tags.ApplyRF].Value == Boolean.TrueString) ||
                        (acctType != AT.Cash && acctType != AT.Special && acctType != AT.ReadyFinance && acctType != AT.StoreCard && rule.Attributes[Tags.ApplyHP].Value == Boolean.TrueString))
                    {
                        objEquifaxScoreCard.EachEvaluateRule(InputOptionRow, rule, parms1);
                    }
                    string colName = parms1.RuleName;
                    parms1.RuleName = "";
                    if (rule.Attributes[Tags.State].Value == Boolean.TrueString)
                    {
                        RuleOutputRow[colName] = Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
                    }
                    else if (rule.Attributes[Tags.State].Value == Boolean.FalseString && parms1.IsRuleNumeric)
                    {
                        RuleOutputRow[colName] = parms1.RuleValue;
                        parms1.RuleValue = 0;
                        parms1.IsRuleNumeric = false;
                    }
                }
                if(Convert.ToString(RuleOutputRow["flag_customerstatus_his_woe"])=="N" || Convert.ToString(RuleOutputRow["flag_customerstatus_his_woe"]) == "E")
                {
                    RuleOutputRow["flag_customerstatus_his_woe"] = 0;
                }

                return RuleOutputdt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable CustHistFuncData(string acctType, DataTable ocrdt, DataTable ochdt, XmlNodeList scoringRules)
        {
            try
            {
                EquifaxScoreCard objEquifaxScoreCard = new EquifaxScoreCard();
                EquifaxParameters parms2 = new EquifaxParameters();

                DataRow custRowDataRow = null;
                DataRow custHistDataRow = null;
                foreach (DataRow row in ocrdt.Rows)
                { custRowDataRow = row; }
                if (ochdt != null)
                {
                    foreach (DataRow row in ochdt.Rows)
                    { custHistDataRow = row; }
                }
                DataTable RuleOutputdt = GetCustomerFunVariableTable();
                DataRow RuleOutputRow = RuleOutputdt.NewRow();
                RuleOutputRow["avg_agreement_total_1m_sq"]      = Convert.ToDouble(custHistDataRow["avg_agreement_total_1m"]) == 0 ? 0 : Math.Pow(Convert.ToDouble(custHistDataRow["avg_agreement_total_1m"]), 2);
                RuleOutputRow["avg_balance_arrears_12m_ln"]     = Convert.ToDouble(custHistDataRow["avg_balance_arrears_12m"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["avg_balance_arrears_12m"]));
                RuleOutputRow["avg_balance_arrears_1m_ln"]      = Convert.ToDouble(custHistDataRow["avg_balance_arrears_1m"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["avg_balance_arrears_1m"]));
                RuleOutputRow["balancearrears_pound_6m_ln"]     = Convert.ToDouble(custHistDataRow["balancearrears_pound_6m"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["balancearrears_pound_6m"]));
                RuleOutputRow["count_daysarrear_30more_17m_ln"] = Convert.ToDouble(custHistDataRow["count_daysarrear_30more_17m"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["count_daysarrear_30more_17m"])+1);
                RuleOutputRow["count_daysarrear_60more_17m_ln"] = Convert.ToDouble(custHistDataRow["count_daysarrear_60more_17m"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["count_daysarrear_60more_17m"]));               
                RuleOutputRow["max_perc_outs_3m_sq"]            = Convert.ToDouble(custHistDataRow["max_perc_outs_3m"]) == 0 ? 0 : Math.Pow(Convert.ToDouble(custHistDataRow["max_perc_outs_3m"]), 2);
                RuleOutputRow["max_perc_outsarrears_6m_ln"]     = Convert.ToDouble(custHistDataRow["max_perc_outsarrears_6m"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["max_perc_outsarrears_6m"])+1);
                RuleOutputRow["newest_credit_sq"]               = Convert.ToDouble(custHistDataRow["newest_credit"]) == 0 ? 0 : Math.Pow(Convert.ToDouble(custHistDataRow["newest_credit"]), 2);
                RuleOutputRow["number_account_opened_3m_cr"]    = Convert.ToDouble(custHistDataRow["number_account_opened_3m"]) == 0 ? 0 : Math.Ceiling(Math.Pow(Convert.ToDouble(custHistDataRow["number_account_opened_3m"]), (double)1 / 3));
                RuleOutputRow["numberdependents_cr"]            = Convert.ToDouble(custRowDataRow["numberdependents"]) == 0 ? 0 : Math.Ceiling(Math.Pow(Convert.ToDouble(custRowDataRow["numberdependents"]), (double)1 / 3));
                RuleOutputRow["numberdependents_sq"]            = Convert.ToDouble(custRowDataRow["numberdependents"]) == 0 ? 0 : Math.Pow(Convert.ToDouble(custRowDataRow["numberdependents"]), 2);
                RuleOutputRow["oldest_credit_ln"]               = Convert.ToDouble(custHistDataRow["oldest_credit"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custHistDataRow["oldest_credit"])+1);
                RuleOutputRow["timecurrentaddress_ln"]          = Convert.ToDouble(custRowDataRow["timecurrentaddress"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custRowDataRow["timecurrentaddress"])+1);
                RuleOutputRow["timecurrentemployment_ln"]       = Convert.ToDouble(custRowDataRow["timecurrentemployment"]) == 0 ? 0 : Math.Log(Convert.ToDouble(custRowDataRow["timecurrentemployment"])+1);
                RuleOutputRow["timecurrentemployment_sr"]       = Convert.ToDouble(custRowDataRow["timecurrentemployment"]) == 0 ? 0 : Math.Sqrt(Convert.ToDouble(custRowDataRow["timecurrentemployment"]));
                RuleOutputRow["ratio_ndependent_to_age"]        = Convert.ToDouble(custRowDataRow["timecurrentemployment"]) / Convert.ToDouble(custRowDataRow["age"]);
                RuleOutputRow["ratio_tcurrentemploy_to_age"]    = Convert.ToDouble(custRowDataRow["timecurrentemployment"]) / Convert.ToDouble(custRowDataRow["age"]);
                RuleOutputdt.Rows.Add(RuleOutputRow);

                foreach (XmlNode rule in scoringRules)
                {
                    if (((acctType == AT.ReadyFinance || acctType == AT.StoreCard) && rule.Attributes[Tags.ApplyRF].Value == Boolean.TrueString) ||
                        (acctType != AT.Cash && acctType != AT.Special && acctType != AT.ReadyFinance && acctType != AT.StoreCard && rule.Attributes[Tags.ApplyHP].Value == Boolean.TrueString))
                    {
                        objEquifaxScoreCard.EachEvaluateRule(RuleOutputRow, rule, parms2);
                    }
                    string colName = parms2.RuleName;
                    parms2.RuleName = "";
                    if (rule.Attributes[Tags.State].Value == Boolean.TrueString)
                    {
                        RuleOutputRow[colName] = Convert.ToDecimal(rule.Attributes[Tags.Result].Value);
                    }
                    else if (rule.Attributes[Tags.State].Value == Boolean.FalseString && parms2.IsRuleNumeric)
                    {
                        RuleOutputRow[colName] = parms2.RuleValue;
                        parms2.RuleValue = 0;
                        parms2.IsRuleNumeric = false;
                    }
                }
                return RuleOutputdt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int equifaxScoreCal(string customerID,string accountNo, decimal intercept, string custtype,DataTable custInput, DataTable custrowdata, DataTable custhistdat, DataTable custhistfuncdata)
        {
            DataRow custInputDataRow = null;
            DataRow custRowDataRow = null;
            DataRow custHistDataRow = null;
            DataRow custFuncDataRow = null;
            int score = 0;
            foreach (DataRow row in custInput.Rows)
            { custInputDataRow = row; }
            foreach (DataRow row in custrowdata.Rows)
            { custRowDataRow = row; }
            foreach (DataRow row in custhistdat.Rows)
            { custHistDataRow = row; }
            foreach (DataRow row in custhistfuncdata.Rows)
            { custFuncDataRow = row; }

            DScoring s = new DScoring();
            score = s.CalculateScore(
                customerID,
                accountNo,
            intercept,
            custtype,           

            Convert.ToString(custRowDataRow["age"]),
            Convert.ToString(custRowDataRow["employmentstatus_woe"]),
            Convert.ToString(custRowDataRow["gender_woe"]),
            Convert.ToString(custRowDataRow["maritalstatus_woe"]),
            Convert.ToString(custRowDataRow["mobilenumber_woe"]),
            Convert.ToString(custRowDataRow["occupation_woe"]),
            Convert.ToString(custRowDataRow["postcode_woe"]),
            Convert.ToString(custRowDataRow["residentialstatus_woe"]),
            Convert.ToString(custRowDataRow["timecurrentemployment"]),
            Convert.ToString(custRowDataRow["numberdependents"]),
            Convert.ToString(custRowDataRow["timecurrentaddress"]),            
            Convert.ToString(custFuncDataRow["avg_agreement_total_1m_sq"]),
            Convert.ToString(custFuncDataRow["avg_balance_arrears_1m_ln"]),
            Convert.ToString(custFuncDataRow["avg_balance_arrears_12m_ln"]),
            Convert.ToString(custHistDataRow["balancearrears_pound_6m"]),
            Convert.ToString(custFuncDataRow["balancearrears_pound_6m_ln"]),
            Convert.ToString(custFuncDataRow["count_daysarrear_30more_17m_ln"]),
            Convert.ToString(custFuncDataRow["count_daysarrear_60more_17m_ln"]),
            Convert.ToString(custHistDataRow["flag_customerstatus_his_woe"]),
            Convert.ToString(custHistDataRow["daysarrears_pound_6m"]),
            Convert.ToString(custFuncDataRow["max_perc_outs_3m_sq"]),
            Convert.ToString(custFuncDataRow["max_perc_outsarrears_6m_ln"]),
            Convert.ToString(custFuncDataRow["newest_credit_sq"]),
            Convert.ToString(custHistDataRow["number_account_17m"]),
            Convert.ToString(custHistDataRow["number_account_opened_3m"]),
            Convert.ToString(custFuncDataRow["number_account_opened_3m_cr"]),
            Convert.ToString(custFuncDataRow["numberdependents_cr"]),
            Convert.ToString(custFuncDataRow["numberdependents_sq"]),
            Convert.ToString(custFuncDataRow["oldest_credit_ln"]),
            Convert.ToString(custFuncDataRow["ratio_ndependent_to_age"]),
            Convert.ToString(custFuncDataRow["ratio_tcurrentemploy_to_age"]),
            Convert.ToString(custFuncDataRow["timecurrentaddress_ln"]),
            Convert.ToString(custFuncDataRow["timecurrentemployment_ln"]),
            Convert.ToString(custFuncDataRow["timecurrentemployment_sr"]));
            
             return score;
        }
        static DataTable GetCustomerRowDataTable()
        {
            DataTable CRtbl = new DataTable();
            CRtbl.Columns.Add("age", typeof(int));
            CRtbl.Columns.Add("employmentstatus_woe", typeof(string));
            CRtbl.Columns.Add("gender_woe", typeof(string));
            CRtbl.Columns.Add("maritalstatus_woe", typeof(string));
            CRtbl.Columns.Add("mobilenumber_woe", typeof(string));
            CRtbl.Columns.Add("numberdependents", typeof(int));
            CRtbl.Columns.Add("occupation_woe", typeof(string));
            CRtbl.Columns.Add("postcode_woe", typeof(string));
            CRtbl.Columns.Add("residentialstatus_woe", typeof(string));
            CRtbl.Columns.Add("timecurrentaddress", typeof(int));
            CRtbl.Columns.Add("timecurrentemployment", typeof(int));
            return CRtbl;
        }
        static DataTable GetCustomerHistDataTable()
        {
            DataTable CHtbl = new DataTable();
            CHtbl.Columns.Add("avg_agreement_total_1m", typeof(string));
            CHtbl.Columns.Add("avg_balance_arrears_1m", typeof(string));
            CHtbl.Columns.Add("avg_balance_arrears_12m", typeof(string));
            CHtbl.Columns.Add("balancearrears_pound_6m", typeof(string));
            CHtbl.Columns.Add("count_daysarrear_30more_17m", typeof(string));
            CHtbl.Columns.Add("count_daysarrear_60more_17m", typeof(string));
            CHtbl.Columns.Add("daysarrears_pound_6m", typeof(string));
            CHtbl.Columns.Add("flag_customerstatus_his_woe", typeof(string));
            CHtbl.Columns.Add("max_perc_outs_3m", typeof(string));
            CHtbl.Columns.Add("max_perc_outsarrears_6m", typeof(string));
            CHtbl.Columns.Add("newest_credit", typeof(string));
            CHtbl.Columns.Add("number_account_17m", typeof(string));
            CHtbl.Columns.Add("number_account_opened_3m", typeof(string));
            CHtbl.Columns.Add("oldest_credit", typeof(string));
            return CHtbl;
        }
        static DataTable GetCustomerFunVariableTable()
        {
            DataTable CFtbl = new DataTable();
            CFtbl.Columns.Add("avg_agreement_total_1m_sq", typeof(string));
            CFtbl.Columns.Add("avg_balance_arrears_12m_ln", typeof(string));
            CFtbl.Columns.Add("avg_balance_arrears_1m_ln", typeof(string));
            CFtbl.Columns.Add("balancearrears_pound_6m_ln", typeof(string));
            CFtbl.Columns.Add("count_daysarrear_30more_17m_ln", typeof(string));
            CFtbl.Columns.Add("count_daysarrear_60more_17m_ln", typeof(string));
            CFtbl.Columns.Add("max_perc_outs_3m_sq", typeof(string));
            CFtbl.Columns.Add("max_perc_outsarrears_6m_ln", typeof(string));
            CFtbl.Columns.Add("newest_credit_sq", typeof(string));
            CFtbl.Columns.Add("number_account_opened_3m_cr", typeof(string));
            CFtbl.Columns.Add("numberdependents_cr", typeof(string));
            CFtbl.Columns.Add("numberdependents_sq", typeof(string));
            CFtbl.Columns.Add("oldest_credit_ln", typeof(string));
            CFtbl.Columns.Add("timecurrentaddress_ln", typeof(string));
            CFtbl.Columns.Add("timecurrentemployment_ln", typeof(string));
            CFtbl.Columns.Add("timecurrentemployment_sr", typeof(string));
            CFtbl.Columns.Add("ratio_ndependent_to_age", typeof(string));
            CFtbl.Columns.Add("ratio_tcurrentemploy_to_age", typeof(string));
            return CFtbl;
        }

        public class AllEquifaxVariable
        {
            public decimal age { get; set; }
            public decimal avg_agreement_total_1m_sq { get; set; }
            public decimal avg_balance_arrears_12m_ln { get; set; }
            public decimal avg_balance_arrears_1m_ln { get; set; }
            public decimal balancearrears_pound_6m { get; set; }
            public decimal balancearrears_pound_6m_ln { get; set; }
            public decimal count_daysarrear_30more_17m_ln { get; set; }
            public decimal count_daysarrear_60more_17m_ln { get; set; }
            public decimal daysarrears_pound_6m { get; set; }
            public decimal employmentstatus_woe { get; set; }
            public decimal flag_customerstatus_his_woe { get; set; }
            public decimal gender_woe { get; set; }
            public decimal maritalstatus_woe { get; set; }
            public decimal max_perc_outs_3m_sq { get; set; }
            public decimal max_perc_outsarrears_6m_ln { get; set; }
            public decimal mobilenumber_woe { get; set; }
            public decimal newest_credit_sq { get; set; }
            public decimal number_account_17m { get; set; }
            public decimal number_account_opened_3m { get; set; }
            public decimal number_account_opened_3m_cr { get; set; }
            public decimal numberdependents { get; set; }
            public decimal numberdependents_cr { get; set; }
            public decimal numberdependents_sq { get; set; }
            public decimal occupation_woe { get; set; }
            public decimal oldest_credit_ln { get; set; }
            public decimal postcode_woe { get; set; }
            public decimal ratio_ndependent_to_age { get; set; }
            public decimal ratio_tcurrentemploy_to_age { get; set; }
            public decimal residentialstatus_woe { get; set; }
            public decimal timecurrentaddress { get; set; }
            public decimal timecurrentaddress_ln { get; set; }
            public decimal timecurrentemployment { get; set; }
            public decimal timecurrentemployment_ln { get; set; }
            public decimal timecurrentemployment_sr { get; set; }
        }
    }
}

