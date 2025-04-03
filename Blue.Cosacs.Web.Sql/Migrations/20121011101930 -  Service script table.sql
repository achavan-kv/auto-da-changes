ALTER TABLE Service.RequestScriptAnswer
	DROP CONSTRAINT FK_RequestScript_Request
GO

ALTER TABLE Service.Request SET (LOCK_ESCALATION = TABLE)
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_RequestScript_ScriptLookup]') AND parent_object_id = OBJECT_ID(N'[Service].[RequestScriptAnswer]'))
ALTER TABLE [Service].[RequestScriptAnswer] DROP CONSTRAINT [FK_RequestScript_ScriptLookup]
GO

DROP TABLE Service.RequestScriptAnswer
GO

CREATE TABLE Service.RequestScriptAnswer
	(
	Id int NOT NULL IDENTITY (1, 1),
	RequestId int NOT NULL,
	Question varchar(500) NOT NULL,
	Answer char(3) NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE Service.RequestScriptAnswer ADD CONSTRAINT
	FK_RequestScript_Request FOREIGN KEY
	(
	RequestId
	) REFERENCES Service.Request
	(
	Id
	) ON UPDATE NO ACTION 
	 ON DELETE CASCADE 
GO


