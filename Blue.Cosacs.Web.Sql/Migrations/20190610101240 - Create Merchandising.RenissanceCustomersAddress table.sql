IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'RenissanceCustomersAddress'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN

CREATE TABLE [Merchandising].[RenissanceCustomersAddress](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CutomerId] [int] NULL,
	[AddressType] [varchar](400) NULL,
	[Address1] [varchar](400) NULL,
	[Address2] [varchar](400) NULL,
	[Address3] [varchar](400) NULL,
	[PostCode] [varchar](40) NULL,
	[Deliveryarea] [varchar](40) NULL,
	[EMail] [varchar](40) NULL,
	[DialCode] [varchar](40) NULL,
	[PhoneNo] [varchar](10) NULL,
	[Ext] [varchar](10) NULL,
	[DELTitleC] [varchar](10) NULL,
	[DELFirstname] [varchar](10) NULL,
	[DELLastname] [varchar](10) NULL,
	[Notes] [varchar](50) NULL,
	[DateIn] [datetime] NULL,
	[NewRecord] [bit] NULL,
	[Zone] [varchar](10) NULL,
	[RecordImpDate] [datetime] NULL,
	[insertRecordStatus] [int] NULL,
 CONSTRAINT [PK_RenissanceCustomersAddress] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO