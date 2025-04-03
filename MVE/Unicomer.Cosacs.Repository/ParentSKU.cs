using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Repository
{
    #region UpdateParentSKUMasterRepository 
    public partial class UpdateParentSKUMasterRepository : Blue.Transactions.Command<ContextBase>
    {
        public UpdateParentSKUMasterRepository() : base("dbo.VE_UpdateParentSKUMaster")
        {
            base.AddInParameter("@XmlString", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class UpdateParentSKUMasterRepository
    {
        public List<string> UpdateParentSKUMaster()
        {
            List<string> returnExternalProductID = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnExternalProductID.Add(this.Message);
            returnExternalProductID.Add(Convert.ToString(this.StatusCode));
            return returnExternalProductID; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> UpdateParentSKUMaster(string userJson)
        {
            this.UserJson = userJson;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return UpdateParentSKUMaster();
        }

        #region "properties"
        public string UserJson
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public int StatusCode
        {
            get { return (int)base[2]; }
            set { base[2] = value; }
        }
        #endregion

    }
    #endregion

    #region  ParentSKUEOD
    public partial class ParentSKUEOD : Blue.Transactions.Command<ContextBase>
    {
        public ParentSKUEOD() : base("dbo.VE_ParentSKURealTime")
        {
            base.AddInParameter("@spanInMinutes", DbType.Int32);
            base.AddInParameter("@ProductID", DbType.String);
        }
    }

    partial class ParentSKUEOD
    {
        public new void Fill(DataSet ds, int spanInMinutes, string ProductID)
        {
            this.SpanInMinutes = spanInMinutes;
            this.ProductID = ProductID;

            base.Fill(ds);
        }

        #region "properties"
        public int SpanInMinutes
        {
            get { return (int)base[0]; }
            set { base[0] = value; }
        }
        public string ProductID
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        #endregion
    }
    #endregion



}
