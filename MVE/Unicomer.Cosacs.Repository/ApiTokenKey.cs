using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Repository
{
    #region ApiTokenKey
    public partial class ReadWriteToken : Blue.Transactions.Command<ContextBase>
    {
        public ReadWriteToken() : base("dbo.VE_SecretTokenDetails")
        {
            base.AddInParameter("@Token", DbType.String);
            base.AddInParameter("@Expiration", DbType.DateTime);
        }
    }

    partial class ReadWriteToken
    {
        public void ReadWriteTokenDeatils(DataSet dt, DateTime expiration, string token = "")
        {
            if (!string.IsNullOrWhiteSpace(token))
                this.Token = token;
            if (!expiration.Equals(DateTime.MinValue))
                this.Expiration = expiration;
            base.Fill(dt);
        }

        #region "properties"
        public string Token
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public DateTime Expiration
        {
            get { return (DateTime)base[1]; }
            set { base[1] = value; }
        }
        #endregion
    }
    #endregion

}
