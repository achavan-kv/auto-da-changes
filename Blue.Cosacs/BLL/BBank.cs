using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Configuration;

namespace STL.BLL
{
    public class BBank : CommonObject
    {
        public BBank()
        {
        }

        public DataSet GetBankDetails()
        {
            DataSet bankSet = new DataSet();
            DBank bank = new DBank();

            bank.GetBankDetails();
            bankSet.Tables.Add(bank.Table);

            return bankSet;
        }

        //Method to update Bank details
        public int UpdateBank(SqlConnection conn, SqlTransaction trans, string bankcode, string bankname,
            string bankaddr1, string bankaddr2, string bankaddr3, string bankpocode)
        {
            int status = 0;
            DBank bank = new DBank();
            status = bank.UpdateBank(conn, trans, bankcode, bankname, bankaddr1, bankaddr2, bankaddr3, bankpocode);

            return status;
        }

        public int DeleteBank(SqlConnection conn, SqlTransaction trans, string bankcode)
        {
            int status = 0;
            DBank bank = new DBank();
            status = bank.DeleteBank(conn, trans, bankcode);

            return status;
        }
    }
}
