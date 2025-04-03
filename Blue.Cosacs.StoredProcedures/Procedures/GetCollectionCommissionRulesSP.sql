SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS
WHERE id = object_id('[dbo].[GetCollectionCommissionRulesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetCollectionCommissionRulesSP]
GO

CREATE PROCEDURE dbo.GetCollectionCommissionRulesSP
    @empeetype VARCHAR(3),
    @return int OUTPUT
    
-- **********************************************************************
-- Title: GetCollectionCommissionRulesSP.sql
-- Developer: Ilyas Parker
-- Date: 16/06/2010
-- Purpose: Retrieves all collection commission rules for an employee type
--			to be displayed on the Collection Commission tab.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/006/10 IP  Created
-- **********************************************************************

AS

    SET     @return = 0            --initialise return code

    SELECT  ID,
			RuleName,
			CommissionType,
			PcentArrearsColl,
			PcentOfCalls,
			PcentOfWorklist,
			NoOfCalls,
			NoOfDaysSinceAction,
			TimeFrameDays,
			round(convert(decimal,MinBal),2) as MinBal,
			round(convert(decimal,MaxBal),2) as MaxBal,
			round(convert(decimal,MinValColl),2) as MinValColl,
			round(Convert(decimal,MaxValColl),2) as MaxValColl,
			MinMnthsArrears,
			MaxMnthsArrears,
			PcentCommOnArrears,
			PcentCommOnAmtPaid,
			PcentCommOnFee,
			round(Convert(decimal,CommSetVal),2) as CommSetVal
	FROM    CollectionCommnRules
    WHERE   EmpeeType = @empeetype


	SELECT ParentID,
		   ActionTaken
	FROM CollectionCommnRuleActions c1
	INNER JOIN CollectionCommnRules c2
	ON c1.ParentID = c2.ID
	AND c2.EmpeeType = @empeetype
	
    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO