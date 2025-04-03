IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.columns WHERE table_name ='Country' AND column_name = 'ISOCountryCode')
    ALTER TABLE dbo.Country add ISOCountryCode CHAR(2) NULL
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.columns WHERE table_name ='LineItemAudit' AND column_name = 'RunNo')
    ALTER TABLE dbo.LineItemAudit add RunNo INT NULL
GO