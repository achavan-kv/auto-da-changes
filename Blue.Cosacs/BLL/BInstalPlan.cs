using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;


namespace STL.BLL
{
	/// <summary>
	/// Summary description for BInstalPlan.
	/// </summary>
	public class BInstalPlan : CommonObject
	{
		private short _monthsintfree = 0;
		public short MonthsInterestFree
		{
			get{return _monthsintfree;}
			set{_monthsintfree = value;}
		}
		private string _accountno = "";
		public string AccountNumber 
		{
			get{return _accountno;}
			set{_accountno = value;}
		}
		private int _agreementNo = 0;
		public int AgreementNumber
		{
			get{return _agreementNo;}
			set{_agreementNo = value;}
		}
        private string _band = "";
        public string Band
        {
            get
            {
                return _band;
            }
            set
            {
                _band = value;
            }
        }

		private DateTime _dateFirst = DateTime.MinValue.AddYears(1899);
		public DateTime DateFirst 
		{
			get{return _dateFirst;}
			set{_dateFirst = value;}
		}
		private DateTime _dateLast = DateTime.MinValue.AddYears(1899);
		public DateTime DateLast
		{
			get{return _dateLast;}
			set{_dateLast = value;}
		}
		private string _instalfreq = "";
		public string InstalmentFrequency 
		{
			get{return _instalfreq;}
			set{_instalfreq = value;}
		}
		private decimal _instalAmount = 0;
		public decimal InstalmentAmount 
		{
			get{return _instalAmount;}
			set{_instalAmount = value;}
		}
		private decimal _finalInstal = 0;
		public decimal FinalInstalment 
		{
			get{return _finalInstal;}
			set{_finalInstal = value;}
		}
		private int _instalNo = 0;
		public int NumberOfInstalments
		{
			get{return _instalNo;}
			set{_instalNo = value;}
		}
		private short _origBr = 0;
		public short OrigBr
		{
			get{return _origBr;}
			set{_origBr = value;}
		}
		private decimal _instalTotal = 0;
		public decimal InstalTotal 
		{
			get{return _instalTotal;}
			set{_instalTotal = value;}
		}
		private short _dueday = 0;
		public short DueDay
		{
			get{return _dueday;}
			set{_dueday = value;}
		}

        private bool _autoda;
        public bool autoda
        {
            get { return _autoda; }
            set { _autoda = value; }
        }

        private Int16 _PrefDay = 0;
        public Int16 PrefDay
        {
            get { return _PrefDay; }
            set { _PrefDay = value; }
        }

        public void Save(SqlConnection conn, SqlTransaction trans)
		{
			DInstalPlan instal = new DInstalPlan();
			instal.OrigBr = this.OrigBr;
			instal.AccountNumber = this.AccountNumber;
			instal.AgreementNumber = this.AgreementNumber;
			instal.DateFirst = this.DateFirst;
			instal.DateLast = this.DateLast;
			instal.NumberOfInstalments = this.NumberOfInstalments;
			instal.InstalmentFrequency = this.InstalmentFrequency;
			instal.InstalmentAmount = this.InstalmentAmount;
			instal.FinalInstalment = this.FinalInstalment;
			instal.InstalTotal = this.InstalTotal;
			instal.MonthsInterestFree = this.MonthsInterestFree;
			instal.User = User;
			instal.DueDay = this.DueDay;
            instal.Band = Band;
            instal.autoda = this._autoda;
            instal.PrefDay = this._PrefDay; 
			instal.Save(conn, trans);
		}

        public void AutoDA(SqlConnection conn, SqlTransaction trans)
        {
            DInstalPlan instal = new DInstalPlan();
            instal.AccountNumber = this.AccountNumber;
            instal.User = this.User;
            instal.AutoDA(conn, trans);
        }

		public void UpdateDateFirst(SqlConnection conn, SqlTransaction trans, DataSet ds)
		{
			DInstalPlan instal = new DInstalPlan();
			DAccount acct = new DAccount();

			foreach (DataRow row in ds.Tables[0].Rows)
			{
				instal.AccountNumber = (string)row[CN.AcctNo];
				instal.DateFirst = Convert.ToDateTime(row[CN.DateFirst]);
				instal.User = this.User;
				instal.UpdateDateFirst(conn, trans);

				acct.AccountNumber = (string)row[CN.AcctNo];
				acct.CalcArrears(conn, trans, 0, 0);
			}
		}

		public DataSet GetAccounts()
		{
			DataSet ds = new DataSet();	
			DInstalPlan instal = new DInstalPlan();
			instal.AccountNumber = this.AccountNumber;
			instal.GetAccounts();

			ds.Tables.Add(instal.Accounts);
			return ds;
		}

		public short GetDueDay(string custID)
		{
			DInstalPlan ip = new DInstalPlan();
			ip.GetDueDay(custID);

			return ip.DueDay;
		}

		public void SaveVariableInstalments(SqlConnection conn, SqlTransaction trans, DataTable dt)
		{
			DInstalPlan instal = new DInstalPlan();

			foreach (DataRow row in dt.Rows)
			{
				instal.SaveVariableInstalments(conn, trans,
												(string)row[CN.AcctNo],
												Convert.ToInt16(row[CN.InstalOrder]),
												Convert.ToDecimal(row[CN.Instalment2]),
												Convert.ToInt16(row[CN.InstalmentNumber]),
												Convert.ToDateTime(row[CN.DateFrom]),
												Convert.ToDecimal(row[CN.ServiceCharge]));
			}
		}
		
		public DataSet GetVariableInstalmentsByAcctNo(string acctNo)
		{
			DInstalPlan dInstal = new DInstalPlan();
			return dInstal.GetVariableInstalmentsByAcctNo(acctNo);
		}

		public BInstalPlan()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
