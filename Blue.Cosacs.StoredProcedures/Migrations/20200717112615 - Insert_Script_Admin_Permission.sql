-- =============================================
-- CREATED BY:	   ANUSHREE URKUNDE
-- CREATE DATE:	   16/07/2020
-- SCRIPT Name:    Insert_Script_Admin_Permission.sql
-- SCRIPT COMMENT: Insert script to add two new user-permission having access on permission 2178 and 2179 
--				   and Updating Existing Permission name having access on permission 2151 and 2153.
-- Discription:    Stock Count Permissions CR 
-- =============================================
----Renaming permission ‘Stock Count Start’ to ‘Stock Count Start - Perpetual & Quarterly’ 
IF EXISTS (
		SELECT 1
		FROM [Admin].[Permission]
		WHERE Id = 2151
		)
	UPDATE [Admin].[Permission]
	SET Name = 'Stock Count Start - Perpetual & Quarterly'
		,Description = 'Allows the user to start Perpetual & Quarterly stock count'
	WHERE Id = 2151

----Renaming permission ‘Stock Count Close’ to ‘Stock Count Close - Perpetual & Quarterly’ 
IF EXISTS (
		SELECT 1
		FROM [Admin].[Permission]
		WHERE Id = 2153
		)
	UPDATE [Admin].[Permission]
	SET Name = 'Stock Count Close - Perpetual & Quarterly'
		,Description = 'Allows the user to finish Perpetual & Quarterly stock count'
	WHERE Id = 2153

----Adding user-permission for Stock Count Start - Perpetual user only
IF NOT EXISTS (
		SELECT 1
		FROM [Admin].[Permission]
		WHERE Id = 2178
		)
BEGIN
	INSERT INTO [Admin].[Permission] (
		[Id]
		,[Name]
		,[CategoryId]
		,[Description]
		,[IsDelegate]
		)
	VALUES (
		2178
		,'Stock Count Start - Perpetual Only'
		,21
		,'Allows the user to start Perpetual stock count only'
		,0
		)
END
ELSE
BEGIN
	IF EXISTS (
			SELECT 1
			FROM [Admin].[Permission]
			WHERE Id = 2178
			)
		UPDATE [Admin].[Permission]
		SET Name = 'Stock Count Start - Perpetual Only'
			,CategoryId = 21
			,Description = 'Allows the user to start Perpetual stock count only'
			,IsDelegate = 0
		WHERE Id = 2178
END

----Adding user-permission for Stock Count Close - Perpetual user only
IF NOT EXISTS (
		SELECT 1
		FROM [Admin].[Permission]
		WHERE Id = 2179
		)
BEGIN
	INSERT INTO [Admin].[Permission] (
		[Id]
		,[Name]
		,[CategoryId]
		,[Description]
		,[IsDelegate]
		)
	VALUES (
		2179
		,'Stock Count Close - Perpetual Only'
		,21
		,'Allows the user to finish Perpetual stock count only'
		,0
		)
END
ELSE
BEGIN
	IF EXISTS (
			SELECT 1
			FROM [Admin].[Permission]
			WHERE Id = 2179
			)
		UPDATE [Admin].[Permission]
		SET Name = 'Stock Count Close - Perpetual Only'
			,CategoryId = 21
			,Description = 'Allows the user to finish Perpetual stock count only'
			,IsDelegate = 0
		WHERE Id = 2179
END
---------------------------------------------------------------------------------------------------------------
