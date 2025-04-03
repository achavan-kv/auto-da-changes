IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'U'
		   AND so.NAME = 'PickingItems'
		   AND ss.name = 'Warehouse')
DROP TABLE  Warehouse.PickingItems
GO

IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'U'
		   AND so.NAME = 'PickingItem'
		   AND ss.name = 'Warehouse')
DROP TABLE  Warehouse.PickingItem
GO

IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'U'
		   AND so.NAME = 'Picking'
		   AND ss.name = 'Warehouse')
DROP TABLE  Warehouse.Picking
GO

IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'U'
		   AND so.NAME = 'Booking'
		   AND ss.name = 'Warehouse')
DROP TABLE  Warehouse.Booking
GO


CREATE TABLE Warehouse.Booking 
(
	Id INT NOT NULL,
	CustomerName VARCHAR(90) NOT NULL,
	AddressLine1 VARCHAR(50),
	AddressLine2 VARCHAR(50),
	AddressLine3 VARCHAR(50),
	PostCode VARCHAR(10),
	StockBranch SMALLINT NOT NULL,
	DeliveryBranch SMALLINT NOT NULL,
	DeliveryOrCollection CHAR(1) NOT NULL,
	DeliveryOrCollectionDate DATETIME NOT NULL,
	ItemNo VARCHAR(18) NOT NULL,
	ItemId INT  NOT NULL,
	ItemUPC VARCHAR(18) NOT NULL,
	ProductDescription VARCHAR(100) NOT NULL,
	ProductBrand VARCHAR(50) NULL,
	ProductModel VARCHAR(50) NULL,
	Quantity SMALLINT  NOT NULL,
	RepoItemId INT,
	Comment VARCHAR(200)
)
    --GRANT SELECT ON SCHEMA::Warehouse TO ALL
GO 

ALTER TABLE Warehouse.Booking ADD CONSTRAINT
PK_Booking PRIMARY KEY CLUSTERED 
(
Id
)


CREATE TABLE Warehouse.Picking
(
	Id INT IDENTITY(1,1) NOT NULL, 
	Createdby INT NOT NULL,
	CreatedOn SMALLDATETIME NOT NULL, 
	TruckId INT NOT NULL,
	Pickedby INT NULL,
	CheckedBy INT NULL,
	EnteredBy INT NULL,
	ConfirmedOn SMALLDATETIME 
)
GO

ALTER TABLE Warehouse.Picking ADD CONSTRAINT
PK_Picking PRIMARY KEY CLUSTERED 
(
	Id
)
GO

CREATE TABLE Warehouse.PickingItem
(
	PickingId INT NOT NULL, 
	BookingId INT NOT NULL
)
GO

ALTER TABLE Warehouse.PickingItem ADD CONSTRAINT
PK_PickingItem PRIMARY KEY CLUSTERED 
(
	PickingId, BookingId
)
GO

ALTER TABLE Warehouse.PickingItem
ADD CONSTRAINT FK_PickingBooking 
FOREIGN KEY (BookingId)
REFERENCES Warehouse.Booking(Id)
GO

ALTER TABLE Warehouse.PickingItem
ADD CONSTRAINT FK_Picking 
FOREIGN KEY (PickingId)
REFERENCES Warehouse.Picking(Id)
GO
