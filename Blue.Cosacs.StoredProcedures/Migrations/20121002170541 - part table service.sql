CREATE TABLE service.Part
	(
	Id int NOT NULL IDENTITY (1, 1),
	RequestId int NOT NULL,
	PartNumber varchar(50) NOT NULL,
	PartType int NOT NULL,
	Quantity float(53) NOT NULL,
	Price money NOT NULL,
	Description varchar(200) NOT NULL,
	StockBranch smallint NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE Service.Part 
ADD CONSTRAINT FK_Part_Request 
FOREIGN KEY ( RequestId ) REFERENCES Service.Request (Id) 
ON DELETE  CASCADE 
GO	 


ALTER TABLE Service.Part ADD CONSTRAINT
	PK_Part PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

