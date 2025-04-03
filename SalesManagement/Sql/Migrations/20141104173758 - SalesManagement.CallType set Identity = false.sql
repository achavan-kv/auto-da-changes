-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE SalesManagement.Tmp_CallType
	(
	Id tinyint NOT NULL,
	Name varchar(32) NOT NULL,
	Icon varchar(32) NULL
	)  ON [PRIMARY]


ALTER TABLE SalesManagement.Tmp_CallType SET (LOCK_ESCALATION = TABLE)

IF EXISTS(SELECT * FROM SalesManagement.CallType)
	 EXEC('INSERT INTO SalesManagement.Tmp_CallType (Id, Name, Icon)
		SELECT Id, Name, Icon FROM SalesManagement.CallType WITH (HOLDLOCK TABLOCKX)')

ALTER TABLE SalesManagement.Call
	DROP CONSTRAINT FK_Call_CallType


DROP TABLE SalesManagement.CallType


EXECUTE sp_rename N'SalesManagement.Tmp_CallType', N'CallType', 'OBJECT' 


ALTER TABLE SalesManagement.CallType ADD CONSTRAINT
	PK_CallType PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_CallType FOREIGN KEY
	(
	CallTypeId
	) REFERENCES SalesManagement.CallType
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 