using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;


namespace STL.PL
{
    public partial class ScoringbandMatrix : CommonForm
    {
        private string _error = "";
        private bool _userChanged = false;
        private DataSet _matrix = null;
        private bool _savePending = false;
        private DateTime _serverDate = Date.blankDate;


        public ScoringbandMatrix(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuMatrix });
        }

        public ScoringbandMatrix(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            HashMenus();
            this.ApplyRoleRestrictions();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile, menuMatrix });
        }


        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":menuSave"] = this.menuSave;
            dynamicMenus[this.Name + ":btnSave"] = this.btnSave;
            dynamicMenus[this.Name + ":drpCountry"] = this.drpCountry;
            dynamicMenus[this.Name + ":dgMatrix"] = this.dgMatrix;
            dynamicMenus[this.Name + ":menuExport"] = this.menuExport;
        }


        private void LoadStatic()
        {
            this._userChanged = false;
            this._serverDate = StaticDataManager.GetServerDate();
            this.dtStartDate.MinDate = this._serverDate;
            this.ttTermsTypeMatrix.SetToolTip(btnApply, GetResource("TT_SERVICECHARGEUPDATE"));

            int index = 0;
            bool found = false;

            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.Countries] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.Countries, new string[] { "CTY", "L" }));

            if (StaticData.Tables[TN.ScoreCards] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.ScoreCards, new string[] { "SCT", "L" })); // CR1034 SC 17-02-10

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

            cmb_scorecardtype.DataSource = (DataTable)StaticData.Tables[TN.ScoreCards]; //SC CR1034 Behavioural Scoring 15/02/2010 Get new scorecard types.
            cmb_scorecardtype.DisplayMember = CN.CodeDescription;
            cmb_scorecardtype.ValueMember = CN.Code;
            this.cmb_scorecardtype.SelectedValueChanged += new System.EventHandler(this.cmb_scorecardtype_SelectedValueChanged);

            drpCountry.DataSource = (DataTable)StaticData.Tables[TN.Countries];
            drpCountry.DisplayMember = CN.CodeDescription;
            foreach (DataRow r in ((DataTable)StaticData.Tables[TN.Countries]).Rows)
            {
                if ((string)r[CN.Code] == Config.CountryCode)
                {
                    found = true;
                    break;
                }
                index++;
            }
            if (found) drpCountry.SelectedIndex = index;

            this._userChanged = true;
        }


        private void LoadMatrix()
        {
            this._userChanged = false;

            _matrix = CreditManager.GetTermsTypeMatrix((string)((DataRowView)drpCountry.SelectedItem)[CN.Code], Convert.ToChar(cmb_scorecardtype.SelectedValue), out _error);// CR1034 SC 17-02-10
            if (_error.Length > 0)
                ShowError(_error);
            else
            {
                foreach (DataTable dt in _matrix.Tables)
                {
                    if (dt.TableName == TN.TermsTypeMatrix)
                    {
                        dgMatrix.DataSource = dt;
                        dgMatrix.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                        dgMatrix.Columns[CN.CountryCode].Visible = false;
                        dgMatrix.Columns[CN.StartDate].Visible = false;

                        dgMatrix.Columns[CN.Band].HeaderText = GetResource("T_BAND");
                        dgMatrix.Columns[CN.PointsFrom].HeaderText = GetResource("T_POINTSFROM");
                        dgMatrix.Columns[CN.PointsTo].HeaderText = GetResource("T_POINTSTO");
                        dgMatrix.Columns[CN.ServiceChargePC].HeaderText = GetResource("T_SERVICECHARGE");

                        // Allow full edit for certain roles
                        dgMatrix.AllowUserToAddRows = this.menuExport.Enabled;
                        dgMatrix.Columns[CN.Band].ReadOnly = !this.menuExport.Enabled;
                        dgMatrix.Columns[CN.PointsFrom].ReadOnly = !this.menuExport.Enabled;
                        dgMatrix.Columns[CN.PointsTo].ReadOnly = !this.menuExport.Enabled;
                        // Other roles can edit service charge
                        dgMatrix.Columns[CN.ServiceChargePC].ReadOnly = false;

                        dgMatrix.Columns[CN.Band].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgMatrix.Columns[CN.PointsFrom].DefaultCellStyle.Format = "N0";
                        dgMatrix.Columns[CN.PointsFrom].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgMatrix.Columns[CN.PointsTo].DefaultCellStyle.Format = "N0";
                        dgMatrix.Columns[CN.PointsTo].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        dgMatrix.Columns[CN.ServiceChargePC].DefaultCellStyle.Format = "F3";
                        dgMatrix.Columns[CN.ServiceChargePC].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                        this.dgMatrix.AutoResizeColumns(
                            DataGridViewAutoSizeColumnsMode.DisplayedCells);

                        this.SetApplyBtn();
                    }
                }
            }

            this._userChanged = true;
        }


        private bool SaveMatrix(string fileName, bool newImport)
        {
            // Save the Terms Type Matrix (this does not change terms types)
            bool valid = true;
            this._userChanged = false;

            // Sort the bands on the points ranges
            DataRow[] bandRows = _matrix.Tables[TN.TermsTypeMatrix].Select("1=1", "PointsFrom ASC");

            // The points ranges must be contiguous
            for (int i = 0; i < bandRows.Length; i++)
            {
                // Each range should be from a lower value to a higher value
                valid = valid && ((short)bandRows[i][CN.PointsFrom] <= (short)bandRows[i][CN.PointsTo]);
                // Each range should follow on from the previous range
                if (i == 0)
                    valid = valid && ((short)bandRows[i][CN.PointsFrom] == 0);
                else
                    valid = valid && ((short)bandRows[i - 1][CN.PointsTo] == (short)bandRows[i][CN.PointsFrom] - 1);
            }

            if (valid)
            {
                errorProvider1.SetError(dgMatrix, "");

                CreditManager.SaveTermsTypeMatrix(
                    fileName,
                    (string)((DataRowView)drpCountry.SelectedItem)[CN.Code],
                    Convert.ToChar(cmb_scorecardtype.SelectedValue), // CR1034 SC 17-02-10
                    _matrix,
                    newImport,
                    out _error);

                if (_error.Length > 0)
                {
                    valid = false;
                    ShowError(_error);
                }
                else
                {
                    this._savePending = false;

                    foreach (DataTable dt in _matrix.Tables)
                        if (dt.TableName == TN.TermsTypeMatrix)
                            foreach (DataRow r in dt.Rows)
                            {
                                r[CN.StartDate] = Date.blankDate;
                            }

                    this.SetApplyBtn();
                }
            }
            else
                errorProvider1.SetError(dgMatrix, GetResource("M_TTBANDOVERLAP"));


            this._userChanged = true;
            return valid;
        }


        private void SetApplyBtn()
        {
            string countryCode = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
            DateTime startDate = this._serverDate;
            if (dgMatrix.Rows.Count > 0)
                if (dgMatrix.Rows[0].Cells[CN.StartDate].Value != null)
                    startDate = (DateTime)dgMatrix.Rows[0].Cells[CN.StartDate].Value;

            if (countryCode == Config.CountryCode &&
                dgMatrix.Rows.Count > 0 && startDate == Date.blankDate)
            {
                // Allow the new matrix to be applied to the Terms Types
                this.dtStartDate.MinDate = this._serverDate;
                this.dtStartDate.Value = this._serverDate;
                this.dtStartDate.Enabled = true;
                this.btnApply.Enabled = true;
            }
            else
            {
                // The matrix has already been applied to the Terms Types
                this.dtStartDate.MinDate = startDate;
                this.dtStartDate.Value = startDate;
                this.dtStartDate.Enabled = false;
                this.btnApply.Enabled = false;
            }
        }



        //
        // Events
        //
        private void TermsTypeMatrix_Load(object sender, EventArgs e)
        {
            try
            {
                Function = "TermsTypeMatrix_Load";
                Wait();
                this.LoadStatic();
                this.LoadMatrix();
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


        private void drpCountry_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Function = "drpCountry_SelectedIndexChanged";
                Wait();
                if (_userChanged) LoadMatrix();
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


        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "btnSave_Click";
                Wait();
                SaveMatrix("", false);
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


        private void menuExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuExit_Click";
                Wait();
                CloseTab();
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


        private void menuImport_Click(object sender, System.EventArgs e)
        {
            int index = 0;
            string country = "";
            char scorecard = ' ';
            bool load = true;

            try
            {
                Function = "menuImport_Click";
                Wait();

                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                open.Title = "Import ScoreBand Matrix for " + cmb_scorecardtype.Text;

                if (open.ShowDialog() == DialogResult.OK)
                {
                    DataSet temp = new DataSet();
                    temp.ReadXml(open.FileName, XmlReadMode.Auto);
                    if (temp.Tables[TN.TermsTypeMatrix].Rows.Count > 0)
                    {
                        country = (string)temp.Tables[TN.TermsTypeMatrix].Rows[0][CN.CountryCode];
                        if (temp.Tables[TN.TermsTypeMatrix].Rows[0][CN.ScoringCard] == null)
                        {
                            scorecard = 'A';
                        }
                        else
                        {
                            scorecard = Convert.ToChar(temp.Tables[TN.TermsTypeMatrix].Rows[0][CN.ScoringCard]);
                        }
                    }

                    if (scorecard != Convert.ToChar(cmb_scorecardtype.SelectedValue)) //SC CR1034 Behavioural Scoring 15/02/2010 Checks if loading wrong scorecard for selected type and wrong country.
                    {
                        if (MessageBox.Show("The Scorecard Matrix you are loading does not match the category selected." +
                     Environment.NewLine +
                     "Are you sure you want to import these rules under " + cmb_scorecardtype.Text + "?" +
                      Environment.NewLine +
                      Environment.NewLine +
                     "Click OK to continue.", "Scorecard type Mismatch", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        {
                            load = false;
                        }
                    }

                    if (country.Trim().ToUpper() != Config.CountryCode)       // #12092 jec 23/01/13
                    {
                        ShowInfo("M_MATRIXTTWRONGCOUNTRY");
                        load = false;
                    }

                    if (load)
                    {
                        _matrix = temp;
                        foreach (DataRow r in ((DataTable)StaticData.Tables[TN.Countries]).Rows)
                        {
                            if ((string)r[CN.Code] == country.Trim().ToUpper())     // #12092 jec 23/01/13
                                break;
                            index++;
                        }
                        drpCountry.SelectedIndex = index;
                        this.SaveMatrix(open.FileName, true);
                        MessageBox.Show("New matrix successfully imported and saved.", "Matrix saved successfully", MessageBoxButtons.OK);
                    }
                }
                dgMatrix.DataSource = _matrix.Tables[TN.TermsTypeMatrix];
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


        private void menuExport_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuExport_Click";
                Wait();

                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                save.Title = "Save Exported ScoreBand Matrix for " + cmb_scorecardtype.Text;

                // If this _matrix has not been saved then the country column 
                // will be set to DBNull rather than the right country which will 
                // cause problems when it comes to importing it. Loop through and
                // set the country column
                string country = (string)((DataRowView)drpCountry.SelectedItem)[CN.Code];
                _matrix.Tables[TN.TermsTypeMatrix].Columns.Add(CN.ScoringCard, typeof(System.Char));

                foreach (DataTable dt in _matrix.Tables)
                    if (dt.TableName == TN.TermsTypeMatrix)
                        foreach (DataRow r in dt.Rows)
                        {
                            r[CN.CountryCode] = country;
                            r[CN.StartDate] = Date.blankDate;
                            r[CN.ScoringCard] = Convert.ToChar(cmb_scorecardtype.SelectedValue);
                        }

                if (save.ShowDialog() == DialogResult.OK)
                {
                    _matrix.WriteXml(save.FileName, XmlWriteMode.WriteSchema);
                    MessageBox.Show("Current matrix successfully exported and saved.", "Matrix saved successfully", MessageBoxButtons.OK);
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


        private void dgMatrix_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            //if (_userChanged)
            //{
            //    try
            //    {
            //        Function = "dgMatrix_CellValidating";
            //        Wait();

            //        this._savePending = true;

            //        // Band should not be blank
            //        if (this.dgMatrix.Columns[e.ColumnIndex].Name == CN.Band)
            //        {
            //            if (String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
            //            {
            //                this.dgMatrix.Rows[e.RowIndex].ErrorText = GetResource("M_ENTERMANDATORY");
            //                e.Cancel = true;
            //            }


            //            if (cmb_scorecardtype.SelectedValue.ToString() == "A" && 
            //                !(Convert.ToChar(e.FormattedValue.ToString().ToUpper()) >= 'A' &&
            //                Convert.ToChar(e.FormattedValue.ToString().ToUpper()) <= 'H'))
            //            {
            //                this.dgMatrix.Rows[e.RowIndex].ErrorText = GetResource("M_SCOREAPPBANDWRONG");
            //                e.Cancel = true;
            //            }

            //            if (cmb_scorecardtype.SelectedValue.ToString() == "B" &&
            //               !(Convert.ToChar(e.FormattedValue.ToString().ToUpper()) >= 'I' &&
            //               Convert.ToChar(e.FormattedValue.ToString().ToUpper()) <= 'P'))
            //            {
            //                this.dgMatrix.Rows[e.RowIndex].ErrorText = GetResource("M_SCOREBHBANDWRONG");
            //                e.Cancel = true;
            //            }
            //        }

            //        // Points and service charge must be numeric
            //        if (this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsFrom ||
            //            this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsTo ||
            //            this.dgMatrix.Columns[e.ColumnIndex].Name == CN.ServiceChargePC)
            //        {
            //            // Validate the numeric cells
            //            bool numberOK = false;

            //            if (!String.IsNullOrEmpty(e.FormattedValue.ToString().Trim()))
            //            {
            //                numberOK = numberOK ||
            //                         ((this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsFrom ||
            //                           this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsTo) &&
            //                          IsPositive(e.FormattedValue.ToString()));

            //                numberOK = numberOK ||
            //                         ((this.dgMatrix.Columns[e.ColumnIndex].Name == CN.ServiceChargePC) &&
            //                          IsStrictNumeric(e.FormattedValue.ToString()));
            //            }

            //            if (numberOK && Convert.ToDecimal(e.FormattedValue) < 0) numberOK = false;

            //            if (!numberOK)
            //            {
            //                this.dgMatrix.Rows[e.RowIndex].ErrorText = GetResource("M_POSITIVENUM");
            //                e.Cancel = true;
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Catch(ex, Function);
            //    }
            //    finally
            //    {
            //        StopWait();
            //    }
            //}

        }


        private void dgMatrix_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool error = false;
            try
            {
                string value = this.dgMatrix[e.ColumnIndex, e.RowIndex].Value.ToString();

                if (String.IsNullOrEmpty(((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value.ToString()))
                {
                    this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_ENTERMANDATORY");
                    error = true;
                }


                if (!error && e.ColumnIndex == 1)
                {
                    if (value.Length > 1)
                    {
                        this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = "Band can only be 1 letter.";
                        error = true;
                    }
                    else
                    {   //Applicant Score Card
                        if (cmb_scorecardtype.SelectedValue.ToString() == "A" &&
                                   !(Convert.ToChar(value.ToUpper()) >= 'A' &&
                                   Convert.ToChar(value.ToUpper()) <= 'H'))
                        {
                            this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_SCOREAPPBANDWRONG");
                            error = true;
                        }
                        //Behavioural.... 
                        if (cmb_scorecardtype.SelectedValue.ToString() == "B" &&
                           !(Convert.ToChar(value.ToUpper()) >= '1' &&
                           Convert.ToChar(value.ToUpper()) <= '9'))
                        {
                            this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_SCOREBHBANDWRONG");
                            error = true;
                        }
                    }
                }

                if (!error && this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsFrom ||
                       this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsTo ||
                       this.dgMatrix.Columns[e.ColumnIndex].Name == CN.ServiceChargePC)
                {
                    // Validate the numeric cells
                    bool numberOK = false;

                    if (!String.IsNullOrEmpty(value.Trim()))
                    {
                        numberOK = numberOK ||
                                 ((this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsFrom ||
                                   this.dgMatrix.Columns[e.ColumnIndex].Name == CN.PointsTo) &&
                                  IsPositive(value.ToString()));

                        numberOK = numberOK ||
                                 ((this.dgMatrix.Columns[e.ColumnIndex].Name == CN.ServiceChargePC) &&
                                  IsStrictNumeric(value));
                    }

                    if (numberOK && Convert.ToDecimal(value) < 0) numberOK = false;

                    if (!numberOK)
                    {
                        this.dgMatrix.Rows[e.RowIndex].ErrorText = GetResource("M_POSITIVENUM");
                        error = true;
                    }
                }

                if (!error)
                {
                    this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = "";
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


        private void btnApply_Click(object sender, EventArgs e)
        {
            // Populate all the terms types with the new matrix
            // Current intratehistory records will be given an end date 
            // and a new intratehistory record will be created for each band.
            try
            {
                Function = "btnApply_Click";
                Wait();

                bool status = true;
                if (this._savePending) status = this.SaveMatrix("", false);

                if (status)
                {
                    CreditManager.ApplyTermsTypeMatrix(dtStartDate.Value, Convert.ToChar(cmb_scorecardtype.SelectedValue), out _error);

                    if (_error.Length > 0)
                    {
                        ShowError(_error);
                    }
                    else
                        this.LoadMatrix();
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

        private void cmb_scorecardtype_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadMatrix();// CR1034 SC 17-02-10
        }

        private void dgMatrix_CellLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgMatrix_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
        }

        private void dgMatrix_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            //  this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = "";
        }

    }
}



