ALTER TABLE dbo.Event
 DROP CONSTRAINT FK_Event_EventType
GO
DROP TABLE dbo.EventType
GO
drop table dbo."Event"
go
CREATE TABLE dbo.[Event]
 (
 Id bigint NOT NULL IDENTITY (1, 1),
 EventOnUtc datetime NOT NULL,
 EventType nvarchar(100) NOT NULL,
 EventCategory nvarchar(50) NULL,
 EventBy nvarchar(50) NULL,
 IndexName1 nvarchar(50) NULL,
 IndexValue1 nvarchar(100) NULL,
 IndexName2 nvarchar(50) NULL,
 IndexValue2 nvarchar(100) NULL,
 IndexName3 nvarchar(50) NULL,
 IndexValue3 nvarchar(100) NULL,
 Payload varbinary(MAX) NOT NULL
 )  ON [AUDIT]
  TEXTIMAGE_ON [AUDIT]
GO
ALTER TABLE dbo.Event ADD CONSTRAINT
 PK_Event PRIMARY KEY CLUSTERED 
 (
 Id
 ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
 ON [AUDIT]
GO