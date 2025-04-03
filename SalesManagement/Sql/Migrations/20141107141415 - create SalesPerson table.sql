-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DROP TABLE SalesManagement.CsrUnavailable
GO

CREATE TABLE SalesManagement.SalesPerson
(
	Id					Int				NOT NULL,
	BranchNo			SmallInt		NOT NULL,
	Name				VarChar(100)	NOT NULL,
	BeggingUnavailable	Date			NOT NULL,
	EndUnavailable		Date			NOT NULL,
	CreatedOn			DateTime		NOT NULL,
	CreatedBy			Int				NOT NULL
)  ON [PRIMARY]

ALTER TABLE SalesManagement.SalesPerson ADD CONSTRAINT PK_SalesManagement_SalesPerson PRIMARY KEY CLUSTERED 
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

CREATE NONCLUSTERED INDEX ix_SalesManagement_SalesPerson_branch ON SalesManagement.SalesPerson
(
	BranchNo ASC
)WITH 
(
	PAD_INDEX = OFF, 
	STATISTICS_NORECOMPUTE = OFF, 
	SORT_IN_TEMPDB = OFF, 
	DROP_EXISTING = OFF, 
	ONLINE = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]