SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[SaveCollectionCommissionRuleActionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveCollectionCommissionRuleActionsSP]
GO

CREATE PROCEDURE SaveCollectionCommissionRuleActionsSP
	@ruleID INT,
    @action VARCHAR(4),
    @dateChanged DATETIME,
    @empeenoChange INT,	
    @newRule BIT,
    @lastAction BIT,
    @noActionsSelected BIT,
    @return int OUTPUT
    
-- **********************************************************************
-- Title: SaveCollectionCommissionRuleActionsSP.sql
-- Developer: Ilyas Parker
-- Date: 16/06/2010
-- Purpose: Saves the actions for a Collection Commission Rule and updates the audit table.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/006/10 IP  Created
-- **********************************************************************

AS
    SET     @return = 0

   
    IF(@newRule = 1)
    BEGIN
		IF(@action!='')
		BEGIN
			INSERT INTO CollectionCommnRuleActions
					( 
						ParentID, 
						ActionTaken 
					)
			VALUES  ( 
						@ruleID,
						@action
					)
			
			INSERT INTO CollectionCommnRuleActionsAudit
					(
						ParentID ,
						ActionTaken ,
						EmpeenoChange,
						DateChanged,
						AuditType
					)
			VALUES  ( 
						@ruleID,
						@action,
						@empeenoChange,
						@datechanged,
						'I'
					)
		 END
    END
	ELSE
    BEGIN
		
			
			IF(@action!='')
			BEGIN
				INSERT INTO CollectionCommnRuleActions
						( 
							ParentID, 
							ActionTaken 
						 )
				VALUES  ( 
							@ruleID,
							@action
						)
			
				--Insert into the audit table with an AuditType = 'I' only where the most recent audit record was not an 'I'.
				INSERT INTO CollectionCommnRuleActionsAudit
					( 
						  ParentID ,
						  ActionTaken ,
						  EmpeenoChange,
						  DateChanged ,
						  AuditType
					)
				 SELECT @ruleID,
						@action,
						@empeenoChange,
						@datechanged,
						'I'
				 WHERE NOT EXISTS
				 (SELECT * FROM CollectionCommnRuleActionsAudit c
					WHERE c.ParentID = @ruleID
					AND c.ActionTaken = @action
					AND c.DateChanged = (SELECT MAX(c1.DateChanged) FROM CollectionCommnRuleActionsAudit c1
											WHERE C1.ParentID = C.ParentID
											AND C1.ActionTaken = C.ActionTaken
											)
					AND c.AuditType = 'I')
			END
		 
		 --If this is the last action that we are adding, need to check if any actions previously in the list are no longer present, and if so
		 --insert a record into the audit table with a 'D'.
		 IF(@lastAction = 1 OR @noActionsSelected = 1)
		 BEGIN
			
			SELECT ParentID, 
				   ActionTaken
			INTO #actionsToDelete
			FROM CollectionCommnRuleActionsAudit c
			WHERE c.ParentID = @ruleID
			AND c.AuditType = 'I'
			AND c.DateChanged = (SELECT MAX(c1.DateChanged) FROM CollectionCommnRuleActionsAudit C1
									WHERE c1.ParentID = c.ParentID
									AND c1.ActionTaken = c.ActionTaken)
			AND NOT EXISTS (SELECT * FROM CollectionCommnRuleActions ca
							 WHERE ca.ParentID = c.ParentID
							 AND ca.ActionTaken = c.ActionTaken)
							 
			
			INSERT INTO CollectionCommnRuleActionsAudit
			        ( ParentID ,
			          ActionTaken ,
			          EmpeenoChange,
			          DateChanged ,
			          AuditType
			        )
			
			SELECT ParentID,
				   ActionTaken,
				   @empeenoChange,
				   @datechanged,
				   'D'
		     FROM  #actionsToDelete
		
			
		 END
		 
		 

    END
    

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
