SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].ProductCategoryGetSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE ProductCategoryGetSP
END
GO

CREATE PROCEDURE dbo.ProductCategoryGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : ProductCategoryGetSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Product category get details
-- Author       : John Croft
-- Date         : 09 December 2010
--
-- This procedure will get the Product category details
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
	
	select case 
		when code<10 then '0' + CAST(code as VARCHAR(3)) 
		else CAST(code as VARCHAR(3)) 
		end	as code,
		case
		when code<10 then '0' + CAST(code as VARCHAR(3)) +': ' + codedescript
		else CAST(code as VARCHAR(3)) +': ' + codedescript
		end as category
	from code c
	where category in('PCE','PCF','PCW','PCO','PCDIS')
	order by code 
		
	
	set @return=@@ERROR

go	
-- end end end end end end end end end end end end end end end end end end end end end end end