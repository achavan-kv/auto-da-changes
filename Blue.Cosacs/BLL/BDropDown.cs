using System;
using STL.DAL;
using STL.Common;
using System.Data;
using STL.Common.Static;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;
using Blue.Cosacs;
using System.Collections.Generic;
using Blue.Cosacs.Repositories;


namespace STL.BLL
{
    /// <summary>
    /// Used to process requests for static drop down list data
    /// Accepts an array of custom objects which each comprise
    /// of the name of the drop down and a variable length list
    /// of parameters which are required for the query
    /// </summary>
    public class BDropDown : CommonObject
    {
        int empeeno; //IP - 26/09/08 - UAT5.2 - UAT(529)
  

        public DataSet GetDropDownData(XmlNode dropDowns)
        {
            DataSet ds = new DataSet();

            foreach (XmlNode node in dropDowns.ChildNodes)
            {
                DCategory cat = new DCategory();
                DCode code = new DCode();
                DUser user = new DUser();
                DTransType tt = new DTransType();
                DEmployee emp = new DEmployee();
                DCollectionsModule dCollections = new DCollectionsModule();
                
                switch (node.Attributes[Tags.Name].Value)
                {

                    case TN.StoreCardStatus:
                        break;
                    case TN.TermsType:
                        DTermsType terms = new DTermsType();
                        if (terms.GetTermsTypeSummary(node.FirstChild) == (int)Return.Success)
                            ds.Tables.Add(terms.TermsTypes);
                        break;
                    case TN.TermsTypeBandList:
                         DTermsType termsbandlist = new DTermsType();
                         ds.Tables.Add(termsbandlist.TermsTypeBandListGet());
                        break;
                    case TN.TermsTypeBand:
                        DTermsType termBands = new DTermsType();
                        if (termBands.GetTermsTypeBands(node.FirstChild) == (int)Return.Success)
                            ds.Tables.Add(termBands.TermsTypeBands);
                        break;
                    case TN.SourceOfAttraction:
                        DSourceOfAttraction soa = new DSourceOfAttraction();
                        if (soa.GetSOASummary() == (int)Return.Success)
                            ds.Tables.Add(soa.SOA);
                        break;
                    case TN.SalesStaff:
                        if (emp.GetStaffSummary(node.FirstChild) == (int)Return.Success)
                            ds.Tables.Add(emp.SalesStaff);
                        break;
                    case TN.AllStaff:
                        if (emp.GetAllStaff() == (int)Return.Success)
                            ds.Tables.Add(emp.AllStaff);
                        break;
                    case TN.CommStaff:              // jec 14/06/07
                        if (emp.GetCommStaff(node.FirstChild) == (int)Return.Success)
                            ds.Tables.Add(emp.CommStaff);
                        break;
                    case TN.SalesCommStaff:         
                        if (emp.GetSalesCommStaff(node.FirstChild) == (int)Return.Success)
                            ds.Tables.Add(emp.SalesCommStaff);
                        break;
                    case TN.MethodOfPayment:
                        DMethodOfPayment mop = new DMethodOfPayment();
                        if (mop.GetMOPSummary() == (int)Return.Success)
                            ds.Tables.Add(mop.MOP);
                        break;
                    case TN.AccountType:
                        DAccountType accttype = new DAccountType();
                        if (accttype.GetAccountTypes(node.FirstChild) == (int)Return.Success)
                            ds.Tables.Add(accttype.AccountTypes);
                        break;
                    case TN.BranchNumber:
                        DBranch branch = new DBranch();
                        if (branch.GetAllBranchNos() == (int)Return.Success)
                            ds.Tables.Add(branch.BranchNumbers);
                        break;
                    case TN.CustomerCodes:
                        if (cat.GetCustomerCodes() == (int)Return.Success)
                            ds.Tables.Add(cat.CustCodes);
                        break;
                    case TN.AccountCodes:
                        if (cat.GetAccountCodes() == (int)Return.Success)
                            ds.Tables.Add(cat.AcctCodes);
                        break;
                    case TN.UserTypes:
                        if (user.GetUserTypes() == (int)Return.Success)
                            ds.Tables.Add(user.UserTypes);
                        break;
                    case TN.UserFunctions:
                        if (user.GetUserFunctions() == (int)Return.Success)
                            ds.Tables.Add(user.UserFunctions);
                        break;
                    case TN.AddressType:
                        DCategory addr = new DCategory();
                        if (addr.GetAddressTypes() == (int)Return.Success)
                            ds.Tables.Add(addr.AddressType);
                        break;
                    case TN.Bank:
                        DBank bank = new DBank();
                        if (bank.GetBankCodes() == (int)Return.Success)
                            ds.Tables.Add(bank.Table);
                        break;
                    case TN.ApplicationType:
                        DProposal prop = new DProposal();
                        if (prop.GetApplicationTypes(node.FirstChild, TN.ApplicationType) == (int)Return.Success)
                            ds.Tables.Add(prop.ApplicationTypes);
                        break;
                    case TN.DDDueDate:
                        BDDDueDay dueDay = new BDDDueDay();
                        if (dueDay.GetDueDayList() == (int)Return.Success)
                            ds.Tables.Add(dueDay.dueDayList);
                        break;
                    case TN.ProductCategories:
                        if (code.GetProductCategories(TN.ProductCategories) == (int)Return.Success)
                            ds.Tables.Add(code.Codes);
                        break;
                    case TN.Deposits:
                        ds.Tables.Add(tt.GetDepositTypes(TN.Deposits));
                        break;
                    case TN.NonDeposits:
                        ds.Tables.Add(tt.GetNonDepositTypes(TN.NonDeposits));
                        break;
                    case TN.GeneralTransactions:
                        ds.Tables.Add(tt.GetGeneralTransactionTypes());
                        break;
                    case TN.WriteOffCodes:
                        if (code.GetWriteOffCategories(TN.WriteOffCodes) == (int)Return.Success)
                            ds.Tables.Add(code.Codes);
                        break;
                    case TN.EndPeriods:
                        DAccount acct = new DAccount();
                        if (acct.GetAllPeriodEndDates() == (int)Return.Success)
                            ds.Tables.Add(acct.EndDates);
                        break;
                    case TN.CountryParameterCategories:
                        if (code.GetCategoryCodes(node.FirstChild, node.Attributes[Tags.Name].Value) == (int)Return.Success)
                        {
                            if (!(bool)Country[CountryParameterNames.TierPCEnabled])
                            {
                                string tierCategory = Country[CountryParameterNames.TierPCEnabled, CN.ParameterCategory];
                                foreach (DataRow row in code.Codes.Rows)
                                    if ((string)row[CN.Code] == tierCategory)
                                    {
                                        // Remove the Privilege Club category to hide it from the users
                                        row.Delete();
                                    }
                                code.Codes.AcceptChanges();
                            }
                            ds.Tables.Add(code.Codes);
                        }
                        break;
                    case TN.EODConfigurations:
                        DInterfaceControl ic = new DInterfaceControl();
                        if (ic.GetEODConfigurations() == (int)Return.Success)
                            ds.Tables.Add(ic.Control);
                        break;
                    case TN.ServiceMake:
                        DServiceRequest srMake = new DServiceRequest();
                        ds.Tables.Add(srMake.LoadPriceIndexMakes());
                        break;
                    case TN.ServiceModel:
                        DServiceRequest srModel = new DServiceRequest();
                        ds.Tables.Add(srModel.LoadPriceIndexModels());
                        break;
                    case TN.Technician:
                        DServiceRequest sr = new DServiceRequest();
                        ds.Tables.Add(sr.GetTechnicians(Date.blankDate));
                        break;
                    case TN.TechnicianByZone:
                        DServiceRequest srz = new DServiceRequest();
                        ds.Tables.Add(srz.GetTechniciansByZone(Date.blankDate));
                        break;
                   case TN.WorkList:
                      DCollectionsModule coll = new DCollectionsModule();
                      coll.GetWorkList();
                      ds.Tables.Add(coll.DataTableCollections);
                      break;
                   case TN.EODOptions: //IP - 22/04/08 - UAT(223) v.5.1 - Added case as previously was going to 'default'. This makes it a little less confusing to follow.
                      if (code.GetEodOptions(node.FirstChild, node.Attributes[Tags.Name].Value) == (int)Return.Success)
                          ds.Tables.Add(code.EodCodes);
                        break;
                      //CR 852
                   case TN.StrategyActions:
                      DCollectionsModule colls = new DCollectionsModule();
                      //colls.GetAllStrategyActions();
                      //IP - 26/09/08 - UAT5.2 - UAT(529) - retrieve the worklist actions for worklists
                      //assigned to the employee logged in.
                      colls.GetStrategyWorklistActions(empeeno);
                      ds.Tables.Add(colls.DataTableCollections);
                      break;
                    case TN.InsuranceTypes:
                      if (cat.GetInsuranceTypes() == (int)Return.Success)
                          ds.Tables.Add(cat.InsuranceTypes);
                      break;
                    case TN.InstallationItemCat:
                      // # 14432 Installation items
                      var Inst = new InstallationRepository();
                      DataTable dt=Inst.GetInstallationItems();
                      dt.TableName=TN.InstallationItemCat;
                      ds.Tables.Add(dt);                      
                      break;
                    case TN.CashLoanDisbursementMethods:
                      ds.Tables.Add(tt.GetCashLoanDisbursementMethods(TN.CashLoanDisbursementMethods));
                      break;
                    case TN.Villages:
                        DataSet dsVillages = dCollections.GetVillages();
                        DataTable dtVillage = dsVillages.Tables[0].Copy();
                        dsVillages.Tables.RemoveAt(0);
                        dtVillage.TableName = TN.Villages;
                        DataRow intialRow = dtVillage.NewRow();
                        intialRow["Village"] = "--Select Village--";
                        dtVillage.Rows.InsertAt(intialRow, 0);
                        ds.Tables.Add(dtVillage);
                        break;
                    default:
                        if (code.GetCategoryCodes(node.FirstChild, node.Attributes[Tags.Name].Value) == (int)Return.Success)
                            ds.Tables.Add(code.Codes);
                        break;
                }
                cat = null;
                code = null;
                user = null;
                tt = null;
                emp = null;
                dCollections = null;
            }

           
                     
            return ds;
        }

        public BDropDown()
        {
            
        }

        //IP - 26/09/08 - UAT5.2 - UAT(529) - Created a new constructor
        //that takes in the user passed into it from the web method
        //allowing access to the employee number of the employee logged in.
        public BDropDown(int user)
        {
              empeeno = user;

        }
    }
}
