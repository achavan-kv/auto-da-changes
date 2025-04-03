using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DPaymentFileDefn.
	/// </summary>
	public class DPaymentFileDefn : DALObject
	{
		private DataTable _table;

		public DPaymentFileDefn()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int GetDefinition()
		{
			try
			{			
				_table = new DataTable(TN.StorderControl);
				result = this.RunSP("DN_PaymentFileDefnSP", _table);
				if(result == 0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
			return result;
		}

		public DataTable Table
		{
			get 
			{
				return _table;
			}
		}

		public void SavePaymentDefinition(
			SqlConnection conn, 
			SqlTransaction trans,
			string bankName,
			string fileExt,                 //IP - 20/08/10 - CR1092 - COASTER to COSACS Enhancements - changed from fileName to fileExt
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
            string delimitedMoneyColNo,      //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements   
            bool isInterest)                //IP - 03/09/10 - CR1112 - Tallyman Interest Charges
     
		{
			try
			{
				// to do parameters
				parmArray = new SqlParameter[38];
				parmArray[0] = new SqlParameter("@bankname", SqlDbType.VarChar,16);
				parmArray[0].Value = bankName;
				parmArray[1] = new SqlParameter("@fileExt", SqlDbType.VarChar,32);  //IP - 20/08/10 - CR1092 - Changed to hold file extension rather than filename
				parmArray[1].Value = fileExt;
				parmArray[2] = new SqlParameter("@acctnobegin", SqlDbType.SmallInt);
				parmArray[2].Value = acctnoBegin;
				parmArray[3] = new SqlParameter("@acctnolength", SqlDbType.SmallInt);
				parmArray[3].Value = acctnoLength;
				parmArray[4] = new SqlParameter("@moneybegin", SqlDbType.SmallInt);
				parmArray[4].Value = moneyBegin;
				parmArray[5] = new SqlParameter("@moneylength", SqlDbType.SmallInt);
				parmArray[5].Value = moneyLength;
				parmArray[6] = new SqlParameter("@moneypoint", SqlDbType.SmallInt);
				parmArray[6].Value = moneyPoint;
				parmArray[7] = new SqlParameter("@headline", SqlDbType.SmallInt);
				parmArray[7].Value = headLine;
				parmArray[8] = new SqlParameter("@datebegin", SqlDbType.SmallInt);
				parmArray[8].Value = dateBegin;
				parmArray[9] = new SqlParameter("@datelength", SqlDbType.SmallInt);
				parmArray[9].Value = dateLength;
				parmArray[10] = new SqlParameter("@dateformat", SqlDbType.VarChar,16);
				parmArray[10].Value = dateFormat;
				parmArray[11] = new SqlParameter("@trailerbegin", SqlDbType.SmallInt);
				parmArray[11].Value = trailerBegin;
				parmArray[12] = new SqlParameter("@trailerlength", SqlDbType.SmallInt);
				parmArray[12].Value = trailerLength;
				parmArray[13] = new SqlParameter("@paymentmethod", SqlDbType.SmallInt);
				parmArray[13].Value = paymentMethod;
				parmArray[14] = new SqlParameter("@hastrailer", SqlDbType.VarChar,1);
				parmArray[14].Value = hasTrailer;
                parmArray[15] = new SqlParameter("@headerIdBegin", SqlDbType.SmallInt);                 //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[15].Value = headerIdBegin;
                parmArray[16] = new SqlParameter("@headerIdLength", SqlDbType.SmallInt);                //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[16].Value = headerIdLength;
                parmArray[17] = new SqlParameter("@headerId", SqlDbType.VarChar, 20);                   //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[17].Value = headerId;
                parmArray[18] = new SqlParameter("@trailerIdBegin", SqlDbType.SmallInt);                //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[18].Value = trailerIdBegin;
                parmArray[19] = new SqlParameter("@trailerIdLength", SqlDbType.SmallInt);               //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[19].Value = trailerIdLength;
                parmArray[20] = new SqlParameter("@trailerId", SqlDbType.VarChar, 20);                  //IP - 12/08/10 - CR1092 - COASTER to CoSACS Enhancements
                parmArray[20].Value = trailerId;
                parmArray[21] = new SqlParameter("@isBatch", SqlDbType.Bit);                            //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements    
                parmArray[21].Value = isBatch;
                parmArray[22] = new SqlParameter("@batchHeaderIdBegin", SqlDbType.SmallInt);            //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[22].Value = batchHeaderIdBegin;
                parmArray[23] = new SqlParameter("@batchHeaderIdLength", SqlDbType.SmallInt);           //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[23].Value = batchHeaderIdLength;
                parmArray[24] = new SqlParameter("@batchHeaderId", SqlDbType.VarChar,20);               //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[24].Value = batchHeaderId;
                parmArray[25] = new SqlParameter("@batchHeaderHasTotal", SqlDbType.Bit);                //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[25].Value = batchHeaderHasTotal;
                parmArray[26] = new SqlParameter("@batchHeaderMoneyBegin", SqlDbType.SmallInt);         //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[26].Value = batchHeaderMoneyBegin;
                parmArray[27] = new SqlParameter("@batchHeaderMoneyLength", SqlDbType.SmallInt);        //IP - 19/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[27].Value = batchHeaderMoneyLength;
                parmArray[28] = new SqlParameter("@batchTrailerIdBegin", SqlDbType.SmallInt);           //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[28].Value = batchTrailerIdBegin;
                parmArray[29] = new SqlParameter("@batchTrailerIdLength", SqlDbType.SmallInt);          //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[29].Value = batchTrailerIdLength;
                parmArray[30] = new SqlParameter("@batchTrailerId", SqlDbType.VarChar,20);              //IP - 03/09/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[30].Value = batchTrailerId;
                parmArray[31] = new SqlParameter("@isDelimited", SqlDbType.Bit);                        //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[31].Value = isDelimited;
                parmArray[32] = new SqlParameter("@delimiter", SqlDbType.VarChar, 15);                  //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[32].Value = delimiter;
                parmArray[33] = new SqlParameter("@delimitedNoOfCols", SqlDbType.SmallInt);             //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[33].Value = delimitedNoOfCols;
                parmArray[34] = new SqlParameter("@delimitedAcctNoColNo", SqlDbType.VarChar, 10);       //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[34].Value = delimitedAcctNoColNo;
                parmArray[35] = new SqlParameter("@delimitedDateColNo", SqlDbType.VarChar, 10);         //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[35].Value = delimitedDateColNo;
                parmArray[36] = new SqlParameter("@delimitedMoneyColNo", SqlDbType.VarChar, 10);        //IP - 24/08/10 - CR1092 - COASTER to CoSACS Enhancements 
                parmArray[36].Value = delimitedMoneyColNo;
                parmArray[37] = new SqlParameter("@isInterest", SqlDbType.Bit);                         //IP - 03/09/10 - CR1112 - Tallyman Interest Charges            
                parmArray[37].Value = isInterest;



				this.RunSP(conn, trans, "DN_PaymentFileDefnUpdateSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

		public void DeletePaymentDefinition(
			SqlConnection conn, 
			SqlTransaction trans,
			string bankName)
		{
			try
			{
				// to do parameters
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@bankname", SqlDbType.VarChar,16);
				parmArray[0].Value = bankName;
										
				this.RunSP(conn, trans, "DN_PaymentFileDefnDeleteSP", parmArray);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}			
		}

        //IP - 16/09/10 - CR1092 
        public int GetDefinitionDelimiters()
        {
            try
            {
                _table = new DataTable(TN.StorderDelimiters);
                result = this.RunSP("GetPaymentFileDelimitersSP", _table);
                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
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
