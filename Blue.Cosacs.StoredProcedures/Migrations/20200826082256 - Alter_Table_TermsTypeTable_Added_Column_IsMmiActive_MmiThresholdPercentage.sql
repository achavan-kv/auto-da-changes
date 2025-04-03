-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF NOT EXISTS (SELECT * FROM  sys.columns WHERE  object_id = OBJECT_ID(N'IsMmiActive') AND name = 'TermsTypeTable')
BEGIN

	ALTER TABLE dbo.TermsTypeTable ADD
	IsMmiActive BIT NOT NULL CONSTRAINT DF_TermsTypeTable_IsMmiActive DEFAULT 0,
	MmiThresholdPercentage FLOAT(53) NOT NULL CONSTRAINT DF_TermsTypeTable_MmiThresholdPercentage DEFAULT 0

END

