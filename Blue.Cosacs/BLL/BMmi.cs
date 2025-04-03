using System;
using STL.Common;
using System.Data;
using STL.DAL;
using System.Xml;
using STL.Common.Constants.ColumnNames;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.Tags;

namespace STL.BLL
{
    public class BMmi : CommonObject
    {
        public DataSet GetMmiMatrix()
        {
            DataSet ds = new DataSet();
            DMmi mmi = new DMmi();
            mmi.GetMmiMatrix();
            ds.Tables.Add(mmi.Matrix);
            return ds;
        }


        public void SaveMmiMatrix(SqlConnection conn, SqlTransaction trans, DataSet matrix)
        {
            DMmi mmi = new DMmi();
            DateTime configuredDate = DateTime.Now;

            mmi.DeleteMmiMatrix(conn, trans);
            matrix.AcceptChanges();

            foreach (DataRow r in matrix.Tables[TN.MmiMatrix].Rows)
            {
                string label = Convert.ToString(r[CN.Label]);
                int fromScore = Convert.ToInt32(r[CN.FromScore]);
                int toScore = Convert.ToInt32(r[CN.ToScore]);
                decimal mmiPercentage = Convert.ToDecimal(r[CN.MmiPercentage]);

                mmi.SaveMmiMatrixRow(conn, trans, label, fromScore, toScore, mmiPercentage, configuredDate, User);
            }
        }

        public void SaveCustomerMmi(SqlConnection conn, SqlTransaction trans, string custId, int userId, string reasonChanged)
        {
            DMmi mmi = new DMmi();
            mmi.SaveCustomerMmi(conn, trans, custId, userId, reasonChanged);
        }

        public void GetMmiThresholdForSale(SqlConnection conn, SqlTransaction trans, string custId, string acctNo, string termType, out bool isMmiAllowed, out decimal mmiLimit, out decimal mmiThreshold)
        {
            DMmi mmi = new DMmi();
            mmi.User = User;
            isMmiAllowed = false;
            mmiLimit = 0;
            mmiThreshold = 0;
            mmi.GetMmiThresholdForSale(conn, trans, custId, acctNo, termType, out isMmiAllowed, out mmiLimit, out mmiThreshold);
        }
    }
}
