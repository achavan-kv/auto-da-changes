using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.Giro;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;


namespace STL.BLL
{
    /// <summary>
    /// Singapore Direct Debit Extra Payments entered by the user
    /// </summary>
    /// 


    public class BDDPaymentExtra : CommonObject
    {
        //
        // DA properties
        //

        //
        // BL private properties
        //

        //
        // BL Methods
        //
        public BDDPaymentExtra()
        {
        }



        //
        // Load a list of Extra Payments
        //
        public DataSet GetDDPaymentExtraList(string countryCode)
        {
            BDDMandate mandate = new BDDMandate(countryCode, DateTime.Today);
            DDDPaymentExtra extraPayment = new DDDPaymentExtra();
            return extraPayment.GetExtraList(mandate.effectiveDate);
        }


        //
        // Insert, update or delete extra payments in a list
        //
        public DataSet SaveDDPaymentExtraList(SqlConnection conn, SqlTransaction trans,
            DataSet extraPaymentSet, out string acctNo, out string customerName)
        {
            // This method will process a list of extra payments. 
            // It will only update what has changed to help avoid locking
            // conflicts and will not reset data that has just been changed
            // by another session at the same time.
            // The out parameters allow a conflict to be reported to the user
            bool sessionConflict = false;
            acctNo = "";
            customerName = "";

            foreach (DataTable extraPaymentList in extraPaymentSet.Tables)
            {
                foreach (DataRow extraPayment in extraPaymentList.Rows)
                {
                    DDDPaymentExtra dExtraPayment = new DDDPaymentExtra();

                    dExtraPayment.mandateId = (int)extraPayment[CN.MandateId];
                    dExtraPayment.paymentId = (int)extraPayment[CN.PaymentId];
                    dExtraPayment.origMonth = Convert.ToInt16(extraPayment[CN.OrigMonth]);
                    dExtraPayment.curAmount = (decimal)extraPayment[CN.CurAmount];
                    dExtraPayment.amount = (decimal)extraPayment[CN.Amount];
                    if (dExtraPayment.amount < 0.01M) extraPayment[CN.Consent] = false;
                    dExtraPayment.curConsent = Convert.ToBoolean(extraPayment[CN.CurConsent]);
                    dExtraPayment.consent = Convert.ToBoolean(extraPayment[CN.Consent]);

                    if (dExtraPayment.curConsent == false
                        && dExtraPayment.consent == true)
                    {
                        // Insert a new row, but check another session has not got here first
                        sessionConflict = dExtraPayment.InsertExtraPayment(conn, trans);
                    }
                    else if (dExtraPayment.curConsent == true
                        && dExtraPayment.consent == true
                        && dExtraPayment.amount != dExtraPayment.curAmount)
                    {
                        // Update the row, matching columns in case it
                        // has been changed by another session.
                        sessionConflict = dExtraPayment.UpdateExtraPayment(conn, trans);
                    }
                    else if (dExtraPayment.curConsent == true
                        && dExtraPayment.consent == false)
                    {
                        // Delete the row, matching columns in case
                        // changed by another session.
                        sessionConflict = dExtraPayment.DeleteExtraPayment(conn, trans);
                    }

                    // Update the new current values
                    extraPayment[CN.CurAmount] = (decimal)extraPayment[CN.Amount];
                    extraPayment[CN.CurConsent] = Convert.ToBoolean(extraPayment[CN.Consent]);
                    extraPayment[CN.CurDDPending] = (decimal)extraPayment[CN.DDPending];

                    if (sessionConflict)
                    {
                        acctNo = (string)extraPayment[CN.AcctNo];
                        customerName = (string)extraPayment[CN.CustomerName];
                        break;
                    }

                }
                if (sessionConflict) break;
            }
            // Return the current set in case there has been a session conflict
            // and the save needs to pick up from where it left off.
            return extraPaymentSet;
        }
    }
}
