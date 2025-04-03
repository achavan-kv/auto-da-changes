SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[RIStockQuantityLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RIStockQuantityLoadSP]
GO

CREATE PROCEDURE RIStockQuantityLoadSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIStockQuantityLoadSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Stock Quantity Data Load
-- Date         : 25 March 2010
--
-- This procedure will load the Stock Quantity detail from the RI interface file.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/07/11  jec Report Error/Warnings
-- ================================================
	-- Add the parameters for the stored procedure here
		@interface varchar(10),
		@runno int,
		@rerun BIT,		
		@repo BIT,
		@return int OUTPUT
AS
BEGIN
    SET 	@return = 0			--initialise return code
    
    DECLARE @dateTimeNow DATETIME = GETDATE() 
    
    SELECT TOP 1 * 
    INTO #RawStkQtyload
    FROM RItemp_RawStkQtyload
    
    TRUNCATE TABLE #RawStkQtyload
    
    IF @repo = 0
		INSERT INTO #RawStkQtyload 
		SELECT * FROM RItemp_RawStkQtyload
	ELSE
		INSERT INTO #RawStkQtyload 
		SELECT * FROM RItemp_RawStkQtyloadRepo
 
	-- Update existing Stock quantities 
	UPDATE StockQuantity
	SET Stock = CONVERT(DECIMAL,r.ActualStock),
		QtyAvailable = CONVERT(DECIMAL,r.AvailableStock),
		DateUpdated	= @dateTimeNow	
	FROM StockInfo i 
	INNER JOIN StockQuantity q ON i.id = q.id AND i.RepossessedItem = @repo
	INNER JOIN  #RawStkQtyload r ON i.IUPC = r.ItemIUPC AND q.stocklocn = r.StockLocn 
	WHERE q.Stock != CONVERT(DECIMAL,r.ActualStock) OR 
		  q.QtyAvailable != CONVERT(DECIMAL,r.AvailableStock)
			
	
	-- Insert new Stock quantities 
	INSERT INTO StockQuantity (itemno, stocklocn, qtyAvailable, stock, stockonorder, stockdamage,
								leadtime, dateupdated, deleted, LastOperationSource, ID)
	SELECT i.itemno, l.StockLocn, l.AvailableStock, l.ActualStock, 0, 0, 
								0, @dateTimeNow, 'N', '', i.ID 
	FROM StockInfo i 
	INNER JOIN #RawStkQtyload l on i.IUPC = l.ItemIUPC AND i.RepossessedItem = @repo
	INNER JOIN dbo.branch B ON B.branchno = l.StockLocn
	WHERE NOT EXISTS (SELECT 1 
					  FROM StockQuantity q 
					  WHERE i.ID = q.ID AND l.StockLocn = q.StockLocn)
	
	-- Set Deleted flag if product status is Excluded
	UPDATE StockQuantity
	SET deleted = 'Y'
	FROM StockInfo i 
	INNER JOIN StockQuantity q on i.id = q.id AND i.RepossessedItem = @repo
	WHERE ProdStatus = 'E' 						
	
	-- Update Cost price (Weighted)
	UPDATE StockPrice
	SET CostPrice = CONVERT(money,r.CostPrice)		
	FROM StockInfo i 
	INNER JOIN StockPrice p ON i.id = p.id AND i.RepossessedItem = @repo
	INNER JOIN #RawStkQtyload r ON r.ItemIUPC = i.IUPC AND p.branchno = r.StockLocn 
	WHERE p.CostPrice != CONVERT(MONEY, r.CostPrice) 
					
	-- Update WarrantyCode table (Also in RIProductLoadSP)
	-- Cost Price may only be entered in the StockQuantityLoad file 
	UPDATE WarrantyCodes
	SET CostPrice = l.CostPrice
	FROM WarrantyCodes w 
	INNER JOIN StockInfo s ON w.ItemID = s.ID 
							AND s.RepossessedItem = @repo
							AND CONVERT(VARCHAR, s.category) IN (SELECT code FROM dbo.code WHERE category = 'WAR')
	--INNER JOIN StockPrice p on w.ItemID = p.ID
	INNER JOIN #RawStkQtyload l ON l.ItemIUPC = s.IUPC
	
	-- If errors - report
	INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
	select distinct @interface, @runno, getdate(),'W', 
	'OHQ file: Item: ' + ItemIUPC + ' does not exists in Cosacs'
	from #RawStkQtyload q
	where not exists(select * from StockInfo i where q.ItemIUPC=i.IUPC)
	
	INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
	select distinct @interface, @runno, getdate(),'W', 
	'OHQ file: StockLocation: ' + StockLocn + ' does not exists in Cosacs'
	from #RawStkQtyload q
	where not exists(select * from Branch b where q.Stocklocn=b.branchno)
						
	      
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
