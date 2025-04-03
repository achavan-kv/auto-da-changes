-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1811, -- Id - int
          'Create/Delete Warranty Return Pecentage', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to create new Warranty Return Pecentage and delete existing ones.'  -- Description - varchar(300)
          )

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1812, -- Id - int
          'View Warranty Return Pecentage', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to view Warranty Return Pecentage.'  -- Description - varchar(300)
          )
