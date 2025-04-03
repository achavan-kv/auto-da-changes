INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 389, -- Id - int
          'View User Audit on Profile Screen', -- Name - varchar(100)
          12, -- CategoryId - int
          'User can see audit on the profile screen'  -- Description - varchar(300)
          )
          
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 390, -- Id - int
          'View Audit', -- Name - varchar(100)
          12, -- CategoryId - int
          'User can see the audit screen'  -- Description - varchar(300)
          )