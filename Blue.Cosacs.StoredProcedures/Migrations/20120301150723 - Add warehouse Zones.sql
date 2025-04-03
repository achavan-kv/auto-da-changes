CREATE TABLE Warehouse.ZoneAssignment
(
	Id INT IDENTITY(1,1),
	ZoneId INT,
	AttributeName VARCHAR(20),
	AttributeValue VARCHAR(50)	
)

CREATE TABLE Warehouse.Zone
(
	Id INT IDENTITY(1,1),
	Name VARCHAR(20),
	Branch SMALLINT
)


ALTER TABLE Warehouse.ZoneAssignment
ADD CONSTRAINT PK_ZoneAssignementId PRIMARY KEY CLUSTERED  (Id)
GO

ALTER TABLE Warehouse.Zone
ADD CONSTRAINT PK_ZoneId PRIMARY KEY CLUSTERED  (Id)
GO

ALTER TABLE Warehouse.ZoneAssignment 
ADD CONSTRAINT FK_ZoneAssignment_Zone FOREIGN KEY
	(
	ZoneId
	) REFERENCES Warehouse.Zone
	(
	Id
	) 
GO
