-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: CR12949 - #13715

IF NOT EXISTS(select * from Admin.Permission where Id = 1205)
BEGIN
	INSERT INTO Admin.Permission
			( Id, Name, CategoryId, Description )
	VALUES  ( 1205, -- Id - int
			  'Contracts Setup Screen', -- Name - varchar(100)
			  12, -- CategoryId - int
			  'Allow access to the Contracts Setup Screen.'  -- Description - varchar(300)
			  )
END