-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/*
	CR		: CLA OutStanding Balance 
	Author  : Rahul D,
	Date	: 20/06/2019
	Details	: Table to capture the payments only for cashloan account.
				It will help to export data for broker EOD
*/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF EXISTS (SELECT 'A' FROM SYS.TABLES WHERE NAME = 'CLANewPaymentDetails')
	DROP TABLE CLANewPaymentDetails


CREATE TABLE [dbo].[CLANewPaymentDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AcctNo] [varchar](12) NULL,
	[TransValue] [money] NULL,
	[TranstypeCode] [varchar](10) NULL,
	[TransDate] [datetime] NULL DEFAULT (getdate()),
	[RunNo] [int] NULL DEFAULT ((0)),
	[CreditDebitNo] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO