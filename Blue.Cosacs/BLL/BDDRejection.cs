using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.Giro;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;


namespace STL.BLL
{
    /// <summary>
    /// Singapore Direct Debit Rejections
    /// </summary>
    /// 


    public class BDDRejection : CommonObject
    {
        //
        // BL private properties
        //
        /* Rejections filename */
        private string _rejectFileName
        {
            get { return DDFileName.ServerPath + DDFileName.Reject + DDFileSuffix.Reject; }
        }

        /* Rejections format filename for BULK INSERT */
        private string _rejectFormatName
        {
            get { return DDFileName.ServerPath + DDFileName.RejectFormat + DDFileSuffix.Format; }
        }

        /* Processed rejections filename */
        private string _processedFileName
        {
            get { return DDFileName.ServerPath + DDFileName.Processed_Reject + DDFileSuffix.Reject; }
        }

        //
        // BL Methods
        //
        public BDDRejection()
        {
        }


        //
        // Load a list of Rejected Payments
        //
        public DataSet GetDDRejectionList(string countryCode)
        {
            BDDMandate mandate = new BDDMandate(countryCode, DateTime.Today);
            DDDRejection dRejection = new DDDRejection();
            return dRejection.GetRejectList(mandate.effectiveDate);
        }


        public void LoadRejections(SqlConnection conn, SqlTransaction trans, BDDMandate giroDates)
        {
            /* Load the rejections file and match each rejection to a submitted payment */

            if (!File.Exists(this._rejectFileName))
            {
                // A new rejections file has not been found so return
                throw new STLException(GetResource("M_DDREJECTIONSNOTEXISTS", new object[] { this._rejectFileName }));
            }

            if (File.Exists(this._processedFileName))
            {
                // A processed rejections file has been found
                // This should not happen when a new rejections file has also been found
                throw new STLException(GetResource("M_DDREJECTIONSPROCESSED", new object[] { this._rejectFileName, this._processedFileName }));
            }

            /* Read the rejection file into the temp table */
            DDDRejection dRejection = new DDDRejection();
            dRejection.LoadRejectionFile(conn, trans, this._rejectFileName, this._rejectFormatName, giroDates.today);
        }


        public void LogUnmatchedRejections(SqlConnection conn, SqlTransaction trans,
            string interfaceName, int runNo)
        {
            /* Log rejection records that do not match any payments */
            DDDRejection dRejection = new DDDRejection();
            dRejection.LogUnmatchedRejections(conn, trans, interfaceName, runNo);
        }


        public bool RejectionsEnabled(BDDMandate giroDates, out string enableMsg)
        {
            // Controlled time period for EOD Rejection processing.
            // Rejections cannot be processed too close to a due date (either before or after).
            enableMsg = "";
            bool enabled = false;

            // Get the nearest due date to today past or present
            DDDRejection dRejection = new DDDRejection();
            DateTime nearestDueDate = dRejection.NearestDueDate();

            DateTime nearestDisableFrom = nearestDueDate.AddDays(-giroDates.countryDDLeadTime - 1);
            DateTime nearestDisableTo = nearestDueDate.AddDays(+giroDates.countryDDLeadTime - 1);

            if (giroDates.today >= nearestDisableFrom && giroDates.today <= nearestDisableTo)
            {
                // Rejections processing is disabled between these dates
                enableMsg = GetResource("M_DDREJECTIONSDISABLED", new object[] { nearestDisableFrom.ToLongDateString(), nearestDisableTo.ToLongDateString(), nearestDisableTo.AddDays(1).ToLongDateString() });
            }
            else
            {
                // Rejections processing is enabled
                enabled = true;
            }

            return enabled;
        }


        public void RejectPayments(SqlConnection conn, SqlTransaction trans, BDDMandate giroDates)
        {
            /* Update payments that have been rejected */
            DDDRejection dRejection = new DDDRejection();
            dRejection.RejectPayments(conn, trans,
                giroDates.today, giroDates.countryDDLeadTime, giroDates.countryFee);

            /* Rename the rejections file so it can not be processed again */
            File.Move(this._rejectFileName, this._processedFileName);
        }


        public DataTable FeeOnHoldList(SqlConnection conn, SqlTransaction trans)
        {
            /* Retrieve the fees on hold waiting to create their financial transaction */
            DataTable feeList = new DataTable();
            DDDRejection dRejection = new DDDRejection();
            feeList = dRejection.FeeOnHoldList(conn, trans);
            return feeList;
        }


        public void FeeOffHold(SqlConnection conn, SqlTransaction trans, int paymentId)
        {
            /* Take the fee payment off hold */
            DDDRejection dRejection = new DDDRejection();
            dRejection.FeeOffHold(conn, trans, paymentId);
        }


        public DataTable CreditList(SqlConnection conn, SqlTransaction trans, BDDMandate giroDates)
        {
            /* Create credit transactions for all payments that were submitted before 
            ** (today - lead time) and have not been rejected. */
            DataTable creditList = new DataTable();
            DDDRejection dRejection = new DDDRejection();
            creditList = dRejection.CreditList(conn, trans,
                giroDates.today, giroDates.countryDDLeadTime);
            return creditList;
        }


        public void CompletePayment(SqlConnection conn, SqlTransaction trans,
            int paymentId, string paymentType, string acctNo)
        {
            /* Mark the DD payment as complete and reset the rejection counter
            ** on the mandate for a successful Normal payment or Representation. */
            DDDRejection dRejection = new DDDRejection();
            dRejection.CompletePayment(conn, trans, paymentId, paymentType, acctNo);
        }


        //
        // Update Reject Actions in a list
        //
        public DataSet SaveDDRejectionList(SqlConnection conn, SqlTransaction trans,
            DataSet rejectionSet, out string acctNo, out string customerName)
        {
            // This method will process a list of Rejections. 
            // It will only update what has changed to help avoid locking
            // conflicts and will not reset data that has just been changed
            // by another session at the same time.
            // The out parameters allow a conflict to be reported to the user
            bool sessionConflict = false;
            acctNo = "";
            customerName = "";

            foreach (DataTable rejectionList in rejectionSet.Tables)
            {
                foreach (DataRow rejection in rejectionList.Rows)
                {
                    DDDRejection dRejection = new DDDRejection();

                    dRejection.rejectAction = (string)rejection[CN.RejectAction];
                    dRejection.paymentId = (int)rejection[CN.PaymentId];
                    dRejection.mandateId = (int)rejection[CN.MandateId];
                    dRejection.paymentType = (string)rejection[CN.PaymentType];
                    dRejection.origMonth = Convert.ToInt16(rejection[CN.OrigMonth]);
                    dRejection.amount = (decimal)rejection[CN.Amount];
                    dRejection.curRejectAction = (string)rejection[CN.CurRejectAction];

                    if (dRejection.rejectAction != dRejection.curRejectAction)
                    {
                        // Update the row, matching columns in case it
                        // has been changed by another session.
                        sessionConflict = dRejection.SaveRejectAction(conn, trans);
                    }

                    // Update the new current value
                    rejection[CN.CurRejectAction] = (string)rejection[CN.RejectAction];

                    if (sessionConflict)
                    {
                        acctNo = (string)rejection[CN.AcctNo];
                        customerName = (string)rejection[CN.CustomerName];
                        break;
                    }

                }
                if (sessionConflict) break;
            }
            // Return the current set in case there has been a session conflict
            // and the save needs to pick up from where it left off.
            return rejectionSet;
        }
    }
}

