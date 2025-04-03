IF OBJECT_ID('NonStocks.PricesView') IS NOT NULL
	DROP VIEW NonStocks.PricesView
GO

CREATE VIEW NonStocks.PricesView
AS
    SELECT
        price.Id,
        price.NonStockId,
        nonStock.[Type],
        price.Fascia,
        price.BranchNumber,
        price.CostPrice,
        price.RetailPrice,
        price.TaxInclusivePrice,
        price.DiscountValue,
        price.EffectiveDate,
        price.EndDate,
        COALESCE(
        (   SELECT count(1)
            FROM NonStocks.NonStockPromotion promo 
            WHERE price.NonStockId = promo.NonStockId
                AND price.EffectiveDate BETWEEN promo.StartDate AND promo.EndDate
        ), 0) AS HasPromotion
    FROM NonStocks.NonStockPrice price
    INNER JOIN NonStocks.NonStock nonStock
        ON price.NonStockId = nonStock.Id

