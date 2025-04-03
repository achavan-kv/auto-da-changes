SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[DeleteCollectionCommissionRuleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteCollectionCommissionRuleSP]
GO

CREATE PROCEDURE DeleteCollectionCommissionRuleSP
    @ruleID INT,
    @empeenoChange INT,
    @return int OUTPUT
    
-- **********************************************************************
-- Title: DeleteCollectionCommissionRuleSP.sql
-- Developer: Ilyas Parker
-- Date: 16/06/2010
-- Purpose: Deletes a Collection Commission rule.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/006/10 IP  Created
-- 08/07/10  IP  UAT(1091) - Missing column TimeFrameDays
-- **********************************************************************

AS
    SET     @return = 0

	DECLARE @dateChanged DATETIME
	
	SET @dateChanged = GETDATE()
	
	--If there are actions, first delete these.

		INSERT INTO CollectionCommnRuleActionsAudit
		        ( ParentID ,
		          ActionTaken ,
		          EmpeenoChange,
		          DateChanged ,
		          AuditType
		        )
		SELECT  c.ParentID,
				c.ActionTaken,
				@empeenoChange,
				@dateChanged,
				'D'
		FROM CollectionCommnRuleActions c
		WHERE c.ParentID = @ruleID
		
		IF(@@ROWCOUNT!=0)
		BEGIN
			DELETE FROM CollectionCommnRuleActions
			WHERE ParentID = @ruleID
		END

	
		INSERT INTO CollectionCommnRuleAudit
	        ( ParentID ,
	          RuleName ,
	          EmpeeType ,
	          CommissionType ,
	          PcentArrearsColl ,
	          PcentOfCalls ,
	          PcentOfWorklist ,
	          NoOfCalls ,
	          NoOfDaysSinceAction ,
	          TimeFrameDays,
	          MinBal ,
	          MaxBal ,
	          MinValColl ,
	          MaxValColl ,
	          MinMnthsArrears ,
	          MaxMnthsArrears ,
	          PcentCommOnArrears ,
	          PcentCommOnAmtPaid ,
	          PcentCommOnFee,
	          CommSetVal ,
	          EmpeenoChange ,
	          DateChanged ,
	          AuditType
	        )
		SELECT c.ID,
		   c.RuleName,
		   c.EmpeeType,
		   c.CommissionType,
		   c.PcentArrearsColl,
		   c.PcentOfCalls,
		   c.PcentOfWorklist,
		   c.NoOfCalls,
		   c.NoOfDaysSinceAction,
		   c.TimeFrameDays,
		   c.MinBal,
		   c.MaxBal,
		   c.MinValColl,
		   c.MaxValColl,
		   c.MinMnthsArrears,
		   c.MaxMnthsArrears,
		   c.PcentCommOnArrears,
		   c.PcentCommOnAmtPaid,
		   c.PcentCommOnFee,
		   c.CommSetVal,
		   @empeenoChange,
		   @dateChanged,
		   'D'
	FROM CollectionCommnRules c
	WHERE c.ID = @ruleID
	
	IF(@@ROWCOUNT!=0)
	BEGIN
    DELETE FROM CollectionCommnRules
		WHERE id = @ruleID
    END
    


GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
