
IF EXISTS (select * from sysobjects where name = 'Trig_StoreCardBranchQualRules_InsertUpdate')
drop trigger Trig_StoreCardBranchQualRules_InsertUpdate
GO

-- **********************************************************************
-- Title: StoreCardBranchQualRulesSave.sql
-- Developer: Ilyas Parker
-- Date: 9/12/2010
-- Purpose: Trigger will write an entry into the StoreCardBranchQualRulesAudit table
--			for any inserts or updates to the StoreCardBranchQualRules table.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 9/12/10   IP  Created
-- 26/04/11  IP  Feature 3001 - Audit Storecard Qualification Criteria changes.
-- 10/05/11  IP  Feature 3593 - NChanges required to cater for new column MaxNoCustForApproval
-- **********************************************************************

CREATE Trigger Trig_StoreCardBranchQualRules_InsertUpdate ON StoreCardBranchQualRules
For UPDATE, INSERT

AS


	INSERT INTO StoreCardBranchQualRulesAudit(BranchNo, 
											  MinApplicationScore, 
											  MinBehaviouralScore,
											  MinMthsAcctHistX,				--IP - 26/04/11
											  MaxCurrMthsInArrs,
											  MaxPrevMthsInArrsX,			--IP - 26/04/11
											  PcentInitRFLimit,				--IP - 26/04/11
											  EmpeenoChange,
											  DateChanged,
											  MinMthsAcctHistY,				--IP - 26/04/11
											  MaxPrevMthsInArrsY,			--IP - 26/04/11
											  MaxNoCustForApproval)			--IP - 10/05/11


	SELECT I.BranchNo,
		   I.MinApplicationScore,
		   I.MinBehaviouralScore,
		   I.MinMthsAcctHistX,			--IP - 26/04/11
		   I.MaxCurrMthsInArrs,
		   I.MaxPrevMthsInArrsX,		--IP - 26/04/11
		   I.PcentInitRFLimit,			--IP - 26/04/11
		   I.EmpeenoChange,
		   I.DateChanged,
		   I.MinMthsAcctHistY,			--IP - 26/04/11
		   I.MaxPrevMthsInArrsY,		--IP - 26/04/11
		   I.MaxNoCustForApproval		--IP - 10/05/11

	FROM INSERTED I
	WHERE NOT EXISTS(SELECT * FROM StoreCardBranchQualRulesAudit S
					WHERE S.BranchNo = I.BranchNo)

	INSERT INTO StoreCardBranchQualRulesAudit(BranchNo, 
											  MinApplicationScore, 
											  MinBehaviouralScore,
											  MinMthsAcctHistX,			--IP - 26/04/11
											  MaxCurrMthsInArrs,
											  MaxPrevMthsInArrsX,		--IP - 26/04/11
											  PcentInitRFLimit,			--IP - 26/04/11
											  EmpeenoChange,
											  DateChanged,
											  MinMthsAcctHistY,			--IP - 26/04/11
											  MaxPrevMthsInArrsY,		--IP - 26/04/11
											  MaxNoCustForApproval)		--IP - 10/05/11


	SELECT I.BranchNo,
		   I.MinApplicationScore,
		   I.MinBehaviouralScore,
		   I.MinMthsAcctHistX,			--IP - 26/04/11
		   I.MaxCurrMthsInArrs,
		   I.MaxPrevMthsInArrsX,		--IP - 26/04/11
		   I.PcentInitRFLimit,			--IP - 26/04/11
		   I.EmpeenoChange,
		   I.DateChanged,
		   I.MinMthsAcctHistY,			--IP - 26/04/11
		   I.MaxPrevMthsInArrsY,		--IP - 26/04/11
		   I.MaxNoCustForApproval		--IP - 10/05/11

	FROM INSERTED I, DELETED D
	WHERE I.BranchNo = D.BranchNo
	AND (isnull(I.MinApplicationScore,0) != isnull(D.MinApplicationScore,0)
	OR	 isnull(I.MinBehaviouralScore,0) != isnull(D.MinBehaviouralScore,0)
	OR   isnull(I.MinMthsAcctHistX,0) != isnull(D.MinMthsAcctHistX,0)			--IP - 26/04/11
	OR	 isnull(I.MinMthsAcctHistY,0) != isnull(D.MinMthsAcctHistY,0)			--IP - 26/04/11
	OR	 isnull(I.MaxCurrMthsInArrs,0) !=  isnull(D.MaxCurrMthsInArrs,0)
	OR	 isnull(I.MaxPrevMthsInArrsX,0) !=  isnull(D.MaxPrevMthsInArrsX,0)		--IP - 26/04/11
	OR	 isnull(I.MaxPrevMthsInArrsY,0) !=  isnull(D.MaxPrevMthsInArrsY,0)		--IP - 26/04/11
	OR	 isnull(I.PcentInitRFLimit,0)	!=	isnull(D.PcentInitRFLimit,0)		--IP - 26/04/11
	OR   isnull(I.MaxNoCustForApproval,0) != isnull(D.MaxNoCustForApproval,0))	--IP - 10/05/11

	
