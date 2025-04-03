IF OBJECT_ID('NonStocks.NonStockPromotionsView') IS NOT NULL
	DROP VIEW NonStocks.NonStockPromotionsView
GO

CREATE VIEW NonStocks.NonStockPromotionsView
AS
	SELECT 
		Data.StartDate,
		Data.EndDate,
		Data.PercentageDiscount,
		Data.RetailPrice,
		Data.BranchNumber,
		'' AS BranchName,
		Data.Fascia,
		Data.NonStockId,
		n.SKU AS NonStockNumber,
		Data.NonStockPriceId,
		Data.EffectiveDate AS NonStockPriceEffectiveDate,
		ROW_NUMBER() OVER(ORDER BY Data.NonStockPriceId) As RowKey,
		Data.NonStockPromotionId
	FROM 
	(
		SELECT 
			NP.StartDate,
			NP.EndDate,
			NP.PercentageDiscount,
			NP.RetailPrice,
			NPr.BranchNumber,
			Npr.Fascia,
			NPr.NonStockId,
			NPr.EffectiveDate AS NonStockPriceEffectiveDate,
			NPr.Id AS NonStockPriceId,
			NPr.EffectiveDate,
			Np.Id AS NonStockPromotionId
		FROM
			NonStockS.NonStockPrice Npr
			INNER JOIN NonStockS.NonStockPromotion Np
				ON Npr.NonStockId = Np.NonStockId
				AND COALESCE(NPr.Fascia, NP.Fascia, 'ALL') = COALESCE(NP.Fascia, NPr.Fascia, 'ALL')
				AND COALESCE(NPr.BranchNumber, NP.BranchNumber, -1) = COALESCE(NP.BranchNumber, NPr.BranchNumber, -1)
		WHERE
			Np.EndDate >= NPr.EffectiveDate
	) AS Data
		INNER JOIN NonStocks.NonStock n
			ON Data.NonStockId = n.id
