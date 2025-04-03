using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Repository
{

    #region PriceUpdateRepository 
    public partial class UpdateInventoryRepository : Blue.Transactions.Command<ContextBase>
    {
        public UpdateInventoryRepository() : base("dbo.VE_PriceUpdate")
        {
            base.AddInParameter("@XmlString", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);

        }
    }
    partial class UpdateInventoryRepository
    {
        public List<string> PriceUpdate()
        {
            List<string> returnExternalProductID = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnExternalProductID.Add(this.Message);
            returnExternalProductID.Add(Convert.ToString(this.StatusCode));
            return returnExternalProductID; // this.ReturnCustId==""?"Message:"+this.Message:"CustID:"+this.ReturnCustId;
        }

        public List<string> PriceUpdate(string userJson)
        {
            this.UserJson = userJson;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return PriceUpdate();
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
        #endregion

        #region StockTransferInsertRepository 
        public partial class StockTransferInsertRepository : Blue.Transactions.Command<ContextBase>
        {
            public StockTransferInsertRepository() : base("dbo.VE_StockTransferSave")
            {
                base.AddInParameter("@StockTransferxml", DbType.Xml);
                base.AddOutParameter("@Message", DbType.String, 5000);
                base.AddOutParameter("@StatusCode", DbType.Int32, 32);
            }
        }
        partial class StockTransferInsertRepository
        {
            public List<string> CreateStockTransfer()
            {
                List<string> returnStockTransferId = new List<string>();// string.Empty;
                int RecordCount = base.ExecuteNonQuery();
                returnStockTransferId.Add(this.Message);
                returnStockTransferId.Add(Convert.ToString(this.StatusCode));
                return returnStockTransferId;
            }

            public List<string> CreateStockTransfer(string strStockTransfer)
            {
                this.StockTransfer = strStockTransfer;
                this.Message = string.Empty;
                this.StatusCode = 0;
                return CreateStockTransfer();
            }

            #region "properties"
            public string StockTransfer
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

    }

    #region StockTransferInsertRepository 
    public partial class StockTransferInsertRepository : Blue.Transactions.Command<ContextBase>
    {
        public StockTransferInsertRepository() : base("dbo.VE_StockTransferSave")
        {
            base.AddInParameter("@StockTransferxml", DbType.Xml);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@StatusCode", DbType.Int32, 32);
        }
    }
    partial class StockTransferInsertRepository
    {
        public List<string> CreateStockTransfer()
        {
            List<string> returnStockTransferId = new List<string>();// string.Empty;
            int RecordCount = base.ExecuteNonQuery();
            returnStockTransferId.Add(this.Message);
            returnStockTransferId.Add(Convert.ToString(this.StatusCode));
            return returnStockTransferId;
        }

        public List<string> CreateStockTransfer(string strStockTransfer)
        {
            this.StockTransfer = strStockTransfer;
            this.Message = string.Empty;
            this.StatusCode = 0;
            return CreateStockTransfer();
        }

        #region "properties"
        public string StockTransfer
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

    #region GetGRNDetails
    public partial class GetStockTransferRepository : Blue.Transactions.Command<ContextBase>
    {
        public GetStockTransferRepository() : base("VE_StockTransferDetails")
        {
            base.AddInParameter("@StkTrfNo", DbType.String);
            base.AddOutParameter("@Message", DbType.String, 5000);
            base.AddOutParameter("@Status", DbType.String, 5000);
        }
    }

    partial class GetStockTransferRepository
    {

        public new void Fill(DataSet ds, string StkTrfNo)
        {
            this.StkTrfNo = StkTrfNo;
            this.Message = this.Message;
            this.Status = this.Status;
            base.Fill(ds);
        }
        #region "properties"
        public string StkTrfNo
        {
            get { return (string)base[0]; }
            set { base[0] = value; }
        }
        public string Message
        {
            get { return (string)base[1]; }
            set { base[1] = value; }
        }
        public string Status
        {
            get { return (string)base[2]; }
            set { base[2] = value; }
        }

        #endregion

    }
    #endregion
}
