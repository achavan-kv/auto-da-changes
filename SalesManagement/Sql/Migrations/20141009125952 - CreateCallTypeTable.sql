-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE SalesManagement.CallType
 (
	 Id tinyint NOT NULL IDENTITY (1, 1),
	 Name varchar(32) NOT NULL,
	 Icon varchar(32) NULL
 )  ON [PRIMARY]
GO

ALTER TABLE SalesManagement.CallType ADD CONSTRAINT
 PK_CallType PRIMARY KEY CLUSTERED 
 (
 Id
 ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]