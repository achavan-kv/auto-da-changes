using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Unicomer.Cosacs.Repository
{
    #region  SyncData    
    public partial class SyncData : Blue.Transactions.Command<ContextBase>
    {
        public SyncData() : base("dbo.VE_GetSyncData")
        {
            //base.AddInParameter("@GRNNo", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class SyncData
    {
        public new void Fill(DataSet ds)
        {
            //this.GRNNo = GRNNo;
            this.Message = this.Message;
            this.Status = this.Status;
            base.Fill(ds);
        }
        #region "properties"
        //public string GRNNo
        //{
        //    get { return (string)base[0]; }
        //    set { base[0] = value; }
        //}
        public string Message
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Status
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }

        #endregion
    }
    #endregion

}
