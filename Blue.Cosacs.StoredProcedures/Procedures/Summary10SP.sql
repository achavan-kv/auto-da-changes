
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary10SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary10SP
END
GO

CREATE PROCEDURE dbo.Summary10SP
/*
** Author	: K. E. Fernandez (Strategic Thought)
** Date		: 27-May-2004
** Version	: 1.0
** Name		: Summary Table 10
** Details	: Created to generate summary table specifically for Seasoned Report
**
** Who  Date     Change
** ---	----	 ------								
*/
--========================================================================================
			@return int
as 

set @return=0

TRUNCATE TABLE summary10
--GO

insert into summary10
	(acctno, datesecuritised, datetrans, transtypecode,
	transvalue, asatdate)
select	f.acctno, datesecuritised, datetrans, transtypecode,
	transvalue, getdate()
from	sec_account s, fintrans f
where	s.acctno = f.acctno
--GO


set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End