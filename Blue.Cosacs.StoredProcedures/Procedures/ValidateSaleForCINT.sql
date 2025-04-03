SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[Merchandising].[ValidateSaleForCINT]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [Merchandising].[ValidateSaleForCINT]
GO

-- =============================================
-- Author:		Rahul D, Zensar
-- Create date: 21/11/2019
-- Description:	Validate Sales details For CINT validation, to aviod PSNs in HUB queue 200
-- =============================================
CREATE PROCEDURE [Merchandising].[ValidateSaleForCINT]
	-- Add the parameters for the stored procedure here
	@Sku VARCHAR(18)
	,@Quantity INT
	,@SaleLocation INT
	,@StockLocation INT
	,@TransactionDate DATE
	,@ErrorMessage VARCHAR(5000) OUT
	,@ErrorCount INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	SET @ErrorMessage = '';
	SET @ErrorCount = 0

	IF EXISTS (
			SELECT 1
			FROM StockInfo
			WHERE sku = @sku
				AND itemtype = 'S' and prodstatus != 'D'
			)
	BEGIN
		IF NOT EXISTS (
				SELECT 1
				FROM Merchandising.Product
				WHERE SKU = @Sku
				)
		BEGIN
			--IF Product not found in Merch Product
			SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. The item number '+ @Sku + 'is not available in Merchandising' + '\n'
			SET @ErrorCount = @ErrorCount + 1
		END
		ELSE
		BEGIN
			--IF Product found in Merch Product
			IF NOT EXISTS (
					SELECT 1
					FROM Merchandising.CurrentCostPriceView
					WHERE ProductId = (
							SELECT ID
							FROM Merchandising.Product
							WHERE SKU = @Sku
							)
					)
			BEGIN
				--IF Product not found in Merch CostPrice
				SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. The Cost Price is not avaialble in Merchandising for itemno ' + @Sku + '\n'
				SET @ErrorCount = @ErrorCount + 1
			END

			IF NOT EXISTS (
					SELECT 1
					FROM Merchandising.CurrentRetailPriceView
					WHERE ProductId = (
							SELECT ID
							FROM Merchandising.Product
							WHERE SKU = @Sku
							)
					)
			BEGIN
				--IF Product not found in Merch Retail Price
				SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. The Retail Price is not avaialble in Merchandising for itemno ' + @Sku + '\n'
				SET @ErrorCount = @ErrorCount + 1
			END
		END

		IF (@Quantity <= 0)
		BEGIN
			SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. Quantity could not be less than or equal to 0 for itemno ' + @Sku + '\n'
			SET @ErrorCount = @ErrorCount + 1
		END

		IF NOT EXISTS (
				SELECT 1
				FROM Merchandising.Location
				WHERE SalesId = @SaleLocation
				)
		BEGIN
			--IF Sales Location not found in Merch Location
			SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. The Sales Location not available in Merchandising for itemno ' + @Sku + '\n'
			SET @ErrorCount = @ErrorCount + 1
		END

		IF NOT EXISTS (
				SELECT 1
				FROM Merchandising.Location
				WHERE SalesId = @StockLocation
				)
		BEGIN
			--IF Stock Location not found in Merch Location
			SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. The Stock Location not available in Merchandising for itemno ' + @Sku + '\n'
			SET @ErrorCount = @ErrorCount + 1
		END

		IF ((CAST(@TransactionDate AS DATE) > (CAST(GETUTCDATE() AS DATE))))
		BEGIN
			--IF Transaction date is future date
			SET @ErrorMessage = @ErrorMessage + CAST(@ErrorCount + 1 AS VARCHAR) + '. Transaction date could not be a future date for itemno ' + @Sku + '\n'
			SET @ErrorCount = @ErrorCount + 1
		END
	END
END
GO

