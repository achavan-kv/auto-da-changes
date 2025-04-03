
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary15SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary15SP
END
GO

CREATE PROCEDURE dbo.Summary15SP
/*
** Author	: K. E. Fernandez (Strategic Thought)
** Date		: 17-Aug-2004
** Version	: 1.0
** Name		: Summary Table 15
** Details	: Created to generate summary table specifically for the Housekeeping Report - AOB Breakdown
**
** Who  Date     Change
** ---	----	 ------								
*/
--========================================================================================
			@return int
as 

set @return=0

TRUNCATE TABLE summary15
--go

--Charges
insert into summary15 (report, acctno, outstbal, outstbalcorr, charges, securitised, asatdate)
SELECT 
	'Charges', f.acctno, outstbal, outstbal - SUM(transvalue), SUM(transvalue), securitised, getdate()
FROM 	acct a, fintrans f
WHERE 	a.acctno = f.acctno
AND	transtypecode in ('INT','ADM','FEE','TRC','INS','DDF')
AND	currstatus <> 'S'
AND	outstbal > 0
AND	accttype not in ('C','S')
AND	(agrmttotal > 0 or accttype = 'T')
GROUP BY f.acctno, outstbal, securitised
HAVING 	outstbal <= SUM(transvalue) 
--go

--update securitised column with N
update	summary15
set	securitised = 'N'
where	isnull(securitised,'') <> 'Y'
--go

set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End