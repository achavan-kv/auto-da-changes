-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DROP TABLE SalesManagement.CsrUnavailable
GO

CREATE TABLE SalesManagement.CsrUnavailable
(
	Id					SmallInt	IDENTITY(1,1) NOT NULL,
	CsrId				Int			NOT NULL,
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

CREATE NONCLUSTERED INDEX ix_SalesManagement_CsrUnavailable_CsrId ON SalesManagement.CsrUnavailable
(
	CsrId ASC
)WITH 
(
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
GO

