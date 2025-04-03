using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Giro;

namespace STL.BLL
{
    /// <summary>
    /// Most of the methods of this class perform batch set operations, and not single
    /// row operations. Therefore, there are no properties of this class used for the
    /// details of one payment. However, if single row operations such as Retrieve and
    /// Save are required, then these methods (and properties) can be added to this class.
    ///
    /// METHODS:
    ///
    /// CreateExtraFile - Copy due Extra payments from the DDPAYMENT table to file
    /// CreateNormalFile - Copy due Normal payments from the DDPAYMENT table to file
    /// CreateFile - Copy due payments from the DDPAYMENT table to file
    /// FileHeader - Get the bank pay code for the next due date and payment type
    /// OutStanding - Sum outstanding DD payments submitted to the bank
    /// PaymentsEnabled - Return true if payments can be processed
    /// SubmitPayments - Add due payments to the DDPAYMENT table
    /// TidyPayments - Delete old data from the DDPayment, DDpaymentfile, DDrejectfile table
    /// </summary>
    /// 


    public class BDDPayment : CommonObject
    {

        //
        // BL Methods
        //
        public BDDPayment()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public void CreateFile(SqlConnection conn, SqlTransaction trans,
            string interfaceName, 
            int runNo,
            BDDMandate giroDates,
            string paymentType,
            string fileName,
            string fileSuffix,
            string msgName,
            int lineLength)
        {
            StreamWriter sw = null;
            string newFileName = "";
            bool fileExists = false;

            try
            {
                /* Construct the filename using the due date and open the file */
                newFileName = DDFileName.ServerPath + fileName + giroDates.fileNameDate + fileSuffix;

                if (File.Exists(newFileName))
                {
                    // The file already exists
                    fileExists = true;
                    throw new STLException(GetResource("M_DDFILEEXISTS", new object[] { msgName, newFileName }));
                }

                // Retrieve all the pending payments
                DDDPayment payments = new DDDPayment();
                DataTable pendingPaymentList = payments.PendingPaymentList(conn, trans, paymentType, giroDates.nextDueDayId);

                sw = new StreamWriter(newFileName);
                int recordCount = 0;
                decimal totalAmount = 0;
                decimal amount = 0;
                string amountStr = "";
                string curLine = "";
                foreach (DataRow paymentRow in pendingPaymentList.Rows)
                {
                    // Process each line to add to the file
                    recordCount++;
                    amount = (int)paymentRow[CN.Amount];
                    if (amount >= 0)
                    {
                        totalAmount += amount;
                        // Prefix an amount with leading zeroes
                        amountStr = DDFormat.Numeric + amount.ToString();
                        amountStr = amountStr.Substring(amountStr.Length - DDFieldLen.Amount);
                    }
                    else
                    {
                        // A negative amount indicates a 'DELETE' record
                        amountStr = DDFormat.Delete;
                    }

                    curLine = (string)paymentRow[CN.RecordLine] + amountStr;
                    sw.WriteLine(curLine.PadRight(lineLength));

                }
                sw.Flush();
                sw.Close();

                if (recordCount == 0)
                {
                    // The file was not created because there was no data
                    File.Delete(newFileName);
                    string msg = GetResource("M_DDFILENOTCREATED", new object[] { msgName, newFileName });
                    BInterfaceError ie = new BInterfaceError(
                        null,
                        null,
                        interfaceName,
                        runNo,
                        DateTime.Now,
                        msg,
                        "W");

                    Console.WriteLine(msg);
                }
                else
                {
                    this.FileHeader(conn, trans,
                        giroDates,
                        paymentType,
                        newFileName,
                        lineLength,
                        recordCount,
                        totalAmount);

                    // Update the pending payments as having been submitted to the bank.
                    payments.SubmittedToBank(conn, trans, paymentType, giroDates.nextDueDayId, giroDates.effectiveDate);
                }
            }
            catch (Exception ex)
            {
                if (sw != null) sw.Close();
                /* Delete an incomplete payment file */
                if (!fileExists) File.Delete(newFileName);
                throw ex;
            }
        }


        /**************************************************************/
        /* Create a file header record                                */
        /**************************************************************/
        /* The file header format is:
        **
        **  Normal Payments
        **  ---------------
        **  G150038099001D07      260900161000K********059630060986780
        **
        **  Fees
        **  ----
        **  G150038099001D11      260900161000K********007070000707000
        **
        **  Re-presentations
        **  ----------------
        **  G150038099001D09      260900171000K********007120007549946
        **
        **  Fields for headers
        **  ------------------
        **  abbbcccccccccdddeeeeeeeeeeeeffffffghhhhhhhhiiiiijjjjjjjjjj
        **
        **  key;
        **  field   Description
        **    
        **  a       'G'
        **  b       '150'
        **  c       Courts Bank Account no. - from parameter file
        **  d       'D06' 'D08' 'D10' if due date less than 16th of month
        **          'D07' 'D09' 'D11' if due date >= than 16th of month
        **  e       actual date created ddmmyy  - right justified
        **  f       BACS processing date - from parameter file
        **  g       'K'
        **  h       '********'
        **  i       No. of detail records
        **  j       value of detail records
        **
        ***************************************************************/
        public void FileHeader(SqlConnection conn, SqlTransaction trans,
            BDDMandate giroDates,
            string paymentType,
            string fileName,
            int lineLength,
            int recordCount,
            decimal totalAmount)
        {
            /* Get the appropriate bank pay code for the due day */
            DDDPayment paymentCode = new DDDPayment();
            string bankPayCode = paymentCode.BankPayCode(conn, trans, paymentType, giroDates.nextDueDayId);

            // The creation date format must be DDMMYY with leading spaces to form a 12 char field
            string creationDateStr =
                giroDates.today.Day.ToString().PadLeft(2, '0') +
                giroDates.today.Month.ToString().PadLeft(2, '0') +
                giroDates.today.Year.ToString().Substring(2,2);

            creationDateStr = creationDateStr.PadLeft(12, ' ');

            // The BACS date format must be DDMMYY
            string bACSDateStr =
                giroDates.nextDueDate.Day.ToString().PadLeft(2, '0') +
                giroDates.nextDueDate.Month.ToString().PadLeft(2, '0') +
                giroDates.nextDueDate.Year.ToString().Substring(2, 2);

            /* The numeric fields require leading zeroes */
            string recordCountStr = recordCount.ToString().PadLeft(DDHeaderFieldLen.RecordCount, '0');
            string totalAmountStr = totalAmount.ToString().PadLeft(DDHeaderFieldLen.TotalAmount, '0');

            /* Create the header line and write it to a temporary file */
            string headerLine = DDHeaderField.FieldA + DDHeaderField.FieldB +
                (string)Country[CountryParameterNames.DDBankAccountNo] + bankPayCode.Substring(0,3) +
                creationDateStr + bACSDateStr + DDHeaderField.FieldG + DDHeaderField.FieldH +
                recordCountStr + totalAmountStr;

            string workFileName = fileName + DDFileSuffix.Tmp;
            if (File.Exists(workFileName)) File.Delete(workFileName);
            StreamWriter swrite = new StreamWriter(workFileName);
            swrite.Write(headerLine.PadRight(lineLength));

            /* Append the detail records to the header */
            StreamReader sread = File.OpenText(fileName);
            string detailLine;
            while ((detailLine = sread.ReadLine()) != null)
            {
                swrite.Write(detailLine.PadRight(lineLength));
            }
            swrite.Flush();
            swrite.Close();
            sread.Close();

            // Delete the original details file
            File.Delete(fileName);

            // Copy the work file back to the original file name
            File.Move(workFileName, fileName);

        }



        public bool PaymentsEnabled(BDDMandate giroDates, out string enableMsg)
        {
            // Controlled time period for EOD Payment processing.
            // This is a short window before the due date minus the lead time.
            enableMsg = "";
            bool enabled = false;
            DateTime paymentEnableTo = giroDates.nextDueDate.AddDays(-giroDates.countryDDLeadTime);
            DateTime paymentEnableFrom = paymentEnableTo.AddDays(-DDEnable.PaymentWindow);

            if (giroDates.today >= paymentEnableFrom && giroDates.today <= paymentEnableTo)
            {
                // Payments processing is enabled between these dates
                enabled = true;
            }
            else
            {
                // Payments processing is disabled
                enableMsg = GetResource("M_DDPAYMENTSDISABLED", new object[] { paymentEnableFrom.ToLongDateString(), paymentEnableTo.ToLongDateString(), giroDates.nextDueDate.ToLongDateString() });
            }

            return enabled;
        }


        public void SubmitPayments(SqlConnection conn, SqlTransaction trans, BDDMandate giroDates)
        {
            // Copy the new payment rows from the temp table that was
            // created by 'NewPaymentList' to the DDPayment table
            DDDPayment eodPayment = new DDDPayment();
            eodPayment.SubmitPayments(conn, trans,
                giroDates.nextDueDayId,
                giroDates.nextDueDate,
                giroDates.effectiveDate,
                giroDates.countryMinPeriod);
        }


        public void TidyPayments(SqlConnection conn, SqlTransaction trans)
        {
            DDDPayment eodPayment = new DDDPayment();
            eodPayment.TidyPayments(conn, trans);
        }

    }
}


