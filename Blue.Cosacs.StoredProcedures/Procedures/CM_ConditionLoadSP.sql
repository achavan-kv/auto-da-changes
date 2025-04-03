

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_ConditionLoadSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_ConditionLoadSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Returns contents of the CM Condition table
-- 
-- Change Control
-- --------------
-- 29/06/09 jec Credit collection walkthrough changes
-- =============================================
CREATE PROCEDURE CM_ConditionLoadSP
	@return INT OUTPUT
AS
SET @return = 0

SELECT Condition AS 'ConditionCode'
      ,Description AS 'Condition'
      ,QualifyingCode
      ,OperandAllowable
      ,Type
      ,FalseStep
      ,AllowReuse
into #tempcondition
  FROM CMCondition
  
-- update any condition using country parameters
declare @mths varchar(2)
select @mths=cast(value as varchar(2)) from countrymaintenance where codename='mthsreposs'
-- Step P months no Movement 
update #tempcondition
set Condition=replace(Condition,'P',@mths) where conditionCode='NoMove'

select * from #tempcondition

SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
