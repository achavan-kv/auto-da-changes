SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_LoadAllocatetableStaffandTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_LoadAllocatetableStaffandTypesSP]
GO

CREATE PROCEDURE 	dbo.CM_LoadAllocatetableStaffandTypesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CM_LoadAllocatetableStaffandTypesSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By		Description
-- ----      --		-----------
-- 12/09/11  NM/IP #4561 - UAT50 - Improve performance loading Worklist setup screen
-- ================================================
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	u.id AS Empeeno,
			u.FullName AS EmployeeName,
			u.branchno,
			ISNULL(r.Name, 'Unknown') AS EmpeeType,
			ISNULL(r.Name, 'Unknown') AS EmpeetypeDescription
	FROM Admin.[User] u
	LEFT JOIN Admin.UserRole ur ON ur.UserId = u.Id
	LEFT JOIN Admin.Role r ON r.Id = ur.RoleId
	WHERE Admin.CheckPermission(u.id, 381) = 1
    OR Admin.CheckPermission(u.id, 380) = 1
	OR EXISTS (SELECT 1 FROM CMActionRights r WHERE r.EmpeeNo = u.Id)
	order by ISNULL(r.Name, 'Unknown'), u.branchno, u.FullName
	set @return = @@error
	
	--12/09/11 - NM/IP
	SELECT distinct EmpeeNo,
		 ISNULL(r.Name, 'Unknown') AS EmpeeType
	 FROM CMActionRights A
	 LEFT JOIN Admin.UserRole ur ON ur.UserId = A.Empeeno
	 LEFT JOIN Admin.Role r ON r.Id = ur.RoleId
	JOIN code c ON c.code = a.ACTION 
	WHERE c.category = 'FUA' 
	
	SELECT EmpeeNo,
		   ISNULL(r.Name, 'Unknown') AS EmpeeType,
		   A.Strategy,
		   ISNULL(ST.description,'') AS StrategyDesc,
		   A.Action,
		   c.codedescript AS Description,
		   CycleToNextFlag,
		   MinNotesLength
	 FROM CMActionRights A
	 LEFT JOIN Admin.UserRole ur ON ur.UserId = A.Empeeno
	 LEFT JOIN Admin.Role r ON r.Id = ur.RoleId
	JOIN code c ON c.code = a.ACTION 
	LEFT OUTER JOIN CMStrategy ST on A.Strategy = ST.Strategy 
	WHERE c.category = 'FUA' order by Name, EmpeeNo, A.Action, A.Strategy
	set @return = @@error

	--12/09/11 - NM/IP
	SELECT distinct EmpeeNo, ISNULL(r.Name, 'Unknown') AS EmpeeType
	FROM CMWorkListRights W
	LEFT JOIN Admin.UserRole ur ON ur.UserId = EmpeeNo
	LEFT JOIN Admin.Role r ON r.Id = ur.RoleId

	SELECT DISTINCT EmpeeNo, W.worklist, ISNULL(WL.Description,'') AS Worklistdesc, ISNULL(r.Name, 'Unknown') AS EmpeeType
	FROM CMWorkListRights W
    LEFT JOIN Admin.UserRole ur ON ur.UserId = w.EmpeeNo
    LEFT JOIN Admin.Role r ON r.Id = ur.RoleId
    LEFT JOIN CMWorkList WL ON wl.worklist = w.worklist
    ORDER BY W.EmpeeNo,W.worklist

	set @return = @@error
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


