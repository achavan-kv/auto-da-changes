CREATE TABLE Warehouse.Cancellation
(
	Id INT NOT NULL,
	UserId int NOT NULL,
	Date DATETIME NOT NULL,
	Reason VARCHAR(4000) NULL
)

ALTER TABLE Warehouse.cancellation
ADD CONSTRAINT PK_WarehouseCancellation PRIMARY KEY CLUSTERED (id)

ALTER TABLE Warehouse.Cancellation
ADD CONSTRAINT FK_CancelId FOREIGN KEY (id) REFERENCES warehouse.booking(Id)
