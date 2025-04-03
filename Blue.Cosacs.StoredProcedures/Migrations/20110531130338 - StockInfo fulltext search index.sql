-- transaction: false
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT 1 FROM  sysfulltextcatalogs WHERE NAME = 'StockCatalog')
CREATE FULLTEXT CATALOG StockCatalog
WITH ACCENT_SENSITIVITY = OFF
GO 

IF NOT EXISTS(SELECT 1 FROM sysobjects s, sysobjects p -- p for parent
		      WHERE p.id = s.parent_obj AND s.TYPE = 'IT' AND p.NAME = 'StockInfo')
CREATE FULLTEXT INDEX ON StockInfo(itemno, itemdescr1, itemdescr2, ItemPOSDescr)
KEY INDEX PK_StockInfo 
ON StockCatalog
GO 
