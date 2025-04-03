using System.Data;

namespace Blue.Cosacs.Merchandising
{   
    #region Summary Reindexing Product
    public partial class ReIndexingProduct : Blue.Transactions.Command<Context>
    {
        public ReIndexingProduct() : base("Merchandising.GetReindexedProducts_StockSummary")          
        {
            base.AddInParameter("@INDEX_UPDATED_DATA_ONLY", DbType.Boolean);
        }
    }
	
    partial class ReIndexingProduct
    {
        public void GetReIndexedProducts(DataSet ds)
        {
            base.Fill(ds);
        }
        public DataTable GetReIndexedProducts(bool _UpdateMerchandigingStockSummary)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            this.UpdateMerchandigingStockSummary = _UpdateMerchandigingStockSummary;
            GetReIndexedProducts(ds);
            dt = ds.Tables[0];        
            return dt;
        }
        #region  Properties
        public bool? UpdateMerchandigingStockSummary
        {
            get { return (bool?)base[0]; }
            set { base[0] = value; }
        }
        #endregion

       
    }
    #endregion

    #region Levels Reindexing Product
    public partial class ReIndexingLevels : Blue.Transactions.Command<Context>
    {
        public ReIndexingLevels() : base("Merchandising.GetReindexedProducts_StockLevel")    
        {
            base.AddInParameter("@INDEX_UPDATED_DATA_ONLY", DbType.Boolean);
        }
    }
    partial class ReIndexingLevels
    {
        public void GetReIndexedLevels(DataSet ds)
        {
            base.Fill(ds);
        }
        public DataTable GetReIndexedLevels(bool _UpdateMerchandigingStockLevel)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            this.UpdateMerchandigingStockLevel = _UpdateMerchandigingStockLevel;
            GetReIndexedLevels(ds);
            dt = ds.Tables[0];
            return dt;
        }
        #region  Properties
        public bool? UpdateMerchandigingStockLevel
        {

            get { return (bool?)base[0]; }
            set { base[0] = value; }

        }
        #endregion

    }
    #endregion

}
