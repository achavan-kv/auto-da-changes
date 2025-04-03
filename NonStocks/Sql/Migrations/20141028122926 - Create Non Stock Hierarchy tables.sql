-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'HierarchyDivision'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE NonStocks.HierarchyDivision
    (
        Division varchar(100) NOT NULL PRIMARY KEY
    )
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'HierarchyDepartment'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE NonStocks.HierarchyDepartment
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Division varchar(100) NOT NULL,
        [Description] varchar(100) NOT NULL,
        CONSTRAINT [FK_NonStocks_Division] FOREIGN KEY ([Division]) REFERENCES [NonStocks].[HierarchyDivision] ([Division])
    )
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'HierarchyClass'
               AND TABLE_SCHEMA = 'NonStocks')
BEGIN
    CREATE TABLE NonStocks.HierarchyClass
    (
        Id int IDENTITY(1,1) PRIMARY KEY,
        DepartmentId int NOT NULL,
        [Description] varchar(100) NOT NULL,
        CONSTRAINT [FK_NonStocks_Department] FOREIGN KEY ([DepartmentId]) REFERENCES [NonStocks].[HierarchyDepartment] (Id)
    )
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND  Column_Name = 'Division'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock ADD Division varchar(100) NOT NULL
END
GO


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND  Column_Name = 'DepartmentId'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock ADD DepartmentId int NOT NULL
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND  Column_Name = 'ClassId'
           AND TABLE_SCHEMA = 'NonStocks')
BEGIN
	ALTER TABLE NonStocks.NonStock ADD ClassId int NOT NULL
END
GO