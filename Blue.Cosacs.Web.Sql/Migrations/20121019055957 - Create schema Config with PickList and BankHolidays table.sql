-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to Feature: #11486

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Config')
EXECUTE sp_executesql N'create schema Config'

                 
IF  NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Config' AND TABLE_NAME = 'PickList')
BEGIN

	CREATE TABLE [Config].[PickList](
	[Id] [varchar](100) NOT NULL,
	[Name] [varchar](100) NULL,
	CONSTRAINT [PK_StringsTable] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
	) ON [PRIMARY]

END



IF  NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Config' AND TABLE_NAME = 'PickRow')
BEGIN
	CREATE TABLE [Config].[PickRow](
		[ListId] [varchar](100) NOT NULL,
		[Order] [smallint] NOT NULL,
		[String] [varchar](200) NOT NULL,
	 CONSTRAINT [PK_StringsRow] PRIMARY KEY CLUSTERED 
	(
		[ListId] ASC,
		[Order] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
	) ON [PRIMARY]
	
	ALTER TABLE [config].[PickRow]  WITH CHECK ADD  CONSTRAINT [FK_PickRow_PickList] FOREIGN KEY([ListId])
	REFERENCES [config].[PickList] ([Id])
	
	ALTER TABLE [config].[PickRow] CHECK CONSTRAINT [FK_PickRow_PickList]
	
END

IF  NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Config' AND TABLE_NAME = 'BankHoliday')
BEGIN

	CREATE TABLE [Config].[BankHoliday](
		[Date] [date]NOT NULL,
	 CONSTRAINT [PK_BankHoliday] PRIMARY KEY CLUSTERED 
	(
		[Date] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = ON, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
	) ON [PRIMARY]	
END

