SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS(SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[UpdateRepossessedStock]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
	DROP PROCEDURE [dbo].[UpdateRepossessedStock]
GO

CREATE PROCEDURE  dbo.[UpdateRepossessedStock]
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : UpdateRepossessedStock.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : Update Repossessed Stock 
-- Date         : 6 May 2011  
--  
-- This procedure will update the repossessed stock price.  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 06/07/11	 jec #4168 Repossessions done for selling price not cost price - cost price calculated 
-- ================================================  
 -- Add the parameters for the stored procedure here  
  @itemId INT,  
  @stockLocn SMALLINT,  
  --@costPrice MONEY,
  @salePrice MONEY,			-- RI #4168 
  @qty FLOAT,  
  @return int OUTPUT
    
  AS  
	SET  @return = 0
   
	DECLARE @IUPC VARCHAR(18) = '', @taxtype VARCHAR(1) 
	
	SELECT @IUPC = IUPC
	FROM dbo.StockInfo 
	WHERE ID = @itemId AND RepossessedItem = 1
	
	select @taxtype=value from CountryMaintenance where codename='taxtype'
	
	-- if Stock exclusive of tax reduce sale price by taxamt(calculated)
	if @taxtype='E'			
		select @salePrice=ISNULL(ROUND(@salePrice*(100/(taxrate+100)),2),0) 
		from StockInfo where id=@itemid
	
	IF(@IUPC = '')
	BEGIN
		PRINT 'Repossessed item not found in StockInfo'
		RETURN 
	END
	
	--DECLARE @salePrice MONEY = @costPrice
	DECLARE @costPrice MONEY = @salePrice
	
	--SELECT @salePrice = @costPrice * (CONVERT(FLOAT, C.Additional) + 100)/100     -- @costPrice + markup %
	SELECT @costPrice = @salePrice / ((CONVERT(FLOAT, C.Additional) + 100)/100)    -- RI #4168  @salePrice / markup %
	FROM dbo.StockInfo SI 
	INNER JOIN dbo.Code C 
		ON C.Category = 'RPV' AND C.Code = CONVERT(VARCHAR, SI.Category)
	WHERE SI.ID = @itemId		  
	
	--SET @salePrice = ROUND(@salePrice, 2)
	SET @costPrice = ROUND(@costPrice, 2)		-- RI #4168 
	
	IF NOT EXISTS(SELECT 1 FROM dbo.StockPrice WHERE ID = @itemId AND BranchNo = @stockLocn)  
	BEGIN  
		INSERT INTO dbo.StockPrice   
			( itemno , branchno , CreditPrice , CashPrice , DutyFreePrice ,  
				CostPrice , Refcode , DateActivated , ID )  
		VALUES ( @IUPC, @stockLocn, @salePrice, @salePrice, @salePrice,  
					@costPrice, '00', NULL, @itemId)  
	END  
	ELSE  
	BEGIN		
		UPDATE dbo.StockPrice  
		SET CreditPrice = @salePrice,  
			CashPrice = @salePrice,  
			DutyFreePrice = @salePrice,  
			CostPrice = @costPrice  
		WHERE ID = @itemId AND   
			  BranchNo = @stockLocn    
	END   
   
	IF NOT EXISTS(SELECT 1 FROM dbo.StockQuantity WHERE ID = @itemId AND StockLocn = @stockLocn)  
	BEGIN  
		INSERT INTO dbo.StockQuantity  
			  ( itemno , stocklocn , qtyAvailable , stock , stockonorder , stockdamage ,  
				leadtime , dateupdated , deleted , LastOperationSource , ID )  
		VALUES ( @IUPC, @stockLocn, @qty, @qty, 0, 0,  
				0, GETDATE(), 'N', '', @itemId)  
	END  
	ELSE  
	BEGIN  
		UPDATE dbo.StockQuantity  
		SET QtyAvailable += @qty 
		WHERE ID = @itemId AND   
			  StockLocn = @stockLocn  
	END  
  
	IF (@@error != 0)  
	BEGIN  
		SET @return = @@error  
	END 
				
 Go
  
 -- End End End End End End End End End End End End End End End End End End End End End End End End End End End End 