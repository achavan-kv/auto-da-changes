using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using STL.Common;

namespace STL.PL
{
	/// <summary>
	/// A user control to list the line items on an account.
	/// </summary>
	public class LineItemsTab : CommonUserControl
	{
		private System.Windows.Forms.GroupBox gbLineItems;
		public System.Windows.Forms.TreeView tvItems;
		private System.Windows.Forms.Splitter splitter1;
		public System.Windows.Forms.DataGrid dgLineItems;
		private System.Windows.Forms.GroupBox gbDelivery;
		public System.Windows.Forms.DataGrid dgDelivery;
		private CommonForm form = null;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;

		public LineItemsTab(TranslationDummy d)
		{
			InitializeComponent();
		}

		public LineItemsTab()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			form = new CommonForm();
			TranslateControls();

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LineItemsTab));
			this.gbLineItems = new System.Windows.Forms.GroupBox();
			this.dgLineItems = new System.Windows.Forms.DataGrid();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.tvItems = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.gbDelivery = new System.Windows.Forms.GroupBox();
			this.dgDelivery = new System.Windows.Forms.DataGrid();
			this.gbLineItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).BeginInit();
			this.gbDelivery.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgDelivery)).BeginInit();
			this.SuspendLayout();
			// 
			// gbLineItems
			// 
			this.gbLineItems.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.dgLineItems,
																					  this.splitter1,
																					  this.tvItems});
			this.gbLineItems.Name = "gbLineItems";
			this.gbLineItems.Size = new System.Drawing.Size(768, 176);
			this.gbLineItems.TabIndex = 0;
			this.gbLineItems.TabStop = false;
			this.gbLineItems.Text = "Line Items";
			// 
			// dgLineItems
			// 
			this.dgLineItems.CaptionText = "Line Items";
			this.dgLineItems.DataMember = "";
			this.dgLineItems.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgLineItems.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgLineItems.Location = new System.Drawing.Point(127, 16);
			this.dgLineItems.Name = "dgLineItems";
			this.dgLineItems.ReadOnly = true;
			this.dgLineItems.Size = new System.Drawing.Size(638, 157);
			this.dgLineItems.TabIndex = 2;
			this.dgLineItems.Click += new System.EventHandler(this.dgLineItems_Click);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(124, 16);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 157);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// tvItems
			// 
			this.tvItems.Dock = System.Windows.Forms.DockStyle.Left;
			this.tvItems.ImageList = this.imageList1;
			this.tvItems.Location = new System.Drawing.Point(3, 16);
			this.tvItems.Name = "tvItems";
			this.tvItems.Size = new System.Drawing.Size(121, 157);
			this.tvItems.TabIndex = 0;
			this.tvItems.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvItems_AfterSelect);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// gbDelivery
			// 
			this.gbDelivery.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.dgDelivery});
			this.gbDelivery.Location = new System.Drawing.Point(0, 176);
			this.gbDelivery.Name = "gbDelivery";
			this.gbDelivery.Size = new System.Drawing.Size(768, 128);
			this.gbDelivery.TabIndex = 1;
			this.gbDelivery.TabStop = false;
			this.gbDelivery.Text = "Delivery";
			// 
			// dgDelivery
			// 
			this.dgDelivery.CaptionText = "Delivery History";
			this.dgDelivery.DataMember = "";
			this.dgDelivery.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgDelivery.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.dgDelivery.Location = new System.Drawing.Point(3, 16);
			this.dgDelivery.Name = "dgDelivery";
			this.dgDelivery.ReadOnly = true;
			this.dgDelivery.Size = new System.Drawing.Size(762, 109);
			this.dgDelivery.TabIndex = 0;
			// 
			// LineItemsTab
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gbDelivery,
																		  this.gbLineItems});
			this.Name = "LineItemsTab";
			this.Size = new System.Drawing.Size(768, 302);
			this.gbLineItems.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgLineItems)).EndInit();
			this.gbDelivery.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgDelivery)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void tvItems_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			try
			{
				if((string)e.Node.Tag != null)
				{
					string key = (string)e.Node.Tag;
					string product = key.Substring(0, key.IndexOf("|"));
					string location = key.Substring(key.IndexOf("|")+1, key.Length-(key.IndexOf("|")+1));

					int index=0;
					foreach(DataRowView row in (DataView)dgLineItems.DataSource)
					{	
						if((string)row.Row["ProductCode"]==product && (string)row.Row["StockLocation"]==location)
						{
							dgLineItems.Select(index);
							dgLineItems.CurrentRowIndex = index;
						}
						else
						{
							dgLineItems.UnSelect(index);
						}
						index++;
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void dgLineItems_Click(object sender, System.EventArgs e)
		{
			form.Function = "dgLineItems_Click";
			
			try
			{
				//Load the delivery stuff
			}		
			catch(Exception ex)
			{
				form.Catch(ex, form.Function);
			}
		}
	}
}
