
-- =======================================================================================
-- Project			: CoSaCS.NET
-- Procedure Name   : ApplicantSpendFactorPermissions
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: Script to insert Permissions

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================

	
DECLARE @CategoryId INT, @NewId INT

SELECT    @CategoryId = Id 
    FROM    [Admin].[PermissionCategory] WITH(NOLOCK)
    WHERE    Name = 'System Administration'



IF NOT EXISTS(SELECT 1 FROM [Admin].[Permission] where [Name] = 'EditApplicantSpend')
BEGIN
INSERT INTO [ADMIN].[PERMISSION] ([ID], [NAME],	[CATEGORYID],	[DESCRIPTION],	[ISDELEGATE])
 SELECT (SELECT MAX(ID)+1 FROM [ADMIN].[PERMISSION] where CategoryId = @CategoryId),'EditApplicantSpend',@CategoryId,'Allow user to Edit applicant Matrix under Spend Factor accessed via Systems Maintenance menu',0 
END


IF NOT EXISTS(SELECT 1 FROM [Admin].[Permission] where [Name] = 'ViewApplicantSpend')
BEGIN
INSERT INTO [ADMIN].[PERMISSION] ([ID], [NAME],	[CATEGORYID],	[DESCRIPTION],	[ISDELEGATE])
 SELECT (SELECT MAX(ID)+1 FROM [ADMIN].[PERMISSION] where CategoryId = @CategoryId),'ViewApplicantSpend',@CategoryId,'Allow user to View applicant Matrix under Spend Factor accessed via Systems Maintenance menu',0 
END

--------------------------------------------------------------------------------------------------------------------------------------





