
IF EXISTS (	SELECT	* 
			FROM	dbo.sysobjects 
			WHERE	id = object_id(N'[Merchandising].[ValidateProductSKU]')
					AND OBJECTPROPERTY(id, N'IsProcedure') = 1
			)
DROP PROCEDURE [Merchandising].[ValidateProductSKU] 
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Snehalata, Zensar
-- Create date: 28/01/2020
-- Description:	Duplicate SKU - Nonstock and Item Master create same SKU code
-- =============================================

CREATE PROCEDURE [Merchandising].[ValidateProductSKU] 
	 @ProductId INT ,
	 @SKU VARCHAR(18)
AS
BEGIN
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @Output VARCHAR(100)
	DECLARE @CountProduct INT = 0

	IF(@ProductId = 0)
		SELECT @CountProduct = COUNT(1) FROM Merchandising.Product WHERE SKU = @SKU
	ELSE  
		SELECT @CountProduct = COUNT(1) FROM Merchandising.Product WHERE SKU = @SKU AND Id <> @ProductId
	
	IF ( @CountProduct > 0) 
	BEGIN 
		SET @Output= 'Product already exists.'
	END
	ELSE IF EXISTS(SELECT Id FROM Nonstocks.NonStock WHERE SKU = @SKU)
	BEGIN
		SET @Output= 'Product SKU must be unique. Another Non product already exists with the same SKU.'
	END

	SELECT @Output

END

GO
