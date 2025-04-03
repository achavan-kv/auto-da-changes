-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from admin.[Permission] where id =394)
Begin

    INSERT INTO Admin.Permission
            ( Id, Name, CategoryId, [Description] )
    VALUES  ( 394, -- Id - int
              'Online Product Maintenance', -- Name - varchar(100)
              14, -- CategoryId - int
              'Allows user access to the Online Product Maintenance screen' 
              )

End


