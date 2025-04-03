using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Web.Services.Protocols;
using System.Xml;
using System.Collections.Specialized;
using System.Reflection;
using mshtml;
using System.Threading;
using STL.PL.Collections.CollectionsClasses;
using STL.PL.WS2;
using STL.PL.WS5;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.EOD;
using STL.Common.Collections;
using System.Text;
using Crownwood.Magic.Menus;
using Blue.Cosacs.Shared;


namespace STL.PL
{
    public partial class StrategyConfiguration : CommonForm
    {

        private DataTable dtPossibleEntryConditions = new DataTable();
        private DataTable dtChosenEntryConditions = new DataTable();
        private DataTable dtPossibleExitConditions = new DataTable();
        private DataTable dtChosenExitConditions = new DataTable();
        private DataTable dtPossibleStepConditions = new DataTable();
        private DataTable dtChosenStepConditions = new DataTable();
        private DataTable dtPossibleActions = new DataTable();
        private DataTable dtChosenActions = new DataTable();
        private DataTable dtStrategies = new DataTable();
        private DataTable dtStrategy = new DataTable();
        StrategyConfigPopulation stratConfig = new StrategyConfigPopulation();
        Validation validator = new Validation();
        Operands operands = new Operands();
        private ArrayList entryOrLetters = new ArrayList();
        //private ArrayList exitOrLetters = new ArrayList();
        private string m_readOnly;
        private string m_isActive;
        private bool m_added;
        private bool m_populatedEntries = false;
        private bool m_populatedExits = false;
        private bool m_populatedSteps = false;
        private bool m_populatedActions = false;

        private bool m_IsWorkListEdit = false; //NM part of CR976 change
                
        public StrategyConfiguration(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            dynamicMenus = new Hashtable();
            HashMenus();
            ApplyRoleRestrictions();
        }

        private void HashMenus()
        {
          dynamicMenus[this.Name + ":activate"] = this.activate;
          dynamicMenus[this.Name + ":btnSave"] = this.btnSave;
          dynamicMenus[this.Name + ":CreateNew"] = this.CreateNew;
          dynamicMenus[this.Name + ":CreateNew"] = this.CreateNew;
          dynamicMenus[this.Name + ":saveAs"] = this.saveAs;
          dynamicMenus[this.Name + ":btnDelete"] = this.btnDelete; //IP - UAT(514) - Added 'Delete' button to delete existing strategies.
        }

        private enum Operator
        {
          GreaterThan, LessThan, Equals, Between, GreaterThanEqual, LessThanEqual, NotEqualTo
        }

        #region --properties ----------------------------------------------------

        private string readOnly
        {
          get
          {
             return m_readOnly;
          }
          set
          {
             m_readOnly = value;
          }
        }

        private string isActive
        {
          get
          {
             return m_isActive;
          }
          set
          {
             m_isActive = value;
          }
        }

        private bool added
        {
          get
          {
             return m_added;
          }
          set
          {
             m_added = value;
          }
        }

        private bool populatedEntries
        {
          get
          {
             return m_populatedEntries;
          }
          set
          {
             m_populatedEntries = value;
          }
        }

        private bool populatedExits
        {
          get
          {
             return m_populatedExits;
          }
          set
          {
             m_populatedExits = value;
          }
        }

        private bool populatedSteps
        {
          get
          {
             return m_populatedSteps;
          }
          set
          {
             m_populatedSteps = value;
          }
        }

        private bool populatedActions
        {
          get
          {
             return m_populatedActions;
          }
          set
          {
             m_populatedActions = value;
          }
        }

        #endregion --------------------------------------------------------------

        private void tabControlStrategies_SelectionChanged(object sender, EventArgs e)
        {
           
          if (Strategies.Enabled)
          {
             LoadConditions(Strategies.SelectedValue.ToString());
          }
          else
          {
             LoadConditions(String.Empty);
          }

          if (activate.Text == ActivateName.Activate || readOnly == "1")
          {
             SetReadOnlyFields(false);
          }
          else
          {
             SetReadOnlyFields(true);
          }
        }

        private void StrategyConfiguration_Load(object sender, EventArgs e)
        {
            try
            {
                LoadStrategies();

                //Load conditions for default tab selected - tabPageEntryConditions
                LoadConditions(Strategies.SelectedValue.ToString());

                EntryConditionsOperators.SelectedItem = DropDownValues.Operator;
                ExitConditionsOperators.SelectedItem = DropDownValues.Operator;
                OperatorsOnSteps.SelectedItem = DropDownValues.Operator;
                previousStrategies.SelectedValue = DropDownValues.PreviousStrategy;

                tabPageEntryConditions.Selected = true;

                //---NM 20/01/2009 --------CR 976 -----------------------------
                InitWorkListTab();
                InitSortOrderTab();
                //-------------------------------------------------------------
            }
            catch (Exception ex)
            {
                Catch(ex, "StrategyConfiguration_Load");
            }
        }
               
        private void LoadStrategies()
        {
            dtStrategies = stratConfig.GetStrategies();
            dtStrategy = stratConfig.GetStrategies();

            Strategies.DataSource = dtStrategies;
            Strategies.DisplayMember = CN.Description;
            Strategies.ValueMember = CN.Strategy;
            // Entry Strategy dropdown  UAT755
            drpStrategy.DataSource = dtStrategy;
            drpStrategy.DisplayMember = CN.Description;
            drpStrategy.ValueMember = CN.Strategy;
            //cbManual.Enabled = false;       //UAT856 //IP - 21/09/09 - Checkbox should never be disabled as user should be allowed to update.
            cbManual.Checked = Convert.ToBoolean(dtStrategies.Rows[Strategies.SelectedIndex][CN.Manual]);  //uat856 jec 18/09/09
            
            //If strategy is pre-created then do not allow add/remove buttons or save buttons to be enabled
            readOnly = dtStrategies.Rows[0][CN.ReadOnly].ToString();
            if (readOnly == "1")
            {
             SetReadOnlyFields(false);   
            }

            isActive = dtStrategies.Rows[0][CN.IsActive].ToString();
            NameActivateButton();

            //Load Letters comboBox
            LoadStrategiesForLetters();

            //Load Exit Strategies
            LoadExitStrategies(Strategies.SelectedValue.ToString());

            //IP - 02/06/09 - Credit Collection Walkthrough Changes 
            //Allocation Checkbox check.
            StrategyAllocationCheck();

            drpStrategy.Visible = false;        // #9589
            lbStrategy.Visible = false;

        }

        private void LoadExitStrategies(string exit)
        {
            DataTable dtExitStrategies = new DataTable();
            dtExitStrategies = dtStrategies.Copy();
            //Filter dtStrategies removing the selected strategy
            DataView dvStrategy = new DataView(dtExitStrategies);
            dvStrategy.RowFilter = "Strategy <> '" + exit + "'";
            dtExitStrategies = dvStrategy.ToTable();

            DataRow drExitStrategies = dtExitStrategies.NewRow();
            drExitStrategies[CN.Strategy] = DropDownValues.ExitStrategy;
            drExitStrategies[CN.Description] = DropDownValues.ExitStrategy;
            dtExitStrategies.Rows.InsertAt(drExitStrategies,0);

            exitStrategy.DataSource = dtExitStrategies;
            exitStrategy.DisplayMember = CN.Description;
            exitStrategy.ValueMember = CN.Strategy;

            //Do the same for the previous strategies drop down
            DataTable dtPreviousStrategies = new DataTable();
            dtPreviousStrategies = dtStrategies.Copy();
            //Filter dtStrategies removing the selected strategy
            DataView dvPrevStrategy = new DataView(dtPreviousStrategies);
            dvPrevStrategy.RowFilter = "Strategy <> '" + exit + "'";
            dtPreviousStrategies = dvPrevStrategy.ToTable();

            DataRow drPreviousStrategies = dtPreviousStrategies.NewRow();
            drPreviousStrategies[CN.Strategy] = DropDownValues.PreviousStrategy;
            drPreviousStrategies[CN.Description] = DropDownValues.PreviousStrategy;
            dtPreviousStrategies.Rows.InsertAt(drPreviousStrategies, 0);

            previousStrategies.DataSource = dtPreviousStrategies;
            previousStrategies.DisplayMember = CN.Description;
            previousStrategies.ValueMember = CN.Strategy;
        }

        private void NameActivateButton()
        {
            if (isActive == "1")
            {
                activate.Text = ActivateName.Deactivate;
            }
            else
            {
                activate.Text = ActivateName.Activate;
                SetReadOnlyFields(false);
            }
        }
              
        private void SetReadOnlyFields(bool result)
        {
            AddEntryConditions.Enabled = result;
            AddExitConditions.Enabled = result;
            AddSteps.Enabled = result;
            addActions.Enabled = result;

            RemoveEntryConditions.Enabled = result;
            RemoveExitConditions.Enabled = result;
            RemoveSteps.Enabled = result;
            removeActions.Enabled = result;

            btnSave.Enabled = result;

            EntryConditionsOr.Enabled = result;

            exitStrategy.Enabled = result;
        }

        private void LoadStrategiesForLetters()
        {
            Letters.DataSource = dtStrategies.Copy();
            Letters.DisplayMember = CN.Description;
            Letters.ValueMember = CN.Strategy;
        }

        private void LoadConditions(string strategy)
        {
            try
            {
                //IP - 02/12/09 - UAT5.2 (929)
                RemoveErrorProvider(AddEntryConditions);
                RemoveErrorProvider(AddSteps);
                RemoveErrorProvider(AddExitConditions);

                StrategyConfigPopulation stratConfig = new StrategyConfigPopulation();
                string tabSelected = tabControlStrategies.SelectedTab.Name;

                const string  tabSelectedEntryConditions = "tabPageEntryConditions";
                const string tabSelectedExitConditions = "tabPageExitConditions";
                const string tabSelectedSteps = "tabPageSteps";
                const string tabSelectectedActions = "tabPageActions";

                foreach(DataRow strategyRow in dtStrategies.Rows)
                {
                    if (strategyRow[CN.Strategy].ToString() == strategy)
                    {
                        readOnly = strategyRow[CN.ReadOnly].ToString();
                        isActive = strategyRow[CN.IsActive].ToString();
                        if (readOnly == "1" || isActive == "0")
                        {
                            SetReadOnlyFields(false);
                        }
                        else
                        {
                            SetReadOnlyFields(true);
                        }
                    }
                }

                if (populatedEntries == false)
                {
                    dtPossibleEntryConditions = stratConfig.GetConditions(tabSelectedEntryConditions);
                    dtChosenEntryConditions = stratConfig.GetStrategyConditions(strategy, tabSelectedEntryConditions);
                    dtChosenEntryConditions.TableName = TN.ChosenEntryConditions;

                    dtPossibleEntryConditions = stratConfig.FilterPossibleConditions(dtChosenEntryConditions, dtPossibleEntryConditions, CN.ConditionCode);
                    populatedEntries = true;
                }
                if (populatedExits == false)
                {
                    dtChosenExitConditions = stratConfig.GetStrategyConditions(strategy, tabSelectedExitConditions);
                    dtChosenExitConditions.TableName = TN.ChosenExitConditions;
                    dtPossibleExitConditions = stratConfig.GetConditions(tabSelectedExitConditions);

                    dtPossibleExitConditions = stratConfig.FilterPossibleConditions(dtChosenExitConditions, dtPossibleExitConditions, CN.ConditionCode);
                    populatedExits = true;
                }
                if (populatedSteps == false)
                {
                    dtChosenStepConditions = stratConfig.GetStrategyConditions(strategy, tabSelectedSteps);
                    dtChosenStepConditions.TableName = TN.ChosenSteps;
                    dtPossibleStepConditions = stratConfig.GetConditions(tabSelectedSteps);
                    dtPossibleStepConditions = stratConfig.FilterPossibleConditions(dtChosenStepConditions, dtPossibleStepConditions, CN.ConditionCode);
                    populatedSteps = true;
                }
                if (populatedActions == false)
                {
                    dtPossibleActions = stratConfig.GetActions();
                    dtChosenActions = stratConfig.GetStrategyActions(strategy);
                    dtChosenActions.TableName = TN.ChosenActions;

                    dtPossibleActions = stratConfig.FilterPossibleConditions(dtChosenActions, dtPossibleActions, CN.ActionCode);
                    populatedActions = true;
                }

                switch (tabSelected)
                {
                    case (tabSelectedEntryConditions):

                    foreach (DataRow row in dtChosenEntryConditions.Rows)
                    {
                        if (row[CN.Condition].ToString().Contains(" X"))
                        {
                            row[CN.Condition] = row[CN.Condition].ToString().Replace(" X", " " + row[CN.Operator1].ToString());
                        }
                        if (row[CN.Condition].ToString().Contains(" Y"))
                        {
                            row[CN.Condition] = row[CN.Condition].ToString().Replace(" Y", " " + row[CN.Operator2].ToString());
                        }
                    }
                    DataGridPossibleEntryConditions.DataSource = dtPossibleEntryConditions;
                    DataGridPossibleEntryConditions.Columns[CN.QualifyingCode].Visible = false;
                    DataGridPossibleEntryConditions.Columns[CN.OperandAllowable].Visible = false;
                    DataGridPossibleEntryConditions.Columns[CN.Type].Visible = false;
                    DataGridPossibleEntryConditions.Columns[CN.FalseStep].Visible = false;
                    DataGridPossibleEntryConditions.Columns[CN.ConditionCode].Visible = false;
                    DataGridPossibleEntryConditions.Columns[CN.AllowReuse].Visible = false;
                    DataGridPossibleEntryConditions.Columns[CN.Condition].Width = 653;

                    dataGridChosenEntryConditions.DataSource = dtChosenEntryConditions;
                    // Disable sorting for the DataGridView.
                    foreach (DataGridViewColumn col in dataGridChosenEntryConditions.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }

                    dataGridChosenEntryConditions.Columns[CN.Strategy].Visible = false;
                    //dataGridChosenEntryConditions.Columns[CN.Operand].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.Operator1].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.Operator2].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.Operator1].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.NextStepTrue].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.NextStepFalse].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.StepActionType].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.ActionCode].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.Type].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.Step].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.ConditionCode].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.FalseStep].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.AllowReuse].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.SavedType].Visible = false;
                    dataGridChosenEntryConditions.Columns[CN.Condition].Width = 550;
                    dataGridChosenEntryConditions.Columns[CN.Operand].HeaderText = "Selected Conditions";
                    dataGridChosenEntryConditions.Columns[CN.Operand].Width = 60;
                    dataGridChosenEntryConditions.Columns[CN.Operand].HeaderText = "Operator";
                    dataGridChosenEntryConditions.Columns[CN.OrClause].HeaderText = "Or";
                    dataGridChosenEntryConditions.Columns[CN.OrClause].Width = 30;
                    // FA 14/12/09 - made dynamic width and height work all the times...
                    if (dataGridChosenEntryConditions.RowCount <= 5)
                    {
                        dataGridChosenEntryConditions.Height = (dataGridChosenEntryConditions.RowCount * 22) + 25;
                        dataGridChosenEntryConditions.Width = 683;
                    }
                    else
                    {
                        dataGridChosenEntryConditions.Height = 138;
                        dataGridChosenEntryConditions.Width = 700;
                    }

                    //Populate ArrayLists with OrClause letters for selected strategy
                    entryOrLetters = stratConfig.CreateLettersArray(dtChosenEntryConditions);

                    DataTable dtCopy = new DataTable();
                    dtCopy = dtChosenEntryConditions.Copy();

                    for (int i = 0; i < entryOrLetters.Count; i++)
                    {
                        Color color = new Color();

                        color = stratConfig.RandomColourGenerator(i);

                        for (int j = 0; j < dtChosenEntryConditions.Rows.Count; j++)
                        {
                            if (dtChosenEntryConditions.Rows[j][CN.OrClause].ToString() == entryOrLetters[i].ToString())
                            {
                                dataGridChosenEntryConditions[CN.OrClause, j].Style.BackColor = color;
                                dataGridChosenEntryConditions[CN.OrClause, j].Value = String.Empty;
                            }
                        }
                    }

                    dtChosenEntryConditions = dtCopy.Copy();
                    dtCopy.Dispose();
                    dtCopy = null;
                    GC.Collect();

                    ShowDataGridsUnselected(DataGridPossibleEntryConditions);
                    ShowDataGridsUnselected(dataGridChosenEntryConditions);

                    break;

                    case (tabSelectedExitConditions):


                    foreach (DataRow row in dtChosenExitConditions.Rows)
                    {
                        if (row[CN.Condition].ToString().Contains(" X"))
                        {
                            row[CN.Condition] = row[CN.Condition].ToString().Replace(" X", " " + row[CN.Operator1].ToString());
                        }
                        if (row[CN.Condition].ToString().Contains(" Y"))
                        {
                            row[CN.Condition] = row[CN.Condition].ToString().Replace(" Y", " " + row[CN.Operator2].ToString());
                        }
                    }
                    dataGridPossibleExitConditions.DataSource = dtPossibleExitConditions;
                    dataGridPossibleExitConditions.Columns[CN.QualifyingCode].Visible = false;
                    dataGridPossibleExitConditions.Columns[CN.OperandAllowable].Visible = false;
                    dataGridPossibleExitConditions.Columns[CN.Type].Visible = false;
                    dataGridPossibleExitConditions.Columns[CN.FalseStep].Visible = false;
                    dataGridPossibleExitConditions.Columns[CN.ConditionCode].Visible = false;
                    dataGridPossibleExitConditions.Columns[CN.AllowReuse].Visible = false;
                    dataGridPossibleExitConditions.Columns[CN.Condition].Width = 653;

                    dataGridChosenExitConditions.DataSource = dtChosenExitConditions;
                    // Disable sorting for the DataGridView.
                    foreach (DataGridViewColumn col in dataGridChosenExitConditions.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridChosenExitConditions.Columns[CN.Strategy].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.Operator1].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.Operator2].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.Operator1].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.NextStepTrue].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.NextStepFalse].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.StepActionType].Visible = false;
                    //dataGridChosenExitConditions.Columns[CN.ActionCode].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.Type].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.Step].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.ConditionCode].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.FalseStep].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.AllowReuse].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.SavedType].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.OrClause].Visible = false;
                    dataGridChosenExitConditions.Columns[CN.Condition].Width = 500;
                    dataGridChosenExitConditions.Columns[CN.Condition].HeaderText = "Selected Conditions";
                    dataGridChosenExitConditions.Columns[CN.Operand].HeaderText = "Operator";
                    dataGridChosenExitConditions.Columns[CN.Operand].Width = 60;
                    dataGridChosenExitConditions.Columns[CN.ActionCode].HeaderText = "Exit Strategy";
                    dataGridChosenExitConditions.Columns[CN.ActionCode].Width = 80;

                    // FA 14/12/09 - made dynamic width and height work all the times...
                    if (dataGridChosenExitConditions.RowCount <= 5)
                    {
                        dataGridChosenExitConditions.Height = (dataGridChosenExitConditions.RowCount * 22) + 25;
                        dataGridChosenExitConditions.Width = 683;
                    }
                    else
                    {
                        dataGridChosenExitConditions.Height = 138;
                        dataGridChosenExitConditions.Width = 700;
                    }

                    ShowDataGridsUnselected(dataGridPossibleExitConditions);
                    ShowDataGridsUnselected(dataGridChosenExitConditions);

                    exitStrategy.SelectedValue = DropDownValues.ExitStrategy;

                    break;

                    case (tabSelectedSteps):


                    foreach (DataRow row in dtChosenStepConditions.Rows)
                    {
                        if (row[CN.Condition].ToString().Contains(" X"))
                        {
                            row[CN.Condition] = row[CN.Condition].ToString().Replace(" X", " " + row[CN.Operator1].ToString());
                        }
                        if (row[CN.Condition].ToString().Contains(" Y"))
                        {
                            row[CN.Condition] = row[CN.Condition].ToString().Replace(" Y", " " + row[CN.Operator2].ToString());
                        }
                    }
                    dataGridChosenStepConditions.DataSource = dtChosenStepConditions;
                    // Disable sorting for the DataGridView.
                    foreach (DataGridViewColumn col in dataGridChosenStepConditions.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dataGridChosenStepConditions.Columns[CN.Strategy].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.Operator1].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.Operator2].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.OrClause].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.Operator1].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.StepActionType].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.ActionCode].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.Type].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.ConditionCode].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.FalseStep].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.AllowReuse].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.SavedType].Visible = false;
                    dataGridChosenStepConditions.Columns[CN.Condition].Width = 480;
                    dataGridChosenStepConditions.Columns[CN.Condition].HeaderText = "Selected Conditions";
                    //UAT 7 Condition cell to be read only
                    dataGridChosenStepConditions.Columns[CN.Condition].ReadOnly = true;
                    dataGridChosenStepConditions.Columns[CN.Operand].HeaderText = "Operator";
                    dataGridChosenStepConditions.Columns[CN.Operand].Width = 60;
                    //UAT 7 Operand cell to be read only
                    dataGridChosenStepConditions.Columns[CN.Operand].ReadOnly = true;
                    dataGridChosenStepConditions.Columns[CN.Step].Width = 30;
                    dataGridChosenStepConditions.Columns[CN.Step].ReadOnly = true;
                    dataGridChosenStepConditions.Columns[CN.NextStepTrue].Width = 40;
                    dataGridChosenStepConditions.Columns[CN.NextStepTrue].HeaderText = "True";

                    foreach (DataRow stepRow in dtChosenStepConditions.Rows)
                    {
                        int i = dtChosenStepConditions.Rows.IndexOf(stepRow);
                        if (stepRow[CN.ConditionCode].ToString() == "")
                        {
                            string actionType = DetermineAction(stepRow[CN.StepActionType].ToString());
                            dataGridChosenStepConditions[CN.Condition, i].Value = actionType + " " + stepRow[CN.ActionCode].ToString();
                            dtChosenStepConditions.Rows[i][CN.FalseStep] = false;
                        }
                    }

                    if (dtChosenStepConditions.Rows.Count == 0)
                    {
                        dataGridChosenStepConditions.Columns[CN.NextStepFalse].Visible = false;
                    }

                    foreach (DataRow row in dtChosenStepConditions.Rows)
                    {
                        if ((bool)row[CN.FalseStep] == true)
                        {
                            dataGridChosenStepConditions.Columns[CN.NextStepFalse].Visible = true;
                            dataGridChosenStepConditions.Columns[CN.NextStepFalse].HeaderText = "False";
                            dataGridChosenStepConditions.Columns[CN.NextStepFalse].Width = 40;
                            break;
                        }
                        else
                        {
                            dataGridChosenStepConditions.Columns[CN.NextStepFalse].Visible = false;
                        }  
                    }
                    // set false step to read only on initial load
                    SetFalseStepToReadOnly();       //UAT970 jec 19/01/10

                    dataGridChosenStepConditions.Width = 710;

                    //ShowDataGridsUnselected(dataGridStepConditions);
                    ShowDataGridsUnselected(dataGridChosenStepConditions);

                    //Populate the steps ListBox - first clearing existing data
                    StepsConditionsActions.Items.Clear();

                    StepsConditionsActions.Items.Add(Actions.SendSMS);
                    StepsConditionsActions.Items.Add(Actions.SendLetter);
                    StepsConditionsActions.Items.Add(Actions.SendToWorklist);
                    StepsConditionsActions.Items.Add(Actions.SendToStrategy);

                    foreach (DataRow row in dtPossibleStepConditions.Rows)
                    {
                        StepsConditionsActions.Items.Add(row[CN.Condition].ToString());
                    }

                    Letters.SelectedValue = "";

                    break;

                    case (tabSelectectedActions):


                    dgPossibleActions.DataSource = dtPossibleActions;
                    dgPossibleActions.Columns[CN.ActionCode].Visible = false;
                    dgPossibleActions.Columns[CN.AllowReuse].Visible = false;
                    dgPossibleActions.Columns[CN.Action].Width = 650;

                    dgChosenActions.DataSource = dtChosenActions;
                    foreach (DataGridViewColumn col in dgChosenActions.Columns)
                    {
                        col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                    dgChosenActions.Columns[CN.ActionCode].Visible = false;
                    dgChosenActions.Columns[CN.Strategy].Visible = false;
                    dgChosenActions.Columns[CN.AllowReuse].Visible = false;
                    dgChosenActions.Columns[CN.Action].Width = 650;

                    ShowDataGridsUnselected(dgPossibleActions);
                    ShowDataGridsUnselected(dgChosenActions);

                    break;
                }

                //exitOrLetters = stratConfig.CreateLettersArray(dtChosenExitConditions);
                //for (int i = 0; i < exitOrLetters.Count; i++)
                //{
                //   Color color = stratConfig.RandomColourGenerator();
                //   for (int j = 0; j<dtChosenExitConditions.Rows.Count; j++)
                //   {
                //      if (dtChosenExitConditions.Rows[j][CN.OrClause].ToString() == exitOrLetters[i].ToString())
                //      {
                //         dataGridChosenExitConditions[CN.OrClause, j].Style.BackColor = color;
                //      }
                //   }
                //}

                CreateNew.Enabled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //IP - 02/12/09 - UAT5.2 (929) - Moved code to private void DataGridPossibleEntryConditions_Click
        //private void DataGridPossibleEntryConditions_RowEnter(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
               
                ////UAT755
                //EntryConditionsOperators.Enabled = false;
                //EntryConditionsValue1.ReadOnly = true;
                //EntryConditionsValue2.ReadOnly = true;                
                //drpStrategy.Visible = false;
                //lbStrategy.Visible = false;
                ////AddEntryConditions.Enabled = true;

                ////string tabSelected = tabControlStrategies.SelectedTab.Name;
                ////string selectedCondition = string.Empty;
                ////int selectedIndex = DataGridPossibleEntryConditions.CurrentRow.Index;

                ////bool disableAddCondition = false;

                ////if (Convert.ToString(DataGridPossibleEntryConditions[CN.Type, selectedIndex].Value) == " ")
                ////{
                ////    selectedCondition = Convert.ToString(DataGridPossibleEntryConditions[CN.ConditionCode, selectedIndex].Value);
                ////    disableAddCondition = disableUsingCondition(tabSelected, selectedCondition);
                ////}

                ////IP - 02/12/09 - UAT5.2 (929) 
                ////if (disableAddCondition == false)
                ////{
                //    if (DataGridPossibleEntryConditions[CN.OperandAllowable, e.RowIndex].Value.ToString() == "1")
                //    {
                //        EntryConditionsOperators.Enabled = true;
                //        EntryConditionsValue1.ReadOnly = false;
                //        EntryConditionsValue2.ReadOnly = true;
                //    }
                //    else if (DataGridPossibleEntryConditions[CN.OperandAllowable, e.RowIndex].Value.ToString() == "2")
                //    {
                //        EntryConditionsOperators.Enabled = true;
                //        EntryConditionsValue1.ReadOnly = false;
                //        EntryConditionsValue2.ReadOnly = false;
                //    }
                //    else if (DataGridPossibleEntryConditions[CN.OperandAllowable, e.RowIndex].Value.ToString() == "P")      //UAT755
                //    {
                //        drpStrategy.Enabled = true;
                //        drpStrategy.Visible = true;
                //        lbStrategy.Visible = true;
                //    }
                //    else
                //    {
                //        EntryConditionsOperators.Enabled = false;
                //        EntryConditionsValue1.ReadOnly = true;
                //        EntryConditionsValue2.ReadOnly = true;
                //        //UAT755
                //        drpStrategy.Visible = false;
                //        lbStrategy.Visible = false;
                //    }
                ////}
                ////else //IP - 02/12/09 - UAT5.2 (929)
                ////{   
                ////    AddEntryConditions.Enabled = false;
                ////}

                //EntryConditionsValue1.Text = "";
                //EntryConditionsValue2.Text = "";

                //EntryConditionsOperators.SelectedItem = DropDownValues.Operator;
        //    }
        //    catch
        //    {
        //        //no functionality required - just prevent system failure if no rows selected
        //    }
        //}

        //IP - 02/12/09 - UAT5.2 (929) - Moved code to  private void dataGridPossibleExitConditions_Click
        //private void dataGridPossibleExitConditions_RowEnter(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        ////AddExitConditions.Enabled = true;
        //        //if (dataGridPossibleExitConditions[CN.OperandAllowable, e.RowIndex].Value.ToString() == "1")
        //        //{
        //        //    ExitConditionsOperators.Enabled = true;
        //        //    ExitConditionsV1.ReadOnly = false;
        //        //    ExitConditionsV2.ReadOnly = true;
        //        //}
        //        //else if (dataGridPossibleExitConditions[CN.OperandAllowable, e.RowIndex].Value.ToString() == "2")
        //        //{
        //        //    ExitConditionsOperators.Enabled = true;
        //        //    ExitConditionsV1.ReadOnly = false;
        //        //    ExitConditionsV2.ReadOnly = false;
        //        //}
        //        //else
        //        //{
        //        //    ExitConditionsOperators.Enabled = false;
        //        //    ExitConditionsV1.ReadOnly = true;
        //        //    ExitConditionsV2.ReadOnly = true;
        //        //}

        //        //ExitConditionsV1.Text = "";
        //        //ExitConditionsV2.Text = "";

        //        //ExitConditionsOperators.SelectedItem = DropDownValues.Operator;
        //    }
        //    catch
        //    {
        //    //no functionality required - just prevent system failure if no rows selected
        //    }
        //}
       

        private void Strategies_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Strategies_SelectionChangeCommitted(object sender, EventArgs e)
        {
            populatedEntries = false;
            populatedExits = false;
            populatedSteps = false;
            populatedActions = false;

            cbManual.Checked = Convert.ToBoolean(dtStrategies.Rows[Strategies.SelectedIndex][CN.Manual]);  //uat856 jec 18/09/09
            //Load Exit Strategies
            LoadExitStrategies(Strategies.SelectedValue.ToString());
            LoadConditions(Strategies.SelectedValue.ToString());
            NameActivateButton();

            //IP - 02/06/09 - Credit Collection Walkthrough Changes 
            //Allocation Checkbox check.
            StrategyAllocationCheck();

            drpStrategy.Visible = false;        // #9589
            lbStrategy.Visible = false;
        }

      # region Add/Remove Functions

       private void AddExitConditions_Click(object sender, EventArgs e)
       {
          Function = "AddExitConditions_Click";
          bool valid1 = true;
          bool valid2 = true;
          bool validOperator = true;
          bool validExit = true;
          try
          {
             //First validate numeric values if enabled
             if (ExitConditionsV1.ReadOnly == false)
             {
                valid1 = ValidateValue(ExitConditionsV1.Text, ExitConditionsV1);
             }

             if (ExitConditionsV2.ReadOnly == false)
             {
                valid2 = ValidateValue(ExitConditionsV2.Text, ExitConditionsV2);
             }

             if (ExitConditionsOperators.Enabled)
             {
                validOperator = ValidateOperator(ExitConditionsOperators.SelectedItem.ToString(), ExitConditionsOperators);
             }

                validExit = ValidateExitStrategy(exitStrategy.SelectedValue.ToString(), exitStrategy);

             int index = dataGridPossibleExitConditions.RowCount;

             if (index > 0 && valid1 && valid2 && validOperator && validExit)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = dataGridPossibleExitConditions.Rows[i];
                   if (dataGridPossibleExitConditions.SelectedRows.Contains(row))
                   {
                      //remove the selected row from the 'possible conditions to exit' datagrid 
                      //and put it into the 'chosen conditions to exit' datagrid
                      string operand = String.Empty;
                      if (ExitConditionsOperators.SelectedItem.ToString() != DropDownValues.Operator)
                      {
                         operand = ExitConditionsOperators.SelectedItem.ToString();
                      }
                      string sign = operands.GetOperandSign(operand);

                      DataRow rowChosenExitConditions = stratConfig.AddDataRow(dtChosenExitConditions, dtPossibleExitConditions, i, 0, ExitConditionsV1.Text, ExitConditionsV2.Text, sign, "exit", exitStrategy.SelectedValue.ToString(), String.Empty);
                      DataRow rowPossibleExitConditions = stratConfig.AddDataRow(dtChosenExitConditions, dtPossibleExitConditions, i, 1, String.Empty, String.Empty, String.Empty, "exit", exitStrategy.SelectedValue.ToString(), String.Empty);

                      dtChosenExitConditions.Rows.Add(rowChosenExitConditions);
                      //Check for AllowReuse value of true to see if condition should not be removed from PossibleConditions
                      if ((bool)rowPossibleExitConditions[CN.AllowReuse] != true)
                      {
                         dtPossibleExitConditions.Rows.Remove(rowPossibleExitConditions);
                      }
                   }
                }
             }

             dtPossibleExitConditions.AcceptChanges();
             dtChosenExitConditions.AcceptChanges();
             dataGridPossibleExitConditions.DataSource = dtPossibleExitConditions;
             // FA 14/12/09 - made dynamic width and height work all the times...
             if (dataGridChosenExitConditions.RowCount <= 5)
             {
                 dataGridChosenExitConditions.Height = (dataGridChosenExitConditions.RowCount * 22) + 25;
                 dataGridChosenExitConditions.Width = 683;
             }
             else
             {
                 dataGridChosenExitConditions.Height = 138;
                 dataGridChosenExitConditions.Width = 700;
             }

             ShowDataGridsUnselected(dataGridChosenExitConditions);
             ShowDataGridsUnselected(dataGridPossibleExitConditions);

             exitStrategy.SelectedValue = DropDownValues.ExitStrategy;
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of AddExitConditions_Click";
          }
       }

       private void RemoveExitConditions_Click(object sender, EventArgs e)
       {
          Function = "RemoveExitConditions_Click";
          try
          {
             //remove the selected row from the 'chosen conditions to enter' datagrid
             int index = dataGridChosenExitConditions.RowCount;
             if (index > 0)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = dataGridChosenExitConditions.Rows[i];
                   if (dataGridChosenExitConditions.SelectedRows.Contains(row))
                   {
                      DataRow rowChosenExitConditions = dtChosenExitConditions.Rows[i];

                      dtChosenExitConditions.Rows.Remove(rowChosenExitConditions);
                   }
                }
                dtChosenExitConditions.AcceptChanges();
                dtPossibleExitConditions = stratConfig.GetConditions("tabPageExitConditions");
                dtPossibleExitConditions = stratConfig.FilterPossibleConditions(dtChosenExitConditions, dtPossibleExitConditions, CN.ConditionCode);
                dtPossibleExitConditions.AcceptChanges();
                dataGridPossibleExitConditions.DataSource = dtPossibleExitConditions;
                
                ShowDataGridsUnselected(dataGridChosenExitConditions);
                ShowDataGridsUnselected(dataGridPossibleExitConditions);
             }

          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of RemoveExitConditions_Click";
          }
       }

       private void AddEntryConditions_Click(object sender, EventArgs e)
       {
          Function = "AddEntryConditions_Click";
          bool valid1 = true;
          bool valid2 = true;
          bool validOperator = true;
          bool validStrategy = true;
          string strategy = String.Empty;       //UAT755

          try
          {
             //First validate numeric values if enabled
             if (EntryConditionsValue1.ReadOnly == false)
             {
                valid1 = ValidateValue(EntryConditionsValue1.Text, EntryConditionsValue1);
             }

             if(EntryConditionsValue2.ReadOnly == false)
             {
                valid2 = ValidateValue(EntryConditionsValue2.Text, EntryConditionsValue2);
             }

             if(EntryConditionsOperators.Enabled)
             {
                validOperator = ValidateOperator(EntryConditionsOperators.SelectedItem.ToString(), EntryConditionsOperators);
             }
                //UAT755 validate strategy
             if (drpStrategy.Enabled)
             {
                 validStrategy = ValidatePreviousStrategy(drpStrategy.SelectedValue.ToString(), drpStrategy);
                 strategy = drpStrategy.SelectedValue.ToString();        //UAT755
             }            

             int index = DataGridPossibleEntryConditions.RowCount;

             if (index > 0 && valid1 && valid2 && validOperator && validStrategy)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = DataGridPossibleEntryConditions.Rows[i];
                   if (DataGridPossibleEntryConditions.SelectedRows.Contains(row))
                   {
                      //remove the selected row from the 'possible conditions to enter' datagrid 
                      //and put it into the 'chosen conditions to enter' datagrid
                      string operand = String.Empty;
                      if (EntryConditionsOperators.SelectedItem.ToString() != DropDownValues.Operator)
                      {
                         operand = EntryConditionsOperators.SelectedItem.ToString();
                      }

                      string sign = operands.GetOperandSign(operand);

                      DataRow rowChosenEntryConditions = stratConfig.AddDataRow(dtChosenEntryConditions, dtPossibleEntryConditions, i, 0, EntryConditionsValue1.Text, EntryConditionsValue2.Text, sign, "entry", strategy, strategy);   //UAT755
                      DataRow rowPossibleEntryConditions = stratConfig.AddDataRow(dtChosenEntryConditions, dtPossibleEntryConditions, i, 1, String.Empty, String.Empty, String.Empty, "entry", String.Empty, String.Empty); 

                      dtChosenEntryConditions.Rows.Add(rowChosenEntryConditions);
                      //Check for AllowReuse value of true to see if condition should not be removed from PossibleConditions
                      if ((bool)rowPossibleEntryConditions[CN.AllowReuse] != true)
                      {
                         dtPossibleEntryConditions.Rows.Remove(rowPossibleEntryConditions);
                      }
                   }
                }
             }

             dtPossibleEntryConditions.AcceptChanges();
             dtChosenEntryConditions.AcceptChanges();
             DataGridPossibleEntryConditions.DataSource = dtPossibleEntryConditions;
             dataGridChosenEntryConditions.DataSource = dtChosenEntryConditions;
             // FA 14/12/09 - made dynamic width and height work all the times...
             if (dataGridChosenEntryConditions.RowCount <= 5)
             {
                 dataGridChosenEntryConditions.Height = (dataGridChosenEntryConditions.RowCount * 22) + 25;
                 dataGridChosenEntryConditions.Width = 683;
             }
             else
             {
                 dataGridChosenEntryConditions.Height = 138;
                 dataGridChosenEntryConditions.Width = 700;
             }

             // #9591 keep colour for any OR conditions        jec 08/03/12
             DataTable dtCopy = new DataTable();
             dtCopy = dtChosenEntryConditions.Copy();

             for (int i = 0; i < entryOrLetters.Count; i++)
             {
                 Color color = new Color();

                 color = stratConfig.RandomColourGenerator(i);

                 for (int j = 0; j < dtChosenEntryConditions.Rows.Count; j++)
                 {
                     if (dtChosenEntryConditions.Rows[j][CN.OrClause].ToString() == entryOrLetters[i].ToString())
                     {
                         dataGridChosenEntryConditions[CN.OrClause, j].Style.BackColor = color;
                         dataGridChosenEntryConditions[CN.OrClause, j].Value = String.Empty;
                     }
                 }
             }

             dtChosenEntryConditions = dtCopy.Copy();
             dtCopy.Dispose();
             dtCopy = null;

             ShowDataGridsUnselected(dataGridChosenEntryConditions);
             ShowDataGridsUnselected(DataGridPossibleEntryConditions);

             added = true;
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of AddEntryConditions_Click";
          }
       }

       private void RemoveEntryConditions_Click(object sender, EventArgs e)
       {
          Function = "RemoveEntryConditions_Click";
          try
          {
                //remove the selected row from the 'chosen conditions to enter' datagrid
             int index = dataGridChosenEntryConditions.RowCount;
             if (index > 0)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = dataGridChosenEntryConditions.Rows[i];
                   if (dataGridChosenEntryConditions.SelectedRows.Contains(row))
                   {
                      DataRow rowChosenEntryConditions = dtChosenEntryConditions.Rows[i];

                      dtChosenEntryConditions.Rows.Remove(rowChosenEntryConditions);
                   }
                }
                dtChosenEntryConditions.AcceptChanges();
                dtPossibleEntryConditions = stratConfig.GetConditions("tabPageEntryConditions");
                dtPossibleEntryConditions = stratConfig.FilterPossibleConditions(dtChosenEntryConditions, dtPossibleEntryConditions, CN.ConditionCode);
                dtPossibleEntryConditions.AcceptChanges();
                DataGridPossibleEntryConditions.DataSource = dtPossibleEntryConditions;
                dataGridChosenEntryConditions.DataSource = dtChosenEntryConditions;
                // #9591 keep colour for any remaining OR conditions        jec 08/03/12
                DataTable dtCopy = new DataTable();
                dtCopy = dtChosenEntryConditions.Copy();

                for (int i = 0; i < entryOrLetters.Count; i++)
                {
                    Color color = new Color();

                    color = stratConfig.RandomColourGenerator(i);

                    for (int j = 0; j < dtChosenEntryConditions.Rows.Count; j++)
                    {
                        if (dtChosenEntryConditions.Rows[j][CN.OrClause].ToString() == entryOrLetters[i].ToString())
                        {
                            dataGridChosenEntryConditions[CN.OrClause, j].Style.BackColor = color;
                            dataGridChosenEntryConditions[CN.OrClause, j].Value = String.Empty;
                        }
                    }
                }

                dtChosenEntryConditions = dtCopy.Copy();
                dtCopy.Dispose();
                dtCopy = null;

                ShowDataGridsUnselected(dataGridChosenEntryConditions);
                ShowDataGridsUnselected(DataGridPossibleEntryConditions);

                added = true;
             }
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of RemoveEntryConditions_Click";
          }
       }

       private void AddSteps_Click(object sender, EventArgs e)
       {
          Function = "AddSteps_Click";
          bool valid1 = true;
          bool valid2 = true;
          bool validOperator = true;
          bool validStrategy = true;

          try
          {
             //First validate numeric values if enabled
             if (V1onSteps.ReadOnly == false)
             {
                valid1 = ValidateValue(V1onSteps.Text, V1onSteps);
             }

             if (V2onSteps.ReadOnly == false)
             {
                valid2 = ValidateValue(V2onSteps.Text, V2onSteps);
             }

             if (OperatorsOnSteps.Enabled)
             {
                validOperator = ValidateOperator(OperatorsOnSteps.SelectedItem.ToString(), OperatorsOnSteps);
             }

             if (previousStrategies.Enabled)
             {
                validStrategy = ValidatePreviousStrategy(previousStrategies.SelectedValue.ToString(), previousStrategies);
             }

             int index = -1;
             int newIndex = -1;
             try
             {
                index = StepsConditionsActions.SelectedIndex;
                if (index > 3)
                {
                   newIndex = index - 4;
                }
             }
             catch
             {
                return;
             }
             if (index >= 0 && valid1 && valid2 && validOperator && validStrategy)
             {
                //remove the selected row from the 'possible conditions to step' listbox 
                //and put it into the 'chosen conditions to step' datagrid
                if (index > 3)
                {
                   string operand = String.Empty;
                   if (OperatorsOnSteps.SelectedItem.ToString() != "")
                   {
                      operand = OperatorsOnSteps.SelectedItem.ToString();
                   }
                   string sign = operands.GetOperandSign(operand);

                   string previousStrategy = String.Empty;
                   if(previousStrategies.Enabled)
                   {
                      previousStrategy = previousStrategies.SelectedValue.ToString();
                   }

                   DataRow rowChosenStepConditions = stratConfig.AddDataRow(dtChosenStepConditions, dtPossibleStepConditions, newIndex, 0, V1onSteps.Text, V2onSteps.Text, sign, "step", String.Empty, previousStrategy);
                   DataRow rowPossibleStepConditions = stratConfig.AddDataRow(dtChosenStepConditions, dtPossibleStepConditions, newIndex, 1, String.Empty, String.Empty, String.Empty, "step", String.Empty, String.Empty);

                   dtChosenStepConditions.Rows.Add(rowChosenStepConditions);
                   //Check for AllowReuse value of true to see if condition should not be removed from PossibleConditions
                   if ((bool)rowPossibleStepConditions[CN.AllowReuse] != true)
                   {
                      dtPossibleStepConditions.Rows.Remove(rowPossibleStepConditions);
                      object a = StepsConditionsActions.Items[index];
                      StepsConditionsActions.Items.Remove(a);
                   }                
                }
                else
                {
                   if (Letters.SelectedValue != null)
                   {
                      DataRow row = dtChosenStepConditions.NewRow();
                      row[CN.Condition] = StepsConditionsActions.SelectedItem.ToString() + " " + Letters.SelectedValue.ToString();
                      row[CN.ActionCode] = Letters.SelectedValue.ToString();
                     int step = dtChosenStepConditions.Rows.Count;
                     row[CN.Step] = step + 1;
                     row[CN.StepActionType] = DetermineActionType(StepsConditionsActions.SelectedItem.ToString());
                     row[CN.FalseStep] = false;
                     row[CN.SavedType] = "S";
                     dtChosenStepConditions.Rows.Add(row);
                  }
                }
                previousStrategies.Enabled = false;
             }

             dtPossibleStepConditions.AcceptChanges();
             dtChosenStepConditions.AcceptChanges();

             SetFalseStepToReadOnly();     

             ShowDataGridsUnselected(dataGridChosenStepConditions);
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of AddSteps_Click";
          }
       }

       /// <summary>
       /// Sets the FalseStep cell to read only
       /// </summary>
       private void SetFalseStepToReadOnly()
       {
          foreach (DataGridViewRow row in dataGridChosenStepConditions.Rows)
          {
             if ((bool)row.Cells[CN.FalseStep].Value == false)
             {
                row.Cells[CN.NextStepFalse].ReadOnly = true;
             }
             else       //remove read only if false step is true  UAT970 jec 19/01/10
             {
                 row.Cells[CN.NextStepFalse].ReadOnly = false;
             }
          }
       }

       private string DetermineActionType(string actionType)
       {
          string actionTypeCode = String.Empty;
          switch (actionType)
          {
             case Actions.SendLetter:
                actionTypeCode = "L";
                break;
             case Actions.SendSMS:
                actionTypeCode =  "S";
                break;
             case Actions.SendToStrategy:
                actionTypeCode = "X";
                break;
             case Actions.SendToWorklist:
                actionTypeCode = "W";
                break;
          }
          return actionTypeCode;
       }

       private string DetermineAction(string actionCode)
       {
          string action = String.Empty;
          switch (actionCode)
          {
             case "L":
                action = Actions.SendLetter;
                break;
             case "S":
                action = Actions.SendSMS;
                break;
             case "X":
                action = Actions.SendToStrategy;
                break;
             case "W":
                action = Actions.SendToWorklist;
                break;
          }
          return action;
       }

       private void RemoveSteps_Click(object sender, EventArgs e)
       {
          Function = "RemoveSteps_Click";
          try
          {
             //remove the selected row from the 'chosen conditions to step' datagrid
             int index = dataGridChosenStepConditions.RowCount;
             if (index > 0)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = dataGridChosenStepConditions.Rows[i];
                   if (dataGridChosenStepConditions.SelectedRows.Contains(row))
                   {
                      DataRow rowChosenStepConditions = dtChosenStepConditions.Rows[i];
                      dtChosenStepConditions.Rows.Remove(rowChosenStepConditions);
                      //Re-number steps and remove true/false step
                      int step = 1;
                      foreach (DataRow stepRow in dtChosenStepConditions.Rows)
                      {
                         stepRow[CN.Step] = step;
                         stepRow[CN.NextStepFalse] = DBNull.Value;
                         stepRow[CN.NextStepTrue] = DBNull.Value;
                         step++;
                      }
                   }
                }
                dtChosenStepConditions.AcceptChanges();
                dtPossibleStepConditions = stratConfig.GetConditions("tabPageSteps");
                dtPossibleStepConditions = stratConfig.FilterPossibleConditions(dtChosenStepConditions, dtPossibleStepConditions, CN.ConditionCode);
                dtPossibleStepConditions.AcceptChanges();

                //Re-populate the steps ListBox - first clearing existing data
                StepsConditionsActions.Items.Clear();

                StepsConditionsActions.Items.Add(Actions.SendSMS);
                StepsConditionsActions.Items.Add(Actions.SendLetter);
                StepsConditionsActions.Items.Add(Actions.SendToWorklist);
                StepsConditionsActions.Items.Add(Actions.SendToStrategy);

                foreach (DataRow row in dtPossibleStepConditions.Rows)
                {
                   StepsConditionsActions.Items.Add(row[CN.Condition].ToString());
                }

                Letters.SelectedValue = "";

                ShowDataGridsUnselected(dataGridChosenStepConditions);
             }

          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of RemoveSteps_Click";
          }
       }

       private void addActions_Click(object sender, EventArgs e)
       {
          Function = "addActions_Click";
          
          try
          {
             //remove the selected row from the 'possible actions to enter' datagrid 
             //and put it into the 'chosen actions to enter' datagrid
             int index = dgPossibleActions.RowCount;
             if (index > 0)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = dgPossibleActions.Rows[i];
                   if (dgPossibleActions.SelectedRows.Contains(row))
                   {
                      DataRow rowChosenActions = dtChosenActions.NewRow();
                      DataRow rowPossibleActions = dtPossibleActions.Rows[i];

                      rowChosenActions[CN.Action] = rowPossibleActions[CN.Action];
                      rowChosenActions[CN.ActionCode] = rowPossibleActions[CN.ActionCode];

                      dtChosenActions.Rows.Add(rowChosenActions);
                      dtPossibleActions.Rows.Remove(rowPossibleActions);
                   }
                }

             dtPossibleActions.AcceptChanges();
             dtChosenActions.AcceptChanges();
             dgPossibleActions.DataSource = dtPossibleActions;

             ShowDataGridsUnselected(dgChosenActions);
             ShowDataGridsUnselected(dgPossibleActions);
            }
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of addActions_Click";
          }
       }

       private void removeActions_Click(object sender, EventArgs e)
       {
          Function = "removeActions_Click";
          try
          {
             //remove the selected row from the 'chosen conditions to enter' datagrid and 
             // add it to the 'possible conditions to enter' datagrid
             int index = dgChosenActions.RowCount;
             if (index > 0)
             {
                for (int i = index - 1; i >= 0; i--)
                {
                   DataGridViewRow row = dgChosenActions.Rows[i];
                   if (dgChosenActions.SelectedRows.Contains(row))
                   {
                      DataRow rowPossibleActions = dtPossibleActions.NewRow();
                      DataRow rowChosenActions = dtChosenActions.Rows[i];

                      rowPossibleActions[CN.Action] = rowChosenActions[CN.Action];
                      rowPossibleActions[CN.ActionCode] = rowChosenActions[CN.ActionCode];

                      dtPossibleActions.Rows.Add(rowPossibleActions);
                      dtChosenActions.Rows.Remove(rowChosenActions);
                   }
                }
                
                dtChosenActions.AcceptChanges();
                dtPossibleActions = stratConfig.GetActions();
                dtPossibleActions = stratConfig.FilterPossibleConditions(dtChosenActions, dtPossibleActions, CN.ActionCode);
                dtPossibleActions.AcceptChanges();
                dgPossibleActions.DataSource = dtPossibleActions;

                ShowDataGridsUnselected(dgChosenActions);
                ShowDataGridsUnselected(dgPossibleActions);
             }
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of removeActions_Click";
          }
       }

      #endregion

        private void dataGridChosenEntryConditions_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
          //try
          //{
          //   EntryConditionsOperators.Enabled = false;
          //   EntryConditionsValue1.ReadOnly = true;
          //   EntryConditionsValue2.ReadOnly = true;

          //  if(dtChosenEntryConditions.Rows[e.RowIndex][CN.Operand] != null)
          //   {
          //     string operand = dtChosenEntryConditions.Rows[e.RowIndex][CN.Operand].ToString();
          //     string displayedOperand = operands.GetDisplayedOperand(operand);
               
          //        if(displayedOperand != "")
          //        {
          //           EntryConditionsOperators.SelectedItem = displayedOperand;
          //        }
          //        else
          //        {
          //           EntryConditionsOperators.Items.Add("");
          //           EntryConditionsOperators.SelectedItem = "";
          //        }
          //   }

          //   if (dtChosenEntryConditions.Rows[e.RowIndex][CN.Operator1] != null)
          //   {
          //      EntryConditionsValue1.Text = dtChosenEntryConditions.Rows[e.RowIndex][CN.Operator1].ToString();
          //   }

          //   if (dtChosenEntryConditions.Rows[e.RowIndex][CN.Operator2] != null)
          //   {
          //      EntryConditionsValue2.Text = dtChosenEntryConditions.Rows[e.RowIndex][CN.Operator2].ToString();
          //   }
          //}
          //catch
          //{
          //   return;
          //}
        }

        private void dataGridChosenExitConditions_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        //private void dataGridStepConditions_RowEnter(object sender, DataGridViewCellEventArgs e)
        //{
          
        //}

        private void dataGridChosenStepConditions_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private void EntryConditionsOperators_SelectionChangeCommitted(object sender, EventArgs e)
        {
          //EntryConditionsOperators.Items.Remove(DropDownValues.Operator);
        }

        private void StrategyConfiguration_Shown(object sender, EventArgs e)
        {
          ShowDataGridsUnselected(DataGridPossibleEntryConditions);
          ShowDataGridsUnselected(dataGridChosenEntryConditions);
        }

        private void ShowDataGridsUnselected(DataGridView sentDataGrid)
        {
            foreach (DataGridViewRow dgv in sentDataGrid.Rows)
            {
                dgv.Selected = false;
            }
        }

        private void StepsConditionsActions_Enter(object sender, EventArgs e)
        {
          
        }

        private void StepsConditionsActions_SelectedValueChanged(object sender, EventArgs e)
        {
          Function = "StepsConditionsActions_SelectedValueChanged";
          try
          {
             if (StepsConditionsActions.SelectedIndex != -1)
             {
                string condition = StepsConditionsActions.SelectedItem.ToString();
                string operand = "";
                bool showFalse = false;

                //IP - 02/12/09 - UAT5.2 (929)
                AddSteps.Enabled = true;
                string tabSelected = tabControlStrategies.SelectedTab.Name;
                string conditionCode = string.Empty;
                string type = string.Empty;
                //bool disableAddCondition = false;                                                 //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                RemoveErrorProvider(AddSteps);

                //If the Type is ' ' (indicating this condition can be used for 'Entry', 'Steps' and 'Exit') then we need to check if it is currently being used as a entry
                // or an exit condition. If it is being used, then we do not want the user to add this condition, therefore disable the button that adds the condition.

                //Select the ConditionCode for the selected condition.
                foreach (DataRow row in dtPossibleStepConditions.Rows)
                {
                    if (Convert.ToString(row[CN.Condition]) == condition)
                    {
                        conditionCode = Convert.ToString(row[CN.ConditionCode]);
                        type = Convert.ToString(row[CN.Type]);
                        break;
                    }
                }


                //if (conditionCode != string.Empty && type == " ")                                 //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                //{
                //    disableAddCondition = disableUsingCondition(tabSelected, conditionCode);
                //}


                //if (disableAddCondition == false) //IP - 02/12/09 - UAT5.2 (929)                  //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                //{
                    foreach (DataRow row in dtPossibleStepConditions.Rows)
                    {
                        if (row[CN.Condition].ToString() == condition)
                        {
                            operand = row[CN.OperandAllowable].ToString();
                            if (Convert.ToBoolean(row[CN.FalseStep].ToString()) == true)
                            {
                                showFalse = Convert.ToBoolean(row[CN.FalseStep].ToString());
                            }
                        }
                    }

                    //if there is already a condition that allows a false next step then continue to show the CN.NextStepFalse column
                    foreach (DataRow row in dtChosenStepConditions.Rows)
                    {
                        try
                        {
                            if ((bool)row[CN.FalseStep] == true)
                            {
                                showFalse = true;
                                break;
                            }
                        }
                        catch
                        {
                            //catch for any actions in dtChosenStepConditions
                        }
                    }

                    if (operand == "1")
                    {
                        OperatorsOnSteps.Enabled = true;
                        V1onSteps.ReadOnly = false;
                    }
                    else if (operand == "2")
                    {
                        OperatorsOnSteps.Enabled = true;
                        V1onSteps.ReadOnly = false;
                        V2onSteps.ReadOnly = false;
                    }
                    else
                    {
                        OperatorsOnSteps.Enabled = false;
                        V1onSteps.ReadOnly = true;
                        V2onSteps.ReadOnly = true;
                    }

                    if (operand == "P")
                    {
                        previousStrategies.Enabled = true;
                    }
                    else
                    {
                        previousStrategies.Enabled = false;
                    }

                    if (showFalse == true)
                    {
                        dataGridChosenStepConditions.Columns[CN.NextStepFalse].Visible = true;
                        dataGridChosenStepConditions.Columns[CN.NextStepFalse].HeaderText = "False";
                        dataGridChosenStepConditions.Columns[CN.NextStepFalse].Width = 40;
                    }
                    else
                    {
                        dataGridChosenStepConditions.Columns[CN.NextStepFalse].Visible = false;
                    }

                    btnViewSMS.Visible = false;
                    //Populate the Letters ComboBox depending on what has been selected from StepsConditionsActions
                    switch (StepsConditionsActions.SelectedItem.ToString())
                    {
                        case (Actions.SendLetter):
                            //populate with letters
                            Letters.Enabled = true;
                            Letters.DataSource = stratConfig.GetLetters();
                            Letters.DisplayMember = CN.CodeDescript;
                            Letters.ValueMember = CN.CodeDescript;
                            break;
                        case (Actions.SendSMS):
                            //populate with SMS
                            Letters.Enabled = true;
                            Letters.DataSource = stratConfig.GetSMS();
                            Letters.DisplayMember = CN.descr1;
                            Letters.ValueMember = CN.SMSName;
                            btnViewSMS.Visible = true;
                            break;
                        case (Actions.SendToWorklist):
                            //populate with worklists
                            Letters.Enabled = true;
                            Letters.DataSource = stratConfig.GetWorklists();
                            Letters.DisplayMember = CN.WorkList;
                            Letters.ValueMember = CN.WorkList;
                            break;
                        case (Actions.SendToStrategy):
                            //populate with strategies
                            Letters.Enabled = true;
                            LoadStrategiesForLetters();
                            break;
                        default:
                            //disable the comboBox
                            Letters.SelectedValue = "";
                            Letters.Enabled = false;
                            break;
                    }
                 //}//IP - 02/12/09 - UAT5.2 (929)                                                                   //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                //else
                //{
                //    AddSteps.Enabled = false;
                //    errorProvider1.SetError(AddSteps, GetResource("M_CANNOTADDSTEPCONDITION"));
                //}
             } 
             RemoveErrorProvider(previousStrategies);
             RemoveErrorProvider(OperatorsOnSteps);
             RemoveErrorProvider(V1onSteps);
             RemoveErrorProvider(V2onSteps);
             V1onSteps.Text = "";
             V2onSteps.Text = "";

             OperatorsOnSteps.SelectedItem = DropDownValues.Operator;
             previousStrategies.SelectedValue = DropDownValues.PreviousStrategy;
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of StepsConditionsActions_SelectedValueChanged";
          }
        }

        private void EntryConditionsOr_Click(object sender, EventArgs e)
        {
          Function = "EntryConditionsOr_Click";
          try
          {
             if (dataGridChosenEntryConditions.SelectedRows.Count > 1)
             {
                int colours = 0;
                foreach (DataRow rowOr in dtChosenEntryConditions.Rows)
                {
                   if (rowOr[CN.OrClause].ToString() != string.Empty)
                   {
                      colours++;
                   }
                }

                Color color = stratConfig.RandomColourGenerator(colours);
                string letter = stratConfig.RandomLetterGenerator();
                //Check that letter is not contained in the ArrayList for that strategy
                   while(entryOrLetters.Contains(letter))
                   {
                      letter = stratConfig.RandomLetterGenerator();
                   }

                foreach (DataGridViewRow row in dataGridChosenEntryConditions.SelectedRows)
                {
                   int index = row.Index;
                   //mark each of these selected conditions as being an 'OR' condition and show this in the dataGrid
                   //add an 'OR' value of a randomly selected letter to the DataTable for each of these rows

                   dataGridChosenEntryConditions[CN.OrClause, index].Style.BackColor = color;
                   dtChosenEntryConditions.Rows[index][CN.OrClause] = letter;
                }
                ShowDataGridsUnselected(dataGridChosenEntryConditions);
             //dtChosenEntryConditions.AcceptChanges();
             }
             else
             {
                MessageBox.Show("You must first select more than one condition to apply the 'OR' attribute.");
             }
          }
          catch(Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of EntryConditionsOr_Click";
          }
        }

        //private void ExitConditionsOr_Click(object sender, EventArgs e)
        //{
          
        //}

       private bool ValidateValue(string value1,Control cont)
       {
            bool valid = true;
            valid = validator.Valid(value1);
            if (valid == true)
            {
             errorProvider1.SetError(cont, "");
            }
            else
            {
             errorProvider1.SetError(cont, GetResource("M_POSITIVENUM"));
            }
            return valid;
       }

        private bool ValidateOperator(string operators,Control cont)
       {
          bool valid = true;
          valid = validator.ValidateOperators(operators);
          if (valid == true)
          {
             errorProvider1.SetError(cont, "");
          }
          else
          {
             errorProvider1.SetError(cont, GetResource("M_ENTERMANDATORY"));
          }
          return valid;
       }

        /// <summary>
        /// Performs validation on the exit strategies combo box
        /// </summary>
        /// <param name="exit"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        private bool ValidateExitStrategy(string exit, Control cont)
        {
            bool valid = true;
            valid = validator.ValidateExitStrategies(exit);
            if (valid == true)
            {
             errorProvider1.SetError(cont, "");
            }
            else
            {
             errorProvider1.SetError(cont, GetResource("M_ENTERMANDATORY"));
            }
            return valid;
        }

        /// <summary>
        /// Performs validation on the previous strategies combo box
        /// </summary>
        /// <param name="exit"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        private bool ValidatePreviousStrategy(string exit, Control cont)
        {
            bool valid = true;
            valid = validator.ValidatePreviousStrategies(exit);
            if (valid == true)
            {
                errorProvider1.SetError(cont, "");
            }
            else
            {
                errorProvider1.SetError(cont, GetResource("M_ENTERMANDATORY"));
            }
            return valid;
        }

       private void CreateNew_Click(object sender, EventArgs e)
       {
          Function = "CreateNew_Click";
          try
          {
             newStrategy.ReadOnly = false;
             strategyCode.ReadOnly = false;
             Strategies.Enabled = false;
             cbManual.Enabled = true;       //UAT856
             cbManual.Checked = false;
             saveAs.Enabled = false;
             CreateNew.Enabled = false;
             populatedEntries = false;
             populatedExits = false;
             populatedSteps = false;
             populatedActions = false;
             LoadConditions(String.Empty);
             SetReadOnlyFields(true);
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of CreateNew_Click";
          }
       }

       #region Save Functionality

       private void btnSave_Click(object sender, EventArgs e)
       {
          Function = "btnSave_Click";
          try
          {
             if (ConfirmSave())
             {
                bool valid = true;
                if (Strategies.Enabled == false)
                {
                   valid = ValidateNewStrategy();
                }

                if (valid)
                {
                   //Save functionality            
                   string type = String.Empty;
                   DataSet dsStrategies = new DataSet();

                   dsStrategies.Tables.Add(dtChosenEntryConditions.Copy());
                   dsStrategies.Tables.Add(dtChosenExitConditions.Copy());
                   dsStrategies.Tables.Add(dtChosenStepConditions.Copy());
                   dsStrategies.Tables.Add(dtChosenActions.Copy());

                   //IP - 02/06/09 - Credit Collection Walkthrough Changes - pass in bool for 'Allocations' check.
                   if (Strategies.Enabled == false)
                   {
                       stratConfig.SaveStrategyCondition(strategyCode.Text.Trim(), newStrategy.Text.Trim(), dsStrategies, Convert.ToBoolean(chkAllocations.Checked), Convert.ToBoolean(cbManual.Checked), Credential.UserId);    //UAT987
                   }
                   else
                   {
                       stratConfig.SaveStrategyCondition(Strategies.SelectedValue.ToString(), Strategies.SelectedText.ToString(), dsStrategies, Convert.ToBoolean(chkAllocations.Checked), Convert.ToBoolean(cbManual.Checked), Credential.UserId);  //UAT987
                   }

                   populatedEntries = false;
                   populatedExits = false;
                   populatedSteps = false;
                   populatedActions = false;

                   if (Strategies.Enabled)
                   {
                      LoadConditions(Strategies.SelectedValue.ToString());
                   }
                   else
                   {
                      LoadConditions(strategyCode.Text);
                      LoadStrategies();
                      Strategies.SelectedValue = strategyCode.Text;
                      cbManual.Checked = Convert.ToBoolean(dtStrategies.Rows[Strategies.SelectedIndex][CN.Manual]);  //uat856 jec 18/09/09
                   }

                   SetStrategyFields(true);
                }
             }
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of btnSave_Click";
          }
       }

       private void saveAs_Click(object sender, EventArgs e)
       {
          Function = "saveAs_Click";
          try
          {
             //On first attempt tell the user that they need to re-name this strategy in the new strategy text box
             if (newStrategy.ReadOnly == true)
             {
                MessageBox.Show("In order to save this strategy under a different name you must first name it in the New Strategy Text Field.",
                   "New Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetStrategyFields(false);
                SetReadOnlyFields(true);
             }
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of saveAs_Click";
          }
       }

       /// <summary>
       /// Performs validation on all the fields required for creating a new strategy
       /// </summary>
       /// <returns></returns>
       private bool ValidateNewStrategy()
       {
          //Validate required controls
          bool validName = true;
          bool validCode = true;
          string strategy = String.Empty;
          string code = String.Empty;

             validName = ValidateOperator(newStrategy.Text, newStrategy);
             validCode = ValidateOperator(strategyCode.Text, strategyCode);
             strategy = newStrategy.Text.Trim();
             code = strategyCode.Text.Trim();
             int noStrategies = dtStrategies.Rows.Count;

             for (int i = 0; i < noStrategies; i++)
             {
                string strategyName = dtStrategies.Rows[i][CN.Description].ToString();
                if (strategyName.Remove(0,strategyName.IndexOf(" ") + 1) ==  strategy)
                {
                   MessageBox.Show("The name you have chosen is the same as an existing strategy. Please re-name.", "Incorrect Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   newStrategy.SelectAll();
                   newStrategy.Focus();
                   return false;
                }
                if (dtStrategies.Rows[i][CN.Strategy].ToString() == code)
                {
                   MessageBox.Show("The code you have chosen is the same as for an existing strategy. Please re-name.", "Incorrect Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   strategyCode.SelectAll();
                   strategyCode.Focus();
                   return false;
                }
             }

             //if (code.Length != 3)        // #9586 - code can now be 5
             //{
             //   MessageBox.Show("All codes must be 3 characters in length. Please re-name.", "Incorrect Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
             //   strategyCode.SelectAll();
             //   strategyCode.Focus();
             //   return false;
             //}

             if (validName == true && validCode == true)
             {
                return true;
             }
             else
             {
                return false;
             }
       }

       #endregion

       /// <summary>
       /// Sets the enabled state of the strategy fields
       /// </summary>
       /// <param name="result"></param>
       private void SetStrategyFields(bool result)
       {
          Strategies.Enabled = result;
          newStrategy.ReadOnly = result;
          newStrategy.Text = "";
          strategyCode.ReadOnly = result;
          strategyCode.Text = "";
          saveAs.Enabled = result;
       }

       private void activate_Click(object sender, EventArgs e)
       {
           Function = "activate_Click";
           try
           {
              int activeValue = 0;
              if (activate.Text == ActivateName.Activate)
              {
                 foreach (DataRow row in dtStrategies.Rows)
                 {
                    if (row[CN.Strategy].ToString() == Strategies.SelectedValue.ToString())
                    {
                       activeValue = 1;
                       isActive = "1";
                       row[CN.IsActive] = "1";
                    }
                 }
              }
              else
              {
                 //First check to see if this is a 'NON arrears' strategy
                 if (Strategies.SelectedValue.ToString() == "NON")
                 {
                    ShowInfo("M_NONARREARS");
                    return;
                 }
                 foreach (DataRow row in dtStrategies.Rows)
                 {
                    if (row[CN.Strategy].ToString() == Strategies.SelectedValue.ToString())
                    {
                       //Now check to see if this strategy has any strategy that is an exit point or has a strategy that appears in the steps of the strategy
                       ArrayList exitStrats = new ArrayList();
                       foreach (DataRow dtRow in dtChosenExitConditions.Rows)
                       {
                          exitStrats.Add(dtRow[CN.ActionCode].ToString());
                       }

                       foreach (DataRow dtRow in dtChosenStepConditions.Rows)
                       {
                          if (dtRow[CN.StepActionType].ToString() == "X")
                          {
                             exitStrats.Add(dtRow[CN.ActionCode].ToString());
                          }
                       }

                       if(exitStrats.Count > 0)
                       {
                           //IP - 24/09/08 - UAT5.2 - UAT(520)
                           //If there are duplicate entries of strategies in the 
                           //'exitStrats' ArrayList, then need to remove them otherwise
                           //they will incorrectly be displayed in the message to the user below.

                           //ArrayList 'linkedStrats' will store the codes of the 'Strategies'
                           //that are currently linked to the strategy that is being de-activated.
                           //The ArrayList should not contain duplicate entries.
                           ArrayList linkedStrats = new ArrayList();

                           object duplicateItem = "";

                           //Loop throug each item of the 'exitStrats' ArrayList
                           foreach (object curritem in exitStrats)
                           {
                               //If the next item in the list is the same as the 
                               //'duplicateItem' object then do not continue.
                                if (curritem.ToString() != duplicateItem.ToString())
                                {
                                    int count = 0;
                                    
                                    //With the curritem selected loop through the
                                    //same ArrayList and check for any duplicates
                                    foreach (object item in exitStrats)
                                    {
                                        if (curritem.ToString() == item.ToString())
                                        {
                                            //Counter to keep a track of the number of ocurrences
                                            //of the item in the ArrayList
                                            count = count + 1;

                                            //If only one occurrence then add the item
                                            //to a new ArrayList 'linkedStrats'.
                                            if (count < 2)
                                            {
                                                linkedStrats.Add(curritem);
                                            }
                                            //else if there is more then one ocurrence
                                            //save the item in 'duplicateItem' object.
                                            else
                                            {
                                                duplicateItem = curritem;
                                            }

                                        }

                                    }
                                }
                           }

                           //IP - 24/09/08 - UAT5.2 - UAT(520)
                           //Extract the names of the strategies that are linked to the
                           //strategy that is being de-activated and display in the
                           //message to the user.
                           StringBuilder sb = new StringBuilder();

                           //Loop through each item in the ArrayList
                           foreach (object item in linkedStrats)
                           {
                               //Compare the item in the ArrayList with that of 
                               //each DataRow in the data table that contains details of the strategies.
                               foreach (DataRow dr in dtStrategies.Rows)
                               {
                                   //If there is a match then add the description of that strategy
                                   //to the StringBuilder object.
                                   if (item.ToString() == dr[CN.Strategy].ToString())
                                   {
                                       sb.Append(dr[CN.Description].ToString());
                                       if (linkedStrats.IndexOf(item) < linkedStrats.Count - 1)
                                       {
                                           sb.Append(",");
                                       }
                                       break;
                                   }
                               }
                           }

                           
                           
                          Convert.ToString(sb);
                          
                          //Display to the user the names of the strategies linked to the Strategy
                          //that is being de-activated.
                          ShowInfo("M_CANNOTDEACTIVATE", new object[] { linkedStrats.Count, sb });
                          return;
                       }

                       isActive = "0";
                       row[CN.IsActive] = "0";
                    }
                 }
              }
              dtStrategies.AcceptChanges();
              string strategy = Strategies.SelectedValue.ToString();

              stratConfig.SetStrategyActive(strategy, activeValue);

              LoadConditions(Strategies.SelectedValue.ToString());
              NameActivateButton();
              //LoadStrategies();
              Strategies.SelectedValue = strategy;
           }
           catch (Exception ex)
           {
              Catch(ex, Function);
           }
           finally
           {
              Function = "End of activate_Click";
           }
       }

       /// <summary>
       /// Returns the status that will determine whether or not a strategy is to be saved
       /// </summary>
       /// <returns></returns>
       private bool ConfirmSave()
       {
          bool status = true;
          
          {
             DialogResult dr = ShowInfo("M_SAVE", MessageBoxButtons.YesNo);
             if (dr == DialogResult.Yes)
             {
                status = true;
             }
             else if (dr == DialogResult.No)
             {
                status = false;
             }
          }
          return status;
       }

       private void RemoveErrorProvider(Control cont)
       {
          errorProvider1.SetError(cont, "");
       }

       private void EntryConditionsValue1_TextChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(EntryConditionsValue1);
       }

       private void EntryConditionsValue2_TextChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(EntryConditionsValue2);
       }

       private void EntryConditionsOperators_SelectedIndexChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(EntryConditionsOperators);
       }

       private void ExitConditionsOperators_SelectedIndexChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(ExitConditionsOperators);
       }

       private void ExitConditionsV1_TextChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(ExitConditionsV1);
       }

       private void ExitConditionsV2_TextChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(ExitConditionsV2);
       }

       private void OperatorsOnSteps_SelectedIndexChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(OperatorsOnSteps);
       }

       private void V1onSteps_TextChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(V1onSteps);
       }

       private void V2onSteps_TextChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(V2onSteps);
       }

       private void tabControlStrategies_Validating(object sender, CancelEventArgs e)
       {
         
       }

       private void tabPageEntryConditions_Validating(object sender, CancelEventArgs e)
       {
          
       }

       private void exitStrategy_SelectedValueChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(exitStrategy);
       }

       private void btnUp_Click(object sender, EventArgs e)
       {
          Function = "btnUp_Click";
          try
          {
             RearrangeRows(true); 
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of btnUp_Click";
          }
       }

       private void btnDown_Click(object sender, EventArgs e)
       {
          Function = "btnDown_Click";
          try
          {
             RearrangeRows(false);          
          }
          catch (Exception ex)
          {
             Catch(ex, Function);
          }
          finally
          {
             Function = "End of btnDown_Click";
          }
       }

       private void RearrangeRows(bool up)
       {
          int x = 0;
          int step = 1;
          int trueStep = 0;
          int falseStep = 0;

          List<Conditions> conditionsList = new List<Conditions>();
          foreach (DataRow row in dtChosenStepConditions.Rows)
          {
             string condition = row[CN.Condition].ToString();
             trueStep = row[CN.NextStepTrue] == DBNull.Value ? 0 : Convert.ToInt32(row[CN.NextStepTrue]);
             falseStep = row[CN.NextStepFalse] == DBNull.Value ? 0 : Convert.ToInt32(row[CN.NextStepFalse]);
             if (row[CN.NextStepTrue] != DBNull.Value)
             {
                foreach (DataRow r in dtChosenStepConditions.Rows)
                {
                   if (Convert.ToInt32(r[CN.Step]) == trueStep)
                   {
                      conditionsList.Add(new Conditions(condition, r[CN.Condition].ToString(), true));
                   }
                }
             }
             if (row[CN.NextStepFalse] != DBNull.Value)
             {
                foreach (DataRow r in dtChosenStepConditions.Rows)
                {
                   if (Convert.ToInt32(r[CN.Step]) == falseStep)
                   {
                      conditionsList.Add(new Conditions(condition, r[CN.Condition].ToString(), false));
                   }
                }
             }
          }

          if (up)
          {
             for (int i = dtChosenStepConditions.Rows.Count - 1; i >= 0; i--)
             {
                if (dataGridChosenStepConditions.Rows[i].Selected && i > 0)
                {
                   DataRow rowChosenStepConditions = stratConfig.GetNewRow(dtChosenStepConditions, i);

                   dtChosenStepConditions.Rows.Remove(dtChosenStepConditions.Rows[i]);
                   dtChosenStepConditions.Rows.InsertAt(rowChosenStepConditions, i - 1);
                   dtChosenStepConditions.AcceptChanges();

                   x = i - 1;
                }
             }
          }
          else
          {
             for (int i = dtChosenStepConditions.Rows.Count - 1; i >= 0; i--)
             {
                if (dataGridChosenStepConditions.Rows[i].Selected && i < dtChosenStepConditions.Rows.Count - 1)
                {
                   DataRow rowChosenStepConditions = stratConfig.GetNewRow(dtChosenStepConditions, i);
                   dtChosenStepConditions.Rows.Remove(dtChosenStepConditions.Rows[i]);
                   dtChosenStepConditions.Rows.InsertAt(rowChosenStepConditions, i + 1);
                   dtChosenStepConditions.AcceptChanges();

                   x = i + 1;
                }
             }
          }

          foreach (DataRow row in dtChosenStepConditions.Rows)
          {
             row[CN.Step] = step;
             step++;
          }

          foreach (DataRow row in dtChosenStepConditions.Rows)
          {
             Conditions found = (Conditions)conditionsList.Find(delegate(Conditions con) { return row[CN.Condition].ToString() == con.nextCondition; });
             if (found != null)
             {
                foreach (DataRow r in dtChosenStepConditions.Rows)
                {
                   trueStep = 0;
                   if (r[CN.Condition].ToString() == found.nextCondition && found.trueStep == true)
                   {
                      trueStep = Convert.ToInt32(r[CN.Step]);
                      break;
                   }
                }

                foreach (DataRow r in dtChosenStepConditions.Rows)
                {
                   falseStep = 0;
                   if (r[CN.Condition].ToString() == found.nextCondition && found.trueStep == false)
                   {
                      falseStep = Convert.ToInt32(r[CN.Step]);
                      break;
                   }
                }
             }

             if (found != null)
             {
                foreach (DataRow r1 in dtChosenStepConditions.Rows)
                {
                   if (r1[CN.Condition].ToString() == found.condtion && found.trueStep == true)
                   {
                      if (trueStep == 0)
                      {
                         r1[CN.NextStepTrue] = DBNull.Value;
                      }
                      else
                      {
                         r1[CN.NextStepTrue] = trueStep;
                      }
                      conditionsList.Remove(found);
                      break;
                   }

                   if (r1[CN.Condition].ToString() == found.condtion && found.trueStep == false)
                   {
                      if (falseStep == 0)
                      {
                         r1[CN.NextStepFalse] = DBNull.Value;
                      }
                      else
                      {
                         r1[CN.NextStepFalse] = falseStep;
                      }
                      conditionsList.Remove(found);
                      break;
                   }
                }
             }
          }

          if (up)
          {
             if (dtChosenStepConditions.Rows.Count > 0)
             {
                ShowDataGridsUnselected(dataGridChosenStepConditions);
                dataGridChosenStepConditions.Rows[x].Selected = true;
                dataGridChosenStepConditions.CurrentCell = dataGridChosenStepConditions.Rows[x].Cells[5];
             }
          }
          else
          {
             if (x != 0)
             {
                ShowDataGridsUnselected(dataGridChosenStepConditions);
                dataGridChosenStepConditions.Rows[x].Selected = true;
                dataGridChosenStepConditions.CurrentCell = dataGridChosenStepConditions.Rows[x].Cells[5];
             }
          }

          SetFalseStepToReadOnly();
       }

       private void DataGridPossibleEntryConditions_Sorted(object sender, EventArgs e)
       {
          DataView dvSort = new DataView(dtPossibleEntryConditions);
          //ListSortDirection direction;
          //First time event is fired will sort ascending
          if (DataGridPossibleEntryConditions.SortOrder == SortOrder.Ascending)
          {             
             dvSort.Sort = CN.Condition + " ASC";
             //direction = ListSortDirection.Descending;
          }
          else
          {
             dvSort.Sort = CN.Condition + " DESC";
             //direction = ListSortDirection.Ascending;
          }
          dtPossibleEntryConditions = dvSort.ToTable();
          dtPossibleEntryConditions.AcceptChanges();
       }

       private void dataGridPossibleExitConditions_Sorted(object sender, EventArgs e)
       {
          DataView dvSort = new DataView(dtPossibleExitConditions);
          //ListSortDirection direction;
          //First time event is fired will sort ascending
          if (dataGridPossibleExitConditions.SortOrder == SortOrder.Ascending)
          {
             dvSort.Sort = CN.Condition + " ASC";
             //direction = ListSortDirection.Descending;
          }
          else
          {
             dvSort.Sort = CN.Condition + " DESC";
             //direction = ListSortDirection.Ascending;
          }
          dtPossibleExitConditions = dvSort.ToTable();
          dtPossibleExitConditions.AcceptChanges();
       }

       private void dgPossibleActions_Sorted(object sender, EventArgs e)
       {
          DataView dvSort = new DataView(dtPossibleActions);
          //ListSortDirection direction;
          //First time event is fired will sort ascending
          if (dgPossibleActions.SortOrder == SortOrder.Ascending)
          {
             dvSort.Sort = CN.Action + " ASC";
             //direction = ListSortDirection.Descending;
          }
          else
          {
             dvSort.Sort = CN.Action + " DESC";
             //direction = ListSortDirection.Ascending;
          }
          dtPossibleActions = dvSort.ToTable();
          dtPossibleActions.AcceptChanges();
       }

       private void previousStrategies_SelectedIndexChanged(object sender, EventArgs e)
       {
          RemoveErrorProvider(previousStrategies);
       }

       private void btnViewSMS_Click(object sender, EventArgs e)
       {
           if (StepsConditionsActions.SelectedItem == null || 
                StepsConditionsActions.SelectedItem.ToString() != Actions.SendSMS)
               return;

           if (Letters.DataSource != null && Letters.SelectedValue != null)
           {
               string text = ((DataTable)Letters.DataSource).
                               Select(CN.SMSName + " = '" + Letters.SelectedValue + "'")[0][CN.SMSText].ToString();

               // UAT 896 - add description to title of pop up
               string header = Letters.SelectedValue + " - " + ((DataTable)Letters.DataSource).
                                Select(CN.SMSName + " = '" + Letters.SelectedValue + "'")[0][CN.Description].ToString();
               
              ViewSMSPopup smsPop = new ViewSMSPopup(text);
              smsPop.Text = header;
               smsPop.ShowDialog(this);
           }

       }

        //private void Delete_Click(object sender, EventArgs e)
        //{
           
        //}

        //private void btnSave_ClientSizeChanged(object sender, EventArgs e)
        //{

        //}

        //IP - UAT(514) - Delete the selected 'Existing Strategy'.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                string strategyToDelete;

                if (DialogResult.Yes == ShowInfo("M_DELETEEXISTINGSTRATEGY", MessageBoxButtons.YesNo))
                {
                    strategyToDelete = Strategies.SelectedValue.ToString();
                    stratConfig.DeleteExistingStrategy(strategyToDelete);
                    StrategyConfiguration_Load(null, null);
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #region--CR 976 changes (NM)----------------------------------------
        //------------------------------------------------------------------
        private void InitWorkListTab()
        {
            DataTable dtWorkList = stratConfig.GetWorklists();
            dtWorkList.Columns.Add("Status" , Type.GetType("System.String")); //Default Value is Empty
            foreach (DataRow dr in dtWorkList.Rows) dr["Status"] = "";
                
            dtWorkList.DefaultView.RowFilter = "Status <> 'DELETED'";
            dgvWorkList.DataSource = dtWorkList.DefaultView;

            dgvWorkList.Width = 555;
            pnlWorkListDGV.Width = dgvWorkList.Width + 10;
        }
        
        private void InitSortOrderTab()
        {
            //--Populating AscDesc ComboBox---------------------------------------------
            DataTable dtAscDesc = new DataTable();
            dtAscDesc.Columns.Add("Name");
            dtAscDesc.Columns.Add("Value");
            
            DataRow dr1 = dtAscDesc.NewRow();
            dtAscDesc.Rows.Add(dr1);
            dr1["Name"] = "Ascending";
            dr1["Value"] = "ASC";

            DataRow dr2 = dtAscDesc.NewRow();
            dtAscDesc.Rows.Add(dr2);
            dr2["Name"] = "Descending";
            dr2["Value"] = "DESC";

            cmbSortAscDesc.DataSource = dtAscDesc;
            cmbSortAscDesc.ValueMember = "Value";
            cmbSortAscDesc.DisplayMember = "Name";
            //--------------------------------------------------------------------------

            //--Populating Sort Column ComboBox-----------------------------------------
            DataTable dtColumnNames = CollectionsManager.LoadWorkListOrderColumns(out Error).Tables[0]; ;
            cmbSortColumnName.DataSource = dtColumnNames;
            cmbSortColumnName.DisplayMember = "ColumnName";
            cmbSortColumnName.ValueMember = "ColumnName";
            //--------------------------------------------------------------------------

            //--Populating Employee Type ComboBox---------------------------------------
            DataTable dtEmployeeType = CollectionsManager.RolesGet((int)CosacsPermissionEnum.Bailiff).Tables[0];
            cmbEmpType.DataSource = dtEmployeeType;
            cmbEmpType.DisplayMember = "Name";
            cmbEmpType.ValueMember = "RoleId";
            //--------------------------------------------------------------------------

            //--Populating Sort Order DataGridView--------------------------------------
            DataTable dtSortOrder = CollectionsManager.LoadWorkListSortOrder(out Error).Tables[0];
            dgvSortOrder.DataSource = dtSortOrder.DefaultView;
            //--------------------------------------------------------------------------

            //if (dtSortOrder != null && dtSortOrder.Rows.Count > 0)
            //{
            //    cmbEmpType.SelectedValue = dtSortOrder.Rows[0][CN.EmployeeType].ToString();
            //    cmbEmpType_SelectedIndexChanged(cmbEmpType, null);
            //}

            //IP - 15/05/09 - Credit Collection cosmetic changes - hide the 'Employee Type' column.
            dgvSortOrder.Columns[CN.EmployeeType].Visible = false;
            //dgvSortOrder.Width = 473;
            //dgvSortOrder.Height = (dgvSortOrder.RowCount * 22) + 38;
            //pnlSortOrderDGV.Width = 485;
            //pnlSortOrderDGV.Height = dgvSortOrder.Height + 35;

        }

        private void btnAddWorkList_Click(object sender, EventArgs e)
        {
            try
            {
                errorProvider1.SetError(txtWorkListCode, "");
                errorProvider1.SetError(txtWorkListDesc, "");
                
                //-- Validating the Fields -----------------------------------------
                bool valid = true;  

                if(txtWorkListCode.Text.Trim() == "")
                {
                    errorProvider1.SetError(txtWorkListCode, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                if(txtWorkListDesc.Text.Trim()== "")
                {
                    errorProvider1.SetError(txtWorkListDesc, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if(valid == false)
                    return;
                //------------------------------------------------------------------

                if (dgvWorkList.DataSource != null)
                {
                    DataTable dtSource = ((DataView)dgvWorkList.DataSource).Table;
                    DataRow[] drArraySelected = dtSource.Select("WorkListCode = '" + txtWorkListCode.Text.Trim() + "'");

                    if (drArraySelected.Length > 0)
                    {
                        if (drArraySelected[0]["Status"].ToString() != "DELETED" && m_IsWorkListEdit == false &&
                            DialogResult.No == MessageBox.Show("The Work List ( " + txtWorkListCode.Text.Trim() + " ) is already in the list. Are you sure you want to overwrite the record?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            return;
                        }

                        drArraySelected[0]["description"] = txtWorkListDesc.Text.Trim(); 

                        if (drArraySelected[0]["Status"].ToString() != "ADDED")
                            drArraySelected[0]["Status"] = "EDITED";
                    }
                    else
                    {
                        DataRow dr = dtSource.NewRow();
                        dtSource.Rows.Add(dr);
                        dr["WorkListCode"]  = txtWorkListCode.Text.Trim();
                        dr["description"]   = txtWorkListDesc.Text.Trim();
                        dr["Status"]        = "ADDED";
                    }

                    txtWorkListCode.Text = "";
                    txtWorkListDesc.Text = "";
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAddWorkList_Click");
            }
        }

        private void dgvWorkList_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (dgvWorkList.DataSource != null && dgvWorkList.RowCount >= 0 && e.RowIndex >= 0)
                {
                    txtWorkListCode.Text = ((DataView)dgvWorkList.DataSource)[e.RowIndex]["WorkListCode"].ToString().Trim();
                    txtWorkListDesc.Text = ((DataView)dgvWorkList.DataSource)[e.RowIndex]["description"].ToString().Trim();
                    m_IsWorkListEdit = true;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgvWorkList_RowHeaderMouseDoubleClick");
            }
        }

        private void btnSaveWorkList_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvWorkList.DataSource == null)
                {
                    return;
                }

                DataTable dtSource = ((DataView)dgvWorkList.DataSource).Table;
                DataRow[] drModifedArray = dtSource.Select("Status = 'ADDED' OR Status = 'EDITED'");
                DataRow[] drDeletedArray = dtSource.Select("Status = 'DELETED'");

                if (drModifedArray.Length > 0 || drDeletedArray.Length > 0)
                {
                    DataSet dsSave = new DataSet();
                    dsSave.Tables.Add(dtSource.Copy());
                    dsSave.Tables[0].TableName = "MODIFIED"; //Modifed Records
                    dsSave.Tables.Add(dtSource.Copy());
                    dsSave.Tables[1].TableName = "DELETED"; //Deleted Records
                    dsSave.Clear();

                    foreach (DataRow drModified in drModifedArray)
                    {
                        DataRow drSave = dsSave.Tables[0].NewRow();
                        dsSave.Tables[0].Rows.Add(drSave);

                        for (int i = 0; i < dsSave.Tables[0].Columns.Count; i++)
                        {
                            drSave[i] = drModified[i];
                        }
                    }

                    foreach (DataRow drDeleted in drDeletedArray)
                    {
                        DataRow drSave = dsSave.Tables[1].NewRow();
                        dsSave.Tables[1].Rows.Add(drSave);

                        for (int i = 0; i < dsSave.Tables[1].Columns.Count; i++)
                        {
                            drSave[i] = drDeleted[i];
                        }
                    }
                    
                    CollectionsManager.SaveWorkLists(dsSave, out Error);
                    
                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                        return;
                    }
                   
                    //-- Resetting the dgvWorkList DataSource -----------------
                    foreach (DataRow drModified in drModifedArray)
                    {
                        drModified["Status"] = "";
                    }

                    foreach (DataRow drDeleted in drDeletedArray)
                    {
                        dtSource.Rows.Remove(drDeleted);
                    }
                    dgvWorkList.Refresh();
                    //---------------------------------------------------------
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveWorkList_Click");
            }
        }

        //IP - 15/05/09 - Credit Collection Walkthrough cosmetic changes.
        //Replaced with menuDeleteWorklist_Click
        //private void btnRemoveWorkList_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgvWorkList.DataSource == null || dgvWorkList.SelectedRows.Count <= 0)
        //        {
        //            return;
        //        }

        //        string strWorkListCode = ((DataView)dgvWorkList.DataSource)[dgvWorkList.SelectedRows[0].Index]["WorkListCode"].ToString().Trim();
        //        string strStrategy = ((DataView)dgvWorkList.DataSource)[dgvWorkList.SelectedRows[0].Index]["Strategy"].ToString().Trim();

        //        if (strStrategy != "") //This worklist cannot be deleted, It's been used in strategy conditions
        //        {
        //            MessageBox.Show("The Work List (" + strWorkListCode.Trim() + ") cannot be deleted. It's in use with Strategy (" + strStrategy + ")",
        //                            "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
                
        //        DataTable dtSource = ((DataView)dgvWorkList.DataSource).Table;
        //        DataRow[] drArraySelected = dtSource.Select("WorkListCode = '" + strWorkListCode + "'");

        //        if (drArraySelected.Length > 0)
        //        {
        //            //If the record is a newly added it should be physically removed from the underlying table
        //            if (drArraySelected[0]["Status"].ToString() == "ADDED")
        //            {
        //                dtSource.Rows.Remove(drArraySelected[0]);
        //            }
        //            else
        //            {
        //                drArraySelected[0]["Status"] = "DELETED";
        //            }                  
        //        }

        //        dgvWorkList.Refresh();
        //    }
        //    catch(Exception ex)
        //    {
        //        Catch(ex, "btnRemoveWorkList_Click");
        //    }
            
        //}

        private void txtWorkListCode_TextChanged(object sender, EventArgs e)
        {
            m_IsWorkListEdit = false;
        }

        private void cmbEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbEmpType.SelectedIndex >= 0 && dgvSortOrder.DataSource != null)
                {
                    ((DataView)dgvSortOrder.DataSource).RowFilter = CN.EmployeeType + " = '" + cmbEmpType.SelectedValue.ToString().Trim() + "'";
                    dgvSortOrder.Refresh();
                    calcSortOrderGridSize();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "cmbEmpType_SelectedIndexChanged");
            }
            
        }

        //IP - 15/05/09 - Credit Collections Walkthrough Cosmetic changes
        //Replaced with menuDeleteSort_Click
        //private void btnRemoveSortOrder_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dgvSortOrder.DataSource == null || dgvSortOrder.SelectedRows.Count <= 0)
        //        {
        //            return;
        //        }

        //        DataTable dtSource = ((DataView)dgvSortOrder.DataSource).Table;
        //        DataRow dr = ((DataView)dgvSortOrder.DataSource)[dgvSortOrder.SelectedRows[0].Index].Row;
                
        //        if (dr != null)
        //        {
        //            int sortOrderRemoved = Convert.ToInt16(dr["SortOrder"].ToString());
        //            dtSource.Rows.Remove(dr);
                    
        //            //Resettin sort order for existing records
        //            foreach (DataRowView drv in ((DataView)dgvSortOrder.DataSource))
        //            {
        //                if (sortOrderRemoved < Convert.ToInt16(drv["SortOrder"].ToString()))
        //                {
        //                    drv["SortOrder"] = Convert.ToInt16(drv["SortOrder"].ToString()) - 1;
        //                }                        
        //            }                    
        //        }

        //        dgvSortOrder.Refresh();
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "btnRemoveSortOrder_Click");
        //    }
            
        //}

        private void btnAddSortOrder_Click(object sender, EventArgs e)
        {
            try
            {
                errorProvider1.SetError(cmbSortAscDesc, "");
                
                //-- Validating the Fields -----------------------------------------
                bool valid = true;

                if (cmbSortColumnName.SelectedIndex < 0)
                {
                    errorProvider1.SetError(cmbSortAscDesc, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }
                if (cmbSortAscDesc.Text.Trim() == "")
                {
                    errorProvider1.SetError(cmbSortAscDesc, GetResource("M_ENTERMANDATORY"));
                    valid = false;
                }

                if (valid == false)
                    return;
                //------------------------------------------------------------------

                if (dgvSortOrder.DataSource != null)
                {
                    DataTable dtSource = ((DataView)dgvSortOrder.DataSource).Table;
                    DataRow[] drArraySelected = dtSource.Select( //CN.EmployeeType + " = '" + cmbEmpType.SelectedValue.ToString() + "' and " + 
                                                               "ColumnName = '" + cmbSortColumnName.SelectedValue.ToString() + "'");

                    if (drArraySelected.Length > 0)
                    {
                        drArraySelected[0]["AscDesc"] = cmbSortAscDesc.SelectedValue.ToString();
                    }
                    else
                    {
                        DataRow dr = dtSource.NewRow();
                        dtSource.Rows.Add(dr);

                        //dr[CN.EmployeeType] = cmbEmpType.SelectedValue.ToString();
                        //dr[CN.CodeDescript] = cmbEmpType.Text;
                        dr["ColumnName"]    = cmbSortColumnName.SelectedValue.ToString();
                        dr["SortOrder"]     = ((DataView)dgvSortOrder.DataSource).Count.ToString();
                        dr["AscDesc"]       = cmbSortAscDesc.SelectedValue.ToString();
                    }

                    dgvSortOrder.Refresh();
                    cmbSortAscDesc.SelectedIndex = 0;
                    calcSortOrderGridSize();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAddSortOrder_Click");
            }
        }

        private void dgvSortOrder_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (dgvSortOrder.DataSource != null && dgvSortOrder.RowCount >= 0 && e.RowIndex >= 0)
                {
                    cmbSortColumnName.SelectedValue = ((DataView)dgvSortOrder.DataSource)[e.RowIndex]["ColumnName"].ToString();
                    cmbSortAscDesc.SelectedValue = ((DataView)dgvSortOrder.DataSource)[e.RowIndex]["AscDesc"].ToString();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgvSortOrder_RowHeaderMouseDoubleClick");
            }
        }

        private void btnSaveSortOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSortOrder.DataSource == null)
                {
                    return;
                }

                DataTable dtSource = ((DataView)dgvSortOrder.DataSource).Table;
                dtSource.AcceptChanges();
                DataSet dsSave = new DataSet();
                dsSave.Tables.Add(dtSource.Copy());
                dsSave.Tables[0].TableName = "SORT_ORDER";
                dsSave.Tables.Add(((DataTable)cmbEmpType.DataSource).Copy());
                dsSave.Tables[1].TableName = "EMP_TYPE";
                
                CollectionsManager.UpdateWorkListSortOrder(dsSave, out Error);
                if (Error.Length > 0)
                {
                    ShowError(Error);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnSaveSortOrder_Click");
            }
        }
        
        //------------------------------------------------------------------
        #endregion----------------------------------------------------------
        

        //IP - 15/05/09 - Credit Collection Walthrough changes
        //Right-click menu option to delete 
        private void dgvSortOrder_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                DataGridView ctl = (DataGridView)sender;

                MenuCommand deleteSort = new MenuCommand(GetResource("P_DELETE"));
                deleteSort.Click += new System.EventHandler(this.menuDeleteSort_Click);

                deleteSort.Enabled = true;
                deleteSort.Visible = true;

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;

                popup.MenuCommands.Add(deleteSort);

                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
            }

        }
        
        //IP - 15/05/09 - Credit Collection Walthrough changes
        //Right-click menu option to delete 
        private void menuDeleteSort_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgvSortOrder.DataSource == null || dgvSortOrder.SelectedRows.Count <= 0)
                {
                    return;
                }

                DataTable dtSource = ((DataView)dgvSortOrder.DataSource).Table;
                DataRow dr = ((DataView)dgvSortOrder.DataSource)[dgvSortOrder.SelectedRows[0].Index].Row;

                if (dr != null)
                {
                    int sortOrderRemoved = Convert.ToInt16(dr["SortOrder"].ToString());
                    dtSource.Rows.Remove(dr);

                    //Resettin sort order for existing records
                    foreach (DataRowView drv in ((DataView)dgvSortOrder.DataSource))
                    {
                        if (sortOrderRemoved < Convert.ToInt16(drv["SortOrder"].ToString()))
                        {
                            drv["SortOrder"] = Convert.ToInt16(drv["SortOrder"].ToString()) - 1;
                        }
                    }
                }

                dgvSortOrder.Refresh();
                calcSortOrderGridSize();
            }
            catch (Exception ex)
            {
                Catch(ex, "menuDelete_Click");
            }
            

        }
        // Calc grid size 
        private void calcSortOrderGridSize()
        {
            if (dgvSortOrder.RowCount < 10)
            {
                dgvSortOrder.Height = (dgvSortOrder.RowCount * 22) + 38;
                pnlSortOrderDGV.Height = dgvSortOrder.Height + 35;
            }
            else
            {
                dgvSortOrder.Height = 252;
                dgvSortOrder.Width = 445;
            }

        }

        //IP - 15/05/09 - Credit Collection Walkthrough cosmetic changes
        //Right-click delete.
        private void dgvWorkList_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView ctl = (DataGridView)sender;

                MenuCommand deleteWorklist = new MenuCommand(GetResource("P_DELETE"));
                deleteWorklist.Click += new System.EventHandler(this.menuDeleteWorklist_Click);

                deleteWorklist.Enabled = true;
                deleteWorklist.Visible = true;

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;

                popup.MenuCommands.Add(deleteWorklist);

                MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
            }
        }

        //IP - 15/05/09 - Credit Collection Walkthrough cosmetic changes
        //Right-click delete.
        private void menuDeleteWorklist_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvWorkList.DataSource == null || dgvWorkList.SelectedRows.Count <= 0)
                {
                    return;
                }

                string strWorkListCode = ((DataView)dgvWorkList.DataSource)[dgvWorkList.SelectedRows[0].Index]["WorkListCode"].ToString().Trim();
                string strStrategy = ((DataView)dgvWorkList.DataSource)[dgvWorkList.SelectedRows[0].Index]["Strategy"].ToString().Trim();

                if (strStrategy != "") //This worklist cannot be deleted, It's been used in strategy conditions
                {
                    MessageBox.Show("The Work List (" + strWorkListCode.Trim() + ") cannot be deleted. It's in use with Strategy (" + strStrategy + ")",
                                    "Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable dtSource = ((DataView)dgvWorkList.DataSource).Table;
                DataRow[] drArraySelected = dtSource.Select("WorkListCode = '" + strWorkListCode + "'");

                if (drArraySelected.Length > 0)
                {
                    //If the record is a newly added it should be physically removed from the underlying table
                    if (drArraySelected[0]["Status"].ToString() == "ADDED")
                    {
                        dtSource.Rows.Remove(drArraySelected[0]);
                    }
                    else
                    {
                        drArraySelected[0]["Status"] = "DELETED";
                    }
                }

                dgvWorkList.Refresh();
            }
            catch (Exception ex)
            {
                Catch(ex, "btnRemoveWorkList_Click");
            }
        }

        //IP - 02/06/09 - Credit Collection Walkthrough Changes
        //Allocations checkbox - Checkbox is checked if the strategies accounts can be allocated to Bailiff / Collector.
        private void StrategyAllocationCheck()
        {

            int index = 0;

            index = Strategies.SelectedIndex;

            if (Convert.ToString(dtStrategies.Rows[index][CN.Reference]) == "")
            {
                chkAllocations.Checked = false;
            }
            else
            {
                chkAllocations.Checked = Convert.ToBoolean(Convert.ToInt32(dtStrategies.Rows[index][CN.Reference]));
            }
        }

        //IP - 02/06/09 - Credit Collections Walkthrough Changes - Allocations Check.
        private void chkAllocations_CheckedChanged(object sender, EventArgs e)
        {
            int index = 0;

            index = Strategies.SelectedIndex;

            dtStrategies.Rows[index][CN.Reference] = Convert.ToInt32(chkAllocations.Checked);

        }

        //IP - 03/06/09 - Credit Collection Walkthrough Changes.
        private void chkAllocations_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(chkAllocations, "Determines if accounts in a strategy can be allocated to Bailiff / Collectors.");
        }
        //jec - 18/09/09 UAT856
        private void cbManual_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cbManual, "Indicates that accounts are only sent to this strategy by a manual action");
        }

        //IP - 21/009/09 - UAT(856)
        private void cbManual_CheckedChanged(object sender, EventArgs e)
        {
        
            dtStrategies.Rows[Strategies.SelectedIndex][CN.Manual] = cbManual.Checked;
        }

        //IP - 02/12/09 - UAT5.2 (929) - Moved code from  private void DataGridPossibleEntryConditions_RowEnter
        /// <summary>
        /// Event to handle the selection of a possible 'Entry' condition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridPossibleEntryConditions_Click(object sender, EventArgs e)
        {
            //UAT755
            EntryConditionsOperators.Enabled = false;
            EntryConditionsValue1.ReadOnly = true;
            EntryConditionsValue2.ReadOnly = true;
            drpStrategy.Visible = false;
            lbStrategy.Visible = false;

            AddEntryConditions.Enabled = true; //IP - 02/12/09 - UAT5.2 (929)

            //IP - 02/12/09 - UAT5.2 (929)
            string tabSelected = tabControlStrategies.SelectedTab.Name;
            string conditionCode = string.Empty;
            int selectedIndex = DataGridPossibleEntryConditions.CurrentRow.Index;
            //bool disableAddCondition = false;                                     //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            RemoveErrorProvider(AddEntryConditions);

            //If the Type is ' ' (indicating this condition can be used for 'Entry', 'Steps' and 'Exit') then we need to check if it is currently being used as a step
            // or an exit condition. If it is being used, then we do not want the user to add this condition, therefore disable the button that adds the condition.

            //if (Convert.ToString(DataGridPossibleEntryConditions[CN.Type, selectedIndex].Value) == " ")           //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            //{
            //    conditionCode = Convert.ToString(DataGridPossibleEntryConditions[CN.ConditionCode, selectedIndex].Value);
            //    disableAddCondition = disableUsingCondition(tabSelected, conditionCode);
            //}

            //IP - 02/12/09 - UAT5.2 (929) 
            //If the condition is being used as a 'Step' or 'Exit' condition, then we want to prevent the user from adding this condition.
            //if (disableAddCondition == false)                                                                       //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            //{
                if (DataGridPossibleEntryConditions[CN.OperandAllowable, selectedIndex].Value.ToString() == "1")
                {
                    EntryConditionsOperators.Enabled = true;
                    EntryConditionsValue1.ReadOnly = false;
                    EntryConditionsValue2.ReadOnly = true;
                }
                else if (DataGridPossibleEntryConditions[CN.OperandAllowable, selectedIndex].Value.ToString() == "2")
                {
                    EntryConditionsOperators.Enabled = true;
                    EntryConditionsValue1.ReadOnly = false;
                    EntryConditionsValue2.ReadOnly = false;
                }
                else if (DataGridPossibleEntryConditions[CN.OperandAllowable, selectedIndex].Value.ToString() == "P")      //UAT755
                {
                    drpStrategy.Enabled = true;
                    drpStrategy.Visible = true;
                    lbStrategy.Visible = true;
                }
                else
                {
                    EntryConditionsOperators.Enabled = false;
                    EntryConditionsValue1.ReadOnly = true;
                    EntryConditionsValue2.ReadOnly = true;
                    //UAT755
                    drpStrategy.Visible = false;
                    lbStrategy.Visible = false;
                }
                //}                                                                                                        //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                //else //IP - 02/12/09 - UAT5.2 (929)                                                                      //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                //{                                                                                                        //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            //    AddEntryConditions.Enabled = false;
            //    errorProvider1.SetError(AddEntryConditions, GetResource("M_CANNOTADDENTRYCONDITION"));
            //}

            EntryConditionsValue1.Text = "";
            EntryConditionsValue2.Text = "";

            EntryConditionsOperators.SelectedItem = DropDownValues.Operator;

        }

        //IP - 02/12/09 - UAT5.2 (929)
        /// <summary>
        /// Method determines if a condition selected to be added from 'Entry', 'Steps', 'Exit' tab 
        /// is currently being used as a condition in one of the tabs. This method is only called
        /// for conditions where the Type = ' ' (can appear under all three tabs). If the condition
        /// is already being used in one of the tabs, then we do not want the user to be able to add the condition.
        /// </summary>
        /// <returns></returns>
        //private bool disableUsingCondition(string tabSelected, string conditionCode)
        //{
        //    bool disableCondition = false;

        //    switch (tabSelected)
        //    {
        //        case ("tabPageEntryConditions"):

        //            foreach (DataRow dr in dtChosenStepConditions.Rows)
        //            {
        //                if (Convert.ToString(dr[CN.ConditionCode]) == conditionCode)
        //                {
        //                    disableCondition = true;
        //                    break;
        //                }
        //            }

        //            if (disableCondition == false)
        //            {
        //                foreach (DataRow dr in dtChosenExitConditions.Rows)
        //                {
        //                    if (Convert.ToString(dr[CN.ConditionCode]) == conditionCode)
        //                    {
        //                        disableCondition = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //        case ("tabPageSteps"):

        //            foreach (DataRow dr in dtChosenEntryConditions.Rows)
        //            {
        //                if (Convert.ToString(dr[CN.ConditionCode]) == conditionCode)
        //                {
        //                    disableCondition = true;
        //                    break;
        //                }
        //            }

        //            if (disableCondition == false)
        //            {
        //                foreach (DataRow dr in dtChosenExitConditions.Rows)
        //                {
        //                    if (Convert.ToString(dr[CN.ConditionCode]) == conditionCode)
        //                    {
        //                        disableCondition = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            break;
        //        case ("tabPageExitConditions"):

        //            foreach (DataRow dr in dtChosenEntryConditions.Rows)
        //            {
        //                if (Convert.ToString(dr[CN.ConditionCode]) == conditionCode)
        //                {
        //                    disableCondition = true;
        //                    break;
        //                }
        //            }

        //            if (disableCondition == false)
        //            {
        //                foreach (DataRow dr in dtChosenStepConditions.Rows)
        //                {
        //                    if (Convert.ToString(dr[CN.ConditionCode]) == conditionCode)
        //                    {
        //                        disableCondition = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            break; 
                   
        //    }
        //    return disableCondition;
        //}

        //IP - 02/12/09 - UAT5.2 (929) - Moved code from private void dataGridPossibleExitConditions_RowEnter
        /// <summary>
        /// Event to handle the selection of a possible 'Exit' condition.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridPossibleExitConditions_Click(object sender, EventArgs e)
        {
           
            //IP - 02/12/09 - UAT5.2 (929)
            AddExitConditions.Enabled = true;
            string tabSelected = tabControlStrategies.SelectedTab.Name;
            string conditionCode = string.Empty;
            int selectedIndex = dataGridPossibleExitConditions.CurrentRow.Index;
            //bool disableAddCondition = false;                                     //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            //RemoveErrorProvider(AddExitConditions);                               //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929

            //If the Type is ' ' (indicating this condition can be used for 'Entry', 'Steps' and 'Exit') then we need to check if it is currently being used as a entry     //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            // or step condition. If it is being used, then we do not want the user to add this condition, therefore disable the button that adds the condition.
            //if (Convert.ToString(dataGridPossibleExitConditions[CN.Type, selectedIndex].Value) == " ")
            //{
            //    conditionCode = Convert.ToString(dataGridPossibleExitConditions[CN.ConditionCode, selectedIndex].Value);
            //    disableAddCondition = disableUsingCondition(tabSelected, conditionCode);
            //}

            //if (disableAddCondition == false)                                        //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            //{
                //AddExitConditions.Enabled = true;
                if (dataGridPossibleExitConditions[CN.OperandAllowable, selectedIndex].Value.ToString() == "1")
                {
                    ExitConditionsOperators.Enabled = true;
                    ExitConditionsV1.ReadOnly = false;
                    ExitConditionsV2.ReadOnly = true;
                }
                else if (dataGridPossibleExitConditions[CN.OperandAllowable, selectedIndex].Value.ToString() == "2")
                {
                    ExitConditionsOperators.Enabled = true;
                    ExitConditionsV1.ReadOnly = false;
                    ExitConditionsV2.ReadOnly = false;
                }
                else
                {
                    ExitConditionsOperators.Enabled = false;
                    ExitConditionsV1.ReadOnly = true;
                    ExitConditionsV2.ReadOnly = true;
                }
                //}                                                                      //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
                //else                                                                    //IP - 16/03/12 - #9803 - LW74600 - Reversing change UAT929
            //{
            //    AddExitConditions.Enabled = false;
            //    errorProvider1.SetError(AddExitConditions, GetResource("M_CANNOTADDEXITCONDITION"));
            //}

            ExitConditionsV1.Text = "";
            ExitConditionsV2.Text = "";

            ExitConditionsOperators.SelectedItem = DropDownValues.Operator;
        }


    }

   public class Conditions
   {
      public string condtion;
      public string nextCondition;
      internal bool trueStep;
      //public int step;

      public Conditions(string condition, string nextCondition,bool trueStep)
      {
         //this.step = step;
         this.condtion = condition;
         this.nextCondition = nextCondition;
         this.trueStep = trueStep;
      }
   }
}