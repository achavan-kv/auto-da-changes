-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This script create DependentSpendFactor table.
-- =======================================================================================

GO
IF EXISTS (
			SELECT	1
			FROM	sys.Tables WITH (NOLOCK)
			WHERE	NAME = 'DependentSpendFactor'
					AND type = 'U')
BEGIN
		DROP TABLE [dbo].[DependentSpendFactor]
END
GO

SET ANSI_PADDING OFF
GO


CREATE TABLE [dbo].[DependentSpendFactor]
(
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[NumOfDependents] varchar(3),
	[DependnetSpendFactorInPercent] [numeric](18, 2),
	[OrgNumOfDep] varchar(3),
	[IsBaseOfNext] bit,
	[CreatedDate] [datetime],
	[IsActive] [bit] DEFAULT 1,
	[DateDeactivated] [datetime]
)

GO