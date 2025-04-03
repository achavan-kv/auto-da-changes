using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using System.Data;
using System.Web.Services.Protocols;
using STL.PL.WS1;
using STL.PL.WS2;
using STL.PL.WS3;
using STL.PL.WS4;
using STL.PL.WS5;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ColumnNames;
using System.Collections.Specialized;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.Common.Constants.ItemTypes;
using STL.Common.Static;
namespace STL.PL
{
	/// <summary>
	/// Popup prompt to list the availability of stock at other branches.
	/// This is used when adding an out of stock item to a customer order.
	/// Stock at another branch can be selected and that stock location is
	/// then used for this item on the customer order.
	/// </summary>
	public class StockAvailabilityRegion : CommonForm
	{
		private System.Windows.Forms.DataGrid dgLocations;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnEnter;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StockAvailabilityRegion(TranslationDummy d)
		{
			InitializeComponent();
		}

		public StockAvailabilityRegion(DataView itemsInRegion, System.Windows.Forms.Form par, Form root)
		{
			InitializeComponent();

			this.FormParent = par;
			this.FormRoot = root;

			dgLocations.DataSource = itemsInRegion;

			DataGridTableStyle tabStyle = new DataGridTableStyle();
			tabStyle.MappingName = itemsInRegion.Table.TableName;
			dgLocations.TableStyles.Add(tabStyle);
				
			tabStyle.GridColumnStyles[CN.StockLocn].Width = 100;
			tabStyle.GridColumnStyles[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

			tabStyle.GridColumnStyles[CN.AvailableStock].Width = 100;
			tabStyle.GridColumnStyles[CN.AvailableStock].HeaderText = GetResource("T_AVAILSTOCK");
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
			this.dgLocations = new System.Windows.Forms.DataGrid();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnEnter = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.dgLocations)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgLocations
			// 
			this.dgLocations.CaptionText = "Available Stock At Branches";
			this.dgLocations.DataMember = "";
			this.dgLocations.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgLocations.Location = new System.Drawing.Point(80, 32);
			this.dgLocations.Name = "dgLocations";
			this.dgLocations.ReadOnly = true;
			this.dgLocations.Size = new System.Drawing.Size(232, 176);
			this.dgLocations.TabIndex = 8;
			this.dgLocations.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgLocations_MouseUp);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.btnEnter,
																					this.dgLocations});
			this.groupBox1.Location = new System.Drawing.Point(12, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(388, 264);
			this.groupBox1.TabIndex = 9;
			this.groupBox1.TabStop = false;
			// 
			// btnEnter
			// 
			this.btnEnter.Location = new System.Drawing.Point(168, 224);
			this.btnEnter.Name = "btnEnter";
			this.btnEnter.Size = new System.Drawing.Size(48, 23);
			this.btnEnter.TabIndex = 14;
			this.btnEnter.Text = "Exit";
			this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
			// 
			// StockAvailabilityRegion
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 293);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StockAvailabilityRegion";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Stock Availability";
			((System.ComponentModel.ISupportInitialize)(this.dgLocations)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			try
			{
				if(FormParent.GetType().Name == "NewAccount")
				{
					string loc = ((NewAccount)this.FormParent).Location;
					((NewAccount)this.FormParent).ClearItemDetails();
					((NewAccount)this.FormParent).Location = loc;
					((NewAccount)FormParent).drpLocation_Validating(this, new CancelEventArgs());
				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
			finally
			{
				Close();
			}
		}

		private void dgLocations_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if(dgLocations.CurrentRowIndex>=0)
				{
					dgLocations.Select(dgLocations.CurrentCell.RowNumber);
					
					if(FormParent.GetType().Name == "NewAccount")
					{
						((NewAccount)this.FormParent).Location = Convert.ToString(dgLocations[dgLocations.CurrentRowIndex,0]);
					}

				}
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}
	}
}
