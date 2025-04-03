SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'SetCurrentStockRetailPrice'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE [Merchandising].[SetCurrentStockRetailPrice]
END
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE  PROCEDURE  [Merchandising].[SetCurrentStockRetailPrice]
As

TRUNCATE TABLE [Merchandising].[CurrentStockRetailPriceView1]

	SELECT distinct t.EffectiveDate, t.ProductId, t.Rate, ROW_NUMBER () OVER (PARTITION BY t.ProductId ORDER BY t.EffectiveDate desc, Id desc) [Row]
	INTO #Temp FROM Merchandising.TaxRate t
	WHERE EffectiveDate <= GETDATE()

INSERT INTO [Merchandising].[CurrentStockRetailPriceView1]
SELECT 
	Id, 
	LocationId, 
	ProductId, 
	EffectiveDate, 
	RegularPrice, 
	CashPrice, 
	DutyFreePrice, 
	Fascia, 
	TaxRate, 
	Name 
FROM (
	SELECT
		rp.Id,
		rp.LocationId,
		rp.ProductId,
		rp.EffectiveDate,
		rp.RegularPrice,
		rp.CashPrice,
		rp.DutyFreePrice,
		rp.Fascia,
		COALESCE(tId.Rate, mt.Rate, 0) AS TaxRate,
		l.Name
	FROM [Merchandising].[RetailPrice] rp With(NoLock)
	INNER JOIN Merchandising.Product p With(NoLock)
		ON p.Id = rp.ProductId
		AND p.ProductType not in ('Combo', 'Set')
	INNER JOIN (
		SELECT Productid, Locationid, Fascia, max(EffectiveDate) as EffectiveDate
		FROM [Merchandising].retailprice p
		WHERE EffectiveDate <= GETDATE()
		GROUP BY Productid, Locationid, Fascia
	) AS CurrentPrice
		ON CurrentPrice.ProductId = rp.ProductId
		AND ISNULL(CurrentPrice.LocationId, 0) = ISNULL(rp.LocationId, 0)
		AND ISNULL(CurrentPrice.Fascia, 'none') = ISNULL(rp.Fascia, 'none')
		AND CurrentPrice.EffectiveDate = rp.EffectiveDate
	LEFT JOIN [Merchandising].[Location] l With(NoLock)
		ON rp.LocationId = l.id
	LEFT JOIN #Temp AS tId With(NoLock) ON tId.ProductId = p.Id AND tId.[row] = 1
    LEFT JOIN #Temp AS mt  With(NoLock) ON  mt.ProductId IS NULL  AND mt.[Row] = 1
) as RPView