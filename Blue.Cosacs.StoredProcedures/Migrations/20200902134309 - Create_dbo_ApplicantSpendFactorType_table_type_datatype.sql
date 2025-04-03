-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


GO
-- =======================================================================================
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This script create new datatype ApplicantSpendFactorType. 
-- =======================================================================================


IF TYPE_ID('ApplicantSpendFactorType') IS NOT NULL
BEGIN
    DROP TYPE ApplicantSpendFactorType;
END

CREATE TYPE ApplicantSpendFactorType AS TABLE
(
	[Id] [int] NOT NULL,
	[MinimumIncome] VARCHAR(10),
	[MaximumIncome] VARCHAR(10),
	[ApplicantSpendFactorInPercent] NUMERIC(18,2)
)
GO