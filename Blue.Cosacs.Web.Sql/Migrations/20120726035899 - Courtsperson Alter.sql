
IF exists (SELECT * FROM syscolumns
				WHERE name = 'firstname'
				AND OBJECT_NAME(id) = 'courtsperson')
BEGIN
EXECUTE sp_rename N'dbo.courtsperson.empeeno', N'Tmp_UserId', 'COLUMN' 
END
GO

IF exists (SELECT * FROM syscolumns
				WHERE name = 'firstname'
				AND OBJECT_NAME(id) = 'courtsperson')
BEGIN
EXECUTE sp_rename N'dbo.courtsperson.Tmp_UserId', N'UserId', 'COLUMN'
END
GO

IF exists (SELECT * FROM syscolumns
				WHERE name = 'firstname'
				AND OBJECT_NAME(id) = 'courtsperson')
BEGIN
DECLARE @name NVARCHAR(1000)

SET @Name = (SELECT TOP 1 name FROM sysobjects
			WHERE xtype = 'D'
			AND OBJECT_NAME(parent_obj) = 'courtsperson'
			AND NAME LIKE '%logge%')

WHILE LEN(@name) > 0
BEGIN 

	SET @name = 'ALTER TABLE Courtsperson DROP CONSTRAINT ' + @name
    EXEC sp_executesql @name
    
SET @Name = (SELECT TOP 1 name FROM sysobjects
			WHERE xtype = 'D'
			AND OBJECT_NAME(parent_obj) = 'courtsperson'
			AND NAME LIKE '%logge%')
END
END
GO

IF exists (SELECT * FROM syscolumns
				WHERE name = 'firstname'
				AND OBJECT_NAME(id) = 'courtsperson')
BEGIN
ALTER TABLE dbo.courtsperson
	DROP COLUMN empeename, origbr, branchno, empeetype, password, datepasschge, firstname, lastname
END 
GO

IF NOT exists (sELECT * FROM sysobjects
				WHERE name = 'FK_CourtsPerson_User'
				AND xtype = 'F')
BEGIN
ALTER TABLE Courtsperson  WITH CHECK 
ADD  CONSTRAINT [FK_CourtsPerson_User] FOREIGN KEY(UserId)
REFERENCES [Admin].[User] ([Id])
END
GO

ALTER TABLE Courtsperson CHECK CONSTRAINT [FK_CourtsPerson_User]
GO

