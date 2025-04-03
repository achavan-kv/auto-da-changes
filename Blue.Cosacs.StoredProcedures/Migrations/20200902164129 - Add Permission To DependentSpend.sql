-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This script is used to add permission for Dependent Spend Factor. 
-- =======================================================================================

GO
DECLARE @CategoryId INT, @NewId INT

SELECT	@CategoryId = Id
FROM	[Admin].[PermissionCategory] WITH (NOLOCK)
WHERE	Name = 'System Administration'

IF NOT EXISTS (SELECT 1 FROM [Admin].[Permission] WHERE [Name] = 'EditDependentSpend')
BEGIN
	INSERT INTO [ADMIN].[PERMISSION] (
		[ID]
		,[NAME]
		,[CATEGORYID]
		,[DESCRIPTION]
		,[ISDELEGATE]
		)
	SELECT (
			SELECT MAX(ID) + 1
			FROM [ADMIN].[PERMISSION]
			WHERE CategoryId = @CategoryId
			)
		,'EditDependentSpend'
		,@CategoryId
		,'Allow user to Edit dependent Matrix under Spend Factor accessed via Systems Maintenance menu'
		,0
END

IF NOT EXISTS (
		SELECT 1
		FROM [Admin].[Permission]
		WHERE [Name] = 'ViewDependentSpend'
		)
BEGIN
	INSERT INTO [ADMIN].[PERMISSION] (
		[ID]
		,[NAME]
		,[CATEGORYID]
		,[DESCRIPTION]
		,[ISDELEGATE]
		)
	SELECT (
			SELECT MAX(id) + 1
			FROM [Admin].[Permission]
			WHERE CategoryId = @CategoryId
			)
		,'ViewDependentSpend'
		,@CategoryId
		,'Allow user to view dependent Matrix under Spend Factor accessed via Systems Maintenance menu'
		,0
END

GO