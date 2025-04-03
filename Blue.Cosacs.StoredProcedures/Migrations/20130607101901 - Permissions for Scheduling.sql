INSERT INTO Admin.PermissionCategory
(Id, Name)
VALUES
(19, 'Scheduling')

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1901, -- Id - int
          'View Schedules', -- Name - varchar(100)
          19, -- CategoryId - int
          'Allow user to view scheduled tasks.'  -- Description - varchar(300)
          )

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1902, -- Id - int
          'Edit Schedules', -- Name - varchar(100)
          19, -- CategoryId - int
          'Allow user to add/edit scheduled tasks.'  -- Description - varchar(300)
          )
