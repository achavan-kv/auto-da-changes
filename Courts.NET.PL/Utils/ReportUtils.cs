using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using AxSHDocVw;
using System.Runtime.InteropServices;
using STL.Common.Constants.ColumnNames;


namespace STL.PL.Utils
{
    internal class ReportUtils
    {
        public static void OpenExcelCSV(string filePath)
        {
            if (!IsExcelInstalled())
            {
                MessageBox.Show("Excel not found. Please install to use this feature. " + Environment.NewLine + "File located at: " + filePath, "Excel not installed", MessageBoxButtons.OK);
                return;
            }

            string comma = ",";
            //try
            //{
                /* we have to use Reflection to call the Open method because 
                        * the methods have different argument lists for the 
                        * different versions of Excel - JJ */
                object[] args = null;
                Excel.Application excel = new Excel.Application();

                if (excel.Version == "10.0")	/* Excel2002 */
                    args = new object[] { filePath, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true, false, false };
                else
                    args = new object[] { filePath, 0, true, 2, "", "", true, Excel.XlPlatform.xlWindows, comma, true, false, 0, true };

                /* Retrieve the Workbooks property */
                object wbs = excel.GetType().InvokeMember("Workbooks", BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, excel, new Object[] { });
                
                /* call the Open method */
                object wb = wbs.GetType().InvokeMember("Open", BindingFlags.Public | BindingFlags.InvokeMethod, null, wbs, args);

                excel.Visible = true;
            //}
            //catch (COMException)
            //{
                /*change back slashes to forward slashes so the path doesn't
                        * get split into multiple lines */
                //throw new COMException();

               //ShowInfo("M_EXCELNOTFOUND", new object[] { path.Replace("\\", "/") });
            //}
        }
        /// <summary>
        /// Checks to see if excel is installed
        /// </summary>
        /// <returns></returns>
        public static bool IsExcelInstalled()
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                   
                return true;
            }
            catch(COMException)
            {
                return false;
            }
        }

        /*
        public static void SaveExcel(DataGridView dg)
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                Excel.Workbook wb = excel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
                Excel.Worksheet sheet = (Excel.Worksheet)excel.ActiveSheet;     
                //wb.Close(true, "c:\\test.xls", System.Type.Missing);
                    
                wb.SaveAs("c:\\test.xls", Excel.XlFileFormat.xlWorkbookNormal, null, null, null, null, Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null);

                //excel.Save("c:\\test.xls");
                string[,] values = new string[dg.Rows.Count + 1, dg.Columns.GetColumnCount(DataGridViewElementStates.Visible)];
                
                DataGridViewColumn dc = dg.Columns.GetFirstColumn(DataGridViewElementStates.Visible);
                int i = 0;
                while(!dc.Equals(dg.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None)))
                {
                    values[0, i++] = dc.HeaderText;       
                    dc = dg.Columns.GetNextColumn(dc, DataGridViewElementStates.Visible, DataGridViewElementStates.None);
                }
                
                sheet.get_Range(sheet.Cells[1, 1], sheet.Cells[dg.Rows.Count, dg.Columns.GetColumnCount(DataGridViewElementStates.Visible) + 1]).Value2 = values;
               
            }
            catch (COMException ex)
            {
               
            }
        }
        */
         

        //Creates a CSV file and returns the file path
        public static string CreateCSVFile(DataGridView dg, string dialogTitle)
        {
            string path = "";
            
            StringBuilder sb = new StringBuilder();
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
                save.Title = dialogTitle;
                save.CreatePrompt = true;

                if (save.ShowDialog() == DialogResult.OK)
                {
                    //Create the column headings
                    foreach (DataGridViewColumn dc in dg.Columns)
                    {
                        if (dc.Visible)
                        {
                            sb.Append(dc.HeaderText);
                            if (dc.Index != dg.Columns.Count - 1)
                                sb.Append(",");
                        }
                    }

                    sb.AppendLine();
                    //Create Data
                    foreach (DataGridViewRow dr in dg.Rows)
                    {
                        foreach (DataGridViewCell dc in dr.Cells)
                        {
                            if (dc.Visible)
                            {
                                if (dc.Value == null)
                                    break;
                                if (dc.OwningColumn.Name == "Account No" || dc.OwningColumn.Name == "Acctno"
                                    || dc.OwningColumn.Name == CN.acctno || dc.OwningColumn.Name == CN.AccountNumber)
                                    sb.Append("'" + dc.FormattedValue.ToString().Replace(",", "") + "'"); //putting in quotes to prevent excel displaying account number as shite
                                else
                                {
                                    if (dc.OwningColumn.DefaultCellStyle.Format == "P2") 
                                        sb.Append(dc.FormattedValue.ToString().Replace(",", ""));
                                    else
                                        sb.Append(dc.Value.ToString().Replace(",", ""));
                                }
                                  
                                
                                sb.Append(",");
                            }
                        }
                        sb.AppendLine();
                    }

                    path = save.FileName;
                    FileInfo fi = new FileInfo(path);
                    FileStream fs = fi.OpenWrite();
                    byte[] blob = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                    fs.Write(blob, 0, blob.Length);
                    fs.Close();
                    sb = null;
                }
            }
            catch
            {
                //TODO handle error here
            }
            return path;
        }


        /// <summary>
        /// Create a CSV for excel export
        /// </summary>
        /// <param name="dg">A datagridview containing rows to be exported</param>
        /// <returns>The path to the created CSV file</returns>
        public static string CreateCSVFile(DataGridView dg)
        {
            return CreateCSVFile(dg, "Save Report to CSV");
            
        }

        public static void PrintBasicReport(DataGridView dg, WebBrowser b, string heading)
        {
            //WebBrowser b = new WebBrowser();

            b.DocumentStream = null;
            StringBuilder sb = new StringBuilder("");

            //Create the column headings
            sb.AppendLine("<html>");

            if (!heading.Equals(string.Empty))
            {
               sb.Append("<h3 align='center'>");
                sb.Append(heading);
                sb.Append("</h3>") ;
            }

            sb.Append("<table width=100%><tr>");
            string align;
            string width;

            decimal totalWidth = 0;
            foreach (DataGridViewColumn dc in dg.Columns)
                totalWidth += dc.Width;

            foreach (DataGridViewColumn dc in dg.Columns)
            {
                if (dc.Visible)
                {
                    width = "width=" + (dc.Width / totalWidth * 100).ToString() + "%";
                    sb.Append("<td " + width + ">");
                    sb.Append(dc.HeaderText);
                    sb.Append("</td>");
                }
            }

            sb.Append("</tr>");

            //Create Data
            foreach (DataGridViewRow dr in dg.Rows)
            {
                sb.Append("<tr>");
                foreach (DataGridViewCell dc in dr.Cells)
                {
                    if (dc.Visible)
                    {
                        if (dc.Value == null)
                            break;

                        if (dc.OwningColumn.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                            align = "align=right";
                        else
                            align = string.Empty;

                        sb.Append("<td " + align + ">");
                        sb.Append(dc.FormattedValue.ToString());
                        sb.Append("</td>");
                    }
                }
                sb.Append("</tr>");
            }
            sb.Append("</table></html>");

            //System.IO.TextReader tr = new System.IO.TextReader();
            //tr.re
            b.DocumentText = "";
            b.Document.Write(sb.ToString());

            b.ShowPrintDialog();
            sb = null;
       
        }

       /// <summary>
       /// Unique method required for printing Technician Payments Report
       /// </summary>
       /// <param name="dg"></param>
       /// <param name="b"></param>
       /// <param name="technicianName"></param>
       /// <param name="technicianID"></param>
       public static void PrintTechnicianPaymentsReport(DataGridView dg, WebBrowser b, string technicianName,int technicianID,decimal[] chargeToSummary)
       {

          b.DocumentStream = null;
          StringBuilder sb = new StringBuilder("");

          //Create the column headings
          sb.AppendLine("<html>");

             sb.Append("<h3 align='center'>");
             sb.Append("Technician Payments");
             sb.Append("</h3>");

          sb.Append("<table width=100%><tr>");
          string align;
          string width;

          //UAT 334 - technician name, ID & date printed also required for Technician Payments report
          
             sb.Append("<td align='left' colspan='2'>");
             sb.Append(technicianName + " (" + technicianID.ToString() + ")");
             sb.Append("</td>");
             sb.Append("</tr>");
             sb.Append("<tr>");
             sb.Append("<td align='left' colspan='2'>");
             sb.Append(DateTime.Now.ToLongDateString());
             sb.Append("<BR><BR>");
             sb.Append("</td>");
             sb.Append("</tr>");
             sb.Append("<tr>");

          decimal totalWidth = 0;
          foreach (DataGridViewColumn dc in dg.Columns)
             totalWidth += dc.Width;

          foreach (DataGridViewColumn dc in dg.Columns)
          {
             if (dc.Visible)
             {
                width = "width=" + (dc.Width / totalWidth * 100).ToString() + "%";
                sb.Append("<td " + width + ">");
                sb.Append(dc.HeaderText);
                sb.Append("</td>");
             }
          }

          sb.Append("</tr>");

          //Create Data
          foreach (DataGridViewRow dr in dg.Rows)
          {
             sb.Append("<tr>");
             foreach (DataGridViewCell dc in dr.Cells)
             {
                if (dc.Visible)
                {
                   if (dc.Value == null)
                      break;

                   if (dc.OwningColumn.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
                      align = "align=right";
                   else
                      align = string.Empty;

                   sb.Append("<td " + align + ">");
                   if (dc.ValueType.Name == "DateTime")
                   {
                      sb.Append(((DateTime)dc.Value).ToShortDateString());
                   }
                   else if (dc.ValueType.Name == "Decimal")
                   {
                      sb.Append(((Decimal)dc.Value).ToString("N2"));
                   }
                   else
                   {
                      sb.Append(dc.FormattedValue.ToString());
                   }
                   sb.Append("</td>");
                }
             }
             sb.Append("</tr>");
          }
          //UAT 485 Report to include charge to summary
          
          sb.Append("<TR>");
          sb.Append("<TD align='left' colspan='2'>");
          sb.Append("<BR><BR>");
          sb.Append("Charge To Summary");
          sb.Append("</TD>");
          sb.Append("</TR>");
          sb.Append("<TR>");
          sb.Append("<TD width='20%'>");
          sb.Append("Internal");
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append("EW");      //CR1030 jec
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append("Supplier");
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append("Deliverer");
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append("Customer");
          sb.Append("</TD>");
          sb.Append("</TR>");
          sb.Append("<TR>");
          sb.Append("<TD width='20%'>");
          sb.Append(chargeToSummary[1].ToString("N2"));
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append(chargeToSummary[2].ToString("N2"));
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append(chargeToSummary[3].ToString("N2"));
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append(chargeToSummary[4].ToString("N2"));
          sb.Append("</TD>");
          sb.Append("<TD width='20%'>");
          sb.Append(chargeToSummary[5].ToString("N2"));
          sb.Append("</TD>");
          sb.Append("</TR>");


          sb.Append("</table></html>");

          //System.IO.TextReader tr = new System.IO.TextReader();

          //UAT 368 The WebBrowser is maintaining the text from a previous technician
          b.Document.OpenNew(true);
          b.Document.Write(sb.ToString());

          b.ShowPrintDialog();
          sb = null;

       }

        public static void PrintBasicReport(DataGridViewSelectedRowCollection dgs, WebBrowser b, string heading)
        {
            DataGridView dg = new DataGridView();
            
            
            foreach (DataGridViewRow r in dgs)
            {
                dg.Rows.Add(r);
            }

            //WebBrowser b = new WebBrowser();

            //b.DocumentStream = null;
            //StringBuilder sb = new StringBuilder("");

            ////Create the column headings
            //sb.AppendLine("<html>");

            //if (!heading.Equals(string.Empty))
            //{
            //    sb.Append("<h3>");
            //    sb.Append(heading);
            //    sb.Append("</h3>");
            //}

            //sb.Append("<table width=100%><tr>");
            //string align;
            //string width;

            //decimal totalWidth = 0;
            //foreach (DataGridView dc in dg)
            //    totalWidth += dc.Width;

            //foreach (DataGridViewColumn dc in dgs)
            //{
            //    if (dc.Visible)
            //    {
            //        width = "width=" + (dc.Width / totalWidth * 100).ToString() + "%";
            //        sb.Append("<td " + width + ">");
            //        sb.Append(dc.HeaderText);
            //        sb.Append("</td>");
            //    }
            //}

            //sb.Append("</tr>");
            ////Create Data
            //foreach (DataGridViewRow dr in dgs)
            //{
            //    sb.Append("<tr>");
            //    foreach (DataGridViewCell dc in dr.Cells)
            //    {
            //        if (dc.Visible)
            //        {
            //            if (dc.Value == null)
            //                break;

            //            if (dc.OwningColumn.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
            //                align = "align=right";
            //            else
            //                align = string.Empty;

            //            sb.Append("<td " + align + ">");
            //            sb.Append(dc.FormattedValue.ToString());
            //            sb.Append("</td>");
            //        }
            //    }
            //    sb.Append("</tr>");
            //}
            //sb.Append("</table></html>");

            ////System.IO.TextReader tr = new System.IO.TextReader();
            ////tr.re
            //b.Document.Write(sb.ToString());

            //b.ShowPrintDialog();
            //sb = null;

        }

        public static void PrintBasicReport(DataGridView dg, WebBrowser b)
        {
            PrintBasicReport(dg, b, string.Empty);
        }

        public static void PrintBasicReport(DataGridViewSelectedRowCollection dgs, WebBrowser b)
        {
            PrintBasicReport(dgs, b, string.Empty);
        }

        public static void ApplyGridHeadings(DataGridView dg, CommonForm f)
        {
            foreach (DataGridViewColumn dc in dg.Columns)
                dc.HeaderText = GetColumnTitle(dc.Name, f); 
        }
        public static void ApplyGridHeadings(GridColumnStylesCollection cs, CommonForm f)
        {
            
            for (int i = 0; i < cs.Count; i++)
                cs[i].HeaderText = GetColumnTitle(cs[i].MappingName, f);
        }

        private static string GetColumnTitle(string colName, CommonForm f)
        {
            string ret;
            StringComparison sc = StringComparison.InvariantCultureIgnoreCase;

            if (colName.Equals(CN.ServiceRequestNoStr, sc)) ret = CommonForm.GetResource("T_SERVICEREQUESTNO");
            else if (colName.Equals(CN.DateLogged, sc)) ret = CommonForm.GetResource("T_DATELOGGED");
            else if (colName.Equals(CN.DepositAmount, sc)) ret = CommonForm.GetResource("T_DEPOSITAMOUNT");
            else if (colName.Equals(CN.DepositPaid, sc)) ret = CommonForm.GetResource("T_DEPOSITPAID");
            else if (colName.Equals(CN.ProductCode, sc)) ret = CommonForm.GetResource("T_PRODCODE");
            else if (colName.Equals(CN.Description, sc)) ret = CommonForm.GetResource("T_DESCRIPTION");
            else if (colName.Equals(CN.DateAllocated, sc)) ret = CommonForm.GetResource("T_DATEALLOC");
            else if (colName.Equals(CN.RepairDate, sc)) ret = CommonForm.GetResource("T_REPAIRDATE");
            else if (colName.Equals(CN.OutstBal, sc)) ret = CommonForm.GetResource("T_OUTBAL");
            else if (colName.Equals(CN.TechnicianId, sc)) ret = CommonForm.GetResource("T_TECHNICIAN");
            else if (colName.Equals(CN.PartsDate, sc)) ret = CommonForm.GetResource("T_PARTSDATE");
            else if (colName.Equals(CN.TotalCost, sc)) ret = CommonForm.GetResource("T_TOTALCOST");
            else if (colName.Equals(CN.DateClosed, sc)) ret = CommonForm.GetResource("T_DATECLOSED");
            else if (colName.Equals(CN.Status, sc)) ret = CommonForm.GetResource("T_STATUS");
            else if (colName.Equals(CN.Address1, sc)) ret = CommonForm.GetResource("T_ADDRESS1");
            else if (colName.Equals(CN.Address2, sc)) ret = CommonForm.GetResource("T_ADDRESS2");
            else if (colName.Equals(CN.Address3, sc)) ret = CommonForm.GetResource("T_ADDRESS3");
            else if (colName.Equals(CN.LoggedBy, sc)) ret = CommonForm.GetResource("T_LOGGEDBY");
            else if (colName.Equals(CN.AcctNo, sc)) ret = CommonForm.GetResource("T_ACCTNO");
            else if (colName.Equals(CN.ServiceType, sc)) ret = CommonForm.GetResource("T_SERVICETYPE");
            else if (colName.Equals(CN.Name, sc)) ret = CommonForm.GetResource("T_NAME");
            else if (colName.Equals(CN.AcctNo, sc)) ret = CommonForm.GetResource("T_ACCTNO");
            else if (colName.Equals(CN.TelHome, sc)) ret = CommonForm.GetResource("T_PHONE");
            else if (colName.Equals(CN.TelWork, sc)) ret = CommonForm.GetResource("T_WORKPHONE");
            else if (colName.Equals(CN.TelMobile, sc)) ret = CommonForm.GetResource("T_MOBILEPHONE");
            else if (colName.Equals(CN.Arrears, sc)) ret = CommonForm.GetResource("T_ARREARS");
            else if (colName.Equals(CN.PurchaseDate, sc)) ret = CommonForm.GetResource("T_PURCHASEDATE");
            else if (colName.Equals(CN.ModelNo, sc)) ret = CommonForm.GetResource("T_MODELNO");
            else if (colName.Equals(CN.SerialNo, sc)) ret = CommonForm.GetResource("T_SERIALNO");
            else if (colName.Equals(CN.ExtWarranty, sc)) ret = CommonForm.GetResource("T_EXTWARRANTY");
            else if (colName.Equals(CN.ContractNo, sc)) ret = CommonForm.GetResource("T_CONTRACTNO");
            else if (colName.Equals(CN.SoftScript, sc)) ret = CommonForm.GetResource("T_SOFTSCRIPT");
            else if (colName.Equals(CN.FirstName, sc)) ret = CommonForm.GetResource("T_FIRSTNAME");
            else if (colName.Equals(CN.LastName, sc)) ret = CommonForm.GetResource("T_LASTNAME");
            else if (colName.Equals(CN.PhoneNo, sc)) ret = CommonForm.GetResource("T_PHONENO");
            else if (colName.Equals(CN.MobileNo, sc)) ret = CommonForm.GetResource("T_MOBILENO");
            else if (colName.Equals(CN.CustID, sc)) ret = CommonForm.GetResource("T_CUSTID");
            else if (colName.Equals(CN.CustID, sc)) ret = CommonForm.GetResource("T_CUSTID");
            else if (colName.Equals(CN.Item1, sc)) ret = CommonForm.GetResource("T_PRODUCT1");
            else if (colName.Equals(CN.Item2, sc)) ret = CommonForm.GetResource("T_PRODUCT2");
            else if (colName.Equals(CN.Item3, sc)) ret = CommonForm.GetResource("T_PRODUCT3");
            else if (colName.Equals(CN.Item4, sc)) ret = CommonForm.GetResource("T_PRODUCT4");
            else if (colName.Equals(CN.Item5, sc)) ret = CommonForm.GetResource("T_PRODUCT5");
            else if (colName.Equals(CN.EmployeeNo, sc)) ret = CommonForm.GetResource("T_EMPEENO");
            else if (colName.Equals(CN.ItemNo, sc)) ret = CommonForm.GetResource("T_ITEMNO");
            else if (colName.Equals(CN.InvoiceNo, sc)) ret = CommonForm.GetResource("T_INVOICENO");
            else if (colName.Equals(CN.CommissionType, sc)) ret = CommonForm.GetResource("T_COMMISSIONTYPE");
            else if (colName.Equals(CN.CommissionAmount, sc)) ret = CommonForm.GetResource("T_COMMISSIONAMOUNT");
            else if (colName.Equals(CN.DeliveryAmount, sc)) ret = CommonForm.GetResource("T_DELIVERYAMOUNT");
            else if (colName.Equals(CN.RebateTotal, sc)) ret = CommonForm.GetResource("T_REBATETOTAL");
            else if (colName.Equals(CN.RepossessionTotal, sc)) ret = CommonForm.GetResource("T_REPOSSESSIONTOTAL");
            else if (colName.Equals(CN.ProductTotal, sc)) ret = CommonForm.GetResource("T_PRODUCTTOTAL");
            else if (colName.Equals(CN.CommissionPercent, sc)) ret = CommonForm.GetResource("T_COMMISSIONPERCENT");
            else if (colName.Equals(CN.EmployeeName, sc)) ret = CommonForm.GetResource("T_EMPLOYEENAME");
            else if (colName.Equals(CN.CommissionTotal, sc)) ret = CommonForm.GetResource("T_COMMISSIONTOTAL");
            else if (colName.Equals(CN.SPIFFTotal, sc)) ret = CommonForm.GetResource("T_SPIFFTOTAL");
            else if (colName.Equals(CN.TransTypeCode, sc)) ret = CommonForm.GetResource("T_TRANSTYPECODE");
            else if (colName.Equals(CN.CancellationTotal)) ret = CommonForm.GetResource("T_CANCELLATIONTOTAL");
            else if (colName.Equals(CN.ReplacementStatus)) ret = CommonForm.GetResource("T_REPLACEMENTSTATUS");
            else if (colName.Equals(CN.TotalDue)) ret = CommonForm.GetResource("T_TOTALDUE");
            else if (colName.Equals(CN.TechnicianName)) ret = CommonForm.GetResource("T_TECHNICIANNAME");
            else if (colName.Equals(CN.WarrantyTotal)) ret = CommonForm.GetResource("T_WARRANTYTOTAL");      //CR1035
            else if (colName.Equals(CN.ProductCommissionTotal)) ret = CommonForm.GetResource("T_PRODUCTCOMMISSIONTOTAL");      //CR1035

            else ret = colName;
            
            return ret;
           
        }
    }
}


