IF EXISTS(
                     SELECT 1
                     FROM   SYS.PROCEDURES WITH (NOLOCK)
                     WHERE  NAME = 'ValidateNonSaleableNonStocks'
                                  AND TYPE = 'P'
              )
       DROP PROCEDURE [dbo].[ValidateNonSaleableNonStocks]

GO

-- ===================================================================================
-- SP Name:		ValidateNonSaleableNonStocks
-- Author:		Shubham Gaikwad
-- Create date: 03/02/2019
-- Description:	Procedure to get nonsalebale nonstock items for 'nonstock only' sale
-- ===================================================================================

CREATE PROCEDURE [dbo].[ValidateNonSaleableNonStocks]
@Sku varchar(18),
@return INT OUT
AS 
BEGIN
		SET @return = 0

		SELECT	SKU AS SKU
				,TYPE AS TYPE 
		FROM	[dbo].[NonSaleableNonStocks] where SKU = @Sku
END
