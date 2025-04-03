-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- *************************************************************************************************
-- Developer:	Amit
-- Date:		20 August 2020
-- Purpose:		Created table to capture feature details like ActivationDate etc..
-- *************************************************************************************************

GO

IF NOT EXISTS (SELECT	1 
				FROM	sys.objects WITH (NOLOCK)
				WHERE	object_id = OBJECT_ID(N'[FeatureConfiguration]') 
						AND type in (N'U')
				)
BEGIN

	CREATE TABLE dbo.FeatureConfiguration
	(
		Id int NOT NULL,
		FeatureName varchar(100) NOT NULL,
		ActivationDate datetime NOT NULL
	)  ON [PRIMARY]
	ALTER TABLE dbo.FeatureConfiguration ADD CONSTRAINT
	PK_FeatureConfiguration PRIMARY KEY CLUSTERED 
	(
		Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


END

GO