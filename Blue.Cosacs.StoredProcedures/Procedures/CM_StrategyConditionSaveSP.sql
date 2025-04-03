
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_StrategyConditionSaveSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_StrategyConditionSaveSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Saves strategy condition details to the CMStrategyCondition table
-- Change Control
-- --------------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies	
-- =============================================
CREATE procedure [dbo].[CM_StrategyConditionSaveSP]
   @Strategy varchar(7),		-- #9521
   @Condition varchar(12),
   @Operand varchar(10),
   @Operator1 varchar(24),
   @Operator2 varchar(24),
   @OrClause char(1),  --  can be A/B/C/etc - used to match
   @NextStepTrue smallint,
   @NextStepFalse smallint,
   @ActionCode varchar(50),
   @StepActionType char(1),
   @Step smallint,
   @SavedType char(1),
   @return int OUT
AS
SET @return = 0

--UPDATE dbo.CMStrategyCondition
--   SET Strategy = @Strategy 
--      ,Condition = @Condition 
--      ,Operand = @Operand 
--      ,Operator1 = @Operator1
--      ,Operator2 = @Operator2 
--      ,OrClause = @OrClause
--      ,NextStepTrue = @NextStepTrue
--      ,NextStepFalse = @NextStepFalse
--      ,ActionCode = @ActionCode
--      ,StepActionType=@StepActionType
--      ,Step = @Step
-- WHERE Strategy = @Strategy and Condition= @Condition
--
-- IF @@rowcount = 0 AND @@error = 0

	INSERT INTO dbo.CMStrategyCondition
           (Strategy
           ,Condition
           ,Operand
           ,Operator1
           ,Operator2
           ,OrClause
            ,NextStepTrue
            ,NextStepFalse
            ,ActionCode
            ,StepActionType
            ,Step
            ,SavedType)
     VALUES
           (@Strategy
           ,@Condition
           ,@Operand
           ,@Operator1
           ,@Operator2
           ,@OrClause
           ,@NextStepTrue
           ,@NextStepFalse
           ,@ActionCode
           ,@StepActionType
           ,@Step
           ,@SavedType )

   SET @return = @@error
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End