using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Reflection;
using STL.Common.Printing.AgreementPrinting;
using STL.Common.Static;
using System.Xml;
using STL.PL.XMLPrinting;
using System.Collections.Specialized;
using STL.Common.Constants.Elements;



namespace STL.PL
{
	/// <summary>
	/// Popup prompt to allow the user to set the printer type, name and paper
	/// source for the laser printer.
	/// </summary>
	public class PrinterProperties : CommonForm
	{
		private System.Windows.Forms.ComboBox drpPrinterBox;
		private System.Windows.Forms.ComboBox drpSourceBox;
		private System.ComponentModel.Container components = null;

		private PrintDocument _pDoc = null;
		private XmlDocument _properties = null;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ComboBox drpPrintType;

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;

		private string _printType = "";
		private string[] _printTypes;
		private bool _showScreen = false;
		public bool ShowScreen
		{
			get{return _showScreen;}
		}
	
	
		public PrinterProperties(TranslationDummy d)
		{
			InitializeComponent();
		}

		public PrinterProperties(string[] printTypes, XmlDocument properties)
		{
			InitializeComponent();
			TranslateControls();

			/* set the show dialog flag if any of the required types are not fully specified */
			foreach(string type in printTypes)
				if(properties.DocumentElement[type][Elements.Printer].InnerText.Length==0 ||
					properties.DocumentElement[type][Elements.Tray].InnerText.Length==0)
					_showScreen = true;

			/* populate the print types drop down */
			drpPrintType.DataSource = printTypes;

			/* populate the installed printers drop down */
			foreach(string ip in PrinterSettings.InstalledPrinters)
				drpPrinterBox.Items.Add(ip);

			_properties = properties;
			_printType = printTypes[0];
			_printTypes = printTypes;

			/* if the document already contains a value for this print type select it */
			foreach(object o in drpPrinterBox.Items)
				if((string)o==properties.DocumentElement[drpPrintType.Text][Elements.Printer].InnerText)
					drpPrinterBox.SelectedItem = o;	
		}

		private void drpPrintType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			/* must record the settings of the previously selected print type */
			_properties.DocumentElement[_printType][Elements.Printer].InnerText = drpPrinterBox.Text;
			_properties.DocumentElement[_printType][Elements.Tray].InnerText = drpSourceBox.Text;
			_printType = drpPrintType.Text;

			foreach(object o in drpPrinterBox.Items)
				if((string)o==_properties.DocumentElement[drpPrintType.Text][Elements.Printer].InnerText)
				{
					drpPrinterBox.SelectedItem = o;	
					drpPrinterBox_SelectedIndexChanged(null, null);
				}
		}

		private void drpPrinterBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_pDoc = new PrintDocument();
			_pDoc.PrinterSettings.PrinterName = drpPrinterBox.Text;

			drpSourceBox.Items.Clear();
			drpSourceBox.Enabled = true;

			foreach(PaperSource ps in _pDoc.PrinterSettings.PaperSources) 
				drpSourceBox.Items.Add(ps.SourceName);

			foreach(object o in drpSourceBox.Items)
				if((string)o==_properties.DocumentElement[drpPrintType.Text][Elements.Tray].InnerText)
					drpSourceBox.SelectedItem = o;	
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
			this.drpPrinterBox = new System.Windows.Forms.ComboBox();
			this.drpSourceBox = new System.Windows.Forms.ComboBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.drpPrintType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// drpPrinterBox
			// 
			this.drpPrinterBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPrinterBox.Location = new System.Drawing.Point(16, 96);
			this.drpPrinterBox.Name = "drpPrinterBox";
			this.drpPrinterBox.Size = new System.Drawing.Size(328, 21);
			this.drpPrinterBox.TabIndex = 1;
			this.drpPrinterBox.SelectedIndexChanged += new System.EventHandler(this.drpPrinterBox_SelectedIndexChanged);
			// 
			// drpSourceBox
			// 
			this.drpSourceBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpSourceBox.Enabled = false;
			this.drpSourceBox.Location = new System.Drawing.Point(16, 152);
			this.drpSourceBox.Name = "drpSourceBox";
			this.drpSourceBox.Size = new System.Drawing.Size(328, 21);
			this.drpSourceBox.TabIndex = 1;
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(152, 208);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(48, 23);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Save";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// drpPrintType
			// 
			this.drpPrintType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.drpPrintType.Location = new System.Drawing.Point(16, 40);
			this.drpPrintType.Name = "drpPrintType";
			this.drpPrintType.Size = new System.Drawing.Size(328, 21);
			this.drpPrintType.TabIndex = 5;
			this.drpPrintType.SelectedIndexChanged += new System.EventHandler(this.drpPrintType_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "Printer Type";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 136);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 7;
			this.label2.Text = "Paper Source";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Printer Name";
			// 
			// PrinterProperties
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(368, 245);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.label3,
																		  this.label2,
																		  this.label1,
																		  this.drpPrintType,
																		  this.btnCancel,
																		  this.drpSourceBox,
																		  this.drpPrinterBox});
			this.Name = "PrinterProperties";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Printer Properties";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.PrinterProperties_Closing);
			this.ResumeLayout(false);

		}
		#endregion		

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void PrinterProperties_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_properties.DocumentElement[_printType][Elements.Printer].InnerText = drpPrinterBox.Text;
			_properties.DocumentElement[_printType][Elements.Tray].InnerText = drpSourceBox.Text;

			/* make sure all required print types have been set */
			string unset = "";
			foreach(string s in _printTypes)
			{
				if( _properties.DocumentElement[s][Elements.Printer].InnerText == "" ||
					_properties.DocumentElement[s][Elements.Tray].InnerText == "" )
					unset += s+Environment.NewLine;
			}

			if(unset.Length!=0)
			{
				if(DialogResult.OK != MessageBox.Show(GetResource("M_MISSINGPRINTPROPERTIES", new object[]{unset}), GetResource("M_INFORMATION"), MessageBoxButtons.OKCancel))
				{
					e.Cancel = true;
				}
			}
		}
	}
}
