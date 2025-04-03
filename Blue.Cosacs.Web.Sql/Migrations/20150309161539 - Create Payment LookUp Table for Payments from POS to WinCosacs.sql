-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentMethodLookUp]') AND type in (N'U'))
DROP TABLE [dbo].[PaymentMethodLookUp]
GO

CREATE TABLE [dbo].[PaymentMethodLookUp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BranchNumber] smallint NOT NULL,
	[WinCosacsPayMethodId] smallint NOT NULL,
	[PosPayMethodId] smallint NOT NULL
 CONSTRAINT [PK_PaymentMethodLookUp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
