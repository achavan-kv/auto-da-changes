CREATE SCHEMA Service
GO

CREATE TABLE Service.Request
	(
	Id int NOT NULL IDENTITY (1, 1),
	CreatedOn smalldatetime NOT NULL,
	CreatedBy smallint NOT NULL,
	Branch smallint NOT NULL,
	Internal bit NOT NULL,
	RequestState int NOT NULL,
	InvoiceNumber varchar(50) NULL,
	CustomerId varchar(50) NULL,
	CustomerTitle varchar(25) NULL,
	CustomerFirstName varchar(50) NULL,
	CustomerLastName varchar(50) NULL,
	CustomerContact1 varchar(255) NULL,
	CustomerContactType1 char(1) NULL,
	CustomerContact2 varchar(255) NULL,
	CustomerContactType2 char(1) NULL,
	CustomerContact3 varchar(255) NULL,
	CustomerContactType3 char(1) NULL,
	CustomerContact4 varchar(255) NULL,
	CustomerContactType4 char(1) NULL,
	CustomerAddressLine1 varchar(50) NULL,
	CustomerAddressLine2 varchar(50) NULL,
	CustomerAddressLine3 varchar(50) NULL,
	CustomerPostcode varchar(10) NULL,
	CustomerNotes varchar(4000) NULL,
	ItemId int NOT NULL,
	Quantity smallint NULL,
	ItemAmount money NULL,
	ItemSoldOn smalldatetime NULL,
	ItemSoldBy varchar(50) NULL,
	ItemDeliveredOn smalldatetime NULL,
	ItemStockLocation smallint NULL,
	Item varchar(100) NOT NULL,
	ItemSupplier varchar(50) NOT NULL,
	ItemSerialNumber varchar(50) NOT NULL,
	WarrantyContractId int NULL,
	WarrantyLength smallint NULL,
	)  ON [PRIMARY]
GO

ALTER TABLE Service.Request ADD CONSTRAINT
	PK_Request PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Service.Request SET (LOCK_ESCALATION = TABLE)
GO
