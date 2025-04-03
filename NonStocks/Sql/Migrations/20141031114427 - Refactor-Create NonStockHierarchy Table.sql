-- transaction: true


-- Drop old FOREIGN KEY CONSTRAINT'S
IF OBJECT_ID('[NonStocks].[FK_NonStocks_Division]') IS NOT NULL
    ALTER TABLE [NonStocks].[HierarchyDepartment] DROP CONSTRAINT FK_NonStocks_Division

IF OBJECT_ID('[NonStocks].[FK_NonStocks_Department]') IS NOT NULL
    ALTER TABLE [NonStocks].[HierarchyClass] DROP CONSTRAINT FK_NonStocks_Department
-- Drop old FOREIGN KEY CONSTRAINT'S



-- Drop old Hierarchy Tables
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'HierarchyDivision' AND TABLE_SCHEMA = 'NonStocks')
    DROP TABLE [NonStocks].[HierarchyDivision]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'HierarchyDepartment' AND TABLE_SCHEMA = 'NonStocks')
    DROP TABLE [NonStocks].[HierarchyDepartment]

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'HierarchyClass' AND TABLE_SCHEMA = 'NonStocks')
    DROP TABLE [NonStocks].[HierarchyClass]
-- Drop old Hierarchy Tables



-- Create new NonStockHierarchy Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'NonStockHierarchy' AND TABLE_SCHEMA = 'NonStocks')
    CREATE TABLE NonStocks.NonStockHierarchy
    (
        Id INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_NonStocks_NonStockHierarchy_Id] PRIMARY KEY,
        NonStockId INT NOT NULL CONSTRAINT [FK_NonStocks_Hierarchy_NonStockHierarchy_Id] FOREIGN KEY REFERENCES [NonStocks].[NonStock] (Id),
        [Level] TINYINT NOT NULL,
        LevelKey VARCHAR(100) NOT NULL,
        LevelName VARCHAR(100) NOT NULL
    )
-- Create new NonStockHierarchy Table



-- Drop columns Division, DepartmentId and ClassId on Table NonStock
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND  Column_Name = 'Division' AND TABLE_SCHEMA = 'NonStocks')
    ALTER TABLE [NonStocks].[NonStock] DROP COLUMN Division

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND  Column_Name = 'DepartmentId' AND TABLE_SCHEMA = 'NonStocks')
    ALTER TABLE [NonStocks].[NonStock] DROP COLUMN DepartmentId

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'NonStock' AND  Column_Name = 'ClassId' AND TABLE_SCHEMA = 'NonStocks')
    ALTER TABLE [NonStocks].[NonStock] DROP COLUMN ClassId
-- Drop columns Division, DepartmentId and ClassId on Table NonStock
