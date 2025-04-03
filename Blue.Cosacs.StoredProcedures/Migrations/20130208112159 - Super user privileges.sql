INSERT INTO Admin.PermissionCategory
        ( Id, Name )
VALUES  ( 100, -- Id - int
          'Super User'  -- Name - varchar(50)
          )
          
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 100000, -- Id - int
          'SuperUser', -- Name - varchar(100)
          100, -- CategoryId - int
          'Grant user SuperUser powers.'  -- Description - varchar(300)
          )
          
          