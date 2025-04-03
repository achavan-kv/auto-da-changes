using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using STL.PL.WS7;
using STL.Common.Static;
using STL.Common.Constants.SanctionStages;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.ScreenModes;

namespace STL.PL
{
	/// <summary>
	/// When a customer opens a hire purchase or Ready Finanance account
	/// additional customer details are entered to check the customer is
	/// credit worthy. These details are split into a number of stages.
	/// All stages have to be completed before the goods can be authorised
	/// for delivery. This common user control appears as a row of icons
	/// on various screens to show which stages have been completed and
	/// which stages are still open. There is one icon for each stage shown
	/// as red when open and green when completed.
	/// </summary>
	public class SanctionStatus : CommonUserControl
	{
		public System.Windows.Forms.ToolBar tbSanction;
		private System.Windows.Forms.ToolBarButton tbbStage1;
		private System.Windows.Forms.ToolBarButton tbbStage2;
		private System.Windows.Forms.ToolBarButton tbbAddData;
		private System.Windows.Forms.ToolBarButton tbbDoc;
		private System.Windows.Forms.ToolBarButton tbbReferral;
		private System.Windows.Forms.ImageList ilSanction;
		private System.ComponentModel.IContainer components;
		private BasicCustomerDetails customerScreen = null;
		public BasicCustomerDetails CustomerScreen
		{
			get{return customerScreen;}
			set{customerScreen = value;}
		}

		private bool _holdProp = false;
		public bool HoldProp
		{
			get{return _holdProp;}
			set{_holdProp = value;}
		}

		private string _custid = "";
		public string CustomerID
		{
			get{return _custid;}
			set{_custid = value;}
		}

		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

		private DateTime _dateprop;
		public DateTime DateProp
		{
			get{return _dateprop;}
			set{_dateprop = value;}
		}

		private string _acctType = "";
		public string AccountType
		{
			get{return _acctType;}
			set{_acctType = value;}
		}

		private string _screenMode = SM.Edit;
		public string ScreenMode
		{
			get{return _screenMode;}
			set{_screenMode = value;}
		}

		private string _currentStatus = "";
		public string CurrentStatus
		{
			get{return _currentStatus;}
			set{_currentStatus = value;}
		}
		
		private CommonForm cf = null;
		public CommonForm Common
		{
			get{return cf;}
			set{cf = value;}
		}

		private bool _settled = false;
		public bool Settled
		{
			get{return _settled;}
			set{_settled = value;}
		}

      //Livewire 69230 new public property required to determine if the 'convert to HP' option should be available in the 'RFCreditRefused' dialog box
      private bool m_allowConversionToHP = false;
      public bool allowConversionToHP
      {
         get { return m_allowConversionToHP; }
         set { m_allowConversionToHP = value; }
      }

		/// <summary>
		/// This method will return true if the current stage is complete
		/// and flase if it is not.
		/// </summary>
		public bool ReadOnly(string stage)
		{
			return StageComplete(stage);
		}

		private void HashMenus()
		{
			cf.dynamicMenus[this.Name+":tbbStage1"] = this.tbbStage1;
			cf.dynamicMenus[this.Name+":tbbStage2"] = this.tbbStage2;
			cf.dynamicMenus[this.Name+":tbbReferral"] = this.tbbReferral;
			cf.dynamicMenus[this.Name+":tbbDoc"] = this.tbbDoc;
		}


		public SanctionStatus(TranslationDummy d)
		{
			InitializeComponent();
		}

		/// <summary>
		/// will work out whether a stage is complete or not and return the result
		/// </summary>
		/// <param name="stage"></param>
		/// <returns></returns>
		public bool StageComplete(string stage)
		{
			bool cleared = true;
			flags.Tables[TN.ProposalFlags].DefaultView.RowFilter = CN.CheckType+" = '"+stage+"'";
			if(flags.Tables[TN.ProposalFlags].DefaultView.Count>0)
			{
				if(flags.Tables[TN.ProposalFlags].DefaultView[0][CN.DateCleared]!=DBNull.Value)
					cleared = true;
				else
					cleared = false;
			}
			else
				cleared = false;
			flags.Tables[TN.ProposalFlags].DefaultView.RowFilter = "";

			return cleared;
		}

		public DateTime StageCleared(string stage)
		{
			DateTime dateCleared = Date.blankDate;
			flags.Tables[TN.ProposalFlags].DefaultView.RowFilter = CN.CheckType+" = '"+stage+"'";
			if (flags.Tables[TN.ProposalFlags].DefaultView.Count > 0)
			{
				if (flags.Tables[TN.ProposalFlags].DefaultView[0][CN.DateCleared] != DBNull.Value)
					dateCleared = (DateTime)(flags.Tables[TN.ProposalFlags].DefaultView[0][CN.DateCleared]);
			}
			return dateCleared;
		}

		private string Err = "";
		private DataSet flags = null;

		private enum Stage1
		{
			Complete = 0,
			Current = 1,
			Incomplete = 2
		}

		private enum Stage2
		{
			Complete = 3,
			Current = 4,
			Incomplete = 5
		}

		private enum Doc
		{
			Complete = 6,
			Current = 7,
			Incomplete = 8
		}

		private enum Referral
		{
			Complete = 9,
			Current = 10,
			Incomplete = 11
		}

		private enum AddData
		{
			Complete = 12,
			Current = 13,
			Incomplete = 14
		}		

		public SanctionStatus()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			cf = new CommonForm();
			cf.Name = "SanctionStatus";

			cf.dynamicMenus = new Hashtable();
			HashMenus();
		}	

		public new void Load()
		{
			_load();
		}

		public new void Load(bool hp,string customerID, DateTime dateProp, 
						string accountNo, string accountType, 
						string screenMode)
		{
         this.allowConversionToHP = hp;
			this.CustomerID = customerID;
			this.DateProp = dateProp;
			this.AccountNo = accountNo;
			this.AccountType = accountType;
			this.ScreenMode = screenMode;
			_load();
		}

		public void SetInvisible()
		{
			foreach(ToolBarButton bt in tbSanction.Buttons)
				bt.Visible = false;
		}

		private void Disable()
		{
			foreach(ToolBarButton bt in tbSanction.Buttons)
				bt.Enabled = false;
		}

		private void _load()
		{

			Disable();
			cf.ApplyRoleRestrictions();
			SetInvisible();				

			flags = cf.CreditManager.LoadProposalFlags(this.AccountNo, this.CustomerID, this.DateProp, out _holdProp, out _currentStatus, out Err);
			if(Err.Length>0)
				cf.ShowError(Err);
			else
			{	
				foreach(DataRow r in flags.Tables[TN.ProposalFlags].Rows)
				{
					switch((string)r[CN.CheckType])
					{
						case SS.S1:	SetStage1(r);
							break;
						case SS.S2: SetStage2(r);
							break;
						case SS.DC: SetDocumentControl(r);
							break;
						case SS.AD: SetAdditionalData(r);
							break;
						case SS.R:	SetUnderWriter(r);
							break;
						default:
							break;
					}
				}	
				Shuffle();
			}
		}

		private void Shuffle()
		{
			int totalWidth = 0;
			foreach(ToolBarButton b in tbSanction.Buttons)
				if(b.Visible)
					totalWidth += 20;
			totalWidth = 100 - totalWidth;
			Point p = new Point(4,0);
			p.X+=totalWidth;
			tbSanction.Location = p;
		}

		private void SetStage1(DataRow r)
		{			
			if(tbbStage1.Enabled)
				tbbStage1.Visible = true;

			if(r[CN.DateCleared]==DBNull.Value)
				tbbStage1.ImageIndex = (int)Stage1.Incomplete;
			else
				tbbStage1.ImageIndex = (int)Stage1.Complete;
		}

		private void SetStage2(DataRow r)
		{
			if(tbbStage2.Enabled)
				tbbStage2.Visible = true;

			if(this.Settled && tbbStage2.Visible)
				tbbStage2.Visible = false;
			
			if(r[CN.DateCleared]==DBNull.Value)
			{
				tbbStage2.ImageIndex = (int)Stage2.Incomplete;
				this.ScreenMode = SM.Edit;
			}
			else
			{
				tbbStage2.ImageIndex = (int)Stage2.Complete;
				this.ScreenMode = SM.View;
			}
		} 

		private void SetDocumentControl(DataRow r)
		{
			if(tbbDoc.Enabled)
				tbbDoc.Visible = true;

			if(this.Settled && tbbDoc.Visible)
				tbbDoc.Visible = false;
			
			if(r[CN.DateCleared]==DBNull.Value)
				tbbDoc.ImageIndex = (int)Doc.Incomplete;
			else
				tbbDoc.ImageIndex = (int)Doc.Complete;
		}

		private void SetAdditionalData(DataRow r)
		{
			if(tbbAddData.Enabled)
				tbbAddData.Visible = true;

			if(this.Settled && tbbAddData.Visible)
				tbbAddData.Visible = false;
			
			if(r[CN.DateCleared]==DBNull.Value)
				tbbAddData.ImageIndex = (int)AddData.Incomplete;
			else
				tbbAddData.ImageIndex = (int)AddData.Complete;
		}

		private void SetUnderWriter(DataRow r)
		{
			if(tbbReferral.Enabled)
				tbbReferral.Visible = true;

			if(this.Settled && tbbReferral.Visible)
				tbbReferral.Visible = false;
			
			if(r[CN.DateCleared]==DBNull.Value)
				tbbReferral.ImageIndex = (int)Referral.Incomplete;
			else
				tbbReferral.ImageIndex = (int)Referral.Complete;
		}

		public void SetCurrentStage(string stage)
		{
			/*
			switch(stage)
			{
				case SS.S1:	tbbStage1.ImageIndex = (int)Stage1.Current;
					break;
				case SS.S2: tbbStage2.ImageIndex = (int)Stage2.Current;
					break;
				case SS.DC: tbbDoc.ImageIndex = (int)Doc.Current;
					break;
				case SS.AD: tbbAddData.ImageIndex = (int)AddData.Current;
					break;
				case SS.R:	tbbReferral.ImageIndex = (int)Referral.Current;
					break;
				default:
					break;
			}
			*/
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SanctionStatus));
			this.tbSanction = new System.Windows.Forms.ToolBar();
			this.tbbStage1 = new System.Windows.Forms.ToolBarButton();
			this.tbbStage2 = new System.Windows.Forms.ToolBarButton();
			this.tbbDoc = new System.Windows.Forms.ToolBarButton();
			this.tbbAddData = new System.Windows.Forms.ToolBarButton();
			this.tbbReferral = new System.Windows.Forms.ToolBarButton();
			this.ilSanction = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// tbSanction
			// 
			this.tbSanction.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.tbSanction.AutoSize = false;
			this.tbSanction.BackColor = System.Drawing.SystemColors.ControlDark;
			this.tbSanction.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						  this.tbbStage1,
																						  this.tbbStage2,
																						  this.tbbDoc,
																						  this.tbbAddData,
																						  this.tbbReferral});
			this.tbSanction.ButtonSize = new System.Drawing.Size(14, 14);
			this.tbSanction.Cursor = System.Windows.Forms.Cursors.Hand;
			this.tbSanction.Divider = false;
			this.tbSanction.Dock = System.Windows.Forms.DockStyle.None;
			this.tbSanction.DropDownArrows = true;
			this.tbSanction.ImageList = this.ilSanction;
			this.tbSanction.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.tbSanction.Name = "tbSanction";
			this.tbSanction.ShowToolTips = true;
			this.tbSanction.Size = new System.Drawing.Size(120, 24);
			this.tbSanction.TabIndex = 7;
			this.tbSanction.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbSanction_ButtonClick);
			// 
			// tbbStage1
			// 
			this.tbbStage1.ImageIndex = 2;
			this.tbbStage1.Tag = "S1";
			this.tbbStage1.ToolTipText = "Stage 1";
			// 
			// tbbStage2
			// 
			this.tbbStage2.ImageIndex = 5;
			this.tbbStage2.Tag = "S2";
			this.tbbStage2.ToolTipText = "Stage 2";
			// 
			// tbbDoc
			// 
			this.tbbDoc.ImageIndex = 8;
			this.tbbDoc.Tag = "DC";
			this.tbbDoc.ToolTipText = "Document Control";
			// 
			// tbbAddData
			// 
			this.tbbAddData.ImageIndex = 14;
			this.tbbAddData.Tag = "AD";
			this.tbbAddData.ToolTipText = "Additional Data";
			// 
			// tbbReferral
			// 
			this.tbbReferral.ImageIndex = 11;
			this.tbbReferral.Tag = "R";
			this.tbbReferral.ToolTipText = "Underwriter referral";
			// 
			// ilSanction
			// 
			this.ilSanction.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.ilSanction.ImageSize = new System.Drawing.Size(14, 14);
			this.ilSanction.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilSanction.ImageStream")));
			this.ilSanction.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// SanctionStatus
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tbSanction});
			this.Name = "SanctionStatus";
			this.Size = new System.Drawing.Size(116, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private void LaunchStage1()
		{
			if(CalledFrom.Name!="SanctionStage1")
			{
				string[] parms = new String[3];
				parms[0] = this.CustomerID;
				parms[1] = this.AccountNo;
				parms[2] = this.AccountType;

            SanctionStage1 s1 = new SanctionStage1(allowConversionToHP, parms, 
														ScreenMode, 
														cf.FormRoot, 
														null, customerScreen);
				//SS1 s1 = new SS1(parms, ScreenMode);
				s1.FormRoot = cf.FormRoot;
				// do not close if called from incomplete credits
				if(CalledFrom.Name!="Incomplete")
				     cf.CloseTab();
				((MainForm)cf.FormRoot).AddTabPage(s1,18);				
			}
		}

		private void LaunchStage2()
		{
			if(CalledFrom.Name!="SanctionStage2")
			{
				SanctionStage2 s2 = new SanctionStage2(this.CustomerID,
														this.DateProp,
														this.AccountNo,
														this.AccountType,
														this.ScreenMode,
														cf.FormRoot,	
														null, customerScreen);
				s2.FormRoot = cf.FormRoot;

				if(CalledFrom.Name!="Incomplete")
            		 cf.CloseTab();

				((MainForm)cf.FormRoot).AddTabPage(s2,19);
			}
		}

		public CommonForm CalledFrom
		{
			get
			{
				return (CommonForm)((MainForm)cf.FormRoot).MainTabControl.SelectedTab.Control;
			}
		}

		private void LaunchAdditionalData()
		{
		}

		private void LaunchReferral()
		{
			if(CalledFrom.Name!="Referral")
			{
				STL.PL.Referral r = new STL.PL.Referral(true,this.CustomerID,
					this.DateProp,
					this.AccountNo,
					this.AccountType,
					this.ScreenMode,
					cf.FormRoot,
					null, customerScreen, false);
				r.FormRoot = cf.FormRoot;

				if(CalledFrom.Name!="Incomplete")
					cf.CloseTab();

				((MainForm)cf.FormRoot).AddTabPage(r,20);
			}
		}

		private void LaunchDocumentControl()
		{
			if(CalledFrom.Name!="DocumentConfirmation")
			{
				DocumentConfirmation dc = new DocumentConfirmation(this.CustomerID,
					this.DateProp,
					this.AccountNo,
					this.AccountType,
					this.ScreenMode,
					cf.FormRoot,
					null, customerScreen);
				dc.FormRoot = cf.FormRoot;

				if(CalledFrom.Name!="Incomplete")
					cf.CloseTab();

				((MainForm)cf.FormRoot).AddTabPage(dc,21);
			}
		}

		private void tbSanction_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			try
			{
				cf.Function = "tbSanction_ButtonClick";
				cf.Wait();
				
				// Evaluate the Button property to determine which button was clicked.
				switch((string)e.Button.Tag)
				{
					case SS.S1:	LaunchStage1();
						break; 
					case SS.S2:	LaunchStage2();
						break; 
					case SS.DC: LaunchDocumentControl();
						break;
					case SS.R:	LaunchReferral();
						break;
					case SS.AD: LaunchAdditionalData();
						break; 
					default:
						break;
				}
			}
			catch(Exception ex)
			{
				cf.Catch(ex, cf.Function);
			}
			finally
			{
				cf.StopWait();
			}		
		}
	}
}
