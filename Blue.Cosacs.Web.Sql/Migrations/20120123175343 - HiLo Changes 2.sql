-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
DROP TABLE dbo.hibernate_unique_key
GO
DROP TABLE dbo.HiLo
GO
CREATE TABLE dbo.HiLo
(
	Sequence varchar(128) NOT NULL,
	NextHi int NOT NULL,
	MaxLo int NOT NULL
)
GO
ALTER TABLE dbo.HiLo ADD CONSTRAINT
	PK_HiLo PRIMARY KEY CLUSTERED 
	(
	Sequence
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

