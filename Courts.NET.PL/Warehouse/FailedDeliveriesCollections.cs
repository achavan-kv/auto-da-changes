using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Warehouse;
using Crownwood.Magic.Menus;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL.Warehouse
{
    public partial class FailedDeliveriesCollections : CommonForm
    {
        private int index = 0;
        private int salesperson = 0;        // #10618 - add Salesperson dropdown
        private StringCollection salesStaff = null;
 
        public FailedDeliveriesCollections(Form root, Form parent)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            //HashMenus();

            InitialiseStaticData();
            LoadFailures();
        }

        //private void HashMenus()
        //{
        //    dynamicMenus = new Hashtable();
        //    dynamicMenus[this.Name + ":drpSalesperson"] = this.drpSalesperson;
        //    dynamicMenus[this.Name + ":drpBranch"] = this.drpBranch;      
        //}

        private int? SelectedBranch()
        {
            if (drpBranch.SelectedIndex< 0)
                return int.Parse(Config.BranchCode);
            var v = (string)drpBranch.Items[drpBranch.SelectedIndex];
            if (v == "ALL")
                return null;
            return int.Parse(v);
        }

        private void InitialiseStaticData()
        {
            Function = "BStaticDataManager::GetDropDownData";
            try
            {
                Wait();                

                drpBranch.Enabled = Credential.HasPermission(CosacsPermissionEnum.FailedDeliveriesAllCSR);          // #12230
                drpSalesperson.Enabled = Credential.HasPermission(CosacsPermissionEnum.FailedDeliveriesAllCSR);       // #12230

                if (drpBranch.Enabled)
                {
                    var xml = new XmlUtilities();
                    var dropDowns = new XmlDocument();
                    dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                    if (StaticData.Tables[TN.BranchNumber] == null)
                        dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                    var branchDT = (DataTable)StaticData.Tables[TN.BranchNumber];

                    drpBranch.Items.Add("ALL");
                    foreach (DataRow row in branchDT.Rows)
                        drpBranch.Items.Add(row[CN.BranchNo].ToString());
                    drpBranch.SelectedIndex = 0;
                }
                else
                {
                    drpBranch.Items.Add(Config.BranchCode);
                    drpBranch.SelectedIndex = 0;
                }

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

        private void LoadFailures()
        {
            Client.Call(new GetLineItemBookingFailuresRequest
            {
                Branch = SelectedBranch(),
                Salesperson = salesperson       // #10618 - add Salesperson dropdown
            },
                response =>
                {
                    PopulateScreen(response);
                },
                this); 
        }

        private void PopulateScreen(GetLineItemBookingFailuresResponse failures)
        {
            dgDeliveryFails.DataSource = failures.Failures;

            dgDeliveryFails.ColumnHeadersVisible = true;
            dgDeliveryFails.AutoGenerateColumns = true;
            dgDeliveryFails.Columns["id"].Visible=false;
            dgDeliveryFails.Columns[CN.BookingID].Width = 50;
            dgDeliveryFails.Columns[CN.BookingID].HeaderText= "Booking Id";
            dgDeliveryFails.Columns["OriginalBookingID"].HeaderText = "Original Booking Id";
            dgDeliveryFails.Columns["OriginalBookingID"].Width = 50;
            dgDeliveryFails.Columns[CN.FailedQty].HeaderText = "Failed Qty";
            dgDeliveryFails.Columns[CN.FailedQty].Width = 50;
            dgDeliveryFails.Columns[CN.FailedQty].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; 
            dgDeliveryFails.Columns[CN.acctno].HeaderText = GetResource("T_ACCTNO");
            dgDeliveryFails.Columns[CN.acctno].Width = 80;
            dgDeliveryFails.Columns[CN.OrderedQuantity].HeaderText = "Ordered Quantity";
            dgDeliveryFails.Columns[CN.OrderedQuantity].Width = 50;
            dgDeliveryFails.Columns[CN.OrderedQuantity].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgDeliveryFails.Columns[CN.ItemDescr1].HeaderText = GetResource("T_ITEM_DESCRIPTION");
            dgDeliveryFails.Columns[CN.ItemDescr1].Width = 150;
            dgDeliveryFails.Columns[CN.ItemDescr2].HeaderText = GetResource("T_ITEM_DESCRIPTION2");
            dgDeliveryFails.Columns[CN.ItemDescr2].Width = 150;
            dgDeliveryFails.Columns[CN.CustID].HeaderText = GetResource("T_CUSTID");
            dgDeliveryFails.Columns["SalesBrnNo"].HeaderText = "Sales Branch No.";
            dgDeliveryFails.Columns["SalesBrnNo"].Width = 50;
            dgDeliveryFails.Columns["LineItemID"].Visible = false;
            dgDeliveryFails.Columns["Actioned"].Visible = false;
            dgDeliveryFails.Columns["RetItemId"].Visible = false;
            dgDeliveryFails.Columns["RetVal"].Visible = false;
            dgDeliveryFails.Columns["RetStockLocn"].Visible = false;

            dgDeliveryFails.Select();  //IP - 15/06/12 - #10381
         

            ((MainForm)FormRoot).StatusBarText = Convert.ToString(dgDeliveryFails.RowCount) + " failures loaded"; 
                  
        }

        private void dgDeliveryFails_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgDeliveryFails.RowCount > 0)
            {
                index = dgDeliveryFails.CurrentRow.Index;

                if (e.Button == MouseButtons.Right)
                {

                    DataGridView ctl = (DataGridView)sender;

                    MenuCommand m1 = new MenuCommand(GetResource("P_REVISE_ORDER"));
                    MenuCommand m2 = new MenuCommand(GetResource("P_CUSTDETAILS"));
                   // #14602 MenuCommand m3 = new MenuCommand(GetResource("P_CHANGE_ORDER_DETAILS"));
                    MenuCommand m4 = new MenuCommand(GetResource("P_SUBMIT_BOOKING"));
                    MenuCommand m5 = new MenuCommand(GetResource("P_ADDFAILURE_NOTES"));  //#10358
                    MenuCommand m6 = new MenuCommand(GetResource("P_CANCEL_COLLECTION_NOTE"));  //#13644
                    MenuCommand m7 = new MenuCommand(GetResource("P_CANCEL_REDELIVERY"));  //#13675

                    m1.Click += new System.EventHandler(this.ReviseOrder_Click);
                    m2.Click += new System.EventHandler(this.CustDetails_Click);
                    // #14602 m3.Click += new System.EventHandler(this.ChangeDetails_Click);
                    m4.Click += new System.EventHandler(this.SubmitOrder_Click);
                    m5.Click += new System.EventHandler(this.AddNotes_Click); //#10358
                    m6.Click += new System.EventHandler(this.CancelCollectionNotes_Click); //#13644
                    m7.Click += new System.EventHandler(this.CancelRedelivery_Click); //#13675

                    PopupMenu popup = new PopupMenu();
                    popup.Animate = Animate.Yes;
                    popup.AnimateStyle = Animation.SlideHorVerPositive;
                    // #14602 popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, m3, m4, m5 }); //#10358
                    popup.MenuCommands.AddRange(new MenuCommand[] { m1, m2, m4, m5 });

                    if (Convert.ToString(dgDeliveryFails.Rows[index].Cells[CN.BookingType].Value) == "Collection")          //#13644
                    {
                        // #14602 popup.MenuCommands.Remove(m3);
                        popup.MenuCommands.Add(m6);
                        popup.MenuCommands.Remove(m1);      // #14636 
                    }

                    if (Convert.ToString(dgDeliveryFails.Rows[index].Cells[CN.BookingType].Value) == "Re-Delivery")         //#13675  
                    {
                        // #14602 popup.MenuCommands.Remove(m3);
                        popup.MenuCommands.Add(m7);
                    }

                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadFailures();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void ReviseOrder_Click(object sender, EventArgs e)
        {
            var x = dgDeliveryFails.Rows[index].Cells[CN.acctno].Value.ToString();

            //#18125 Mark booking as actioned
            var bookingId = Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["OriginalBookingId"].Value);
            AccountManager.ReviseBookingFailure(bookingId, out Error);
            btnReload_Click(null, null);

            NewAccount reviseAcct = new NewAccount(dgDeliveryFails.Rows[index].Cells[CN.acctno].Value.ToString(), 1, null, false, FormRoot, this);
            reviseAcct.Text = "Revise Sales Order";
            if (reviseAcct.AccountLocked && reviseAcct.AccountLoaded)
            {
                ((MainForm)this.FormRoot).AddTabPage(reviseAcct, 6);
                reviseAcct.SupressEvents = false;
            }
        }

        private void SubmitOrder_Click(object sender, EventArgs e)
        {
            bool actioned = AccountManager.SubmitBookingFailure(Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["ID"].Value.ToString()),
                                Convert.ToInt32(dgDeliveryFails.Rows[index].Cells[CN.FailedQty].Value.ToString()), 
                                Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["LineItemID"].Value.ToString()), Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["RetItemId"].Value.ToString()), 
                                Convert.ToDecimal(dgDeliveryFails.Rows[index].Cells["RetVal"].Value), Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["RetStockLocn"].Value),
                                Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["OriginalBookingId"].Value), out Error);   //#13604
            if (actioned)
            {
                ((MainForm)FormRoot).StatusBarText = "Shipment has already been submitted - reload screen to show outstanding failures";
            }
            else
            {
                ((MainForm)FormRoot).StatusBarText = "Shipment submitted";
            }
        }

        private void CustDetails_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "CustDetails_Click";
                
                if (index >= 0)
                {
                        BasicCustomerDetails cust = new BasicCustomerDetails(false, dgDeliveryFails.Rows[index].Cells[CN.CustID].Value.ToString(), 
                                dgDeliveryFails.Rows[index].Cells[CN.acctno].Value.ToString(), "H","", FormRoot, null);
                        cust.FormRoot = this.FormRoot;
                        cust.FormParent = null;
                        cust.SanctionScreen = null;
                        ((MainForm)FormRoot).AddTabPage(cust, 10);
                        cust.loaded = true;

                        cust.txtCustID.Text = dgDeliveryFails.Rows[index].Cells[CN.CustID].Value.ToString();
                        cust.txtCustID_Leave(this, null);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void ChangeDetails_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "ChangeDetails_Click";

                if (index >= 0)
                {
                    ChangeItemLocation change = new ChangeItemLocation(FormRoot, FormParent);
                    change.FormRoot = this.FormRoot;
                    change.FormParent = null;

                    ((MainForm)FormRoot).AddTabPage(change, 10);
                    change.txtAccountNo.Text = dgDeliveryFails.Rows[index].Cells[CN.acctno].Value.ToString();
                    change.txtAccountNo_Leave(this, null);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //#13644
        private void CancelCollectionNotes_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "CancelCollectionNotes_Click";

                if (index >= 0)
                {
                    CancelCollectionNotes cancelNotes = new CancelCollectionNotes(FormRoot, FormParent);
                    cancelNotes.FormRoot = this.FormRoot;
                    cancelNotes.FormParent = null;                    

                    ((MainForm)FormRoot).AddTabPage(cancelNotes, 10);
                    cancelNotes.txtAccountNo.Text = dgDeliveryFails.Rows[index].Cells[CN.acctno].Value.ToString();
                    cancelNotes.btnLoad_Click(this, null);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //#13675
        private void CancelRedelivery_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "CancelRedelivery_Click";

                if (index >= 0)
                {
                    AccountManager.CancelRedelivery(Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["OriginalBookingID"].Value), Convert.ToInt32(dgDeliveryFails.Rows[index].Cells["BookingID"].Value));
                   ((MainForm)FormRoot).StatusBarText = "Reload screen to show outstanding failures";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

        }

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            drpSalesperson.DataSource = null;
            salesStaff = new StringCollection();

            // #10618 - add Salesperson dropdown
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.SalesStaff, new string[] { Convert.ToString(SelectedBranch() ??  int.Parse(Config.BranchCode)), "S" }));
            DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);            

            DataRow newRow = ds.Tables[0].NewRow();       
            newRow["EmployeeName"] = "All Salespersons";
            newRow["EmpeeNo"] = "0";
            ds.Tables[0].Rows.InsertAt(newRow, 0);

            drpSalesperson.DataSource = ds.Tables[0];
            drpSalesperson.ValueMember = "EmpeeNo";
            drpSalesperson.DisplayMember = "EmployeeName";

            int i = drpSalesperson.FindString(Credential.UserId.ToString() + " : " + Credential.Name);
            if (i != -1)
            {
                drpSalesperson.SelectedIndex = i;
                salesperson = Convert.ToInt32(drpSalesperson.SelectedValue);        // #12230
            }
            else drpSalesperson.SelectedIndex = 0;

            //LoadFailures();
        }

        //#10358
        private void AddNotes_Click(object sender, EventArgs e)
        {
            var acctNo = dgDeliveryFails.Rows[index].Cells[CN.acctno].Value.ToString();
            FailureNotes notes = new FailureNotes(acctNo, this, this.FormRoot);
            notes.ShowDialog();
        }

        private void drpSalesperson_SelectedIndexChanged(object sender, EventArgs e)
        {       // #10618 - add Salesperson dropdown
            salesperson = Convert.ToInt32(drpSalesperson.SelectedValue);
            LoadFailures();
        }
    }
}
