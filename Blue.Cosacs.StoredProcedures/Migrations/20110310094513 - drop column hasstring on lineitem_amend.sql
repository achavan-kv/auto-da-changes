-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--Comments: a previous migration dropped column hasstring from lineitem, therefore need to drop this from lineitem_amend

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'lineitem_amend'
           AND    Column_Name = 'hasstring')
BEGIN  
	ALTER TABLE lineitem_amend DROP COLUMN hasstring
END
GO 