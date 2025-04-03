-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE SalesManagement.Call DROP CONSTRAINT FK_Call_Customer
GO

DROP TABLE [SalesManagement].[CustomerSalesPerson]
GO

CREATE TABLE SalesManagement.CustomerSalesPerson
(
	CustomerId varchar(20) NOT NULL,
	SalesPersonId int NOT NULL,
	TempSalesPersonId int NULL,
	TimeFrameTempSalesPerson date NULL,

	 CONSTRAINT PK_CustomerSalesPerson PRIMARY KEY CLUSTERED 
	(
		CustomerId ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
)
GO

ALTER TABLE SalesManagement.Call
ALTER COLUMN CustomerId VarChar(20) NULL
GO

ALTER TABLE SalesManagement.Call  WITH CHECK ADD  CONSTRAINT FK_Call_CustomerSalesPerson FOREIGN KEY(CustomerId)
REFERENCES SalesManagement.CustomerSalesPerson (CustomerId)
GO

ALTER TABLE SalesManagement.Call CHECK CONSTRAINT FK_Call_CustomerSalesPerson
GO

