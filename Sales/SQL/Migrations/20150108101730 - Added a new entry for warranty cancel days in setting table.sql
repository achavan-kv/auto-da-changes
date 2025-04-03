-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
DECLARE @WarrantyCancelDays int


SELECT @WarrantyCancelDays = Value FROM CountryMaintenance WHERE CodeName = 'warrantycanceldays'

IF EXISTS(SELECT s.Id FROM Config.Setting s 
			WHERE s.Id = 'WarrantyCancelDays' AND s.Namespace = 'Blue.Cosacs.Sales') BEGIN
	UPDATE Config.Setting
	SET ValueInt = @WarrantyCancelDays
	WHERE Id = 'WarrantyCancelDays' AND [Namespace] = 'Blue.Cosacs.Sales'
END
ELSE BEGIN
	INSERT INTO Config.Setting ([Namespace], Id, ValueInt)
	VALUES ('Blue.Cosacs.Sales', 'WarrantyCancelDays', @WarrantyCancelDays)

END
