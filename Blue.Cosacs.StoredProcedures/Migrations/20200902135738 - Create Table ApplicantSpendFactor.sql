-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This script create ApplicantSpendFactor table.
-- =======================================================================================

GO
IF EXISTS (	SELECT	1
			FROM	sys.Tables WITH (NOLOCK)
			WHERE	NAME = 'ApplicantSpendFactor'
					AND type = 'U')
BEGIN
	DROP TABLE [dbo].[ApplicantSpendFactor]
END
GO


SET ANSI_PADDING OFF
GO


CREATE TABLE [dbo].[ApplicantSpendFactor]
(
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[MinimumIncome] [varchar](10) NULL,
	[MaximumIncome] [varchar](10) NULL,
	[ApplicantSpendFactorInPercent] [numeric](18, 2) NULL,
	[CreatedDate] [datetime] NULL,
	[IsBaseOfNext] [bit] NULL,
	[OrgMinimumIncome] [varchar](10) NULL,
	[IsActive] [bit] NULL DEFAULT ((1)),
	[DateDeactivated] [datetime] NULL	
)

GO