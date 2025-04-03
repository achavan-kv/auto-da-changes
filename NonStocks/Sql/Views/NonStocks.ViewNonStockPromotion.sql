IF OBJECT_ID('NonStocks.ViewNonStockPromotion') IS NOT NULL
	DROP VIEW NonStocks.ViewNonStockPromotion
GO

CREATE VIEW NonStocks.ViewNonStockPromotion
AS
	SELECT
		p.Id,
		p.Fascia,
		ISNULL(CONVERT(VarChar, p.BranchNumber), '') AS BranchNumber,
		n.Id AS NonStockId,
		n.SKU AS NonStockNumber,
        n.ShortDescription,
        n.LongDescription,
		p.StartDate,
		p.EndDate,
		p.PercentageDiscount,
		p.RetailPrice
	FROM
		NonStocks.NonStockPromotion p
		INNER JOIN NonStocks.NonStock n
			ON p.NonStockId = n.Id
