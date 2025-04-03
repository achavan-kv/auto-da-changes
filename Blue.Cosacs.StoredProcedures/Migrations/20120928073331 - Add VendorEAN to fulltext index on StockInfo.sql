-- transaction: false
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- #10393 - LW74980

DECLARE @value INT
SELECT @value = COLUMNPROPERTY(OBJECT_ID('dbo.stockinfo'), 'vendorean', 'IsFulltextIndexed')

IF(@value != 1)
BEGIN

	ALTER FULLTEXT INDEX ON [dbo].[StockInfo] ADD ([VendorEAN])

	ALTER FULLTEXT INDEX ON [dbo].[StockInfo] START FULL POPULATION
	
END

