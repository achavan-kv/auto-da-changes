using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using System.Data;
using DapperLite;

namespace Blue.Cosacs.StockCountApp
{
    public class StockCountRepository : Repository
    {  
        public void SaveStockCount(SimpleStockCountViewModel count)
        {         
            using (var conn = GetConnection())
            {               
                using (var trans = conn.BeginTransaction())
                {
                    if (GetStockCount(count.Id) == null)
                    {
                        conn.Insert(trans, "StockCount", count);
                    }
                    else
                    {
                        conn.Update(trans, "StockCount", count);
                    }
                    trans.Commit();
                }
            }

            foreach (var product in count.Products)
            {
                SaveStockCountProduct(product);
            }
        }

        public void SaveStockCountProduct(SimpleStockCountProductViewModel stockCountProduct)
        {
            using (var conn = GetConnection())
            {   
                using (var trans = conn.BeginTransaction())
                {
                    if (FindStockCountProductBySkuOrBarcode(stockCountProduct.StockCountId, stockCountProduct.Sku) == null)
                    {
                        conn.Insert(trans, "StockCountProduct", stockCountProduct);
                    }
                    else
                    {
                        conn.Update(trans, "StockCountProduct", stockCountProduct);
                    }
                    trans.Commit();
                }              
            }
        }

        public void RemoveStockCount(int id)
        {
            using (var conn = GetConnection())
            {
                using (var trans = conn.BeginTransaction()) 
                {   
                    conn.Execute("DELETE FROM StockCountProduct WHERE StockCountId = @Id", new { Id = id }, trans);
                    conn.Execute("DELETE FROM StockCount WHERE Id = @Id", new { Id = id }, trans);
                    trans.Commit();
                }
            }
        }

        public void ClearAll()
        {
            using (var conn = GetConnection())
            {
                using (var trans = conn.BeginTransaction())
                {
                    conn.Execute("DELETE FROM StockCountProduct");
                    conn.Execute("DELETE FROM StockCount");
                    trans.Commit();
                }
            }
        }

        public List<SimpleStockCountViewModel> GetStockCounts(bool includeProducts)
        {
            using (var conn = GetConnection())
            {
                var stockCounts = conn.Query<SimpleStockCountViewModel>("SELECT * FROM StockCount").ToList();
                if (includeProducts)
                {
                    var products = conn.Query<SimpleStockCountProductViewModel>("SELECT * FROM StockCountProduct").ToList();
                    foreach (var count in stockCounts)
                    {
                        count.Products = products.Where(p => p.StockCountId == count.Id).ToList();
                    }
                }
                return stockCounts;
            }
        }

        public SimpleStockCountViewModel GetStockCount(int id)
        {
            using (var conn = GetConnection())
            {
                var stockCount = conn.Query<SimpleStockCountViewModel>("SELECT * FROM StockCount WHERE Id = @Id", new { Id = id }).FirstOrDefault();
                if (stockCount != null)
                {
                    stockCount.Products = conn.Query<SimpleStockCountProductViewModel>("SELECT * FROM StockCountProduct WHERE StockCountId = @Id", new { Id = id }).ToList();
                }
                return stockCount;
            }
        }

        public SimpleStockCountProductViewModel FindStockCountProductBySkuOrBarcode(int stockCountId, string key)
        {
            using (var conn = GetConnection())
            {
                return conn.Query<SimpleStockCountProductViewModel>("SELECT * FROM StockCountProduct WHERE StockCountId = @StockCountId AND (Sku = @Key OR Barcode = @Key)", new { Key = key, StockCountId = stockCountId }).FirstOrDefault();               
            }
        }

        private SqlCeConnection GetConnection()
        {
            var conn = new SqlCeConnection(Settings.ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
