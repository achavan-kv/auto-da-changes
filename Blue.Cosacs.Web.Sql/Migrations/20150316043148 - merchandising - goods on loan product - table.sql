-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[GoodsOnLoanProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GoodsOnLoanId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Comments] [varchar](max) NULL,
	[ReferenceNumber] [varchar](50) NULL,
	[AverageWeightedCost] [money] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_Merchandising_GoodsOnLoanProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]