SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetZoneAllocatableEmpeeInfo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetZoneAllocatableEmpeeInfo]
GO

--Author : Mohamed Nasmi

CREATE PROCEDURE [dbo].[CM_GetZoneAllocatableEmpeeInfo] 
		@return int output
AS  
	SET @return = 0    --initialise return code	
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--//-------------------------------------------------
	select Roleid AS code, name, name + ' - ' + CONVERT(VARCHAR,Roleid) as concatDesc 
	FROM Admin.RolePermission rp
	INNER JOIN admin.role ON RoleId = id
	WHERE PermissionId = 381 
	AND rp.[Deny] != 1
	order by NAME
		
	--//-------------------------------------------------
	
	--//-------------------------------------------------
	select branchno, branchname, (Convert( varchar, branchno) + ' ' + branchname) as concatDesc  
	from branch 
	order by branchno
	--//-------------------------------------------------
	
	--//-------------------------------------------------
	select u.branchno, u.id AS empeeno, FullName as EmployeeName, name AS EmpeeType, minAccounts, maxAccounts, allocationRank, alloccount AS allocation, (Convert( varchar, u.id) + ' - ' + FullName) as concatDesc 
	from Admin.[User] u
	INNER JOIN Admin.UserRole ur ON u.Id = ur.UserId
	INNER JOIN dbo.courtsperson c ON c.UserId = u.id
	INNER JOIN Admin.Role r ON ur.RoleId = r.Id
	WHERE admin.CheckPermission(u.id,381) = 1
	order by u.id
	--//-------------------------------------------------


	SET @return = @@error
	
	
	
