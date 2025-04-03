
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary7SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary7SP
END
GO

CREATE PROCEDURE dbo.Summary7SP
/*
** Author	: A. Ayscough and K. Fernandez (Strategic Thought)
** Date		: 18-May-2004
** Version	: 1.0
** Name		: Summary Table 7
** Details	: CR603 - Report to show non-securitised accounts and reason why
**
** Who  Date     Change
** ---	----	 ------								
** AA   20/05/04 If there are no accounts securitised then removing all fully delivered accounts
*/
--========================================================================================
			@return int
as 

set @return=0
/* Summary7 table is populated from a stored procedure: Report15NonSecuritiseAccounts
** This is called from CRTSecuritise from the EOD 'Securitisation' job in the main CoSACS image */

/* Also need to call from 'Creating Report Summary Data' job */

if (select countrycode from country) = 'S'
begin
    --declare @return int
    execute Report15NonSecuritiseAccounts @return = @return out
end
--go


set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End