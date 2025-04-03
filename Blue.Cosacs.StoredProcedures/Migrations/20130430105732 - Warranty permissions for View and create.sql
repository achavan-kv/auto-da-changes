UPDATE Admin.Permission
SET Name = 'View Warranty Hierarchy', Description = 'Allow the user to view Warranty Levels and Tags'
where Id = 1801

UPDATE Admin.Permission
SET Name = 'Manage Warranty Hierarchy', Description = 'Allow the user to add/edit Warranty Levels and Tags'
where Id = 1802

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1803, -- Id - int
          'Create ad-hoc Tags', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to create new Tags when adding Warranties.'  -- Description - varchar(300)
          )
