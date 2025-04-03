


SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[StoreCardBranchQualRulesSave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[StoreCardBranchQualRulesSave]
GO

CREATE PROCEDURE StoreCardBranchQualRulesSave
-- **********************************************************************
-- Title: StoreCardBranchQualRulesSave.sql
-- Developer: Ilyas Parker
-- Date: 7/12/2010
-- Purpose: Save Store Card qualification rules for a branch

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 7/12/10   IP  Created
-- 21/04/11  IP  Feature 3000 - Store Card Branch Qualification rule changes.
-- 10/05/11  IP  Feature 3593 - Update new column MaxNoCustForApproval
-- **********************************************************************
    @branchNo smallint,
    @minApplicationScore int = null,
    @minBehaviouralScore int = null,
    @minMthsAcctHistX int = null,			--IP - 21/04/11
    @minMthsAcctHistY int = null,			--IP - 21/04/11
    @maxCurrMthsInArrs DECIMAL(5,2) = null,
    @maxPrevMthsInArrsX DECIMAL(5,2) = null,			--IP - 21/04/11
    @maxPrevMthsInArrsY int = null,			--IP - 21/04/11
    @pcentInitRFLimit float = null,			--IP - 21/04/11
    @maxNoCustForApproval int = null,		--IP - 10/05/11
    @dateChanged datetime,
    @empeenoChange int = null,
    @applyTo varchar(15)
 

AS
 

   IF(@applyTo = 'All')
   BEGIN
   
			
		--Update the values for all existing records for branches
		UPDATE StoreCardBranchQualRules
		SET	MinApplicationScore = @minApplicationScore,
				MinBehaviouralScore = @minBehaviouralScore,
				MinMthsAcctHistX = @minMthsAcctHistX,		  --IP - 21/04/11 		
				MinMthsAcctHistY = @minMthsAcctHistY,		  --IP - 21/04/11 
				MaxCurrMthsInArrs = @maxCurrMthsInArrs,
				MaxPrevMthsInArrsX = @maxPrevMthsInArrsX,	  --IP - 21/04/11 
				MaxPrevMthsInArrsY = @maxPrevMthsInArrsY,	  --IP - 21/04/11 
				PcentInitRFLimit = @pcentInitRFLimit,		  --IP - 21/04/11
				MaxNoCustForApproval = @maxNoCustForApproval, --IP - 10/05/11
				EmpeenoChange = @empeenoChange,
				DateChanged = @dateChanged
		WHERE MinApplicationScore != @minApplicationScore
		OR	 MinBehaviouralScore != @minBehaviouralScore
		OR   MinMthsAcctHistX != @minMthsAcctHistX			--IP - 21/04/11
		OR   MinMthsAcctHistY != @minMthsAcctHistY			--IP - 21/04/11
		OR   MaxCurrMthsInArrs != @maxCurrMthsInArrs
		OR   MaxPrevMthsInArrsX != @maxPrevMthsInArrsX		--IP - 21/04/11
		OR   MaxPrevMthsInArrsY != @maxPrevMthsInArrsY		--IP - 21/04/11	
		OR   PcentInitRFLimit != @pcentInitRFLimit			--IP - 21/04/11	
		OR	 MaxNoCustForApproval != @maxNoCustForApproval  --IP - 10/05/11 
		
	
		--Finally insert a new record for the branches that do not have any rules defined
		IF EXISTS(select * from Branch b 
					where not exists(select * from StoreCardBranchQualRules s
										where s.BranchNo = b.BranchNo))
		BEGIN
		
				
				--Insert the rules			
				INSERT INTO StoreCardBranchQualRules
					(
						BranchNo,
						MinApplicationScore,
						MinBehaviouralScore,
						MinMthsAcctHistX,		--IP - 21/04/11
						MaxCurrMthsInArrs,
						MaxPrevMthsInArrsX,		--IP - 21/04/11
						PcentInitRFLimit,		--IP - 21/04/11
						EmpeenoChange,
						DateChanged,
						MinMthsAcctHistY,		--IP - 21/04/11
						MaxPrevMthsInArrsY,		--IP - 21/04/11
						MaxNoCustForApproval	--IP - 10/05/11
					)
			 SELECT
						b.BranchNo,
						@minApplicationScore,
						@minBehaviouralScore,
						@minMthsAcctHistX,		--IP - 21/04/11
						@maxCurrMthsInArrs,
						@maxPrevMthsInArrsX,	--IP - 21/04/11
						@pcentInitRFLimit,		--IP - 21/04/11
						@empeenoChange,
						@dateChanged,
						@minMthsAcctHistY,		--IP - 21/04/11
						@maxPrevMthsInArrsY,	--IP - 21/04/11
						@maxNoCustForApproval	--IP - 10/05/11
			FROM Branch b 
			WHERE NOT EXISTS(select * from StoreCardBranchQualRules s
							where s.BranchNo = b.BranchNo)
							
			
		         
		END
   END
   
   
   IF(@applyTo = 'Current')
   BEGIN

		--First update the values for all existing records for branches
		UPDATE StoreCardBranchQualRules
		SET	MinApplicationScore = @minApplicationScore,
				MinBehaviouralScore = @minBehaviouralScore,
				MinMthsAcctHistX = @minMthsAcctHistX,		--IP - 21/04/11
				MinMthsAcctHistY = @minMthsAcctHistY,		--IP - 21/04/11
				MaxCurrMthsInArrs = @maxCurrMthsInArrs,
				MaxPrevMthsInArrsX = @maxPrevMthsInArrsX,	--IP - 21/04/11
				MaxPrevMthsInArrsY = @maxPrevMthsInArrsY,	--IP - 21/04/11
				PcentInitRFLimit = @pcentInitRFLimit,
				MaxNoCustForApproval = @maxNoCustForApproval,--IP - 10/05/11		
				EmpeenoChange = @empeenoChange,
				DateChanged = @dateChanged
		WHERE BranchNo = @branchNo
		AND (MinApplicationScore != @minApplicationScore
		OR	MinBehaviouralScore != @minBehaviouralScore
		OR  MinMthsAcctHistX != @minMthsAcctHistX			--IP - 21/04/11
		OR  MinMthsAcctHistY != @minMthsAcctHistY			--IP - 21/04/11
		OR  MaxCurrMthsInArrs != @maxCurrMthsInArrs
		OR  MaxPrevMthsInArrsX != @maxPrevMthsInArrsX		--IP - 21/04/11
		OR  MaxPrevMthsInArrsY != @maxPrevMthsInArrsY		--IP - 21/04/11
		OR  PcentInitRFLimit != @pcentInitRFLimit			--IP - 21/04/11
		OR  MaxNoCustForApproval != @maxNoCustForApproval)	--IP - 10/05/11
		
		--If no records exist insert a new record for the branch
		IF NOT EXISTS (SELECT * FROM StoreCardBranchQualRules s
						INNER JOIN Branch b on s.BranchNo = b.BranchNo
						AND S.BranchNo = @branchNo)
						
		BEGIN		
			
			INSERT INTO StoreCardBranchQualRules
		        (
					BranchNo,
					MinApplicationScore,
					MinBehaviouralScore,
					MinMthsAcctHistX,
					MaxCurrMthsInArrs,
					MaxPrevMthsInArrsX,
					PcentInitRFLimit,
					EmpeenoChange,
					DateChanged,
					MinMthsAcctHistY,
					MaxPrevMthsInArrsY,
					MaxNoCustForApproval	--IP - 10/05/11
		        )
		VALUES  ( 
					@branchNo,
					@minApplicationScore,
					@minBehaviouralScore,
					@minMthsAcctHistX,		--IP - 21/04/11
					@maxCurrMthsInArrs,
					@maxPrevMthsInArrsX,	--IP - 21/04/11
					@pcentInitRFLimit,		--IP - 21/04/11
					@empeenoChange,
					@dateChanged,
					@minMthsAcctHistY,		--IP - 21/04/11
					@maxPrevMthsInArrsY,	--IP - 21/04/11
					@maxNoCustForApproval	--IP - 10/05/11
		        )
		     
		END
	
   END
   
    IF(@applyTo = 'Courts')
    BEGIN
		

		--First update the values for all existing records for branches
		UPDATE StoreCardBranchQualRules
		SET	MinApplicationScore = @minApplicationScore,
				MinBehaviouralScore = @minBehaviouralScore,
				MinMthsAcctHistX = @minMthsAcctHistX,			--IP - 21/04/11
				MinMthsAcctHistY = @minMthsAcctHistY,			--IP - 21/04/11
				MaxCurrMthsInArrs = @maxCurrMthsInArrs,
				MaxPrevMthsInArrsX = @maxPrevMthsInArrsX,		--IP - 21/04/11
				MaxPrevMthsInArrsY = @maxPrevMthsInArrsY,		--IP - 21/04/11
				PcentInitRFLimit = @pcentInitRFLimit,			--IP - 21/04/11
				MaxNoCustForApproval = @maxNoCustForApproval,   --IP - 10/05/11
				EmpeenoChange = @empeenoChange,
				DateChanged = @dateChanged
		FROM StoreCardBranchQualRules s inner join branch b on s.BranchNo = b.BranchNo
		AND b.StoreType = 'C'
		WHERE MinApplicationScore != @minApplicationScore
		OR	  MinBehaviouralScore != @minBehaviouralScore
		OR	  MinMthsAcctHistX != @minMthsAcctHistX				--IP - 21/04/11
		OR	  MinMthsAcctHistY != @minMthsAcctHistY				--IP - 21/04/11
		OR	  MaxCurrMthsInArrs != @maxCurrMthsInArrs
		OR	  MaxPrevMthsInArrsX != @maxPrevMthsInArrsX			--IP - 21/04/11
		OR	  MaxPrevMthsInArrsY != @maxPrevMthsInArrsY			--IP - 21/04/11
		OR	  PcentInitRFLimit != @pcentInitRFLimit				--IP - 21/04/11
		OR    MaxNoCustForApproval != @maxNoCustForApproval		--IP - 10/05/11
		
		
		--Finally insert a new record for the branches that do not have any rules defined
		IF EXISTS(select * from Branch b 
					where not exists(select * from StoreCardBranchQualRules s
										where s.BranchNo = b.BranchNo
										and b.StoreType = 'C'))
		BEGIN
		
		
				INSERT INTO StoreCardBranchQualRules
					(
						BranchNo,
						MinApplicationScore,
						MinBehaviouralScore,
						MinMthsAcctHistX,			--IP - 21/04/11
						MaxCurrMthsInArrs,
						MaxPrevMthsInArrsX,			--IP - 21/04/11
						PcentInitRFLimit,			--IP - 21/04/11
						EmpeenoChange,
						DateChanged,
						MinMthsAcctHistY,			--IP - 21/04/11
						MaxPrevMthsInArrsY,			--IP - 21/04/11
						MaxNoCustForApproval		--IP - 10/05/11
					)		
			 SELECT
						b.BranchNo,
						@minApplicationScore,
						@minBehaviouralScore,
						@minMthsAcctHistX,			--IP - 21/04/11
						@maxCurrMthsInArrs,
						@maxPrevMthsInArrsX,		--IP - 21/04/11
						@pcentInitRFLimit,			--IP - 21/04/11
						@empeenoChange,
						@dateChanged,
						@minMthsAcctHistY,			--IP - 21/04/11
						@maxPrevMthsInArrsY,		--IP - 21/04/11
						@maxNoCustForApproval		--IP - 10/05/11
			FROM Branch b 
			WHERE NOT EXISTS(select * from StoreCardBranchQualRules s
							where s.BranchNo = b.BranchNo)
			AND b.StoreType = 'C'
		END		
    END
    
    IF(@applyTo = 'NonCourts')
    BEGIN
		

		--First update the values for all existing records for branches
		UPDATE StoreCardBranchQualRules
		SET	MinApplicationScore = @minApplicationScore,
				MinBehaviouralScore = @minBehaviouralScore,
				MinMthsAcctHistX = @minMthsAcctHistX,			--IP - 21/04/11
				MinMthsAcctHistY = @minMthsAcctHistY,			--IP - 21/04/11
				MaxCurrMthsInArrs = @maxCurrMthsInArrs,
				MaxPrevMthsInArrsX = @maxPrevMthsInArrsX,		--IP - 21/04/11
				MaxPrevMthsInArrsY = @maxPrevMthsInArrsY,		--IP - 21/04/11
				PcentInitRFLimit = @pcentInitRFLimit,			--IP - 21/04/11
				MaxNoCustForApproval = @maxNoCustForApproval,	--IP - 10/05/11
				EmpeenoChange = @empeenoChange,
				DateChanged = @dateChanged
		FROM StoreCardBranchQualRules s inner join branch b on s.BranchNo = b.BranchNo
		AND b.StoreType = 'N'
		WHERE MinApplicationScore != @minApplicationScore
		OR	  MinBehaviouralScore != @minBehaviouralScore
		OR	  MinMthsAcctHistX != @minMthsAcctHistX				--IP - 21/04/11
		OR	  MinMthsAcctHistY != @minMthsAcctHistY				--IP - 21/04/11
		OR	  MaxCurrMthsInArrs != @maxCurrMthsInArrs
		OR	  MaxPrevMthsInArrsX != @maxPrevMthsInArrsX			--IP - 21/04/11
		OR	  MaxPrevMthsInArrsY != @maxPrevMthsInArrsY			--IP - 21/04/11
		OR	  PcentInitRFLimit != @pcentInitRFLimit				--IP - 21/04/11
		OR	  MaxNoCustForApproval != @maxNoCustForApproval		--IP - 10/05/11
		
		
		--Finally insert a new record for the branches that do not have any rules defined
		IF EXISTS(select * from Branch b 
					where not exists(select * from StoreCardBranchQualRules s
										where s.BranchNo = b.BranchNo
										and b.StoreType = 'N'))
		BEGIN
		
		
				INSERT INTO StoreCardBranchQualRules
					(
						BranchNo,
						MinApplicationScore,
						MinBehaviouralScore,
						MinMthsAcctHistX,			--IP - 21/04/11
						MaxCurrMthsInArrs,
						MaxPrevMthsInArrsX,			--IP - 21/04/11
						PcentInitRFLimit,			--IP - 21/04/11
						EmpeenoChange,
						DateChanged,
						MinMthsAcctHistY,			--IP - 21/04/11
						MaxPrevMthsInArrsY,			--IP - 21/04/11
						MaxNoCustForApproval		--IP - 10/05/11
					)
			 SELECT
						b.BranchNo,
						@minApplicationScore,
						@minBehaviouralScore,
						@minMthsAcctHistX,			--IP - 21/04/11
						@maxCurrMthsInArrs,
						@maxPrevMthsInArrsX,		--IP - 21/04/11
						@pcentInitRFLimit,			--IP - 21/04/11
						@empeenoChange,
						@dateChanged,
						@minMthsAcctHistY,			--IP - 21/04/11
						@maxPrevMthsInArrsY,		--IP - 21/04/11
						@maxNoCustForApproval		--IP - 10/05/11
			FROM Branch b 
			WHERE NOT EXISTS(select * from StoreCardBranchQualRules s
							where s.BranchNo = b.BranchNo)
			AND b.StoreType = 'N'
		END		
    END
   
    --Now finally delete the entries for branches where the rules have been disabled.
    DELETE FROM StoreCardBranchQualRules
    WHERE MinApplicationScore = NULL
    AND	  MinBehaviouralScore = NULL
    AND   MinMthsAcctHistX = NULL
    AND	  MinMthsAcctHistY = NULL
    AND   MaxCurrMthsInArrs = NULL
    AND   MaxPrevMthsInArrsX = NULL
    AND   MaxPrevMthsInArrsY = NULL
    AND   PcentInitRFLimit = NULL
    AND   MaxNoCustForApproval = NULL	--IP - 10/05/11

   
    

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO