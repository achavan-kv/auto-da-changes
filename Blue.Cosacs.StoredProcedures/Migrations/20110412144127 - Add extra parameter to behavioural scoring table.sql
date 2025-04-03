-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE column_name = 'CustomerScoreHist' AND table_name ='Applied')
ALTER TABLE CustomerScoreHist ADD Applied TINYINT 
GO 
UPDATE CustomerScoreHist SET applied = 1