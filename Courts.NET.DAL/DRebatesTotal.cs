using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;


namespace STL.DAL
{
    /// <summary>
    /// Data Access methods for the Rebates_Total table.
    /// </summary>
    public class DRebatesTotal : DALObject
    {
        private int _sequence = 0;
        public int Sequence
        {
            get{return _sequence;}
            set{_sequence = value;}
        }

        private int _branchNo = 0;
        public int BranchNo
        {
            get{return _branchNo;}
            set{_branchNo = value;}
        }

        private string _arrearsGroup = "";
        public string ArrearsGroup
        {
            get{return _arrearsGroup;}
            set{_arrearsGroup = value;}
        }

        private decimal _rebate = 0;
        public decimal Rebate
        {
            get{return _rebate;}
            set{_rebate = value;}
        }

        private decimal _rebateWithin12Mths = 0;
        public decimal RebateWithin12Mths
        {
            get{return _rebateWithin12Mths;}
            set{_rebateWithin12Mths = value;}
        }

        private decimal _rebateAfter12Mths = 0;
        public decimal RebateAfter12Mths
        {
            get{return _rebateAfter12Mths;}
            set{_rebateAfter12Mths = value;}
        }

        private DataTable _rebatesByBranch = null;
        public DataTable RebatesByBranch
        {
            get{return _rebatesByBranch;}
        }

        private DataTable _rebatesTotals = null;
        public DataTable RebatesTotals
        {
            get{return _rebatesTotals;}
        }

        public DateTime AsAtDate
        {
            get
            {
                DateTime asAtDate;
                GetRebatesAsAt(out asAtDate);
                return asAtDate;
            }
        }

        public DataSet GetRebatesTotal(int branchNo)
        {
            DataSet ds = new DataSet();

            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                result = this.RunSP("DN_RebateTotalGetSP", parmArray, ds);
                _rebatesByBranch = ds.Tables[0];
                _rebatesTotals = ds.Tables[1];
                _rebatesByBranch.TableName = TN.RebatesByBranch;
                _rebatesTotals.TableName = TN.RebatesTotals;
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
            return ds;
        }

        public DataSet GetRebatesAsAt(out DateTime asAtDate)
        {
            DataSet ds = new DataSet();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@asatdate", SqlDbType.DateTime);
                parmArray[0].Direction = System.Data.ParameterDirection.Output;
                this.RunSP("DN_GetRebateAsAtDateSP", parmArray,ds);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            finally 
            {
                asAtDate = Convert.ToDateTime(parmArray[0].Value);
            }
            return ds;
        }

        public void UpdateTotals(SqlConnection conn, SqlTransaction trans,
                    string acctNo,
                    DateTime fromThresDate,
                    DateTime toThresDate,
                    DateTime acctsFromDate,
                    DateTime ruleChangeDate,
                    DateTime rebateDate,
                    out decimal poRebate,
                    out decimal poRebateWithin12Mths,
                    out decimal poRebateAfter12Mths)
        {
            try
            {
                poRebate = 0;
                poRebateWithin12Mths = 0;
                poRebateAfter12Mths = 0;
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@AcctNo", SqlDbType.VarChar,12);
                parmArray[0].Value = acctNo;
                parmArray[1] = new SqlParameter("@FromThresDate", SqlDbType.DateTime);
                parmArray[1].Value = fromThresDate;
                parmArray[2] = new SqlParameter("@UntilThresDate", SqlDbType.DateTime);
                parmArray[2].Value = toThresDate;
                parmArray[3] = new SqlParameter("@FromDate", SqlDbType.DateTime);
                parmArray[3].Value = acctsFromDate;
                parmArray[4] = new SqlParameter("@RuleChangeDate", SqlDbType.DateTime);
                parmArray[4].Value = ruleChangeDate;
                parmArray[5] = new SqlParameter("@RebateDate", SqlDbType.DateTime);
                parmArray[5].Value = rebateDate;
                parmArray[6] = new SqlParameter("@poRebate", SqlDbType.Money);
                parmArray[6].Direction = ParameterDirection.Output;
                parmArray[7] = new SqlParameter("@poRebateWithin12Mths", SqlDbType.Money);
                parmArray[7].Direction = ParameterDirection.Output;
                parmArray[8] = new SqlParameter("@poRebateAfter12Mths", SqlDbType.Money);
                parmArray[8].Direction = ParameterDirection.Output;
                this.RunSP(conn, trans, "DN_RebateSP",parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            finally 
            {
                if (parmArray[6].Value != DBNull.Value) 
                {
                    poRebate = Convert.ToDecimal(parmArray[6].Value);
                }
                if (parmArray[7].Value != DBNull.Value)
                {
                    poRebateWithin12Mths = Convert.ToDecimal(parmArray[7].Value);
                }
                if (parmArray[8].Value != DBNull.Value)
                {
                    poRebateAfter12Mths = Convert.ToDecimal(parmArray[8].Value);
                }
            }
        }


        public DRebatesTotal()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
