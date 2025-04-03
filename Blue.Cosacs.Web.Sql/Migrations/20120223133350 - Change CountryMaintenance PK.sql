-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE dbo.CountryMaintenance
	DROP CONSTRAINT pk_CountryMaintenance
GO
ALTER TABLE dbo.CountryMaintenance ADD CONSTRAINT
	PK_CountryMaintenance PRIMARY KEY CLUSTERED 
	(
	ParameterID
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 90, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_CountryMaintenance_CodeName ON dbo.CountryMaintenance
	(
	CodeName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO