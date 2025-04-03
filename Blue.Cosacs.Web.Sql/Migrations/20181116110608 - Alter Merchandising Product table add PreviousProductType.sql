 --transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Product' AND  Column_Name = 'PreviousProductType'
           AND TABLE_SCHEMA = 'Merchandising')
BEGIN
	ALTER TABLE Merchandising.Product ADD PreviousProductType nvarchar(50) NULL
END
