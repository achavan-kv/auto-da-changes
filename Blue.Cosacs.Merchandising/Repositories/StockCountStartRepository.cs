namespace Blue.Cosacs.Merchandising.Repositories
{
    using System;
    using System.Data;

  
    public partial class StockCountStartRepository : Blue.Transactions.Command<Context>
    {
        public StockCountStartRepository() : base("Merchandising.StockCountStart")
        {
            base.AddInParameter("@StockCountId", DbType.Int32);
            base.AddInParameter("@StartedById", DbType.Int32);

        }
    }
    partial class StockCountStartRepository
    {
        public void StartStockCount(DataSet ds)
        {
            try
            {
                base.Fill(ds);
            }
            catch (System.Exception ex)
            {
                throw ex; 
            }
            
        }
        public string PopulateStockCount(int _StockCountId,int _StartedById)
        {   
            DataSet ds = new DataSet();
            string productIds = string.Empty;
            this.StockCountId = _StockCountId;
            this.StartedById = _StartedById;
            StartStockCount(ds);
            productIds = Convert.ToString(ds.Tables[0]);
            return productIds;
        }
        #region  Properties
        public int StockCountId
        {
            get { return (int)base[0]; }
            set { base[0] = value; }
        }
        public int StartedById
        {
            get { return (int)base[1]; }
            set { base[1] = value; }
        }
        #endregion

    }
}
