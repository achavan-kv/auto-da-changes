using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace STL.DAL
{
    public class DBSPaymentDetails : DALObject
    {
        public decimal TotalAmountReceived { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalCheque { get; set; }
        public decimal CashChange { get; set; }
        public decimal ChequeChange { get; set; }
        public decimal TotalPaymentPaid { get; set; }
        public List<PaymentDetail> PaymentDetails { get; set; }

        public void Save(SqlConnection conn, SqlTransaction trans, string transactionType)
        {
            try
            {
                DataTable paymentDetailTable = new DataTable();

                var acctNo = new DataColumn();
                acctNo.ColumnName = "AcctNo";
                acctNo.DataType = typeof(string);
                paymentDetailTable.Columns.Add(acctNo);

                var dateTrans = new DataColumn();
                dateTrans.ColumnName = "DateTrans";
                dateTrans.DataType = typeof(DateTime);
                paymentDetailTable.Columns.Add(dateTrans);

                var transTypeCode = new DataColumn();
                transTypeCode.ColumnName = "TransTypeCode";
                transTypeCode.DataType = typeof(string);
                paymentDetailTable.Columns.Add(transTypeCode);

                var transRefNo = new DataColumn();
                transRefNo.ColumnName = "TransRefNo";
                transRefNo.DataType = typeof(int);
                paymentDetailTable.Columns.Add(transRefNo);

                var transValue = new DataColumn();
                transValue.ColumnName = "TransValue";
                transValue.DataType = typeof(decimal);
                paymentDetailTable.Columns.Add(transValue);

                var finTransId = new DataColumn();
                finTransId.ColumnName = "FinTransId";
                finTransId.DataType = typeof(int);
                paymentDetailTable.Columns.Add(finTransId);

                var origionalFinTransId = new DataColumn();
                origionalFinTransId.ColumnName = "OrigionalFinTransId";
                origionalFinTransId.DataType = typeof(int);
                paymentDetailTable.Columns.Add(origionalFinTransId);

                var isDeposit = new DataColumn();
                isDeposit.ColumnName = "IsDeposit";
                isDeposit.DataType = typeof(bool);
                paymentDetailTable.Columns.Add(isDeposit);

                var isSuccessful = new DataColumn();
                isSuccessful.ColumnName = "IsSuccessful";
                isSuccessful.DataType = typeof(bool);
                paymentDetailTable.Columns.Add(isSuccessful);

                var errorMessage = new DataColumn();
                errorMessage.ColumnName = "ErrorMessage";
                errorMessage.DataType = typeof(string);
                paymentDetailTable.Columns.Add(errorMessage);

                if (PaymentDetails != null)
                {
                    foreach (var paymentDetail in PaymentDetails)
                    {
                        DataRow newRow = paymentDetailTable.NewRow();
                        newRow[0] = paymentDetail.AcctNo;

                        if (paymentDetail.DateTrans.HasValue)
                        {
                            newRow[1] = paymentDetail.DateTrans.Value;
                        }
                        else
                        {
                            newRow[1] = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(paymentDetail.TransTypeCode))
                        {
                            newRow[2] = paymentDetail.TransTypeCode;
                        }
                        else
                        {
                            newRow[2] = DBNull.Value;
                        }

                        if (paymentDetail.TransRefNo.HasValue)
                        {
                            newRow[3] = paymentDetail.TransRefNo;
                        }
                        else
                        {
                            newRow[3] = DBNull.Value;
                        }

                        if (paymentDetail.TransValue.HasValue)
                        {
                            newRow[4] = paymentDetail.TransValue;
                        }
                        else
                        {
                            newRow[4] = DBNull.Value;
                        }

                        if (paymentDetail.FinTransId.HasValue)
                        {
                            newRow[5] = paymentDetail.FinTransId.Value;
                        }
                        else
                        {
                            newRow[5] = DBNull.Value;
                        }


                        if (paymentDetail.OrigionalFinTransId.HasValue)
                        {
                            newRow[6] = paymentDetail.OrigionalFinTransId.Value;
                        }
                        else
                        {
                            newRow[6] = DBNull.Value;
                        }

                        newRow[7] = paymentDetail.IsDeposit;

                        if (paymentDetail.IsSuccessful.HasValue)
                        {
                            newRow[8] = paymentDetail.IsSuccessful.Value;
                        }
                        else
                        {
                            newRow[8] = DBNull.Value;
                        }

                        if (!string.IsNullOrEmpty(paymentDetail.ErrorMessage))
                        {
                            newRow[9] = paymentDetail.ErrorMessage;
                        }
                        else
                        {
                            newRow[9] = DBNull.Value;
                        }

                        paymentDetailTable.Rows.Add(newRow);
                    }

                    parmArray = new SqlParameter[8];
                    parmArray[0] = new SqlParameter("@TotalAmountReceived", SqlDbType.Money);
                    parmArray[0].Value = this.TotalAmountReceived;
                    parmArray[1] = new SqlParameter("@TotalCash", SqlDbType.Money);
                    parmArray[1].Value = this.TotalCash;
                    parmArray[2] = new SqlParameter("@TotalCheque", SqlDbType.Money);
                    parmArray[2].Value = this.TotalCheque;
                    parmArray[3] = new SqlParameter("@CashChange", SqlDbType.Money);
                    parmArray[3].Value = this.CashChange;
                    parmArray[4] = new SqlParameter("@ChequeChange", SqlDbType.Money);
                    parmArray[4].Value = this.ChequeChange;
                    parmArray[5] = new SqlParameter("@TotalPaymentPaid", SqlDbType.Money);
                    parmArray[5].Value = this.TotalPaymentPaid;
                    parmArray[6] = new SqlParameter("@TransactionType", SqlDbType.VarChar,10);
                    parmArray[6].Value = transactionType;
                    parmArray[7] = new SqlParameter("@PaymentDetails", SqlDbType.Structured);
                    parmArray[7].Value = paymentDetailTable;

                    this.RunSP(conn, trans, "SP_BS_InsertPaymentDetailInfo", parmArray);
                }
            }

            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public DataSet GetTransactionDetails(int FinTransId)
        {
            DataSet transDetails = new DataSet();

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@FintransID", SqlDbType.Int);
                parmArray[0].Value = FinTransId;

                this.RunSP("SP_BS_TransactionPaymentDetail", parmArray, transDetails);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return transDetails;
        }
    }

    public class PaymentDetail
    {
        public string AcctNo { get; set; }
        public DateTime? DateTrans { get; set; }
        public string TransTypeCode { get; set; }
        public DateTime? Acctopened { get; set; }
        public int? TransRefNo { get; set; }
        public decimal? TransValue { get; set; }
        public int? FinTransId { get; set; }
        public int? OrigionalFinTransId { get; set; }
        public bool IsDeposit { get; set; }
        public bool? IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }

}
