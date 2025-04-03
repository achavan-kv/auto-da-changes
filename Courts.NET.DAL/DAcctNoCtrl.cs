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
    /// Data Access methods for the AcctNoCtrl table.
    /// </summary>
    public class DAcctNoCtrl : DALObject
    {
        private int _branchno = 0;
        public int BranchNo
        {
            get{return _branchno;}
            set{_branchno = value;}
        }

        private string _acctcat = "";
        public string AcctCat
        {
            get{return _acctcat;}
            set{_acctcat = value;}
        }

        private string _acctcatdesc = "";
        public string AcctCatDesc
        {
            get{return _acctcatdesc;}
            set{_acctcatdesc = value;}
        }

        private int _hiallocated = 0;
        public int HiAllocated
        {
            get{return _hiallocated;}
            set{_hiallocated = value;}
        }

        private int _hiallowed = 0;
        public int HiAllowed
        {
            get{return _hiallowed;}
            set{_hiallowed = value;}
        }

        private DataTable _AcctNoCtrldata = null;
        public DataTable AcctNoCtrlData
        {
            get{return _AcctNoCtrldata;}
        }

        /// <summary>
        /// Returns a datatable of AcctNoCtrl details.
        /// </summary>
        /// <param name="branchNo">The branch to retrieve AcctNoCtrl rows for.</param>
        /// <returns>int Status value</returns>
        public int GetAcctNoCtrl(int branchNo)
        {
            try
            {
                _AcctNoCtrldata = new DataTable(TN.AcctNoCtrl);			
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = branchNo;
                result = this.RunSP("DN_AcctNoCtrlGetSP", parmArray, _AcctNoCtrldata);
			
                if(result==0)
                {
                    result = (int)Return.Success;
                }
                else
                {
                    result = (int)Return.Fail;
                }
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@branchno", SqlDbType.SmallInt);
                parmArray[0].Value = this.BranchNo;
                parmArray[1] = new SqlParameter("@acctcat", SqlDbType.VarChar,3);
                parmArray[1].Value = this.AcctCat;
                parmArray[2] = new SqlParameter("@acctcatdesc", SqlDbType.VarChar,25);
                parmArray[2].Value = this.AcctCatDesc;
                parmArray[3] = new SqlParameter("@hiallocated", SqlDbType.Int);
                parmArray[3].Value = this.HiAllocated;
                parmArray[4] = new SqlParameter("@hiallowed", SqlDbType.Int);
                parmArray[4].Value = this.HiAllowed;
                this.RunSP(conn, trans, "DN_AcctNoCtrlSaveSP", parmArray);
            }
            catch(SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }			
        }

        public DAcctNoCtrl()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
