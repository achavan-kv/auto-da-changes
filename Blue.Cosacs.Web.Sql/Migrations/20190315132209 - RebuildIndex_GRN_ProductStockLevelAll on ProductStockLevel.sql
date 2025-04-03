IF EXISTS (SELECT top 1 * FROM sys.indexes WHERE name='NonClusteredIndex-ProductStockLevelAll' 
			   AND object_id = OBJECT_ID('Merchandising.ProductStockLevel'))
			BEGIN 
			    DROP INDEX [NonClusteredIndex-ProductStockLevelAll] ON [Merchandising].[ProductStockLevel]
			END 
			GO
			
			CREATE NONCLUSTERED INDEX [NonClusteredIndex-ProductStockLevelAll] ON [Merchandising].[ProductStockLevel]
			(
				[Id] ASC,
				[LocationId] ASC,
				[ProductId] ASC,
				[StockOnHand] ASC,
				[StockOnOrder] ASC,
				[StockAvailable] ASC
			)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
			GO