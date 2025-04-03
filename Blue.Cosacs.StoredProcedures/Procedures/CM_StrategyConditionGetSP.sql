
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_StrategyConditionGetSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_StrategyConditionGetSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Returns the contents of the CM StrategyCondition table for the strategy specified
--
-- Change Control
-- --------------
-- 01/07/09 jec Credit collection walkthrough changes 
-- 07/08/09 jec UAT755 CR852-Entry Condition 'X' value 
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies
-- =============================================

--exec CM_StrategyConditionGetSP '123',0

CREATE PROCEDURE CM_StrategyConditionGetSP
	@Strategy varchar(7),		-- #9521
    @return INT OUTPUT
AS
SET @return = 0
SELECT Strategy
      ,s.Condition AS 'ConditionCode'      
      ,Operator1
      ,Operator2      
      ,StepActionType
      ,Step
      ,Case		-- include ActionCode in Condition Description
		when isnull(StepActionType,'')='P' and s.Condition='PrevStrat' then replace(Description,'was:','was: '+ ActionCode)
		when isnull(StepActionType,'')='P' and s.Condition='STRX' then replace(Description,'X', ActionCode)		--UAT755 jec
		else Description End AS 'Condition'
      ,Operand
      ,Type
      ,NextStepTrue
      ,s.NextStepFalse
      ,OrClause
      ,FalseStep
	  ,AllowReuse
	  ,SavedType
      ,ActionCode
  into #tempcondition		-- use temporary table
  FROM dbo.CMStrategyCondition s LEFT OUTER JOIN dbo.CMCondition c ON s.Condition = c.Condition
  WHERE Strategy =@Strategy
  
    -- update any condition using country parameters
declare @mths varchar(2)
select @mths=cast(value as varchar(2)) from countrymaintenance where codename='mthsreposs'
-- Step P months no Movement 
update #tempcondition
	set Condition=replace(Condition,'P',@mths) where conditionCode='NoMove'
	

select * from #tempcondition
  ORDER BY Step

  SET @return = @@error

GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
