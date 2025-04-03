-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1814, -- Id - int
          'View Warranty Product Link', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to view Warranty Product Link screen.'  -- Description - varchar(300)
          )

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1816, -- Id - int
          'Edit Warranty Product Link', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to edit Warranty Product Links.'  -- Description - varchar(300)
          )