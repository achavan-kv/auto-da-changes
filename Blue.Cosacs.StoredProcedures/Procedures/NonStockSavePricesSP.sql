SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].NonStockSavePricesSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE NonStockSavePricesSP
END
GO

CREATE PROCEDURE dbo.NonStockSavePricesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : NonStockSavePricesSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Non Stock Save Price details
-- Author       : John Croft
-- Date         : 08 February 2010
--
-- This procedure will save the Non Stock Price details
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/05/11  jec CR1212 RI Integration  
-- 14/06/11  ip  CR1212 RI - #3815 - item was not being saved to StockPrice
-- ================================================
	-- Add the parameters for the stored procedure here
	@itemNo VARCHAR(8),
	@branchno int,	
	@hpprice MONEY,
	@cashprice MONEY,
	@dutyfreeprice MONEY,
	@costprice MONEY,
	@startDate DATETIME,
	@itemId INT,			-- RI	
	@return INT output

As
	set @return=0
	
	-- Update Prices
	if @return=0
	Begin
		
		----IP - 14/06/11 - CR1212 - RI - #3815
		if(@itemId = 0)
		begin
			set @itemId = (select ID from StockInfo where IUPC = @ItemNo)
		end
		
		if exists (select top 1 itemno from stockPrice where ID=@itemId and branchno=@branchno)		-- RI itemno=@itemNo
		BEGIN
			-- item exists 
			UPDATE stockPrice 
				set CreditPrice=@hpprice, CashPrice=@cashprice,DutyFreePrice=@dutyfreeprice,
					CostPrice=@costprice,DateActivated=@StartDate		
			Where ID=@itemId			-- RI	itemno=@itemNo 
				and branchno=@branchno
				and (CreditPrice!=@hpprice or CashPrice!=@cashprice or DutyFreePrice!=@dutyfreeprice
					or CostPrice!=@costprice or ISNULL(DateActivated,'1900-01-01')!=@StartDate)		
			
		END	
		else
		Begin
			-- new item
			Insert into StockPrice (itemno, branchno, CreditPrice, CashPrice, DutyFreePrice, CostPrice, Refcode,DateActivated,ID)		-- RI
			
			Select '',@branchno,@hpprice,@cashprice,@dutyfreeprice,@costprice,'',
				case when @StartDate='1900-01-01' then null	else @StartDate end, @itemId
			--from StockInfo s																					--IP - 14/06/11 - CR1212 - RI - #3815
			--Where s.ID=case when @itemId=0 then (select ID from StockInfo s2 where IUPC=@itemNo)
			--			else @itemId end
		End
		
	End
	
	set @return=@@ERROR	

go	
-- end end end end end end end end end end end end end end end end end end end end end end end 