DECLARE @TaxType nvarchar(256)
DECLARE @TaxRate decimal

SELECT @TaxType = Value FROM CountryMaintenance WHERE CodeName = 'taxtype'

SELECT @TaxRate = CAST(ISNULL(Value, 0) as decimal(28,12)) FROM CountryMaintenance WHERE CodeName = 'taxrate'

IF EXISTS(SELECT s.Id FROM Config.Setting s 
			WHERE s.Id = 'TaxRate' AND s.Namespace = 'Blue.Cosacs.Sales') BEGIN
	UPDATE Config.Setting
	SET ValueDecimal = @TaxRate
	WHERE Id = 'TaxRate' AND [Namespace] = 'Blue.Cosacs.Sales'
END
ELSE BEGIN
	INSERT INTO Config.Setting ([Namespace], Id, ValueDecimal)
	VALUES ('Blue.Cosacs.Sales', 'TaxRate', @TaxRate)

END

IF EXISTS(SELECT s.Id FROM Config.Setting s 
			WHERE s.Id = 'TaxType' AND s.Namespace = 'Blue.Cosacs.Sales') BEGIN
	UPDATE Config.Setting
	SET ValueString = @TaxType
	WHERE Id = 'TaxType' AND [Namespace] = 'Blue.Cosacs.Sales'
END
ELSE BEGIN
	INSERT INTO Config.Setting ([Namespace], Id, ValueString)
	VALUES ('Blue.Cosacs.Sales', 'TaxType', @TaxType)
END