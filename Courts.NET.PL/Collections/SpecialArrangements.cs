using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Structs;
using STL.Common.Static;
using System.Collections;
using STL.Common;

namespace STL.PL.Collections
{
  
    public partial class SpecialArrangements : CommonForm
    {        
        string acctNo;
        string txtAccountNumber;
        int noOfInstalments;
        decimal instalmentAmount;
        decimal highInstalAmount;
        decimal arrangementAmount;
        decimal oddPaymentAmount;
        decimal outstbal;
        decimal serviceCharge;
        decimal insuranceCharge;            //IP - 29/04/10 - UAT(983)UAT5.2
        decimal adminCharge;                //IP - 29/04/10 - UAT(983)UAT5.2
        string selectedPeriod = string.Empty;
        DateTime firstPaymentDate;
        DateTime _serverDate;
        double interestTot;
        double adminTot;
        decimal amount;
        string transtype;
        //string acctType;
        string[] acctArray;
        char period;
        bool arrForConsolidated; //IP - 15/01/09
        string err;
        DateTime finalPayDate = new DateTime(1900, 1, 1);        
        int remainInstals;
        int lowerInstals;

        //string customerID;
        private DataTable arrangementSchedule = new DataTable();
        private SPAAccountDetails arrDetails;
        private DataTable combinedSPADetails = new DataTable();
        private bool _creditBlocked = false;                     //IP - 28/04/10 - UAT(983) UAT5.2

        STL.PL.WS2.SPAAccountDetails spaacctdetails = new STL.PL.WS2.SPAAccountDetails();
        
       

        //IP & JC - 
        private string custID;
        public string CustomerID
        {
            get
            {
                return custID;
            }
            set
            {
                custID = value;
            }
        }

        //IP & JC - 
        private bool acceptBtn;
        public bool AcceptBtn
        {
            get
            {
                return acceptBtn;
            }
            
        }
        private string acctType;
        public string AcctType
        {
            get
            {
                return acctType;
            }
            set
            {
                acctType = value;
            }
        }

        private string _name;
        public string customerName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public DataTable dtSPADetails
        {
            get
            {
                return combinedSPADetails;
            }
           
        }
        
        //IP & JC - 12/01/09 - CR976 - Needed to pass in the CustomerID
        //IP - 28/04/10 - UAT(983) UAT5.2 - Added creditBlocked to the parameter list.
        public  SpecialArrangements(string accountNumber, string customerID, string customerName, bool creditBlocked, DataTable dtReasons)
        {
            custID = customerID;
            _name = customerName;
            _creditBlocked = creditBlocked;     //IP - 28/04/10 - UAT(983) UAT5.2
            this._serverDate = StaticDataManager.GetServerDate();

            InitializeComponent();
            //IP - 01/10/08 - Set the account number to the account selected in the 'Telephone Actions' screen.
            txtAccountNumber = accountNumber;            

            //IP - Set the 'Reason' drop downs datasource to the static data table passed in from the
            //'Telephone Actions' screen.
            drpReason.DataSource = dtReasons;
            drpReason.DisplayMember = CN.CodeDescription;
            drpReason.ValueMember = CN.Code;
            cbPrintSchedule.Checked = true;
      
        }

        //Method that loads the account details for the selected account.
        //The method 'GetSPAAcctDetails' returns a struct that is used to set the account details
        //which is then used to set the alues for the fields.
        private void LoadSPADetails()
        { 
            string Error = "";
            acctNo = txtAccountNumber.Replace("-", "");

            try
            {
                //IP - Retrieve the details for the account and set the values of a struct 
                //to the values retrieved which will be displayed on the screen.
                //STL.PL.WS2.SPAAccountDetails spaacctdetails  = new STL.PL.WS2.SPAAccountDetails();
                spaacctdetails = AccountManager.GetSPAAcctDetails(acctNo,out Error);

                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
                else
                {
                    //Set the fields to the values in the struct.
                    this.Text = "Special Arrangements - " + acctNo;
                    txtCurrBalance.Text = spaacctdetails.Outstbal.ToString();
                    txtArrears.Text = spaacctdetails.Arrears.ToString();
                    txtMonthlyInstalment.Text = spaacctdetails.Instalamount.ToString();
                    dpDateAcctOpen.Text = spaacctdetails.DateAcctOpen.ToString();
                    txtPercentPaid.Text = spaacctdetails.PercentPaid.ToString();
                    dpFinPayDate.Text = spaacctdetails.FinalPayDate.ToString();
                    txtInterest.Text = spaacctdetails.Interest.ToString();
                    txtAdmin.Text = spaacctdetails.Admin.ToString();
                    // save charges for comparison
                    interestTot = (double)spaacctdetails.Interest;
                    adminTot = (double)spaacctdetails.Admin;
                    acctType = (string)spaacctdetails.AcctType;
                    firstPaymentDate = dtFirstPaymentDate.Value;                    
                    txtTerm.Text = spaacctdetails.Term.ToString();
                    txtArrangementAmt.Text = spaacctdetails.Arrears.ToString();
                    
                }

               
                lblPeriodType.Visible = rbNormal.Checked;
                cbArrSchedule.Visible = false;
                lblArrSchedule.Visible = false;
                btnAccept.Enabled = false;

                // Enable charges reversal if > zero
                btnAdminRev.Enabled = adminTot > 0;
                btnInterestRev.Enabled = interestTot > 0;
                txtAdmin.Enabled = adminTot > 0;
                txtInterest.Enabled = interestTot > 0;
                btnViewConsolidated.Enabled = false;
                btnCalculate.Enabled = false;

                drpPeriodType.Visible = rbNormal.Checked;
                //drpPeriodType.Text = Convert.ToString(drpPeriodType.Items[0]);
                drpPeriodType.Text = Convert.ToString(drpPeriodType.Items[1]); //IP - 08/12/09 - UAT(933) Default to Monthly
                txtServiceChg.Text = Convert.ToString(0 * 1.00);

                // Calc normal arrangement
                numNoOfIns_ValueChanged(null, null);

                // Show view consolidated if this is an 'Ready Finance' account.
                if (acctType == "R")
                {
                    btnViewConsolidated.Visible = true;
                    lblViewConsolidated.Visible = true;
                }
                else
                {
                    btnViewConsolidated.Visible = false;
                    lblViewConsolidated.Visible = false;
                }

                //Do not display the following check boxes until the 'Refinance' option has been selected.
                rbExtendTerm.Visible = false;
                rbExtendTerm.Enabled = false;
                rbTermRemains.Visible = false;
                rbTermRemains.Enabled = false;
                grpAddInt.Visible = false;
                txtHighInstalAmt.Visible = false;

                if ((bool)Country[CountryParameterNames.PrintSPAscheduleOfPayments] == false)       //UAT1012 jec
                {
                    cbPrintSchedule.Enabled = false;
                    cbPrintSchedule.Checked = false;
                }
                else
                {
                    cbPrintSchedule.Enabled = true;
                    cbPrintSchedule.Checked = true;
                }

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private void SpecialArrangements_Load(object sender, EventArgs e)
        {
            try
            {
                Wait();
                //Load the 'Account Details' of the selected account.
                LoadSPADetails();              

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                StopWait();
            }
        }

        //When the number of instalments has been changed then the 'Arrangement Amount'
        //will be calculated.
        private void numNoOfIns_ValueChanged(object sender, EventArgs e)
        {
                    
            //Store the Number of Instalments entered.
            noOfInstalments = Convert.ToInt32(numNoOfIns.Value);
            // Set the review date
            if (rbNormal.Checked == true && selectedPeriod != string.Empty)
            {
                if(Convert.ToString(selectedPeriod[0])=="W")
                    dtReviewDate.Value = dtFirstPaymentDate.Value.AddDays(noOfInstalments*7);
                if(Convert.ToString(selectedPeriod[0])=="F")
                    dtReviewDate.Value = dtFirstPaymentDate.Value.AddDays(noOfInstalments*14);
                if(Convert.ToString(selectedPeriod[0])=="M")
                    dtReviewDate.Value = dtFirstPaymentDate.Value.AddMonths(noOfInstalments);
            }
            else
            {
                dtReviewDate.Value = dtFirstPaymentDate.Value.AddMonths(noOfInstalments);
            }
            //Store the instalment amount entered.
            instalmentAmount = Convert.ToDecimal((txtInstalmentAmt.Text));
            outstbal = Convert.ToDecimal(txtCurrBalance.Text);            
            errorProvider1.SetError(this.txtOddPaymentAmt, "");
            
            if (errorProvider1.GetError(dtFirstPaymentDate).Length == 0 && errorProvider1.GetError(dtReviewDate).Length == 0
                 && instalmentAmount > 0)
            {
                btnViewConsolidated.Enabled = true;
                btnCalculate.Enabled = true;
            }

            //Local variable to hold the number of instalments
            int instals = 0;
            instals = Convert.ToInt32(numNoOfIns.Value);
            // calculate service charge
            if (cbAddInt.Checked)
            {     
                //IP - 30/04/10 - UAT(983) UAT5.2 
                //Calculate insurance and admin charge if applicable.
                if (spaacctdetails.InsPcent > 0)
                {
                    //insuranceCharge = outstbal * instals / 12 * spaacctdetails.InsPcent / 100;
                    insuranceCharge = (outstbal - spaacctdetails.Rebate) * instals / 12  * spaacctdetails.InsPcent / 100;
                }

                //Calculate admin charge if applicable.
                if (spaacctdetails.AdminPcent > 0)
                {
                    //adminCharge = outstbal * instals / 12 * spaacctdetails.AdminPcent / 100;
                    adminCharge = (outstbal - spaacctdetails.Rebate) * instals / 12 * spaacctdetails.AdminPcent / 100;
                }

                //serviceCharge =outstbal * instals / 12 * spaacctdetails.ServPcent / 100;  
 
                serviceCharge = Math.Round(((outstbal - spaacctdetails.Rebate) + adminCharge + insuranceCharge) * instals / 12 * spaacctdetails.ServPcent / 100,2); //IP - 23/09/10 - UAT(1017) UAT5.2 - Re-instated line.
                //serviceCharge = Math.Round(((outstbal - spaacctdetails.Rebate) * instals / 12 * spaacctdetails.ServPcent / 100) + adminCharge + insuranceCharge, 2);    //jec - 07/07/10 - UAT1023 //IP - 23/09/10 - UAT(1017) UAT5.2 - Commented out as incorrect calculation for Service Charge 

                //serviceCharge = Convert.ToDecimal(Math.Floor(Convert.ToDouble(serviceCharge)));

                
                
            }
            else
            {
                serviceCharge = 0;
            }
            try
            {                
                if (rbNormal.Checked == true)
                {
                    //calculate the 'Odd Payment Amount' as one of the instalments.
                    if (noOfInstalments > 1)
                    {
                        instalmentAmount = Math.Floor(Math.Floor(arrangementAmount) / instals);
                        oddPaymentAmount = arrangementAmount - ((instalmentAmount * instals) - instalmentAmount);
                    }
                    else
                    {
                        instalmentAmount = arrangementAmount;
                        oddPaymentAmount = 0;
                    }

                   // oddPaymentAmount = outstbal - ((instalmentAmount * noOfInstalments) - instalmentAmount);
                    txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                    txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);      // UAT931 jec + ".00";
                    if (oddPaymentAmount < 0)
                    {
                        errorProvider1.SetError(this.txtOddPaymentAmt, GetResource("M_ODDPAYMENT"));
                        btnViewConsolidated.Enabled = false;
                        btnCalculate.Enabled = false;
                    }
                    //Call the method that calculates the 'Arrangement Amount'
                    //and pass in the 'Odd Payment Amount' if one has been entered.
                    CalculateArrangementAmount(oddPaymentAmount);
                }
                else if(rbExtendTerm.Checked == true)
                {
                    instals = Convert.ToInt32(numNoOfIns.Value);
                    if (instals > 1)
                    {
                        //instals = instals - 1;
                        //instalmentAmount = Math.Ceiling(Math.Ceiling(outstbal+serviceCharge) / instals);
                        instalmentAmount = Math.Ceiling(Math.Ceiling((outstbal - spaacctdetails.Rebate) + insuranceCharge + adminCharge + serviceCharge) / instals); //IP - 30/04/10 - UAT(983) UAT5.2 --IP - 24/09/10 - UAT(1017) - Re-instated line
                        //instalmentAmount = Math.Ceiling(Math.Ceiling((outstbal - spaacctdetails.Rebate) + serviceCharge) / instals); //jec - 07/07/10 - UAT1023 //IP - 24/09/10 - UAT(1017) - Commented out line
                        //If Instalment * Term-1 > balance + service charge - reduce instalment amount by 1 otherwise odd payment would be -ve
                        //if ((instalmentAmount * (instals - 1)) > (outstbal + serviceCharge))
                        if ((instalmentAmount * (instals - 1)) > ((outstbal - spaacctdetails.Rebate) + insuranceCharge + adminCharge + serviceCharge))  //IP - 30/04/10 - UAT(983) UAT5.2 --IP - 24/09/10 - UAT(1017) - Re-instated line
                        //if ((instalmentAmount * (instals - 1)) > ((outstbal - spaacctdetails.Rebate) + serviceCharge))  //jec - 07/07/10 - UAT1023    //IP - 24/09/10 - UAT(1017) - Commented out line
                        {
                            instalmentAmount -= 1;
                        }
                        //oddPaymentAmount = (outstbal+serviceCharge) - ((instalmentAmount * instals) - instalmentAmount);
                        oddPaymentAmount = Math.Round(((outstbal - spaacctdetails.Rebate) + insuranceCharge + adminCharge + serviceCharge) - ((instalmentAmount * instals) - instalmentAmount), 2);  //IP - 30/04/10 - UAT(983) UAT5.2 //IP - 24/09/10 - UAT(1017) - Re-instated line
                        //oddPaymentAmount = Math.Round(((outstbal - spaacctdetails.Rebate) + serviceCharge) - ((instalmentAmount * instals) - instalmentAmount), 2);  //jec - 07/07/10 - UAT1023 //IP - 24/09/10 - UAT(1017) - Commented out line
                        
                    }
                    else
                    {
                        //instalmentAmount = outstbal+serviceCharge;
                        instalmentAmount = Math.Round((outstbal - spaacctdetails.Rebate) + insuranceCharge + adminCharge + serviceCharge, 2);          //IP - 30/04/10 - UAT(983)UAT5.2 //IP - 24/09/10 - UAT(1017) - Re-instated line
                        //instalmentAmount = Math.Round((outstbal - spaacctdetails.Rebate) + serviceCharge, 2);          //jec - 07/07/10 - UAT1023 //IP - 24/09/10 - UAT(1017) - Commented out line
                        oddPaymentAmount = 0;
                        //numNoOfIns.Value = 1;
                    }
                    //arrangementAmount = outstbal;     
                    arrangementAmount = outstbal - spaacctdetails.Rebate;               //IP - 30/04/10 - UAT(983)UAT5.2
                    arrangementAmount = arrangementAmount + insuranceCharge + adminCharge + serviceCharge; //IP - 24/09/10 - UAT(1017) - Re-instated line
                    //arrangementAmount = arrangementAmount + serviceCharge;      //jec - 07/07/10 - UAT1023 //IP - 24/09/10 - UAT(1017) - Commented out line
                    txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                    
                    if (instalmentAmount-Math.Floor(instalmentAmount)==0)       //UAT1022  jec 02/07/10
                    {
                    txtInstalmentAmt.Text = Convert.ToString(instalmentAmount)+".00";
                    }
                    else
                    {
                        txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
                    }
                    
                    txtArrangementAmt.Text = Convert.ToString(arrangementAmount);
                    txtServiceChg.Text = Convert.ToString(serviceCharge); 
 
                }
                else if (rbTermRemains.Checked == true)
                {

                    TermRemains();

                }
                dgArrangementSched.DataSource = null;
                btnAccept.Enabled = false;
            }
           
            catch (Exception ex)
            {
                
                throw ex;
            }

        }

        private void CalculateArrangementAmount(decimal oddPayAmount)
        {
            if (errorProvider1.GetError(dtFirstPaymentDate).Length == 0 && errorProvider1.GetError(dtReviewDate).Length == 0)
            {
                btnViewConsolidated.Enabled = true;
                btnCalculate.Enabled = true;
            }
            if (oddPaymentAmount >= 0)
            {
                errorProvider1.SetError(this.txtOddPaymentAmt, "");
            }
            try
            {
                //If an 'Odd Payment Amount' has been entered.
                if (oddPaymentAmount > 0)
                {
                    //Then calculate the 'Arrangement Amount' including the 'Odd Payment Amount' as one of
                    //the instalments.
                    arrangementAmount = (instalmentAmount * noOfInstalments) - instalmentAmount + (oddPaymentAmount);
                }
                //Calculate the 'Arrangement Amount' from the instalment amount entered and
                //the number of instalments.
                else
                {
                    arrangementAmount = (instalmentAmount * noOfInstalments);
                }

                txtArrangementAmt.Text = Convert.ToString(arrangementAmount);
            }
            catch (Exception ex)    
            {
                
                throw ex;
            }
        }      

        private void txtInstalmentAmt_Leave(object sender, EventArgs e)
        {
            int instals = 0;

            if (IsNumeric(txtInstalmentAmt.Text) && txtInstalmentAmt.Text.Length > 0)
            {
                instalmentAmount = Convert.ToDecimal(txtInstalmentAmt.Text);
            }
            else
            {
                instalmentAmount = 0;
                txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
            }

            instalmentAmount = Convert.ToDecimal((txtInstalmentAmt.Text));
            noOfInstalments = Convert.ToInt32(numNoOfIns.Value);
            outstbal = Convert.ToDecimal(txtCurrBalance.Text);
            if (instalmentAmount * noOfInstalments > outstbal && rbExtendTerm.Checked == false)
            {
                instalmentAmount = Math.Floor(outstbal / noOfInstalments);
                txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
            }

            btnViewConsolidated.Enabled = false;
            if (Convert.ToDouble(txtInstalmentAmt.Text) > 0)
            {
                if (rbNormal.Checked == true)
                {
                    //Calculate the 'Arrangement Amount' if the user leaves the 'Instalment Amount' field.
                    numNoOfIns_ValueChanged(null, null);
                    //calculate the 'Odd Payment Amount' as one of the instalments.                
                    oddPaymentAmount = arrangementAmount - ((instalmentAmount * noOfInstalments) - instalmentAmount);
                    txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                   
                    btnViewConsolidated.Enabled = true;
                    btnCalculate.Enabled = true;
                }
                else if(rbExtendTerm.Checked == true)
                {                 
                    //Check number of instalments not greater than max term.
                    instalmentAmount = Convert.ToDecimal((txtInstalmentAmt.Text));
                    //noOfInstalments = Convert.ToInt32(Math.Floor(outstbal / instalmentAmount)); jec 15/04/09 
                    noOfInstalments = Convert.ToInt32(Math.Floor(arrangementAmount / instalmentAmount));
                   
                    instals = noOfInstalments;
                    if (instals == 1)
                    {
                        instalmentAmount = outstbal;
                        oddPaymentAmount = 0;                       
                    }
                    else
                    {
                        oddPaymentAmount = outstbal - ((instalmentAmount * instals) - instalmentAmount);
                        //numNoOfIns.Value = spaacctdetails.Term + (instals - (spaacctdetails.Term - spaacctdetails.CurrInstNo));
                    }
                    numNoOfIns.Value = instals;
                    txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                    txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
                  
                }
                else if (rbTermRemains.Checked == true)
                {

                    instalmentAmount = Convert.ToDecimal(txtInstalmentAmt.Text);
                    TermRemains();
                    btnCalculate.Enabled = true;
                }
            }
            btnCalculate.Focus();
            dgArrangementSched.DataSource = null;
        }

        private void txtOddPaymentAmt_Leave(object sender, EventArgs e)
        {
           
            try
            {
                int instals = 0;
                if (IsNumeric(txtOddPaymentAmt.Text) && txtOddPaymentAmt.Text.Length > 0)
                {
                    oddPaymentAmount = Convert.ToDecimal(txtOddPaymentAmt.Text);
                }
                else
                {
                    oddPaymentAmount = 0;
                    txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                }
                
                if (!rbTermRemains.Checked == true)
                {
                    instals = Convert.ToInt32(numNoOfIns.Value);

                    instalmentAmount = Math.Floor(Math.Floor(outstbal) / instals);
                    oddPaymentAmount = outstbal - ((instalmentAmount * instals) - instalmentAmount);
                    txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                    txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
                    arrangementAmount = outstbal;
                    txtArrangementAmt.Text = Convert.ToString(arrangementAmount);
                }
              
                CalculateArrangementAmount(oddPaymentAmount);                
                dgArrangementSched.DataSource = null;
            }
            catch (Exception ex)    
            {
                
                throw ex;
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (rbNormal.Checked)
            {
                selectedPeriod = drpPeriodType.SelectedItem.ToString();
            }
            else
            {
                selectedPeriod = "Monthly";
            }
            //Store the first character of the selected period
            period = Convert.ToChar(selectedPeriod[0]);
            firstPaymentDate = dtFirstPaymentDate.Value;
            // fill struct
            fillStruct();


            //DataTable arrangementSchedule = new DataTable();
            arrangementSchedule = null;

            outstbal = Convert.ToDecimal(txtCurrBalance.Text);

            try
            {
                if (instalmentAmount == 0)
                {
                    //Set the 'Number of Instalments' back to what was originally displayed.
                    ShowInfo("M_INSTALMENTAMT");
                }
                else
                {
                    //If the 'Arrangement Amount' is calculated to be greater than the 'Arrears' amount
                    //then disable the 'Calculate' button.
                    if (arrangementAmount > outstbal+serviceCharge && rbNormal.Checked) //IP - 04/05/10 - UAT(983) UAT5.2
                    {
                        ShowInfo("M_ARRANGEMENTGREATERARREARS");
                        //txtArrangementAmt.Text = string.Empty;
                    }
                    else
                    {
                        SPACalculateSingle();
                        // apply dynamic formatting 
                        formatGrid();                        
                    }
                }
                // apply dynamic formatting 
                //formatGrid();
                btnAccept.Enabled = true;                            
            }
            catch (Exception ex)    
            {
                throw ex;
            }
        }
        // Reverse Interest ot Admin value
        private void ReverseCharges()
        {
            try
            {
                PaymentManager.WriteGeneralTransaction(txtAccountNumber,
                                                      Convert.ToInt16(Config.BranchCode),
                                                      amount,transtype,
                                                      "", "", "", 0, Config.CountryCode,
                                                      "",0, out Error);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        // Reverse interest and display remaining interest
        private void btnInterestRev_click(object sender, EventArgs e)
        {
            transtype = "INT";
            amount = Convert.ToDecimal(StripCurrency(txtInterest.Text)) * -1;
            ReverseCharges();
            interestTot += (double)amount;
            
            // display status message
            statusStrip1.Text = "Interest " + txtInterest.Text + " reversed ";

            txtInterest.Text = Convert.ToString(interestTot);

            //Reload account details
            LoadSPADetails();
            numNoOfIns_ValueChanged(null, null);        //jec 20/4/09  force recalc of instal amount

        }
        // Reverse admin chgs and display remaining admin chgs
        private void btnAdminRev_click(object sender, EventArgs e)
        {
            transtype = "ADM";
            amount = Convert.ToDecimal(StripCurrency(txtAdmin.Text)) * -1;
            ReverseCharges();
            adminTot += (double)amount;
            // display status message
            statusStrip1.Text = "Admin Charge " + txtAdmin.Text + " reversed ";
            txtAdmin.Text = Convert.ToString(adminTot);

            //Reload account details
            LoadSPADetails();
        }

        private void btnViewConsolidated_click(object sender, EventArgs e)
        {
            try
            {
                Function = "Arrangement Screen: Arrangement List popup button";

                Wait();
                selectedPeriod = drpPeriodType.SelectedItem.ToString();
                //Store the first character of the selected period
                period = Convert.ToChar(selectedPeriod[0]);
                // fill struct
                fillStruct();

                SPACalculateCombined(); 
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void btnCancel_click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAccept_click(object sender, EventArgs e)
        {
            //If arrangements have not been setup against consolidated accounts
            //then we need to clear the 'combinedSPADetails' table which holds
            //the details for the consolidated accounts and instead add a row
            //for the account on the 'Special Arrangements' screen.

            if (cbPrintSchedule.Checked && (rbExtendTerm.Checked == false || rbExtendTerm.Visible == false)) //IP - 04/05/10 - UAT(983) UAT5.2
            {
                printSchedule();
            }

            acceptBtn = true;

            if (arrForConsolidated == false)
            {
                if (combinedSPADetails.Columns.Count == 0)
                {
                        combinedSPADetails.TableName = TN.Arrangements;

                        combinedSPADetails.Columns.Add(CN.AccountNo);
                        combinedSPADetails.Columns.Add(CN.OutstandingBalance);
                        combinedSPADetails.Columns.Add(CN.Arrears);
                        combinedSPADetails.Columns.Add(CN.InstalAmount);
                        combinedSPADetails.Columns.Add(CN.ArrangementAmount);
                        combinedSPADetails.Columns.Add(CN.OddPayment);
                        combinedSPADetails.Columns.Add(CN.Period);
                        combinedSPADetails.Columns.Add(CN.NoOfIns);
                        combinedSPADetails.Columns.Add(CN.FirstPayDate);
                        combinedSPADetails.Columns.Add(CN.FinalPayDate);
                        combinedSPADetails.Columns.Add(CN.ReasonCode);
                        combinedSPADetails.Columns.Add(CN.ReviewDate);
                        combinedSPADetails.Columns.Add(CN.FreezeIntAdmin);
                        combinedSPADetails.Columns.Add(CN.ServiceCharge);
                        combinedSPADetails.Columns.Add(CN.ExtendTerm);
                        combinedSPADetails.Columns.Add(CN.NoOfRemainInstals);
                        combinedSPADetails.Columns.Add(CN.RemainInstalAmt);
                        combinedSPADetails.Columns.Add(CN.CustID);
                        combinedSPADetails.Columns.Add(CN.AcctType);
                        combinedSPADetails.Columns.Add(CN.TermsType);
                        combinedSPADetails.Columns.Add(CN.RefinDeposit);
                        combinedSPADetails.Columns.Add(CN.CashPrice);
                        combinedSPADetails.Columns.Add(CN.DueDay);
                        combinedSPADetails.Columns.Add(CN.ScoringBand);         //IP - 23/09/10 - UAT(1017)UAT5.2
                        combinedSPADetails.Columns.Add("PrintSched");           //UAT1012 jec

                }
                combinedSPADetails.Rows.Clear();
                //Add a row to the datatable for the single account that the 
                //schedule is being calculated for in the 'Special Arrangements' screen.

                DataRow dr;
                dr = combinedSPADetails.NewRow();

                dr[CN.AccountNo] = acctNo; 
                dr[CN.OutstandingBalance] = txtCurrBalance.Text;
                dr[CN.Arrears] = txtArrears.Text;
                dr[CN.InstalAmount] = txtInstalmentAmt.Text; 
                dr[CN.ArrangementAmount] = arrDetails.ArrangementAmount.ToString();
                dr[CN.OddPayment] = arrDetails.OddPayment.ToString();
                dr[CN.Period] = arrDetails.Period;
                dr[CN.NoOfIns] = arrDetails.NumInstal;
                dr[CN.FirstPayDate] = arrDetails.FirstPayDate;
                dr[CN.FinalPayDate] = dtFinalPaymentDate.Value.ToString();
                dr[CN.ReviewDate] = dtReviewDate.Value.ToString();
                dr[CN.CustID] = custID;
                dr[CN.ScoringBand] = arrDetails.ScoringBand;                //IP - 23/09/10 - UAT(1017)UAT5.2
                //If 'Refinance - Extended Terms selected then also include these)
                if (rbRefinance.Checked == true && rbExtendTerm.Checked == true)
                {
                    //dr[CN.InstalAmount] = txtInstalmentAmt.Text;
                    dr[CN.FreezeIntAdmin] = Convert.ToString(cbFreezeInt.Checked);
                    dr[CN.ServiceCharge] = txtServiceChg.Text;
                    dr[CN.ExtendTerm] = Convert.ToString(rbExtendTerm.Checked);
                    dr[CN.ArrangementAmount] = arrDetails.CashPrice + Convert.ToDecimal(dr[CN.ServiceCharge]);

                }
                else
                {
                    dr[CN.FreezeIntAdmin] = Convert.ToString(cbFreezeInt.Checked);
                    dr[CN.ServiceCharge] = 0;
                    dr[CN.ExtendTerm] = "false";
                    //dr[CN.ArrangementAmount] = arrDetails.CashPrice + Convert.ToDecimal(dr[CN.ServiceCharge]);
                }
                //If 'Term Remains' then include the following.
                if (rbRefinance.Checked == true && rbTermRemains.Checked == true)
                {
                    dr[CN.NoOfRemainInstals] = remainInstals;
                    dr[CN.RemainInstalAmt] = txtHighInstalAmt.Text;
                }
                else
                {
                    dr[CN.NoOfRemainInstals] = 0;
                    dr[CN.RemainInstalAmt] = 0;
                }

                dr["PrintSched"] = cbPrintSchedule.Checked;      //UAT1012 jec

                combinedSPADetails.Rows.Add(dr);
            }

            foreach (DataRow dr in combinedSPADetails.Rows)
            {
                dr[CN.ReasonCode] = drpReason.SelectedValue.ToString();
                dr[CN.ReviewDate] = dtReviewDate.Value.ToString();               
                dr[CN.CustID] = custID;
                dr[CN.AcctType] = arrDetails.AcctType;
                dr[CN.TermsType] = arrDetails.TermsType;
                dr[CN.RefinDeposit] = arrDetails.RefinDeposit;
                dr[CN.CashPrice] = arrDetails.CashPrice;
                dr[CN.DueDay] = arrDetails.DueDay;
            
                if (arrForConsolidated == true)
                {
                    dr[CN.NoOfRemainInstals] = 0;
                    dr[CN.RemainInstalAmt] = 0;
                    dr[CN.ServiceCharge] = 0;
                }
            }            

            Close();            

        }

        private void rbNormal_checkChanged(object sender, EventArgs e)
        {
            remainInstals = 0;
            highInstalAmount = 0;

            if (rbNormal.Checked == true)
            {
                //IP - 28/04/10 - UAT(983) UAT5.2
                CreditBlockedDisableFields(true);

                txtInstalmentAmt.Text = Convert.ToString(0);

                lblArrangementAmt.Text = "Arrangement Amount";
                lblNumOfIns.Text = "Number of Instalments";
                numNoOfIns.Minimum = 1;
                numNoOfIns.Maximum = 36;
                numNoOfIns.Value = 6;
               

                txtOddPaymentAmt.Text = Convert.ToString(0);
                oddPaymentAmount = 0;
                rbExtendTerm.Checked = true;
                txtArrangementAmt.Text = spaacctdetails.Arrears.ToString();
                dgArrangementSched.DataSource = null;
                // Calc normal arrangement
                numNoOfIns_ValueChanged(null, null);

            }

            //IP - 02/02/10 - UAT(977) - Only make the View Consolidated button visible if the account is an RF account.
            btnViewConsolidated.Visible = rbNormal.Checked && acctType == "R"; 
            lblViewConsolidated.Visible = rbNormal.Checked && acctType == "R";

            lblArrangementAmt.Visible = rbNormal.Checked;
            txtArrangementAmt.Visible = rbNormal.Checked;
           
            drpPeriodType.Visible = rbNormal.Checked;
            lblPeriodType.Visible = rbNormal.Checked;

            rbExtendTerm.Visible = !rbNormal.Checked;
            rbExtendTerm.Enabled = !rbNormal.Checked;

            rbTermRemains.Visible = !rbNormal.Checked;
            rbTermRemains.Enabled = !rbNormal.Checked;

            grpAddInt.Visible = !rbNormal.Checked;

            if (Convert.ToDecimal(txtInstalmentAmt.Text) == 0)
            {
                btnCalculate.Enabled = false;
                btnViewConsolidated.Enabled = false;
            }
        }

        private void txtInterest_leave(object sender, EventArgs e)
        {
            if (!IsStrictNumeric(txtInterest.Text) || txtInterest.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(this.txtInterest, GetResource("M_NUMERIC"));                
            }
            else
                if (Convert.ToDouble(txtInterest.Text) > interestTot || Convert.ToDouble(txtInterest.Text) < 0)
                    errorProvider1.SetError(this.txtInterest, GetResource("M_VALUEBETWEEN", new object[] { 0, interestTot }));
            else
                errorProvider1.SetError(this.txtInterest, "");

            btnInterestRev.Enabled = interestTot > 0 && Convert.ToDouble(txtInterest.Text) > 0;
        }

        private void txtAdmin_leave(object sender, EventArgs e)
        {
            if (!IsStrictNumeric(txtAdmin.Text) || txtAdmin.Text.Trim().Length == 0)
            {
                errorProvider1.SetError(this.txtAdmin, GetResource("M_NUMERIC"));
            }
            else
                if (Convert.ToDouble(txtAdmin.Text) > adminTot || Convert.ToDouble(txtAdmin.Text)<0)
                    errorProvider1.SetError(this.txtAdmin, GetResource("M_VALUEBETWEEN", new object[] { 0, adminTot }));
            else
                errorProvider1.SetError(this.txtAdmin, "");

            btnAdminRev.Enabled = adminTot > 0 && Convert.ToDouble(txtAdmin.Text) > 0;

        }

        private void cbArrSchedule_SelectedIndexChanged(object sender, EventArgs e)
        {
            arrangementSchedule.DefaultView.RowFilter = "AcctNo  = " + cbArrSchedule.SelectedItem.ToString();
            dgArrangementSched.DataSource = arrangementSchedule.DefaultView;
            formatGrid();

        }

         private void fillStruct()
        {
            //arrDetails = arrDetails = spaacctdetails;;
            arrDetails.ArrangementAmount = Convert.ToDecimal(txtArrangementAmt.Text);
            arrDetails.Instalamount = Convert.ToDecimal(txtInstalmentAmt.Text);
            arrDetails.OddPayment = Convert.ToDecimal(txtOddPaymentAmt.Text);
            arrDetails.NumInstal = Convert.ToInt32(numNoOfIns.Value);
            arrDetails.FirstPayDate = dtFirstPaymentDate.Value;
            arrDetails.Period = Convert.ToString(period);
            arrDetails.FinalPayDate = Convert.ToDateTime(dpFinPayDate.Text);
            arrDetails.AccountNo = acctNo;
            arrDetails.AcctType = spaacctdetails.AcctType;
            arrDetails.TermsType = spaacctdetails.TermsType;
            arrDetails.RefinDeposit = spaacctdetails.RefinDeposit;
            arrDetails.CashPrice = spaacctdetails.CashPrice;
            arrDetails.DueDay = spaacctdetails.DueDay;
            arrDetails.ScoringBand = spaacctdetails.ScoringBand;            //IP - 23/09/10 - UAT(1017)UAT5.2
        }

         private void formatGrid()
         {
             dgArrangementSched.DataSource = arrangementSchedule;
             dgArrangementSched.Width = 310;
             dgArrangementSched.Columns[0].Visible = false;
             dgArrangementSched.Columns[1].Width = 40;
             dgArrangementSched.Columns[1].HeaderText = "Instal No";
             dgArrangementSched.Columns[2].Width = 75;
             dgArrangementSched.Columns[3].Width = 75;  
             dgArrangementSched.Columns[4].Width = 75;
             if (dgArrangementSched.RowCount <= 8)
             {
                 dgArrangementSched.Height = (dgArrangementSched.RowCount - 1) * 22 + 72;
             }
             else
             {
                 dgArrangementSched.Width = 325;
                 dgArrangementSched.Height = 240;
             }
         }

         private void drpPeriodType_SelectedIndexChanged(object sender, EventArgs e)
         {
             if (btnCalculate.Enabled == true || btnViewConsolidated.Enabled == true)
             {
                 selectedPeriod = drpPeriodType.SelectedItem.ToString();
                 period = Convert.ToChar(selectedPeriod[0]);
                 arrDetails.Period = Convert.ToString(period);

                 if (btnCalculate.Enabled == false)
                 {
                     SPACalculateCombined();

                     if (combinedSPADetails.Rows.Count != 0)
                     {
                         foreach (DataRow dr in combinedSPADetails.Rows)
                         {
                             dr[CN.Period] = period;
                         }
                     }
                 }
                 else
                 {
                     SPACalculateSingle();
                     formatGrid();
                 }
             }
         }

         private void SPACalculateCombined()
         {

                 SpecialArrangementsConsolidated spac = new SpecialArrangementsConsolidated(CustomerID, customerName, arrDetails, combinedSPADetails);
                 spac.ShowDialog();

                dgArrangementSched.DataSource = null;
                cbArrSchedule.Visible = false;
                lblArrSchedule.Visible = false;
                cbArrSchedule.Items.Clear();
                btnCalculate.Enabled = true;
                btnAccept.Enabled = false;                

                //Populate the local array from the 'SpecialArrangementConsolidated'.
                acctArray = spac.ArrangementAccts;
                arrangementSchedule = spac.ArrangementScheduleForAccts;
                combinedSPADetails = spac.CombinedSPADetails;

                int arrsize=0;
                arrsize=acctArray.GetLength(0);
                int idx=0;
                if (acctArray != null)              // no accounts returned
                {
                    while (idx < arrsize && acctArray[idx] != null)
                    {
                        cbArrSchedule.Items.Add(acctArray[idx]).ToString();
                        idx += 1;
                    }
                    arrForConsolidated = false;
                }

                if (idx != 0)     // at least one account returned
                {
                    idx = 0;
                    cbArrSchedule.SelectedIndex = 0;
                    arrangementSchedule.DefaultView.RowFilter = "AcctNo  = " + cbArrSchedule.SelectedItem.ToString();
                    dgArrangementSched.DataSource = arrangementSchedule.DefaultView;

                    btnCalculate.Enabled = false;
                    cbArrSchedule.Visible = true;
                    lblArrSchedule.Visible = true;
                    btnAccept.Enabled = true;
                    arrForConsolidated = true;

                    //Set the 'Final Payment Date' textbox value.
                    dtFinalPaymentDate.Text = combinedSPADetails.Rows[0][CN.FinalPayDate].ToString();
                }
         }

         private void SPACalculateSingle()
         {
             int noOfIns = noOfInstalments;
             //Method calculates the 'Arrangement Schedule' and displays the schedule on the screen.
             arrangementSchedule = AccountManager.SPACalculateArrangementSchedule(acctNo, period, arrangementAmount, noOfIns,
                                                       instalmentAmount, oddPaymentAmount, firstPaymentDate,remainInstals,highInstalAmount, out finalPayDate,out err);

             //Set the finalPayDate on the table to the parameter returned from 'SPACalculateArrangementSchedule'.
             dtFinalPaymentDate.Text = finalPayDate.ToString();
         }

         private void groupBox1_Enter(object sender, EventArgs e)
         {

         }

         private void txtArrangementAmt_changed(object sender, EventArgs e)
         {
             arrangementAmount=Convert.ToDecimal(txtArrangementAmt.Text);
             if (arrangementAmount > outstbal + serviceCharge && rbNormal.Checked) //IP - 04/05/10 - UAT(983) - Only applies for Normal
             {
                 btnViewConsolidated.Enabled = false;
                 btnCalculate.Enabled = false;
             }
         }

         private void rbRefinance_CheckedChanged(object sender, EventArgs e)
         {
             //Display the 'Extended Term' and 'Term Remains' checkboxes if 'Refinance' selected.
             if (rbRefinance.Checked == true)
             {
                 dgArrangementSched.DataSource = null; //IP - 28/04/10 - UAT(983) UAT5.2

                 btnViewConsolidated.Visible = false;
                 lblViewConsolidated.Visible = false;

                 rbExtendTerm.Visible = true;
                 rbExtendTerm.Enabled = true;

                 rbTermRemains.Visible = true;
                 rbTermRemains.Enabled = true;

                 if (rbExtendTerm.Checked == true)
                 {
                     if (_creditBlocked == false)
                     {
                         lblNumOfIns.Text = "New Term";
                         // do not allow entry of instalment if extended term
                         txtInstalmentAmt.Enabled = false;
                         txtOddPaymentAmt.Enabled = false;

                         //IP - 22/09/10 - UAT(1017)UAT5.2 - If Extended Term then set the Minimum to the minimum allowable for the Terms Type.
                         if (spaacctdetails.MinTerm > 0)
                         {
                             numNoOfIns.Minimum = spaacctdetails.MinTerm;
                         }
                         else
                         {
                             numNoOfIns.Minimum = 1;
                         }
                     }
                     else
                     {

                         CreditBlockedDisableFields(false);

                         ShowInfo("M_CUSTCREDBLOCKED");
                        
                     }
                 }
               
                 //Set the max number 
                 //IP - 05/05/10 - UAT(983) UAT5.2
                 if (spaacctdetails.Term > spaacctdetails.MaxTerm)
                 {
                     numNoOfIns.Maximum = spaacctdetails.Term;
                 }
                 else
                 {
                     numNoOfIns.Maximum = spaacctdetails.MaxTerm;
                 }
                 
                 //If the current number of instalments is atleast 4 less than max term
                 //term remains is allowed.                
                 if (spaacctdetails.CurrInstNo <= spaacctdetails.Term - 4)
                 {
                     rbTermRemains.Enabled = true;
                 }
                 else
                 {
                     rbTermRemains.Enabled = false;
                     txtHighInstalAmt.Visible = false;
                 }

                     //numNoOfIns.Minimum = spaacctdetails.Term;
                 //IP - 28/04/10 - UAT(983) UAT5.2 - btnCalculate was being enabled when customers credit is blocked and extended term selected.
                 //therefore prevent the below from firing the numNoOfIns_ValueChanged event.
                 if (!(rbExtendTerm.Checked && _creditBlocked))
                 {
                     numNoOfIns.Value = (decimal)spaacctdetails.Term;
                 }
             }
             btnAccept.Enabled = false;
         }

        //IP - 28/04/10 - UAT(983) UAT5.2
         private void CreditBlockedDisableFields(bool disable)
         {
             rbExtendTerm.Enabled = disable;
             btnCalculate.Enabled = disable;
             txtArrangementAmt.Enabled = disable;
             numNoOfIns.Enabled = disable;
             txtInstalmentAmt.Enabled = disable;
             txtOddPaymentAmt.Enabled = disable;
             drpReason.Enabled = disable;
             dtFirstPaymentDate.Enabled = disable;
             //dtFinalPaymentDate.Enabled = disable;
             dtReviewDate.Enabled = disable;
             btnViewConsolidated.Enabled = disable;

             if (Convert.ToDouble(txtInterest.Text) != 0)
             {
                 txtInterest.Enabled = disable;
                 btnInterestRev.Enabled = disable;
             }
            
             if (Convert.ToDouble(txtAdmin.Text) != 0)
             {
                 txtAdmin.Enabled = disable;
                 btnAdminRev.Enabled = disable;
             }
             
             cbAddInt.Enabled = disable;
             cbFreezeInt.Enabled = disable;
             cbArrSchedule.Enabled = disable;
             cbPrintSchedule.Enabled = disable;
         }

         private void rbExtendTerm_CheckedChanged(object sender, EventArgs e)
         {
             //_extendTermSelected = rbExtendTerm.Checked;

             if (rbExtendTerm.Checked == true)
             {
                
                     lblArrangementAmt.Visible = false;
                     txtHighInstalAmt.Visible = false;
                     txtArrangementAmt.Visible = false;
                     if (!rbNormal.Checked == true)
                     {
                         lblNumOfIns.Text = "New Term";
                     }
                     lblInstalmentAmt.Text = "Arrangement Instalment Amount";
                     txtHighInstalAmt.Visible = false;
                     //set values as at original load 
                     rbRefinance_CheckedChanged(null, null);
                     grpAddInt.Visible = true;
                     // do not allow entry of instalment if extended term
                     txtInstalmentAmt.Enabled = false;
                     txtOddPaymentAmt.Enabled = false;
               
             }
             else
             {
                 txtInstalmentAmt.Enabled = true;
                 txtOddPaymentAmt.Enabled = true;
                 txtHighInstalAmt.Visible = true;
                 btnCalculate.Enabled = false;
             }
         }

         private void rbTermRemains_CheckedChanged(object sender, EventArgs e)
         {
             int lowerInstal = 0;
             numNoOfIns.Minimum = 1;    //IP - 22/09/10 - UAT(1017) - UAT5.2

             Int32.TryParse(Country[CountryParameterNames.SpaMaxLowerInstals].ToString(), out lowerInstal);

             if (rbTermRemains.Checked == true)
             {
                 //IP - 28/04/10 - UAT(983) UAT5.2
                 CreditBlockedDisableFields(true);

                 lblArrangementAmt.Visible = false;
                 txtArrangementAmt.Visible = false;
                 lblInstalmentAmt.Text = "Lower Instalment Amount";
                 lblNumOfIns.Text = "Number of Instalments";

                 txtHighInstalAmt.Visible = true;
                 lblArrangementAmt.Visible = true;
                 lblArrangementAmt.Text = "Remain Instalment Amount";
                 grpAddInt.Visible = false;

                 txtInstalmentAmt.Text = Convert.ToString(0);
                 txtOddPaymentAmt.Text = Convert.ToString(0);
                 txtHighInstalAmt.Text = Convert.ToString(0);

                 //Set max to lower or term - currinsno
                 if (spaacctdetails.Term - spaacctdetails.CurrInstNo < lowerInstal)
                 {
                     numNoOfIns.Value = spaacctdetails.Term - spaacctdetails.CurrInstNo;
                     numNoOfIns.Maximum = numNoOfIns.Value;
                 }
                 else
                 {
                     numNoOfIns.Value = lowerInstal;
                     numNoOfIns.Maximum = lowerInstal;
                 }
             }
             btnAccept.Enabled = false;
         }

         private void TermRemains()
         {
             //int remainInstals = 0;
             //int lowerInstals = 0;
             decimal x = 0;
             if (instalmentAmount > 0)
             {
                 //Lower instals entered
                 lowerInstals = Convert.ToInt32(numNoOfIns.Value);
                 remainInstals = (spaacctdetails.Term - spaacctdetails.CurrInstNo) - lowerInstals;

                 if (remainInstals > 0)
                 {
                     if (Math.Floor((outstbal - (instalmentAmount * lowerInstals)) / remainInstals) < 0)
                     {
                         instalmentAmount = instalmentAmount + highInstalAmount;
                     }
                     highInstalAmount = Math.Floor((outstbal - (instalmentAmount * lowerInstals)) / remainInstals); //Higher Instal

                     oddPaymentAmount = outstbal - (instalmentAmount * lowerInstals)
                                                 - ((highInstalAmount * remainInstals) - highInstalAmount);

                 }
                 else
                 {
                     highInstalAmount = 0;
                     oddPaymentAmount = outstbal - ((instalmentAmount * lowerInstals) - instalmentAmount);

                 }

                 if (oddPaymentAmount < 0)
                 {
                     x = Math.Abs(Math.Floor(oddPaymentAmount / lowerInstals - 1));
                     instalmentAmount = instalmentAmount - x;
                     TermRemains();
                 }


                 if (remainInstals == 1)
                 {
                     highInstalAmount = 0;
                 }
                 arrangementAmount = outstbal;
                 txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
                 txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
                 txtArrangementAmt.Text = Convert.ToString(arrangementAmount);
                 txtHighInstalAmt.Text = Convert.ToString(highInstalAmount);
             }
         }

         private void dtFirstPaymentDate_valueChanged(object sender, EventArgs e)
            {
             
             errorProvider1.SetError(this.dtFirstPaymentDate, "");
             if (dtFirstPaymentDate.Value < _serverDate)
             {
                 errorProvider1.SetError(this.dtFirstPaymentDate, GetResource("M_INCORRECTDATE"));
                 btnViewConsolidated.Enabled = false;
                 btnCalculate.Enabled = false;
             }
             else
             {
                 btnViewConsolidated.Enabled = btnCalculate.Enabled=instalmentAmount > 0;                
             }

             btnAccept.Enabled = false;
             dgArrangementSched.DataSource = null;
             //dtReviewDate.Value = _serverDate;
             dtReviewDate.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
             dtReviewDate.Value = dtFirstPaymentDate.Value;
             dtReviewDate.Text = Convert.ToString(dtReviewDate.Value);

          }

         private void dtReviewDate_ValueChanged(object sender, EventArgs e)
         {
             errorProvider1.SetError(this.dtReviewDate, "");
             if (dtReviewDate.Value < _serverDate && dtReviewDate.Value != new System.DateTime(1900, 1, 1, 0, 0, 0, 0))
             {
                 errorProvider1.SetError(this.dtReviewDate, GetResource("M_INCORRECTDATE"));
                 btnViewConsolidated.Enabled = false;
                 btnCalculate.Enabled = false;
             }
             else
             {
                 btnViewConsolidated.Enabled = btnCalculate.Enabled = instalmentAmount > 0;                
             }
         }

         private void txtArrangementAmt_Leave(object sender, EventArgs e)
         {
             arrangementAmount = Math.Abs(arrangementAmount);
             
             instalmentAmount = Math.Floor(arrangementAmount / noOfInstalments);
             //calculate the 'Odd Payment Amount' as one of the instalments.                
             oddPaymentAmount = arrangementAmount - ((instalmentAmount * noOfInstalments) - instalmentAmount);
             txtOddPaymentAmt.Text = Convert.ToString(oddPaymentAmount);
             txtInstalmentAmt.Text = Convert.ToString(instalmentAmount);
             txtArrangementAmt.Text = Convert.ToString(arrangementAmount);
         
         }

         private void cbAddInt_CheckedChanged(object sender, EventArgs e)
         {
             //treat as if Number of instalments has changed to force recalculation
             numNoOfIns_ValueChanged(null, null);
         }

         private void printSchedule()
         {
             int noBrowsers = 4;
             int noPrints = 1;

             ((MainForm)FormRoot).browsers = ((CommonForm)FormRoot).CreateBrowserArray(noBrowsers);
                 noBrowsers = 0;
                 PrintArrScheduleOfPayments(((MainForm)FormRoot).browsers[noBrowsers++], arrDetails.AccountNo
                     , custID, arrDetails.AcctType, ref noPrints, period, arrangementAmount, noOfInstalments,
                     instalmentAmount, oddPaymentAmount, firstPaymentDate, remainInstals, highInstalAmount);
         }
    }
}