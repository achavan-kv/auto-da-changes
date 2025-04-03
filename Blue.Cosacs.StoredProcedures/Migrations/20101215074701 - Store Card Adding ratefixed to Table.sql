-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='storecardrate'
AND column_name = 'ratefixed')
BEGIN
	ALTER TABLE storecardrate ADD ratefixed BIT 
END
GO
IF EXISTS (SELECT * FROM dbo.StoreCardRate WHERE ratefixed IS NULL)
UPDATE dbo.StoreCardRate SET ratefixed = 0 WHERE ratefixed IS NULL 
