using System;
using STL.PL.WS2;
using System.Data;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using Crownwood.Magic.Menus;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace STL.PL
{
    public partial class FactoringReports : CommonForm
    {
        public FactoringReports(Form root,Form Parent)
        {
            InitializeComponent();
        }

        private void btnTransTypes_Click(object sender, System.EventArgs e)
        {
            SetSelection selection = new SetSelection("Transaction Types",45,180,64,this.txtTransactionTypes,TN.TNameTransType,TN.NonDeposits,false);
            selection.FormRoot = this.FormRoot;
            selection.FormParent = this;
            selection.ShowDialog(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();
                WAccountManager Ac = new WAccountManager(true);
                DataSet DS = new DataSet();
                string Errortext;
                if (comboReport.SelectedIndex == 0) //balances
                {
                    DS = Ac.FincoBalances(Convert.ToDateTime(dateTimeDatefrom.Text), Convert.ToDateTime(dateTimeDateto.Text), out Errortext);
                }
                else //transactions
                {
                    DS = Ac.FincoTransactions(Convert.ToDateTime(dateTimeDatefrom.Text), Convert.ToDateTime(dateTimeDateto.Text), txtTransactionTypes.Text, out Errortext);

                }
//                if (Errortext != "")
                    //dataGridViewResults.DataSource = DS;
                    dataGridViewResults.DataSource = DS.Tables[TN.Accounts].DefaultView;
                decimal total = 0;
                decimal fincoedtotal = 0;
                decimal nonfincoedtotal = 0;
                foreach (DataTable dt in DS.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (comboReport.SelectedIndex == 0) //balances
                        {
                            if (DBNull.Value != row[CN.Balance])
                                total += ((decimal)row[CN.Balance]);
                        }
                        else
                        {
                            if (DBNull.Value != row[CN.Value])
                                total += ((decimal)row[CN.Value]);
                        }

                        if (comboReport.SelectedIndex == 1)
                            if (DBNull.Value != row["FinCoed"])
                            {
                                if (row["FinCoed"].ToString() =="Y")
                                   fincoedtotal += ((decimal)row[CN.Value]);
                                else
                                    nonfincoedtotal += ((decimal)row[CN.Value]);
                            }

                    }

                    txtDetailsTotal.Text = total.ToString(DecimalPlaces);
                    txtDetailsTotal.Visible = true;

                    if (comboReport.SelectedIndex == 1) //financials
                    {
                        textBoxFinco.Enabled = true;
                        textBoxNonFinco.Enabled = true;
                        textBoxFinco.Text = fincoedtotal.ToString(DecimalPlaces);
                        textBoxNonFinco.Text = nonfincoedtotal.ToString(DecimalPlaces);
                        //textBoxNonFinco..FORM Format = "N2";
                    }
                    else
                    {
                        textBoxFinco.Enabled = false;
                        textBoxNonFinco.Enabled = false;
                        textBoxFinco.Text = "";
                        textBoxNonFinco.Text = "";

                    }
                }

            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                btnExcel.Enabled = true;
                StopWait();
            }
        }

        private void drpTransTypeCriteria_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string selection = drpTransTypeCriteria.SelectedItem.ToString();
            switch (selection)
            {
                case "":
                    {
                        drpTransTypes.Visible = false;
                        txtTransactionTypes.Visible = false;
                        btnTransTypes.Visible = false;
                        break;
                    }
                case "=":
                    {
                        drpTransTypes.Visible = true;
                        txtTransactionTypes.Visible = false;
                        btnTransTypes.Visible = false;
                        break;
                    }
                case "In Set":
                    {
                        drpTransTypes.Visible = false;
                        txtTransactionTypes.Visible = true;
                        btnTransTypes.Visible = true;
                        break;
                    }
                default:
                    {
                        drpTransTypes.Visible = false;
                        txtTransactionTypes.Visible = false;
                        btnTransTypes.Visible = false;
                        break;
                    }
            }
        }

        private void comboReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboReport.SelectedIndex == 1) //financials
            {
                drpTransTypeCriteria.Enabled = true;
                labelType.Enabled = true;
                txtTransactionTypes.Enabled = true;
                btnTransTypes.Enabled = true;
            }
            else
            {
                drpTransTypeCriteria.Enabled = false;
                labelType.Enabled = false;
                txtTransactionTypes.Enabled = false;
                btnTransTypes.Enabled = false;
            }


        }

        private void drpTransTypeCriteria_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            
            try
            {
                Wait();

                /* save the current data grid contents to a CSV */
                string comma = ",";
                string path = "";

                if(dataGridViewResults.RowCount >= 0)
                {
                    DataView dv = (DataView)dataGridViewResults.DataSource;

                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*" ;
                    save.Title = "Save Fin Trans query data";
                    save.CreatePrompt = true;

                    if(save.ShowDialog() == DialogResult.OK)
                    {
                        path = save.FileName;
                        FileInfo fi = new FileInfo(path);
                        FileStream fs = fi.OpenWrite();
			
                        //Write heading line..
                        string line = string.Empty;

                       if (comboReport.SelectedIndex == 1) //financials
                       {
                            line = line + GetResource("T_ACCTNO") + comma +
                                GetResource("T_TRANSACTIONDATE") + comma +
                                GetResource("T_TYPE") + comma +
                                GetResource("T_VALUE") + comma + "FinCoed" + Environment.NewLine + Environment.NewLine;
                       }
                       else
                       {
                               line = line + GetResource("T_ACCTNO") + comma +
                                GetResource("T_BRANCH") + comma +
                                GetResource("T_BALANCE") + Environment.NewLine + Environment.NewLine;
                                              
                       }
                       byte[] blob = System.Text.Encoding.UTF8.GetBytes(line);
                        fs.Write(blob,0,blob.Length);
			
                        foreach(DataRowView row in dv)
                        {					
                            line = string.Empty;

                      if (comboReport.SelectedIndex == 0) //BALANCE
                            {

                                line += "'" + row["AccountNo"].ToString().Replace(",", "") + "'" + comma +
                                    Convert.ToString(row["BranchNo"]).Replace(",","") + comma +
                                    row["Balance"].ToString().Replace(",","") +
                                    Environment.NewLine;
                            }
                            else //FINANCIALS -- lets confusing reverse the order above
                            {
                                   line +=	"'" + row["AccountNo"].ToString().Replace(",","") + "'" + comma +
                                    Convert.ToString(row["TransactionDate"]).Replace(",","") + comma +
                                    row[CN.Code].ToString().Replace(",","") + comma +
                                    ((decimal)row[CN.Value]).ToString(DecimalPlaces).Replace(",","") + comma +
                                    row["FinCoed"].ToString().Replace(",","") +
                                    Environment.NewLine;
                        
                            }

                            blob = System.Text.Encoding.UTF8.GetBytes(line);
                            fs.Write(blob,0,blob.Length);
                        }
                        fs.Close();						

                        /* attempt to launch Excel. May get a COMException if Excel is not 
                            * installed */
                        try
                        {
                            /* we have to use Reflection to call the Open method because 
                                * the methods have different argument lists for the 
                                * different versions of Excel - JJ */
                            object[] args = null;
                            Excel.Application excel = new Excel.Application();

                            if(excel.Version == "10.0")	/* Excel2002 */
                                args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                            else
                                args = new object[]{path, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                            /* Retrieve the Workbooks property */
                            object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public|BindingFlags.GetField|BindingFlags.GetProperty, null, excel, new Object[] {});

                            /* call the Open method */
                            object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                            excel.Visible = true;
                        }
                        catch(COMException)
                        {
                            /*change back slashes to forward slashes so the path doesn't
                                * get split into multiple lines */
                            ShowInfo("M_EXCELNOTFOUND", new object[] {path.Replace("\\", "/")});
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
        }

        

        
    }
}