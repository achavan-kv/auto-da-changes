DECLARE @newid INT

SELECT @newid = MAX(TaskID) + 1 FROM Task

IF NOT EXISTS (SELECT * FROM task 
			   WHERE TaskName = 'View StoreCardView screen.')
BEGIN
INSERT INTO Task
        ( TaskID, TaskName )
SELECT @newid, 'View StoreCardView screen.'
END

IF NOT EXISTS (SELECT * FROM Control 
			   WHERE Control = 'menuViewStoreCard')
BEGIN
INSERT INTO Control
        ( TaskID ,
          Screen ,
          Control ,
          Visible ,
          Enabled ,
          ParentMenu
        )
VALUES  ( @newid , -- TaskID - int
          'MainForm' , -- Screen - varchar(50)
          'menuViewStoreCard' , -- Control - varchar(50)
          1 , -- Visible - int
          1 , -- Enabled - int
          'menuCustomer'  -- ParentMenu - varchar(50)
        )
END
 
 
