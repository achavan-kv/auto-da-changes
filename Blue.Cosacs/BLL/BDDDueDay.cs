using System;
using System.Data;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.ColumnNames;

namespace STL.BLL
{
    /// <summary>
    /// Summary description for DDDueDay.
    /// </summary>
    public class BDDDueDay : CommonObject
    {
        //
        // DA properties
        //
        private int _dueDayId;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _dueDay;

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


        //
        // BL private properties
        //
        private DDDDueDay _dddueDay = new DDDDueDay();

        //
        // BL Methods
        //
        public BDDDueDay()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public DataSet Get(int dueDayId)
        {
            int rowCount = 0;
            rowCount = _dddueDay.Get(dueDayId);
            return this.ReturnProperties(rowCount);
        }


        private DataSet ReturnProperties(int rowCount)
        {
            DataSet dueDaySet = null;

            if (rowCount > 0)
            {
                // Get OK - copy properties
                this._dueDayId = _dddueDay.dueDayId;
                this._startDate = _dddueDay.startDate;
                this._endDate = _dddueDay.endDate;
                this._dueDay = _dddueDay.dueDay;

                // Return as a DatSet

                DataTable details = new DataTable("DueDayDetails");
                details.Columns.AddRange(new DataColumn[]	{   new DataColumn(CN.DueDay),
																new DataColumn(CN.DueDayId),
																new DataColumn(CN.EndDate),
																new DataColumn(CN.StartDate) });

                DataRow row = details.NewRow();

                row[CN.DueDay] = _dueDay;
                row[CN.DueDayId] = _dueDayId;
                row[CN.EndDate] = _endDate;
                row[CN.StartDate] = _startDate;

                details.Rows.Add(row);

                dueDaySet = new DataSet();
                dueDaySet.Tables.Add(details);
            }

            return dueDaySet;
        }


        public int GetDueDayList()
        {
            int result = _dddueDay.GetDueDayList();
            _dueDayList = _dddueDay.dueDayList.Copy();
            foreach (DataRow row in _dueDayList.Rows)
            {
                if (row[CN.DueDay].ToString() == "1"
                    || row[CN.DueDay].ToString() == "21"
                    || row[CN.DueDay].ToString() == "31")
                {
                    row[CN.DueDay] = row[CN.DueDay].ToString() + " st";
                }
                else if (row[CN.DueDay].ToString() == "2"
                         || row[CN.DueDay].ToString() == "22")
                {
                    row[CN.DueDay] = row[CN.DueDay].ToString() + " nd";
                }
                else if (row[CN.DueDay].ToString() == "3"
                         || row[CN.DueDay].ToString() == "23")
                {
                    row[CN.DueDay] = row[CN.DueDay].ToString() + " rd";
                }
                else if (row[CN.DueDay].ToString().Trim() != "")
                {
                    row[CN.DueDay] = row[CN.DueDay].ToString() + " th";
                }
            }
            return result;
        }

    }
}
