IF OBJECT_ID('Merchandising.CurrentComboRetailPriceView') IS NOT NULL
	DROP VIEW [Merchandising].[CurrentComboRetailPriceView]
GO 

CREATE VIEW [Merchandising].[CurrentComboRetailPriceView]
AS

SELECT
	CONVERT(Int,ROW_NUMBER() OVER (ORDER BY productid DESC)) as Id,
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
		c.LocationId,
		c.ComboId as ProductId,
		CONVERT(date, co.StartDate) as EffectiveDate,
		SUM(regularPrice * quantity) as RegularPrice,
		SUM(cashprice * quantity) as CashPrice,
		SUM(dutyfreeprice * quantity) as DutyFreePrice,
		c.Fascia,
		(
			(SUM(cashprice * quantity * (1 + c.TaxRate)) - SUM(cashprice * quantity))
			/ CASE SUM(cashprice * quantity)
				WHEN 0 THEN 1
				ELSE SUM(cashprice * quantity)
			END
			) as TaxRate,
		l.Name as Name
	FROM Merchandising.ComboPriceView c
	INNER JOIN Merchandising.Combo co
		ON co.Id = c.ComboId
	INNER JOIN Merchandising.Product p
		ON p.id = c.comboid
	LEFT JOIN Merchandising.Location l
		ON l.id = c.LocationId
	GROUP BY
		c.LocationId,
		c.ComboId,
		c.Fascia,
		l.Name,
		co.StartDate
) as RPView

GO


