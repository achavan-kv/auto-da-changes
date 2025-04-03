-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE dbo.HiLo
(
	[Namespace] varchar(128) NOT NULL,
	NextHi int NOT NULL
) 
GO
ALTER TABLE dbo.HiLo ADD CONSTRAINT
PK_HiLo PRIMARY KEY CLUSTERED 
(
[Namespace]
) 
GO