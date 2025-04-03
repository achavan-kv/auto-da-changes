using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using Blue.Cosacs.Shared;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.CountryParameters;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    /// <summary>
    /// Maintenance screen for the country parameters. These parameters
    /// define a range of customised settings that vary from country to
    /// country. Normally only set up after a server installation or when
    /// new parameters are added when upgrading to a new release.
    /// These parameters are grouped into categories such as 'Customer',
    /// 'Delivery' and 'Follow Up'. Within each category the parameters
    /// are listed with descriptions. Some parameter values are simple yes/no
    /// options where as others contain specific textual or numeric values.
    /// </summary>
    public class CountryMaintenance : CommonForm
    {
        private new string Error = "";
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private System.Windows.Forms.GroupBox gbMain;
        private TreeViewExtended treeView;
        private System.Windows.Forms.ImageList images;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.DataGrid datagrid;
        private System.ComponentModel.IContainer components;
        private ComboBox dropDown;
        private CheckBox checkBox;
        private DateTimePicker dateTimePicker;
        private NumericUpDown numericUpDown;
        private TextBox textBox;
        private string CurrentCategory;
        private System.Windows.Forms.RichTextBox txtDescription;
        private DataView Parameters;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUndo;
        private bool suspend = false;
        private bool loadingData = false;

        public CountryMaintenance(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
        }

        public CountryMaintenance(Form root, Form parent)
        {
            try
            {
                Wait();

                InitializeComponent();

                menuMain = new Crownwood.Magic.Menus.MenuControl();
                menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] { menuFile });
                FormRoot = root;
                FormParent = parent;

                //dropDown.TextChanged += new EventHandler(OnTextChanged);
                dropDown.SelectedIndexChanged += new EventHandler(OnTextChanged);
                dropDown.Name = "dropDown";
                dropDown.Visible = false;
                dropDown.DropDownStyle = ComboBoxStyle.DropDownList;
                datagrid.PreferredRowHeight = dropDown.Height;
                datagrid.Controls.Add(dropDown);

                checkBox.CheckStateChanged += new EventHandler(OnTextChanged);
                checkBox.Name = "checkBox";
                checkBox.Visible = false;
                checkBox.Height = dropDown.Height;

                dateTimePicker.ValueChanged += new EventHandler(OnTextChanged);
                dateTimePicker.Name = "dateTimePicker";
                dateTimePicker.Visible = false;
                dateTimePicker.Height = dropDown.Height;

                //numericUpDown.ValueChanged += new EventHandler(OnTextChanged);
                numericUpDown.TextChanged += new EventHandler(OnTextChanged);
                numericUpDown.Name = "numericUpDown";
                numericUpDown.Visible = false;
                numericUpDown.Height = dropDown.Height;
                numericUpDown.DecimalPlaces = 0;
                numericUpDown.Maximum = Int32.MaxValue;
                numericUpDown.Minimum = Int32.MinValue;

                textBox.TextChanged += new EventHandler(OnTextChanged);
                textBox.Name = "textBox";
                textBox.Visible = false;
                textBox.Height = dropDown.Height;

                datagrid.PreferredRowHeight = dropDown.Height;
                datagrid.Controls.AddRange(new Control[] { dropDown, checkBox, dateTimePicker, numericUpDown, textBox });

                LoadStaticData();

                LoadNodes();
            }
            catch (Exception ex)
            {
                Catch(ex, "CountryMaintenance");
            }
            finally
            {
                StopWait();
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();

                if (!suspend)
                {
                    string text = "";
                    switch ((string)Parameters[datagrid.CurrentCell.RowNumber][CN.Type])
                    {
                        case "dropdown": text = dropDown.Text;
                            break;
                        case "checkbox": text = checkBox.Checked.ToString();
                            break;
                        case "datetime": text = dateTimePicker.Value.ToLongDateString();
                            break;
                        case "numeric": text = numericUpDown.Value.ToString();
                            break;
                        case "text": text = textBox.Text;
                            break;
                        default:
                            text = (string)Parameters[datagrid.CurrentCell.RowNumber][CN.Value];
                            break;
                    }
                    if (Parameters.Table.Columns[CN.Value].Ordinal == datagrid.CurrentCell.ColumnNumber)
                        Parameters[datagrid.CurrentCell.RowNumber][CN.Value] = text;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "OnTextChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void LoadStaticData()
        {
            XmlUtilities xml = new XmlUtilities();
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.CountryParameterCategories] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.CountryParameterCategories, new string[] { "CMC", "L" }));

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

            /* set up the tree view with the country maintenance categories */
            DataView categoryView = ((DataTable)StaticData.Tables[TN.CountryParameterCategories]).DefaultView;
            categoryView.Sort = CN.CodeDescription;
            foreach (DataRowView r in categoryView)
            {
                TreeNode n = new TreeNode((string)r[CN.CodeDescription]);
                n.Tag = (string)r[CN.Code];
                treeView.Nodes.Add(n);
            }
        }

        private void LoadNodes()
        {
            DataSet countryData = StaticDataManager.GetCountryMaintenanceParameters(Config.CountryCode, out Error);
            if (Error.Length > 0)
                ShowError(Error);
            else
            {
                // Hide certain parameters from the users (but not the application)
                this.HideParameters(countryData.Tables[TN.CountryParameters]);

                Parameters = countryData.Tables[TN.CountryParameters].DefaultView;
                datagrid.DataSource = Parameters;

                if (datagrid.TableStyles.Count == 0)
                {
                    DataGridTableStyle tabStyle = new DataGridTableStyle();
                    tabStyle.MappingName = TN.CountryParameters;
                    AddColumnStyle(CN.ParameterID, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.CountryCode, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.ParameterCategory, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.name, tabStyle, 320, true, GetResource("T_NAME"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Value, tabStyle, 130, true, GetResource("T_VALUE"), "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Type, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Precision, tabStyle, 0, true, "", "", HorizontalAlignment.Right);
                    AddColumnStyle(CN.OptionCategory, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.OptionListName, tabStyle, 0, true, "", "", HorizontalAlignment.Left);
                    AddColumnStyle(CN.Description, tabStyle, 0, true, "", "", HorizontalAlignment.Left);

                    tabStyle.PreferredRowHeight = 20;

                    datagrid.TableStyles.Add(tabStyle);
                }
            }

            treeView.SelectedNode = treeView.Nodes[0];
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CountryMaintenance));
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.dropDown = new System.Windows.Forms.ComboBox();
            this.checkBox = new System.Windows.Forms.CheckBox();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.textBox = new System.Windows.Forms.TextBox();
            this.images = new System.Windows.Forms.ImageList(this.components);
            this.gbMain = new System.Windows.Forms.GroupBox();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.datagrid = new System.Windows.Forms.DataGrid();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.txtDescription = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.treeView = new STL.PL.TreeViewExtended();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
            this.gbMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).BeginInit();
            this.SuspendLayout();
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // dropDown
            // 
            this.dropDown.Location = new System.Drawing.Point(0, 0);
            this.dropDown.Name = "dropDown";
            this.dropDown.Size = new System.Drawing.Size(121, 21);
            this.dropDown.TabIndex = 0;
            this.dropDown.Visible = false;
            // 
            // checkBox
            // 
            this.checkBox.Location = new System.Drawing.Point(0, 0);
            this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(104, 24);
            this.checkBox.TabIndex = 0;
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Location = new System.Drawing.Point(0, 0);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker.TabIndex = 0;
            // 
            // numericUpDown
            // 
            this.numericUpDown.Location = new System.Drawing.Point(0, 0);
            this.numericUpDown.Name = "numericUpDown";
            this.numericUpDown.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown.TabIndex = 0;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(100, 20);
            this.textBox.TabIndex = 0;
            // 
            // images
            // 
            this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
            this.images.TransparentColor = System.Drawing.Color.Transparent;
            this.images.Images.SetKeyName(0, "");
            this.images.Images.SetKeyName(1, "");
            // 
            // gbMain
            // 
            this.gbMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMain.BackColor = System.Drawing.SystemColors.Control;
            this.gbMain.Controls.Add(this.btnUndo);
            this.gbMain.Controls.Add(this.btnExit);
            this.gbMain.Controls.Add(this.btnSave);
            this.gbMain.Controls.Add(this.groupBox1);
            this.gbMain.Location = new System.Drawing.Point(8, 0);
            this.gbMain.Name = "gbMain";
            this.gbMain.Size = new System.Drawing.Size(776, 472);
            this.gbMain.TabIndex = 0;
            this.gbMain.TabStop = false;
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(592, 16);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(48, 23);
            this.btnUndo.TabIndex = 0;
            this.btnUndo.TabStop = false;
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(704, 16);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 0;
            this.btnExit.TabStop = false;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(648, 16);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.datagrid);
            this.groupBox1.Controls.Add(this.splitter2);
            this.groupBox1.Controls.Add(this.txtDescription);
            this.groupBox1.Controls.Add(this.splitter1);
            this.groupBox1.Controls.Add(this.treeView);
            this.groupBox1.Location = new System.Drawing.Point(24, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(728, 400);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // datagrid
            // 
            this.datagrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.datagrid.DataMember = "";
            this.datagrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.datagrid.Location = new System.Drawing.Point(230, 16);
            this.datagrid.Name = "datagrid";
            this.datagrid.ReadOnly = true;
            this.datagrid.Size = new System.Drawing.Size(495, 274);
            this.datagrid.TabIndex = 0;
            this.datagrid.CurrentCellChanged += new System.EventHandler(this.datagrid_CurrentCellChanged);
            this.datagrid.Paint += new System.Windows.Forms.PaintEventHandler(this.datagrid_Paint);
            this.datagrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.datagrid_MouseUp);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(230, 394);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(495, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtDescription.Location = new System.Drawing.Point(230, 293);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(495, 104);
            this.txtDescription.TabIndex = 0;
            this.txtDescription.TabStop = false;
            this.txtDescription.Text = "";
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(227, 16);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 381);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point(3, 16);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(224, 381);
            this.treeView.TabIndex = 0;
            this.treeView.TabStop = false;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // CountryMaintenance
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(792, 477);
            this.Controls.Add(this.gbMain);
            this.Name = "CountryMaintenance";
            this.Text = "Country Maintenance";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
            this.gbMain.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.datagrid)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void menuExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                CloseTab();
            }
            catch (Exception ex)
            {
                Catch(ex, "menuExit_Click");
            }
            finally
            {
                StopWait();
            }
        }



        private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            try
            {
                Wait();

                HideAll();

                ///find out the category that has been selected
                CurrentCategory = (string)e.Node.Tag;

                ///Filter the country options so we only see the ones for this category
                Parameters.RowFilter = "ParameterCategory = '" + (string)e.Node.Tag + "'"
                    // DSR 16/8/04 Hide certain parameters from the maintenance screen for now
                    //+ " AND CodeName <> 'allowdaunpaid' "
                    + " AND CodeName <> 'custidmask' ";
                //+ " AND CodeName <> 'discountpcent' ";

                ///Set the description for the currently selected row in the datagrid
                if (Parameters.Count > 0)
                {
                    datagrid.CurrentRowIndex = 0;
                    this.loadingData = true;
                    txtDescription.Text = (string)Parameters[0][CN.Description];
                    this.loadingData = false;
                    if (e.Node.Text.ToString() == "Service Request")
                    {
                        Parameters.Sort = CN.ParameterID;
                    }
                }
                else
                    txtDescription.Text = "";



            }
            catch (Exception ex)
            {
                Catch(ex, "treeView_AfterSelect");
            }
            finally
            {
                StopWait();
            }
        }

        private CultureInfo[] GetCultures()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            BubbleSort(cultures);
            return cultures;
        }

        private void datagrid_CurrentCellChanged(object sender, System.EventArgs e)
        {
            try
            {
                Wait();
                string ParmId;      //CR1030 
                int i = datagrid.CurrentRowIndex;
                loadingData = true;
                if (i >= 0)
                    txtDescription.Text = (string)Parameters[i][CN.Description];
                else
                    txtDescription.Text = "";

                loadingData = false;

                if (Parameters.Table.Columns[CN.Value].Ordinal == datagrid.CurrentCell.ColumnNumber)
                {
                    switch ((string)((DataView)datagrid.DataSource)[datagrid.CurrentCell.RowNumber][CN.Type])
                    {
                        case "dropdown": dropDown.Visible = false;
                            dropDown.Width = 0;
                            dropDown.Left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
                            dropDown.Top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;

                            suspend = true;
                            if ((((DataView)datagrid.DataSource)[datagrid.CurrentCell.RowNumber][CN.CodeName]).ToString() == "Culture")
                            {
                                dropDown.DataSource = GetCultures();
                                dropDown.DisplayMember = "DisplayName";
                            }
                            else if ((((DataView)datagrid.DataSource)[datagrid.CurrentCell.RowNumber][CN.CodeName]).ToString() == "MaxTimeLimit")
                            {
                                dropDown.DataSource = new List<string>() { CashierWriteLimits.Day, CashierWriteLimits.WeekSat, CashierWriteLimits.WeekSun, };
                            }
                            else
                            {
                                dropDown.DataSource = StaticData.Tables[Parameters[datagrid.CurrentCell.RowNumber][CN.OptionListName]];
                                dropDown.DisplayMember = CN.CodeDescription;
                            }
                            suspend = false;

                            string tmp = (string)Parameters[datagrid.CurrentCell.RowNumber][CN.Value];
                            i = dropDown.FindStringExact(tmp);
                            dropDown.SelectedIndex = i >= 0 ? i : 0;

                            dropDown.Visible = true;

                            HideOthers(dropDown);
                            break;
                        case "checkbox": checkBox.Visible = false;
                            checkBox.Width = 0;
                            checkBox.Left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
                            checkBox.Top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;
                            checkBox.Checked = Convert.ToBoolean(Parameters[datagrid.CurrentCell.RowNumber][CN.Value]);
                            checkBox.Visible = true;

                            HideOthers(checkBox);
                            break;
                        case "datetime": dateTimePicker.Visible = false;
                            dateTimePicker.Width = 0;
                            dateTimePicker.Left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
                            dateTimePicker.Top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;
                            dateTimePicker.Value = Convert.ToDateTime(Parameters[datagrid.CurrentCell.RowNumber][CN.Value]);
                            dateTimePicker.Visible = true;

                            HideOthers(dateTimePicker);
                            break;
                        case "numeric": numericUpDown.Visible = false;
                            numericUpDown.Increment = 1;
                            // allow increment of 0.5   jec 22/11/07
                            ParmId = Convert.ToString(Parameters[datagrid.CurrentCell.RowNumber][CN.CodeName]);
                            if (ParmId == Codename.MaxArrearsLevel)
                            {
                                numericUpDown.Increment = Convert.ToDecimal(0.5);
                            }
                            if (ParmId == Codename.MaxExceedCRLimit)    //CR1113 -  allow increment of 0.25 and minimum 0
                            {
                                numericUpDown.Increment = Convert.ToDecimal(0.25);
                                numericUpDown.Minimum = 0;
                            }
                            numericUpDown.Width = 0;
                            numericUpDown.Left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
                            numericUpDown.Top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;
                            numericUpDown.Value = Convert.ToDecimal(Parameters[datagrid.CurrentCell.RowNumber][CN.Value]);
                            numericUpDown.Visible = true;
                            numericUpDown.DecimalPlaces = Convert.ToInt32(Parameters[datagrid.CurrentCell.RowNumber][CN.Precision]);

                            HideOthers(numericUpDown);
                            break;
                        case "text": textBox.Visible = false;
                            ParmId = Convert.ToString(Parameters[datagrid.CurrentCell.RowNumber][CN.CodeName]);
                            if (ParmId == Codename.SRAcctName)
                            {
                                textBox.MaxLength = 7;
                            }
                            else
                            {
                                textBox.MaxLength = 32767;      //Default
                            }
                            textBox.Width = 0;
                            textBox.Left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
                            textBox.Top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;
                            textBox.Text = (string)Parameters[datagrid.CurrentCell.RowNumber][CN.Value];
                            textBox.Visible = true;
                            HideOthers(textBox);
                            break;
                        case "MultiText": textBox.Visible = false;
                            textBox.Width = 0;
                            textBox.Left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
                            textBox.Top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;
                            textBox.Text = (string)Parameters[datagrid.CurrentCell.RowNumber][CN.Value];
                            textBox.Visible = true;
                            HideOthers(textBox);
                            break;
                        default: HideAll();
                            break;
                    }
                }
                else
                {
                    HideAll();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "datagrid_CurrentCellChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void datagrid_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (Parameters.Table.Columns[CN.Value].Ordinal == datagrid.CurrentCell.ColumnNumber)
            {
                AlignAll();
            }
        }

        private void AlignAll()
        {
            int left = datagrid.GetCellBounds(datagrid.CurrentCell).Left;
            int top = datagrid.GetCellBounds(datagrid.CurrentCell).Top;
            int width = datagrid.GetCellBounds(datagrid.CurrentCell).Width;

            dropDown.Left = left;
            dropDown.Top = top;
            dropDown.Width = width;

            checkBox.Left = left;
            checkBox.Top = top;
            checkBox.Width = width;

            dateTimePicker.Left = left;
            dateTimePicker.Top = top;
            dateTimePicker.Width = width;

            numericUpDown.Left = left;
            numericUpDown.Top = top;
            numericUpDown.Width = width;

            textBox.Left = left;
            textBox.Top = top;
            textBox.Width = width;
        }

        private void HideAll()
        {
            dropDown.Visible = false;
            dropDown.Width = 0;

            checkBox.Visible = false;
            checkBox.Width = 0;

            dateTimePicker.Visible = false;
            dateTimePicker.Width = 0;

            numericUpDown.Visible = false;
            numericUpDown.Width = 0;

            textBox.Visible = false;
            textBox.Width = 0;
        }

        private void HideOthers(Control show)
        {
            if (show != null)
            {
                string type = show.GetType().Name;
                switch (type)
                {
                    case "ComboBox":
                        checkBox.Visible = false;
                        checkBox.Width = 0;
                        dateTimePicker.Visible = false;
                        dateTimePicker.Width = 0;
                        numericUpDown.Visible = false;
                        numericUpDown.Width = 0;
                        textBox.Visible = false;
                        textBox.Width = 0;
                        break;
                    case "CheckBox":
                        dropDown.Visible = false;
                        dropDown.Width = 0;
                        dateTimePicker.Visible = false;
                        dateTimePicker.Width = 0;
                        numericUpDown.Visible = false;
                        numericUpDown.Width = 0;
                        textBox.Visible = false;
                        textBox.Width = 0;
                        break;
                    case "DateTimePicker":
                        checkBox.Visible = false;
                        checkBox.Width = 0;
                        dropDown.Visible = false;
                        dropDown.Width = 0;
                        numericUpDown.Visible = false;
                        numericUpDown.Width = 0;
                        textBox.Visible = false;
                        textBox.Width = 0;
                        break;
                    case "NumericUpDown":
                        checkBox.Visible = false;
                        checkBox.Width = 0;
                        dropDown.Visible = false;
                        dropDown.Width = 0;
                        dateTimePicker.Visible = false;
                        dateTimePicker.Width = 0;
                        textBox.Visible = false;
                        textBox.Width = 0;
                        break;
                    case "TextBox":
                        checkBox.Visible = false;
                        checkBox.Width = 0;
                        dropDown.Visible = false;
                        dropDown.Width = 0;
                        dateTimePicker.Visible = false;
                        dateTimePicker.Width = 0;
                        numericUpDown.Visible = false;
                        numericUpDown.Width = 0;
                        break;
                    default:
                        break;
                }
                show.Focus();
            }
            else
            {
                HideAll();
            }

        }

        private void datagrid_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                Wait();
                loadingData = true;
                int i = datagrid.CurrentRowIndex;
                if (i >= 0)
                {

                    txtDescription.Text = (string)Parameters[i][CN.Description];
                    //if ( (DataView)datagrid.DataSource)[datagrid.CurrentCell.RowNumber][CN.Type]=="Text" )
                    if ((string)Parameters[i][CN.Type] == "MultiText")
                    {
                        txtDescription.ReadOnly = false;
                    }
                    else
                    {
                        txtDescription.ReadOnly = true;
                    }


                }
                loadingData = false;

            }
            catch (Exception ex)
            {
                Catch(ex, "datagrid_MouseUp");
            }
            finally
            {
                StopWait();
            }
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                Parameters.Table.RejectChanges();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnUndo_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private bool CheckSPVAndPrincipalFactor()
        {
            decimal pfoVal;
            decimal spvVal;
            bool cont = true;
            DataTable dt = Parameters.Table.GetChanges(DataRowState.Modified);


            DataRow[] spv = dt.Select("codename = 'spv'");
            if (spv.Length > 0)
            {
                spvVal = Convert.ToDecimal(spv[0]["value"]);


                // find pfo record and ensure set to -1
                DataRow[] a = Parameters.Table.Select("codename = 'principalfactoroverride'");
                pfoVal = Convert.ToDecimal(a[0]["value"]);

                if (spvVal != -1 && pfoVal != -1)
                {
                    MessageBox.Show("The SPV and Principal Factor Override values are both set, change one of them to -1 to save.");
                    cont = false;
                }

            }

            if (cont)
            {
                DataRow[] pfo = dt.Select("codename = 'principalfactoroverride'");

                if (pfo.Length > 0)
                {
                    pfoVal = Convert.ToDecimal(pfo[0]["value"]);


                    // find pfo record and ensure set to -1
                    DataRow[] a = Parameters.Table.Select("codename = 'spv'");
                    spvVal = Convert.ToDecimal(a[0]["value"]);

                    if (spvVal != -1 && pfoVal != -1)
                    {
                        MessageBox.Show("The SPV and Principal Factor Override values are both set, change one of them to -1 to save.");
                        cont = false;
                    }

                }

            }

            return cont;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            try
            {
                Wait();

                if (Parameters.Table.DataSet.HasChanges(DataRowState.Modified))
                {
                    // cr947 
                    if (CheckSPVAndPrincipalFactor())
                    {
                        DataSet ds = new DataSet();
                        ds.Tables.Add(Parameters.Table.GetChanges(DataRowState.Modified));

                        StaticDataManager.SaveCountryMaintenanceChanges(Config.CountryCode, ds, Credential.UserId, out Error);
                        if (Error.Length > 0)
                            ShowError(Error);
                        else
                        {
                            ///commit the changes so we won't try to change them again 
                            Parameters.Table.AcceptChanges();
                            ((MainForm)this.FormRoot).statusBar1.Text = "Users should exit and reenter system to ensure changes take effect";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSave_Click");
            }
            finally
            {
                StopWait();
            }
        }

        private void HideParameters(DataTable dt)
        {
            // If Privilege Club Tier 1/2 is enabled then hide the enable parameter
            if ((bool)Country[CountryParameterNames.TierPCEnabled])
            {
                string tierCategory = Country[CountryParameterNames.TierPCEnabled, CN.ParameterCategory];
                foreach (DataRow row in dt.Rows)
                    if ((string)row[CN.ParameterCategory] == tierCategory
                        && (string)row[CN.CodeName] == CountryParameterNames.TierPCEnabled)
                    {
                        // Remove this parameter to hide it from the users
                        row.Delete();
                    }
                dt.AcceptChanges();
            }
            else
            {
                // Otherwise remove the Privilege Club Tier 1/2 category completely
                string tierCategory = Country[CountryParameterNames.TierPCEnabled, CN.ParameterCategory];
                foreach (DataRow row in dt.Rows)
                    if ((string)row[CN.ParameterCategory] == tierCategory)
                        row.Delete();
                dt.AcceptChanges();
            }

            // Hide the Terms Type Scoring Matrix enable parameter
            foreach (DataRow row in dt.Rows)
                if ((string)row[CN.CodeName] == CountryParameterNames.TermsTypeBandEnabled)
                {
                    // Remove this parameter to hide it from the users
                    row.Delete();
                }
            dt.AcceptChanges();

        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {

            int i = datagrid.CurrentRowIndex;
            if (i >= 0)
            {
                if ((string)Parameters[i][CN.Type] == "MultiText")
                {
                    if (this.loadingData == false) // don't update this if user clicking on it -- only if updating...
                        Parameters[i][CN.Value] = txtDescription.Text;
                }
            }
        }

    }
}
