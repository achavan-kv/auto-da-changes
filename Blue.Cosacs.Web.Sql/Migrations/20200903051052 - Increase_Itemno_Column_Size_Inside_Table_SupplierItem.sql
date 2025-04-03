-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


-- Script Comment	: Increase the Item no length size
-- Script Name		: AlterTableSupplierItem.sql
-- Created By		: Ashok 
-- Created On		: 03/07/2020


GO
		
ALTER TABLE SupplierItem ALTER COLUMN Itemno VARCHAR (18)

GO
