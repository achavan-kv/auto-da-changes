using System;
using STL.DAL;
using STL.Common;
using System.Data.SqlClient;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BLetter.
	/// </summary>
	public class BLetter : CommonObject
	{
		public void Write(SqlConnection conn, SqlTransaction trans, string accountNo, 
							DateTime letterDate, DateTime letterDue, string letterCode, 
							decimal addToValue, string excelGen)
		{
			DLetter l = new DLetter();
			l.AccountNo = accountNo;
			l.LetterCode = letterCode;
			l.LetterDate = letterDate;
			l.LetterDue = letterDue;
			l.AddToValue = addToValue;
            l.ExcelGen = excelGen;
			l.Write(conn, trans);
		}

		public void WritePotentialSpend(SqlConnection conn, SqlTransaction trans, string accountNo, 
			DateTime letterDate, DateTime letterDue, string letterCode, 
			decimal addToValue)
		{
			DLetter l = new DLetter();
			l.AccountNo = accountNo;
			l.LetterCode = letterCode;
			l.LetterDate = letterDate;
			l.LetterDue = letterDue;
			l.AddToValue = addToValue;
			l.WritePotentialSpend(conn, trans);
		}


        public void WriteBHLetter(SqlConnection conn, SqlTransaction trans, string accountNo, decimal oldLimit, decimal newLimit,
            string NewBand, string OldBand )
        {
            string letterCode = "";

            if (newLimit > oldLimit) //higher limit
                   letterCode = "BHHL";


            // checking whether old band is numeric -- if old band numeric and new band smaller then both on behavioural
            // then will get a better interest rate. Don't do this if previously on applicant and now on behavioural
            double result = 0;
            bool oldisNum = double.TryParse(OldBand, out result);

            // same limit but better interest rate.
            if ( newLimit ==oldLimit && NewBand.CompareTo(OldBand) <0 && oldisNum) //better interest rate looking at lower band
                    letterCode = "BHBI";

            // newband 1 is best 5 is worst... 
            if (NewBand.CompareTo(OldBand) < 0  && newLimit > oldLimit && oldisNum)
                letterCode = "BHHB"; //better interest rate and higher limit. 

            if (letterCode != "")
            {
                Write(conn, trans, accountNo, DateTime.Now, DateTime.Now, letterCode, 0, "0");
            }
        }

		public BLetter()
		{

		}
	}
}
