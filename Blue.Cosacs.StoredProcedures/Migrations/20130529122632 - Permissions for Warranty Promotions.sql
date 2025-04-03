INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1809, -- Id - int
          'Create/Delete Warranty Promotions', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to create new Warranty promotions and delete existing promotions.'  -- Description - varchar(300)
          )

INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 1810, -- Id - int
          'View Warranty Promotions', -- Name - varchar(100)
          18, -- CategoryId - int
          'Allow user to view Warranty promotions.'  -- Description - varchar(300)
          )
