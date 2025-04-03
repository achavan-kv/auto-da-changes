-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #12856

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Booking' AND  Column_Name = 'DeliveryConfirmedDate'
           AND TABLE_SCHEMA = 'Warehouse')
BEGIN

	ALTER TABLE Warehouse.Booking ADD DeliveryConfirmedDate datetime null

END
