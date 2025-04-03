SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_OracleInteg_UpdateStockPrice]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_OracleInteg_UpdateStockPrice]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 03/03/2009
-- Description:	To update StockPrice table from Oracle inbound CSV files
-- =============================================

CREATE PROCEDURE [dbo].[DN_OracleInteg_UpdateStockPrice] 
    @itemno	varchar(8),	
    @branchno smallint,	
    @CreditPrice money,	
    @CashPrice money,	
    @DutyFreePrice money,	
    @CostPrice money,	
    @taxrate float,	
	@return INT OUTPUT

AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	UPDATE StockPrice
	SET 
		CreditPrice = @CreditPrice,	
		CashPrice = @CashPrice,	
		DutyFreePrice = @DutyFreePrice,	
		CostPrice = @CostPrice
		--taxrate = @taxrate Tax rate is on stockinfo 
	WHERE 
	itemno = @itemno and branchno = @branchno

	IF @@ROWCOUNT = 0

    INSERT INTO StockPrice
    ( 
		itemno, branchno, CreditPrice,
		CashPrice, DutyFreePrice, CostPrice

	)
	VALUES
	( 
		@itemno, @branchno, @CreditPrice,
		@CashPrice, @DutyFreePrice, @CostPrice
		
	) 

	UPDATE StockInfo SET taxrate = @taxrate WHERE itemno = @itemno  -- stockinfo now contains taxrate

	SET @return = @@ERROR

	RETURN @return
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO