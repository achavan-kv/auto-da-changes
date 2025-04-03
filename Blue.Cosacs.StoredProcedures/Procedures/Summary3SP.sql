
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary3SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary3SP
END
GO

CREATE PROCEDURE dbo.Summary3SP
/*
** Created by	: M. S. Davies, M. A. King (Strategic Thought)
** Created on	: 10-Nov-1999
** Version	: 1.0
** Name		: Summary Table 3
**
** Who  Date     Description
** ---  ----     -----------
** CJB  20/06/00 Add countrycode
** KEF  21/01/03 FR121052 - Re-tune indexes: Removed index ix_summary3
** KEF  16/06/03 Removed countrycode column as it's not used.
*/

/* MODIFY SUMMARY3 TABLE TO TRUNCATED TO DELETE ALL ROWS */

 @return     int OUTPUT
AS

set @return = 0

TRUNCATE TABLE summary3;
--Go

/* INSERT REQUIRED CUSTID TABLE DATA */
INSERT INTO summary3 (custid)
SELECT 	distinct(isnull(custid, ' '))
FROM	summary1
WHERE	currstatus <> 'S';
--GO

drop index summary3.ix_smrydata
CREATE UNIQUE INDEX ix_smrydata
ON summary3(
  Custid);
--GO


/* UPDATE REMAINING COLUMNS */

UPDATE	summary3
SET	custname = (isnull(rtrim(title),'')     + ' ' + 
                   isnull(rtrim(firstname),'') + ' ' + 
                   isnull(rtrim(name),''))
FROM	customer
WHERE	summary3.custid = customer.custid;
--GO

UPDATE	summary3
SET	custaddress1    = custaddress.cusaddr1,
	custaddress2    = custaddress.cusaddr2,
	custaddress3    = custaddress.cusaddr3
FROM	custaddress
WHERE	summary3.custid = custaddress.custid
AND	addtype         = 'H'
AND	(datemoved IS NULL OR datemoved = '')
AND	datein = (SELECT MAX(datein) 
		  FROM	 custaddress 
		  WHERE  summary3.custid = custaddress.custid
		  AND	 addtype = 'H'
		  AND	 (datemoved IS NULL OR datemoved = '')
		  );
--GO

UPDATE	summary3
SET	custtel1        = ('H:' + custtel.telno)
FROM	custtel
WHERE	summary3.custid = custtel.custid
AND	tellocn         = 'H'
AND	(telno IS NOT NULL AND telno != '')
AND	datediscon IS NULL
AND	dateteladd = (SELECT 	MAX(dateteladd)
		      FROM	custtel
		      WHERE 	summary3.custid = custtel.custid
		      AND	tellocn = 'H'
		      AND	datediscon IS NULL
		      );
--GO

UPDATE	summary3
SET	custtel2        = ('W:' + custtel.telno)
FROM	custtel
WHERE	summary3.custid = custtel.custid
AND	tellocn         = 'W'
AND	(telno IS NOT NULL AND telno != '')
AND	datediscon IS NULL
AND	dateteladd = (SELECT 	MAX(dateteladd)
		      FROM	custtel
		      WHERE 	summary3.custid = custtel.custid
		      AND	tellocn = 'W'
		      AND	datediscon IS NULL
		      );
--GO

UPDATE	summary3
SET	custtel3 	= ('M:' + custtel.telno)
FROM	custtel
WHERE	summary3.custid = custtel.custid
AND	tellocn  	= 'M'
AND	(telno IS NOT NULL AND telno != '')
AND	datediscon IS NULL
AND	dateteladd = (SELECT 	MAX(dateteladd)
		      FROM	custtel
		      WHERE 	tellocn = 'M'
		      AND	summary3.custid = custtel.custid
		      AND	datediscon IS NULL
		      );
--GO

/***** KEF 16/06/03 
UPDATE	summary3
SET	countrycode	= c.countrycode
FROM	country c
*****/


/* INDEXES */
/* FR121052 21/01/03 KEF Removed as not used anymore */
/****drop index summary3.ix_summary3
CREATE INDEX ix_summary3
ON summary3 (
	custid); ****/
	

SET @Return = @@ERROR
--GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
