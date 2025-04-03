using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.Enums;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BPaymentFileDefn.
	/// </summary>
	public class BPaymentFileDefn : CommonObject
	{
	/// Get Definitions.
		public DataSet GetDefinition()
		{
			DataSet ds = new DataSet(); 
			DPaymentFileDefn pfDefn = new DPaymentFileDefn();
			pfDefn.GetDefinition();
			ds.Tables.Add(pfDefn.Table);

            pfDefn.GetDefinitionDelimiters();   //IP - 16/09/10 - CR1092 - COASTER to CoSACS Enhancements
            ds.Tables.Add(pfDefn.Table);
			return ds;
		}
	/// Save Definitions.
		public void SavePaymentDefinition(
			SqlConnection conn, 
			SqlTransaction trans,
			string bankName,
			string fileExt,                 //IP - 20/08/10 - CR1092 - COASTER to CoSACS Enhancements - Changed from fileName to fileExt
			int acctnoBegin,
			int acctnoLength,
			int moneyBegin,
			int moneyLength,
			int moneyPoint,
			int headLine,
			int dateBegin,
			int dateLength,
			string dateFormat,
			int trailerBegin,
			int trailerLength,
			int paymentMethod,
			int hasTrailer,
            int headerIdBegin,              //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int headerIdLength,             //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string headerId,                //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int trailerIdBegin,             //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int trailerIdLength,            //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string trailerId,               //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
            bool isBatch,                   //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int batchHeaderIdBegin,         //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int batchHeaderIdLength,        //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string batchHeaderId,           //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
            bool batchHeaderHasTotal,       //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int batchHeaderMoneyBegin,      //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int batchHeaderMoneyLength,     //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
            int batchTrailerIdBegin,        //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements 
            int batchTrailerIdLength,       //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
            string batchTrailerId,          //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
            bool isDelimited,               //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string delimiter,               //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            int delimitedNoOfCols,          //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string delimitedAcctNoColNo,    //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string delimitedDateColNo,      //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements
            string delimitedMoneyColNo,     //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
            bool isInterest)                //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
		{
			DPaymentFileDefn pd =  new DPaymentFileDefn();
            pd.SavePaymentDefinition(
                conn,
                trans,
                bankName,
                fileExt,                    //IP - 20/08/10 - CR1092 - Changed from filename to fileExt
                acctnoBegin,
                acctnoLength,
                moneyBegin,
                moneyLength,
                moneyPoint,
                headLine,
                dateBegin,
                dateLength,
                dateFormat,
                trailerBegin,
                trailerLength,
                paymentMethod,
                hasTrailer,
                headerIdBegin,              //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                headerIdLength,             //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                headerId,                   //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                trailerIdBegin,             //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                trailerIdLength,            //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                trailerId,                  //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                isBatch,                    //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchHeaderIdBegin,         //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchHeaderIdLength,        //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchHeaderId,              //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchHeaderHasTotal,        //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchHeaderMoneyBegin,      //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchHeaderMoneyLength,     //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements
                batchTrailerIdBegin,        //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
                batchTrailerIdLength,       //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
                batchTrailerId,             //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements
                isDelimited,                //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                delimiter,                  //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                delimitedNoOfCols,          //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                delimitedAcctNoColNo,       //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                delimitedDateColNo,         //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                delimitedMoneyColNo,        //IP - 25/08/10 - CR1092 - COASTER to CoSACS Enhancements     
                isInterest);                //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
		}
	/// Delete Definitions.
		public void DeletePaymentDefinition(
			SqlConnection conn, 
			SqlTransaction trans,
			string bankName)
		{
			DPaymentFileDefn pd =  new DPaymentFileDefn();
			pd.DeletePaymentDefinition(
				conn, 
				trans, 
				bankName);
		}
	/// Get Payment Methods.
		public DataSet GetPayMethod(string category, string statusFlag, string tableName)
		{
			DataSet pm = new DataSet(); 
			DCode pmCode = new DCode();
			pmCode.GetCategoryCodes(category, statusFlag, tableName);
			pm.Tables.Add(pmCode.Codes);
			return pm;
		}
        		
		public BPaymentFileDefn()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
