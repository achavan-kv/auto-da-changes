
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary9SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary9SP
END
GO

CREATE PROCEDURE dbo.Summary9SP
/*
** Author	: K. E. Fernandez
** Date		: 26-Feb-2002
** Version	: 1.0
** Name		: Summary Table 9
** Details	: Created to generate summary table specifically for IRR Report
**
** Who  Date     Change
** ---  ----     ------
** KEF  27/05/03 Change float->money for money type columns and varchar->nvarchar for all varchar columns.
** KEF  16/06/03 CR453 Added accttypegroup in 'RF' so Ready Finance type accounts are included in totals.
**               Removed countryname column as not used. 
** KEF  25/06/03 Added if statement so nvarchar changes only affect Thailand 
** KEF  15/04/04 CR597 Reports need to be split between securitised and non-securitised accounts
*/
--========================================================================================
			@return int
as 

set @return=0

/***** Summary9_sec *****/

--DROP AND CREATE TABLE summary9_sec_acct
If exists (select * from sysobjects where id = object_id(N'[summary9_sec_acct]') 
	and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [summary9_sec_acct]
--go

if (select countrycode from country) <> 'H'
begin
CREATE TABLE [summary9_sec_acct] (
	[acctno] [varchar] (12) NOT NULL DEFAULT 0,
	[instalno] [smallint] NOT NULL DEFAULT 0,
	[instalamount] [money] NOT NULL DEFAULT 0,
	[outstbal] [money] NOT NULL DEFAULT 0,
	[agrmttotal] [money] NOT NULL DEFAULT 0,
	[servicechg] [money] NOT NULL DEFAULT 0,
	[deposit] [money] NOT NULL DEFAULT 0
) ON [PRIMARY]
end
--go



/* Load all accounts which have >12 months left to pay */
DECLARE @date12mths datetime
SET @date12mths = DATEADD(mm, 12, getdate())

INSERT summary9_sec_acct
	SELECT acctno, instalno, instalamount, outstbal, agrmttotal, servicechg, deposit
	FROM summary1
	WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
	AND	instalamount > 0
        AND   (datelast > @date12mths)
        AND   currstatus != 'S'
        AND   accttypegroup in ('HP', 'IFC','RF')	/* 16/06/03 KEF */
--go

/* MODIFY SUMMARY9 TABLE TO TRUNCATED TO DELETE ALL ROWS  */
TRUNCATE TABLE summary9_sec
--go

/* Insert data into summary9_sec from summary9_sec_acct grouped by instalno */
INSERT INTO summary9_sec
	(instalno, outstbal, servicechg, deposit, agrmttotal, agrmtterm)
	SELECT instalno, sum(outstbal), sum(servicechg), sum(deposit), 
		sum(agrmttotal), 'months'
FROM summary9_sec_acct 
GROUP BY instalno
--go

/* Update irrpcent column */
UPDATE summary9_sec
SET irrpcent = ((servicechg / (agrmttotal - servicechg - deposit)) / (instalno / 12) * 100)
FROM summary9_sec
WHERE (agrmttotal - servicechg - deposit) > 0
AND (instalno / 12) > 0

/* Update asatdate column */
UPDATE summary9_sec
SET asatdate = getdate()
--go

/* drop temporary table summary9_acct */
DROP TABLE summary9_sec_acct



/***** Summary9_non *****/


--DROP AND CREATE TABLE summary9_non_acct
If exists (select * from sysobjects where id = object_id(N'[summary9_non_acct]') 
	and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	DROP TABLE [summary9_non_acct]
--go

if (select countrycode from country) <> 'H'
begin
CREATE TABLE [summary9_non_acct] (
	[acctno] [varchar] (12) NOT NULL DEFAULT 0,
	[instalno] [smallint] NOT NULL DEFAULT 0,
	[instalamount] [money] NOT NULL DEFAULT 0,
	[outstbal] [money] NOT NULL DEFAULT 0,
	[agrmttotal] [money] NOT NULL DEFAULT 0,
	[servicechg] [money] NOT NULL DEFAULT 0,
	[deposit] [money] NOT NULL DEFAULT 0
) ON [PRIMARY]
end
--GO

--if (select countrycode from country) = 'H'
--begin
--CREATE TABLE [summary9_non_acct] (
--	[acctno] [nvarchar] (12) NOT NULL DEFAULT 0,
--	[instalno] [smallint] NOT NULL DEFAULT 0,
--	[instalamount] [money] NOT NULL DEFAULT 0,
--	[outstbal] [money] NOT NULL DEFAULT 0,
--	[agrmttotal] [money] NOT NULL DEFAULT 0,
--	[servicechg] [money] NOT NULL DEFAULT 0,
--	[deposit] [money] NOT NULL DEFAULT 0
--) ON [PRIMARY]
--end
--GO

/* Load all accounts which have >12 months left to pay */
--DECLARE @date12mths datetime
--SET @date12mths = DATEADD(mm, 12, getdate())

INSERT summary9_non_acct
	SELECT acctno, instalno, instalamount, outstbal, agrmttotal, servicechg, deposit
	FROM summary1
	WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
	AND	instalamount > 0
        AND   (datelast > @date12mths)
        AND   currstatus != 'S'
        AND   accttypegroup in ('HP', 'IFC','RF')	/* 16/06/03 KEF */
--go

/* MODIFY SUMMARY9_non TABLE TO TRUNCATED TO DELETE ALL ROWS  */
TRUNCATE TABLE summary9_non
--go

/* Insert data into summary9_non from summary9_non_acct grouped by instalno */
INSERT INTO summary9_non
	(instalno, outstbal, servicechg, deposit, agrmttotal, agrmtterm)
	SELECT instalno, sum(outstbal), sum(servicechg), sum(deposit), 
		sum(agrmttotal), 'months'
FROM summary9_non_acct 
GROUP BY instalno
--go

/* Update irrpcent column */
UPDATE summary9_non
SET irrpcent = ((servicechg / (agrmttotal - servicechg - deposit)) / (instalno / 12) * 100)
FROM summary9_non
WHERE (agrmttotal - servicechg - deposit) > 0
AND (instalno / 12) > 0


/* Update asatdate column */
UPDATE summary9_non
SET asatdate = getdate()
--go

/* drop temporary table summary9_non_acct */
DROP TABLE summary9_non_acct


set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
