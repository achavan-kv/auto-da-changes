-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: CR12949 - #13715

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Sales')
BEGIN
	EXECUTE sp_executesql N'create schema Sales'
END

                 
IF  NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Sales' AND TABLE_NAME = 'LinkedContracts')
BEGIN

	CREATE TABLE [Sales].[LinkedContracts](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Contract] [varchar](25) NOT NULL,
		[ItemNo] [varchar](18) NOT NULL DEFAULT '',
		[Category] [smallint] NOT NULL DEFAULT 0,

	 CONSTRAINT [pk_LinkedContracts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [Unq_LinkedContracts] UNIQUE NONCLUSTERED 
(
	[Contract] ASC,
	[ItemNo] ASC,
	[Category] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


END
GO