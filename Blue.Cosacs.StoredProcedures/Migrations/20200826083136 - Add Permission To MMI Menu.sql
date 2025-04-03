-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(SELECT 1 FROM [dbo].[Control] WHERE [Control] = 'menuMmiMatrix')
BEGIN

	DECLARE @CategoryId INT, @NewId INT

	-- Get Category Id of 'System Administration' permission category inside which MMI Matrix menu need to add.
	SELECT	@CategoryId = Id 
	FROM	[Admin].[PermissionCategory] WITH(NOLOCK)
	WHERE	Name = 'System Administration'

	-- Get last Id of permission which are under Category Id of 'System Administration' and one to make next permission Id.
	SELECT  @NewId = MAX(Id) + 1
	FROM	[Admin].Permission
	WHERE	CategoryId = @CategoryId

	-- Add entry inside Permission for 'MMI - View Matrix' menu association.
	INSERT	INTO [Admin].Permission 
			(id, Name, CategoryId, [Description], IsDelegate)
	VALUES	(@NewId
			, 'MMI - View Matrix'
			, @CategoryId
			, 'Allows user to view the configured MMI Matrices under MMI Matrix menu accessed via the Systems Maintenance menu'
			, 0)

	-- Add menu 'menuMmiMatrix' entry inside control table to visible it inside application.
	INSERT INTO [dbo].[Control]
			( TaskID,
			  Screen,
			  Control,
			  Visible,
			  Enabled,
			  ParentMenu
			)
	VALUES  ( @NewId, 
			  'MainForm',
			  'menuMmiMatrix',
			  1,
			  1,
			  'menuSysMaint'
			)


	SET  @NewId = @NewId + 1

	-- Add entry inside Permission for 'MMI - Edit Matrix' menu association.
	INSERT	INTO [Admin].Permission 
			(id, Name, CategoryId, [Description], IsDelegate)
	VALUES	(@NewId
			, 'MMI - Edit Matrix'
			, @CategoryId
			, 'Allows user to edit the MMI Matrices under MMI Matrix menu accessed via the Systems Maintenance menu'
			, 0)
	
	-- Add control 'allowEdit' entry inside control table to provide edit access inside inside application.
	INSERT INTO [dbo].[Control]
			( TaskID,
			  Screen,
			  Control,
			  Visible,
			  Enabled,
			  ParentMenu
			)
	VALUES  ( @NewId, 
			  'MmiMatrix',
			  'allowEdit',
			  1,
			  1,
			  ''
			)




END