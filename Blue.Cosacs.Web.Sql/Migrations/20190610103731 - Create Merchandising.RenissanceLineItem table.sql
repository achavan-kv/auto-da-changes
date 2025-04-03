IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'RenissanceLineItem'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN

CREATE TABLE [Merchandising].[RenissanceLineItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CutomerId] [int] NULL,
	[ProductId] [varchar](18) NULL,
	[quantity] [varchar](50) NULL,
	[stocklocn] [smallint] NULL,
	[datereqdel] [varchar](50) NULL,
	[SKU] [varchar](50) NULL,
	[ItemID] [int] NULL,
	[RecordIMpdate] [datetime] NULL,
	[InsertRecordStatus] [int] NULL
) ON [PRIMARY]
END
Go