-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.CustomerSalesPerson DROP CONSTRAINT FK_CustomerSalesPerson_SalesPerson
DROP TABLE [SalesManagement].[SalesPerson]
GO

CREATE TABLE SalesManagement.CsrUnavailable
(
	Id					Int	NOT NULL,
	BeggingUnavailable	Date		NOT NULL,
	EndUnavailable		Date		NOT NULL,
	CreatedOn			DateTime	NOT NULL,
	CreatedBy			Int			NOT NULL
)  ON [PRIMARY]

ALTER TABLE SalesManagement.CsrUnavailable ADD CONSTRAINT PK_SalesManagement_CsrUnavailable PRIMARY KEY CLUSTERED 
(
	Id
) WITH
( 
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
GO