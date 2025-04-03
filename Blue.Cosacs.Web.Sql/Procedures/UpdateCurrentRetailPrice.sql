SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'UpdateCurrentRetailPrice'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE [warranty].[UpdateCurrentRetailPrice]
END
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
Create PROCEDURE  [warranty].[UpdateCurrentRetailPrice]
As

Truncate TABLE [Warranty].CurrentRetailPriceView1 
INSERT INTO [Warranty].CurrentRetailPriceView1
SELECT CONVERT(INT, ROW_NUMBER() OVER (ORDER BY productid DESC)) as Id, LocationId, ProductId, EffectiveDate, RegularPrice, CashPrice, DutyFreePrice, Fascia, TaxRate, Name 
FROM (
	SELECT LocationId, ProductId, EffectiveDate, RegularPrice, CashPrice, DutyFreePrice, Fascia, TaxRate, Name
	FROM Merchandising.CurrentStockRetailPriceView
	UNION
	SELECT LocationId, ProductId, EffectiveDate, RegularPrice, CashPrice, DutyFreePrice, Fascia, TaxRate, Name
	FROM Merchandising.[CurrentRepossessedRetailPriceView]
	UNION
	SELECT LocationId, ProductId, EffectiveDate, RegularPrice, CashPrice, DutyFreePrice, Fascia, TaxRate, Name
	FROM Merchandising.CurrentSetRetailPriceView
	UNION
	SELECT LocationId, ProductId, EffectiveDate, RegularPrice, CashPrice, DutyFreePrice, Fascia, TaxRate, Name
	FROM Merchandising.CurrentComboRetailPriceView
) as RPVIEW
