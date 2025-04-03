using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using Crownwood.Magic.Menus;
using Microsoft.Win32;

namespace STL.PL.Collections
{
    public partial class WorkLists : CommonForm
    {
        private static class NodeTypes
        {
            public static NodeType Employee = new NodeType("Employee");
            public static NodeType AllEmployee = new NodeType("AllEmployee");
            public static NodeType EmployeeGroup = new NodeType("EmployeeGroup");
            public static NodeType Branch = new NodeType("Branch");
            public static NodeType WorkList = new NodeType("WorkList");
            public static NodeType Action = new NodeType("Action");
            public static NodeType Strategy = new NodeType("Strategy");
            public static NodeType AllStrategy = new NodeType("AllStrategy");
            public static NodeType Undefined = new NodeType("Undefined");
        }

        #region --Private Member Variables------------------------------------------------
        //--------------------------------------------------------------------------------

        private string error = "";
        DataSet dsEmployeeWithAllocations;
        private DataTable dtWorkListRights;
        private DataTable dtActionRights;
        private Timer timer;    // Timer for scrolling
        private CMTreeNode tempDropNode = null;  // Temporary drop node for selection    
        private CMTreeNode dragNode = null;
        UserRight allowAddUserToSupWorkList = UserRight.Create("AllowAddUserToSUPWorkList");
        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------

        #region --Private Methods---------------------------------------------------------
        //--------------------------------------------------------------------------------

        private bool PopulateEmployeeTree()
        {
            dsEmployeeWithAllocations = CollectionsManager.LoadAllocateableStaffandTypes(out error);

            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }

            treeViewEmployees.Nodes.Clear();

            if (dsEmployeeWithAllocations.Tables.Count > 0)
            {
                #region --Populate TreeView (Employee-Groups, Branches, Employees)----------------
                //--------------------------------------------------------------------------------

                var strEmpNo = "";
                var strEmpName = "";
                var strEmpGroup = "";
                var strEmpGroupDesc = "";
                var strBranch = "";

                CMTreeNode empGroupNode;

                foreach (DataRow dr in dsEmployeeWithAllocations.Tables[0].Rows)
                {
                    strEmpNo = dr[CN.EmployeeNo].ToString();
                    strEmpName = dr[CN.EmployeeName].ToString();
                    strEmpGroup = dr[CN.EmployeeType].ToString();
                    strEmpGroupDesc = dr["EmpeetypeDescription"].ToString();
                    strBranch = dr[CN.BranchNo].ToString();

                    //-- Employee Group---------------------------------------------------------------
                    empGroupNode = treeViewEmployees.Nodes.FindCMNode(strEmpGroup, false);

                    if (empGroupNode == null)
                    {
                        empGroupNode = treeViewEmployees.Nodes.AddCMNode(strEmpGroup, strEmpGroupDesc, 0, NodeTypes.EmployeeGroup);

                        //-- Adding the All-Employee-Node immediately under Employee-Group-Node --
                        empGroupNode
                            .Nodes.AddCMNode("ALL_" + strEmpGroup, "All " + strEmpGroupDesc, 2, NodeTypes.AllEmployee);
                    }
                    //--------------------------------------------------------------------------------

                    empGroupNode
                        .Nodes.AddCMNode(strBranch, strBranch, 3, NodeTypes.Branch)
                        .Nodes.AddCMNode(string.Format("{0}:{1}", strEmpNo, dr["EmpeeType"]), strEmpNo + " " + strEmpName, 1, NodeTypes.Employee);
                }

                //--------------------------------------------------------------------------------
                #endregion -----------------------------------------------------------------------
            }

            if (dsEmployeeWithAllocations.Tables.Count > 2) //12/09/11 - NM/IP #4561 - UAT50
            {
                #region --Populate TreeView (Action-Rights, Strategies)---------------------------
                //--------------------------------------------------------------------------------                    

                var strEmpNo = "";
                var strEmpGroup = "";
                var strAction = "";
                var strActionDesc = "";
                var strStrategy = "";
                var strStrategyDesc = "";
                DataView dvActionRights = dsEmployeeWithAllocations.Tables[2].DefaultView; //12/09/11 - NM/IP #4561 - UAT50

                CMTreeNode empNode = null;

                foreach (DataRow dr in dsEmployeeWithAllocations.Tables[1].Rows)
                {
                    strEmpNo = dr[CN.EmployeeNo].ToString();
                    strEmpGroup = dr[CN.EmployeeType].ToString();


                    if (strEmpNo.Trim() == "0") //-- Special Case where EmpNo = '0'. Then Action-Node should appear under All-Employee-Node
                        empNode = treeViewEmployees.Nodes.FindCMNode("ALL_" + strEmpGroup, true);
                    else
                        empNode = treeViewEmployees.Nodes.FindCMNode(string.Format("{0}:{1}",strEmpNo,dr["EmpeeType"]), true);

                    if (empNode == null)
                        continue; //cannot find the Employee-Node, so continue with the next record

                    dvActionRights.RowFilter = string.Format("{0}='{1}' and {2}='{3}'", CN.EmployeeNo, strEmpNo, CN.EmployeeType, strEmpGroup);//12/09/11 - NM/IP #4561 - UAT50

                    foreach (DataRowView drv in dvActionRights)
                    {
                        strAction = drv[CN.Action].ToString();
                        strActionDesc = drv["Description"].ToString();
                        strStrategy = drv[CN.Strategy].ToString().Trim() == "" ? "ALL" : drv[CN.Strategy].ToString().Trim();
                        strStrategyDesc = drv["StrategyDesc"].ToString() == "" ? "ALL" : drv["StrategyDesc"].ToString();

                        if (dr["EmpeeType"].ToString() == drv["EmpeeType"].ToString())
                        {
                            empNode
                                .Nodes.AddCMNode(strAction, strActionDesc, 6, NodeTypes.Action, toolTip: "Action: " + strActionDesc)  //Action-Node
                                .Nodes.AddCMNode(strStrategy, strStrategyDesc, 4, NodeTypes.Strategy, toolTip: "Strategy: " + strStrategyDesc);  //Strategy-Node 
                        }
                    }
                }

                //--------------------------------------------------------------------------------
                #endregion -----------------------------------------------------------------------
            }

            if (dsEmployeeWithAllocations.Tables.Count > 4)
            {
                #region --Populate TreeView (WorkList-Rights)-------------------------------------
                //--------------------------------------------------------------------------------

                var strEmpNo = "";
                var strEmpGroup = "";
                var strWorkList = "";
                var strWorkListDesc = "";

                CMTreeNode empNode = null;
                DataView dvWorklistRights = dsEmployeeWithAllocations.Tables[4].DefaultView; //12/09/11 - NM/IP #4561 - UAT50

                foreach (DataRow dr in dsEmployeeWithAllocations.Tables[3].Rows)
                {
                    strEmpNo = dr[CN.EmployeeNo].ToString();
                    strEmpGroup = dr[CN.EmployeeType].ToString();

                    //-- Special Case where EmpNo = '0' --
                    if (strEmpNo.Trim() == "0")
                        empNode = treeViewEmployees.Nodes.FindCMNode("ALL_" + strEmpGroup, true);
                    else
                        empNode = treeViewEmployees.Nodes.FindCMNode(string.Format("{0}:{1}", strEmpNo, dr["EmpeeType"]), true);

                    if (empNode == null)
                        continue; //cannot find the Employee-Node, so continue with the next record

                    dvWorklistRights.RowFilter = string.Format("{0}='{1}' and {2}='{3}'", CN.EmployeeNo, strEmpNo, CN.EmployeeType, strEmpGroup); //12/09/11 - NM/IP #4561 - UAT50

                    foreach (DataRowView drv in dvWorklistRights)          //12/09/11 - NM/IP #4561 - UAT50
                    {
                        strWorkList = drv[CN.WorkList].ToString();
                        strWorkListDesc = drv["Worklistdesc"].ToString();

                        if (dr["EmpeeType"].ToString() == drv["EmpeeType"].ToString())
                        {
                            empNode
                                .Nodes.AddCMNode(strWorkList, strWorkListDesc, 5, NodeTypes.WorkList, toolTip: "WorkList: " + strWorkListDesc);
                        }
                    }
                }

                //--------------------------------------------------------------------------------
                #endregion -----------------------------------------------------------------------
            }

            return true;
        }

        private bool PopulateWorkListTree()
        {
            DataSet dsWorkList = CollectionsManager.GetWorkLists(out error);

            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }

            if (dsWorkList.Tables.Count > 0)
            {
                #region --Populate TreeView (Strategies, WorkLists)--------------------------------
                //--------------------------------------------------------------------------------

                var strStrategy = "";
                var strStrategyDesc = "";
                var strWorkList = "";
                var strWorkListDesc = "";

                //UAT(5.2) - 743
                CMTreeNode supervisorStrategyNode = null;
                var drArray = dsWorkList.Tables[0].Select("Column1 = 'SUP'");

                //if (drArray.Length > 0)
                //    supervisorStrategyNode = treeViewWorkList
                //                                .Nodes.AddCMNode(drArray[0]["Column1"].ToString(), drArray[0][CN.Strategy].ToString(), 4, NodeTypes.Strategy);

                CMTreeNode strategyNode = null;

                foreach (DataRow dr in dsWorkList.Tables[0].Rows)
                {
                    
                    
                    strStrategy = dr["Column1"].ToString();
                    strStrategyDesc = dr[CN.Strategy].ToString();
                    strWorkList = dr[CN.WorkList].ToString();
                    strWorkListDesc = dr["Description"].ToString();

                    if (!allowAddUserToSupWorkList.IsAllowed && strStrategy == "SUP")
                    {
                        continue;
                    }
                    else
                    {
                        strategyNode = treeViewWorkList
                                 .Nodes.AddCMNode(strStrategy, strStrategyDesc, 4, NodeTypes.Strategy);

                        strategyNode
                            .Nodes.AddCMNode(strWorkList, strWorkListDesc, 5, NodeTypes.WorkList);
                    }

                    //UAT(5.2) - 743 - Listing all the worklists under supervisor strategy
                    //if (supervisorStrategyNode != null && supervisorStrategyNode != strategyNode)
                    //{
                    //    supervisorStrategyNode
                    //        .Nodes.AddCMNode(strWorkList, strWorkListDesc, 5, NodeTypes.WorkList);
                    //}
                }

                treeViewWorkList.Sort();

                //--------------------------------------------------------------------------------
                #endregion -----------------------------------------------------------------------
            }

            return true;
        }

        private bool PopulateActionTree()
        {
            DataSet dsAction = CollectionsManager.GetActionsWithStrategy(out error);

            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }

            if (dsAction.Tables.Count > 0)
            {
                #region --Populate TreeView (Strategies, Actions)----------------------------------
                //--------------------------------------------------------------------------------

                var strStrategy = "";
                var strStrategyDesc = "";
                var strAction = "";
                var strActionDesc = "";

                //Adding the All-Strategy-Node before other strategies UAT(5.2) - 743
                var allStrategyNode = treeViewAction
                                        .Nodes.AddCMNode("N_A", "Actions for All Strategies", 4, NodeTypes.AllStrategy);

                CMTreeNode strategyNode = null;

                foreach (DataRow dr in dsAction.Tables[0].Rows)
                {
                    strStrategy = dr[CN.Strategy].ToString();
                    strStrategyDesc = dr[CN.Description].ToString();
                    strAction = dr[CN.Code].ToString();
                    strActionDesc = dr[CN.CodeDescript].ToString();

                    strategyNode = treeViewAction
                                    .Nodes.AddCMNode(strStrategy, strStrategyDesc, 4, NodeTypes.Strategy);

                    strategyNode
                        .Nodes.AddCMNode(strAction, strActionDesc, 6, NodeTypes.Action);

                    //UAT(5.2) - 743 - Listing all the worklists under supervisor strategy
                    if (allStrategyNode != null && allStrategyNode != strategyNode)
                        allStrategyNode.Nodes.AddCMNode(strAction, strActionDesc, 6, NodeTypes.Action);
                }

                //--------------------------------------------------------------------------------
                #endregion -----------------------------------------------------------------------
            }

            return true;
        }

        private bool PopulateWorkListGridView()
        {
            DataSet dsWorkListView = CollectionsManager.GetWorklistRightsHierarchy(out error);

            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }

            if (dsWorkListView.Tables.Count > 0)
                dgvWorkListView.DataSource = dsWorkListView.Tables[0];
            else
                dgvWorkListView.DataSource = null;

            dgvWorkListView.AutoGenerateColumns = false;

            return true;
        }

        private bool PopulateActionGridView()
        {
            DataSet dsActionView = CollectionsManager.GetActionRightsHierarchy(out error);

            if (error.Length > 0)
            {
                ShowError(error);
                return false;
            }

            if (dsActionView.Tables.Count > 0)
                dgvActionView.DataSource = dsActionView.Tables[0];
            else
                dgvActionView.DataSource = null;

            dgvActionView.AutoGenerateColumns = false;

            return true;
        }

        private DataTable CreateActionRightsTable()
        {
            DataTable dtReturn = new DataTable("ActionRights");
            dtReturn.Columns.Add("Key");
            dtReturn.Columns.Add(CN.EmployeeNo);
            dtReturn.Columns.Add(CN.Strategy);
            dtReturn.Columns.Add(CN.Action);
            dtReturn.Columns.Add(CN.EmployeeType);
            dtReturn.Columns.Add(CN.CycleToNextFlag);
            dtReturn.Columns.Add(CN.MinNotesLength);
            dtReturn.Columns.Add("AddRemoveFlag");

            return dtReturn;
        }

        private DataTable CreateWorkListRightsTable()
        {
            DataTable dtReturn = new DataTable("WorkListRights");
            dtReturn.Columns.Add("Key");
            dtReturn.Columns.Add(CN.WorkList);
            dtReturn.Columns.Add(CN.EmployeeType);
            dtReturn.Columns.Add("AddRemoveFlag");

            return dtReturn;
        }

        private CMTreeNode GetEmployeeGroupNode(CMTreeNode empNode)
        {
            var tempNode = empNode;

            do
            {
                if (tempNode.NodeType == NodeTypes.EmployeeGroup)
                    return tempNode;

                tempNode = (CMTreeNode)tempNode.Parent;
            }
            while (tempNode != null);

            return null;
        }

        private void AddEmployeeActionRights(NodeType empNodeType, string strKey, string strEmpGroup, string strStrategy, string strAction, int minNoteLength, bool cycleToNextFlag)
        {
            if (empNodeType == NodeTypes.AllEmployee)
                strKey = "0:" + strEmpGroup;

            var drArray = dtActionRights.Select("Key = '" + strKey + "' AND "
                                + CN.Strategy + " = '" + strStrategy + "' AND " + CN.Action + " = '" + strAction + "' AND "
                                + CN.EmployeeType + " = '" + strEmpGroup + "' AND AddRemoveFlag = 'REMOVE' ");

            if (drArray.Length > 0)
            {
                dtActionRights.Rows.Remove(drArray[0]);
                //drArray[0][CN.CycleToNextFlag] = cycleToNextFlag.ToString();
                //drArray[0][CN.MinNotesLength]  = minNoteLength.ToString();
                //drArray[0]["AddRemoveFlag"]    = "UPDATE";
            }
            else
            {
                var dr = dtActionRights.NewRow();
                dtActionRights.Rows.Add(dr);
                dr["Key"] = strKey;
               // dr[CN.EmployeeNo] = strEmpNo;
                dr[CN.Strategy] = strStrategy;
                dr[CN.Action] = strAction;
                dr[CN.EmployeeType] = strEmpGroup;
                dr[CN.CycleToNextFlag] = cycleToNextFlag.ToString();
                dr[CN.MinNotesLength] = minNoteLength.ToString();
                dr["AddRemoveFlag"] = "ADD";
            }
        }

        private void RemoveEmployeeActionRights(NodeType empNodeType, string strKey, string strEmpGroup, string strStrategy, string strAction)
        {
            if (empNodeType == NodeTypes.AllEmployee)
                strKey = "0:" + strEmpGroup;

            var drArray = dtActionRights.Select("Key = '" + strKey + "' AND "
                                + CN.Strategy + " = '" + strStrategy + "' AND " + CN.Action + " = '" + strAction + "' AND "
                                + CN.EmployeeType + " = '" + strEmpGroup + "' AND AddRemoveFlag = 'ADD' ");

            if (drArray.Length > 0)
            {
                dtActionRights.Rows.Remove(drArray[0]);
            }
            else
            {
                var dr = dtActionRights.NewRow();
                dtActionRights.Rows.Add(dr);
                dr["Key"] = strKey;
                //dr[CN.EmployeeNo] = strEmpNo;
                dr[CN.Strategy] = strStrategy;
                dr[CN.Action] = strAction;
                dr[CN.EmployeeType] = strEmpGroup;
                dr[CN.CycleToNextFlag] = "False";
                dr[CN.MinNotesLength] = "0";
                dr["AddRemoveFlag"] = "REMOVE";
            }
        }

        private void AddEmployeeWorkListRights(NodeType empNodeType, string strKey, string strEmpGroup, string strWorklist)
        {
            if (empNodeType == NodeTypes.AllEmployee)
                strKey = "0:" + strEmpGroup;

            var drArray = dtWorkListRights.Select("Key = '" + strKey + "' AND "
                                + CN.WorkList + " = '" + strWorklist + "' AND " + CN.EmployeeType + " = '" + strEmpGroup + "' AND "
                                + "AddRemoveFlag = 'REMOVE' ");

            if (drArray.Length > 0)
            {
                dtWorkListRights.Rows.Remove(drArray[0]);
            }
            else
            {
                var dr = dtWorkListRights.NewRow();
                dtWorkListRights.Rows.Add(dr);

                dr["Key"] = strKey;
                dr[CN.WorkList] = strWorklist;
                dr[CN.EmployeeType] = strEmpGroup;
                dr["AddRemoveFlag"] = "ADD";
            }
        }

        private void RemoveEmployeeWorkListRights(NodeType empNodeType, string strKey, string strEmpGroup, string strWorklist)
        {
            if (empNodeType == NodeTypes.AllEmployee)
                strKey = "0:" + strEmpGroup;

            var drArray = dtWorkListRights.Select("Key = '" + strKey + "' AND "
                                + CN.WorkList + " = '" + strWorklist + "' AND " + CN.EmployeeType + " = '" + strEmpGroup + "' AND "
                                + "AddRemoveFlag = 'ADD' ");

            if (drArray.Length > 0)
            {
                dtWorkListRights.Rows.Remove(drArray[0]);
            }
            else
            {
                var dr = dtWorkListRights.NewRow();
                dtWorkListRights.Rows.Add(dr);

                dr["Key"] = strKey;
                dr[CN.WorkList] = strWorklist;
                dr[CN.EmployeeType] = strEmpGroup;
                dr["AddRemoveFlag"] = "REMOVE";
            }
        }

        private void ExportToExcel(DataGridView dataGridView)
        {
            if (dataGridView.DataSource == null || dataGridView.Rows.Count <= 0)
                return;

            Excel.ApplicationClass excelApp = new Excel.ApplicationClass();

            excelApp.Application.Workbooks.Add(true);

            //--Writing the headers ---------------------------------------------
            int columnIndex = 0;
            foreach (DataGridViewColumn dgvc in dataGridView.Columns)
            {
                if (dgvc.Visible == false)
                    continue;

                columnIndex++;
                excelApp.Cells[1, columnIndex] = dgvc.HeaderText;
                ((Excel.Range)excelApp.Cells[1, columnIndex]).Interior.Color = Color.BlanchedAlmond.ToArgb();
                ((Excel.Range)excelApp.Cells[1, columnIndex]).EntireColumn.NumberFormat = "@";
            }

            //-------------------------------------------------------------------

            //--Writing the data ------------------------------------------------
            int rowIndex = 1;
            foreach (DataGridViewRow dgvr in dataGridView.Rows)
            {
                rowIndex++;
                columnIndex = 0;
                foreach (DataGridViewColumn dgvc in dataGridView.Columns)
                {
                    if (dgvc.Visible == false)
                        continue;

                    columnIndex++;
                    excelApp.Cells[rowIndex, columnIndex] = dgvr.Cells[dgvc.Name].Value.ToString();
                }
            }
            //-------------------------------------------------------------------

            excelApp.Visible = true;
            Excel.Worksheet worksheet = (Excel.Worksheet)excelApp.ActiveSheet;
            worksheet.Columns.AutoFit();
            worksheet.Activate();
        }

        private bool IsExcelInstalled()
        {
            var excelKey = Registry.ClassesRoot.OpenSubKey("Excel.Application");
            return excelKey != null;
        }
        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------

        #region --Commented Old Code------------------------------------------------------
        //--------------------------------------------------------------------------------

        /*
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtWorkList.Text = "";
            txtDescription.Text = "";
            //drpWorkLists.SelectedIndex = 0;
            //drpAction.SelectedIndex = 0;
            rbExit.Checked = true;
            rbLeave.Checked = false;

            //foreach (DataRowView row in dvEmployees)
            //{
            //   row[CN.Assigned] = false;
            //}
            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
               row.Cells[CN.Assigned].Value = false;
            }

            foreach (DataGridViewRow row in dgvActions.Rows)
            {
               row.Cells[CN.Assigned].Value = false;
               row.Cells[CN.Exit].Value = false;
            }
        }
        */
        /*private void btnNewWorkList_Click(object sender, EventArgs e)
        {
            txtWorkList.Enabled = true;
        }
            
       private DataTable FilterActions(DataView dv, string worklist)
       {
          dv.RowFilter = "Worklist = '" + worklist + "' OR Worklist IS NULL";
          DataTable dt = new DataTable();
          dt = dv.ToTable();
          return dt;
       }*/

        /*private void PopulateActionAssignedValues(string workList)
       {
           //First set all the assigned values to false
          foreach (DataGridViewRow actionRow in dgvActions.Rows)
          {
             actionRow.Cells[CN.Assigned].Value = false;
             actionRow.Cells[CN.Exit].Value = false;
          }
           //Filter actions according to worklist selected
           DataTable dtWorkListActions = new DataTable();
           dtWorkListActions = FilterActions(dvWorkListActions, drpWorkLists.SelectedValue.ToString());
           foreach (DataRow row in dtWorkListActions.Rows)
           {
              string action = row[CN.Action].ToString();
              foreach (DataGridViewRow actionRow in dgvActions.Rows)
              {               
                 int index = dgvActions.Rows.IndexOf(actionRow);
                 if (actionRow.Cells[CN.Code].Value.ToString() == action)
                 {
                    if (Convert.ToInt16(row[CN.Exit]) == 0)
                    {
                       dgvActions[CN.Assigned, index].Value = true;
                    }
                    dgvActions[CN.Exit, index].Value = row[CN.Exit];
                 }             
              }
           }
        }*/


        //private void btnSave1_Click(object sender, EventArgs e)
        //{
        //    /*try
        //    {
        //        string workList = "";
        //        string effect = "";

        //        bool valid = true;
        //        /*
        //        errorProvider1.SetError(drpWorkLists, "");
        //        errorProvider1.SetError(txtWorkList, "");
        //        errorProvider1.SetError(txtDescription, "");
        //        errorProvider1.SetError(dgvEmployees, "");

        //        if (drpWorkLists.Text.Length == 0 && txtWorkList.Text.Length == 0)
        //        {
        //            errorProvider1.SetError(drpWorkLists, GetResource("M_INVALIDWORKLIST"));
        //            errorProvider1.SetError(txtWorkList, GetResource("M_INVALIDWORKLIST"));
        //            valid = false;
        //        }

        //        if (txtDescription.Text.Length == 0)
        //        {
        //            errorProvider1.SetError(txtDescription, GetResource("M_ENTERMANDATORY"));
        //            valid = false;
        //        }

        //        if (valid)
        //        {
        //            valid = workListData.EmployeeListValid(dvEmployees);
        //            if(!valid)
        //                errorProvider1.SetError(dgvEmployees, GetResource("M_WORKLISTEMPLOYEE"));
        //        }

        //        if (valid)
        //        {
        //            DataTable dtWorkList = workListData.BuildEmployeeList(dvEmployees);
        //            DataTable dtActions = workListData.BuildActionsList(dvActions);

        //            DataSet dsWorkList = new DataSet();
        //            dsWorkList.Tables.Add(dtWorkList);
        //            dsWorkList.Tables.Add(dtActions);

        //            if (txtWorkList.Text.Length > 0)
        //                workList = txtWorkList.Text;
        //            else
        //                workList = drpWorkLists.Text;

        //            //if (rbExit.Checked)
        //            //    effect = exit;

        //            //if (rbLeave.Checked)
        //            //    effect = leave;

        //            CollectionsManager.SaveWorkList(workList, txtDescription.Text, "",dsWorkList, out error);

        //            if (error.Length > 0)
        //                ShowError(error);
        //            else
        //            {
        //                btnClear_Click(this, null);
        //                WorkLists_Load(this, null);
        //            }
        //            drpWorkLists.SelectedItem = workList;
        //            txtWorkList.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }*/
        //}

        //private void btnDeleteWorkList_Click(object sender, EventArgs e)
        //{
        //    Function = "btnDeleteWorkList_Click";
        //    try
        //    {
        //        if (DialogResult.Yes == ShowInfo("M_DELETEWORKLIST", MessageBoxButtons.YesNo))
        //        {
        //            //workListData.DeleteWorkList(drpWorkLists.SelectedValue.ToString());
        //            //btnClear_Click(this, null);
        //            WorkLists_Load(this, null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //    finally
        //    {
        //        Function = "End of btnDeleteWorkList_Click";
        //    }
        //}

        //private void timer_Tick(object sender, EventArgs e)
        //{
        //    // get node at mouse position
        //    var pt = PointToClient(Control.MousePosition);
        //    var node = (CMTreeNode)treeViewEmployees.GetNodeAt(pt);

        //    if (node == null)
        //        return;

        //    // if mouse is near to the top, scroll up
        //    if (pt.Y < 30)
        //    {
        //        // set actual node to the upper one
        //        if (node.PrevVisibleNode != null)
        //        {
        //            node = (CMTreeNode)node.PrevVisibleNode;

        //            DragHelper.ImageList_DragShowNolock(false); // hide drag image
        //            // scroll and refresh
        //            node.EnsureVisible();
        //            treeViewEmployees.Refresh();
        //            DragHelper.ImageList_DragShowNolock(true); // show drag image
        //        }
        //    }
        //    // if mouse is near to the bottom, scroll down
        //    else if (pt.Y > treeViewEmployees.Size.Height - 30)
        //    {
        //        if (node.NextVisibleNode != null)
        //        {
        //            node = (CMTreeNode)node.NextVisibleNode;

        //            DragHelper.ImageList_DragShowNolock(false);
        //            node.EnsureVisible();
        //            treeViewEmployees.Refresh();
        //            DragHelper.ImageList_DragShowNolock(true);
        //        }
        //    }
        //}

        ////--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------

        public WorkLists(Form root, Form parent)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            dtWorkListRights = CreateWorkListRightsTable();
            dtActionRights = CreateActionRightsTable();
            timer = new Timer();

            base.CheckUserRights(allowAddUserToSupWorkList);
        }

        private void WorkLists_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                PopulateEmployeeTree();
                PopulateActionTree();
                PopulateWorkListTree();
                PopulateActionGridView();
                PopulateWorkListGridView();
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        //IP - 15/05/09 - Credit Collections Walkthrough cosmetic changes
        //Replaced with menuDeleteNode_Click
        //private void btnDeleteNode_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        CMTreeNode selectedNode = (CMTreeNode)treeViewEmployees.SelectedNode;

        //        if (selectedNode == null)
        //            return;

        //        if (selectedNode.NodeType == NodeTypes.WorkList)
        //        {
        //            RemoveEmployeeWorkListRights(((CMTreeNode)selectedNode.Parent).NodeType, selectedNode.Parent.Name, GetEmployeeGroupNode((CMTreeNode)selectedNode.Parent).Name, selectedNode.Name);
        //            treeViewEmployees.Nodes.Remove(selectedNode);                    
        //        }
        //        else if (selectedNode.NodeType == NodeTypes.Action)
        //        {   
        //            foreach (CMTreeNode childStrategyNode in selectedNode.Nodes)
        //            {
        //                RemoveEmployeeActionRights(((CMTreeNode)selectedNode.Parent).NodeType, selectedNode.Parent.Name, GetEmployeeGroupNode((CMTreeNode)selectedNode.Parent).Name, childStrategyNode.Name, selectedNode.Name);  
        //            }
        //            treeViewEmployees.Nodes.Remove(selectedNode);                 
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, Function);
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtActionRights.Rows.Count == 0 && dtWorkListRights.Rows.Count == 0)
                {
                    //Show Message Box
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;
                DataSet dsSave = new DataSet();
                dsSave.Tables.Add(dtActionRights.Copy());
                dsSave.Tables.Add(dtWorkListRights.Copy());

                CollectionsManager.UpdateWorkList_ActionRights(dsSave, out error);

                if (error.Length > 0)
                {
                    ShowError(error);
                }
                else
                {
                    //TODO
                    //IP - 28/05/09 - UAT(617)
                    dtActionRights.Rows.Clear();
                    dtWorkListRights.Rows.Clear();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        // This method overides the Common Form Method to catch FormClose when X clicked
        public override bool ConfirmClose()
        {
            bool status = false;
            try
            {
                Function = "ConfirmClose()";
                Wait();
                //Returns the status that will determine whether worklist setup is to be saved
                DialogResult dr = ShowInfo("M_SAVE", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    btnSave_Click(null, null);
                }
                status = true;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }

            return status;
        }

        #region --TreeView Event Handlers-------------------------------------------------
        //--------------------------------------------------------------------------------

        private void treeViewWorkList_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            try
            {
                CMTreeNode tempNode = (CMTreeNode)e.Item; //The node being dragged
                if (tempNode != null)
                    treeViewWorkList.SelectedNode = tempNode;

                // Reset image list used for drag image
                imageListDrag.Images.Clear();
                imageListDrag.ImageSize = new Size(tempNode.Bounds.Size.Width + treeViewWorkList.Indent, tempNode.Bounds.Height);

                // This bitmap will contain the tree node image to be dragged
                Bitmap bmp = new Bitmap(tempNode.Bounds.Width, tempNode.Bounds.Height);
                Graphics gfx = Graphics.FromImage(bmp);// Get graphics from bitmap

                // Draw node icon into the bitmap
                //gfx.DrawImage(imageListTreeView.Images[0], 0, 0);

                //gfx.FillRectangle(new SolidBrush(Color.Azure), 0, 0, bmp.Width, bmp.Height);
                //gfx.DrawRectangle(new Pen(new SolidBrush(Color.Blue)), 0, 0, bmp.Width, bmp.Height);

                // Draw node label into bitmap
                gfx.DrawString(tempNode.Text, treeViewWorkList.Font, new SolidBrush(Color.Navy), 1.0f, 1.0f);
                // Add bitmap to imagelist
                imageListDrag.Images.Add(bmp);

                // Get mouse position in client coordinates
                Point p = treeViewWorkList.PointToClient(Control.MousePosition);

                // Compute delta between mouse position and node bounds
                int dx = p.X + tempNode.Bounds.Left;
                int dy = p.Y - tempNode.Bounds.Top;

                dragNode = (CMTreeNode)tempNode.Clone();

                // Begin dragging image
                if (DragHelper.ImageList_BeginDrag(imageListDrag.Handle, 0, dx, dy))
                {
                    // Begin dragging
                    treeViewWorkList.DoDragDrop(bmp, DragDropEffects.Move);
                    // End dragging image
                    DragHelper.ImageList_EndDrag();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void treeViewWorkList_DragOver(object sender, DragEventArgs e)
        {
            // Compute drag position and move image
            Point formP = PointToClient(new Point(e.X, e.Y));
            DragHelper.ImageList_DragMove(formP.X, formP.Y);

            e.Effect = DragDropEffects.Copy;
        }

        private void treeViewWorklist_DragLeave(object sender, System.EventArgs e)
        {
            DragHelper.ImageList_DragLeave(treeViewWorkList.Handle);

            // Disable timer for scrolling dragged item
            timer.Enabled = false;
        }

        private void treeViewAction_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            try
            {
                CMTreeNode tempNode = (CMTreeNode)e.Item; //The node being dragged
                if (tempNode != null)
                    treeViewAction.SelectedNode = tempNode;

                // Reset image list used for drag image
                imageListDrag.Images.Clear();
                imageListDrag.ImageSize = new Size(tempNode.Bounds.Size.Width + treeViewAction.Indent, tempNode.Bounds.Height);

                // This bitmap will contain the tree node image to be dragged
                Bitmap bmp = new Bitmap(tempNode.Bounds.Width, tempNode.Bounds.Height);
                Graphics gfx = Graphics.FromImage(bmp);// Get graphics from bitmap

                // Draw node icon into the bitmap
                //gfx.DrawImage(imageListTreeView.Images[0], 0, 0);

                //gfx.FillRectangle(new SolidBrush(Color.Azure), 0, 0, bmp.Width, bmp.Height);
                //gfx.DrawRectangle(new Pen(new SolidBrush(Color.Blue)), 0, 0, bmp.Width, bmp.Height);

                // Draw node label into bitmap
                gfx.DrawString(tempNode.Text, treeViewAction.Font, new SolidBrush(Color.Navy), 1.0f, 1.0f);
                // Add bitmap to imagelist
                imageListDrag.Images.Add(bmp);

                // Get mouse position in client coordinates
                Point p = treeViewAction.PointToClient(Control.MousePosition);

                // Compute delta between mouse position and node bounds
                int dx = p.X + tempNode.Bounds.Left;
                int dy = p.Y - tempNode.Bounds.Top;

                dragNode = (CMTreeNode)tempNode.Clone();

                if (tempNode.Parent != null)
                {
                    CMTreeNode parentNode = (CMTreeNode)(tempNode.Parent).Clone();
                    parentNode.Nodes.Add(dragNode);
                }

                // Begin dragging image
                if (DragHelper.ImageList_BeginDrag(imageListDrag.Handle, 0, dx, dy))
                {
                    // Begin dragging
                    treeViewAction.DoDragDrop(bmp, DragDropEffects.Move);
                    // End dragging image
                    DragHelper.ImageList_EndDrag();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void treeViewAction_DragOver(object sender, DragEventArgs e)
        {
            // Compute drag position and move image
            Point formP = PointToClient(new Point(e.X, e.Y));
            DragHelper.ImageList_DragMove(formP.X, formP.Y);

            e.Effect = DragDropEffects.Copy;
        }

        private void treeViewAction_DragLeave(object sender, System.EventArgs e)
        {
            DragHelper.ImageList_DragLeave(treeViewAction.Handle);

            // Disable timer for scrolling dragged item
            timer.Enabled = false;
        }

        private void treeViewEmployees_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            // Compute drag position and move image
            Point formP = PointToClient(new Point(e.X, e.Y));
            DragHelper.ImageList_DragMove(formP.X - treeViewEmployees.Left, formP.Y - treeViewEmployees.Top);

            //treeViewEmployees.Focus();
            // Get actual drop node
            CMTreeNode dropNode = (CMTreeNode)treeViewEmployees.GetNodeAt(treeViewEmployees.PointToClient(new Point(e.X, e.Y)));
            if (dropNode == null)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
            else
                treeViewEmployees.SelectedNode = (TreeNode)dropNode;

            if (dropNode.NodeType == NodeTypes.EmployeeGroup || dropNode.NodeType == NodeTypes.Branch)
                dropNode.Expand();

            if (dropNode.NodeType == NodeTypes.Employee || dropNode.NodeType == NodeTypes.AllEmployee)
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;

            // TODO only allow drop onto an employee and only if they do not have the same action already
            // TODO store the new actions in a table so that they can be updated later
            // if mouse is on a new node select it

            if (tempDropNode != dropNode)
            {
                DragHelper.ImageList_DragShowNolock(false);
                treeViewEmployees.SelectedNode = dropNode;
                DragHelper.ImageList_DragShowNolock(true);
                tempDropNode = dropNode;
            }

            // Avoid that drop node is child of drag node 
            CMTreeNode tmpNode = dropNode;
            while (tmpNode.Parent != null)
            {
                if (tmpNode.Parent == dragNode)
                    e.Effect = DragDropEffects.None;
                tmpNode = (CMTreeNode)tmpNode.Parent;
            }
        }

        private void treeViewEmployees_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                // Unlock updates
                DragHelper.ImageList_DragLeave(treeViewEmployees.Handle);

                // Get drop node
                CMTreeNode dropNode = (CMTreeNode)treeViewEmployees.GetNodeAt(treeViewEmployees.PointToClient(new Point(e.X, e.Y)));

                // If drop node isn't equal to drag node, add drag node as child of drop node

                if (dragNode.NodeType == NodeTypes.Strategy) //Strategy folder, children nodes may be either worklists or actions
                {
                    foreach (CMTreeNode subDragNode in dragNode.Nodes)
                    {
                        CMTreeNode subNodeAdded = dropNode.Nodes.AddCMNode(subDragNode.Name, subDragNode.Text, subDragNode.ImageIndex, subDragNode.NodeType);

                        if (subDragNode.NodeType == NodeTypes.Action)
                        {
                            subNodeAdded.Nodes.AddCMNode(dragNode.Name, dragNode.Text, dragNode.ImageIndex, dragNode.NodeType);
                            AddEmployeeActionRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, dragNode.Name, subDragNode.Name, 0, false);
                        }
                        else if (subDragNode.NodeType == NodeTypes.WorkList)
                        {
                            AddEmployeeWorkListRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, subDragNode.Name);
                        }
                    }
                }
                else if (dragNode.NodeType == NodeTypes.AllStrategy)
                {
                    foreach (CMTreeNode subDragNode in dragNode.Nodes)
                    {
                        CMTreeNode subNodeAdded = dropNode.Nodes.AddCMNode(subDragNode.Name, subDragNode.Text, subDragNode.ImageIndex, subDragNode.NodeType);

                        if (subDragNode.NodeType == NodeTypes.Action)
                        {
                            subNodeAdded.Nodes.AddCMNode("ALL", "ALL", dragNode.ImageIndex, dragNode.NodeType);
                            AddEmployeeActionRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, "", subDragNode.Name, 0, false);
                        }
                        else if (subDragNode.NodeType == NodeTypes.WorkList)
                        {
                            AddEmployeeWorkListRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, subDragNode.Name);
                        }
                    }
                }
                else if (dragNode.NodeType == NodeTypes.WorkList)
                {
                    dropNode.Nodes.AddCMNode(dragNode.Name, dragNode.Text, dragNode.ImageIndex, dragNode.NodeType);
                    AddEmployeeWorkListRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, dragNode.Name);
                }
                else if (dragNode.NodeType == NodeTypes.Action)
                {
                    CMTreeNode nodeAdded = dropNode.Nodes.AddCMNode(dragNode.Name, dragNode.Text, dragNode.ImageIndex, dragNode.NodeType);

                    CMTreeNode parentDragNode = (CMTreeNode)dragNode.Parent;

                    if (parentDragNode != null && parentDragNode.NodeType == NodeTypes.AllStrategy)
                    {
                        nodeAdded.Nodes.AddCMNode("ALL", "ALL", 4, NodeTypes.AllStrategy);
                        AddEmployeeActionRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, "", dragNode.Name, 0, false);
                    }
                    else if (parentDragNode != null && parentDragNode.NodeType == NodeTypes.Strategy)
                    {
                        nodeAdded.Nodes.AddCMNode(parentDragNode.Name, parentDragNode.Text, parentDragNode.ImageIndex, parentDragNode.NodeType);
                        AddEmployeeActionRights(dropNode.NodeType, dropNode.Name, GetEmployeeGroupNode(dropNode).Name, parentDragNode.Name, dragNode.Name, 0, false);
                    }
                }

                dropNode.Expand();

                // Set drag node to null
                dragNode = null;

                // Disable scroll timer
                timer.Enabled = false;
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

        }

        private void treeViewEmployees_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            DragHelper.ImageList_DragEnter(treeViewEmployees.Handle, e.X - treeViewEmployees.Left,
                e.Y - treeViewEmployees.Top);
            e.Effect = DragDropEffects.Copy;
            // Enable timer for scrolling dragged item
            timer.Enabled = true;
        }

        //private void treeViewEmployees_DragLeave(object sender, System.EventArgs e)
        //{
        //    DragHelper.ImageList_DragLeave(treeViewWorkList.Handle);

        //    // Disable timer for scrolling dragged item
        //    timer.Enabled = false;
        //}

        private void treeViewEmployees_GiveFeedback(object sender, System.Windows.Forms.GiveFeedbackEventArgs e)
        {
            if (e.Effect == DragDropEffects.Move)
            {
                // Show pointer cursor while dragging
                e.UseDefaultCursors = false;
                treeViewEmployees.Cursor = Cursors.Default;
            }
            else
                e.UseDefaultCursors = true;
        }

        private void treeViewEmployees_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    CMTreeNode selectedNode = (CMTreeNode)treeViewEmployees.SelectedNode;

                    if (selectedNode == null)
                        return;

                    if (selectedNode.NodeType == NodeTypes.WorkList)
                    {
                        RemoveEmployeeWorkListRights(((CMTreeNode)selectedNode.Parent).NodeType, selectedNode.Parent.Name, GetEmployeeGroupNode((CMTreeNode)selectedNode.Parent).Name, selectedNode.Name);
                        treeViewEmployees.Nodes.Remove(selectedNode);
                    }
                    else if (selectedNode.NodeType == NodeTypes.Action)
                    {
                        foreach (CMTreeNode childStrategyNode in selectedNode.Nodes)
                        {
                            RemoveEmployeeActionRights(((CMTreeNode)selectedNode.Parent).NodeType, selectedNode.Parent.Name, GetEmployeeGroupNode((CMTreeNode)selectedNode.Parent).Name, childStrategyNode.Name, selectedNode.Name);
                        }
                        treeViewEmployees.Nodes.Remove(selectedNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //--------------------------------------------------------------------------------
        #endregion -----------------------------------------------------------------------

        private void btnExportActions_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsExcelInstalled())
                    ExportToExcel(dgvActionView);
                else
                    ShowWarning("Excel application not found");
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExportActions_Click");
            }
        }

        private void btnExportWorklist_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsExcelInstalled())
                    ExportToExcel(dgvWorkListView);
                else
                    ShowWarning("Excel application not found");
            }
            catch (Exception ex)
            {
                Catch(ex, "btnExportWorklist_Click");
            }
        }

        //IP - 15/05/09 - Credit Collections Walkthrough cosmetic changes
        //Right-click delete
        private void menuDeleteNode_Click(object sender, EventArgs e)
        {
            try
            {
                CMTreeNode selectedNode = (CMTreeNode)treeViewEmployees.SelectedNode;

                if (selectedNode == null)
                    return;

                if (selectedNode.NodeType == NodeTypes.WorkList)
                {
                    RemoveEmployeeWorkListRights(((CMTreeNode)selectedNode.Parent).NodeType, selectedNode.Parent.Name, GetEmployeeGroupNode((CMTreeNode)selectedNode.Parent).Name, selectedNode.Name);
                    //treeViewEmployees.Nodes.Remove(selectedNode);
                    selectedNode.ImageIndex = 7;        // set icon to deleted - parent
                    selectedNode.SelectedImageIndex = 7;
                }
                else if (selectedNode.NodeType == NodeTypes.Action)
                {
                    foreach (CMTreeNode childStrategyNode in selectedNode.Nodes)
                    {
                        RemoveEmployeeActionRights(((CMTreeNode)selectedNode.Parent).NodeType, selectedNode.Parent.Name, GetEmployeeGroupNode((CMTreeNode)selectedNode.Parent).Name, childStrategyNode.Name, selectedNode.Name);
                        childStrategyNode.ImageIndex = 7;       // set icon to deleted - child
                        childStrategyNode.SelectedImageIndex = 7;
                    }

                    selectedNode.ImageIndex = 7;        // set icon to deleted - parent
                    selectedNode.SelectedImageIndex = 7;
                    treeViewEmployees.Refresh();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        //IP - 15/05/09 - Credit Collections Walkthrough cosmetic changes
        //Right-click delete
        // This method has been replaced by treeViewEmployees_NodeMouseClick
        private void treeViewEmployees_MouseUp(object sender, MouseEventArgs e)
        {
            // Compute drag position and move image
            //Point formP = PointToClient(new Point(e.X, e.Y));
            //CMTreeNode selectedNode = (CMTreeNode)treeViewEmployees.SelectedNode;

            //CMTreeNode selectedNode = (STL.PL.Collections.CMTreeNode<STL.PL.Collections.WorkLists.NodeType>)e.Node; // (CMTreeNode)treeViewEmployees.SelectedNode;

            //CMTreeNode selectedNode = (CMTreeNode)treeViewEmployees.GetNodeAt(treeViewEmployees.PointToClient(new Point(e.X, e.Y)));

            //if (e.Button == MouseButtons.Right && selectedNode.ImageIndex != 7)
            //{
            //    TreeView ctl = (TreeView)sender;

            //    MenuCommand deleteNode = new MenuCommand(GetResource("P_DELETE"));
            //    deleteNode.Click += new System.EventHandler(menuDeleteNode_Click);

            //    deleteNode.Enabled = true;
            //    deleteNode.Visible = true;

            //    PopupMenu popup = new PopupMenu();
            //    popup.Animate = Animate.Yes;
            //    popup.AnimateStyle = Animation.SlideHorVerPositive;

            //    popup.MenuCommands.Add(deleteNode);

            //    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
            //}

        }

        // Refresh screen 
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //dsEmployeeWithAllocations = null;
            //dtActionRights = null;
            WorkLists_Load(null, null);
        }

        private void treeViewEmployees_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // treeViewEmployees_MouseUp(sender, e);            

            var selectedNode = (CMTreeNode)e.Node;

            if (selectedNode != null)
                treeViewEmployees.SelectedNode = selectedNode;

            if (e.Button == MouseButtons.Right && selectedNode.ImageIndex != 7)
            {
                var treeView = (TreeView)sender;

                MenuCommand deleteNode = new MenuCommand(GetResource("P_DELETE"));
                deleteNode.Click += new System.EventHandler(menuDeleteNode_Click);

                deleteNode.Enabled = true;
                deleteNode.Visible = true;

                PopupMenu popup = new PopupMenu();
                popup.Animate = Animate.Yes;
                popup.AnimateStyle = Animation.SlideHorVerPositive;

                popup.MenuCommands.Add(deleteNode);

                MenuCommand selected = popup.TrackPopup(treeView.PointToScreen(new Point(e.X, e.Y)));
            }
        }
    }

    internal class NodeType
    {
        private string value;
        internal NodeType(string value)
        {
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            var objNodeType = (obj as NodeType);

            if (objNodeType == null)
                return false;

            return String.Equals(this.ToString(), objNodeType.ToString());
        }

        public override string ToString()
        {
            return value;
        }

        public override int GetHashCode()
        {
            return (value ?? "").GetHashCode();  //Not sure is this right
        }
    }

    internal class CMTreeNode : TreeNode
    {
        private NodeType nodeType;

        public CMTreeNode(NodeType nodeType)
            : base()
        {
            initialize(nodeType);
        }

        public CMTreeNode(string text, NodeType nodeType)
            : base(text)
        {
            initialize(nodeType);
        }

        public CMTreeNode()
            : base() { }

        protected CMTreeNode(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context, NodeType nodeType)
            : base(serializationInfo, context)
        {
            initialize(nodeType);
        }

        public CMTreeNode(string text, CMTreeNode[] children, NodeType nodeType)
            : base(text, children)
        {
            initialize(nodeType);
        }

        public CMTreeNode(string text, int imageIndex, int selectedImageIndex, NodeType nodeType)
            : base(text, imageIndex, selectedImageIndex)
        {
            initialize(nodeType);
        }

        public CMTreeNode(string text, int imageIndex, int selectedImageIndex, CMTreeNode[] children, NodeType nodeType)
            : base(text, imageIndex, selectedImageIndex, children)
        {
            initialize(nodeType);
        }

        private void initialize(NodeType nodeType)
        {
            this.nodeType = nodeType;
        }

        public override object Clone()
        {
            var clonedNode = (CMTreeNode)base.Clone();
            clonedNode.NodeType = nodeType;
            return clonedNode;
        }

        public NodeType NodeType
        {
            get { return nodeType; }
            set { nodeType = value; }
        }
    }

    internal static class CMTreeNodeExtension
    {
        public static CMTreeNode FindCMNode(this TreeNodeCollection nodesCollection, string key, bool findSubnodes)
        {
            if (nodesCollection == null)
                return null;

            var nodes = nodesCollection.Find(key, findSubnodes);

            return nodes.Length > 0 ? (CMTreeNode)nodes[0] : null;
        }

        public static CMTreeNode AddCMNode(this TreeNodeCollection nodesCollection, string key, string text,
                                                int? imageIndex, NodeType nodeType, string toolTip = null)
        {
            if (nodesCollection == null)
                return null;

            CMTreeNode node;

            if (nodesCollection.ContainsKey(key))
            {
                node = (CMTreeNode)nodesCollection.Find(key, false)[0];
                node.ImageIndex = imageIndex.HasValue ? (int)imageIndex : node.ImageIndex;
                node.SelectedImageIndex = node.ImageIndex;
            }
            else
            {
                node = new CMTreeNode(text, nodeType);
                node.Name = key;  //Name will internally be used as the key
                node.ImageIndex = imageIndex.HasValue ? (int)imageIndex : node.ImageIndex;
                node.SelectedImageIndex = node.ImageIndex;
                nodesCollection.Add(node);
            }

            if (toolTip != null)
                node.ToolTipText = toolTip;

            return node;
        }
    }
}