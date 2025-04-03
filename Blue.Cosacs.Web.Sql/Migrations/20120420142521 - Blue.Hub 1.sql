-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE SCHEMA [Hub] AUTHORIZATION [dbo]
GO
/****** Object:  Table [Hub].[Queue]    Script Date: 04/20/2012 15:25:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Hub].[Queue](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Schema] [xml] NULL,
	[SubscriberAssemblyName] [nvarchar](200) NULL,
	[SubscriberTypeName] [nvarchar](200) NULL,
 CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [Hub].[Message]    Script Date: 04/20/2012 15:25:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [Hub].[Message](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[QueueId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Body] [xml] NOT NULL,
	[State] [char](1) NOT NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Default [DF_Message_Status]    Script Date: 04/20/2012 15:25:04 ******/
ALTER TABLE [Hub].[Message] ADD  CONSTRAINT [DF_Message_Status]  DEFAULT ('I') FOR [State]
GO
/****** Object:  Check [HubMessageState]    Script Date: 04/20/2012 15:25:04 ******/
ALTER TABLE [Hub].[Message]  WITH CHECK ADD  CONSTRAINT [HubMessageState] CHECK  (([State]='P' OR [State]='S' OR [State]='R' OR [State]='I'))
GO
ALTER TABLE [Hub].[Message] CHECK CONSTRAINT [HubMessageState]
GO
/****** Object:  ForeignKey [FK_Message_Queue]    Script Date: 04/20/2012 15:25:04 ******/
ALTER TABLE [Hub].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Queue] FOREIGN KEY([QueueId])
REFERENCES [Hub].[Queue] ([Id])
GO
ALTER TABLE [Hub].[Message] CHECK CONSTRAINT [FK_Message_Queue]
GO
