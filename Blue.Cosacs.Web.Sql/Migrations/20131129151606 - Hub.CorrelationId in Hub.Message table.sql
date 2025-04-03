-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Hub.Message ADD
	Routing varchar(100) NULL
GO

ALTER TABLE Hub.Message ADD
	CorrelationId varchar(256) NULL
GO

alter table Hub.[Queue]
 alter column [Binding] varchar(100) not null

go

update Hub.[Message]
set Routing = Q.[Binding], CorrelationId = QM.CorrelationId
from Hub.[Message] M
inner join Hub.[QueueMessage] QM
on M.Id = QM.MessageId
inner join Hub.[Queue] Q
on QM.QueueId = Q.Id

go

alter table Hub.[Message]
  alter column Routing varchar(100) not null

go

drop index Hub.QueueMessage.IX_Hub_QueueMessage_CorrelationId

go

CREATE NONCLUSTERED INDEX IX_Hub_Message_CorrelationId ON Hub.Message (CorrelationId) 
GO

alter table  Hub.QueueMessage
  drop column [CorrelationId]

go

ALTER TABLE Hub.Message ADD
	IsRouted bit NULL
GO

update Hub.Message 
set IsRouted = 1

go

ALTER TABLE Hub.Message alter column
	IsRouted bit NOT NULL
GO

ALTER TABLE Hub.Message ADD CONSTRAINT
	DF_Message_IsRouted DEFAULT 0 FOR IsRouted
GO

