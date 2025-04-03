SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdateStockQuantity]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdateStockQuantity]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/03/2009
-- Description:	To update StockQuantity table from Oracle inbound CSV files
-- =============================================

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdateStockQuantity] 
    @itemno varchar(8),	
	@stocklocn smallint,	
	@qtyAvailable float,	
	@stock float,
	@stockonorder float,
	@stockdamage	float,
	@leadtime smallint,
	@dateupdated datetime,
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DECLARE @existingItemNo varchar(8),
			@existingQtyAvailable float,	
			@existingStock float,
			@existingStockOnOrder float,
			@existingStockDamage	float,
			@existingLeadTime smallint,
			@existingDateUpdated datetime,
			@qtyChangedAfterExport float			
	
	----------------------------------------------------------
	
	SELECT	@existingItemNo = itemNo,
			@existingQtyAvailable = qtyAvailable, 
			@existingStock = stock,
			@existingStockOnOrder = stockonorder,
			@existingStockDamage = 	stockdamage,
			@existingLeadTime = leadtime,
			@existingDateUpdated = dateupdated
	FROM StockQuantity 
	WHERE itemno = @itemno and stocklocn = @stocklocn
	
	--------------------------------------------------------
	
	SELECT @qtyChangedAfterExport = IsNull(SUM(QtyChange), 0)
	FROM StockQuantityAuditCosacs 
	WHERE itemNo = @itemno and 	stocklocn = @stocklocn and dateChange > @dateupdated
		
	--------------------------------------------------------
	
	SET @qtyAvailable = @qtyAvailable + @qtyChangedAfterExport
	
	IF  @existingItemNo is not NULL and
		   ( @existingQtyAvailable != @qtyAvailable or @existingStock != @stock or 
			 @existingStockOnOrder != @stockonorder or @existingStockDamage != @stockdamage or 
			 @existingLeadTime != @leadtime or @existingDateUpdated != @dateupdated )
		BEGIN
			-- This update will call a trigger and write entries to StockQuantityAuditCosacs table --
			UPDATE StockQuantity   
			SET 
				qtyAvailable = @qtyAvailable,	
				stock = @stock,
				stockonorder = @stockonorder,
				stockdamage = @stockdamage,
				leadtime = @leadtime,
				dateupdated = @dateupdated,
				LastOperationSource = 'ORACLE'
			WHERE 
			itemno = @itemno and stocklocn = @stocklocn	
			-----------------------------------------------------------------------------------------
			
			-- This update will reset LastOperationSource -------------------------------------------
			UPDATE StockQuantity   
			SET 
				LastOperationSource = ''
			WHERE 
			itemno = @itemno and stocklocn = @stocklocn	
			-----------------------------------------------------------------------------------------
			
		END
	ELSE IF @existingItemNo is NULL
		BEGIN
			INSERT INTO StockQuantity
			( 
				itemno,	stocklocn, qtyAvailable,	
				stock, stockonorder, stockdamage,
				leadtime, dateupdated, LastOperationSource		
			)
			VALUES
			( 
				@itemno, @stocklocn, @qtyAvailable,	
				@stock, @stockonorder, @stockdamage,
				@leadtime, @dateupdated, 'ORACLE'	
			) 
			
			-- This update will reset LastOperationSource -------------------------------------------
			UPDATE StockQuantity   
			SET 
				LastOperationSource = ''
			WHERE 
			itemno = @itemno and stocklocn = @stocklocn	
			-----------------------------------------------------------------------------------------
		END
	
	----------------------------------------------------------
	
	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO