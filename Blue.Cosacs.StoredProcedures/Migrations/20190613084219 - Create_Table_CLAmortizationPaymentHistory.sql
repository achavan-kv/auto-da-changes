
/****** Object:  Table [dbo].[CLAmortizationSchedule]    Script Date: 2019-06-13 14:06:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'CLAmortizationPaymentHistory' )
BEGIN
CREATE TABLE [dbo].[CLAmortizationPaymentHistory](
	[AcctNo] [char](12) NOT NULL,
	[InstalDueDate] [datetime] NOT NULL,
	[OpeningBal] [decimal](15, 2) NOT NULL,
	[Instalment] [decimal](15, 2) NOT NULL,
	[Principal] [decimal](15, 2) NOT NULL,
	[ServiceChg] [decimal](15, 2) NOT NULL,
	[ClosingBal] [decimal](15, 2) NOT NULL,
	[AdminFee] [decimal](15, 2) NULL,
	[IsPaid] [bit] NOT NULL CONSTRAINT [DF_CLAmortizationPaymentHistory_isPaid]  DEFAULT ((0)),
	[Interest] [money] NOT NULL CONSTRAINT [DF_CLAmortizationPaymentHistory_interest]  DEFAULT ((0)),
	[InstallmentNo] [int] NOT NULL CONSTRAINT [DF_CLAmortizationPaymentHistory_installmentNo]  DEFAULT ((0)),
	[PrevPrincipal] [decimal](15, 2) NULL DEFAULT ((0)),
	[PrevServiceChg] [decimal](15, 2) NULL DEFAULT ((0)),
	[PrevAdminFee] [decimal](15, 2) NULL DEFAULT ((0)),
	[PrevInterest] [money] NULL DEFAULT ((0)),
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_CLAmortizationPaymentHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

SET ANSI_PADDING OFF
GO


