-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE Credit.StoreCardImport
(
    Id INT IDENTITY,
    CardNumber BIGINT NOT NULL,
    Amount SMALLMONEY NOT NULL,
    TransactionDate SMALLDATETIME NOT NULL,
    ImportDate SMALLDATETIME NOT NULL,
    Imported BIT DEFAULT 0
);
GO

--Add primary key constraint
ALTER TABLE Credit.StoreCardImport
ADD CONSTRAINT PK_StoreCardImport PRIMARY KEY CLUSTERED (Id)
GO

--Add Unique constraint on transacions
ALTER TABLE Credit.StoreCardImport
ADD CONSTRAINT UQ_StoreCardImportTransaction UNIQUE (CardNumber, TransactionDate)
GO