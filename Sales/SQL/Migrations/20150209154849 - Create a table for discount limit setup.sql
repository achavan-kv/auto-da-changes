-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Sales].[DiscountLimit]') AND type in (N'U'))
DROP TABLE [Sales].[DiscountLimit]
GO

CREATE TABLE [Sales].[DiscountLimit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BranchNumber] smallint NULL,
	[StoreType] [char](1) NULL,
	[LimitPrice] [BlueAmount] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[CreatedBy] int NOT NULL,
 CONSTRAINT [PK_DiscountLimit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


ALTER TABLE [Sales].[DiscountLimit]  WITH CHECK ADD  CONSTRAINT [FK_DiscountLimit_User] FOREIGN KEY([CreatedBy])
REFERENCES [Admin].[User] ([Id])
GO

ALTER TABLE [Sales].[DiscountLimit] CHECK CONSTRAINT [FK_DiscountLimit_User]
GO
