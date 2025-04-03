using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using STL.Common;

namespace STL.DAL
{
    public class DLoyalty : DALObject
    {

        public Loyalty LoyaltyGetByCustid(string custid)
        {
            var LoyaltyData = new Loyalty();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20) { Value = custid };

                using (var connection = new SqlConnection(connectionStr))
                {
                    connection.Open();
                    var command = CreateCommand("LoyaltyGetDetailsByCustid", parmArray, connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ReadData(reader, ref LoyaltyData);

                        if (reader["rejections"] != DBNull.Value)
                        {
                            LoyaltyData.rejections = Convert.ToInt32(reader["rejections"]);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            return LoyaltyData;
        }

        public Loyalty LoyaltyGetByAcctno(string acctno)
        {
            var LoyaltyData = new Loyalty();

            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12) { Value = acctno };

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand("LoyaltyGetDetailsByAcctno", parmArray, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ReadData(reader, ref LoyaltyData);

                    if (reader["rejections"] != DBNull.Value)
                    {
                        LoyaltyData.rejections = Convert.ToInt32(reader["rejections"]);
                    }

                }
            }
            return LoyaltyData;
        }




        public Loyalty LoyaltyGetByMemberno(string memberno)
        {
            var LoyaltyData = new Loyalty();

            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@memberno", SqlDbType.VarChar, 16)
                               {
                                   Direction = ParameterDirection.Input,
                                   Value = memberno
                               };

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand("LoyaltyGetDetailsByMemberno", parmArray, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ReadData(reader, ref LoyaltyData);
                }
            }

            return LoyaltyData;
        }

        public List<LoyaltyVoucher> GetLoyaltyVoucher(IDataParameter[] parameters)
        {
            LoyaltyVoucher LoyaltyData;

            var Vouchers = new List<LoyaltyVoucher>();

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand("LoyaltyGetVouchers", parameters, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    LoyaltyData = new LoyaltyVoucher();
                    if (reader["Custid"] != DBNull.Value)
                    {
                        LoyaltyData.custid = reader["Custid"].ToString();
                    }
                    if (reader["memberno"] != DBNull.Value)
                    {
                        LoyaltyData.memberno = reader["memberno"].ToString();
                    }
                    if (reader["AcctnoGen"] != DBNull.Value)
                    {
                        LoyaltyData.acctno = reader["AcctnoGen"].ToString();
                    }
                    if (reader["VoucherRef"] != DBNull.Value)
                    {
                        LoyaltyData.voucherref = Convert.ToInt32(reader["VoucherRef"]);
                    }
                    if (reader["VoucherValue"] != DBNull.Value)
                    {
                        LoyaltyData.vouchervalue = Convert.ToDecimal(reader["VoucherValue"]);
                    }
                    if (reader["VoucherDate"] != DBNull.Value)
                    {
                        LoyaltyData.voucherdate = Convert.ToDateTime(reader["VoucherDate"]);
                    }

                    Vouchers.Add(LoyaltyData);
                }
            }
            return Vouchers;
        }


        private void ReadData(SqlDataReader reader, ref Loyalty LoyaltyData)
        {
            if (reader["Custid"] != DBNull.Value)
            {
                LoyaltyData.custid = reader["Custid"].ToString();
            }
            if (reader["memberno"] != DBNull.Value)
            {
                LoyaltyData.memberno = reader["memberno"].ToString();
            }
            if (reader["StartDate"] != DBNull.Value)
            {
                LoyaltyData.startdate = Convert.ToDateTime(reader["StartDate"]);
            }
            if (reader["Enddate"] != DBNull.Value)
            {
                LoyaltyData.enddate = Convert.ToDateTime(reader["Enddate"]);
            }
            if (reader["MemberType"] != DBNull.Value)
            {
                LoyaltyData.membertype = Convert.ToChar(reader["MemberType"]);
            }
            if (reader["StatusAcct"] != DBNull.Value)
            {
                LoyaltyData.statusacct = Convert.ToInt32(reader["StatusAcct"]);
            }
            if (reader["StatusVoucher"] != DBNull.Value)
            {
                LoyaltyData.statusvoucher = Convert.ToInt32(reader["StatusVoucher"]);
            }
        }










        //public Loyalty LoyaltyGetData(string custid,string acctno)
        //{

        //    try
        //    {
        //        parmArray = new SqlParameter[2];
        //        parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
        //        parmArray[0].Value = custid;
        //        parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
        //        parmArray[1].Value = acctno;
        //        return GetLoyaltyData(parmArray);

        //    }
        //    catch (SqlException ex)
        //    {
        //        LogSqlException(ex);
        //        throw ex;
        //    }
        //}

        public DataSet GetLoyaltyDropData()
        {
            DataSet dropdata = new DataSet();
            try
            {
                //parmArray = new SqlParameter[1];
                //parmArray[0] = new SqlParameter("@return", SqlDbType.Int);
                //parmArray[0].Direction = ParameterDirection.Output;

                RunSP("LoyaltyGetDropData", dropdata);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dropdata;
        }

        public string LoyaltySave(Loyalty loyalty)
        {
            try
            {
                parmArray = new SqlParameter[9];
                parmArray[0] = new SqlParameter("@MemberNo", SqlDbType.VarChar, 16);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = loyalty.memberno;
                parmArray[1] = new SqlParameter("@Custid", SqlDbType.VarChar, 20);
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[1].Value = loyalty.custid;
                parmArray[2] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                parmArray[2].Direction = ParameterDirection.Input;
                parmArray[2].Value = loyalty.startdate;
                parmArray[3] = new SqlParameter("@Enddate", SqlDbType.DateTime);
                parmArray[3].Direction = ParameterDirection.Input;
                parmArray[3].Value = loyalty.enddate;
                parmArray[4] = new SqlParameter("@MemberType", SqlDbType.Char, 1);
                parmArray[4].Direction = ParameterDirection.Input;
                parmArray[4].Value = loyalty.membertype;
                parmArray[5] = new SqlParameter("@StatusAcct", SqlDbType.Int);
                parmArray[5].Direction = ParameterDirection.Input;
                parmArray[5].Value = loyalty.statusacct;
                parmArray[6] = new SqlParameter("@StatusVoucher", SqlDbType.Int);
                parmArray[6].Direction = ParameterDirection.Input;
                parmArray[6].Value = loyalty.statusvoucher;
                parmArray[7] = new SqlParameter("@cancel", SqlDbType.Char, 1);
                parmArray[7].Direction = ParameterDirection.Input;
                parmArray[7].Value = loyalty.cancel;
                parmArray[8] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[8].Direction = ParameterDirection.Input;
                parmArray[8].Value = Convert.ToInt32(loyalty.user);
                return ReturnString("LoyaltySave", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public void LoyaltyAddFee(string acctno, string membertype, string custid, int user)
        {

            try
            {
                parmArray = new SqlParameter[4];

                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@membertype", SqlDbType.Char, 1);
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[1].Value = membertype.Trim();
                parmArray[2] = new SqlParameter("@custid", SqlDbType.Char, 20);
                parmArray[2].Direction = ParameterDirection.Input;
                parmArray[2].Value = custid;
                parmArray[3] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[3].Direction = ParameterDirection.Input;
                parmArray[3].Value = user;

                RunSP("LoyaltyAddFee", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public string LoyaltyGetCashAccount(string custid)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = custid;

                return ReturnString("LoyaltyGetCashAccount", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public bool LoyaltyCheckCustomer(string custid)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = custid;

                return ReturnBool("LoyaltyCheckCustomer", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }


        }

        public List<LoyaltyVoucher> LoyaltyGetVouchers(string custid)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = custid;

                return GetLoyaltyVoucher(parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void LoyaltyAddRejection(string custid, string acctno)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = custid;

                parmArray[1] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[1].Value = acctno;
                RunNonQuery("LoyaltyAddRejection", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void LoyaltyPay(string acctno)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.Char, 12);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = acctno;
                RunNonQuery("LoyaltyPay", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void LoyaltyGetCharges(string custid, ref string acctno, ref decimal amount, ref bool active)
        {

            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = custid;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[1].Direction = ParameterDirection.Output;
                parmArray[2] = new SqlParameter("@amount", SqlDbType.Money);
                parmArray[2].Direction = ParameterDirection.Output;
                parmArray[3] = new SqlParameter("@active", SqlDbType.Bit);
                parmArray[3].Direction = ParameterDirection.Output;

                RunSP("LoyaltyGetCharges", parmArray); //uat(4.3) - 144

                if (!Convert.IsDBNull(parmArray[1].Value))
                    acctno = parmArray[1].Value.ToString();

                if (!Convert.IsDBNull(parmArray[2].Value))
                    amount = (decimal)parmArray[2].Value;

                if (!Convert.IsDBNull(parmArray[3].Value))
                    active = (bool)parmArray[3].Value;

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public void LoyaltySaveVouchers(int voucher, bool add, string acctno)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@voucher", SqlDbType.Int, 32);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = voucher;
                parmArray[1] = new SqlParameter("@add", SqlDbType.Bit);
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[1].Value = add;
                parmArray[2] = new SqlParameter("@AcctnoRedeem", SqlDbType.VarChar, 12);
                parmArray[2].Direction = ParameterDirection.Input;
                parmArray[2].Value = acctno;

                RunNonQuery("LoyaltySaveVouchers", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }



        public bool LoyaltyIsLinkAccount(string acctno, string custid)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@custid", SqlDbType.VarChar, 20);
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[1].Value = custid;

                return ReturnBool("LoyaltyIsLinkAccount", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }



        public List<LoyaltyValidAddresses> LoyaltyGetValidAddress(string custid)
        {
            var addtypes = new List<LoyaltyValidAddresses>();
            LoyaltyValidAddresses address;
            parmArray = new SqlParameter[1];
            parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 20)
                               {
                                   Direction = ParameterDirection.Input,
                                   Value = custid
                               };

            using (var connection = new SqlConnection(connectionStr))
            {
                connection.Open();
                var command = CreateCommand("LoyaltyGetValidAddress", parmArray, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (!Convert.ToBoolean(reader["valid"])) continue;
                    address = new LoyaltyValidAddresses { addtype = reader["addtype"].ToString().Trim() };
                    addtypes.Add(address);
                }
            }

            return addtypes;
        }
        public void LoyaltyCancelAccount(string acctno, int user)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Direction = ParameterDirection.Input;
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[1].Direction = ParameterDirection.Input;
                parmArray[1].Value = user;


                RunNonQuery("LoyaltyCancelAccount", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public bool LoyaltyCheckAccountPeriod(string acctno)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;

                return ReturnBool("LoyaltyCheckAccountPeriod", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

        }

        public void LoyaltyCheckRemoveFreeDel(string acctno, int user)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[1].Value = user;

                RunNonQuery("LoyaltyGRTFreeDelAdjust", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void LoyaltyGRTCancelFreeDelAdjust(string acctno, int user)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acctno;
                parmArray[1] = new SqlParameter("@user", SqlDbType.Int);
                parmArray[1].Value = user;

                RunNonQuery("LoyaltyGRTCancelFreeDelAdjust", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


        public DataTable LoyaltyGetHistory(string custid)
        {
            DataTable DTLoyalty = new DataTable();
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar, 12);
                parmArray[0].Value = custid;
                RunSP("LoyaltyGetHistory", parmArray, DTLoyalty);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }

            return DTLoyalty;
        }

        public bool LoyaltyCheckLinkedAccount(string acct)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acct;
                return ReturnBool("LoyaltyCheckLinkedAccount", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public decimal? LoyaltyGetVoucherValue(string acct)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acct;
                return ReturnDecimal("LoyaltyGetVoucherValue", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void LoyaltyRemoveVoucher(string acct)
        {
            try
            {
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.VarChar, 12);
                parmArray[0].Value = acct;
                RunNonQuery("LoyaltyRemoveVoucher", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }
    }
}
