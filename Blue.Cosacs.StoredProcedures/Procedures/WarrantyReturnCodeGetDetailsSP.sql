SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].WarrantyReturnCodeGetDetailsSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE WarrantyReturnCodeGetDetailsSP
END
GO

CREATE PROCEDURE dbo.WarrantyReturnCodeGetDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : WarrantyReturnCodeGetDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Warranty Return Code Get details
-- Author       : John Croft
-- Date         : 15 December 2010
--
-- This procedure will get the Warranty Return Code details
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
	-- Add the parameters for the stored procedure here
	
	@return INT output

As
	set @return=0
	
	select ProductType,
	case when ProductType='E' then 'Electrical'
				  when ProductType='F' then 'Furniture'
				  else 'Instant Replacement' end as Category,
	ReturnCode,WarrantyMonths,ManufacturerMonths,
			MonthSinceDelivery as ExpiredPortion,refundpercentfromAIG as RefundPercentage
	from WarrantyReturnCodes
	order by ProductType, WarrantyMonths, MonthSinceDelivery
	
	set @return=@@ERROR

go	
-- end end end end end end end end end end end end end end end end end end end end end end end 