IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[BuyerSalesHistoryView]'))
	DROP VIEW [Merchandising].[BuyerSalesHistoryView]

/****** Object:  View [Merchandising].[BuyerSalesHistoryView]    Script Date: 11/10/2019 15:26:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Merchandising].[BuyerSalesHistoryView]
AS

SELECT 
	MAX(sales.Id) as Id, 
	dates.[Year], 
	dates.NumericYear,
	DateName(Month, transactiondate) as [Month],
	DATEPART(month, transactiondate) as NumericMonth,
	sales.ProductId, 
	sales.SalesLocationId as LocationId,
	SUM(ISNULL(sales.Discount, 0)) as Discount, 
	SUM(ISNULL(sales.Tax, 0)) as Tax,
	SUM(ISNULL(sales.Quantity, 0)) as SalesVolume,
	SUM(ISNULL(sales.cashprice * CONVERT(decimal, sales.Quantity), 0)) as SalesValue,
	SUM(ISNULL(sales.AverageWeightedCost * CONVERT(decimal, sales.Quantity), 0)) as TotalCost
FROM [Merchandising].CintOrderCostView sales 
	INNER JOIN merchandising.buyersaleshistoryyearview dates
	ON sales.transactiondate BETWEEN dates.startdate AND dates.enddate
	AND sales.[Type] in ('Delivery', 'Return')
GROUP BY 	
	dates.[Year], 
	dates.NumericYear,
	DATENAME(Month, transactiondate),
	DATEPART(month, transactiondate) ,
	sales.productId, 
	sales.SalesLocationId


GO



GO


