IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'AutoPOMapping'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN

CREATE TABLE [Merchandising].[AutoPOMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookingID] [int] NULL,
	[AcctNo] [bigint] NULL,
	[ItemNo] [varchar](40) NULL,
	[TotalpoQuantity] [int] NULL,
	[TypesofProduct] [varchar](1) NULL,
	[Status] [varchar](30) NULL,
	[CreatedPONo] [int] NULL,
	[XMLFileStatus] [varchar](50) NULL,
	[XMLFileCreatedDate] [datetime] NULL
) ON [PRIMARY]
END
Go
