-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
CREATE TABLE SalesManagement.SummaryTable
(
	Id int Identity(1,1) NOT NULL,
	Date date NOT NULL,
	SalesPersonId int NOT NULL,
	BranchId smallint NOT NULL,
	Amount dbo.BlueAmount NOT NULL,
	Matrix varchar(64) NOT NULL
)  ON [PRIMARY]
GO

ALTER TABLE SalesManagement.SummaryTable ADD CONSTRAINT
	PK_SummaryTable PRIMARY KEY CLUSTERED 
(
	Id
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX ix_SummaryTable_Date ON SalesManagement.SummaryTable
(
	Date Desc
)
GO

