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
	/// Summary description for DBailAction.
	/// </summary>
	public class DBailAction : DALObject
	{
		private string _acctno = "";
		public string AccountNo
		{
			get{return _acctno;}
			set{_acctno = value;}
		}

		private int _allocno = 0;
		public int AllocNo 
		{
			get{return _allocno;}
			set{_allocno = value;}
		}

		private int _empeeno = 0;
		public int EmployeeNo
		{
			get{return _empeeno;}
			set{_empeeno = value;}
		}

		private DateTime _dateAdded;
		public DateTime DateAdded
		{
			get{return _dateAdded;}
			set{_dateAdded = value;}
		}

		private string _code = "";
		public string Code
		{
			get{return _code;}
			set{_code = value;}
		}

		private string _notes = "";
		public string Notes
		{
			get{return _notes;}
			set{_notes = value;}
		}

		private DateTime _dateDue = DateTime.MinValue.AddYears(1899);
		public DateTime DateDue
		{
			get{return _dateDue;}
			set{_dateDue = value;}
		}

        private DateTime _reminderDateTime = DateTime.MinValue.AddYears(1899);
        public DateTime ReminderDateTime
        {
            get { return _reminderDateTime; }
            set { _reminderDateTime = value; }
        }

        //NM & IP - 02/01/09 - CR976 - Call Reminders
        private string _callingSource = string.Empty;
        public string CallingSource
        {
            get { return _callingSource; }
            set { _callingSource = value; }
        }
        
       //NM & IP - 02/01/09 - CR976 - Call Reminders
       private bool _cancelOutstandingReminders = false;
       public bool CancelOutstandingReminders
        {
            get { return _cancelOutstandingReminders; }
            set { _cancelOutstandingReminders = value; }
        }

		private short _actionNo = 0;
		public short ActionNo
		{
			get{return _actionNo;}
			set{_actionNo = value;}
		}

		private double _actionvalue = 0;
		public double ActionValue
		{
			get{return _actionvalue;}
			set{_actionvalue = value;}
		}

		private double _amtcommpaidon = 0;
		public double AmtCommPaidOn
		{
			get{return _amtcommpaidon;}
			set{_amtcommpaidon = value;}
		}

		private DataTable _bailActions = null;
		public DataTable BailActions
		{
			get{return _bailActions;}
		}

		private int _addedby = 0;
		public int AddedBy
		{
			get{return _addedby;}
			set{_addedby = value;}
		}

        private DateTime _spadateexpiry = DateTime.MinValue.AddYears(1899);
        public DateTime SpaDateExpiry
        {
            get{return _spadateexpiry;}
            set{_spadateexpiry = value;}
        }

        private string _spareasoncode = "";
        public string SpaReasonCode
        {
            get{return _spareasoncode;}
            set{_spareasoncode = value;}
        }

        private double _spainstal = 0;
        public double SpaInstal
        {
            get{return _spainstal;}
            set{_spainstal = value;}
        }


		public void Save(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[16];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNo;
				parmArray[1] = new SqlParameter("@employeeno", SqlDbType.Int);
				parmArray[1].Value = this.EmployeeNo;
				parmArray[2] = new SqlParameter("@dateadded", SqlDbType.DateTime);
				parmArray[2].Value = this.DateAdded;
				parmArray[3] = new SqlParameter("@code", SqlDbType.NVarChar,4);
				parmArray[3].Value = this.Code;
				parmArray[4] = new SqlParameter("@notes", SqlDbType.NVarChar,700);
				parmArray[4].Value = this.Notes;
				parmArray[5] = new SqlParameter("@datedue", SqlDbType.DateTime);
				parmArray[5].Value = this.DateDue;
				parmArray[6] = new SqlParameter("@actionvalue", SqlDbType.Float);
				parmArray[6].Value = this.ActionValue;
				parmArray[7] = new SqlParameter("@amtcommpaidon", SqlDbType.Float);
				parmArray[7].Value = this.AmtCommPaidOn;
				parmArray[8] = new SqlParameter("@addedby", SqlDbType.Int);
				parmArray[8].Value = this.AddedBy;
                parmArray[9] = new SqlParameter("@spadateexpiry", SqlDbType.DateTime);
                parmArray[9].Value = this.SpaDateExpiry;
                parmArray[10] = new SqlParameter("@spareasoncode", SqlDbType.NVarChar,4);
                parmArray[10].Value = this.SpaReasonCode;
                parmArray[11] = new SqlParameter("@spainstal", SqlDbType.Float);
                parmArray[11].Value = this.SpaInstal;
                parmArray[12] = new SqlParameter("@remDateTime", SqlDbType.DateTime);
                parmArray[12].Value = this.ReminderDateTime;
                parmArray[13] = new SqlParameter("@deleteAllReminders", SqlDbType.Bit);
                parmArray[13].Value = this.CancelOutstandingReminders;
                parmArray[14] = new SqlParameter("@callingSource", SqlDbType.VarChar, 10);
                parmArray[14].Value = this.CallingSource;
                parmArray[15] = new SqlParameter("@allocno", SqlDbType.Int);
                parmArray[15].Value = this.AllocNo;

				this.RunSP(conn, trans, "DN_BailActionSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public int GetBailActions(string accountNo)
		{
			_bailActions = new DataTable("BailActions");

			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;
						
				result = this.RunSP("DN_BailActionGetSP", parmArray, _bailActions);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public DBailAction()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
