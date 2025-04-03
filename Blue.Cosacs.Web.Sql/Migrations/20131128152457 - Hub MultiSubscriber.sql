-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

alter table Hub.[Message] alter column [CreatedOn] smalldatetime NOT NULL

GO

alter table Hub.[Message] alter column [Exception] varchar(max) sparse null 

go

go

ALTER TABLE Hub.Message
	DROP CONSTRAINT FK_Message_Queue
GO

ALTER TABLE Hub.Queue
	DROP CONSTRAINT PK_Queue
GO

alter table Hub.[Queue] alter column [Id] tinyint not null

GO

alter table Hub.[Message] alter column [QueueId] tinyint not null

GO

ALTER TABLE Hub.[Queue] ADD CONSTRAINT
	PK_Queue PRIMARY KEY CLUSTERED 
	(
	Id
	) 
GO

ALTER TABLE Hub.[Message] ADD CONSTRAINT
	FK_Message_Queue FOREIGN KEY
	(
	QueueId
	) REFERENCES Hub.[Queue]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO


EXECUTE sp_rename N'Hub.Message', N'QueueMessage', 'OBJECT' 

GO

alter table Hub.[Queue] 
 add [SubscriberSqlConnectionName] varchar(50) NULL

GO

alter table Hub.[Queue] 
 add [SubscriberSqlProcedureName] varchar(100) NULL

GO

EXECUTE sp_rename N'Hub.Queue.SubscriberAssemblyName', N'SubscriberClrAssemblyName', 'COLUMN' 
GO

EXECUTE sp_rename N'Hub.Queue.SubscriberTypeName', N'SubscriberClrTypeName', 'COLUMN' 
GO

alter table Hub.[Queue] 
 alter column  SubscriberClrAssemblyName varchar(200) NULL

GO

alter table Hub.[Queue] 
 alter column  SubscriberClrTypeName varchar(200) NULL

GO

ALTER TABLE Hub.QueueMessage
	DROP CONSTRAINT PK_Message
GO

alter table Hub.QueueMessage
  alter column Id int NOT NULL

GO

ALTER TABLE Hub.QueueMessage ADD CONSTRAINT
	PK_QueueMessage PRIMARY KEY CLUSTERED 
	(
	Id
	) 
GO

exec sp_rename 'Hub.FK_Message_Queue', 'FK_Hub_QueueMessage_Queue', 'object'

GO

exec sp_rename 'Hub.QueueMessage.IX_Hub_Message_Index', 'IX_Hub_QueueMessage_CorrelationId', 'INDEX'

GO

exec sp_rename 'Hub.HubMessageState', 'CK_Hub_QueueMessage_State', 'object'

GO

select Id, CreatedOn, Body
into Hub.[Message]
from Hub.QueueMessage

GO

ALTER TABLE Hub.[Message] ADD CONSTRAINT
	PK_Message PRIMARY KEY CLUSTERED 
	(
	Id
	) 
GO

alter table Hub.QueueMessage 
 add [MessageId] INT NULL

go

update Hub.QueueMessage 
set MessageId = Id

go

alter table Hub.QueueMessage 
 alter column [MessageId] INT NOT NULL

go

ALTER TABLE Hub.QueueMessage ADD CONSTRAINT
	FK_QueueMessage_Message FOREIGN KEY
	(
	MessageId
	) REFERENCES Hub.[Message]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

EXECUTE sp_rename N'Hub.Queue.Name', N'Binding', 'COLUMN' 

GO

alter table Hub.QueueMessage drop column [Body]

GO

ALTER TABLE [Hub].[QueueMessage]
ADD [RunCount2] smallINT 
CONSTRAINT CK_Hub_QueueMessage_RunCount_Default DEFAULT 0 NULL;

GO

update Hub.QueueMessage
set RunCount2 = RunCount

go

alter table Hub.[QueueMessage] drop constraint  CK_Hub_Message_RunCount_Default
go


alter table Hub.[QueueMessage] drop column [RunCount] 

go

ALTER TABLE [Hub].[QueueMessage] alter column [RunCount2] smallINT not null

go 

EXECUTE sp_rename N'Hub.[QueueMessage].RunCount2', N'RunCount', 'COLUMN' 

go

alter table Hub.QueueMessage alter column [CorrelationId] varchar(256) null

go
