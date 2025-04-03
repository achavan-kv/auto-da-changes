using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Static;
using System.Data;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.FTransaction;
using System.Xml;
using Crownwood.Magic.Menus;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace STL.PL
{
    /// <summary>
    /// This screen allows the user to display Orders for Delivery or Collection based
    /// on various selection criteria. Once data is retrieved the user can (depending on
    /// access rights) either associate specified rows with a Picklist and print the picklist,
    /// or print delivery notes for selected orders.
    /// </summary>
    public class CancelCollectionNotes : CommonForm
    {
        private string previousAccount = "";
        private string error = "";
        private bool isDotNetWarehouse = false;
        //private DateTime _serverDate;
        //private DataTable _branchData;
        //private DataTable _deliveryAreaData;
        //private DataTable _minorProductCategories;
        //private DataTable _majorProductCategories;
        private DataTable _displayedData;
        //private Hashtable _showCategories;
        private string _noSetSelection;
        private StringCollection _sc = new StringCollection();
        private DateTime TimeLocked = DateTime.MinValue.AddYears(1899);
        private Crownwood.Magic.Menus.MenuCommand menuFile;
        private Crownwood.Magic.Menus.MenuCommand menuExit;
        private System.Windows.Forms.DataGrid dgOrders;
        private System.Windows.Forms.GroupBox gbDeliveries;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem cmenuComments;
        public STL.PL.AccountTextBox txtAccountNo;
        private System.Windows.Forms.GroupBox gbOrders;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnDelete;
      //  private System.ComponentModel.IContainer components;
		private bool accountLocked = false;


        public CancelCollectionNotes(TranslationDummy d)
        {
            InitializeComponent();
            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
            _noSetSelection = GetResource("NoSetSpecified");
        }

        public CancelCollectionNotes(Form root, Form parent)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            menuMain = new Crownwood.Magic.Menus.MenuControl();
            menuMain.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {menuFile});
            _noSetSelection = GetResource("NoSetSpecified");
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            //if( disposing )
            //{
            //    if(components != null)
            //    {
            //        components.Dispose();
            //    }
            //}
            base.Dispose( disposing );
        }

		#region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CancelCollectionNotes));
            this.menuFile = new Crownwood.Magic.Menus.MenuCommand();
            this.menuExit = new Crownwood.Magic.Menus.MenuCommand();
            this.gbDeliveries = new System.Windows.Forms.GroupBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.gbOrders = new System.Windows.Forms.GroupBox();
            this.dgOrders = new System.Windows.Forms.DataGrid();
            this.txtAccountNo = new STL.PL.AccountTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.cmenuComments = new System.Windows.Forms.MenuItem();
            this.gbDeliveries.SuspendLayout();
            this.gbOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgOrders)).BeginInit();
            this.SuspendLayout();
            // 
            // menuFile
            // 
            this.menuFile.Description = "MenuItem";
            this.menuFile.MenuCommands.AddRange(new Crownwood.Magic.Menus.MenuCommand[] {
            this.menuExit});
            this.menuFile.Text = "&File";
            // 
            // menuExit
            // 
            this.menuExit.Description = "MenuItem";
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.fileExit_Click);
            // 
            // gbDeliveries
            // 
            this.gbDeliveries.BackColor = System.Drawing.SystemColors.Control;
            this.gbDeliveries.Controls.Add(this.btnExit);
            this.gbDeliveries.Controls.Add(this.btnLoad);
            this.gbDeliveries.Controls.Add(this.gbOrders);
            this.gbDeliveries.Controls.Add(this.txtAccountNo);
            this.gbDeliveries.Controls.Add(this.label4);
            this.gbDeliveries.Controls.Add(this.btnDelete);
            this.gbDeliveries.Location = new System.Drawing.Point(0, 0);
            this.gbDeliveries.Name = "gbDeliveries";
            this.gbDeliveries.Size = new System.Drawing.Size(786, 480);
            this.gbDeliveries.TabIndex = 0;
            this.gbDeliveries.TabStop = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(480, 32);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 53;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.CausesValidation = false;
            this.btnLoad.Image = ((System.Drawing.Image)(resources.GetObject("btnLoad.Image")));
            this.btnLoad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoad.Location = new System.Drawing.Point(264, 32);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(32, 23);
            this.btnLoad.TabIndex = 52;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // gbOrders
            // 
            this.gbOrders.Controls.Add(this.dgOrders);
            this.gbOrders.Location = new System.Drawing.Point(16, 64);
            this.gbOrders.Name = "gbOrders";
            this.gbOrders.Size = new System.Drawing.Size(593, 401);
            this.gbOrders.TabIndex = 51;
            this.gbOrders.TabStop = false;
            // 
            // dgOrders
            // 
            this.dgOrders.DataMember = "";
            this.dgOrders.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgOrders.Location = new System.Drawing.Point(0, 8);
            this.dgOrders.Name = "dgOrders";
            this.dgOrders.ReadOnly = true;
            this.dgOrders.Size = new System.Drawing.Size(588, 400);
            this.dgOrders.TabIndex = 0;
            this.dgOrders.TabStop = false;
            // 
            // txtAccountNo
            // 
            this.txtAccountNo.Location = new System.Drawing.Point(144, 32);
            this.txtAccountNo.Name = "txtAccountNo";
            this.txtAccountNo.PreventPaste = false;
            this.txtAccountNo.Size = new System.Drawing.Size(112, 20);
            this.txtAccountNo.TabIndex = 50;
            this.txtAccountNo.Tag = "ACCNO";
            this.txtAccountNo.Text = "000-0000-0000-0";
            this.txtAccountNo.TextChanged += new System.EventHandler(this.txtAccountNo_TextChanged);
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(40, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 18);
            this.label4.TabIndex = 49;
            this.label4.Text = "Account Number";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(424, 32);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(48, 23);
            this.btnDelete.TabIndex = 40;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.cmenuComments});
            // 
            // cmenuComments
            // 
            this.cmenuComments.Index = 0;
            this.cmenuComments.Text = "";
            // 
            // CancelCollectionNotes
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(784, 477);
            this.Controls.Add(this.gbDeliveries);
            this.Name = "CancelCollectionNotes";
            this.Text = "Cancel Collection Notes";
            this.Load += new System.EventHandler(this.CancelCollectionNotes_Load);
            this.gbDeliveries.ResumeLayout(false);
            this.gbDeliveries.PerformLayout();
            this.gbOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgOrders)).EndInit();
            this.ResumeLayout(false);

        }
		#endregion

        /// <summary>
        /// Set all screen controls to their default values.
        /// </summary>
        private void Clear()
        {
            // Initial custom settings
            txtAccountNo.Text = "000-0000-0000-0";
            ClearGrid();
            ((MainForm)this.FormRoot).statusBar1.Text = "";
        }

        /// <summary>
        /// Populate the DataGrid based on current selection criteria.
        /// </summary>
        private void LoadOrders()
        {
            bool deliveryNotesFound = false;
            string statusText = GetResource("M_ORDERSZERO");
            DataSet ordersSet = null;
            DataView ordersListView = null;

            try 
            {
                Wait();
                	((MainForm)this.FormRoot).statusBar1.Text = GetResource("M_LOADINGDATA");

					ordersSet = AccountManager.GetDeliveryNotesForAcct(
						txtAccountNo.Text.Replace("-",""),
						Credential.UserId,
						true,
						out TimeLocked,
						out error);

					if(error.Length > 0)
					{
						ShowError(error);
						StopWait();
						return;
					}

					foreach (DataTable ordersDetails in ordersSet.Tables)
					{
						if (ordersDetails.TableName == TN.Deliveries)
						{
							_displayedData = ordersDetails;
							// Display list of Delivery notes
							deliveryNotesFound = (ordersDetails.Rows.Count > 0);
							statusText = ordersDetails.Rows.Count + GetResource("M_ORDERSLISTED");

							if (deliveryNotesFound) 
							{
								ordersListView = new DataView(ordersDetails);
								dgOrders.DataSource = ordersListView;
								ordersListView.AllowNew = false;

								//Override some retrieved data with user friendly values
								foreach (DataRow row in ordersDetails.Rows)
								{
									if (row[CN.DelOrColl].ToString() == "D")
									{
										row[CN.DelOrColl] = GetResource("Delivery");
									}
									else 
									{
										row[CN.DelOrColl] = GetResource("Collection");
									}
								}
					
								if (dgOrders.TableStyles.Count == 0)
								{
									DataGridTableStyle tabStyle = new DataGridTableStyle();
									tabStyle.MappingName = ordersListView.Table.TableName;

									dgOrders.TableStyles.Clear();
									dgOrders.TableStyles.Add(tabStyle);
									dgOrders.DataSource = ordersListView;

									// Displayed columns
          
									//Column Widths
									tabStyle.GridColumnStyles[CN.ItemNo.ToLower()].Width = 60;
									tabStyle.GridColumnStyles[CN.StockLocn.ToLower()].Width = 90;
									tabStyle.GridColumnStyles[CN.DelOrColl].Width = 70;
									tabStyle.GridColumnStyles[CN.Quantity.ToLower()].Width = 70;
									tabStyle.GridColumnStyles[CN.BuffBranchNo.ToLower()].Width = 100;
									tabStyle.GridColumnStyles[CN.BuffNo.ToLower()].Width = 80;
									tabStyle.GridColumnStyles[CN.DelQty.ToLower()].Width = 0;
									tabStyle.GridColumnStyles[CN.OrdVal.ToLower()].Width = 0;
									tabStyle.GridColumnStyles[CN.Price].Width = 0;
									tabStyle.GridColumnStyles[CN.LbfQuantity].Width = 0;
									tabStyle.GridColumnStyles[CN.LbfPrice].Width = 0;
									tabStyle.GridColumnStyles[CN.LbfOrdval].Width = 0;
									tabStyle.GridColumnStyles[CN.AgrmtNo].Width = 0;
									tabStyle.GridColumnStyles[CN.ContractNo].Width = 0;
                                    tabStyle.GridColumnStyles[CN.ItemId].Width = 0;                     //IP - CR1212 - RI - #3806
                                    tabStyle.GridColumnStyles[CN.LineItemId].Width = 0;                 // #10544

									//Headers
									tabStyle.GridColumnStyles[CN.ItemNo.ToLower()].HeaderText = GetResource("T_ITEMNO");
									tabStyle.GridColumnStyles[CN.StockLocn.ToLower()].HeaderText = GetResource("T_STOCKLOCN");
									tabStyle.GridColumnStyles[CN.DelOrColl].HeaderText = GetResource("T_DELORCOLL");
									tabStyle.GridColumnStyles[CN.Quantity.ToLower()].HeaderText = GetResource("T_QUANTITY");
									tabStyle.GridColumnStyles[CN.BuffBranchNo.ToLower()].HeaderText = GetResource("T_DELNOTEBRANCH");
									tabStyle.GridColumnStyles[CN.BuffNo.ToLower()].HeaderText = GetResource("T_DELNOTENUMBER");
								}
							}
						}
					
					((MainForm)this.FormRoot).statusBar1.Text = statusText;

					if (deliveryNotesFound) 
					{
						dgOrders.Focus();
						dgOrders.Select(0);
					}
				}
			}
            
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }	
        }
    
        //
        // Events
        //
        private void CancelCollectionNotes_Load(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Orders For Delivery Screen: Form Load";
                Wait();
                Clear();
                // Initial focus
                if (dgOrders.VisibleRowCount == 0)
                {
                    txtAccountNo.Focus();
                }
                this.isDotNetWarehouse = AccountManager.IsDotNetWarehouse(Convert.ToInt16(Config.BranchCode), out error);
                if (error.Length > 0)
                {
                    ShowError(error);
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        public void btnLoad_Click(object sender, System.EventArgs e)
        {
            // Load the deliveriess for this branch
            try
            {
                Function = "Orders For Delivery Screen: Load Branch";
                Wait();
                ClearGrid();
                if (previousAccount != "" && previousAccount.Length > 2)
                    AccountManager.UnlockAccount(previousAccount, Credential.UserId, out error);
                if (error.Length ==0 )
                     accountLocked = AccountManager.LockAccount(txtAccountNo.UnformattedText, Credential.UserId.ToString(),
                                                            out error);
                if (error.Length > 0)
                    ShowError(error);
                else
                {
                    previousAccount = txtAccountNo.UnformattedText;
                     LoadOrders();
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }
 

        private void fileExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Orders For Delivery Screen: File - Exit";
                Wait();
                CloseTab();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void txtAccountNo_TextChanged(object sender, System.EventArgs e)
        {
            ClearGrid();        
        }

        private void ClearGrid()
        {
            ClearControls(gbOrders.Controls);
            //We may need to change what is displayed or hidden in response
            //to this change in selection so clear the TableStyles so a new
            //entry (and therefore grid layout) will be created.
            dgOrders.TableStyles.Clear();
        }

        ////Unlock 'AccountLocking' rows that were locked by this screen
        //private void Unlock()
        //{
        //    int status = 0;
        //    try
        //    {
        //        if (!TimeLocked.Equals(DateTime.MinValue.AddYears(1899)))
        //        {
        //            Function = "Unlock()";
        //            Wait();
        //            status = AccountManager.UnlockAccountsLockedAt(Convert.ToInt32(Credential.User), TimeLocked, out error);
        //            if(error.Length > 0)
        //                ShowError(error);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        public override bool ConfirmClose()
        {
            try
            {
				AccountManager.UnlockAccount(txtAccountNo.UnformattedText, Credential.UserId,
											out error);
				if(error.Length > 0)
					ShowError(error);
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			
			return true;
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "Cancell Collection Notes: Close Tab";
                Wait();                
                CloseTab();
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void btnDelete_Click(object sender, System.EventArgs e)
        {
            try 
            {
                Wait();

                if (dgOrders.CurrentRowIndex != -1)      //IP - 28/06/11 - 5.13 - #4117
                {
                    //Cancel the selected Delivery Note
                    string acctNo = txtAccountNo.Text.Replace("-", "");
                    int agrmtNo = Convert.ToInt32(GetSelectedGridValueForColName(CN.AgrmtNo.ToLower()));
                    string itemNo = Convert.ToString(GetSelectedGridValueForColName(CN.ItemNo.ToLower()));
                    string contractNo = Convert.ToString(GetSelectedGridValueForColName(CN.ContractNo));
                    short stockLocn = Convert.ToInt16(GetSelectedGridValueForColName(CN.StockLocn.ToLower()));
                    int buffNo = Convert.ToInt32(GetSelectedGridValueForColName(CN.BuffNo.ToLower()));
                    int buffBranchNo = Convert.ToInt32(GetSelectedGridValueForColName(CN.BuffBranchNo.ToLower()));
                    string DelOrColl = (string)(GetSelectedGridValueForColName(CN.DelOrColl));
                    int itemID = Convert.ToInt32(GetSelectedGridValueForColName(CN.ItemId));           //IP - 06/06/11 - CR1212 - RI - #3806
                    int lineItemId = Convert.ToInt32(GetSelectedGridValueForColName(CN.LineItemId));   //#10544

                    // Cancel the collection
                    AccountManager.CancelCollectionNote(acctNo, agrmtNo, itemID, contractNo, stockLocn,
                        buffNo, buffBranchNo, Convert.ToInt16(Config.BranchCode),lineItemId, out error);    //#10544
                    if (error.Length > 0)
                    {
                        ShowError(error);
                    }
                    ClearGrid();
                    this.LoadOrders();
                }
            }
            catch(Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private object GetSelectedGridValueForColName(string colName)
        {
            int i = dgOrders.CurrentRowIndex;
            int colIndex = dgOrders.TableStyles[0].GridColumnStyles.IndexOf(dgOrders.TableStyles[0].GridColumnStyles[colName]);
            return dgOrders[i,colIndex];
        }

        
    }
}
