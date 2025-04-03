using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Static;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DDDDueDay.
    /// </summary>
    public class DDDDueDay : DALObject
    {
        private int _dueDayId;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _dueDay;

        private DataTable _dueDayRow = null;
        private DataTable _dueDayList = null;

        public int dueDayId
        {
            get
            {
                return _dueDayId;
            }
            set
            {
                _dueDayId = value;
            }
        }

        public DateTime startDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        public DateTime endDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
        }

        public int dueDay
        {
            get
            {
                return _dueDay;
            }
            set
            {
                _dueDay = value;
            }
        }


        public DataTable dueDayList
        {
            get { return _dueDayList; }
            set { _dueDayList = value; }
        }



        public DDDDueDay()
        {
            this.Init();
        }

        public void Init()
        {
            // Constructor Init
            _dueDayId = 0;
            _startDate = new System.DateTime(0);
            _endDate = new System.DateTime(0);
            _dueDay = 0;
        }


        public int Get(int dueDayID)
        {
            int result = 0;
            int rowCount = 0;
            try
            {
                this.Init();
                parmArray = new SqlParameter[1];

                parmArray[0] = new SqlParameter("@piDueDayId", SqlDbType.Int);
                parmArray[0].Value = _dueDayId;

                _dueDayRow = new DataTable();
                result = this.RunSP("DN_DDDueDayGetSP", parmArray, _dueDayRow);

                if (result == 0 && this._dueDayRow.Rows.Count > 0)
                {
                    rowCount = 1;
                    this._dueDayId = (int)this._dueDayRow.Rows[0][CN.DueDayId];
                    this._startDate = (DateTime)this._dueDayRow.Rows[0][CN.StartDate];
                    if (!Convert.IsDBNull(this._dueDayRow.Rows[0][CN.EndDate]))
                        this._endDate = (DateTime)this._dueDayRow.Rows[0][CN.EndDate];
                    this._dueDay = (int)this._dueDayRow.Rows[0][CN.DueDay];
                }

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return rowCount;
        }


        public int GetDueDayList()
        {
            try
            {
                _dueDayList = new DataTable(TN.DDDueDate);
                result = this.RunSP("DN_DDDueDayListSP", _dueDayList);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }


        public DateTime NextDueDate(DateTime effectiveDate, out int dueDayId, out short dueDay)
        {
            int result = 0;
            DateTime nextDueDate = Date.blankDate;
            dueDayId = 0;
            dueDay = 0;
            try
            {
                DataTable nextDueDateRow = new DataTable();
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@piEffectiveDate", SqlDbType.SmallDateTime);
                parmArray[0].Value = effectiveDate;
                parmArray[1] = new SqlParameter("@poDueDate", SqlDbType.SmallDateTime);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@poDueDayId", SqlDbType.Int);
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@poDueDay", SqlDbType.SmallInt);
                parmArray[3].Direction = ParameterDirection.Output;

                result = this.RunSP("DN_DDNextDueDateSP", parmArray);

                if (result == 0)
                {
                    if (!Convert.IsDBNull(parmArray[1].Value)) nextDueDate = (DateTime)parmArray[1].Value;
                    if (!Convert.IsDBNull(parmArray[2].Value)) dueDayId = (int)parmArray[2].Value;
                    if (!Convert.IsDBNull(parmArray[3].Value)) dueDay = (short)parmArray[3].Value;
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return nextDueDate;
        }

    }
}

