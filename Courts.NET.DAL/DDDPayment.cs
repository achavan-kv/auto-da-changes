using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DDDPayment.
    /// </summary>
    public class DDDPayment : DALObject
    {
        public DDDPayment()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public string BankPayCode(SqlConnection conn, SqlTransaction trans,
            string paymentType, int dueDayId)
        {
            // Get the bank Pay Code for the payment type and due day
            string payCode = "";

            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piPaymentType", SqlDbType.NChar, 1);
                parmArray[0].Value = paymentType;
                parmArray[1] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[1].Value = dueDayId;
                parmArray[2] = new SqlParameter("@poPayCode", SqlDbType.NChar, 5);
                parmArray[2].Direction = ParameterDirection.Output;

                result = this.RunSP(conn, trans, "DN_DDBankPayCodeSP", parmArray);

                if (result == 0 && !Convert.IsDBNull(parmArray[2].Value))
                {
                    payCode = (string)parmArray[2].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return payCode;
        }


        public DataTable PendingPaymentList(SqlConnection conn, SqlTransaction trans,
            string paymentType, int dueDayId)
        {
            // Get the list of pending payments for this payment type and due day
            DataTable pendingPayments = new DataTable();

            try
            {
                parmArray = new SqlParameter[2];

                parmArray[0] = new SqlParameter("@piPaymentType", SqlDbType.NChar,1);
                parmArray[0].Value = paymentType;
                parmArray[1] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[1].Value = dueDayId;

                this.RunSP(conn, trans, "DN_DDPendingPaymentListSP", parmArray, pendingPayments);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return pendingPayments;
        }


        public void SubmittedToBank(SqlConnection conn, SqlTransaction trans,
            string paymentType, int dueDayId, DateTime effectiveDate)
        {
            // Mark the pending payments as submitted to the bank
            try
            {
                parmArray = new SqlParameter[3];

                parmArray[0] = new SqlParameter("@piPaymentType", SqlDbType.NChar,1);
                parmArray[0].Value = paymentType;
                parmArray[1] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[1].Value = dueDayId;
                parmArray[2] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[2].Value = effectiveDate;

                this.RunSP(conn, trans, "DN_DDSubmittedToBankSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public int SubmitPayments(SqlConnection conn, SqlTransaction trans,
            int dueDayId, DateTime dueDate, DateTime effectiveDate, int minPeriod)
        {
            // Add new due payments to the DDPAYMENT table
            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[0].Value = dueDayId;
                parmArray[1] = new SqlParameter("@piDueDate", SqlDbType.SmallDateTime);
                parmArray[1].Value = dueDate;
                parmArray[2] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[2].Value = effectiveDate;
                parmArray[3] = new SqlParameter("@piMinPeriod", SqlDbType.Int);
                parmArray[3].Value = minPeriod;

                result = this.RunSP(conn, trans, "DN_DDSubmitPaymentsSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        public int TidyPayments(SqlConnection conn, SqlTransaction trans)
        {
            // Delete data over two months old from the DDPayment table
            try
            {
                result = this.RunSP(conn, trans, "DN_DDTidyPaymentsSP");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

    }
}
