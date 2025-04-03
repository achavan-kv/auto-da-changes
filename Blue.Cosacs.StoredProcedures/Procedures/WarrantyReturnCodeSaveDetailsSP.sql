SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'WarrantyReturnCodeSaveDetailsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE WarrantyReturnCodeSaveDetailsSP
END
GO

CREATE PROCEDURE WarrantyReturnCodeSaveDetailsSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : WarrantyReturnCodeSaveDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Warranty Return Codes 
-- Author       : John Croft
-- Date         : 16 December 2010
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/12/10 jec ManufactureMonths added to primary key
--------------------------------------------------------------------------------
    -- Parameters
    @productType	    char(1),
    @category           varchar(20),
    @returnCode		    VARCHAR(8),
	@warrantyMonths	   int,
	@manufactMonths	   int,
	@expiredMonths	   int,
	@refundPct		   FLOAT,
	@dateNow			DATETIME,    
    @return             INTEGER OUTPUT

AS  

    SET @return = 0
    SET NOCOUNT ON
-- if type, expired period, warranty length exists & not flagged for delete
if exists(select * from WarrantyReturnCodes 
			where ProductType=@productType 
				and WarrantyMonths=@WarrantyMonths
				and ManufacturerMonths=@ManufactMonths		--jec
				and MonthSinceDelivery=@ExpiredMonths and @category!='Delete')
	Begin
  UPDATE WarrantyReturnCodes 
	set ReturnCode=@returnCode,						--jec	21/12/10
		refundpercentfromAIG=@RefundPct,DateChange=@DateNow
	where ProductType=@productType 
			and WarrantyMonths=@WarrantyMonths
			and MonthSinceDelivery=@ExpiredMonths
			and(ReturnCode!=@returnCode
			--or ManufacturerMonths!=@ManufactMonths		--jec  21/12/10
			or	refundpercentfromAIG!=@RefundPct)
		
	End	

else		
--	if type, expired period, warranty length, Manufact length doesn't exist or flagged for delete
	BEGIN
		-- delete code if marked for delete
		delete WarrantyReturnCodes
		where ProductType=@productType 
			and WarrantyMonths=@WarrantyMonths
			and ManufacturerMonths=@ManufactMonths		--jec 21/12/10
			and MonthSinceDelivery=@ExpiredMonths and @Category = 'Delete'
		
		-- insert new codes
		insert into WarrantyReturnCodes (ProductType, MonthSinceDelivery, ReturnCode, refundpercentfromAIG,
					 WarrantyMonths, ManufacturerMonths, DateChange)
		select @ProductType, @ExpiredMonths, @ReturnCode, @RefundPct,
					@WarrantyMonths, @ManufactMonths, @DateNow
		where @Category != 'Delete'		
		
	END
	
    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- end end end end end end end end end end end end end end end end end end end end end end end 
