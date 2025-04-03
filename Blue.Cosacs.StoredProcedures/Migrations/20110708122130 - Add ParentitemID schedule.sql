
IF NOT EXISTS (SELECT OBJECT_NAME(id),* FROM syscolumns 
			   WHERE name = 'ParentItemID'
			   AND OBJECT_NAME(id) = 'schedule')
BEGIN
	ALTER TABLE schedule
	ADD ParentItemID INT NULL
END