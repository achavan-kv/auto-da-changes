

GO
 
 IF EXISTS(
            SELECT 1
            FROM   sys.procedures WITH (NOLOCK)
            WHERE  NAME = 'GetDependentSpendFactorByVal'
                        AND type = 'P'
			)
BEGIN
              DROP PROCEDURE [dbo].[GetDependentSpendFactorByVal]
END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- =======================================================================================
-- Project			: CoSaCS.NET
-- PROCEDURE Name   : GetDependentSpendFactorByVal
-- Author			: SHUBHAM GAIKWAD
-- Create Date		: 28 July 2020
-- Description		: This procedure is used to get dependent spent factor by value.

-- Change Control
-- --------------
-- Date			By			Description
-- ----			--			-----------
-- 
-- =======================================================================================


CREATE PROCEDURE [dbo].[GetDependentSpendFactorByVal] 
		@noOfDep varchar
		, @SpendFactor varchar(10) OUT
		, @return INT OUT
AS
BEGIN

set @return = 0;
Declare @OrgbaseValue varchar(10)
Declare @baseValue varchar(10)


Create Table #DependnetSpendFactorInPercentTemp(
Id int,
NumOfDependents varchar(3),
DependnetSpendFactorInPercent numeric(9,2)
)

insert into #DependnetSpendFactorInPercentTemp
EXEC GetDependentSpendFactor 0


Select @OrgbaseValue =  NumOfDependents from #DependnetSpendFactorInPercentTemp where NumOfDependents like '%>%'

SET @basevalue = SUBSTRING(@OrgbaseValue,2,10);


Set @spendFactor = (Select top 1 DependnetSpendFactorInPercent as SpendFactor from #DependnetSpendFactorInPercentTemp where NumOfDependents = @noOfDep) 

if((@spendFactor IS NULL OR @spendFactor ='') AND @noOfDep > @baseValue)
BEGIN 
 Set @spendFactor= (Select top 1 DependnetSpendFactorInPercent as SpendFactor from #DependnetSpendFactorInPercentTemp where NumOfDependents = @OrgbaseValue)
END

if(@spendFactor IS NULL OR @spendFactor ='')
BEGIN 
Set @spendFactor = '0';
END

IF OBJECT_ID('tempdb..#DependnetSpendFactorInPercentTemp') IS NOT NULL
		 DROP TABLE #DependnetSpendFactorInPercentTemp

END 

GO
