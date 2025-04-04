
CREATE TABLE Service.[ServiceSupplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Supplier] [varchar](100) NOT NULL,
	[Account] [varchar](12) NOT NULL,
 CONSTRAINT [PK_ServiceSupplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


