-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'LimitPrice' AND [object_id] = OBJECT_ID(N'Sales.DiscountLimit'))BEGIN

	EXECUTE sp_rename N'Sales.Discountlimit.LimitPrice', N'Tmp_limitValue', 'COLUMN'

	EXECUTE sp_rename N'Sales.Discountlimit.Tmp_limitValue', N'LimitPercentage', 'COLUMN'

END
GO
