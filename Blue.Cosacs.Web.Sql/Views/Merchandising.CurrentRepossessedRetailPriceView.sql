IF OBJECT_ID('Merchandising.CurrentRepossessedRetailPriceView') IS NOT NULL
	DROP VIEW Merchandising.CurrentRepossessedRetailPriceView
GO

CREATE VIEW [Merchandising].[CurrentRepossessedRetailPriceView]
AS

WITH Tax(ProductId, LocationId, Fascia, EffectiveDate)
AS (
	SELECT p.ProductId, 
           p.LocationId, 
           p.Fascia, 
           MAX(EffectiveDate) AS EffectiveDate
		   FROM [Merchandising].[RepossessedPriceView] p
		   WHERE EffectiveDate <= GETDATE()
           GROUP BY p.ProductId, 
               p.LocationId, 
               p.Fascia
   )

SELECT Id, LocationId, ProductId, EffectiveDate, RegularPrice, CashPrice, DutyFreePrice, Fascia, TaxRate, Name
FROM (
	SELECT
		rp.ProductId as Id,
		rp.LocationId,
		rp.ProductId,
		rp.EffectiveDate,
		rp.RegularPrice,
		rp.CashPrice,
		rp.DutyFreePrice,
		rp.Fascia,
		rp.TaxRate AS TaxRate,
		rp.Name
	FROM Merchandising.[RepossessedPriceView] rp
	INNER JOIN Tax 
	ON Tax.ProductId = rp.ProductId
	AND 
	(
		Tax.LocationId = rp.LocationId 
		OR Tax.LocationId IS NULL
	)
	AND ISNULL(Tax.Fascia, '') = ISNULL(rp.Fascia, '')
	AND Tax.EffectiveDate = rp.EffectiveDate
	AND Tax.EffectiveDate = Tax.EffectiveDate
	LEFT JOIN Merchandising.Location l
		ON rp.LocationId = l.id
) as RPView


GO

