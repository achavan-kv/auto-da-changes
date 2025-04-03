IF NOT EXISTS (SELECT * FROM sysobjects
               WHERE xtype = 'u'
               AND name = 'tempstatustype')
BEGIN
	CREATE TABLE [dbo].[tempstatustype](
		[AcctNo] [char](12) NOT NULL,
		[ItemNo] [varchar](8) NOT NULL,
		[AgrmtNo] [int] NOT NULL,
		[StockLocn] [smallint] NOT NULL,
		[ContractNo] [varchar](10) NOT NULL,
		[NewStatusType] [varchar](1) NOT NULL,
		[quantity] [float] NOT NULL,
		[ordval] [money] NULL,
		[PrevInttoOr] [varchar](1) NULL,
		[PrevDel] [char](1) NULL
	) ON [PRIMARY]
	SET ANSI_PADDING OFF
	ALTER TABLE [dbo].[tempstatustype] ADD [Itemtype] [varchar](1) NOT NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [PrevIntVal] [money] NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [PrevIntQ] [float] NULL
	SET ANSI_PADDING ON
	ALTER TABLE [dbo].[tempstatustype] ADD [deleteflag] [char](1) NULL
	SET ANSI_PADDING OFF
	ALTER TABLE [dbo].[tempstatustype] ADD [Currenttype] [varchar](1) NOT NULL
	SET ANSI_PADDING ON
	ALTER TABLE [dbo].[tempstatustype] ADD [Del2bX] [char](1) NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [DELETEdQty] [float] NOT NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [deleteval] [money] NOT NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [orderno] [int] NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [orderlineno] [smallint] NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [PrevDelVal] [money] NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [PrevDelQ] [float] NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [CURRENTDel] [float] NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [category] [smallint] NOT NULL
	ALTER TABLE [dbo].[tempstatustype] ADD [ID] [int] IDENTITY(1,1) NOT NULL
END
