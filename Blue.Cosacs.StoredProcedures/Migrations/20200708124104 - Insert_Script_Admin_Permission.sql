
-- =============================================
-- CREATED BY:	   ANUSHREE URKUNDE
-- CREATE DATE:	   02/07/2020
-- SCRIPT Name:    Insert_Script_Admin_Permission.sql
-- SCRIPT COMMENT: Insert script to add two new user-permission having access on permission 2178 and 2179 
--				   and Updating Existing Permission name having access on permission 2151 and 2153.
-- Discription:    Stock Count Permissions CR 
-- =============================================


----Renaming permission ‘Stock Count Start’ to ‘Stock Count Start - Perpetual & Quarterly’ 
IF EXISTS(
			SELECT 1 FROM  
			[Admin].[Permission] 
			 WHERE Id=2151 AND Name ='Stock Count Start'
)
		
			UPDATE [Admin].[Permission]
			SET Name='Stock Count Start - Perpetual & Quarterly' 
			WHERE Id=2151 AND Name ='Stock Count Start'

----Renaming permission ‘Stock Count Close’ to ‘Stock Count Close - Perpetual & Quarterly’ 
IF EXISTS(
			SELECT 1 FROM  
			[Admin].[Permission] 
			 WHERE Id=2153 AND Name ='Stock Count Close'
)

			UPDATE [Admin].[Permission] 
			SET Name='Stock Count Close - Perpetual & Quarterly' 
			WHERE Id=2153 AND Name ='Stock Count Close'

----Adding user-permission for Stock Count Start - Perpetual user only
IF NOT EXISTS(
				SELECT 1 FROM  [Admin].[Permission] WHERE Id =2178
)

	INSERT INTO [Admin].[Permission]
		([Id], 
		[Name],
		[CategoryId], 
		[Description],
		[IsDelegate])
	VALUES 
		(2178, 
		'Stock Count Start - Perpetual', 
		21, 
		'Allows the user to start the stock perpetual counts',
		0);

----Adding user-permission for Stock Count Close - Perpetual user only

IF NOT EXISTS(
				SELECT 1 FROM  [Admin].[Permission] WHERE Id =2179
)

	INSERT INTO [Admin].[Permission] 
	    ([Id], 
		[Name],
		[CategoryId], 
		[Description],
		[IsDelegate])
	VALUES
		(2179, 
		'Stock Count Close - Perpetual',
		 21,
		'Allows the user to close the stock perpetual counts',
		0);

---------------------------------------------------------------------------------------------------------------