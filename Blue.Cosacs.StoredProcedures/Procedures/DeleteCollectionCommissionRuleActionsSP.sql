SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[DeleteCollectionCommissionRuleActionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DeleteCollectionCommissionRuleActionsSP]
GO

CREATE PROCEDURE DeleteCollectionCommissionRuleActionsSP
	@ruleID INT,
    @return int OUTPUT
    
-- **********************************************************************
-- Title: DeleteCollectionCommissionRuleActionsSP.sql
-- Developer: Ilyas Parker
-- Date: 16/06/2010
-- Purpose: Deletes the actions for a Collection Commission Rule before inserting 
--			the actions again. This is executed when the user modifies the actions
--			for a rule on the Collection Commission tab.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/006/10 IP  Created
-- **********************************************************************

AS
    SET     @return = 0

   DELETE FROM CollectionCommnRuleActions
   WHERE ParentID = @ruleID
   

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
