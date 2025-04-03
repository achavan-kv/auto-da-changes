IF OBJECT_ID('Merchandising.CurrentSetRetailPriceView') IS NOT NULL
	DROP VIEW [Merchandising].[CurrentSetRetailPriceView]
GO 

CREATE VIEW [Merchandising].[CurrentSetRetailPriceView]
AS

SELECT
	CONVERT(INT, ROW_NUMBER() OVER (ORDER BY productid DESC)) as Id,
	LocationId,
	ProductId,
	EffectiveDate,
	RegularPrice,
	CashPrice,
	DutyFreePrice,
	Fascia,
	TaxRate,
	Name
FROM
(
	SELECT
		s.LocationId,
		s.SetId as ProductId,
		s.EffectiveDate as EffectiveDate,
		s.regularPrice, s.CashPrice, s.DutyFreePrice,
		s.Fascia,
		SUM(CAST(sp.cashprice * sp.quantity * sp.TaxRate AS DECIMAL(38,18))) /
			CASE SUM(sp.cashprice * sp.quantity)
				WHEN 0 THEN 1
				ELSE SUM(sp.cashprice * sp.quantity)
			END
		as TaxRate,
		l.Name as Name
	FROM Merchandising.SetLocationView s
	INNER JOIN Merchandising.Setproductview sp
		ON sp.SetId = s.SetId
		AND ISNULL(sp.LocationId, '') = ISNULL(s.LocationId, '')
		AND ISNULL(sp.Fascia, '') = ISNULL(s.Fascia, '')
	INNER JOIN merchandising.CurrentStockRetailPriceView csp
		on csp.EffectiveDate = sp.PriceEffectiveDate
		AND csp.ProductId = sp.ProductId
		AND ISNULL(csp.LocationId, '') = ISNULL(sp.LocationId, '')
		AND ISNULL(csp.Fascia, '') = ISNULL(sp.Fascia, '')
	INNER JOIN Merchandising.Product p
		ON p.id = s.SetId
	INNER JOIN
	(
		SELECT
			SetId,
			Locationid,
			Fascia,
			MAX(EffectiveDate) as EffectiveDate
		FROM [Merchandising].SetLocationView p
		WHERE EffectiveDate <= GETDATE()
		GROUP BY
			Setid,
			Locationid,
			Fascia
	) AS CurrentPrice
		ON CurrentPrice.SetId = s.SetId
		AND
		(
			CurrentPrice.LocationId = s.LocationId
			OR CurrentPrice.LocationId IS NULL
		)
		AND
		(
			CurrentPrice.Fascia = s.Fascia
			OR CurrentPrice.Fascia IS NULL
		)
		AND CurrentPrice.EffectiveDate = s.EffectiveDate
	LEFT JOIN Merchandising.Location l
		ON l.id = s.LocationId
	GROUP BY
		s.LocationId,
		s.SetId,
		s.regularPrice,
		s.CashPrice,
		s.DutyFreePrice,
		s.Fascia,
		l.Name,
		s.Effectivedate
) AS rp

GO


