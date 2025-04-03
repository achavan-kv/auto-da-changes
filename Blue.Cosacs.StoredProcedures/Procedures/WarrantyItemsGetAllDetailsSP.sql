SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].WarrantyItemsGetAllDetailsSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE WarrantyItemsGetAllDetailsSP
END
GO

CREATE PROCEDURE dbo.WarrantyItemsGetAllDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : WarrantyItemsGetAllDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Warranty Items Get All details
-- Author       : John Croft
-- Date         : 23 December 2010
--
-- This procedure will get All the Warranty Item details
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
	
	select ItemNo,itemdescr1,s.category
	From StockInfo s INNER JOIN code c on CAST(s.category as VARCHAR(3)) = c.code and c.category='WAR'
		
	set @return=@@ERROR

go	
-- end end end end end end end end end end end end end end end end end end end end end end end 