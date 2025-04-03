using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using Crownwood.Magic.Menus;
using STL.Common.Constants.EmployeeTypes;
using STL.Common.Constants.FTransaction;




namespace STL.PL
{
	/// <summary>
	/// When a customer completes a credit application referees are added to support
	/// the application. This screen lists all references for a customer entered
	/// from all previous credit applications. This list can be used to copy a
	/// previous referee to the current credit application.
	/// </summary>
	public class ReferenceList : CommonForm
	{
		private new string Error = "";
		private int _copyLeft = 0;
		public DataTable referenceTable = new DataTable();
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Label lCustomerName;
		private System.Windows.Forms.TextBox txtCustId;
		private System.Windows.Forms.TextBox txtCustomerName;
		private System.Windows.Forms.Label lCustomerId;
		private System.Windows.Forms.Button btnOK;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lCopyLeft;
		private System.Windows.Forms.DataGrid dgReferenceList;


		public ReferenceList()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public ReferenceList(string custId, string custName, int maxReferences)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.txtCustId.Text = custId;
			this.txtCustomerName.Text = custName;
			this._copyLeft = maxReferences;
			this.lCopyLeft.Text = GetResource("M_COPYREFERENCES", new object[]{this._copyLeft.ToString()});
			this.LoadData(custId);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ReferenceList));
			this.dgReferenceList = new System.Windows.Forms.DataGrid();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.lCustomerName = new System.Windows.Forms.Label();
			this.txtCustId = new System.Windows.Forms.TextBox();
			this.txtCustomerName = new System.Windows.Forms.TextBox();
			this.lCustomerId = new System.Windows.Forms.Label();
			this.lCopyLeft = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgReferenceList)).BeginInit();
			this.SuspendLayout();
			// 
			// dgReferenceList
			// 
			this.dgReferenceList.CaptionText = "Reference List";
			this.dgReferenceList.DataMember = "";
			this.dgReferenceList.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgReferenceList.Location = new System.Drawing.Point(8, 67);
			this.dgReferenceList.Name = "dgReferenceList";
			this.dgReferenceList.Size = new System.Drawing.Size(760, 216);
			this.dgReferenceList.TabIndex = 0;
			this.dgReferenceList.TabStop = false;
			this.dgReferenceList.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgReferenceList_MouseUp);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(264, 291);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(424, 291);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// lCustomerName
			// 
			this.lCustomerName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lCustomerName.Location = new System.Drawing.Point(256, 8);
			this.lCustomerName.Name = "lCustomerName";
			this.lCustomerName.Size = new System.Drawing.Size(88, 16);
			this.lCustomerName.TabIndex = 10;
			this.lCustomerName.Text = "Customer Name";
			this.lCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtCustId
			// 
			this.txtCustId.BackColor = System.Drawing.SystemColors.Window;
			this.txtCustId.Location = new System.Drawing.Point(112, 8);
			this.txtCustId.MaxLength = 20;
			this.txtCustId.Name = "txtCustId";
			this.txtCustId.ReadOnly = true;
			this.txtCustId.Size = new System.Drawing.Size(120, 20);
			this.txtCustId.TabIndex = 9;
			this.txtCustId.Text = "";
			// 
			// txtCustomerName
			// 
			this.txtCustomerName.BackColor = System.Drawing.SystemColors.Window;
			this.txtCustomerName.Location = new System.Drawing.Point(352, 8);
			this.txtCustomerName.MaxLength = 80;
			this.txtCustomerName.Name = "txtCustomerName";
			this.txtCustomerName.ReadOnly = true;
			this.txtCustomerName.Size = new System.Drawing.Size(312, 20);
			this.txtCustomerName.TabIndex = 7;
			this.txtCustomerName.TabStop = false;
			this.txtCustomerName.Text = "";
			// 
			// lCustomerId
			// 
			this.lCustomerId.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lCustomerId.Location = new System.Drawing.Point(32, 8);
			this.lCustomerId.Name = "lCustomerId";
			this.lCustomerId.Size = new System.Drawing.Size(72, 16);
			this.lCustomerId.TabIndex = 8;
			this.lCustomerId.Text = "Customer ID";
			this.lCustomerId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lCopyLeft
			// 
			this.lCopyLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lCopyLeft.Location = new System.Drawing.Point(113, 42);
			this.lCopyLeft.Name = "lCopyLeft";
			this.lCopyLeft.Size = new System.Drawing.Size(295, 20);
			this.lCopyLeft.TabIndex = 11;
			this.lCopyLeft.Text = "You may select  n more references";
			// 
			// ReferenceList
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(778, 325);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lCopyLeft,
																		  this.lCustomerName,
																		  this.txtCustId,
																		  this.txtCustomerName,
																		  this.lCustomerId,
																		  this.btnCancel,
																		  this.btnOK,
																		  this.dgReferenceList});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReferenceList";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "CoSACS Customer References";
			((System.ComponentModel.ISupportInitialize)(this.dgReferenceList)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		// Local Procedures
		private void AddColumnStyle (string columnName, DataGridTableStyle tabStyle,
			int width, bool readOnly, string headerText, string format)
		{
			DataGridTextBoxColumn aColumnTextColumn = new DataGridTextBoxColumn();
			aColumnTextColumn.MappingName = columnName;
			aColumnTextColumn.Width = width;
			aColumnTextColumn.ReadOnly = readOnly;
			aColumnTextColumn.HeaderText = headerText;
			aColumnTextColumn.Format = format;
			tabStyle.GridColumnStyles.Add(aColumnTextColumn);
		}  // End of AddColumnStyle

		private void LoadData (string custId)
		{
			try
			{
				Function = "Reference List Screen: Load Data";
				Wait();

				DataSet ReferenceSet = CreditManager.GetReferenceList(custId, out Error);
				if (Error.Length > 0)
				{
					ShowError(Error);
					return;
				}

				DataView ReferenceListView = null;
				string statusText = "";

				foreach (DataTable ReferenceDetails in ReferenceSet.Tables)
				{
					if (ReferenceDetails.TableName == TN.References)
					{
						//
						// Display the list of Customer References
						//
						statusText = ReferenceDetails.Rows.Count + GetResource("M_ACCOUNTSLISTED");

						// Create a view for the DataGrid
						ReferenceListView = new DataView(ReferenceDetails);
						ReferenceListView.AllowNew = false;
						//ReferenceListView.Sort = CN.RefAcctOpen + "," + CN.RefAcctNo + "," + CN.RefNo + " DESC ";
						dgReferenceList.CausesValidation = false;
						dgReferenceList.DataSource = ReferenceListView;

						if (dgReferenceList.TableStyles.Count == 0)
						{
							// Create the table style for the DataGrid
							DataGridTableStyle tabStyle = new DataGridTableStyle();
							tabStyle.MappingName = ReferenceListView.Table.TableName;

							// Add an unbound stand-alone icon column to mark new entries
							ReferenceDetails.Columns.Add(CN.Status, Type.GetType("System.String"));
							AddColumnStyle(CN.Status, tabStyle, 0, false, "", "");
							ReferenceListView.Table.Columns.Add("Icon");
							DataGridIconColumn iconColumn = new DataGridIconColumn(imageList1.Images[0], CN.Status, "0");
							iconColumn.HeaderText = "";
							iconColumn.MappingName = "Icon";
							iconColumn.Width = imageList1.Images[0].Size.Width;
							tabStyle.GridColumnStyles.Add(iconColumn);

							//
							// Style each column that needs to be displayed
							//

							// Hidden Columns
							AddColumnStyle(CN.RefRelation,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.YrsKnown,			tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefWAddress1,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefWAddress2,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefWCity,			tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefWPostCode,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefPhoneNo,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefDialCode,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefWPhoneNo,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefWDialCode,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefMPhoneNo,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefMDialCode,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefDirections,	tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.RefComment,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.DateChange,		tabStyle,   0, true,  "", "");
							AddColumnStyle(CN.EmpeeNoChange,	tabStyle,   0, true,  "", "");
							
							// Normal columns
							//AddColumnStyle(CN.RefAcctOpen,		tabStyle, 100, true,  GetResource("T_DATEOPENED"),	"");
							//AddColumnStyle(CN.RefAcctNo,		tabStyle,  90, true,  GetResource("T_ACCOUNTNO"),	"");
							AddColumnStyle(CN.RefNo,			tabStyle,  40, true,  GetResource("T_REFNOSHORT"),  "");
							AddColumnStyle(CN.RefFirstName,		tabStyle, 100, true,  GetResource("T_FIRSTNAME"),	"");
							AddColumnStyle(CN.RefLastName,		tabStyle, 100, true,  GetResource("T_LASTNAME"),	"");
							AddColumnStyle(CN.RefRelationText,	tabStyle,  90, true,  GetResource("T_RELATION"),	"");
							AddColumnStyle(CN.RefAddress1,		tabStyle, 120, true,  GetResource("T_ADDRESS1"),	"");
							AddColumnStyle(CN.RefAddress2,		tabStyle, 120, true,  GetResource("T_ADDRESS2"),	"");
							AddColumnStyle(CN.RefCity,			tabStyle,  70, true,  GetResource("T_CITY"),		"");
							AddColumnStyle(CN.RefPostCode,		tabStyle,  60, true,  GetResource("T_POSTCODE"),	"");

							dgReferenceList.TableStyles.Clear();
							dgReferenceList.TableStyles.Add(tabStyle);
							dgReferenceList.DataSource = ReferenceListView;
						}
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
		}  // End of  LoadData


		// Form Events
		private void btnOK_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Reference List Screen: OK button";
				Wait();

				// Remove the unmarked rows to only return the selected references
				this.referenceTable = ((DataView)dgReferenceList.DataSource).Table;
				foreach (DataRow row in this.referenceTable.Rows)
				{
					if (row[CN.Status].ToString() != "1") row.Delete();
				}

				Close();
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

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "Reference List Screen: Cancel button";
				Wait();

				Close();
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

		private void dgReferenceList_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Context menu to select or remove a row to be copied
			try
			{
				Function = "Reference List Screen: Right click on Reference List";
				Wait();

				int index = dgReferenceList.CurrentRowIndex;
				DataView currentView = (DataView)dgReferenceList.DataSource;

				if (index >= 0)
				{
					if (e.Button == MouseButtons.Right
						&& currentView[index][CN.Status].ToString() != "1"
						&& this._copyLeft > 0)
					{
						// Mark a reference to be copied
						DataGrid ctl = (DataGrid)sender;

						MenuCommand m1 = new MenuCommand(GetResource("P_COPY"));
						m1.Click += new System.EventHandler(this.cmenuCopy_Click);
						
						PopupMenu popup = new PopupMenu();
						popup.Animate = Animate.Yes;
						popup.AnimateStyle = Animation.SlideHorVerPositive;

						popup.MenuCommands.AddRange(new MenuCommand[] {m1});
						MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
					}
					else if (e.Button == MouseButtons.Right && currentView[index][CN.Status].ToString() == "1")
					{
						// Unmark a reference not to be copied
						DataGrid ctl = (DataGrid)sender;

						MenuCommand m1 = new MenuCommand(GetResource("P_NOCOPY"));
						m1.Click += new System.EventHandler(this.cmenuNoCopy_Click);
						
						PopupMenu popup = new PopupMenu();
						popup.Animate = Animate.Yes;
						popup.AnimateStyle = Animation.SlideHorVerPositive;

						popup.MenuCommands.AddRange(new MenuCommand[] {m1});
						MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
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
		}  // End of dgReferenceList_MouseUp

		private void cmenuCopy_Click(object sender, System.EventArgs e)
		{
			// Mark a reference to be copied
			try
			{
				Function = "Reference List Screen: Copy context menu";
				Wait();

				int index = dgReferenceList.CurrentRowIndex;
				DataView currentView = (DataView)dgReferenceList.DataSource;

				if (index >= 0)
				{
					this._copyLeft--;
					this.lCopyLeft.Text = GetResource("M_COPYREFERENCES", new object[]{this._copyLeft.ToString()});
					this.lCopyLeft.Enabled = (this._copyLeft > 0);
					currentView.AllowEdit = true;
					currentView[index][CN.Status] = "1";
					currentView.AllowEdit = false;
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
		}  // End of cmenuCopy_Click

		private void cmenuNoCopy_Click(object sender, System.EventArgs e)
		{
			// Unmark a reference NOT to be copied
			try
			{
				Function = "Reference List Screen: Do NOT Copy context menu";
				Wait();

				int index = dgReferenceList.CurrentRowIndex;
				DataView currentView = (DataView)dgReferenceList.DataSource;

				if (index >= 0)
				{
					this._copyLeft++;
					this.lCopyLeft.Text = GetResource("M_COPYREFERENCES", new object[]{this._copyLeft.ToString()});
					this.lCopyLeft.Enabled = true;
					currentView.AllowEdit = true;
					currentView[index][CN.Status] = "0";
					currentView.AllowEdit = false;
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
		}  // End of cmenuNoCopy_Click

	}

}
