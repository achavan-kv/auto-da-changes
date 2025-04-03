INSERT INTO Admin.PermissionCategory
        ( Id, Name )
VALUES  ( 16, -- Id - int
          'Service'  -- Name - varchar(50)
          )
          
          
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1601, -- Id - int
          'Technician Profile Edit', -- Name - varchar(100)
          16, -- CategoryId - int
          'Allow user to edit the Technician Profile on the the User Profile screen.'  -- Description - varchar(300)
          )
          
