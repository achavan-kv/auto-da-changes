SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[StoreCardBranchQualRulesGet]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[StoreCardBranchQualRulesGet]
GO

CREATE PROCEDURE StoreCardBranchQualRulesGet
-- **********************************************************************
-- Title: StoreCardBranchQualRulesGet.sql
-- Developer: Ilyas Parker
-- Date: 8/12/2010
-- Purpose: Get Store Card qualification rules for a branch to be displayed
--			in the Branch Maintenance screen.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 8/12/10   IP  Created
-- 21/04/11  IP  Feature 3000 - Store Card Branch Qualification rule changes.
-- 10/05/11  IP  Feature 2593 - Retrieve new added column MaxNoCustForApproval
-- **********************************************************************
    @branchNo smallint
 
AS
	 
	SELECT	MinApplicationScore,
		    MinBehaviouralScore,
		    MinMthsAcctHistX,				--IP - 21/04/11
		    MinMthsAcctHistY,				--IP - 21/04/11
		    MaxCurrMthsInArrs,
		    MaxPrevMthsInArrsX,				--IP - 21/04/11
		    MaxPrevMthsInArrsY,				--IP - 21/04/11
		    PcentInitRFLimit,				--IP - 21/04/11	
		    MaxNoCustForApproval			--IP - 10/05/11
		   
	FROM   StoreCardBranchQualRules
	WHERE  BranchNo = @branchNo

	   
	   
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
