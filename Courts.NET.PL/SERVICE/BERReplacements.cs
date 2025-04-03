using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common;
using System.Xml;
using STL.Common.Static;
using System.Collections.Specialized;
using STL.Common.Constants.TableNames;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Service;
using STL.Common.Constants.ColumnNames;
using Crownwood.Magic.Menus;

namespace STL.PL.SERVICE
{
    public partial class BERReplacements : CommonForm
    {
        private int branch = 0;
        private bool loaded = false;
        private int index { get; set; }
        private string acctNo;
        private int serviceRequestNo;

        private string AcctNo
        {
            get
            {
                if (dgItems.CurrentRow.Index >= 0)
                {
                    acctNo = dgItems.Rows[index].Cells["AccountNumber"].Value.ToString();
                }
                return acctNo;
            }

            set { }

        }

        private int RequestNo
        {
            get
            {
                if (dgItems.CurrentRow.Index >= 0)
                {
                    serviceRequestNo = Convert.ToInt32(dgItems.Rows[index].Cells["SRNo"].Value);
                }

                return serviceRequestNo;
            }

            set { }
        }

        public BERReplacements(Form root, Form parent)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            InitialiseStaticData();

            var x = drpBranch.FindString(branch.ToString());
            if (x != -1)
                drpBranch.SelectedIndex = x;

            loaded = true;
            LoadReplacementsDue();
        }

        private void InitialiseStaticData()
        {
            Function = "BStaticDataManager::GetDropDownData";
            try
            {
                Wait();

                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                drpBranch.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
                drpBranch.DisplayMember = "branchno";
                drpBranch.ValueMember = "branchno";

                branch = Convert.ToInt32(Config.BranchCode);

                if (branch == (decimal)Country[CountryParameterNames.HOBranchNo])
                {
                    drpBranch.DataSource = (DataTable)StaticData.Tables[TN.BranchNumber];
                    drpBranch.Enabled = true;
                }
                else
                {
                    drpBranch.Enabled = false;
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

        private void LoadReplacementsDue()
        {
            Client.Call(new GetItemsIssuedForReplacementRequest
            {
                Branch = branch
            },
                response =>
                {
                    PopulateScreen(response);
                },
                this);
        }

        private void PopulateScreen(GetItemsIssuedForReplacementResponse replacementsDue)
        {
            dgItems.DataSource = replacementsDue.itemsForReplacement;

            ((MainForm)FormRoot).StatusBarText = Convert.ToString(dgItems.RowCount) + " items loaded";

        }

        private void drpBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpBranch.Enabled == true)
            {
                branch = Convert.ToInt32(((DataRowView)drpBranch.SelectedItem)[CN.BranchNo]);
            }
            else
            {
                branch = Convert.ToInt32(Config.BranchCode);
            }

            if (loaded)
            {
                LoadReplacementsDue();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            CloseTab();
        }

        private void dgItems_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgItems.CurrentRow != null)
            {
                index = dgItems.CurrentRow.Index;

                if (index >= 0)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        PopupMenu popup = new PopupMenu();

                        DataGridView ctl = (DataGridView)sender;

                        MenuCommand accountDet = new MenuCommand("Account Details");
                        MenuCommand goodsRet = new MenuCommand("Goods Return");

                        accountDet.Click += new System.EventHandler(this.LoadAccountDet);
                        goodsRet.Click += new System.EventHandler(this.LoadGoodsRet);

                        popup.MenuCommands.Add(accountDet);
                        popup.MenuCommands.Add(goodsRet);

                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                }
            }
        }

        private void LoadAccountDet(object sender, System.EventArgs e)
        {
            AccountDetails details = new AccountDetails(AcctNo.Replace("-", ""), FormRoot, this);
            ((MainForm)this.FormRoot).AddTabPage(details, 7);
        }

        private void LoadGoodsRet(object sender, System.EventArgs e)
        {
            GoodsReturn goodsRet = new GoodsReturn(FormRoot, this, AcctNo.Replace("-", ""), RequestNo);
            ((MainForm)this.FormRoot).AddTabPage(goodsRet, 7);
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            this.statusBar.Text = "";

            LoadReplacementsDue();
        }
    }
}
