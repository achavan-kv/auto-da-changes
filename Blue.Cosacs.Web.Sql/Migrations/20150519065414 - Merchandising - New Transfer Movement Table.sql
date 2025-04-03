-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Merchandising.StockTransferMovement
(
	Id int identity(1,1),
	BookingId int,
	ProductId int,
	SendingLocationId int,
	ReceivingLocationId int,
	[Type] varchar(12),
	Quantity int,
	DateProcessed datetime

CONSTRAINT PK_StockTransferMovement PRIMARY KEY (id),
CONSTRAINT FK_StockTransferMovement_ProductId FOREIGN KEY (ProductId) REFERENCES Merchandising.Product (id),
CONSTRAINT FK_StockTransferMovement_SendingLocationId FOREIGN KEY (SendingLocationId) REFERENCES Merchandising.Location (id),
CONSTRAINT FK_StockTransferMovement_ReceivingLocationId FOREIGN KEY (ReceivingLocationId) REFERENCES Merchandising.Location (id)
)