using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Web.Services.Protocols;
using System.Xml;
using System.Data;
using STL.PL.WS5;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.TabPageNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Static;
using STL.Common;
using Crownwood.Magic.Menus;
using mshtml;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    /// <summary>
    /// Lists all debt collectors for an employee type. One or more
    /// can be selected from the list to print their commission payments.
    /// </summary>
    public class ReprintBailiffCommission : CommonForm
    {
        private System.Windows.Forms.DataGrid dgStaffDetails;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox drpBranch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox drpEmpType;
        private System.Windows.Forms.Label label2;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private new string Error = "";

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public ReprintBailiffCommission(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            InitialiseStaticData();

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });

        }

        private void InitialiseStaticData()
        {
            try
            {
                Function = "BStaticDataManager::GetDropDownData";
                XmlUtilities xml = new XmlUtilities();
                XmlDocument dropDowns = new XmlDocument();
                dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

                StringCollection branchNos = new StringCollection();
                StringCollection empTypes = new StringCollection();
                empTypes.Add("Staff Types");

                if (StaticData.Tables[TN.EmployeeTypes] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.EmployeeTypes,
                        new string[] { "ET1", "L" }));

                if (StaticData.Tables[TN.BranchNumber] == null)
                    dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.BranchNumber, null));

                if (dropDowns.DocumentElement.ChildNodes.Count > 0)
                {
                    DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        foreach (DataTable dt in ds.Tables)
                            StaticData.Tables[dt.TableName] = dt;
                    }
                }

                foreach (DataRow row in CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0].Rows)
                {
                    string str = string.Format("{0} : {1}", row[0], row[1]);
                    // Only show employee types with 'reference' column set
                    empTypes.Add(str.ToUpper());
                }

                drpEmpType.DataSource = empTypes;

                foreach (DataRow row in ((DataTable)StaticData.Tables[TN.BranchNumber]).Rows)
                {
                    branchNos.Add(Convert.ToString(row["branchno"]));
                }
                drpBranch.DataSource = branchNos;
                drpBranch.Text = Config.BranchCode;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ReprintBailiffCommission));
            this.dgStaffDetails = new System.Windows.Forms.DataGrid();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.drpBranch = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.drpEmpType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            ((System.ComponentModel.ISupportInitialize)(this.dgStaffDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // dgStaffDetails
            // 
            this.dgStaffDetails.CaptionText = "Courts Persons";
            this.dgStaffDetails.DataMember = "";
            this.dgStaffDetails.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgStaffDetails.Location = new System.Drawing.Point(64, 72);
            this.dgStaffDetails.Name = "dgStaffDetails";
            this.dgStaffDetails.ReadOnly = true;
            this.dgStaffDetails.Size = new System.Drawing.Size(600, 376);
            this.dgStaffDetails.TabIndex = 66;
            // 
            // btnPrint
            // 
            this.btnPrint.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnPrint.Image")));
            this.btnPrint.Location = new System.Drawing.Point(712, 224);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(32, 32);
            this.btnPrint.TabIndex = 83;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(360, 24);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(32, 32);
            this.btnSearch.TabIndex = 88;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // drpBranch
            // 
            this.drpBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.drpBranch.Location = new System.Drawing.Point(72, 32);
            this.drpBranch.Name = "drpBranch";
            this.drpBranch.Size = new System.Drawing.Size(48, 21);
            this.drpBranch.TabIndex = 86;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(72, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 87;
            this.label5.Text = "Branch";
            // 
            // drpEmpType
            // 
            this.drpEmpType.ItemHeight = 13;
            this.drpEmpType.Location = new System.Drawing.Point(152, 32);
            this.drpEmpType.Name = "drpEmpType";
            this.drpEmpType.Size = new System.Drawing.Size(176, 21);
            this.drpEmpType.TabIndex = 84;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(152, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 16);
            this.label2.TabIndex = 85;
            this.label2.Text = "Employee Type";
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.Text = "&File";
            // 
            // ReprintBailiffCommission
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnSearch,
																		  this.drpBranch,
																		  this.label5,
																		  this.drpEmpType,
																		  this.label2,
																		  this.btnPrint,
																		  this.dgStaffDetails});
            this.Name = "ReprintBailiffCommission";
            this.Text = "Reprint Bailiff Commission";
            ((System.ComponentModel.ISupportInitialize)(this.dgStaffDetails)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            string empType;
            string empTypeStr;
            DataSet ds = null;

            try
            {
                Wait();

                if (drpEmpType.SelectedIndex > 0)
                {
                    empTypeStr = (string)drpEmpType.SelectedItem;
                    int index = empTypeStr.IndexOf(":");
                    empType = empTypeStr.Substring(0, index - 1);

                    int branchNo = Convert.ToInt32(drpBranch.Text);

                    Login.CalculateBailiffCommission(empType, branchNo, out Error);

                    if (Error.Length > 0)
                        ShowError(Error);
                    else
                    {
                        ds = Login.LoadCommissionDetailsByType(empType, branchNo, out Error);

                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            if (ds != null)
                            {
                                dgStaffDetails.DataSource = ds.Tables[TN.SalesStaff].DefaultView;

                                if (dgStaffDetails.TableStyles.Count == 0)
                                {
                                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                                    tabStyle.MappingName = ds.Tables[TN.SalesStaff].TableName;
                                    dgStaffDetails.TableStyles.Add(tabStyle);

                                    tabStyle.GridColumnStyles[CN.BranchNo].Width = 0;

                                    tabStyle.GridColumnStyles[CN.EmployeeNo].Width = 90;
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.EmployeeNo].HeaderText = GetResource("T_EMPEENO");

                                    tabStyle.GridColumnStyles[CN.EmployeeName].Width = 140;
                                    tabStyle.GridColumnStyles[CN.EmployeeName].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.EmployeeName].HeaderText = GetResource("T_EMPEENAME");

                                    tabStyle.GridColumnStyles[CN.CommnDue].Width = 150;
                                    tabStyle.GridColumnStyles[CN.CommnDue].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.CommnDue].HeaderText = GetResource("T_COMMISSIONDUE");
                                    tabStyle.GridColumnStyles[CN.CommnDue].Alignment = HorizontalAlignment.Right;
                                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.CommnDue]).Format = DecimalPlaces;

                                    tabStyle.GridColumnStyles[CN.LstCommn].Width = 164; //IP - 10/06/08 - Format screen
                                    tabStyle.GridColumnStyles[CN.LstCommn].ReadOnly = true;
                                    tabStyle.GridColumnStyles[CN.LstCommn].HeaderText = GetResource("T_LASTCOMMN");
                                    tabStyle.GridColumnStyles[CN.LstCommn].Alignment = HorizontalAlignment.Right;
                                    ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.LstCommn]).Format = DecimalPlaces;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSearch_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnPrint_Click(object sender, System.EventArgs e)
        {
            RePrintBailiffCommission(dgStaffDetails);
        }
    }
}
