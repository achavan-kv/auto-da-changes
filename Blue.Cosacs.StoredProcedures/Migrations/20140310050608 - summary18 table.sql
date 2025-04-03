-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[summary18]') AND type in (N'U'))
DROP TABLE [dbo].[summary18]
GO


CREATE TABLE [dbo].[summary18](
	[Branchno] [int] NULL,
	[UserId] [int] NULL,
	[Employee] [varchar](150) NOT NULL,
	[Option] [varchar](50) NOT NULL,
	[storetype] [char](1) NOT NULL,
	[NumberOfAccounts] [int] NULL,
	[PctAccounts] [float] NULL,
	[AverageTerm] [money] NULL,
	[AverageTermSettled] [money] NULL,
	[AccountsSettledInPeriod] [int] NULL,
	[AverageTermSettledInPeriod] [money] NULL,
	[TotalServiceCharge] [money] NULL,
	[AgreementTotal] [money] NULL,
	[PctAgreementTotal] [float] NULL,
	[WeightedAverageTerm] [money] NULL,
	[WeightedAverageTermSettled] [money] NULL,
	[TotalRebatesPaid] [money] NULL,
	[UnearnedIncome] [money] NULL,
	[EarnedIncome] [money] NULL,
	[BranchName] [varchar](20) NOT NULL,
	[TotalTerm] [int] NULL,
	[NumSettled] [int] NULL,
	[TermSettled] [int] NULL,
	[WgtTermAgrTot] [money] NULL,
	[WgtSettAgrTot] [money] NULL,
	[AgrmtTotalSettled] [money] NULL,
	[PeriodTermSettled] [int] NULL,
	[PeriodNumSettled] [int] NULL,
	[ScoringBand] [varchar](4) NOT NULL,
	[TotCashPrice] [money] NULL,
	[IntRate] [float] NULL,
	[PeriodSettledAgrmtTotal] [money] NULL
) ON [PRIMARY]

GO

