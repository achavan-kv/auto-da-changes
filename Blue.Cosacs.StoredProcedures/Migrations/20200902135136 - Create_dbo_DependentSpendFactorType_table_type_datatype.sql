-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

GO
-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This script create new datatype DependentSpendFactorType. 
-- =======================================================================================


IF TYPE_ID('DependentSpendFactorType') IS NOT NULL
BEGIN
    DROP TYPE DependentSpendFactorType;
END


CREATE TYPE [dbo].[DependentSpendFactorType] AS TABLE
(
	[Id] [int] NOT NULL,
	[NumOfDependents] [varchar](3) NULL,
	[DependnetSpendFactorInPercent] [numeric](18, 2) NULL
)
GO
