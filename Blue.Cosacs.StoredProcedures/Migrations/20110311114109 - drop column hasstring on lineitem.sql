-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'lineitem'
           AND    Column_Name = 'hasstring')
           BEGIN
           
				exec drop_defaultsp  'hasstring', 'lineitem'
           		ALTER TABLE lineitem DROP COLUMN hasstring
           END
