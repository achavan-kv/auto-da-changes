CREATE TABLE Service.ScriptLookup
	(
	Id int NOT NULL IDENTITY (1, 1),
	Question varchar(500) NOT NULL,
	Active bit NOT NULL,
	[Order] int NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE SERVICE.ScriptLookup ADD CONSTRAINT
	PK_ScriptLookup PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Service.ScriptLookup SET (LOCK_ESCALATION = TABLE)
GO

CREATE TABLE Service.RequestScriptAnswer
	(
	Id int NOT NULL IDENTITY (1, 1),
	ScriptId INT NOT NULL, 
	RequestId INT NOT NULL,
	Question INT NOT NULL,
	Answer bit NOT NULL
	)  ON [PRIMARY]
GO


ALTER TABLE Service.RequestScriptAnswer
ADD CONSTRAINT FK_RequestScript_ScriptLookup FOREIGN KEY (ScriptId) 
REFERENCES Service.ScriptLookup (Id) 
GO

ALTER TABLE Service.RequestScriptAnswer
ADD CONSTRAINT FK_RequestScript_Request FOREIGN KEY (RequestId) 
REFERENCES Service.Request (Id) 
GO

