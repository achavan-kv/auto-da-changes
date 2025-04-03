INSERT INTO Admin.PermissionCategory
	( Id, Name )
VALUES  ( 18, -- Id - int
	'Warranty'  -- Name - varchar(50)
	)

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1801, -- Id - int
          'View Warranty Category', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to view Warranty categories.'  -- Description - varchar(300)
          )

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1802, -- Id - int
          'Create Warranty Category', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to create a new Warranty category.'  -- Description - varchar(300)
          )

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1803, -- Id - int
          'Edit Warranty Category', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to edit existing Warranty categories.'  -- Description - varchar(300)
          )
