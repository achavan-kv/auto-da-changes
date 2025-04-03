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
using System.Collections;

namespace STL.PL.Collections
{
    public partial class SpecialArrangementsConsolidated : CommonForm 
    {
        private string error;

        private DataTable combinedSPADetailsDT = new DataTable();
        private DataTable arrangementSchedule = new DataTable();
        private DataTable arrangementScheduleMerged = new DataTable();
        private DataView ArrangementListView = null;
        private SPAAccountDetails arrangementDet;


        private string[] arrAccts = null;
        public string[] ArrangementAccts
        {
            get
            {
                return arrAccts;
            }
            set
            {
                arrAccts = value;
            }
        }


        public DataTable ArrangementScheduleForAccts
        {
            get
            {
                return arrangementScheduleMerged;
            }
            set
            {
                arrangementScheduleMerged = value;
            }
        }

        public DataTable CombinedSPADetails
        {
            get
            {
                return combinedSPADetailsDT;
            }
            //set
            //{
            //    combinedSPADetailsDT = value;
            //}
        }
       
        public SpecialArrangementsConsolidated()
        {
            InitializeComponent();
        }


        public SpecialArrangementsConsolidated(string customerID, string name, SPAAccountDetails arrDetails, DataTable acctsSPADetailsDT)
        {
            InitializeComponent();

            //DataTable combinedSPADetailsDT = new DataTable();
            //combinedSPADetailsDT = null;

            //Initialise the local struct with the struct (that has its field members populated) passed into
            //the constructor.
            arrangementDet = arrDetails;

            if (acctsSPADetailsDT.Rows.Count == 0)
            {
                LoadData(customerID, name, arrDetails);
            }
            else
            {
                combinedSPADetailsDT = acctsSPADetailsDT;

                //Apply a style to the grid and set the source of the grid.
                ApplyTableStyleAndGridSource();
  
                //Set the text boxes to display the details.
                txtCustomerName.Text = name;
                txtCustId.Text = customerID;
                txtArrangementAmt.Text = Convert.ToString((arrDetails.Instalamount));
                txtExtraAmount.Text = arrDetails.OddPayment.ToString();

                
            }
         
        }

        public void LoadData(string customerID, string name, SPAAccountDetails arrDetails)
        {
            
            //IP & JC - 12/01/09 - CR976 - Special Arrangements
            //Need to retrieve the consolidated RF accounts for the customer
            //and populate the 'dgSPAList' grid.
            

                DataSet ds = CollectionsManager.AccountGetCombinedSPADetails(customerID, out error);
                combinedSPADetailsDT = ds.Tables[0];

                //Add extra columns needed which will be hidden.
                combinedSPADetailsDT.Columns.Add(CN.Period);
                combinedSPADetailsDT.Columns.Add(CN.NoOfIns);
                combinedSPADetailsDT.Columns.Add(CN.FirstPayDate);
                combinedSPADetailsDT.Columns.Add(CN.FinalPayDate);
                combinedSPADetailsDT.Columns.Add(CN.ReasonCode);
                combinedSPADetailsDT.Columns.Add(CN.ReviewDate);
                // Columns added jec 15/06/09
                combinedSPADetailsDT.Columns.Add(CN.FreezeIntAdmin);
                combinedSPADetailsDT.Columns.Add(CN.ServiceCharge);
                combinedSPADetailsDT.Columns.Add(CN.ExtendTerm);
                combinedSPADetailsDT.Columns.Add(CN.NoOfRemainInstals);
                combinedSPADetailsDT.Columns.Add(CN.RemainInstalAmt);
                combinedSPADetailsDT.Columns.Add(CN.CustID);
                combinedSPADetailsDT.Columns.Add(CN.AcctType);
                combinedSPADetailsDT.Columns.Add(CN.TermsType);
                combinedSPADetailsDT.Columns.Add(CN.RefinDeposit);
                combinedSPADetailsDT.Columns.Add(CN.CashPrice);
                combinedSPADetailsDT.Columns.Add(CN.DueDay);                
                
                //Apply a style to the grid and set the source of the grid.
                ApplyTableStyleAndGridSource();

                //Set the text boxes to display the details.
                txtCustomerName.Text = name;
                txtCustId.Text = customerID;
                txtArrangementAmt.Text = Convert.ToString((arrDetails.Instalamount));
                txtExtraAmount.Text = arrDetails.OddPayment.ToString();
                // set values on selected account
                foreach (DataRow dr in ArrangementListView.Table.Rows)
                {
                    if (dr[CN.AccountNo].ToString()==arrDetails.AccountNo.ToString())
                    {
                        dr[CN.ArrangementAmount] = arrDetails.Instalamount;
                        dr[CN.OddPayment] = arrDetails.OddPayment;
                        dr[CN.DateLast] = arrDetails.FirstPayDate.AddMonths(arrDetails.NumInstal);
                    }
                }
           
            

        }

        private void AddColumnStyle(string columnName, DataGridTableStyle tabStyle,
            int width, bool readOnly, string headerText, string format)
        {
            DataGridTextBoxColumn aColumnTextColumn = new DataGridTextBoxColumn();
            aColumnTextColumn.MappingName = columnName;
            aColumnTextColumn.Width = width;
            aColumnTextColumn.ReadOnly = readOnly;
            aColumnTextColumn.HeaderText = headerText;
            aColumnTextColumn.Format = format;
            tabStyle.GridColumnStyles.Add(aColumnTextColumn);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            ValidateArrangmentAndExtra();
          
        }

        public void ValidateArrangmentAndExtra()
        {
            //Need to check that the SPA amounts and Extra amounts entered across the RF accounts are 
            //equal to the Arrangment Amount and Odd Payment amounts entered on the 'Special Arrangements'
            //screen.
            //Loop through the rows in the datatable
            decimal arrangementInsTot = 0;
            decimal oddPaymentAmountTot = 0;
            bool validationOK = false;
            int rowCount = 0;
            DateTime finalPayDate = new DateTime(1900, 1, 1);

            rowCount = ArrangementListView.Table.Rows.Count;

            //Set the size of the array to the number of accounts displayed in the list.
            arrAccts = new string[rowCount];

            errorProvider1.SetError(this.dgSPAList, "");

            foreach (DataRow dr in ArrangementListView.Table.Rows)
            {
                dr[CN.ArrangementAmount] = Math.Round(Convert.ToDecimal(dr[CN.ArrangementAmount]), 2);
                dr[CN.OddPayment] = Math.Round(Convert.ToDecimal(dr[CN.OddPayment]), 2);

                arrangementInsTot += Convert.ToDecimal(dr[CN.ArrangementAmount]);
                oddPaymentAmountTot += Convert.ToDecimal(dr[CN.OddPayment]);

                dr[CN.Period] = arrangementDet.Period.ToString();
                dr[CN.NoOfIns] = arrangementDet.NumInstal.ToString();
                dr[CN.FirstPayDate] = arrangementDet.FirstPayDate.ToString();

                

                //If an Odd Payment amount has been entered but no Arrangement Amount
                //then we need to display a message as an Odd Payment amount can only be entered against
                //an account that has an Arrangement amount entered.
                if (Convert.ToDecimal(dr[CN.ArrangementAmount]) == 0 && Convert.ToDecimal(dr[CN.OddPayment]) > 0)
                {
                    errorProvider1.SetError(this.dgSPAList, GetResource("M_SPACONSOLIDATEDMSG"));
                    //validationOK = false;
                    return;
                }

            }

            //Check that the total Arrangement amounts and Odd Payment amounts entered against each account
            //are equal to the calculated Arrangement amount and Odd Payment amount.
            if (arrangementInsTot != Convert.ToDecimal(txtArrangementAmt.Text.ToString()))
            {
                errorProvider1.SetError(this.dgSPAList, GetResource("M_ARRANGEMENTAMOUNT"));
                //validationOK = false;
                return;

            }
            else if (oddPaymentAmountTot != Convert.ToDecimal(txtExtraAmount.Text.ToString()))
            {
                errorProvider1.SetError(this.dgSPAList, GetResource("M_ODDPAYMENTAMT"));
                //validationOK = false;
                return;
            }
            else
            {
                //No messages were encountered so we are ok to continue.
                validationOK = true;

            }

            //If no messages encountered then we need to select the accounts
            //where an Arranegent amount has been entered and save them in 
            //the datatable 'dtacctToProcessArrangements'.
            if (validationOK)
            {

                int index = 0;
                //Select the account numbers where an arrangement amount greater than 0 has been entered
                //and place them into an array which will later be used to populate an 'Account Number'
                //combo box on the 'Special Arrangements' screen.
                //Also send the arrangement details for the account to the procedure
                //which will calculate the arrangement schedule.
                foreach (DataRow r in ArrangementListView.Table.Rows)
                {
                    if (Convert.ToDecimal(r[CN.ArrangementAmount]) > 0)
                    {
                        //Add the account numbers to an array.
                        arrAccts[index] = Convert.ToString(r[CN.AccountNo]);

                        arrangementSchedule = AccountManager.SPACalculateArrangementSchedule(r[CN.AccountNo].ToString(), Convert.ToChar(arrangementDet.Period), arrangementDet.ArrangementAmount, arrangementDet.NumInstal,
                                                                     Convert.ToDecimal(r[CN.ArrangementAmount]), Convert.ToDecimal(r[CN.OddPayment]), arrangementDet.FirstPayDate,0,0, out finalPayDate, out error);

                        index += 1;

                        if (index >= 1)
                        {
                            arrangementScheduleMerged.Merge(arrangementSchedule);
                            arrangementScheduleMerged.AcceptChanges();
                        }

                    }

                    //Set the finalPayDate on the table to the parameter returned from 'SPACalculateArrangementSchedule'.
                    r[CN.FinalPayDate] = finalPayDate.ToString();

                }
                //Set the datarow for 'Final Payment Date' here once been recieved.
            }
            //Initialise the public property to the private array.
            //The public property will be used to access the accounts numbers in the 'Special Arrangements'
            //screen to populate a combobox.
            ArrangementAccts = arrAccts;
            ArrangementListView.Table.AcceptChanges();
            
            //CombinedSPADetails.AcceptChanges();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            arrAccts = new string[1];
            arrAccts[0] = null;
            combinedSPADetailsDT.Rows.Clear();


        }

        private void ApplyTableStyleAndGridSource()
        {
            ArrangementListView = new DataView(combinedSPADetailsDT);
            ArrangementListView.AllowNew = false;
            ArrangementListView.Sort = CN.AccountNo + " ASC ";
            dgSPAList.CausesValidation = false;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = ArrangementListView.Table.TableName;

            AddColumnStyle(CN.AccountNo, tabStyle, 100, true, GetResource("T_ACCOUNTNO"), "");
            AddColumnStyle(CN.OutstandingBalance, tabStyle, 90, true, GetResource("T_OUTBAL"), DecimalPlaces);
            AddColumnStyle(CN.Arrears, tabStyle, 90, true, GetResource("T_ARREARS"), DecimalPlaces);
            AddColumnStyle(CN.InstalAmount, tabStyle, 90, true, GetResource("T_INSTALLMENT"), DecimalPlaces);
            AddColumnStyle(CN.ArrangementAmount, tabStyle, 90, false, GetResource("T_ARRANGEMENTAMOUNT"), DecimalPlaces);
            AddColumnStyle(CN.OddPayment, tabStyle, 90, false, GetResource("T_FINALARRANGEAMOUNT"), DecimalPlaces);
            AddColumnStyle(CN.DateLast, tabStyle, 90, true, GetResource("T_EXPSETTLEMENTDATE"), "");
            AddColumnStyle(CN.Period, tabStyle, 0, true, GetResource("T_SPAPERIOD"), "");
            AddColumnStyle(CN.NoOfIns, tabStyle, 0, true, GetResource("T_NOOFINS"), "");
            AddColumnStyle(CN.FirstPayDate, tabStyle, 0, true, GetResource("T_FIRSTPAYDATE"), "");
            AddColumnStyle(CN.FinalPayDate, tabStyle, 0, true, GetResource("T_FINPAYDATE"), "");
            AddColumnStyle(CN.ReasonCode, tabStyle, 0, true, GetResource("T_REASON"), "");
            AddColumnStyle(CN.ReviewDate, tabStyle, 0, true, GetResource("T_REVIEWDATE"), "");
            // Columns added jec 15/06/09
            AddColumnStyle(CN.FreezeIntAdmin, tabStyle, 0, true, GetResource("T_FRZINTADMIN"), "");
            AddColumnStyle(CN.ServiceCharge, tabStyle, 0, true, GetResource("T_SERVCHARGE"), "");
            AddColumnStyle(CN.ExtendTerm, tabStyle, 0, true, GetResource("T_EXTTERM"), "");
            AddColumnStyle(CN.NoOfRemainInstals, tabStyle, 0, true, GetResource("T_REMINST"), "");
            AddColumnStyle(CN.RemainInstalAmt, tabStyle, 0, true, GetResource("T_REMINSTAMT"), "");
            AddColumnStyle(CN.CustID, tabStyle, 0, true, GetResource("T_CUSTID"), "");
            AddColumnStyle(CN.AcctType, tabStyle, 0, true, GetResource("T_ACCOUNTTYPE"), "");
            AddColumnStyle(CN.TermsType, tabStyle, 0, true, GetResource("T_TERMSTYPE"), "");
            AddColumnStyle(CN.RefinDeposit, tabStyle, 0, true, GetResource("T_REFINDEP"), "");
            AddColumnStyle(CN.CashPrice, tabStyle, 0, true, GetResource("T_CASHPRICE"), "");
            AddColumnStyle(CN.DueDay, tabStyle, 0, true, GetResource("T_DUEDAY"), "");            

            //dgSPAList.DataSource = combinedSPADetailsDT;
            dgSPAList.DataSource = ArrangementListView;
            dgSPAList.TableStyles.Clear();
            dgSPAList.TableStyles.Add(tabStyle);


        }

       
    }
}
