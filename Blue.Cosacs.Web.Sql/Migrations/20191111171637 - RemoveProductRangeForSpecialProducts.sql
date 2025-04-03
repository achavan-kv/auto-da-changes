IF EXISTS (SELECT * FROM SYS.OBJECTS AS o JOIN SYS.SCHEMAS S ON O.schema_id = S.schema_id WHERE O.NAME  = 'RemoveProductRangeForSpecialProducts' AND S.NAME = 'Merchandising')
	DROP TRIGGER [Merchandising].[RemoveProductRangeForSpecialProducts] 
GO

CREATE TRIGGER [Merchandising].[RemoveProductRangeForSpecialProducts] on[Merchandising].[ProductAttributes] 
FOR UPDATE,INSERT 
AS 

IF EXISTS (SELECT * FROM INSERTED)
BEGIN
	DECLARE  @IsSpecialProduct BIT, @ProductId INT
	SELECT @IsSpecialProduct = IsSpecialProduct, @ProductId = ProductId FROM INSERTED
	IF (@IsSpecialProduct = 1)
	BEGIN
		IF EXISTS (SELECT *  FROM [Merchandising].[ProductStockRanges] WHERE ProductId = @ProductId)
		BEGIN
			DELETE FROM [Merchandising].[ProductStockRanges] WHERE ProductId = @ProductId
		END
	END
END




