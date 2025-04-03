using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Blue.Cosacs
{
    public partial class DN_LineItemAuditUpdateSP : Blue.Cosacs.Procedure
    {
        public DN_LineItemAuditUpdateSP(SqlConnection connection = null, SqlTransaction transaction = null)
            : base("DN_LineItemAuditUpdateSP", connection, transaction)
        {

            cmd.Parameters.Add(new SqlParameter("@acctno", SqlDbType.VarChar, 12)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@agrmtno", SqlDbType.Int, 4)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@empeenochange", SqlDbType.Int, 4)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@itemID", SqlDbType.Int, 4)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@stocklocn", SqlDbType.SmallInt, 2)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@quantitybefore", SqlDbType.Float, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@quantityafter", SqlDbType.Float, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@valuebefore", SqlDbType.Money, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@valueafter", SqlDbType.Money, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@taxamtbefore", SqlDbType.Float, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@taxamtafter", SqlDbType.Float, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@datechange", SqlDbType.DateTime, 8)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@contractno", SqlDbType.VarChar, 10)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@source", SqlDbType.VarChar, 15)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@parentitemID", SqlDbType.Int, 4)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@parentStockLocn", SqlDbType.SmallInt, 2)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@delnotebranch", SqlDbType.SmallInt, 2)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@salesBrnNo", SqlDbType.SmallInt, 2)).Direction = ParameterDirection.Input;

            cmd.Parameters.Add(new SqlParameter("@return", SqlDbType.Int, 4)).Direction = ParameterDirection.Output;

        }


        public string acctno
        {
            get
            {
                var v = cmd.Parameters[0].Value;
                if (v == DBNull.Value)
                    return null;
                return (string)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[0].Value = DBNull.Value;
                else
                    cmd.Parameters[0].Value = value;
            }
        }

        public int? agrmtno
        {
            get
            {
                var v = cmd.Parameters[1].Value;
                if (v == DBNull.Value)
                    return null;
                return (int?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[1].Value = DBNull.Value;
                else
                    cmd.Parameters[1].Value = value;
            }
        }

        public int? empeenochange
        {
            get
            {
                var v = cmd.Parameters[2].Value;
                if (v == DBNull.Value)
                    return null;
                return (int?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[2].Value = DBNull.Value;
                else
                    cmd.Parameters[2].Value = value;
            }
        }

        public int? itemID
        {
            get
            {
                var v = cmd.Parameters[3].Value;
                if (v == DBNull.Value)
                    return null;
                return (int?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[3].Value = DBNull.Value;
                else
                    cmd.Parameters[3].Value = value;
            }
        }

        public short? stocklocn
        {
            get
            {
                var v = cmd.Parameters[4].Value;
                if (v == DBNull.Value)
                    return null;
                return (short?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[4].Value = DBNull.Value;
                else
                    cmd.Parameters[4].Value = value;
            }
        }

        public double? quantitybefore
        {
            get
            {
                var v = cmd.Parameters[5].Value;
                if (v == DBNull.Value)
                    return null;
                return (double?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[5].Value = DBNull.Value;
                else
                    cmd.Parameters[5].Value = value;
            }
        }

        public double? quantityafter
        {
            get
            {
                var v = cmd.Parameters[6].Value;
                if (v == DBNull.Value)
                    return null;
                return (double?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[6].Value = DBNull.Value;
                else
                    cmd.Parameters[6].Value = value;
            }
        }

        public decimal? valuebefore
        {
            get
            {
                var v = cmd.Parameters[7].Value;
                if (v == DBNull.Value)
                    return null;
                return (decimal?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[7].Value = DBNull.Value;
                else
                    cmd.Parameters[7].Value = value;
            }
        }

        public decimal? valueafter
        {
            get
            {
                var v = cmd.Parameters[8].Value;
                if (v == DBNull.Value)
                    return null;
                return (decimal?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[8].Value = DBNull.Value;
                else
                    cmd.Parameters[8].Value = value;
            }
        }

        public double? taxamtbefore
        {
            get
            {
                var v = cmd.Parameters[9].Value;
                if (v == DBNull.Value)
                    return null;
                return (double?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[9].Value = DBNull.Value;
                else
                    cmd.Parameters[9].Value = value;
            }
        }

        public double? taxamtafter
        {
            get
            {
                var v = cmd.Parameters[10].Value;
                if (v == DBNull.Value)
                    return null;
                return (double?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[10].Value = DBNull.Value;
                else
                    cmd.Parameters[10].Value = value;
            }
        }

        public DateTime? datechange
        {
            get
            {
                var v = cmd.Parameters[11].Value;
                if (v == DBNull.Value)
                    return null;
                return (DateTime?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[11].Value = DBNull.Value;
                else
                    cmd.Parameters[11].Value = value;
            }
        }

        public string contractno
        {
            get
            {
                var v = cmd.Parameters[12].Value;
                if (v == DBNull.Value)
                    return null;
                return (string)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[12].Value = DBNull.Value;
                else
                    cmd.Parameters[12].Value = value;
            }
        }

        public string source
        {
            get
            {
                var v = cmd.Parameters[13].Value;
                if (v == DBNull.Value)
                    return null;
                return (string)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[13].Value = DBNull.Value;
                else
                    cmd.Parameters[13].Value = value;
            }
        }

        public int? parentitemID
        {
            get
            {
                var v = cmd.Parameters[14].Value;
                if (v == DBNull.Value)
                    return null;
                return (int?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[14].Value = DBNull.Value;
                else
                    cmd.Parameters[14].Value = value;
            }
        }

        public short? parentStockLocn
        {
            get
            {
                var v = cmd.Parameters[15].Value;
                if (v == DBNull.Value)
                    return null;
                return (short?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[15].Value = DBNull.Value;
                else
                    cmd.Parameters[15].Value = value;
            }
        }

        public short? delnotebranch
        {
            get
            {
                var v = cmd.Parameters[16].Value;
                if (v == DBNull.Value)
                    return null;
                return (short?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[16].Value = DBNull.Value;
                else
                    cmd.Parameters[16].Value = value;
            }
        }

        public short? salesBrnNo
        {
            get
            {
                var v = cmd.Parameters[17].Value;
                if (v == DBNull.Value)
                    return null;
                return (short?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[17].Value = DBNull.Value;
                else
                    cmd.Parameters[17].Value = value;
            }
        }

        public int? Return
        {
            get
            {
                var v = cmd.Parameters[18].Value;
                if (v == DBNull.Value)
                    return null;
                return (int?)v;
            }
            set
            {
                if (value == null)
                    cmd.Parameters[18].Value = DBNull.Value;
                else
                    cmd.Parameters[18].Value = value;
            }
        }

    }


    partial class DN_LineItemAuditUpdateSP
    {
        public int ExecuteNonQuery()
        {
            return ((cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) ? cmd.ExecuteNonQuery() : Context.Database().ExecuteNonQuery(cmd));
        }


        public object ExecuteNonQuery(string _acctno, int? _agrmtno, int? _empeenochange, int? _itemID, short? _stocklocn, double? _quantitybefore, double? _quantityafter, decimal? _valuebefore, decimal? _valueafter, double? _taxamtbefore, double? _taxamtafter, DateTime? _datechange, string _contractno, string _source, int? _parentitemID, short? _parentStockLocn, short? _delnotebranch, short? _salesBrnNo, out int? _Return)
        {
            this.acctno = _acctno;
            this.agrmtno = _agrmtno;
            this.empeenochange = _empeenochange;
            this.itemID = _itemID;
            this.stocklocn = _stocklocn;
            this.quantitybefore = _quantitybefore;
            this.quantityafter = _quantityafter;
            this.valuebefore = _valuebefore;
            this.valueafter = _valueafter;
            this.taxamtbefore = _taxamtbefore;
            this.taxamtafter = _taxamtafter;
            this.datechange = _datechange;
            this.contractno = _contractno;
            this.source = _source;
            this.parentitemID = _parentitemID;
            this.parentStockLocn = _parentStockLocn;
            this.delnotebranch = _delnotebranch;
            this.salesBrnNo = _salesBrnNo;

            var __result = ExecuteNonQuery();
            _Return = this.Return;

            return __result;
        }

    }

}