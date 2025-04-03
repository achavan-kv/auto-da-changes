using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using STL.Common.Constants.Elements;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
using STL.PL.WS7;
using STL.PL.WS5;
using STL.Common.Static;
using System.Data;
using System.Web.Services.Protocols;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.OperandTypes;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt used to view or edit scoring rules. Each rule is
	/// formed from conditional expressions using operands with comparison 
	/// operators. One or more conditional expressions can be used with 
	/// logical AND and OR operators.
	/// The rules are either scoring rules or referal rules. The scoring
	/// rules add a number of points to the customer score when a condition
	/// is met, such as a certain age range or a certain occupation.
	/// The referral rules add a referral code to the application and
	/// force it to be referred when a certain condition is met, such
	/// as a low disposable income.
	/// </summary>
	public class ScoringRule : CommonForm
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox drpOperand1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox drpOperand2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox drpOperator;
		private System.Windows.Forms.ComboBox drpResult;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.RadioButton rbAnd;
		private System.Windows.Forms.RadioButton rbOr;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TreeView tvRule;
		private XmlDocument rulesDoc = null;
		private System.Windows.Forms.TextBox txtRuleName;
		private System.Windows.Forms.Label label6;
		private DataTable operands = null;
		private DataTable operators = null;
        private DataTable equifaxOperands = null;  //Equifax Score card 
        //private string Error; // = "";
        private System.Windows.Forms.TextBox txtOperand2;
		private System.Windows.Forms.NumericUpDown noOperand2;
		string operandType = "";
		string tableName = "";
		private System.Windows.Forms.Label lOutcome;
		private System.Windows.Forms.NumericUpDown noPoints;
		private XmlNode newRule = null;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button btnSave;
		private bool readOnly = false;
		private System.Windows.Forms.CheckBox chxApplyRF;
		private System.Windows.Forms.CheckBox chxApplyHP;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.CheckBox chxReferDeclined;
		private int _ruleType;
		private System.Windows.Forms.CheckBox chxReject;
		private System.Windows.Forms.CheckBox chxReferToBureau;
		private string selectedOperand1 = "";
		public int RuleType
		{
			get{return _ruleType;}
			set{_ruleType = value;}
		}

		public ScoringRule(TranslationDummy d)
		{
			InitializeComponent();
		}

		public ScoringRule(XmlDocument doc, DataSet staticData, int ruleType, XmlNode oldRule, bool _readOnly, Form root, Form parent, bool IsEquifaxScorecard = false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			FormRoot = root;
			FormParent = parent;

			//TranslateControls();
			rulesDoc = doc;
			rbAnd.Checked = true;
			//int index = 0;
			RuleType = ruleType;
			readOnly = _readOnly;
			btnAdd.Enabled = !readOnly;
			btnSave.Enabled = !readOnly;
			txtRuleName.ReadOnly = readOnly;
			txtRuleName.BackColor = SystemColors.Window;
			drpResult.Enabled = !readOnly;
			noPoints.Enabled = !readOnly;

			//Create the new rule node
			newRule = oldRule;				

			foreach(DataTable dt in staticData.Tables)
			{
				switch(dt.TableName)
				{
					case TN.Operands:	operands = dt;
						break;
					case TN.Operators:	operators = dt;
						break;
                    case TN.EquifaxOperands:   //Equifax Score card 
                        equifaxOperands = dt;
                        break;
                    default:
						break;
				}
			}
			drpOperand1.DataSource = IsEquifaxScorecard ? equifaxOperands.DefaultView : operands.DefaultView; //Equifax Score card 
            drpOperand1.DisplayMember = CN.OperandName;
			drpOperator.DataSource = operators.DefaultView;
			drpOperator.DisplayMember = CN.Operator;

			switch(ruleType)
			{
				case 0:	lOutcome.Text = "Result";
					noPoints.Visible = false;
					break;
				case 1:	lOutcome.Text = IsEquifaxScorecard ? "Weight" : "Points"; //Equifax Score card 
                    drpResult.Visible = false;
					chxReferDeclined.Enabled = false;
					break;
				default:
					break;
			}
			drpResult.DataSource = (DataTable)StaticData.Tables[TN.ReferralCodes];
			drpResult.DisplayMember = CN.Code;
		
			txtRuleName.Text = newRule.Attributes[Tags.RuleName].Value;
			chxApplyRF.Checked = Convert.ToBoolean(newRule.Attributes[Tags.ApplyRF].Value);
			chxApplyHP.Checked = Convert.ToBoolean(newRule.Attributes[Tags.ApplyHP].Value);
			try
			{
				chxReferDeclined.Checked = Convert.ToBoolean(newRule.Attributes[Tags.ReferDeclined].Value);
				chxReject.Checked = Convert.ToBoolean(newRule.Attributes[Tags.RuleRejects].Value);
			}
			catch(NullReferenceException)	/* possible that rules may have been created before this
												 * attribute was added therefore we can't
												 * assume it will be here */
			{
				newRule.Attributes.Append(doc.CreateAttribute(Tags.ReferDeclined));
				newRule.Attributes[Tags.ReferDeclined].Value = Boolean.FalseString;
			}

			try
			{
				chxReject.Checked = Convert.ToBoolean(newRule.Attributes[Tags.RuleRejects].Value);
			}
			catch(NullReferenceException)	/* possible that rules may have been created before this
												 * attribute was added therefore we can't
												 * assume it will be here */
			{
				newRule.Attributes.Append(doc.CreateAttribute(Tags.RuleRejects));
				newRule.Attributes[Tags.RuleRejects].Value = Boolean.FalseString;
			}

			try
			{
				chxReferToBureau.Checked = Convert.ToBoolean(newRule.Attributes[Tags.ReferToBureau].Value);
			}
			catch(NullReferenceException)	/* possible that rules may have been created before this
												 * attribute was added therefore we can't
												 * assume it will be here */
			{
				newRule.Attributes.Append(doc.CreateAttribute(Tags.ReferToBureau));
				newRule.Attributes[Tags.ReferToBureau].Value = Boolean.FalseString;
			}

            //Add for edit rule for Equifax score card 
            if (IsEquifaxScorecard)
                this.noPoints.DecimalPlaces = 15;
            else
                this.noPoints.DecimalPlaces = 0;

            switch (newRule.Attributes[Tags.Type].Value)
			{
				case "S":	noPoints.Value = IsEquifaxScorecard ? Convert.ToDecimal(newRule.Attributes[Tags.Result].Value) : Convert.ToInt32(newRule.Attributes[Tags.Result].Value);  //Include Equifax score card rule
                    chxReferDeclined.Enabled = false;
					chxReject.Enabled = false;
					chxReferToBureau.Enabled = false;
					break;
				case "R":	int i = drpResult.FindString(newRule.Attributes[Tags.Result].Value);
					drpResult.SelectedIndex = i;
					break;
				default:
					break;
			}
			this.RenderTreeView();
		}

		public ScoringRule(XmlDocument doc, DataSet staticData, int ruleType, Form root, Form parent, bool IsEquifaxScore = false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			
			FormRoot = root;
			FormParent = parent;

			rulesDoc = doc;
			rbAnd.Checked = true;
			RuleType = ruleType;

			//Create the new rule node
			newRule = this.CreateRuleNode(rulesDoc, ruleType);
			
			this.RenderTreeView();

			foreach(DataTable dt in staticData.Tables)
			{
				switch(dt.TableName)
				{
					case TN.Operands:	operands = dt;
						break;
					case TN.Operators:	operators = dt;
						break;
                    case TN.EquifaxOperands:
                        equifaxOperands = dt;
                        break;
                    default:
						break;
				}
			}
			drpOperand1.DataSource = IsEquifaxScore == true ? equifaxOperands.DefaultView : operands.DefaultView;
            drpOperand1.DisplayMember = CN.OperandName;
			drpOperator.DataSource = operators.DefaultView;
			drpOperator.DisplayMember = CN.Operator;

			switch(ruleType)
			{
				case 0:	lOutcome.Text = "Result";
					noPoints.Visible = false;
					break;
				case 1:	lOutcome.Text = IsEquifaxScore == true ? "Weight" : "Points";
                    drpResult.Visible = false;
					break;
				default:
					break;
			}
			drpResult.DataSource = (DataTable)StaticData.Tables[TN.ReferralCodes];
			drpResult.DisplayMember = CN.Code;
		}

		/// <summary>
		/// this will build the tree view representation of the rule from the newRule node
		/// </summary>
		private void RenderTreeView()
		{
			tvRule.Nodes.Clear();
			tvRule.Nodes.Add(new TreeNode(Elements.Rule));
			tvRule.Nodes[0].Tag = newRule;
			TreeNode tvParent = tvRule.Nodes[0];

			RenderTreeView(newRule, tvParent);

			tvRule.ExpandAll();
			tvRule.SelectedNode = tvRule.Nodes[0];
		}

		private void RenderTreeView(XmlNode comp, TreeNode parent)
		{
			foreach(XmlNode clause in comp.ChildNodes)
			{
				if(clause.Name == Elements.Clause)
				{
					if(clause.Attributes[Tags.Type].Value == "S")
					{
						XmlNode o1 = clause.FirstChild;
						XmlNode op = o1.NextSibling;
						XmlNode o2 = op.NextSibling;
						string clauseStr = o1.Attributes[Tags.Operand].Value + " ";
						clauseStr += op.Attributes[Tags.Operator].Value + " ";
						switch(o1.Attributes[Tags.Type].Value)
						{
							case "option":	string o2Str = o2.Attributes[Tags.Operand].Value;
											string dtName = o1.Attributes[Tags.TableName].Value;
											switch(dtName)
											{
												//case TN.Title:	clauseStr += o2.Attributes[Tags.Operand].Value;
												//	break;
												case TN.Bank:	clauseStr += o2.Attributes[Tags.Operand].Value;		
													break;
												default:		((DataTable)StaticData.Tables[dtName]).DefaultView.RowFilter = CN.Code + " = '"+o2Str+"'";
													clauseStr += ((DataTable)StaticData.Tables[dtName]).DefaultView[0][CN.CodeDescription];
													((DataTable)StaticData.Tables[dtName]).DefaultView.RowFilter = "";
													break;
											}											
								break;
							default:	clauseStr += o2.Attributes[Tags.Operand].Value;
								break;
						}
						TreeNode tvClause = new TreeNode(clauseStr);
						tvClause.Tag = clause;
						parent.Nodes.Add(tvClause);						
					}
					else	//it's a composite clause
					{	
						TreeNode tvClause = new TreeNode("Composite Clause");
						tvClause.Tag = clause;
						parent.Nodes.Add(tvClause);	
						RenderTreeView(clause, tvClause);
					}					
				}
				if(clause.Name == Elements.LogicalOperator)
				{
					TreeNode tvOp = new TreeNode(clause.Attributes[Tags.Operator].Value);
					tvOp.Tag = clause;
					parent.Nodes.Add(tvOp);
				}
			}
		}

		private XmlNode CreateRuleNode(XmlDocument doc, int ruleType)
		{
			
			XmlNode node = doc.CreateElement(Elements.Rule);
			node.Attributes.Append(doc.CreateAttribute(Tags.Type));
			node.Attributes.Append(doc.CreateAttribute(Tags.Result));
			node.Attributes.Append(doc.CreateAttribute(Tags.State));
			node.Attributes.Append(doc.CreateAttribute(Tags.RuleName));
			node.Attributes.Append(doc.CreateAttribute(Tags.ApplyRF));
			node.Attributes.Append(doc.CreateAttribute(Tags.ApplyHP));
			node.Attributes.Append(doc.CreateAttribute(Tags.ReferDeclined));
			node.Attributes.Append(doc.CreateAttribute(Tags.ReferAccepted));
			node.Attributes.Append(doc.CreateAttribute(Tags.RuleRejects));
			node.Attributes.Append(doc.CreateAttribute(Tags.ReferToBureau));

			switch (ruleType)
			{
				case 0:	node.Attributes[Tags.Type].Value = "R";
					break;
				case 1: node.Attributes[Tags.Type].Value = "S";
					break;
				default:
					break;
			}
			return node;
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.noPoints = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRuleName = new System.Windows.Forms.TextBox();
            this.tvRule = new System.Windows.Forms.TreeView();
            this.drpResult = new System.Windows.Forms.ComboBox();
            this.lOutcome = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtOperand2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rbOr = new System.Windows.Forms.RadioButton();
            this.rbAnd = new System.Windows.Forms.RadioButton();
            this.btnAdd = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.drpOperator = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.drpOperand1 = new System.Windows.Forms.ComboBox();
            this.noOperand2 = new System.Windows.Forms.NumericUpDown();
            this.drpOperand2 = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chxReferToBureau = new System.Windows.Forms.CheckBox();
            this.chxReject = new System.Windows.Forms.CheckBox();
            this.chxReferDeclined = new System.Windows.Forms.CheckBox();
            this.chxApplyHP = new System.Windows.Forms.CheckBox();
            this.chxApplyRF = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.noPoints)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.noOperand2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.noPoints);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtRuleName);
            this.groupBox1.Controls.Add(this.tvRule);
            this.groupBox1.Controls.Add(this.drpResult);
            this.groupBox1.Controls.Add(this.lOutcome);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 352);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rule";
            // 
            // noPoints
            // 
            this.noPoints.Location = new System.Drawing.Point(200, 320);
            this.noPoints.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.noPoints.Minimum = new decimal(new int[] {
            20000,
            0,
            0,
            -2147483648});
            this.noPoints.Name = "noPoints";
            this.noPoints.Size = new System.Drawing.Size(80, 20);
            this.noPoints.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(8, 304);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(120, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Rule Name:";
            // 
            // txtRuleName
            // 
            this.txtRuleName.Location = new System.Drawing.Point(8, 320);
            this.txtRuleName.Name = "txtRuleName";
            this.txtRuleName.Size = new System.Drawing.Size(184, 20);
            this.txtRuleName.TabIndex = 8;
            // 
            // tvRule
            // 
            this.tvRule.FullRowSelect = true;
            this.tvRule.HideSelection = false;
            this.errorProvider1.SetIconAlignment(this.tvRule, System.Windows.Forms.ErrorIconAlignment.TopLeft);
            this.tvRule.Location = new System.Drawing.Point(8, 16);
            this.tvRule.Name = "tvRule";
            this.tvRule.ShowRootLines = false;
            this.tvRule.Size = new System.Drawing.Size(296, 280);
            this.tvRule.TabIndex = 0;
            this.tvRule.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvRule_MouseDown);
            // 
            // drpResult
            // 
            this.drpResult.Location = new System.Drawing.Point(200, 320);
            this.drpResult.Name = "drpResult";
            this.drpResult.Size = new System.Drawing.Size(104, 21);
            this.drpResult.TabIndex = 6;
            this.drpResult.SelectedIndexChanged += new System.EventHandler(this.drpResult_SelectedIndexChanged);
            // 
            // lOutcome
            // 
            this.lOutcome.Location = new System.Drawing.Point(200, 304);
            this.lOutcome.Name = "lOutcome";
            this.lOutcome.Size = new System.Drawing.Size(48, 16);
            this.lOutcome.TabIndex = 7;
            this.lOutcome.Text = "Result:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtOperand2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.rbOr);
            this.groupBox2.Controls.Add(this.rbAnd);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.drpOperator);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.drpOperand1);
            this.groupBox2.Controls.Add(this.noOperand2);
            this.groupBox2.Controls.Add(this.drpOperand2);
            this.groupBox2.Location = new System.Drawing.Point(312, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 216);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Clause";
            // 
            // txtOperand2
            // 
            this.txtOperand2.Location = new System.Drawing.Point(24, 120);
            this.txtOperand2.Name = "txtOperand2";
            this.txtOperand2.Size = new System.Drawing.Size(192, 20);
            this.txtOperand2.TabIndex = 12;
            this.txtOperand2.Text = "textBox1";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(24, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 16);
            this.label5.TabIndex = 11;
            this.label5.Text = "Logical Operator:";
            // 
            // rbOr
            // 
            this.rbOr.Location = new System.Drawing.Point(24, 192);
            this.rbOr.Name = "rbOr";
            this.rbOr.Size = new System.Drawing.Size(40, 16);
            this.rbOr.TabIndex = 10;
            this.rbOr.Text = "OR";
            // 
            // rbAnd
            // 
            this.rbAnd.Location = new System.Drawing.Point(24, 176);
            this.rbAnd.Name = "rbAnd";
            this.rbAnd.Size = new System.Drawing.Size(48, 16);
            this.rbAnd.TabIndex = 9;
            this.rbAnd.Text = "AND";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(152, 176);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(56, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(24, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Comparison Operator:";
            // 
            // drpOperator
            // 
            this.drpOperator.Location = new System.Drawing.Point(24, 72);
            this.drpOperator.Name = "drpOperator";
            this.drpOperator.Size = new System.Drawing.Size(104, 21);
            this.drpOperator.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(24, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Operand2:";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(24, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Operand1:";
            // 
            // drpOperand1
            // 
            this.drpOperand1.Location = new System.Drawing.Point(24, 32);
            this.drpOperand1.Name = "drpOperand1";
            this.drpOperand1.Size = new System.Drawing.Size(216, 21);
            this.drpOperand1.TabIndex = 0;
            this.drpOperand1.SelectedIndexChanged += new System.EventHandler(this.drpOperand1_SelectedIndexChanged);
            // 
            // noOperand2
            // 
            this.noOperand2.Location = new System.Drawing.Point(24, 120);
            this.noOperand2.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.noOperand2.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.noOperand2.Name = "noOperand2";
            this.noOperand2.Size = new System.Drawing.Size(80, 20);
            this.noOperand2.TabIndex = 13;
            // 
            // drpOperand2
            // 
            this.drpOperand2.Location = new System.Drawing.Point(24, 120);
            this.drpOperand2.Name = "drpOperand2";
            this.drpOperand2.Size = new System.Drawing.Size(192, 21);
            this.drpOperand2.TabIndex = 2;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            this.errorProvider1.DataMember = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chxReferToBureau);
            this.groupBox3.Controls.Add(this.chxReject);
            this.groupBox3.Controls.Add(this.chxReferDeclined);
            this.groupBox3.Controls.Add(this.chxApplyHP);
            this.groupBox3.Controls.Add(this.chxApplyRF);
            this.groupBox3.Controls.Add(this.btnSave);
            this.groupBox3.Location = new System.Drawing.Point(312, 208);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 144);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // chxReferToBureau
            // 
            this.chxReferToBureau.Location = new System.Drawing.Point(24, 112);
            this.chxReferToBureau.Name = "chxReferToBureau";
            this.chxReferToBureau.Size = new System.Drawing.Size(152, 24);
            this.chxReferToBureau.TabIndex = 5;
            this.chxReferToBureau.Text = "Rule refers to bureau";
            // 
            // chxReject
            // 
            this.chxReject.Location = new System.Drawing.Point(24, 88);
            this.chxReject.Name = "chxReject";
            this.chxReject.Size = new System.Drawing.Size(184, 24);
            this.chxReject.TabIndex = 4;
            this.chxReject.Text = "Rule rejects proposal";
            // 
            // chxReferDeclined
            // 
            this.chxReferDeclined.Location = new System.Drawing.Point(24, 64);
            this.chxReferDeclined.Name = "chxReferDeclined";
            this.chxReferDeclined.Size = new System.Drawing.Size(184, 24);
            this.chxReferDeclined.TabIndex = 3;
            this.chxReferDeclined.Text = "Rule refers declined proposals";
            // 
            // chxApplyHP
            // 
            this.chxApplyHP.Checked = true;
            this.chxApplyHP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxApplyHP.Location = new System.Drawing.Point(24, 40);
            this.chxApplyHP.Name = "chxApplyHP";
            this.chxApplyHP.Size = new System.Drawing.Size(104, 24);
            this.chxApplyHP.TabIndex = 2;
            this.chxApplyHP.Text = "Apply to HP";
            // 
            // chxApplyRF
            // 
            this.chxApplyRF.Checked = true;
            this.chxApplyRF.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chxApplyRF.Location = new System.Drawing.Point(24, 16);
            this.chxApplyRF.Name = "chxApplyRF";
            this.chxApplyRF.Size = new System.Drawing.Size(104, 24);
            this.chxApplyRF.TabIndex = 1;
            this.chxApplyRF.Text = "Apply to RF";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(152, 24);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save Rule";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ScoringRule
            // 
            this.ClientSize = new System.Drawing.Size(647, 357);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ScoringRule";
            this.Text = "Rule";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ScoringRule_Closing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.noPoints)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.noOperand2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				Function = "btnAdd_Click";
				Wait();

				//What are we adding to?
				XmlNode selected = (XmlNode)tvRule.SelectedNode.Tag;
				switch(selected.Name)
				{
					case Elements.Rule:	AttachClauseToComposite(selected);
						break;
					case Elements.Clause:	switch(selected.Attributes[Tags.Type].Value)
											{
												case "S":	AttachClauseToClause(selected);
													break;
												case "C":	AttachClauseToComposite(selected);
													break;
												default:
													break;
											}
						break;
					default:
						break;
				}

				this.RenderTreeView();
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

		private void AttachClauseToComposite(XmlNode comp)
		{
			//only two clauses can be added to a rule. If more are
			//required then one or both of them can be composite
			//clauses
			if(comp.HasChildNodes && comp.ChildNodes.Count > 1)
				ShowInfo("M_MAXCLAUSES");
			else
			{	//create the clause
				XmlNode clause = CreateSimpleClause(rulesDoc);

				if(comp.ChildNodes.Count==1)
				{
					XmlNode lo = this.CreateOperatorNode(rulesDoc, "L");
					comp.AppendChild(lo);
				}
				comp.AppendChild(clause);
			}
		}

		private void AttachClauseToClause(XmlNode selected)
		{
			//this must replace the node we're attaching to with a 
			//composite clause node and attach the existing clause and 
			//the new clause via the selected logical operator to the 
			//child list

			//create new clause
			XmlNode clause = this.CreateSimpleClause(rulesDoc);

			//create composite clause
			XmlNode comp = this.CreateClauseNode(rulesDoc, "C");

			//replace selected clause with composite clause
			selected.ParentNode.ReplaceChild(comp, selected);

			//add the selected node to the composite node
			comp.AppendChild(selected);

			//add the operator node
			comp.AppendChild(this.CreateOperatorNode(rulesDoc, "L"));
			
			//add the new clause
			comp.AppendChild(clause);
		}

		private XmlNode CreateSimpleClause(XmlDocument doc)
		{
			XmlNode clause = this.CreateClauseNode(doc, "S");
			XmlNode o1 = this.CreateOperandNode(doc, 1);
			XmlNode o2 = this.CreateOperandNode(doc, 2);
			XmlNode op = this.CreateOperatorNode(doc, "C");
			clause.AppendChild(o1);
			clause.AppendChild(op);
			clause.AppendChild(o2);
			return clause;
		}

		private XmlNode CreateOperandNode(XmlDocument doc, int num)
		{
			XmlNode node = null;
			switch(num)
			{
				case 1: node = doc.CreateElement(Elements.Operand1);
						node.Attributes.Append(doc.CreateAttribute(Tags.Operand));
						node.Attributes[Tags.Operand].Value = (string)((DataRowView)drpOperand1.SelectedItem)[CN.OperandName];
						node.Attributes.Append(doc.CreateAttribute(Tags.Type));
						node.Attributes[Tags.Type].Value = operandType;
						node.Attributes.Append(doc.CreateAttribute(Tags.TableName));
						node.Attributes[Tags.TableName].Value = (string)((DataRowView)drpOperand1.SelectedItem)[CN.DropDownName];
					break;
				case 2: node = doc.CreateElement(Elements.Operand2);
						node.Attributes.Append(doc.CreateAttribute(Tags.Operand));
						node.Attributes.Append(doc.CreateAttribute(Tags.Type));
						node.Attributes[Tags.Type].Value = operandType;
						switch(operandType)
						{
							case OT.FreeText:	
											{   txtOperand2.Text =txtOperand2.Text.ToUpper(); //force uppercase
												node.Attributes[Tags.Operand].Value = txtOperand2.Text;
											}

								break;
							case OT.Optional:	switch(tableName)
												{
													case TN.Bank: node.Attributes[Tags.Operand].Value = (string)((DataRowView)drpOperand2.SelectedItem)[CN.BankName];
														break;
													//case TN.Title: node.Attributes[Tags.Operand].Value = (string)((DataRowView)drpOperand2.SelectedItem)["title"];
													//	break;
													default:	node.Attributes[Tags.Operand].Value = (string)((DataRowView)drpOperand2.SelectedItem)[CN.Code];
														break;
												}
								break;
							default:	node.Attributes[Tags.Operand].Value = noOperand2.Value.ToString();
								break;
						}
					break;
				default:
					break;
			}
			return node;
		}

		private XmlNode CreateOperatorNode(XmlDocument doc, string type)
		{
			//type can either be C for conditional or L for logical
			XmlNode node = null;
			switch(type)
			{
				case "C":	node = doc.CreateElement(Elements.ComparisonOperator);
							node.Attributes.Append(doc.CreateAttribute(Tags.Operator));
							node.Attributes[Tags.Operator].Value = (string)((DataRowView)drpOperator.SelectedItem)[CN.Operator];
					break;
				case "L":	node = doc.CreateElement(Elements.LogicalOperator);
							node.Attributes.Append(doc.CreateAttribute(Tags.Operator));
							node.Attributes[Tags.Operator].Value = rbAnd.Checked==true?"AND":"OR";
					break;
			}
			return node;
		}

		private XmlNode CreateClauseNode(XmlDocument doc, string type)
		{
			XmlNode node = doc.CreateElement(Elements.Clause);
			node.Attributes.Append(doc.CreateAttribute(Tags.Type));
			node.Attributes.Append(doc.CreateAttribute(Tags.State));
			node.Attributes[Tags.Type].Value = type;
			return node;
		}

		private void tvRule_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				Function = "tvRule_MouseDown";
				TreeView ctl = (TreeView)sender;
				tvRule.SelectedNode = tvRule.GetNodeAt (e.X ,e.Y ); 

				if(e.Button == MouseButtons.Right && !readOnly)  
				{ 					
					MenuCommand m1 = new MenuCommand(GetResource("P_REMOVE"));
					m1.Click += new System.EventHandler(this.OnRemoveNode);
					PopupMenu popup = new PopupMenu();
					popup.Animate = Animate.Yes;
					popup.AnimateStyle = Animation.SlideHorVerPositive;
					popup.MenuCommands.Add(m1);
					MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
				} 
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void OnRemoveNode(object sender, System.EventArgs e)
		{
			try
			{
				Function = "OnRemoveNode";

				//What has been selected?
				XmlNode selected = (XmlNode)tvRule.SelectedNode.Tag;
				XmlNode parent = selected.ParentNode;

				//Make sure we only delete clauses
				if(selected.Name!=Elements.Rule	&&
					selected.Name!=Elements.LogicalOperator)
				{
					if(selected.Attributes[Tags.Type].Value == "S")
					{
						/*	if it's a simple clause we need to delete it.
							if there is a sibling, the logical operator clause
							must be deleted. If the parent of the sibling is 
							a clause then that clause should be 
							replaced with the reamining sibling.	*/
						if(selected.NextSibling!=null)
							parent.RemoveChild(selected.NextSibling);

						if(selected.PreviousSibling!=null)
							parent.RemoveChild(selected.PreviousSibling);

						parent.RemoveChild(selected);
						
						if(parent.Name == Elements.Clause)
						{
							parent.ParentNode.ReplaceChild(parent.FirstChild, parent);
						}						
					}
					else
						parent.RemoveChild(selected);
				}
				this.RenderTreeView();				
			}
			catch(Exception ex)
			{
				Catch(ex, Function);
			}
		}

		private void ScoringRule_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			
		}

		private void drpOperand1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{  
				Function = "drpOperand1_SelectedIndexChanged";

				if(drpOperand1.Text != selectedOperand1)
				{
					selectedOperand1 = drpOperand1.Text;

					Wait();
					//filling txtRuleName with details of drop-down
					int k = txtRuleName.Text.Length-1;
					if (txtRuleName.Text.Length==0 || (string)txtRuleName.Text.Substring(k,1)==":"  )
					{//only putting in maximum of 19 characters before:
						int j = 0;
						if (drpOperand1.Text.Length <20)
							j =drpOperand1.Text.Length;
						else
							j = 19;
					
						txtRuleName.Text=drpOperand1.Text;
						txtRuleName.Text=txtRuleName.Text.Substring(0,j) + ":";
					}

					operandType  = (string)((DataRowView)drpOperand1.SelectedItem)[CN.OperandType];
					short operandOptions = (short)((DataRowView)drpOperand1.SelectedItem)[CN.OperandOptions];
					tableName = (string)((DataRowView)drpOperand1.SelectedItem)[CN.DropDownName];
					operators.DefaultView.RowFilter = CN.OperandType+" = '" + operandType + "'";

					switch(operandType)
					{
						case OT.Optional:	ShowDropDown(tableName);
							break;
						case OT.FreeText:	ShowFreeText();
							break;
						case OT.Numeric:	ShowNumeric(0);
							break;
						case OT.Decimal:	ShowNumeric(Convert.ToInt32(DecimalPlaces.Substring(1,1)));
							break;
						default:
							break;
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

		private void ShowDropDown(string tableName)
		{
			//Populate the drop down list for the second operand based on 
			//which first operand was selected
			drpOperand2.Visible = true;
			txtOperand2.Visible = false;
			noOperand2.Visible = false;
			drpOperand2.Text = "";
			
		
			switch(tableName)
			{
				case TN.Bank:	drpOperand2.DataSource = (DataTable)StaticData.Tables[tableName];
								drpOperand2.DisplayMember = CN.BankName;
					break;
				default:		DataTable dt = (DataTable)StaticData.Tables[tableName];
								if(dt.Columns["Display"]==null)
								{
									dt.Columns.Add(new DataColumn("Display"));
									foreach(DataRow r in dt.Rows)
										if((string)r[CN.CodeDescription]!="")
											r["Display"] = (string)r[CN.CodeDescription] + " - " + (string)r[CN.Code];
								}
								drpOperand2.DataSource = dt;
								drpOperand2.DisplayMember = "Display";
					break;
			}
		}

		private void ShowFreeText()
		{
			drpOperand2.Visible = false;
			txtOperand2.Visible = true;
			noOperand2.Visible = false;
			txtOperand2.Text = "";

		}

		private void ShowNumeric(int decimalPlaces)
		{
			drpOperand2.Visible = false;
			txtOperand2.Visible = false;
			noOperand2.Visible = true;
			noOperand2.DecimalPlaces = decimalPlaces;
			noOperand2.Value = 0;
		}

		private void drpResult_SelectedIndexChanged(object sender, System.EventArgs e)
		{

		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			bool status = true;
			if(txtRuleName.Text.Length==0)
			{
				status = false;
				errorProvider1.SetError(txtRuleName, GetResource("M_ENTERMANDATORY"));
			}
			else
				errorProvider1.SetError(txtRuleName, "");

			if(!newRule.HasChildNodes)
			{
				status = false;
				errorProvider1.SetError(tvRule, GetResource("M_NOCLAUSES"));
			}
			else
				errorProvider1.SetError(tvRule, "");

			if(status)
			{
				string code = (string)((DataRowView)drpResult.SelectedItem)[CN.Code];
				string type = newRule.Attributes[Tags.Type].Value;
				switch(type)
				{
					case "R": 	newRule.Attributes[Tags.Result].Value = code;
						break;
					case "S":	newRule.Attributes[Tags.Result].Value = noPoints.Value.ToString();
						break;
					default:
						break;
				}
				newRule.Attributes[Tags.RuleName].Value = txtRuleName.Text;
				newRule.Attributes[Tags.ApplyRF].Value = chxApplyRF.Checked.ToString();
				newRule.Attributes[Tags.ApplyHP].Value = chxApplyHP.Checked.ToString();
				newRule.Attributes[Tags.ReferDeclined].Value = chxReferDeclined.Checked.ToString();
				newRule.Attributes[Tags.RuleRejects].Value = chxReject.Checked.ToString();
				newRule.Attributes[Tags.ReferToBureau].Value = chxReferToBureau.Checked.ToString();
				rulesDoc.DocumentElement.AppendChild(newRule);

				newRule=null;
				newRule = this.CreateRuleNode(rulesDoc, RuleType);			
				this.RenderTreeView();
			}			
		}
	}
}
