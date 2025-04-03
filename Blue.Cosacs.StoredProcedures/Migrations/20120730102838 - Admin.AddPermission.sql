-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF  NOT OBJECT_ID('Admin.AddPermission') IS NULL
	DROP PROCEDURE Admin.AddPermission
GO

CREATE PROCEDURE Admin.AddPermission
	@id				Int,
	@Name			VarChar(100),
	@CategoryId		Int,
	@Description	VarChar(300)
AS
	IF EXISTS(SELECT 1 FROM  Admin.Permission WHERE id = @id)
	BEGIN
		Declare @ErrorMessage VarChar(256) ='There is already a permission with the number ' + CONVERT(VarChar, @id) + ' in the system.'
		
		RAISERROR (@ErrorMessage, 
		   20,
		   1,
		   100) WITH LOG
	END 
	ELSE
		INSERT INTO [Admin].Permission
			(id, Name, CategoryId, [Description])
		VALUES
			(@id, @Name, @CategoryId, @Description) 
	
GO