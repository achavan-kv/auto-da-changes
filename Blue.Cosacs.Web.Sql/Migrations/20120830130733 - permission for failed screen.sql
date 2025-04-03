INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 393, -- Id - int
          'Failed Delivery and Collection', -- Name - varchar(100)
          14, -- CategoryId - int
          'Allows user access to the Failed Delivery and Collection screen'  -- Description - varchar(300)
          )
          
          
UPDATE dbo.Control
SET TaskID = 393
WHERE Control = 'menuFailedDeliveriesCollections'