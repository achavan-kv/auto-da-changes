-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT 1 FROM sys.schemas s where s.name = 'Stock')
	EXEC sp_executesql N'CREATE SCHEMA Stock'
GO

if OBJECT_ID('[Merchandising].[Branch]') IS NOT NULL
	DROP VIEW [Merchandising].[Branch]

if OBJECT_ID('[Merchandising].[StockItemView]') IS NOT NULL
	DROP VIEW [Merchandising].[StockItemView]

if OBJECT_ID('[Merchandising].[StockItemViewRelations]') IS NOT NULL
	DROP VIEW [Merchandising].[StockItemViewRelations]


if OBJECT_ID('[Merchandising].[StockProductCountView]') IS NOT NULL
	DROP VIEW [Merchandising].[StockProductCountView]

if OBJECT_ID('[Merchandising].[ProductLinkValidateView]') IS NOT NULL
	DROP VIEW [Merchandising].[ProductLinkValidateView]
	
if OBJECT_ID('[Merchandising].[InstallationItemsView]') IS NOT NULL
	DROP VIEW [Merchandising].[InstallationItemsView]