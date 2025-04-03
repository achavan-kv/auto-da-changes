IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_TaskSchedular')
	BEGIN
		select 1
	END
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'VE_TaskSchedular')
	BEGIN
CREATE TABLE [dbo].[VE_TaskSchedular](
	[ServiceCode] [varchar](10) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[IsInsertRecord] [bit] NOT NULL,
	[IsEODRecords] [bit] NOT NULL,
	[Status] [bit] NULL,
	[Message] [varchar](1000) NULL,
	[CreatedDate] [datetime] NULL DEFAULT (getdate()),
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CheckOutID] [int] NULL,
 CONSTRAINT [PK_VE_taskschedular] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END