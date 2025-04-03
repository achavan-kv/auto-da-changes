SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects 
	where id = object_id('[dbo].[StaffCommissionSummaryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[StaffCommissionSummaryGetSP]
GO


CREATE PROCEDURE 	dbo.StaffCommissionSummaryGetSP
			@branchNo char(3) ,		-- allow All
			@empeetype varchar(3), --IP - 02/06/08 - Credit Collections - Need to cater for (3) character Employee Types.
			
			@return int OUTPUT

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : StaffCommissionSummaryGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get All Staff with commission records
-- Author       : John Croft
-- Date         : 12 June 2007
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/07/09 -jec CR1035 enhancements - allow branchno ="All"
--------------------------------------------------------------------------------

AS

	SET 	@return = 0			--initialise return code

	SELECT	DISTINCT u.Login,
            FullName   + ' ' + convert (varchar(8), Login) AS EmployeeName,
			branchno,
			FullName as name
	FROM Admin.[User] u 
	INNER JOIN salesCommission s  ON u.id = s.Employee
	WHERE	u.id != 0
	and (cast(branchno as char(3))=@branchno or @branchno='All')

	--or	@branchno=(select value from countrymaintenance where codename='hobranchno'))

	ORDER BY u.Login 

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
