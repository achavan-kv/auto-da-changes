-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE SalesManagement.Tmp_CallCloseReason
	(
	Id tinyint NOT NULL,
	Name varchar(32) NOT NULL
	)  ON [PRIMARY]


ALTER TABLE SalesManagement.Call
	DROP CONSTRAINT FK_Call_CallCloseReason

DROP TABLE SalesManagement.CallCloseReason

EXECUTE sp_rename N'SalesManagement.Tmp_CallCloseReason', N'CallCloseReason', 'OBJECT' 

ALTER TABLE SalesManagement.CallCloseReason ADD CONSTRAINT
	PK_CallCloseReason PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
