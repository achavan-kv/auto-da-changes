IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[StockProductCountView]'))
DROP VIEW  Merchandising.StockProductCountView
GO