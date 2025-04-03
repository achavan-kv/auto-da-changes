-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

GO
-- =======================================================================================
-- Script Description:	This script add permission for ApplicantSpend Factor.
-- Author:				SHUBHAM GAIKWAD
-- =======================================================================================

	
DECLARE @CategoryId INT, @NewId INT

SELECT	@CategoryId = Id 
FROM	[Admin].[PermissionCategory] WITH(NOLOCK)
WHERE	Name = 'System Administration'

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

GO