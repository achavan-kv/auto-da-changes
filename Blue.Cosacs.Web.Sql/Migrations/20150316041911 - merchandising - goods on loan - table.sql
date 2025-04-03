-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[GoodsOnLoan] (
	Id INT IDENTITY(1,1) NOT NULL,
	ServiceRequestId INT NULL,
	CustomerId varchar(100) NULL,
	BusinessName varchar(100) NULL,
	Title varchar(100) NULL,
	FirstName varchar(100) NOT NULL,
	LastName varchar(100) NOT NULL,
	AddresssLine1 varchar(100) NOT NULL,
	AddressLine2 varchar(100) NULL,
	TownCity varchar(100) NULL,
	PostCode varchar(100) NULL,
	StockLocationId int NOT NULL,
	ExpectedCollectionDate date NOT NULL,
	Comments varchar(100) NULL,
	Contacts varchar(max) NULL,
	CONSTRAINT [PK_Merchandising_GoodsOnLoan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
