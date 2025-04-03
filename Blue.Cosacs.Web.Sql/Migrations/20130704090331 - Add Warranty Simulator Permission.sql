-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1815, -- Id - int
          'View Warranty Simulator', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to view Warranty Simulator screen.'  -- Description - varchar(300)
          )