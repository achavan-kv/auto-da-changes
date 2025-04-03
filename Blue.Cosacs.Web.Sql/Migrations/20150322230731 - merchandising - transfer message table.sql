CREATE TABLE [Financial].[TransferMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NULL,
	[LocationId] [int] NOT NULL,
	[SalesLocationId] [varchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[TotalAWC] money not null,
	CONSTRAINT [PK_Financial_TransferMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


CREATE TABLE [Financial].[TransferProductMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TransferMessageId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Type] [varchar](max) NULL,
	[DepartmentCode] [varchar](max) NULL,
	[Cost] [money] NOT NULL,
 CONSTRAINT [PK_Financial_TransferProductMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [Financial].[TransferProductMessage]  WITH CHECK ADD  CONSTRAINT [FK_Financial_TransferMessage_ProductMessage] FOREIGN KEY([TransferMessageId])
REFERENCES [Financial].[TransferMessage] ([Id])
GO

ALTER TABLE [Financial].[TransferProductMessage] CHECK CONSTRAINT [FK_Financial_TransferMessage_ProductMessage]
GO