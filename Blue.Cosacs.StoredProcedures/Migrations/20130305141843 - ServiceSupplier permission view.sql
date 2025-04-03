INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1626, -- Id - int
          'Service Supplier Edit', -- Name - varchar(100)
          16, -- CategoryId - int
          'Allow user to edit Service Suppliers.'  -- Description - varchar(300)
          )
          
          
UPDATE Admin.Permission
SET Name = 'Service Supplier View', 
    Description = 'Allow user to view Service Suppliers'
WHERE id = 1625