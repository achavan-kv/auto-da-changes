using System;
using STL.Common;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using STL.Common.Constants.OperandTypes;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.BLL.Equifax
{
    public class EquifaxScoreCard : CommonObject
    {
        private string _operandName = "";
        public string OperandName
        {
            get { return _operandName; }
            set { _operandName = value; }
        }

        private string _operandValue = "";
        public string OperandValue
        {
            get { return _operandValue; }
            set { _operandValue = value; }
        }
        public bool EachEvaluateRule(DataRow details, XmlNode rule, EquifaxParameters Parms)
        {
            try
            {
                /*#if(DEBUG)
                            logMessage("Evaluating rule: "+rule.Attributes[Tags.RuleName].Value, "DEBUG", EventLogEntryType.Information);
                #endif*/
                if ((bool)Country[CountryParameterNames.LoggingEnabled])
                {
                    //logMessage("Evaluating rule: "+rule.Attributes[Tags.RuleName].Value, "DEBUG", EventLogEntryType.Information);

                    //need the operand name to save to the Scoring Details table.
                    OperandName = rule.Attributes[Tags.RuleName].Value;
                }

                //loop through all the clauses and evaluate each one
                int i = 0;
                bool[] results = new bool[2];   //storage for the results of the sub clauses
                string lo = "";
                bool result = false;

                foreach (XmlNode clause in rule.ChildNodes)
                {
                    if (clause.Name != Elements.LogicalOperator)    //ignore logical operators
                    {
                        results[i] = EachEvaluateClause(details, clause, ref Parms);
                        i++;
                    }
                    if (clause.Name == Elements.LogicalOperator)
                        lo = clause.Attributes[Tags.Operator].Value;
                }
                switch (lo)
                {
                    case "AND":
                        result = results[0] && results[1] ? true : false;
                        break;
                    case "OR":
                        result = results[0] || results[1] ? true : false;
                        break;
                    default:
                        result = results[0];
                        break;
                }
                rule.Attributes[Tags.State].Value = result.ToString();
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool EachEvaluateClause(DataRow details, XmlNode clause, ref EquifaxParameters Parms)
        {
            try
            {
                //check a clause and set it to true or false accordingly
                string op1 = "";    //operand 1
                string op2 = "";    //operand 2
                string co = "";     //comparison operator	
                string ot = "";     //operand type
                bool result = false;

                //if it's not a simple clause, we need to evaluate each of
                //if's child clauses
                if (clause.Attributes[Tags.Type].Value == "C")
                {
                    int i = 0;
                    bool[] results = new bool[2];   //storage for the results of the sub clauses
                    string lo = "";
                    foreach (XmlNode child in clause.ChildNodes)
                    {
                        if (child.Name == Elements.Clause)      //exclude logical operator nodes
                        {
                            results[i] = EachEvaluateClause(details, child, ref Parms);
                            i++;
                        }
                        if (child.Name == Elements.LogicalOperator)
                            lo = child.Attributes[Tags.Operator].Value;
                    }
                    switch (lo)
                    {
                        case "AND":
                            result = results[0] && results[1] ? true : false;
                            break;
                        case "OR":
                            result = results[0] || results[1] ? true : false;
                            break;
                        default:
                            break;
                    }
                }
                else        //it's a simple clause and we can evaluate it directly
                {
                    //pick out the operands and operator from the clause node
                    foreach (XmlNode child in clause.ChildNodes)
                    {
                        switch (child.Name)
                        {
                            case Elements.Operand1:
                                op1 = child.Attributes[Tags.Operand].Value;
                                ot = child.Attributes[Tags.Type].Value;
                                break;
                            case Elements.Operand2:
                                op2 = child.Attributes[Tags.Operand].Value;
                                //OperandValue = op2;
                                break;
                            case Elements.ComparisonOperator:
                                co = child.Attributes[Tags.Operator].Value;
                                break;
                            default:
                                break;
                        }
                    }
                    //Parms.rulevalue = Convert.ToInt32(details[op1]);
                    Parms.RuleName = op1;
                    var AccountR = new AccountRepository();
                    //evaluate the clause in different ways depending on the type of
                    //the first operand.
                    if (details.Table.Columns.Contains(Parms.RuleName))
                    {    //            {
                        switch (ot)
                        {

                            case OT.FreeText:
                                // OperandValue = Convert.ToString(details[op1]);
                                //if (op1 == "Worst Current Ever") //allow for automated increase spending limit if customer good enough.  
                                //{
                                //    if (OperandValue != "N") // so not new customer
                                //        Parms.isExistingCustomer = true;
                                //}
                                string InputFreeTextValue = Convert.ToString(details[op1]);
                                string RuleFreeTextValue = op2.Replace("'", "").Trim();
                                string[] FreeTextValueArray = RuleFreeTextValue.Split(',');
                                switch (co)
                                {
                                    case "=":
                                        //result = (Convert.ToString(details[op1])).Trim() == op2.Trim() ? true : false;
                                        foreach (string x in FreeTextValueArray)
                                        {
                                            if (InputFreeTextValue.Contains(x))
                                            {
                                                result = true;
                                            }
                                        }
                                        if (op2.Trim().ToLower() == "other")
                                        {
                                            Parms.IsOtherText = true;
                                        }
                                        break;
                                    case "!=":
                                        result = (Convert.ToString(details[op1])).Trim() != op2.Trim() ? true : false;
                                        break;
                                    case "LIKE":
                                        if ((Convert.ToString(details[op1])).Length < op2.Length)
                                            result = false;
                                        else
                                            result = (Convert.ToString(details[op1])).Substring(0, op2.Length) == op2 ? true : false;
                                        break;
                                    case "!LIKE":
                                        if ((Convert.ToString(details[op1])).Length < op2.Length)
                                            result = true;
                                        else
                                            result = (Convert.ToString(details[op1])).Substring(0, op2.Length) == op2 ? false : true;
                                        break;
                                    default:
                                        break;
                                }
                                //if (op1 == "Worst Settled Ever" && Parms.WorstSettledEver == null)
                                //    Parms.WorstSettledEver = OperandValue;
                                //if (op1 == "Worst Current Ever" && Parms.WorstCurrentEver == null)
                                //    Parms.WorstCurrentEver = OperandValue;

                                //if ((op1 == "Worst Settled Ever" || op1 == "Worst Current Ever") && Parms.ruletype == "Refer" && result == true)
                                //{
                                //    if (Parms.MinimumWorstStatustoCheck == null || Convert.ToInt16(Parms.MinimumWorstStatustoCheck) > Convert.ToInt16(op2.Trim()))
                                //    { //want to get the minimum status to refer
                                //        Parms.MinimumWorstStatustoCheck = op2.Trim();
                                //        Parms.AcceptedSinceSCReferralChecked = false;
                                //    }
                                //    AcceptedSinceSCReferralCheck(ref Parms);
                                //    if (Parms.AcceptedSinceSCReferral)
                                //        result = false;
                                //}
                                //if (Parms.HasMobilePhone == false &&
                                //    Convert.ToString(details["Mobile Phone Y/N"]) == "Y")
                                //    Parms.HasMobilePhone = true;


                                // if ((op1 == "Has work phone Y/N" || op1 == "Has home phone Y/N") && Parms.ruletype == "Refer"
                                //     && co == "=" && Parms.HasMobilePhone)
                                // {
                                //     if (Parms.isExistingCustomer && (bool)Country[CountryParameterNames.ReferExistingCustomersWithoutHomeandWorkPhonesButwithMobiles] == false) // do not check this rule if existing customer
                                //     {
                                //         result = false;
                                //     }
                                //     if (!(bool)Country[CountryParameterNames.ReferNewCustomersWithoutHomeandWorkPhonesButwithMobiles] && !Parms.isExistingCustomer)
                                //     {
                                //         result = false;
                                //     }
                                //}
                                break;

                            case OT.Optional:
                                // OperandValue = Convert.ToString(details[op1]);
                                switch (co)
                                {
                                    case "=":
                                        result = (Convert.ToString(details[op1])).Trim() == op2.Trim() ? true : false;
                                        if (op2.Trim().ToLower() == "o" || op2.Trim().ToLower() == "ot")
                                        {
                                            Parms.IsOtherText = true;
                                        }
                                        break;
                                    case "!=":
                                        result = (Convert.ToString(details[op1])).Trim() != op2.Trim() ? true : false;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            /*
                                                    case OT.Decimal:
                                                        OperandValue = Convert.ToDecimal(details[op1]).ToString();
                                                        decimal opvalue = Convert.ToDecimal(op2);
                                                        if (op1 == "RF Spending Limit" && Convert.ToDecimal(Country[CountryParameterNames.MaxSpendLimitRefer]) != opvalue)
                                                        {
                                                            CountryMaintenanceSetValue CMSV = new CountryMaintenanceSetValue(Parms.conn, Parms.trans);
                                                            CMSV.ExecuteNonQuery("MaxSpendLimitRefer", Convert.ToString(opvalue));
                                                            Country[CountryParameterNames.MaxSpendLimitRefer] = op2;
                                                        }
                                                        if (op1 == "RF Spending Limit" && Parms.ruletype == "Refer" && (co == ">" || co == ">=")) //allow for automated increase spending limit if customer good enough. 
                                                        {
                                                            if (Convert.ToDecimal(OperandValue) > opvalue) //check any uplift percentage
                                                                op2 = Convert.ToString(opvalue +
                                                                    opvalue * AccountR.UpliftPercentage(Parms.AccountNo, Parms.conn, Parms.trans) / 100);
                                                            else
                                                            {
                                                                var Pup = new ProposalUpdateUpliftPercent(Parms.conn, Parms.trans);
                                                                Pup.ExecuteNonQuery(Parms.AccountNo, Parms.CustomerId, 0);
                                                            }
                                                        }
                                                        switch (co)
                                                        {
                                                            case "=":
                                                                result = Convert.ToDecimal(OperandValue) == Convert.ToDecimal(op2) ? true : false;
                                                                break;
                                                            case "!=":
                                                                result = Convert.ToDecimal(OperandValue) != Convert.ToDecimal(op2) ? true : false;
                                                                break;
                                                            case "<":
                                                                result = Convert.ToDecimal(OperandValue) < Convert.ToDecimal(op2) ? true : false;
                                                                break;
                                                            case ">":
                                                                result = Convert.ToDecimal(OperandValue) > Convert.ToDecimal(op2) ? true : false;
                                                                break;
                                                            case "<=":
                                                                result = Convert.ToDecimal(OperandValue) <= Convert.ToDecimal(op2) ? true : false;
                                                                break;
                                                            case ">=":
                                                                result = Convert.ToDecimal(OperandValue) >= Convert.ToDecimal(op2) ? true : false;
                                                                break;
                                                            default:
                                                                break;
                                                        }
                                                        if (op1 == "Monthly Expenses(ex accom)" && Parms.ruletype == "Refer" && co == "<" &&
                                                            !(bool)Country[CountryParameterNames.MinExpenseReferforExistingCustomer]) // do not check this rule if existing customer
                                                            if (Parms.isExistingCustomer)
                                                                result = false;
                                                        break;
                                                        */
                            case OT.Numeric:
                                //OperandValue = Convert.ToInt32(details[op1]).ToString();
                                //if (op1 == "Number of settled accounts" || op1 == "No of Current Accounts") //allow for automated increase spending limit if customer good enough.  Agreement Total(- deposit)
                                //{
                                //    if (Convert.ToInt32(OperandValue) > 0)
                                //        Parms.isExistingCustomer = true;
                                //}
                                switch (co)
                                {
                                    case "=":
                                        result = Convert.ToDecimal(details[op1]) == Convert.ToDecimal(op2) ? true : false;
                                        break;                                            
                                    case "!=":                                           
                                        result = Convert.ToDecimal(details[op1]) != Convert.ToDecimal(op2) ? true : false;
                                        break;
                                    case "<":
                                        result = Convert.ToDecimal(details[op1]) < Convert.ToDecimal(op2) ? true : false;
                                        break;
                                    case ">":
                                        result = Convert.ToDecimal(details[op1]) > Convert.ToDecimal(op2) ? true : false;
                                        break;
                                    case "<=":
                                        result = Convert.ToDecimal(details[op1]) <= Convert.ToDecimal(op2) ? true : false;
                                        break;
                                    case ">=":
                                        result = Convert.ToDecimal(details[op1]) >= Convert.ToDecimal(op2) ? true : false;
                                        break;
                                    default:
                                        if (result == false)
                                        {
                                            Parms.IsRuleNumeric = true;
                                            Parms.RuleValue = Convert.ToDecimal(details[op1]);
                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                //Update the clause node to reflect whether it is true or false
                clause.Attributes[Tags.State].Value = result.ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class EquifaxParameters
    {      
        public string RuleName { get; set; }
        public bool IsRuleNumeric { get; set; }
        public decimal RuleValue{get; set;}
        public bool IsOtherText { get; set; }
    } 
}
