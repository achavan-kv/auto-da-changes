using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DapperLite;
using System.Data.SqlServerCe;
using System.IO;

namespace Blue.Cosacs.StockCountApp
{    
    
    public class Repository
    {
        protected SqlCeDatabase<Guid> Db { get; set; }

        public Repository()
        {
            SqlCeConnection conn = null;
            var dirPath = Settings.DatabasePath.Substring(0, Settings.DatabasePath.LastIndexOf('/'));
            
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (!File.Exists(Settings.DatabasePath))
            {
                new SqlCeEngine(Settings.ConnectionString).CreateDatabase();
                conn = GetConnection();
                var cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"CREATE TABLE StockCount(
                    [Id] int NOT NULL PRIMARY KEY,
                    [LocationId] int NOT NULL,
                    [Location] nvarchar(100) NOT NULL,
                    [Type] nchar(10) NOT NULL,
                    [CountDate] datetime NOT NULL)";
                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"CREATE TABLE StockCountProduct( 
                    [Id] int NOT NULL,             
                    [StockCountId] int NOT NULL, 
                    [Sku] nvarchar(20) NULL,                    
                    [Barcode] nvarchar(60) NULL,
                    [LongDescription] nvarchar(240) NULL,
                    [Count] int NULL)";
                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"ALTER TABLE StockCountProduct
                    ADD CONSTRAINT FK_StockCountProduct_StockCount
                    FOREIGN KEY (StockCountId) REFERENCES StockCount(Id)";
                cmd.ExecuteNonQuery();

                cmd = conn.CreateCommand();
                cmd.CommandText =
                    @"ALTER TABLE StockCountProduct
                    ADD CONSTRAINT PK_StockCountProduct
                    PRIMARY KEY (Id, StockCountId)";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    @"CREATE TABLE [User](                     
                    [Id] nvarchar(100) NOT NULL PRIMARY KEY,                    
                    [ModifiedDate] datetime NOT NULL)";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                   @"CREATE TABLE [Settings]( 
                    [Id] int NOT NULL PRIMARY KEY,
                    [Host] nvarchar(500) NOT NULL)";
                cmd.ExecuteNonQuery();
            }
            else
            {
                conn = GetConnection();
            }

            Db = new SqlCeDatabase<Guid>(conn);
            Db.Init();       
        }

        protected SqlCeConnection GetConnection()
        {
            var conn = new SqlCeConnection(Settings.ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
