using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using STL.Common.ServiceRequest;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Categories;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.ItemTypes;
using STL.Common.Constants.Tags;
using Crownwood.Magic.Menus;
namespace STL.PL.Collections
{
    class WorkListData : CommonForm
    {
       private string _error = "";

        public WorkListData()
        {
        }

        /// <summary>
        /// Loads data that will populate the dropdown fields and datagridview.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public DataSet LoadWorkLists(out string error)
        {
            error = "";
            DataSet dsWorkList = new DataSet();

            try
            {
                dsWorkList = CollectionsManager.GetWorkLists(out error);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }

            return dsWorkList;
        }

        /// <summary>
        /// Builds a datatable of Employee Types that have been assigned to a 
        /// particular work list
        /// </summary>
        /// <param name="dvEmployees"></param>
        /// <returns></returns>
        public DataTable BuildEmployeeList(DataView dvEmployees)
        {
            DataTable dtEmployess = new DataTable(TN.EmployeeTypes);
            dtEmployess.Columns.Add(CN.EmployeeType);

            foreach (DataRowView row in dvEmployees)
            {
                if(Convert.ToInt16(row[CN.Assigned]) == 1)
                {
                    DataRow newRow;
                    newRow = dtEmployess.NewRow();
                    newRow[CN.EmployeeType] = row[CN.EmployeeType];

                    dtEmployess.Rows.Add(newRow);
                }
            }

            return dtEmployess;
        }

        /// <summary>
        /// Builds a datatable of Actions that have been assigned to a 
        /// particular work list
        /// </summary>
        /// <param name="dvEmployees"></param>
        /// <returns></returns>
        public DataTable BuildActionsList(DataView dvActions)
        {
            DataTable dtActions = new DataTable(TN.Actions);
            dtActions.Columns.Add(CN.Code);
            dtActions.Columns.Add(CN.Exit);

            foreach (DataRowView row in dvActions)
            {
                if (Convert.ToInt16(row[CN.Assigned]) == 1)
                {
                    DataRow newRow;
                    newRow = dtActions.NewRow();
                    newRow[CN.Code] = row[CN.Code];
                    if (row[CN.Exit].ToString() == "")
                   {
                      newRow[CN.Exit] = false; 
                   }
                   else{
                      newRow[CN.Exit] = row[CN.Exit];
                   }

                    dtActions.Rows.Add(newRow);
                 }
                 if (Convert.ToInt16(row[CN.Exit]) == 1)
                 {
                    DataRow newRow;
                    newRow = dtActions.NewRow();
                    newRow[CN.Code] = row[CN.Code];
                    newRow[CN.Exit] = row[CN.Exit];

                    dtActions.Rows.Add(newRow);
                }
            }

            return dtActions;
        }

        /// <summary>
        /// Checks to see if any Employee Types have been assigned
        /// to a Work List
        /// </summary>
        /// <param name="dvEmployees"></param>
        /// <returns></returns>
        public bool EmployeeListValid(DataView dvEmployees)
        {
            bool valid = false;

            foreach (DataRowView row in dvEmployees)
            {
                if (Convert.ToInt16(row[CN.Assigned]) == 1)
                {
                    valid = true;
                    break;
                }
            }

            return valid;
        }

        /// <summary>
        /// Permanently deletes the worklist from the database
        /// </summary>
        /// <param name="worklist"></param>
        internal void DeleteWorkList(string worklist)
        {
           CollectionsManager.DeleteWorkList(worklist, out _error);
        }

        /// <summary>
        /// Permanently deletes the worklist from the database
        /// </summary>
        /// <param name="worklist"></param>
        internal void DeleteSMS(string sms)
        {
           CollectionsManager.DeleteSMS(sms, out _error);
        }
    }
}
