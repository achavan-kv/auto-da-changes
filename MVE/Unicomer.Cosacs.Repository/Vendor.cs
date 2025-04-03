using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Repository
{

    #region VendorRepository 
    public partial class UpdateVendorRepository : Blue.Transactions.Command<ContextBase>
    {
        public UpdateVendorRepository() : base("dbo.VE_UpdateSupplierMaster")
        {
            base.AddInParameter("@XmlString", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);

        }
    }

    partial class UpdateVendorRepository
    {
        public List<string> UpdateVendorMaster()
        {
            List<string> returnExternalVendorID = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnExternalVendorID.Add(this.Message);
            returnExternalVendorID.Add(Convert.ToString(this.StatusCode));
            return returnExternalVendorID; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> UpdateVendorMaster(string userJson)
        {
            this.UserJson = userJson;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return UpdateVendorMaster();
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


    #region SupplierEodRepository 
    public partial class SupplierEodRepository : Blue.Transactions.Command<ContextBase>
    {
        public SupplierEodRepository() : base("dbo.VE_SupplierEOD")
        {
            base.AddInParameter("@spanInMinutes", DbType.Int32);
        }
    }

    partial class SupplierEodRepository
    {
        public new void Fill(DataSet ds, int spanInMinutes)
        {
            this.SpanInMinutes = spanInMinutes;
            base.Fill(ds);
        }

        #region "properties"
        public int SpanInMinutes
        {
            get { return (int)base[0]; }
            set { base[0] = value; }
        }
        #endregion

    }
    #endregion


    #region SupplierRTSRepository 
    public partial class SupplierRTSRepository : Blue.Transactions.Command<ContextBase>
    {
        public SupplierRTSRepository() : base("dbo.VE_SupplierEOD")
        {
            base.AddInParameter("@VendorCode", DbType.String);
        }
    }

    partial class SupplierRTSRepository
    {
        public new void Fill(DataSet ds, string vendorCode)
        {
            this.VendorCode = vendorCode;
            base.Fill(ds);
        }

        #region "properties"
        public string VendorCode
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        #endregion

    }
    #endregion
}
