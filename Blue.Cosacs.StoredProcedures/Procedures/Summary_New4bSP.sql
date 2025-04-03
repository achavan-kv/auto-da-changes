SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary_New4bSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary_New4bSP
END
GO

CREATE PROCEDURE dbo.Summary_New4bSP
-- =============================================
-- Author:		John Croft
--			(Based on summary4b_sql.sql)
-- Create date: 11th December 2008
-- Description:	Script to create summary data for the New Ageing Report
--
--	The Caribbean has requested for a new ageing report to be developed which will be an addition to
--	the current Analysis of Balances report (Report 11).  
--	The Analysis of Balances report (Report 11) is part of the management suite of reports delivered 
--	within reporting services.
--	Analysis of Balances report (Report 11) displays details of the debtor’s age analysis for account 
--	value and the number of accounts that are in arrears.
--	The new ageing report needs to display value and number in exactly the same format as the current
--	Analysis of Balances report (Report 11) but with new statuses and account life groups.

-- 
-- Change Control
-----------------
-- 
-- =============================================
			@return int
as 

set @return=0

--MODIFY summary4b_New_sec TABLE TO TRUNCATED TO DELETE ALL ROWS
TRUNCATE TABLE summary4b_New_sec


--CREATE TEMPORARY TABLE summary4b_New_sec_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if exists (select * from sysobjects where id = object_id(N'[dbo].[summary4b_New_sec_tmpinitial]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[summary4b_New_sec_tmpinitial]
--go
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_sec_tmpinitial] (
	[branchno] [smallint] NULL ,
	[accttypegroup]  [varchar] (3) NULL,
	[statuscodeband] [varchar] (3) NULL ,
	[daysarrears]  [int] NULL,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go
if (select countrycode from country) = 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_sec_tmpinitial] (
	[branchno] [smallint] NULL ,
	[accttypegroup]  [nvarchar] (3) NULL,
	[statuscodeband] [nvarchar] (3) NULL ,
	[daysarrears]  [int] NULL,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go

/********************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-5	*/
/* WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS		*/
/* AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL		*/
/* AND DELIVERY FLAG IS 'Y'				*/
/********************************************************/
INSERT INTO summary4b_New_sec_tmpinitial(
	branchno,accttypegroup,statuscodeband,daysarrears,balance12,balanceafter12)
	SELECT	branchno,accttypegroup,'1-5',daysarrears,SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3','4','5')
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'	-- KEF 090903	Added to bring in line with AOB
GROUP BY
	branchno,accttypegroup,daysarrears
--go


/****************************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 8			*/
/* FOR HP ACCOUNT TYPES						*/
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS	*/	
/****************************************************************/
INSERT INTO summary4b_New_sec_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'8',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'8'
AND	outstbalcorr 	> 	0
and	deliveryflag	= 	'Y'
GROUP BY
	branchno,accttypegroup,daysarrears
--go

/*** Added daysarrears back in to give full breakdown ***/


/**********************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 6		  */
/* AND ACCOUNT TYPE NOT EQUAL TO 'S'			  */
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS */
/**********************************************************/
INSERT INTO summary4b_New_sec_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'6',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1 
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= '6'
AND	deliveryflag	= 'Y'	
AND	accttype 	<> 'S'	-- KEF 090903 Added to bring in line with AOB
/** AND	outstbalcorr 	>  0    Removed as per summary4 Mac 02/04/01  **/

GROUP BY
	branchno,accttypegroup,daysarrears  
--go

/*******************************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 7			   */
/* AND ACCOUNT TYPE NOT EQUAL TO 'S'				   */
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS	   */
/*******************************************************************/
INSERT INTO summary4b_New_sec_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'7',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= '7'
and	deliveryflag	= 'Y'
and	outstbalcorr	> 0
and	accttype 	<> 'S'
GROUP BY
	branchno,accttypegroup,daysarrears  
--go

/*** Added daysarrears back in to give full breakdown ***/

/*******************************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 9			   */
/* AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO	   */
/* AND ACCOUNT TYPE IS NOT 'S'					   */
/* AND DELIVERY FLAG EQUALS 'Y'					   */
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS	   */
/*******************************************************************/
INSERT INTO summary4b_New_sec_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'9',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'9'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'
GROUP BY
	branchno,accttypegroup,daysarrears  
--go


/********************************************************/
/* INSERT DATA FOR STORE CARD ACCOUNTS	*/
/* AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL		*/
/* AND DELIVERY FLAG IS 'Y'				*/
/********************************************************/
INSERT INTO summary4b_New_sec_tmpinitial(
	branchno,accttypegroup,statuscodeband,daysarrears,balance12,balanceafter12)
	SELECT	branchno,accttypegroup,'StC',daysarrears,SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	statuscodeband = 'StC'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'	-- KEF 090903	Added to bring in line with AOB
GROUP BY
	branchno,accttypegroup,daysarrears

/*** Added daysarrears back in to give full breakdown ***/


/*******************************************************************/
/* UPDATE summary4b_New_sec_tmpinitial accttypegroup COLUMN		   */
/*								   */
/* Despite accounts already having a value in the accttype 	   */
/* group column, this value needs to be generalised for summary4b_New_sec. */
/*								   */
/* Map the account types as follows:				   */
/*								   */
/* From the accttype table, map accounts with accttype 'H' or 'B'  */
/* (or genaccttypes corresponding to these)			   */
/* to group 'HP'.						   */
/*******************************************************************/

--CREATE TEMPORARY TABLE summary4b_New_sec_tmpgroup FOR GROUPING THE DATA IN summary4b_New_sec_tmpinitial
If exists (select * from sysobjects where id = object_id(N'[dbo].[summary4b_New_sec_tmpgroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[summary4b_New_sec_tmpgroup]
--go

if (select countrycode from country) <> 'H'
begin
CREATE TABLE [dbo].[summary4b_New_sec_tmpgroup] (
	[branchno] [smallint] NULL ,
	[statuscodeband] [varchar] (7) NULL ,
	[daysarrears] [varchar] (7) NULL ,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go

if (select countrycode from country) = 'H'
begin
CREATE TABLE [dbo].[summary4b_New_sec_tmpgroup] (
	[branchno] [smallint] NULL ,
	[statuscodeband] [varchar] (7) NULL ,
	[daysarrears] [varchar] (7) NULL ,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go


--POPULATE summary4b_New_sec_tmpgroup FROM summary4b_New_sec_tmpinitial GROUP DATA FOR ACCOUNTS in Advance
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
	SELECT	branchno,statuscodeband,'000',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears <= 0
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go

--POPULATE summary4b_New_sec_tmpgroup FROM summary4b_New_sec_tmpinitial GROUP DATA FOR ACCOUNTS LESS THAN 1 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
	SELECT	branchno,statuscodeband,'001-031',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 0
and daysarrears !> 30	-- 1 month
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 1-2 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'031-060',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 30
AND	daysarrears !>  60
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 2-3 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'061-090',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 60
AND	daysarrears !>  90
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 3-4 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'091-120',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 90
AND	daysarrears !>  120
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,
	statuscodeband
--go

--GROUP DATA FOR ACCOUNTS WITH 4-5 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'121-150',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 120
AND	daysarrears !>  150
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 5-6 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'151-180',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 150
AND	daysarrears !> 180
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 6-12 MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'181-360',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 180
AND	daysarrears !> 360
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 12+ MONTHS ARREARS
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'360+',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_sec_tmpinitial
WHERE	daysarrears > 360
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH NULL MONTHS ARREARS AND NULL ACCOUNT LIFE BAND
INSERT INTO summary4b_New_sec_tmpgroup(
	branchno,statuscodeband,balance12,balanceafter12)
SELECT	branchno,statuscodeband,SUM(balance12),SUM(balanceafter12)
FROM
	summary4b_New_sec_tmpinitial
WHERE	daysarrears IS NULL
AND	accttypegroup in ('HP','IFC','RF', 'SC')
GROUP BY
	branchno,
	statuscodeband
--go


/********************************************************/
/* CREATE TEMPORARY TABLE summary4b_New_sec_tmpstatic		*/
/* FROM STATIC TEXT FILE tmpstatic4b.txt		*/
/* THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL	*/
/* POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT	*/
/********************************************************/
if exists (select * from sysobjects where id = object_id(N'[dbo].[summary4b_New_sec_tmpstatic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[summary4b_New_sec_tmpstatic]
--go

if (select countrycode from country) <> 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_sec_tmpstatic] (
	[statuscodeband] [varchar] (3) NULL ,
	) ON [PRIMARY]
end
--go


if (select countrycode from country) = 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_sec_tmpstatic] (
	[statuscodeband] [varchar] (3) NULL ,
	) ON [PRIMARY]
end
--go

/** INSERT DATA IN summary4b_New_sec_TEMPSTATIC TABLE **/
INSERT INTO summary4b_New_sec_tmpstatic (statuscodeband) values ('1-5')
INSERT INTO summary4b_New_sec_tmpstatic (statuscodeband) values ('6')
INSERT INTO summary4b_New_sec_tmpstatic (statuscodeband) values ('7')
INSERT INTO summary4b_New_sec_tmpstatic (statuscodeband) values ('8')
INSERT INTO summary4b_New_sec_tmpstatic (statuscodeband) values ('9')
INSERT INTO summary4b_New_sec_tmpstatic (statuscodeband) values ('StC')
--go

/**********************************************************************/
/* INSERT INTO summary4b_New_sec BY JOINING WITH THE branch TABLE	      */
/* AND summary4b_New_sec_tmpstatic.  THE RESULT IS ALL POSSIBLE COMBINATIONS. */
/**********************************************************************/
INSERT INTO summary4b_New_sec(
/*        countrycode,	KEF 16/06/03 */
	branchno,
	branchname,
	statuscodeband
	)
SELECT DISTINCT
/*         0,		KEF 16/06/03 */
	branchno,
	branchname,
	statuscodeband
FROM	branch,
	summary4b_New_sec_tmpstatic
--go

/****************************************************************/
/* Update summary4b_New_sec with default zeroes.  This cannot be done 	*/
/* in the above step as there are some rows which require 	*/
/* null entries to cater for display in Showcase.		*/
/****************************************************************/
UPDATE  summary4b_New_sec
SET   	bal_less12_less1 = 0,
	bal_plus12_less1 = 0,
	bal_less12_0to1	= 0,
	bal_plus12_0to1	= 0,
	bal_less12_1to2	= 0,
	bal_plus12_1to2	= 0,
	bal_less12_2to3	= 0,
	bal_plus12_2to3	= 0,
	bal_less12_3to4	= 0,
	bal_plus12_3to4	= 0,
	bal_less12_4to5	= 0,
	bal_plus12_4to5	= 0,
	bal_less12_5to6	= 0,
	bal_plus12_5to6	= 0,
	bal_less12_6to12 = 0,
	bal_plus12_6to12 = 0,	bal_less12_12plus = 0,
	bal_plus12_12plus = 0
--go


--UPDATE summary4b_New_sec FROM summary4b_New_sec_tmpgroup
--UPDATE TABLE FOR in Advance
UPDATE	summary4b_New_sec
SET	bal_less12_less1 = balance12,
	bal_plus12_less1 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '000'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go

--UPDATE TABLE FOR LESS THAN 1 MONTH ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_0to1 = balance12,
	bal_plus12_0to1 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '001-030'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go

--UPDATE TABLE FOR 1-2 MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_1to2 = balance12,
	bal_plus12_1to2 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '031-060'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go


--UPDATE TABLE FOR 2-3 MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_2to3 = balance12,
	bal_plus12_2to3 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '061-090'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go


--UPDATE TABLE FOR 3-4 MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_3to4 = balance12,
	bal_plus12_3to4 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '091-120'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go


--UPDATE TABLE FOR 4-5 MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_4to5 = balance12,
	bal_plus12_4to5 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '121-150'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go

--UPDATE TABLE FOR 5-6 MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_5to6 = balance12,
	bal_plus12_5to6 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '151-180'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go

--UPDATE TABLE FOR 6-12 MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_6to12 = balance12,
	bal_plus12_6to12 = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '181-360'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go


--UPDATE TABLE FOR 12+ MONTHS ARREARS
UPDATE	summary4b_New_sec
SET	bal_less12_12plus = balance12,
	bal_plus12_12plus = balanceafter12
FROM	summary4b_New_sec_tmpgroup
WHERE	summary4b_New_sec_tmpgroup.daysarrears  = '360+'
AND	summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND	summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go


--UPDATE TABLE FOR REMAINING STATUS (6,7,8,9)
UPDATE  summary4b_New_sec
SET     total_bal_less12 = balance12,
        total_bal_plus12 = balanceafter12
FROM    summary4b_New_sec_tmpgroup
WHERE   summary4b_New_sec_tmpgroup.daysarrears IS NULL 
AND     summary4b_New_sec_tmpgroup.branchno       = summary4b_New_sec.branchno
AND     summary4b_New_sec_tmpgroup.statuscodeband = summary4b_New_sec.statuscodeband
--go


--GROUP FOR ALL MONTHSINARREARS TYPES FOR SC 1-5
SELECT 	  branchno,
	  statuscodeband,
	  sum(balance12) as balance12,
	  sum(balanceafter12) as balanceafter12
INTO 	#summary4b_New_sec_totals
FROM    summary4b_New_sec_tmpgroup
WHERE   summary4b_New_sec_tmpgroup.daysarrears IS NOT NULL
AND     summary4b_New_sec_tmpgroup.statuscodeband = '1-5'
GROUP BY
	branchno,	
        statuscodeband
--go


--UPDATE TABLE FOR SC 1-5
UPDATE  summary4b_New_sec
SET     total_bal_less12 = balance12,
        total_bal_plus12 = balanceafter12
FROM    #summary4b_New_sec_totals summary4b_New_sec_totals
WHERE   summary4b_New_sec_totals.branchno       = summary4b_New_sec.branchno
AND	  summary4b_New_sec_totals.statuscodeband = summary4b_New_sec.statuscodeband
AND     summary4b_New_sec_totals.statuscodeband = '1-5'
--go

--INDEXES
--if exists (select * from sysindexes
--		   where name = 'ix_summary4b_New_sec')
--DROP INDEX summary4b_New_sec.ix_summary4b_New_sec
----go
--CREATE INDEX ix_summary4b_New_sec
--ON summary4b_New_sec (branchno)
----go


--DROP TEMPORARY TABLES
DROP TABLE [dbo].[summary4b_New_sec_tmpinitial] 
--go 
DROP TABLE [dbo].[summary4b_New_sec_tmpgroup]
--go
DROP TABLE [dbo].[summary4b_New_sec_tmpstatic]
--go  


--add in As At date
UPDATE summary4b_New_sec
set Asatdate = getdate()
--go

/***** summary4b_New_non *****/

--MODIFY summary4b_New_non TABLE TO TRUNCATED TO DELETE ALL ROWS
TRUNCATE TABLE summary4b_New_non


--CREATE TEMPORARY TABLE summary4b_New_non_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if exists (select * from sysobjects where id = object_id(N'[dbo].[summary4b_New_non_tmpinitial]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[summary4b_New_non_tmpinitial]
--go
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_non_tmpinitial] (
	[branchno] [smallint] NULL ,
	[accttypegroup]  [varchar] (3) NULL,
	[statuscodeband] [varchar] (3) NULL ,
	[daysarrears]  [int] NULL,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go
if (select countrycode from country) = 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_non_tmpinitial] (
	[branchno] [smallint] NULL ,
	[accttypegroup]  [varchar] (3) NULL,
	[statuscodeband] [varchar] (3) NULL ,
	[daysarrears]  [int] NULL,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go

/********************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-5	*/
/* WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS		*/
/* AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL		*/
/* AND DELIVERY FLAG IS 'Y'				*/
/********************************************************/
INSERT INTO summary4b_New_non_tmpinitial(
	branchno,accttypegroup,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'1-5',daysarrears,SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3','4','5')
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'	-- KEF 090903	Added to bring in line with AOB
GROUP BY
	branchno,accttypegroup,daysarrears
--go


/****************************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 8			*/
/* FOR HP ACCOUNT TYPES						*/
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS	*/	
/****************************************************************/
INSERT INTO summary4b_New_non_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
	SELECT	branchno,accttypegroup,'8',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'8'
AND	outstbalcorr 	> 	0
and	deliveryflag	= 	'Y'
GROUP BY
	branchno,accttypegroup,daysarrears
--go

/*** Added daysarrears back in to give full breakdown ***/


/**********************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 6		  */
/* AND ACCOUNT TYPE NOT EQUAL TO 'S'			  */
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS */
/**********************************************************/
INSERT INTO summary4b_New_non_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'6',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1 
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= '6'
AND	deliveryflag	= 'Y'	
AND	accttype 	<> 'S'	-- KEF 090903 Added to bring in line with AOB
/** AND	outstbalcorr 	>  0    Removed as per summary4 Mac 02/04/01  **/

GROUP BY
	branchno,accttypegroup,daysarrears  
--go

/*** cjb 13/09/00 removed "and	outstbalcorr	> 0"  ***/
/*** Added daysarrears back in to give full breakdown ***/


/*******************************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 7			   */
/* AND ACCOUNT TYPE NOT EQUAL TO 'S'				   */
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS	   */
/*******************************************************************/
INSERT INTO summary4b_New_non_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'7',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= '7'
and	deliveryflag	= 'Y'
and	outstbalcorr	> 0
and	accttype 	<> 'S'
GROUP BY
	branchno,accttypegroup,daysarrears  
--go

/*** Added daysarrears back in to give full breakdown ***/

/*******************************************************************/
/* INSERT DATA FOR ACCOUNTS WITH STATUS CODE 9			   */
/* AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO	   */
/* AND ACCOUNT TYPE IS NOT 'S'					   */
/* AND DELIVERY FLAG EQUALS 'Y'					   */
/* NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS	   */
/*******************************************************************/
INSERT INTO summary4b_New_non_tmpinitial(
	branchno,accttypegroup,statuscodeband,balance12,balanceafter12)
SELECT	branchno,accttypegroup,'9',SUM(baldue12mths),SUM(baldueafter12mths)
FROM	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'9'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'
GROUP BY
	branchno,accttypegroup,daysarrears  
--go

/*** Added daysarrears back in to give full breakdown ***/


/*******************************************************************/
/* UPDATE summary4b_New_non_tmpinitial accttypegroup COLUMN		   */
/*								   */
/* Despite accounts already having a value in the accttype 	   */
/* group column, this value needs to be generalised for summary4b_New_non. */
/*								   */
/* Map the account types as follows:				   */
/*								   */
/* From the accttype table, map accounts with accttype 'H' or 'B'  */
/* (or genaccttypes corresponding to these)			   */
/* to group 'HP'.						   */
/*******************************************************************/


--CREATE TEMPORARY TABLE summary4b_New_non_tmpgroup FOR GROUPING THE DATA IN summary4b_New_non_tmpinitial
If exists (select * from sysobjects where id = object_id(N'[dbo].[summary4b_New_non_tmpgroup]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[summary4b_New_non_tmpgroup]
--go

if (select countrycode from country) <> 'H'
begin
CREATE TABLE [dbo].[summary4b_New_non_tmpgroup] (
	[branchno] [smallint] NULL ,
	[statuscodeband] [varchar] (7) NULL ,
	[daysarrears] [varchar] (7) NULL ,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go

if (select countrycode from country) = 'H'
begin
CREATE TABLE [dbo].[summary4b_New_non_tmpgroup] (
	[branchno] [smallint] NULL ,
	[statuscodeband] [varchar] (7) NULL ,
	[daysarrears] [varchar] (7) NULL ,
	[balance12] [money] NULL ,
	[balanceafter12] [money] NULL 
) ON [PRIMARY]
end
--go


--POPULATE summary4b_New_non_tmpgroup FROM summary4b_New_non_tmpinitial GROUP DATA FOR ACCOUNTS in Advance
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
	SELECT	branchno,statuscodeband,'000',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears <= 0
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go

--GROUP DATA FOR ACCOUNTS WITH 0-1 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'001-030',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 0
AND	daysarrears !>  30
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 1-2 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'031-060',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 30
AND	daysarrears !>  60
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 2-3 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'061-090',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 60
AND	daysarrears !>  90
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 3-4 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'091-120',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 90
AND	daysarrears !>  120
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 4-5 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'121-150',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 120
AND	daysarrears !> 150
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,
	statuscodeband
--go

--GROUP DATA FOR ACCOUNTS WITH 5-6 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'151-180',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 150
AND	daysarrears !> 180
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 6-12 MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'181-360',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 180
AND	daysarrears !> 360
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH 12+ MONTHS ARREARS
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,daysarrears,balance12,balanceafter12)
SELECT	branchno,statuscodeband,'360+',SUM(balance12),SUM(balanceafter12)
FROM	summary4b_New_non_tmpinitial
WHERE	daysarrears > 360
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


--GROUP DATA FOR ACCOUNTS WITH NULL MONTHS ARREARS AND NULL ACCOUNT LIFE BAND
INSERT INTO summary4b_New_non_tmpgroup(
	branchno,statuscodeband,balance12,balanceafter12)
SELECT	branchno,statuscodeband,SUM(balance12),SUM(balanceafter12)
FROM
	summary4b_New_non_tmpinitial
WHERE	daysarrears IS NULL
AND	accttypegroup in ('HP','IFC','RF')
GROUP BY
	branchno,statuscodeband
--go


/********************************************************/
/* CREATE TEMPORARY TABLE summary4b_New_non_tmpstatic		*/
/* FROM STATIC TEXT FILE tmpstatic4b.txt		*/
/* THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL	*/
/* POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT	*/
/********************************************************/
if exists (select * from sysobjects where id = object_id(N'[dbo].[summary4b_New_non_tmpstatic]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE [dbo].[summary4b_New_non_tmpstatic]
--go

if (select countrycode from country) <> 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_non_tmpstatic] (
	[statuscodeband] [varchar] (3) NULL ,
	) ON [PRIMARY]
end
--go


if (select countrycode from country) = 'H'
begin
    CREATE TABLE [dbo].[summary4b_New_non_tmpstatic] (
	[statuscodeband] [nvarchar] (3) NULL ,
	) ON [PRIMARY]
end
--go

/** INSERT DATA IN summary4b_New_non_TEMPSTATIC TABLE **/
INSERT INTO summary4b_New_non_tmpstatic (statuscodeband) values ('1-5')
INSERT INTO summary4b_New_non_tmpstatic (statuscodeband) values ('6')
INSERT INTO summary4b_New_non_tmpstatic (statuscodeband) values ('7')
INSERT INTO summary4b_New_non_tmpstatic (statuscodeband) values ('8')
INSERT INTO summary4b_New_non_tmpstatic (statuscodeband) values ('9')
--go

/**********************************************************************/
/* INSERT INTO summary4b_New_non BY JOINING WITH THE branch TABLE	      */
/* AND summary4b_New_non_tmpstatic.  THE RESULT IS ALL POSSIBLE COMBINATIONS. */
/**********************************************************************/
INSERT INTO summary4b_New_non(
/*        countrycode,	KEF 16/06/03 */
	branchno,
	branchname,
	statuscodeband
	)
SELECT DISTINCT
/*         0,		KEF 16/06/03 */
	branchno,
	branchname,
	statuscodeband
FROM	branch,
	summary4b_New_non_tmpstatic
--go

/****************************************************************/
/* Update summary4b_New_non with default zeroes.  This cannot be done 	*/
/* in the above step as there are some rows which require 	*/
/* null entries to cater for display in Showcase.		*/
/****************************************************************/
UPDATE  summary4b_New_non
SET   	bal_less12_less1 = 0,
	bal_plus12_less1 = 0,
	bal_less12_0to1	= 0,
	bal_plus12_0to1	= 0,
	bal_less12_1to2	= 0,
	bal_plus12_1to2	= 0,
	bal_less12_2to3	= 0,
	bal_plus12_2to3	= 0,
	bal_less12_3to4	= 0,
	bal_plus12_3to4	= 0,
	bal_less12_4to5	= 0,
	bal_plus12_4to5	= 0,
	bal_less12_5to6	= 0,
	bal_plus12_5to6	= 0,
	bal_less12_6to12 = 0,
	bal_plus12_6to12 = 0,	bal_less12_12plus = 0,
	bal_plus12_12plus = 0
--go


--UPDATE summary4b_New_non FROM summary4b_New_non_tmpgroup
--UPDATE TABLE FOR in Advance
UPDATE	summary4b_New_non
SET	bal_less12_less1 = balance12,
	bal_plus12_less1 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '000'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go

--UPDATE TABLE FOR 0-1 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_0to1 = balance12,
	bal_plus12_0to1 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '001-030'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go

--UPDATE TABLE FOR 1-2 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_1to2 = balance12,
	bal_plus12_1to2 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '031-060'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--UPDATE TABLE FOR 2-3 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_2to3 = balance12,
	bal_plus12_2to3 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '061-090'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--UPDATE TABLE FOR 3-4 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_3to4 = balance12,
	bal_plus12_3to4 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '091-120'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--UPDATE TABLE FOR 4-5 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_4to5 = balance12,
	bal_plus12_4to5 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '121-150'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go

--UPDATE TABLE FOR 5-6 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_5to6 = balance12,
	bal_plus12_5to6 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '151-180'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--UPDATE TABLE FOR 6-12 MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_6to12 = balance12,
	bal_plus12_6to12 = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '181-360'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--UPDATE TABLE FOR 12+ MONTHS ARREARS
UPDATE	summary4b_New_non
SET	bal_less12_12plus = balance12,
	bal_plus12_12plus = balanceafter12
FROM	summary4b_New_non_tmpgroup
WHERE	summary4b_New_non_tmpgroup.daysarrears  = '360+'
AND	summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND	summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--UPDATE TABLE FOR REMAINING STATUS (6,7,8,9)
UPDATE  summary4b_New_non
SET     total_bal_less12 = balance12,
        total_bal_plus12 = balanceafter12
FROM    summary4b_New_non_tmpgroup
WHERE   summary4b_New_non_tmpgroup.daysarrears IS NULL 
AND     summary4b_New_non_tmpgroup.branchno       = summary4b_New_non.branchno
AND     summary4b_New_non_tmpgroup.statuscodeband = summary4b_New_non.statuscodeband
--go


--GROUP FOR ALL MONTHSINARREARS TYPES FOR SC 1-5
SELECT 	  branchno,
	  statuscodeband,
	  sum(balance12) as balance12,
	  sum(balanceafter12) as balanceafter12
INTO 	#summary4b_New_non_totals
FROM    summary4b_New_non_tmpgroup
WHERE   summary4b_New_non_tmpgroup.daysarrears IS NOT NULL
AND     summary4b_New_non_tmpgroup.statuscodeband = '1-5'
GROUP BY
	branchno,statuscodeband
--go


--UPDATE TABLE FOR SC 1-5
UPDATE  summary4b_New_non
SET     total_bal_less12 = balance12,
        total_bal_plus12 = balanceafter12
FROM    #summary4b_New_non_totals summary4b_New_non_totals
WHERE   summary4b_New_non_totals.branchno       = summary4b_New_non.branchno
AND	  summary4b_New_non_totals.statuscodeband = summary4b_New_non.statuscodeband
AND     summary4b_New_non_totals.statuscodeband = '1-5'
--go


--INDEXES
--if exists (select * from sysindexes
--		   where name = 'ix_summary4b_New_non')
--DROP INDEX summary4b_New_non.ix_summary4b_New_non
----go
--CREATE INDEX ix_summary4b_New_non
--ON summary4b_New_non (branchno)
--go


--DROP TEMPORARY TABLES
DROP TABLE [dbo].[summary4b_New_non_tmpinitial] 
--go 
DROP TABLE [dbo].[summary4b_New_non_tmpgroup]
--go
DROP TABLE [dbo].[summary4b_New_non_tmpstatic]
--go  


--add in As At date
UPDATE summary4b_New_non
set Asatdate = getdate()
--go

set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
