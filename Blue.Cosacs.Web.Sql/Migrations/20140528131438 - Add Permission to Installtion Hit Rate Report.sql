IF NOT EXISTS (SELECT Id FROM Admin.Permission WHERE Id=2014)
BEGIN
INSERT INTO Admin.Permission
	(Id, Name, CategoryId, Description)
VALUES
    (2014, 'Report - Installation Hit Rate', 20, 'Installation Hit Rate Report')
END
