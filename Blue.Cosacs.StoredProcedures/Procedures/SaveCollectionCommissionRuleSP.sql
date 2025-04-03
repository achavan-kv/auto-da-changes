SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[SaveCollectionCommissionRuleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveCollectionCommissionRuleSP]
GO

CREATE PROCEDURE SaveCollectionCommissionRuleSP
-- **********************************************************************
-- Title: SaveCollectionCommissionRuleSP.sql
-- Developer: Ilyas Parker
-- Date: 16/06/2010
-- Purpose: Saves the Collection Commission Rule and updates the audit table
--			based on whether an insert, deletion or update.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/06/10 IP  Created
-- **********************************************************************
	@id INT,
    @ruleName VARCHAR(50),
    @empeeType  VARCHAR(3),
    @commissionType VARCHAR(1),
    @pCentArrearsColl FLOAT,
    @pCentOfCalls FLOAT,
    @pCentOfWorklist FLOAT,
    @noOfCalls INT,
    @noOfDaysSinceAction INT,
    @noTimeFrameDays INT,
    @minBal MONEY,
    @maxBal MONEY,
    @minValColl MONEY,
    @maxValColl MONEY,
    @minMnthsArrears INT,
    @maxMnthsArrears INT,
    @pCentCommOnArrears FLOAT,
    @pCentCommOnAmtPaid FLOAT,
    @pCentCommOnFee FLOAT,
    @commSetVal MONEY,
    @empeenoChange INT,
    @ruleID INT OUTPUT,					
    @ruleDatechanged DATETIME OUTPUT,			
    @return int OUTPUT

AS
    SET     @return = 0
    
	SET @ruleDatechanged = GETDATE()
	
    DECLARE @rowCount int
    SET     @rowCount =  0

    SELECT  @rowCount = count(*)
    FROM    CollectionCommnRules
    WHERE   id = @id
    --WHERE   RuleName = @ruleName
    --AND		Empeetype = @empeeType

	--This is an existing rule (therefore update).
    IF @rowCount > 0
    BEGIN
		
		--Only insert a record into the audit table for an update if a parameter on the rule has been modified.
		
		INSERT INTO CollectionCommnRuleAudit
		
		        ( ParentID,
		          RuleName ,
		          Empeetype ,
		          CommissionType ,
		          PcentArrearsColl ,
		          PcentOfCalls ,
		          PcentOfWorklist ,
		          NoOfCalls ,
		          NoOfDaysSinceAction,
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
		SELECT @id,
			   @ruleName,
			   @empeeType,
			   @commissionType,
			   @pCentArrearsColl,
			   @pCentOfCalls,
			   @pCentOfWorklist,
			   @noOfCalls,
			   @noOfDaysSinceAction,
			   @noTimeFrameDays,
			   @minBal,
			   @maxBal,
			   @minValColl,
			   @maxValColl,
			   @minMnthsArrears,
			   @maxMnthsArrears,
			   @pCentCommOnArrears,
			   @pCentCommOnAmtPaid,
			   @pCentCommOnFee,
			   @commSetVal,
			   @empeenoChange,
			   @ruleDatechanged,
			   'U'
		FROM	CollectionCommnRules c
		WHERE	c.id = @id
		AND		(c.CommissionType != @commissionType
					OR c.PcentArrearsColl != @pCentArrearsColl
					OR c.PcentOfCalls != @pCentOfCalls
					OR c.PcentOfWorklist != @pCentOfWorklist
					OR c.NoOfCalls != @noOfCalls
					OR c.NoOfDaysSinceAction != @noOfDaysSinceAction
					OR c.TimeFrameDays != @noTimeFrameDays
					OR c.MinBal != @minBal
					OR c.MaxBal != @maxBal
					OR c.MinValColl != @minValColl
					OR c.MaxValColl != @maxValColl
					OR c.MinMnthsArrears != @minMnthsArrears
					OR c.MaxMnthsArrears != @maxMnthsArrears
					OR c.PcentCommOnArrears != @pCentCommOnArrears
					OR c.PcentCommOnAmtPaid != @pCentCommOnAmtPaid
					OR c.PcentCommOnFee != @pCentCommOnFee
					OR c.CommSetVal != @commSetVal)
				
		IF(@@ROWCOUNT!=0)
		BEGIN		
					
			UPDATE CollectionCommnRules
			SET	CommissionType = @commissionType,
				PcentArrearsColl = @pCentArrearsColl,
				PcentOfCalls = @pCentOfCalls,
				PcentOfWorklist = @pCentOfWorklist,
				NoOfCalls = @noOfCalls,
				NoOfDaysSinceAction = @noOfDaysSinceAction,
				TimeFrameDays = @noTimeFrameDays,
				MinBal = @minBal,
				MaxBal = @maxBal,
				MinValColl = @minValColl,
				MaxValColl = @maxValColl,
				MinMnthsArrears = @minMnthsArrears,
				MaxMnthsArrears = @maxMnthsArrears,
				PcentCommOnArrears = @pCentCommOnArrears,
				PcentCommOnAmtPaid = @pCentCommOnAmtPaid,
				PcentCommOnFee = @pCentCommOnFee,
				CommSetVal = @commSetVal
			WHERE id = @id
		END
	
	
    END
    ELSE
    BEGIN
    
		INSERT INTO CollectionCommnRules
		        ( RuleName ,
		          EmpeeType ,
		          CommissionType,
		          PcentArrearsColl ,
		          PcentOfCalls ,
		          PcentOfWorklist ,
		          NoOfCalls ,
		          NoOfDaysSinceAction,
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
		          CommSetVal
		        )
		VALUES  ( 
					@ruleName,
					@empeeType,
					@commissionType,
					@pCentArrearsColl,
					@pCentOfCalls,
					@pCentOfWorklist,
					@noOfCalls,
					@noOfDaysSinceAction,
					@noTimeFrameDays,
					@minBal,
					@maxBal,
					@minValColl,
					@maxValColl,
					@minMnthsArrears,
					@maxMnthsArrears,
					@pCentCommOnArrears,
					@pCentCommOnAmtPaid,
					@pCentCommOnFee,
					@commSetVal
		        )
		 --Select the id of the new rule just added.
		 SET @id =  (SELECT MAX(ID) FROM CollectionCommnRules)
		     
		 INSERT INTO CollectionCommnRuleAudit
		         ( 
				   ParentID,
				   RuleName,
		           EmpeeType,
		           CommissionType,
		           PcentArrearsColl,
		           PcentOfCalls,
		           PcentOfWorklist,
		           NoOfCalls,
		           NoOfDaysSinceAction,
		           TimeFrameDays,
		           MinBal,
		           MaxBal,
		           MinValColl,
		           MaxValColl,
		           MinMnthsArrears,
		           MaxMnthsArrears,
		           PcentCommOnArrears,
		           PcentCommOnAmtPaid,
		           PcentCommOnFee,
		           CommSetVal,
		           EmpeenoChange,
		           DateChanged,
		           AuditType
		         )
		 VALUES  ( 
					@id,
					@ruleName,
					@empeeType,
					@commissionType,
					@pCentArrearsColl,
					@pCentOfCalls,
					@pCentOfWorklist,
					@noOfCalls,
					@noOfDaysSinceAction,
					@noTimeFrameDays,
					@minBal,
					@maxBal,
					@minValColl,
					@maxValColl,
					@minMnthsArrears,
					@maxMnthsArrears,
					@pCentCommOnArrears,
					@pCentCommOnAmtPaid,
					@pCentCommOnFee,
					@commSetVal,
					@empeenoChange,
					@ruleDatechanged,
					'I'
		         )

    END
    
    --Return the id of the rule inserted/updated
    SET @ruleID = @id
    
   

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
