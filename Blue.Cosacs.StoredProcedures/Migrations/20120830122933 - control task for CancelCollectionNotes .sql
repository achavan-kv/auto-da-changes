
INSERT INTO Admin.Permission
        ( Id, Name, CategoryId, Description )
VALUES  ( 216, -- Id - int
          'Cancel Collection Notes', -- Name - varchar(100)
          14, -- CategoryId - int
          'Allows user to view the Cancel Collection Note screen'  -- Description - varchar(300)
          )
          
INSERT INTO dbo.Control
        ( TaskID ,
          Screen ,
          Control ,
          Visible ,
          Enabled ,
          ParentMenu
        )
VALUES  ( 216 , -- TaskID - int
          'MainForm' , -- Screen - varchar(50)
          'menuCancelCollectionNotes' , -- Control - varchar(50)
          1 , -- Visible - int
          1 , -- Enabled - int
          'menuWarehouse'  -- ParentMenu - varchar(50)
        )