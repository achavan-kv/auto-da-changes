using System;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;



namespace STL.DAL
{
	/// <summary>
	/// Transport Load Schedule for deliveries
	/// </summary>
	public class DDeliveryLoad:DALObject
	{
          //IP - 21/02/09 - CR929 & 974
      private DataTable _dnItemsLinkedNonStocks;
      public DataTable DNItemsLinkedNonStocks
      {
          get { return _dnItemsLinkedNonStocks; }
      }

		public DDeliveryLoad()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public int DeleteByBuffNo (SqlConnection conn, SqlTransaction trans, short stockLocn, int BuffNo)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@piStockLocn", SqlDbType.Int);
				parmArray[0].Value = stockLocn;
				parmArray[1] = new SqlParameter("@piBuffNo", SqlDbType.Int);
				parmArray[1].Value = BuffNo;

				result = this.RunSP(conn, trans, "DN_DeliveryLoad_DeleteByBuffNo", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        public DataSet LoadByLoadNo(short BranchNo, int LoadNo, DateTime DateDel, out DateTime MinDelDate)
		{
			DataSet deliveryNoteSet = new DataSet();
			MinDelDate = Date.blankDate;

			try
			{
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@piBranchNo", SqlDbType.Int);
				parmArray[0].Value = BranchNo;
				parmArray[1] = new SqlParameter("@piLoadNo", SqlDbType.Int);
				parmArray[1].Value = LoadNo;
				parmArray[2] = new SqlParameter("@piDateDel", SqlDbType.DateTime);
				parmArray[2].Value = DateDel;

				result = this.RunSP("DN_DeliveryLoadByLoadNoSP", parmArray, deliveryNoteSet);

				if (deliveryNoteSet != null)
				{
					if (deliveryNoteSet.Tables.Count > 0)
						deliveryNoteSet.Tables[0].TableName = TN.Deliveries;
					if (deliveryNoteSet.Tables.Count > 1)
						deliveryNoteSet.Tables[1].TableName = TN.Schedules;
					if (deliveryNoteSet.Tables.Count > 2)
						deliveryNoteSet.Tables[2].TableName = TN.LineItem;
					if (deliveryNoteSet.Tables.Count > 3)
						if(!Convert.IsDBNull(deliveryNoteSet.Tables[3].Rows[0][CN.MinDelDate]))
							MinDelDate = Convert.ToDateTime(deliveryNoteSet.Tables[3].Rows[0][CN.MinDelDate]);
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return deliveryNoteSet;
		}

		public int ChangeReqDeliveryDate (SqlConnection conn, SqlTransaction trans,
			                                string acctNo, int agrmtNo, int itemId, 
                                            string contractNo, short stockLocn, 
                                            DateTime reqDeliveryDate, int buffNo, 
                                            string reason)
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@piAcctNo", SqlDbType.Char,12);
				parmArray[0].Value = acctNo;
				parmArray[1] = new SqlParameter("@piAgrmtNo", SqlDbType.Int);
				parmArray[1].Value = agrmtNo;
				parmArray[2] = new SqlParameter("@piItemId", SqlDbType.Int);
				parmArray[2].Value = itemId;
				parmArray[3] = new SqlParameter("@piStockLocn", SqlDbType.Int);
				parmArray[3].Value = stockLocn;
				parmArray[4] = new SqlParameter("@piContractNo", SqlDbType.VarChar,10);
				parmArray[4].Value = contractNo;
				parmArray[5] = new SqlParameter("@piReqDelDate", SqlDbType.DateTime);
				parmArray[5].Value = reqDeliveryDate;
                parmArray[6] = new SqlParameter("@piBuffNo", SqlDbType.Int);
                parmArray[6].Value = buffNo;
                parmArray[7] = new SqlParameter("@piReason", SqlDbType.NVarChar,80);
                parmArray[7].Value = reason;
                parmArray[8] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
                parmArray[8].Value = this.User;

                
				result = this.RunSP(conn, trans, "DN_ChangeReqDelDateSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int RemoveDeliveryNote (SqlConnection conn, SqlTransaction trans,
			short stockLocn, int buffNo, int empeeNo, string reason)
		{
			try
			{
				parmArray = new SqlParameter[4];
				parmArray[0] = new SqlParameter("@piStockLocn", SqlDbType.Int);
				parmArray[0].Value = stockLocn;
				parmArray[1] = new SqlParameter("@piBuffNo", SqlDbType.Int);
				parmArray[1].Value = buffNo;
				parmArray[2] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
				parmArray[2].Value = empeeNo;
				parmArray[3] = new SqlParameter("@piReason", SqlDbType.VarChar,80);
				parmArray[3].Value = reason;

				result = this.RunSP(conn, trans, "DN_RemoveDeliveryNoteSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int RemoveDeliveryNoteItem (SqlConnection conn, SqlTransaction trans,
			short stockLocn, int buffNo, string acctNo, int agrmtNo,
			int itemId, int empeeNo, string reason,bool moreThanOne)
		{
			try
			{
				parmArray = new SqlParameter[8];
				parmArray[0] = new SqlParameter("@piStockLocn", SqlDbType.Int);
				parmArray[0].Value = stockLocn;
				parmArray[1] = new SqlParameter("@piBuffNo", SqlDbType.Int);
				parmArray[1].Value = buffNo;
				parmArray[2] = new SqlParameter("@piAcctNo", SqlDbType.Char,12);
				parmArray[2].Value = acctNo;
				parmArray[3] = new SqlParameter("@piAgrmtNo", SqlDbType.Int);
				parmArray[3].Value = agrmtNo;
                parmArray[4] = new SqlParameter("@piItemId", SqlDbType.Int);
				parmArray[4].Value = itemId;
				parmArray[5] = new SqlParameter("@piEmpeeNo", SqlDbType.Int);
				parmArray[5].Value = empeeNo;
				parmArray[6] = new SqlParameter("@piReason", SqlDbType.VarChar,80);
				parmArray[6].Value = reason;
                parmArray[7] = new SqlParameter("@multiple", SqlDbType.Bit);
                parmArray[7].Value = moreThanOne;

				result = this.RunSP(conn, trans, "DN_RemoveDeliveryNoteItemSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

        //IP - 18/02/09 - CR929 & 974
        public int DeleteDeliveryNoteAndItems(SqlConnection conn, SqlTransaction trans,
            short stockLocn, int buffNo, int empeeNo, string reason)
        {
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@stockLocn", SqlDbType.Int);
                parmArray[0].Value = stockLocn;
                parmArray[1] = new SqlParameter("@buffNo", SqlDbType.Int);
                parmArray[1].Value = buffNo;
                parmArray[2] = new SqlParameter("@empeeNo", SqlDbType.Int);
                parmArray[2].Value = empeeNo;
                parmArray[3] = new SqlParameter("@reason", SqlDbType.VarChar, 80);
                parmArray[3].Value = reason;

                result = this.RunSP(conn, trans, "DeleteDeliveryNoteSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        
        //IP - 21/02/09 - CR929 & 974 - Method to retrieve Non-Stocks linked to 
        //items on a DN
        public void GetNonStockLinkedToDNItems(SqlConnection conn, SqlTransaction trans,
                                                          short stockLocn, int buffNo)
        {
            _dnItemsLinkedNonStocks = new DataTable("NonStockLinkedToDNItem");

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@stockLocn", SqlDbType.Int);
                parmArray[0].Value = stockLocn;
                parmArray[1] = new SqlParameter("@buffNo", SqlDbType.Int);
                parmArray[1].Value = buffNo;

                RunSP("GetNonStockLinkedToDNItemsSP", parmArray, _dnItemsLinkedNonStocks);

            }
            catch (SqlException ex)
            {

                throw ex;
            }

        }

        //IP - 18/11/09 - CR929 & 974 - Audit
        public DataTable GetAuditData(string accountNo, int rowcount)
        {
            DataTable dt = new DataTable(TN.DeliveryNotificationAudit);

            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = accountNo;
                parmArray[1] = new SqlParameter("@rowcount", SqlDbType.Int);
                parmArray[1].Value = rowcount;

                this.RunSP("DelNotificationAuditGetSP", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }

	}
}
