CREATE SCHEMA Warehouse 
GO

CREATE TABLE Warehouse.Booking 
(
	Id INT,
	CustomerName VARCHAR(90),
	AdressLine1 VARCHAR(50),
	AdressLine2 VARCHAR(50),
	AdressLine3 VARCHAR(50),
	PostCode VARCHAR(1),
	[Action] CHAR(1),
	ActionDate DATETIME,
	ItemNo varchar(18),
	ItemId INT,
	IUPC VARCHAR(18),
	ProductDescription VARCHAR(100),
	Brand varchar(50),
	Model varchar(50),
	Quantity SMALLINT,
	RepoItemId INT,
	Code VARCHAR(5),
	Comment VARCHAR(200)
)
    --GRANT SELECT ON SCHEMA::Warehouse TO ALL
GO 