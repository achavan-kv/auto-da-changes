ALTER TABLE [Merchandising].[CintsExceptions] DROP CONSTRAINT [pk_Id_CintExceptions]
GO

ALTER TABLE [Merchandising].[CintsExceptions]
DROP COLUMN Id
GO

ALTER TABLE [Merchandising].[CintsExceptions] 
ADD CreatedOn DateTime NOT NULL,
    Runno INT NOT NULL
GO