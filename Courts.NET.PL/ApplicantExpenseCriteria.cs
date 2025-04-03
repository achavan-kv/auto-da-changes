using Blue.Cosacs.Shared;
using STL.Common.Static;
using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{
    public partial class ApplicantExpenseCriteria : CommonForm
    {

        private bool GridHasError = false;
        public readonly bool EditAppSpendFactor=false;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuSave;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgMatrix;
        private System.Windows.Forms.Button btnSave;

        #region
        // Events

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "menuExit_Click";
                // Wait();
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

        private void dgMatrix_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool error = false;
            double temp1 = 0;
            double temp = 0.0;

            double cellValue = 0;
            try
            {

                string value = this.dgMatrix[e.ColumnIndex, e.RowIndex].Value.ToString();

                if (value.Equals("0"))
                    return;

                if (value.Contains(" ") || string.IsNullOrEmpty(value) || value.Contains("-"))
                {
                    this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_SPACEORNULL");
                    error = true;
                    goto Error;
                }


                if (value.Contains(">") && e.ColumnIndex.Equals(0))
                {
                    value = value.Replace(" ", "");

                    if (!(double.TryParse(value.Substring(1, value.Length - 1), out temp1)))
                    {
                        this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_INVALIDVALUE");
                        error = true;
                    }
                    cellValue = temp1;

                }
                else
                {
                    if (!(double.TryParse(value, out temp)))
                    {
                        this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_INVALIDVALUE");
                        error = true;
                        goto Error;
                    }
                    cellValue = temp;

                }





                if (!e.ColumnIndex.Equals(2))
                {
                    int err = this.checkCriteria(cellValue, (DataTable)this.dgMatrix.DataSource, e, value, out err);
                    if (err > 0)
                    {
                        this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_DUPLICATEPART");
                        error = true;
                        goto Error;
                    }

                }




                //if (!e.ColumnIndex.Equals(2))
                //{
                //    int err = 0;
                //    int Count = 0;//this.getCount(value, (DataTable)this.dgMatrix.DataSource, e, out err);

                //    if (Count > 0 && err.Equals(0))
                //    {
                //        this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = "Duplicate/Invalid Value";
                //        error = true; 

                //    }
                //}

                if (String.IsNullOrEmpty(((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value.ToString()))
                {
                    this.dgMatrix[e.ColumnIndex, e.RowIndex].ErrorText = GetResource("M_ENTERMANDATORY");
                    error = true;
                    goto Error;
                }

                Error:
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

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            foreach (DataGridViewRow row in dgMatrix.Rows)
            {
                foreach (DataGridViewCell x in row.Cells)
                {
                    if (!(string.IsNullOrEmpty(x.ErrorText)))
                    {
                        //set label text 
                        return;
                    }
                }
            }
            try
            {
                GridHasError = false;

                Function = "btnSave_Click";
                Wait();
                DataTable dt = new DataTable();
                dt = RemoveSpaces((DataTable)dgMatrix.DataSource);
                if (this.GridHasError)
                {
                    ShowError(GetResource("M_CHECKVALIDATION"));
                }
                else
                {
                    string Error = string.Empty;
                    CreditManager.SaveApplicantSpendMatrix(dt, out Error);
                    if (Error.Length > 0)
                    {
                        ShowError(Error);                    
                    }
                    else
                    {
                        ((MainForm)FormRoot).StatusBarText = GetResource("M_MATRIXSAVED");
                    }                    
                }

                LoadMatrix();
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

        private int getCount(string Search, DataTable dt, DataGridViewCellEventArgs e, out int err)
        {
            int counter = 0;
            bool specialFlag = false;
            int noOfDEp = 0;
            err = 0;

            int.TryParse(Search, out noOfDEp);

            if (Search.Contains(">"))
            {
                specialFlag = true;
                int.TryParse(Search.Substring(1, Search.Length - 1), out noOfDEp);

            }



            foreach (DataRow dr in dt.Rows)
            {
                if (e.ColumnIndex.Equals(0))
                {
                    if ((dr[GetResource("T_MinimumIncome")].ToString().Equals(Search)) && (!dt.Rows.IndexOf(dr).Equals(e.RowIndex)))
                    {
                        counter++;
                        return counter;
                    }
                    if (specialFlag)
                    {
                        if (dr[GetResource("T_MinimumIncome")].ToString().Contains(">"))
                        {
                            string val = dr[GetResource("T_MinimumIncome")].ToString();
                            if (int.Parse(val.Substring(1, val.Length - 1)) > noOfDEp || (val.Contains(">") && (!dt.Rows.IndexOf(dr).Equals(e.RowIndex))))
                            {
                                counter++;
                                return counter;
                            }

                        }
                        else if (int.Parse(dr[GetResource("T_MinimumIncome")].ToString()) > noOfDEp)
                        {
                            counter++;
                            return counter;
                        }
                    }

                    if (dr[GetResource("T_MinimumIncome")].ToString().Contains(">"))
                    {
                        string val = dr[GetResource("T_MinimumIncome")].ToString();
                        if (int.Parse(val.Substring(1, val.Length - 1)) < noOfDEp)
                        {
                            counter++;
                            return counter;

                        }
                    }

                }
                if (e.ColumnIndex.Equals(1))
                {
                    if ((dr[GetResource("T_MaximumIncome")].ToString().Equals(Search)) && (!dt.Rows.IndexOf(dr).Equals(e.RowIndex)))
                    {
                        counter++;
                        return counter;
                    }
                }

            }
            return counter;
        }

        #endregion

        #region
        // private Methods

        private void dgMatrix_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            this.dgMatrix[anError.ColumnIndex, anError.RowIndex].ErrorText = anError.Exception.Message.ToString();
            MessageBox.Show(anError.Exception.Message);
        }

        private void LoadMatrix()
        {
            DataTable myTable = new DataTable();
            DataSet ds = new DataSet();
            string Error = string.Empty;
            ds = CreditManager.GetApplicantSpendMatrix(out Error);
            if (Error.Length > 0)
            {
                ShowError(Error);
            }                    
            else
            {
                myTable = ds.Tables[0];
                myTable.Columns[CN.ID].ReadOnly = true;
                myTable.Columns[CN.ID].ColumnMapping = MappingType.Hidden;
                myTable.Columns[CN.ID].DefaultValue = 0;
               
                //myTable.Columns[GetResource("T_MinimumIncome")].DefaultValue = 0;
                //myTable.Columns[GetResource("T_MaximumIncome")].DefaultValue = 0;
                //myTable.Columns[GetResource("T_ApplicantPerFactor")].DefaultValue = 0;

                myTable.Columns[CN.MinimumIncome].DefaultValue = 0;
                myTable.Columns[CN.MaximumIncome].DefaultValue = 0;
                myTable.Columns[CN.ApplicantSpendFactorInPercent].DefaultValue = 0;


                myTable.Columns[CN.MinimumIncome].ColumnName = GetResource("T_MinimumIncome");
                myTable.Columns[CN.MaximumIncome].ColumnName = GetResource("T_MaximumIncome");
                myTable.Columns[CN.ApplicantSpendFactorInPercent].ColumnName = GetResource("T_ApplicantPerFactor");


                foreach (DataColumn dt in myTable.Columns)
                {
                     dt.ReadOnly = !Credential.HasPermission(CosacsPermissionEnum.EditAppSpendFactor);
                }

                dgMatrix.DataSource = myTable;

                dgMatrix.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgMatrix.Columns[GetResource("T_MinimumIncome")].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgMatrix.Columns[GetResource("T_MaximumIncome")].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgMatrix.Columns[GetResource("T_ApplicantPerFactor")].SortMode = DataGridViewColumnSortMode.NotSortable;

                this.dgMatrix.AutoResizeColumns();

                //((DataGridViewTextBoxColumn)dgMatrix.Columns["NumOfDependents"]).ValueType = Type.GetType("float");

            }
        }
        private DataTable RemoveSpaces(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr[GetResource("T_MinimumIncome")] = dr[GetResource("T_MinimumIncome")].ToString().Replace(" ", "");
                dr[GetResource("T_MaximumIncome")] = dr[GetResource("T_MaximumIncome")].ToString().Replace(" ", "");
                dr[GetResource("T_ApplicantPerFactor")] = dr[GetResource("T_ApplicantPerFactor")].ToString().Replace(" ", "");
            }

            return dt;
        }
        private int checkCriteria(double cellValue, DataTable dt, DataGridViewCellEventArgs e, string OrgCellValue, out int err)
        {
            err = 0;
            bool specialFlag = false;
            string minvalue = "";
            ;
            if (e.ColumnIndex.Equals(0))
            {
                if (OrgCellValue.Contains(">"))
                {
                    specialFlag = true;
                    cellValue = cellValue + 1;
                }
                else if ((!(string.IsNullOrEmpty(this.dgMatrix[1, e.RowIndex].Value.ToString()))) && cellValue >= double.Parse(this.dgMatrix[1, e.RowIndex].Value.ToString()) && double.Parse(this.dgMatrix[1, e.RowIndex].Value.ToString()) != 0)
                {
                    return 1;
                }
            }
            else if (e.ColumnIndex.Equals(1))
            {
                if (OrgCellValue.Contains(">") || cellValue <= double.Parse(this.dgMatrix[0, e.RowIndex].Value.ToString()))
                {
                    return 1;
                }
            }


            //if (e.ColumnIndex.Equals(0) && !(string.IsNullOrEmpty(this.dgMatrix[1, e.RowIndex].Value.ToString())))
            //{
            //    if (cellValue >= (double)this.dgMatrix[1, e.RowIndex].Value)
            //    {
            //        return 1;
            //    }

            //}
            //else if (e.ColumnIndex.Equals(1) && !(string.IsNullOrEmpty(this.dgMatrix[0, e.RowIndex].Value.ToString())))
            //{
            //    if (cellValue < double.Parse(this.dgMatrix[0, e.RowIndex].Value.ToString()))
            //    {
            //        return 1;
            //    }

            //}


            foreach (DataRow dr in dt.Rows)
            {
                if (dr[GetResource("T_MinimumIncome")].ToString().Equals(0) && dr[GetResource("T_MaximumIncome")].ToString().Equals(0))
                    continue;

                minvalue = dr[GetResource("T_MinimumIncome")].ToString().Contains(">") ? dr[GetResource("T_MinimumIncome")].ToString().Replace(">", "") : dr[GetResource("T_MinimumIncome")].ToString();

                if (specialFlag)
                {

                    //if ((OrgCellValue.Equals(dr[GetResource("T_MinimumIncome")].ToString()) && !(dt.Rows.IndexOf(dr).Equals(e.RowIndex))))

                    if ((dr[GetResource("T_MinimumIncome")].ToString().Contains(">") && OrgCellValue.Contains(">") && !(dt.Rows.IndexOf(dr).Equals(e.RowIndex))))
                        return 1;

                    else if (!((cellValue > double.Parse(minvalue)) && ((cellValue.Equals(double.Parse(dr[GetResource("T_MaximumIncome")].ToString()))) || (cellValue > double.Parse(dr[GetResource("T_MaximumIncome")].ToString())))))
                    {
                        return 1;
                    }
                }
                //if(!((specialFlag)&&(!(OrgCellValue.Equals(dr[GetResource("T_MinimumIncome")].ToString()))) && (cellValue > double.Parse(dr[GetResource("T_MinimumIncome")].ToString())) && (cellValue.Equals(double.Parse(dr[GetResource("T_MaximumIncome")].ToString())))))
                //{

                //       return 1;

                //}
                else if ((cellValue < double.Parse(minvalue)) &&
                        (cellValue < double.Parse(dr[GetResource("T_MaximumIncome")].ToString())))
                {
                    //do nothing
                }

                else if ((cellValue >= double.Parse(minvalue)) &&
                    (cellValue <= double.Parse(dr[GetResource("T_MaximumIncome")].ToString())) && dt.Rows.IndexOf(dr) != e.RowIndex)
                {
                    return 1;
                }
                else if ((dr[GetResource("T_MinimumIncome")].ToString().Contains(">")) && (double.Parse(dr[GetResource("T_MaximumIncome")].ToString()) >= 0 && cellValue > double.Parse(minvalue)))
                {
                    return 1;
                }
            }


            return 0;
        }
        private void HashMenus()
        {
            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":menuSave"] = EditAppSpendFactor;
            dynamicMenus[this.Name + ":dgMatrix"] = true;
        }
        public ApplicantExpenseCriteria(Form root)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = root;
            HashMenus();
            this.ApplyRoleRestrictions();
            this.LoadMatrix();
            this.menuSave.Enabled = Credential.HasPermission(CosacsPermissionEnum.EditAppSpendFactor);
            this.btnSave.Enabled = Credential.HasPermission(CosacsPermissionEnum.EditAppSpendFactor);

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
            dgMatrix.Visible = dgMatrix.Enabled = true;

        }
        private void InitializeComponent()
        {
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuSave = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.btnSave = new System.Windows.Forms.Button();
            this.dgMatrix = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgMatrix)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuSave,
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuSave
            // 
            this.menuSave.Enabled = true;
            this.menuSave.Description = "MenuItem";
            this.menuSave.ImageIndex = 1;
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.ImageIndex = 0;
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = true;//EditAppSpendFactor;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Location = new System.Drawing.Point(684, 33);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(41, 36);
            this.btnSave.TabIndex = 37;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dgMatrix
            // 
            this.dgMatrix.DataMember = "";
            this.dgMatrix.Enabled = false;
            this.dgMatrix.Location = new System.Drawing.Point(112, 64);
            this.dgMatrix.Name = "dgMatrix";
            this.dgMatrix.Size = new System.Drawing.Size(512, 272);
            this.dgMatrix.TabIndex = 0;
            this.dgMatrix.Visible = false;
            this.dgMatrix.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgMatrix_DataError);
            this.dgMatrix.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgMatrix_CellEndEdit);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.dgMatrix);
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Location = new System.Drawing.Point(1, 1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(792, 476);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Applicant Expense Criteria";
            // 
            // ApplicantExpenseCriteria
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 476);
            this.Controls.Add(this.groupBox2);
            this.Name = "ApplicantExpenseCriteria";
            this.Text = "Applicant Expense Matrices";
            ((System.ComponentModel.ISupportInitialize)(this.dgMatrix)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
