-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE SalesManagement.Customer
	(
	Id int NOT NULL IDENTITY (1, 1),
	CustomerId varchar(20) NOT NULL,
	SalesPersonId int NOT NULL,
	TempSalesPersonId int NULL,
	TimeFrameTempSalesPerson date NULL
	)  ON [PRIMARY]
GO
ALTER TABLE SalesManagement.Customer ADD CONSTRAINT
	PK_Customer_1 PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE SalesManagement.Customer ADD CONSTRAINT
	IX_Customer UNIQUE NONCLUSTERED 
	(
	CustomerId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE TABLE SalesManagement.CallCloseReason
	(
	Id tinyint NOT NULL,
	Name varchar(32) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE SalesManagement.CallCloseReason ADD CONSTRAINT
	PK_CallCloseReason PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE TABLE SalesManagement.Call
	(
	Id int NOT NULL,
	CustomerFirstName varchar(32) NULL,
	CustomerLastName varchar(32) NULL,
	CustomerId int NULL,
	CallTypeId tinyint NOT NULL,
	ReasonToCall varchar(32) NOT NULL,
	CallerId int NULL,
	ToCallAt smalldatetime NOT NULL,
	CalledAt smalldatetime NOT NULL,
	SpokeToCustomer bit NOT NULL,
	CallClosedReasonId tinyint NULL,
	PreviousCallId int NOT NULL,
	Comments varchar(1024) NULL,
	CreatedOn smalldatetime NOT NULL,
	CreatedBy int NULL
	)  ON [PRIMARY]
GO
ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	PK_Call PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_CallType FOREIGN KEY
	(
	CallTypeId
	) REFERENCES SalesManagement.CallType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_CallCloseReason FOREIGN KEY
	(
	CallClosedReasonId
	) REFERENCES SalesManagement.CallCloseReason
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_Call FOREIGN KEY
	(
	PreviousCallId
	) REFERENCES SalesManagement.Call
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_Customer FOREIGN KEY
	(
	CustomerId
	) REFERENCES SalesManagement.Customer
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
