if exists (select * from dbo.sysobjects where id = object_id('Warehouse.UpdateStockQty') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE Warehouse.UpdateStockQty
GO

CREATE PROCEDURE Warehouse.UpdateStockQty
	@itemId INT,
	@stockLocn INT,
	@qty INT
AS

	UPDATE StockQuantity 
	SET Stock += @qty
	WHERE ID = @itemID 
	AND StockLocn = @stockLocn
	
GO
