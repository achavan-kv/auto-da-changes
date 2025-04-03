-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE [dbo].[EventType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[Category] [nvarchar](250) NULL,
	[FriendlyName] [nvarchar](250) NULL,
 CONSTRAINT [PK_EventType] PRIMARY KEY CLUSTERED ([Id] ASC) ON [AUDIT]
) ON [AUDIT]
GO

CREATE TABLE [dbo].[Event](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EventOnUtc] [datetime] NOT NULL,
	[EventBy] [nvarchar](50) NULL,
	[EventType] [int] NOT NULL,
	[Payload] [varbinary](max) NOT NULL,
	[IndexName1] [nvarchar](50) NULL,
	[IndexValue1] [nvarchar](100) NULL,
	[IndexName2] [nvarchar](50) NULL,
	[IndexValue2] [nvarchar](100) NULL,
	[IndexName3] [nvarchar](50) NULL,
	[IndexValue3] [nvarchar](100) NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED ([Id] ASC) ON [AUDIT]
) ON [AUDIT]
GO

ALTER TABLE [dbo].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_EventType] FOREIGN KEY([EventType])
REFERENCES [dbo].[EventType] ([Id])
GO
ALTER TABLE [dbo].[Event] CHECK CONSTRAINT [FK_Event_EventType]
GO
