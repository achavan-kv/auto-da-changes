using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.PrivilegeClub;
using STL.Common.Static;

namespace STL.PL
{
    public partial class TermsTypeMatrixPopup : CommonForm
    {
        private string _error = "";
        //private bool _userChanged;// = false;
        private DataView _termsList = null;

        private string _termsType = "";
        public string termsType
        {
            get { return _termsType; }
        }

        private string _band = "";
        public string band
        {
            get { return _band; }
        }

        private string _loyaltyBand = "";
        private bool _affinityTerms = false;
        private string _acctType = "";
        private DataTable _overViewTable = null;
        private DataTable _filteredOverViewTable = null;

        //CR903 Include a public property to hold the selected branch's store type
        private string m_storeType = String.Empty;
        public string SType
        {
            get
            {
                return m_storeType;
            }
            set
            {
                m_storeType = value;
            }
        }
        private int _user;

        public int Authoriser
        {
            get { return _user; }
            set { _user = value; }
        }

        private string CustomerId { get; set; }
        //CR906 Include a public property to hold if the account is a loan account
        private bool _isLoan;
        public bool IsLoan
        {
            get { return _isLoan; }
            set { _isLoan = value; }
        }

        private string AccountNo { get; set; }

        public TermsTypeMatrixPopup(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;
        }

        public TermsTypeMatrixPopup(Form root, Form parent,
            DataView termsList, string termsType,
            string band, string acctNo,
            string customerPClubCode, string privilegeClubDesc,
            bool affinityTerms, string acctType, string storeType, bool isLoan, string custid)
        {
            InitializeComponent();
            AccountNo = acctNo;
            CustomerId = custid;
            HashMenus();
            ApplyRoleRestrictions();
            btnAuthorise.Visible = true;
            if (btnAuthorise.Enabled) this.txtUser.Text = Credential.UserId.ToString();

            FormRoot = root;
            FormParent = parent;
            this._termsList = termsList;
            this._termsType = termsType;
            this._band = band;
            this.Text += "  " + acctNo;
            this._loyaltyBand = customerPClubCode;
            this.lLoyaltyTitle.Text = privilegeClubDesc;
            this._affinityTerms = affinityTerms;
            this._acctType = acctType;
            SType = storeType;
            IsLoan = isLoan;
        }


        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":" + this.btnAuthorise.Name] = this.btnAuthorise;
        }


        private void LoadStatic()
        {
            //this._userChanged = false;

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.TermsTypeBand] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.TermsTypeBand, new string[] { Config.CountryCode }));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out _error);
                if (_error.Length > 0)
                    ShowError(_error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        StaticData.Tables[dt.TableName] = dt;
                    }
                }
            }

            // List the available scoring bands
            this.drpBand.DataSource = (DataTable)StaticData.Tables[TN.TermsTypeBand];
            drpBand.DisplayMember = CN.Band;
            drpBand.ValueMember = CN.Band;

            // Add a loyalty club band if required
            if (this._loyaltyBand == PCCustCodes.Tier1 || this._loyaltyBand == PCCustCodes.Tier2)
            {
                // Add the loyalty band
                DataTable bandTable = ((DataTable)StaticData.Tables[TN.TermsTypeBand]).Copy();
                DataRow newBand = bandTable.NewRow();
                newBand[CN.Band] = this._loyaltyBand;
                bandTable.Rows.InsertAt(newBand, 0);
                this.drpBand.DataSource = bandTable;
            }

            // Set the drop down to the current band
            if (this._band.Trim().Length == 0)
                this._band = Convert.ToString(Country[CountryParameterNames.TermsTypeBandDefault]);
            int index = drpBand.FindString(this._band);

            // Check for an old band that is missing
            if (index == -1)
            {
                // Add the current band
                DataTable bandTable = ((DataTable)StaticData.Tables[TN.TermsTypeBand]).Copy();
                DataRow newBand = bandTable.NewRow();
                newBand[CN.Band] = this._band;
                bandTable.Rows.InsertAt(newBand, 0);
                this.drpBand.DataSource = bandTable;
                index = 0;
            }
            drpBand.SelectedIndex = index;

            //this._userChanged = true;
        }


        private void LoadBandsOverview()
        {
            //this._userChanged = false;

            DataSet ttOverviewSet = StaticDataManager.TermsTypeBandsOverview(out _error);
            if (_error.Length > 0)
                ShowError(_error);
            else
            {
                foreach (DataTable dt in ttOverviewSet.Tables)
                {
                    if (dt.TableName == TN.TTOverview)
                    {
                        _overViewTable = dt;
                        _filteredOverViewTable = _overViewTable.Copy();
                        this.dgOverview.DataSource = _filteredOverViewTable;
                        dgOverview.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgOverview.Columns[CN.TermsType].HeaderText = GetResource("T_TERMS");
                        dgOverview.Columns[CN.TermsType].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgOverview.Columns[CN.Description].HeaderText = GetResource("T_DESCRIPTION");
                        dgOverview.Columns[CN.InsPcent].HeaderText = GetResource("T_INSPC");

                        // Format the dynamic % columns
                        foreach (DataGridViewColumn col in dgOverview.Columns)
                        {
                            if (col.Name != CN.TermsType && col.Name != CN.Description)
                            {
                                col.DefaultCellStyle.Format = "F3";
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                        }

                        dgOverview.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

                        // If a terms type was passed in then select this row
                        for (int i = 0; i < dgOverview.Rows.Count; i++)
                        {
                            if (dgOverview.Rows[i].Cells[CN.TermsType].Value.ToString() == this._termsType)
                                dgOverview.CurrentCell = dgOverview.Rows[i].Cells[CN.TermsType];
                        }

                        // Filter the terms type overview for this band
                        this.ShowCurrentBand();
                    }
                }
            }

            //this._userChanged = true;
        }


        private void ShowCurrentBand()
        {
            //this._userChanged = false;

            string curTermsType = "";
            if (dgOverview.SelectedRows.Count > 0)
                curTermsType = dgOverview.SelectedRows[0].Cells[CN.TermsType].Value.ToString();

            // Only display the columns for the selected band
            foreach (DataGridViewColumn col in dgOverview.Columns)
            {
                col.Visible = (
                    col.Name == CN.TermsType ||
                    col.Name == CN.Description ||
                    col.Name == CN.InsPcent ||
                    col.Name.StartsWith(this.drpBand.Text));
            }

            // Only display Terms Types for the selected band and branch
            FilterTermsType(ref _termsList, _affinityTerms, _acctType, this.drpBand.Text, SType, IsLoan);
            _termsList.Sort = CN.TermsTypeCode;
            _filteredOverViewTable = _overViewTable.Copy();
            for (int i = _filteredOverViewTable.Rows.Count - 1; i >= 0; i--)
            {
                if (_termsList.Find(_filteredOverViewTable.Rows[i][CN.TermsType].ToString()) < 0)
                    _filteredOverViewTable.Rows[i].Delete();
            }
            _filteredOverViewTable.AcceptChanges();
            this.dgOverview.DataSource = _filteredOverViewTable;

            this.btnOK.Enabled = (_termsList.Count > 0 && (btnAuthorise.Enabled || drpBand.Enabled));

            // Select the same terms type again if it is still in the list
            for (int i = 0; i < dgOverview.Rows.Count; i++)
            {
                if (dgOverview.Rows[i].Cells[CN.TermsType].Value.ToString() == curTermsType)
                    dgOverview.CurrentCell = dgOverview.Rows[i].Cells[CN.TermsType];
            }

            //this._userChanged = true;
        }

        private void TermsTypeMatrixPopup_Load(object sender, EventArgs e)
        {
            try
            {
                Function = "TermsTypeMatrixPopup_Load";
                Wait();
                this.LoadStatic();
                this.LoadBandsOverview();
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


        private void drpBand_SelectedIndexChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (_userChanged)
            //    {
            //        Function = "drpBand_SelectedIndexChanged";
            //        Wait();

            //        this.ShowCurrentBand();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Catch(ex, Function);
            //}
            //finally
            //{
            //    StopWait();
            //}
        }

        private void drpBand_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
                Function = "drpBand_SelectionChangeCommitted";
                Wait();

                this.ShowCurrentBand();
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


        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnOK_Click";
                Authoriser = Convert.ToInt32(txtUser.Text);
                // Return the selected terms type and band
                this._termsType = dgOverview.SelectedRows[0].Cells[CN.TermsType].Value.ToString();
                this._band = this.drpBand.Text;
                string err = string.Empty;
                int saveScoreHist = CreditManager.SaveScoreHist(CustomerId, DateTime.Now, null, null, 0.0f, _band,
                                                                Credential.UserId, "New Account", AccountNo, out err);
                CustomerManager.CustomerSaveBand(CustomerId, Convert.ToChar(band));


                Close();
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


        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnCancel_Click";
                // Return the unchanged terms type and band
                Close();
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

        private void txtUser_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Function = "txtUser_Validating";
                Wait();

                txtUser.Text = txtUser.Text.Trim();
                if (txtUser.Text.Length > 0)
                {
                    errorProvider1.SetError(txtUser, "");
                }
                else
                {
                    txtUser.Focus();
                    txtUser.Select(0, txtUser.Text.Length);
                    errorProvider1.SetError(txtUser, GetResource("M_ENTERMANDATORY"));
                }
            }
            catch (Exception ex)
            {
                txtUser.Focus();
                txtUser.Select(0, txtUser.Text.Length);
                errorProvider1.SetError(txtUser, ex.Message);
            }
            finally
            {
                StopWait();
            }
        }

        private void txtPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Function = "txtPassword_Validating";
                Wait();
                btnAuthorise.Enabled = false;

                if (txtUser.Text.Length != 0)
                {
                    var userId = StaticDataManager.ControlPermissionPasswordCheck(txtUser.Text, txtPassword.Text, this.Name, "btnAuthorise");
                    if (userId.HasValue)
                    {
                        btnAuthorise.Visible = true;
                        btnAuthorise.Enabled = true;
                        btnAuthorise.Focus();
                    }
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


        private void txtPassword_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            try
            {
                Function = "txtPassword_KeyPress";
                Wait();

                if (e.KeyChar == (char)13)
                    txtPassword_Validating(null, null);
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


        private void btnAuthorise_Click(object sender, EventArgs e)
        {
            try
            {
                Function = "btnAuthorise_Click";
                Wait();

                this.gbAuthorise.Enabled = false;
                string err = "";
                bool? islatest = AccountManager.IsLatestAccountforCustomer(AccountNo, CustomerId, out err);
                if (Convert.ToBoolean((islatest)))
                    this.drpBand.Enabled = true;
                else
                    errorProvider1.SetError(drpBand, "Account not latest for Customer so band cannot be changed");
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


        private void dgOverview_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Function = "dgOverview_CellEnter";
                Wait();

                dgOverview.Rows[dgOverview.CurrentCell.RowIndex].Selected = true;
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




    }
}
