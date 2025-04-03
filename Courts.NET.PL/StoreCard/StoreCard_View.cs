using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using STL.PL.Utils;
using STL.Common.Constants.StoreCard;
using Blue.Cosacs.Shared.Collections;
using Crownwood.Magic.Menus;
using STL.PL.StoreCard.Common;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using Blue.Cosacs.Shared.Services.Branch;


namespace STL.PL.StoreCard
{
    public partial class StoreCard_View : CommonForm
    {
        public delegate void InvokeDelegate();

        private Form ParentF;
        StoreCardCardReader CardReader = new StoreCardCardReader();

     
        public StoreCard_View(Form parent)
        {
            ParentF = parent;
            InitializeComponent();
            dtp_start.Value = DateTime.Now.AddDays(-7.0);
            var list = new List<StoreCardAccountStatus_Lookup>(StoreCardAccountStatus_Lookup.AsEnumerable());
            cmb_status.DataSource = list;
            cmb_status.DisplayMember = "Description";
            cmb_status.ValueMember = "Code";
            cmb_sourceSetup();
            populateBranch();
            SetupDGV();
        }

        private void cmb_sourceSetup()
        {
            var sourcelist = new List<StringPair>();
            sourcelist.Add(new StringPair("",""));
            sourcelist.Add(new StringPair(StoreCardSource.Preapproval, StoreCardSource.Preapproval));
            sourcelist.Add(new StringPair(StoreCardSource.NewAccount, StoreCardSource.NewAccount));
         
            cmb_source.DataSource = sourcelist;
            cmb_source.DisplayMember = "display";
            cmb_source.ValueMember = "value";
        }

        private void populateBranch()
        {
            Client.Call(new GetBranchListRequest() 
                           ,
                            response =>
                            {
                                var list = response.BranchList;
                                list.Insert(0,"");
                                cmb_branch.DataSource = list;
                            }, 
                            this);
        }
        

        private void SetupDGV()
        {
            dgv_Results
                .ColumnStyleInit()
                .ColumnStylePreLoad("FirstName")
                .ColumnStylePreLoad("LastName")
                .ColumnStylePreLoad("AccountNo")
                .ColumnStylePreLoad("DateAcctOpen")
                .ColumnStylePreLoad("CardDisplay")
                .ColumnStylePreLoad("Holder")
                .ColumnStylePreLoad("CreditBlockedYN","Credit Blocked")
                //.ColumnStylePreLoad("ActivatedOn")
                .ColumnStylePreLoad("StoreCardLimit", align: DataGridViewContentAlignment.MiddleRight, DefaultCellStyleFormat: "0.00")
                .ColumnStylePreLoad("Balance", align: DataGridViewContentAlignment.MiddleRight, DefaultCellStyleFormat: "0.00")
                .ColumnStylePreLoad("StatusName")
                .ColumnStylePreLoad("Source")
                .ColumnStylePreLoad("Address1")
                .ColumnStylePreLoad("Address2")
                .ColumnStylePreLoad("Address3")
                .ColumnStylePreLoad("PostCode")
                .ColumnStylePreLoad("HomeTelNo", align: DataGridViewContentAlignment.MiddleRight)
                .ColumnStylePreLoad("WorkTelNo", align: DataGridViewContentAlignment.MiddleRight)
                .ColumnStylePreLoad("MobileTelNo", align: DataGridViewContentAlignment.MiddleRight);

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            dtp_end.Value = DateTime.Now;
            dtp_start.Value = DateTime.Now.AddDays(-7.0);
            mtb_acctno.Text = "";
            mtb_storecardno.Text = "";
            txt_surname.Text = "";
            cmb_source.SelectedIndex = 0;
            cmb_status.SelectedIndex = 0;
            dgv_Results.DataSource = null;
            cmb_branch.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            Client.Call(new SearchRequest
            {
                AcctNo = CheckBlank(mtb_acctno.Text.Replace("-", "")),
                StartDate = dtp_start.Value.Date,
                EndDate = dtp_end.Value.Date.AddDays(1).AddSeconds(-1),
                Source = CheckBlank(cmb_source.SelectedValue == null ? "" : cmb_source.SelectedValue.ToString()),
                Status = CheckBlank(cmb_status.SelectedValue == null ? "" : cmb_status.SelectedValue.ToString().ToUpper()),
                StoreCardNo = long.Parse(mtb_storecardno.Text.Replace("-", "").Length == 16 ? mtb_storecardno.Text.Replace("-", "") : "0"),
                LastName = CheckBlank(txt_surname.Text),
                Branch = cmb_branch.Text == string.Empty ? null : cmb_branch.Text,
                Holder = checkHolder.Checked
            }, response =>
            {
                    dgv_Results.DataSource = SortableBindingListFactory.Create(response.View_StoreCard);
                    MainForm.Current.ShowStatus(response.View_StoreCard.Count.ToString() + " results returned. - Max results limited to 200.");
            },this);
        }

        private string CheckBlank(string text)
        { 
           return text.Trim().Length != 0 ? text : null;
        }

        private void dgv_Results_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var index = dgv_Results.CurrentCell.RowIndex;
              //  if (dgv_Transactions.SelectedCells.Count > 0)
            //{
            //    Client.Call(new GetDeliveryDetailsRequest
            //    {
            //        AcctNo = Result.Fintransfers[dgv_Transactions.SelectedCells[0].RowIndex].TransferAccount,
                if (index != -1)
                {
                    var source = ((SortableBindingList<View_StoreCard>)dgv_Results.DataSource);
                    PopupMenu popup = new PopupMenu();

                    MenuCommand ViewDetails = new MenuCommand() { Text = "View StoreCard Details" };
                    ViewDetails.Click += new EventHandler(ViewDetails_Click);
                    popup.MenuCommands.Add(ViewDetails);

                    if (StoreCardAccountStatus_Lookup.Suspended.Equals(source[index].Status))
                    {
                        MenuCommand SetAct = new MenuCommand() { Text = "Set Account to Awaiting Activation" };
                        SetAct.Click += new EventHandler(SetAct_Click);
                        popup.MenuCommands.Add(SetAct);
                    }
                    
                    if (!StoreCardAccountStatus_Lookup.Cancelled.Equals(source[index].Status))
                    {
                        MenuCommand CancelAccount = new MenuCommand() { Text = "Cancel StoreCard Account" };
                        CancelAccount.Click += new EventHandler(CancelAccount_Click);
                        popup.MenuCommands.Add(CancelAccount);
                    }

                    MenuCommand selected;
                    if (popup.MenuCommands.Count > 0)
                        selected = popup.TrackPopup(((DataGridView)sender).PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void SetAct_Click(object sender, EventArgs e)
        { 
                bool? success = StoreCardManager.SetAwaitingActivation(dgv_Results.CurrentRow.Cells["AccountNo"].Value.ToString());
                if (success.HasValue && success.Value)
                {
                    ((MainForm)ParentF).ShowStatus("Account status changed successfully.");
                    var storecardaccount = new StoreCardAccount(dgv_Results.CurrentRow.Cells["AccountNo"].Value.ToString(), ParentF, this, (int)StoreCardCommon.StoreCardAccountTabs.Activation);
                    ((MainForm)ParentF).AddTabPage(storecardaccount);
                }
                else
                {
                    MessageBox.Show("The customer failed to qualify for storecard. Can not reactivate.", "Account Failed to Requalify.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
        }

        private void ViewDetails_Click(object sender, EventArgs e)
        {
            var storecardaccount = new StoreCardAccount(dgv_Results.CurrentRow.Cells["AccountNo"].Value.ToString(), ParentF, this, (int)StoreCardCommon.StoreCardAccountTabs.Activation);
            ((MainForm)ParentF).AddTabPage(storecardaccount);
        }

        private void CancelAccount_Click(object sender, EventArgs e)
        {
            var Cancelaccount = new CancelAccount(dgv_Results.CurrentRow.Cells["AccountNo"].Value.ToString(), ParentF, this);
            Cancelaccount.ShowDialog();
            dgv_Results.Rows.RemoveAt(dgv_Results.SelectedRows[0].Index);
        }


        public void Setmtb_storecardno(string cardno)
        {
            mtb_storecardno.Text = cardno;
        }

        private void StoreCard_View_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            var cardno = CardReader.ReadKey(e.KeyChar);

            if (CardReader.capture)
            {
                e.Handled = true;
            }

            if (!String.IsNullOrEmpty(cardno))
            {
                Setmtb_storecardno(cardno);
            }
        }
    }

    public class StringPair
    {
        public string display { get; set; }
        public string value { get; set; }

        public StringPair (string d, string v)
        {
            display = d;
            value = v;
        }
    }

   

}
