SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects 
	where id = object_id('[dbo].[StaffSalesCommissionSummaryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[StaffSalesCommissionSummaryGetSP]
GO


CREATE PROCEDURE 	dbo.StaffSalesCommissionSummaryGetSP
			@branchNo char(3) ,	
			@empeetype varchar(3),
			
			@return int OUTPUT

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : StaffSalesCommissionSummaryGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get All Staff with commission records
-- Author       : Ilyas Parker
-- Date         : 21st October 2014
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--------------------------------------------------------------------------------

AS

	SET 	@return = 0			--initialise return code

	SELECT	DISTINCT u.id,
            FullName   + ' ' + convert (varchar(8), u.id) AS EmployeeName
	FROM 
		Admin.[User] u 
	INNER JOIN 
		salesCommission s  ON u.id = s.Employee
	WHERE	u.id != 0
		and (cast(s.StockLocn as char(3))=@branchno or @branchno='All')

	ORDER BY u.id

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
