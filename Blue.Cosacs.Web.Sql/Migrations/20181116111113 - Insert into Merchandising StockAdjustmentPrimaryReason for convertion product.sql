


IF NOT EXISTS (SELECT 1 FROM Merchandising.StockAdjustmentPrimaryReason WHERE Id = 11)
BEGIN
	INSERT INTO Merchandising.StockAdjustmentPrimaryReason(Name)
	VALUES ('Conversion from Stock to Non-Stock')
END
