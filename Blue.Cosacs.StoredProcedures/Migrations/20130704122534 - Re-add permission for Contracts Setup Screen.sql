-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: CR12949 - #13715

IF EXISTS(select * from Admin.Permission where Id = 1205)
BEGIN
	delete from Admin.Permission where id = 1205 and name = 'Contracts Setup Screen'
END


IF NOT EXISTS(select * from Admin.Permission where Id = 1002)
BEGIN
	INSERT INTO Admin.Permission
			( Id, Name, CategoryId, Description )
	VALUES  ( 1002, -- Id - int
			  'Contracts Setup Screen', -- Name - varchar(100)
			  10, -- CategoryId - int
			  'Allow access to the Contracts Setup Screen.'  -- Description - varchar(300)
			  )
END


