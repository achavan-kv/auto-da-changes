-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN BookingId int not null

ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN 	ProductId int not null

ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN 	SendingLocationId int not null

ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN 	ReceivingLocationId int not null

ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN 	[Type] varchar(12) not null

ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN 	Quantity int not null

ALTER TABLE Merchandising.StockTransferMovement
Alter COLUMN 	DateProcessed datetime not null
