using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using STL.Common.Constants.AccountTypes;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using Crownwood.Magic.Menus;

namespace STL.PL
{
    public partial class InstantReplacement : CommonForm
    {
        bool staticLoaded = false;
        private bool valid = true;
        private string err = "";
        private int index = 0;

        public InstantReplacement(Form root, Form parent)
        {
            InitializeComponent();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

            FormRoot = root;
            FormParent = parent;

            dtStart.MaxDate = DateTime.Now;
            dtEnd.MaxDate = DateTime.Now;
            dtStart.Value = DateTime.Now;
            dtEnd.Value = DateTime.Now;

            StringCollection accountTypes = new StringCollection();
            accountTypes.Add("Type");
            accountTypes.Add(AT.ReadyFinance);
            accountTypes.Add(AT.Cash);
            accountTypes.Add(AT.Special);

            drpAccountType.DataSource = accountTypes;

            staticLoaded = true;
        }

        private void drpAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (staticLoaded)
            {
                btnSearch.Enabled = !(drpAccountType.Text == "Type");
                
                txtAccountNo.Enabled = !(drpAccountType.Text == AT.Special);
                txtCustID.Enabled = !(drpAccountType.Text == AT.Special);
                txtInvoiceNum.Enabled = (drpAccountType.Text == AT.Special);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Function = "btnSearch_Click";

            try
            {
                Wait();

                if (Valid())
                {
                    // Clear the data grid
                    dgAccounts.DataSource = null;

                    ((MainForm)this.FormRoot).statusBar1.Text = "Searching for accounts";

                    string acctNo = txtAccountNo.UnformattedText;
                    if (acctNo == "000000000000")
                        acctNo = "%";
                    else
                        acctNo += '%';

                    int buffno = 0;
                    if (this.txtInvoiceNum.Text.Length > 0)
                        buffno = Convert.ToInt32(this.txtInvoiceNum.Text);

                    string custID = txtCustID.Text + "%";

                    DataSet ds = AccountManager.GetIRItems(acctNo, custID, buffno, 
                                                           dtStart.Value, dtEnd.Value, 
                                                           drpAccountType.Text, out err);
                    if (err.Length > 0)
                        ShowError(err);
                    else
                    {
                        if (ds != null)
                        {
                            ((MainForm)this.FormRoot).statusBar1.Text = ds.Tables[0].Rows.Count + " Row(s) returned.";

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ds.Tables[0].DefaultView.AllowNew = false;
                                dgAccounts.DataSource = ds.Tables[0].DefaultView;

                                if (dgAccounts.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = ds.Tables[0].TableName;
                                    dgAccounts.TableStyles.Add(tabStyle);

                                    /* set up the header text */
                                    tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DELIVERYDATE");
                                    tabStyle.GridColumnStyles[CN.AcctNo].HeaderText = GetResource("T_ACCTNO");
                                    tabStyle.GridColumnStyles[CN.CustID].HeaderText = GetResource("T_CUSTID");
                                    tabStyle.GridColumnStyles[CN.WarrantyNo].HeaderText = GetResource("T_WARRANTY_NO");
                                    tabStyle.GridColumnStyles[CN.ContractNo].HeaderText = GetResource("T_CONTRACTNO");
                                    tabStyle.GridColumnStyles[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
                                    tabStyle.GridColumnStyles[CN.ItemDescr1].HeaderText = GetResource("T_ITEM_DESCRIPTION");
                                    tabStyle.GridColumnStyles[CN.BuffNo].HeaderText = GetResource("T_INVOICE_NUMBER");

                                    /* set up the column widths */
                                    tabStyle.GridColumnStyles[CN.DateTrans].Width = 80;
                                    tabStyle.GridColumnStyles[CN.AcctNo].Width = 85;
                                    tabStyle.GridColumnStyles[CN.CustID].Width = 90;
                                    tabStyle.GridColumnStyles[CN.WarrantyNo].Width = 75;
                                    tabStyle.GridColumnStyles[CN.ContractNo].Width = 95;
                                    tabStyle.GridColumnStyles[CN.ItemNo].Width = 75;
                                    tabStyle.GridColumnStyles[CN.ItemDescr1].Width = 285;
                                    tabStyle.GridColumnStyles[CN.BuffNo].Width = 90;

                                    tabStyle.GridColumnStyles[CN.EmpeeNoSale].Width = 0;
                                    tabStyle.GridColumnStyles[CN.TaxRate].Width = 0;
                                    tabStyle.GridColumnStyles[CN.StockLocn].Width = 0;
                                    tabStyle.GridColumnStyles[CN.TransValue].Width = 0;
                                    tabStyle.GridColumnStyles[CN.Discount].Width = 0;
                                    tabStyle.GridColumnStyles[CN.Quantity].Width = 0;
                                    tabStyle.GridColumnStyles[CN.Price].Width = 0;
                                    tabStyle.GridColumnStyles[CN.ItemID].Width = 0;                         //IP - 28/07/11 - RI - #4429
                                    tabStyle.GridColumnStyles[CN.WarrantyId].Width = 0;                     //IP - 28/07/11 - RI - #4429

                                    //IP - 01/08/11 - RI - #4445
                                    if (Convert.ToBoolean(Country[CountryParameterNames.RIDispCourtsCode]))
                                    {
                                        tabStyle.GridColumnStyles[CN.Courts_Code].Width = 100;
                                        tabStyle.GridColumnStyles[CN.Warranty_Courts_Code].Width = 100;
                                    }
                                    else
                                    {
                                        tabStyle.GridColumnStyles[CN.Courts_Code].Width = 0;
                                        tabStyle.GridColumnStyles[CN.Warranty_Courts_Code].Width = 0;
                                    }

                                    tabStyle.GridColumnStyles[CN.ContractNo].NullText = "";
                                }
                            }
                        }
                        else
                        {
                            ((MainForm)this.FormRoot).statusBar1.Text = "0 Row(s) returned.";
                        }
                    }
                }
                Function = "End of btnSearch_Click";
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

        private void txtInvoiceNum_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (!IsStrictNumeric(txtInvoiceNum.Text))
                {
                    valid = false;
                    errorProvider1.SetError(txtInvoiceNum, GetResource("M_ENTERMANDATORY"));
                }
                else
                {
                    valid = valid ? true : false;
                    errorProvider1.SetError(txtInvoiceNum, "");
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "txtInvoiceNum_Validating");
            }
        }

        private bool Valid()
        {
            valid = true;

            if (drpAccountType.Text != "S")
            {
                if (txtAccountNo.UnformattedText == "000000000000" && txtCustID.Text.Length == 0)
                {
                    valid = false;
                    errorProvider1.SetError(txtAccountNo, GetResource("M_ENTERMANDATORY"));
                }
                else
                {
                    valid = true;
                    errorProvider1.SetError(txtAccountNo, "");
                }
            }

            if(valid)
                txtInvoiceNum_Validating(null, null);
            
            return valid;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtAccountNo.Text = "000000000000";
            txtCustID.Text = "";
            txtInvoiceNum.Text = "0";
            dtStart.Value = dtStart.MaxDate;
            dtEnd.Value = dtEnd.MaxDate;
            txtAccountNo.Enabled = false;
            txtCustID.Enabled = false;
            txtInvoiceNum.Enabled = false;
            btnSearch.Enabled = false;
            staticLoaded = false;
            drpAccountType.SelectedIndex = 0;
            dgAccounts.DataSource = null;
            staticLoaded = true;
        }

        private void dgAccounts_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                Wait();

                index = dgAccounts.CurrentRowIndex;

                if (index >= 0)
                {
                    dgAccounts.Select(dgAccounts.CurrentCell.RowNumber);

                    if (e.Button == MouseButtons.Right)
                    {
                        DataGrid ctl = (DataGrid)sender;
                        DataView view = (DataView)dgAccounts.DataSource;

                        MenuCommand m1 = new MenuCommand(GetResource("P_WARRANTY_REPLACE"));
                        m1.Click += new System.EventHandler(this.OnWarrantyReplace);

                        PopupMenu popup = new PopupMenu();
                        //if (menuInstantReplacement.Enabled)
                        popup.MenuCommands.Add(m1);

                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgAccounts_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnWarrantyReplace(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                DataView dv = (DataView)dgAccounts.DataSource;
                string acctNo = (string)dv[index][CN.AcctNo];
                string productCode = (string)dv[index][CN.ItemNo];
                string model = (string)dv[index][CN.ItemNo];
                short stockLocn = Convert.ToInt16(dv[index][CN.StockLocn]);
                string productDescription = (string)dv[index][CN.ItemDescr1];
                DateTime dateTrans = (DateTime)dv[index][CN.DateTrans];
                int buffNo = (int)dv[index][CN.BuffNo];
                decimal price = Convert.ToDecimal(dv[index][CN.Price]);
                decimal quantity = Convert.ToDecimal(dv[index][CN.Quantity]);
                decimal orderValue = Convert.ToDecimal(dv[index][CN.TransValue]);
                string warrantyNo = (string)dv[index][CN.WarrantyNo];
                decimal taxRate = Convert.ToDecimal(dv[index][CN.TaxRate]);
                string contractno = (string)dv[index][CN.ContractNo];
                int empeeNoSale = (int)dv[index][CN.EmpeeNoSale];
                int itemId = (int)dv[index]["ItemId"];
                int warrantyID = (int)dv[index]["WarrantyId"];                                                  //IP - 29/07/11 - #4429

                OneForOneReplacement o = new OneForOneReplacement(acctNo, productCode,
                                                                  model, productDescription,
                                                                  dateTrans, stockLocn.ToString(),
                                                                  buffNo, price, quantity,
                                                                  orderValue, warrantyNo,
                                                                  taxRate, FormRoot, this,
                                                                  contractno, empeeNoSale,
                                                                  itemId, warrantyID);                          //IP - 29/07/11 - #4429
                o.ShowDialog();
                RemoveLine();
            }
            catch (Exception ex)
            {
                Catch(ex, "OnWarrantyReplace");
            }
            finally
            {
                StopWait();
            }
        }

        private void RemoveLine()
        {
            /* remove the line given by the curernt value of index from the dg */
            ((DataView)dgAccounts.DataSource).Delete(index);
        }
    }
}