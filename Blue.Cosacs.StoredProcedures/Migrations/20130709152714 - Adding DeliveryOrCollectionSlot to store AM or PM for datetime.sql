-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- Related to issue: #14601


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Booking' AND  Column_Name = 'DeliveryOrCollectionSlot'
           AND TABLE_SCHEMA = 'Warehouse')
BEGIN

	ALTER TABLE Warehouse.Booking ADD DeliveryOrCollectionSlot char(2) NOT NULL DEFAULT ''

END

go

if((select DATA_TYPE from INFORMATION_SCHEMA.COLUMNS IC where TABLE_NAME = 'Booking' and COLUMN_NAME = 'DeliveryOrCollectionDate') = 'datetime')
BEGIN
	update warehouse.booking
	set DeliveryOrCollectionSlot = case when datepart(hh, DeliveryOrCollectionDate) = '12' then 'PM' else 'AM' end
	from warehouse.booking
END


