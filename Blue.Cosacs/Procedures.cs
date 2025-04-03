using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue.Cosacs.Shared;
using System.Data.SqlClient;

namespace Blue.Cosacs
{
    partial class InsertCustAddress
    {
        public static void Execute(CustAddress custaddress, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            new InsertCustAddress(connection, transaction).ExecuteNonQuery(
                custaddress.origbr,
                custaddress.custid,
                custaddress.addtype,
                custaddress.datein,
                custaddress.cusaddr1,
                custaddress.cusaddr2,
                custaddress.cusaddr3,
                custaddress.cuspocode,
                custaddress.custlocn,
                custaddress.resstatus,
                custaddress.mthlyrent,
                custaddress.datemoved,
                custaddress.hasstring,
                custaddress.Email,
                custaddress.PropType,
                custaddress.empeenochange,
                custaddress.datechange,
                custaddress.Notes,
                custaddress.deliveryarea,
                custaddress.zone);
        }
    }
}
