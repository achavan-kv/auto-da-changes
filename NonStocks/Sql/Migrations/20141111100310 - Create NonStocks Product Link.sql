-- transaction: true

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA = 'NonStocks' AND Table_Name = 'Link')
BEGIN
    CREATE TABLE NonStocks.Link
    (
        Id INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_NonStocks_Link_Id PRIMARY KEY,
        Name VARCHAR(100) NOT NULL,
        EffectiveDate DATE NOT NULL
    )
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA = 'NonStocks' AND Table_Name = 'LinkProduct')
BEGIN
    CREATE TABLE NonStocks.LinkProduct
    (
        Id INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_NonStocks_LinkProduct_Id PRIMARY KEY,
        LinkId INT NOT NULL CONSTRAINT FK_NonStocks_LinkProduct_LinkId FOREIGN KEY REFERENCES NonStocks.Link(Id),
        Level_1 VARCHAR(100) NULL,
        Level_2 VARCHAR(100) NULL,
        Level_3 VARCHAR(100) NULL,
        Level_4 VARCHAR(100) NULL,
        Level_5 VARCHAR(100) NULL
    )
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_SCHEMA = 'NonStocks' AND Table_Name = 'LinkNonStock')
BEGIN
    CREATE TABLE NonStocks.LinkNonStock
    (
        Id INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_NonStocks_LinkNonStock_Id PRIMARY KEY,
        LinkId INT NOT NULL CONSTRAINT FK_NonStocks_LinkNonStock_LinkId FOREIGN KEY REFERENCES NonStocks.Link(Id),
        NonStockId INT NOT NULL CONSTRAINT FK_NonStocks_LinkNonStock_NonStockId FOREIGN KEY REFERENCES NonStocks.NonStock(Id)
    )
END
GO

