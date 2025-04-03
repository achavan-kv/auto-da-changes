IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'ProductAttributes'
               AND TABLE_SCHEMA = 'Merchandising')
BEGIN
CREATE TABLE [Merchandising].[ProductAttributes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[SKU] [varchar](20) NOT NULL,
	[IsAshleyProduct] [bit] NOT NULL CONSTRAINT [DF_ProductAttributes_IsAshleyProduct]  DEFAULT ((0)),
	[IsSpecialProduct] [bit] NOT NULL CONSTRAINT [DF_ProductAttributes_IsSpecialProduct]  DEFAULT ((0)),
	[ISAutoPO] [bit] NOT NULL CONSTRAINT [DF_ProductAttributes_ISAutoPO]  DEFAULT ((0)),
	[ISOnlineAvailable] [bit] NOT NULL CONSTRAINT [DF_ProductAttributse_ISOnlineAvailable]  DEFAULT ((0)),
 CONSTRAINT [PK_Merchandising.ProductAttributes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF


ALTER TABLE [Merchandising].[ProductAttributes]  WITH CHECK ADD  CONSTRAINT [FK_ProductAttributes_Productid] FOREIGN KEY([ProductId])
REFERENCES [Merchandising].[Product] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE


ALTER TABLE [Merchandising].[ProductAttributes] CHECK CONSTRAINT [FK_ProductAttributes_Productid]

END
Go