SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary_New4SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary_New4SP
END
GO

CREATE PROCEDURE dbo.Summary_New4SP
-- =============================================
-- Author:		John Croft
--			(Based on summary4_sql.sql)
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
-- 07/03/13 jec #12563 Balances for statuscodeband='St' don't match AgeListing report (Undone)
-- 30/05/13 jec #13760 CR12949 - Summary Changes
-- 23/07/14 IP  #19510 - CR15594 - Remove AST code for Ready Assist
-- =============================================
			@return int
as 

set @return=0
set implicit_transactions off
--go
/*set nojournaling */

--TABLE TO TRUNCATED TO DELETE ALL ROWS
truncate table summary4_New_sec
--go

--CREATE TEMPORARY TABLE summary4_New_sec_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_New_sec_tmpinitial(
	branchno	smallint,
	accttype	char(1),
	accttypegroup	char(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(7),
	daysarrears	int,
	balance		money,
	number		integer,
	other_balance	money
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_New_sec_tmpinitial(
	branchno	smallint,
	accttype	char(1),
	accttypegroup	char(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(7),
	daysarrears	int,
	balance		money,
	number		integer,
	other_balance	money
	)
end
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS up to 1 MONTH
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
AND	acctlife 	<=     31    /* this is 1 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS >1 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
AND	acctlife between 32 and 183   /* this is upto 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears;
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365   /* this is upto 12 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
AND	acctlife between 365 and 730  -- 12 to 24 months
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS up to 1 MONTH
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
AND	acctlife 	<=     31    /* this is 1 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS >1 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
AND	acctlife between 32 and 183   /* this is upto 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365   /* this is upto 12 months */	
AND	outstbalcorr 	> 	0
and outstbal not between -0.00999999 and 0.009999999
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
AND	acctlife between 365 and 730  -- 12 to 24 months
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

-- sc4



--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS up to 1 MONTH
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('4')
AND	acctlife 	<=     31    /* this is 1 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears;
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS >1 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('4')
AND	acctlife between 32 and 183   /* this is upto 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears;
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('4')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365   /* this is upto 12 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('4')
AND	acctlife between 365 and 730  -- 12 to 24 months
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('4')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--sc5


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS up to 1 MONTH
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('5')
AND	acctlife 	<=     31    /* this is 1 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS >1 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('5')
AND	acctlife between 32 and 183   /* this is upto 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('5')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365   /* this is upto 12 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('5')
AND	acctlife between 366 and 730  -- 12 to 24 months
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('5')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE T WHERE THE ACCOUNT LIFE IS up to 1 MONTH
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'StC','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
AND	acctlife 	<=     31    /* this is 1 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS >1 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'StC','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
AND	acctlife between 32 and 183   /* this is upto 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'StC','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365   /* this is upto 12 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'StC','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
AND	acctlife between 366 and 730  -- 12 to 24 months
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'StC','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

-- got to here

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 8 FOR ALL ACCOUNT TYPES
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'8','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'8'
AND	outstbalcorr 	> 	0
and	deliveryflag	= 	'Y'
and accttype!='S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--CJB 21/06/00 INSERT DATA FOR settled accounts with balances
--KEF FR119992 START
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number,other_balance)
SELECT	branchno,accttype,accttypegroup,'St',SUM(outstbalcorr),COUNT(acctno),SUM(outstbal)	    /* KEF FR94058 Show balance of settled accs inc. interest and admin */
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND	outstbal <> 0
AND accttype!='S'
and outstbal not between -0.00999999 and 0.009999999		-- #12563 
GROUP BY
	branchno,accttype,accttypegroup
--go
--KEF FR119992 END



--INSERT DATA FOR ACCOUNTS WITH AN ACCOUNT TYPE 'S' AND CURRENT STATUS NOT EQUAL TO 8
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Sp',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype = 'S'
--AND	deliveryflag	= 'Y'	/* CJB 14/09/00 - removed 'AND	outstbalcorr	> 0' */
--AND	currstatus	<> 'S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 6 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'6','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '6'
and	deliveryflag	= 'Y'	/* cjb 13/09/00 removed "and	outstbalcorr	> 0" */
and outstbal not between -0.00999999 and 0.009999999
and outstbalcorr>0
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 7 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'7','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '7'
and	deliveryflag	= 'Y'
and	outstbalcorr	> 0
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears;
--go


--Calculate Int and Adm for all unsettled accounts
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'AU',SUM(outstbal - outstbalcorr),0	
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus <> 'S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go

--Calculate Int and Adm for all settled accounts with non-zero balance
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'AS',SUM(outstbal - outstbalcorr),0	
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND     outstbal <> 0
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go

--Insert as one figure for summary4_New_sec
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'AD',sum(balance),0	
FROM
	summary4_New_sec_tmpinitial
WHERE   statuscodeband IN ('AU','AS')
GROUP BY
	branchno,accttype,accttypegroup
--go
--KEF END


--CJB 07/06/00 INSERT DATA FOR value of accounts with no status code
--for non cash accounts 
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Sc',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	(currstatus 	in ('','U','0')
or	currstatus 	is null )
and	deliveryflag	= 'Y'
and	outstbalcorr 	> 	0
and outstbal not between -0.00999999 and 0.009999999
and	accttype <> 'S'	/* KEF 13/01/05 66582 Added so doesn't get counted twice */
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WHERE THE DELIVERY FLAG IS  NOT EQUAL TO 'Y',
--THE CURRENT STATUS IS 1-5 OR 9, AND THE ACCOUNT TYPE NOT EQUAL TO 'S'
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Nd',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	deliveryflag 	<> 'Y'	
AND	currstatus	<> 'S'
and accttype!='S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-5 OR 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Cr',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	outstbalcorr 	<= 	0
AND	deliveryflag 	= 	'Y'
AND	accttype	<> 	'S'	/* CJB 14/09/00 - exclude specials */
AND	currstatus	<> 	'S'
and outstbal not between -0.00999999 and 0.009999999
--AND	currstatus	<>	'6'	/* CJB 13/09/00 - exclude status 6 */
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_sec_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'9','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'9'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
AND	accttype 	<> 	'S'
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go
/* cjb 06/06/00 removed as covered by summary1 extract */

--OBTAIN UNSETTLED ACCOUNTS NOT IN SUMMARY1 TABLE I.E. THEY ARE MORE THAN 2 YEARS OLD AND STILL UNSETTLED
--THIS IS DONE BY CREATING A TEMP TABLE FIRST THEN UPDATING SUMMARY TABLE 4 LATER

	/*
	** UPDATE summary4_New_sec_tmpinitial accttypegroup COLUMN
	**
	** Despite accounts already having
	** a value in the accttypegroup column, this value
	** needs to be generalised for summary4_New_sec.
	**
	** Map the account types as follows:
	**
	** From the accttype table, map accounts with accttype
	** 'H' or 'B' (or genaccttypes corresponding to these)
	** to group 'HP'.
	** Map the others ('C' or 'S') to group 'OPT' i.e.
	** 'Option' accounts. 
	*/

UPDATE	summary4_New_sec_tmpinitial
SET	accttypegroup = 'HP'
FROM	accttype
WHERE	(summary4_New_sec_tmpinitial.accttype = accttype.accttype
OR	 summary4_New_sec_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('H','B','D','E','F','G','R')  /* Mac 3/4/01 D,E,F,G ALSO valid HP accts */ /*RF change CR453 19/06/03 */
and accttypegroup != 'CLN'
and accttypegroup != 'SGR'		-- Singer account
-- #19510 and accttypegroup != 'AST'		-- #13760 Assist account


UPDATE	summary4_New_sec_tmpinitial
SET	accttypegroup = 'OPT'
FROM	accttype
WHERE	(summary4_New_sec_tmpinitial.accttype = accttype.accttype
OR	 summary4_New_sec_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('C','S')
-- #19510 and accttypegroup != 'AST'		-- #13760 Assist account
--go


UPDATE	summary4_New_sec_tmpinitial
SET	accttypegroup = 'SC'
FROM	accttype
WHERE	(summary4_New_sec_tmpinitial.accttype = accttype.accttype
OR	 summary4_New_sec_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('T');
--go


--CREATE TEMPORARY TABLE summary4_New_sec_tmpgroup FOR GROUPING THE DATA IN summary_tmpinitial
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_New_sec_tmpgroup(
	branchno	smallint,
	accttypegroup	varchar(3),
	statuscodeband	varchar(7),
	acctlifeband	varchar(7),
	daysarrears	varchar(7),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_New_sec_tmpgroup(
	branchno	smallint,
	accttypegroup	varchar(3),
	statuscodeband	varchar(7),
	acctlifeband	varchar(7),
	daysarrears	varchar(7),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	)
end
--go

--POPULATE summary4_New_sec_tmpgroup FROM summary4_New_sec_tmpinitial GROUP DATA FOR ACCOUNTS in Advance
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'000',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears <= 0	-- advance
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--POPULATE summary4_New_sec_tmpgroup FROM summary4_New_sec_tmpinitial GROUP DATA FOR ACCOUNTS LESS THAN 1 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'001-030',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears > 0
and	daysarrears !> 30	-- up to 30 days
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 1-2 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'031-060',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears > 30
AND	daysarrears !>  60
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 2-3 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'061-090',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears > 60
AND	daysarrears !>  90
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 3-4 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'091-120',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears > 90
AND	daysarrears !>  120
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 4-5 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'121-150',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears >120
AND	daysarrears !> 150
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go

--GROUP DATA FOR ACCOUNTS WITH 5-6 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'151-180',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears >150
AND	daysarrears !> 180
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 06-12 MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'181-360',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears > 180
AND	daysarrears !> 360
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 12+ MONTHS ARREARS
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'360+',SUM(balance),SUM(number)
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears > 360
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH NULL MONTHS ARREARS AND NULL ACCOUNT LIFE BAND
INSERT INTO summary4_New_sec_tmpgroup(
	branchno,accttypegroup,statuscodeband,balance,number,other_balance			/* KEF 02/06/02 FR94058 */
	)
SELECT	branchno,accttypegroup,statuscodeband,SUM(balance),SUM(number),SUM(other_balance)		/* KEF 02/06/02 FR94058 */
FROM
	summary4_New_sec_tmpinitial
WHERE
	daysarrears IS NULL
AND	acctlifeband IS NULL
GROUP BY
	branchno,accttypegroup,statuscodeband
--go


--CREATE TEMPORARY TABLE summary4_New_sec_tmpstatic FROM STATIC TEXT FILE tmpstatic.txt
--THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_New_sec_tmpstatic(
	accttypegroup	varchar(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(7)
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_New_sec_tmpstatic(
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(3),
	acctlifeband	nvarchar(7)
	)
end
--go

--INSERT INTO summary4_New_sec BY JOINING WITH THE branch TABLE
--AND summary4_New_sec_tmpstatic.  THE RESULT IS ALL POSSIBLE COMBINATIONS.
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','8','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Sp','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','6','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','7','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Nd','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Cr','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Ad','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Sc','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','St','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','9','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','8','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sp','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','6','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','7','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Nd','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Cr','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Ad','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sc','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','St','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','9','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>00-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','8','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','6','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','7','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Nd','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Cr','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Ad','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sc','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','St','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','9','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC','StC','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC','StC','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC','StC','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC','StC','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC','StC','>24')
-- #11763 Singer Accounts
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>00-01')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>01-06')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>06-12')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>12-24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>24')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','8','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Sp','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','6','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','7','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Nd','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Cr','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Ad','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Sc','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','St','')
INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','9','')

-- #19510 - Commented out the below
-- #13760 Assist Accounts
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>00-01')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>01-06')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>06-12')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>12-24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>00-01')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>01-06')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>06-12')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>12-24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>00-01')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>01-06')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>06-12')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>12-24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>00-01')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>01-06')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>06-12')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>12-24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>24')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','8','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Sp','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','6','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','7','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Nd','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Cr','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Ad','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Sc','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','St','')
--INSERT INTO summary4_New_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','9','')


INSERT INTO summary4_New_sec(
	branchno,branchname,accttypegroup,statuscodeband,acctlifeband,asatdate)
SELECT DISTINCT
	branchno,branchname,accttypegroup,statuscodeband,acctlifeband,getdate()
FROM
	branch,
	summary4_New_sec_tmpstatic
--go


--Update summary4_New_sec with default zeroes.  This cannot be done in the above step as 
--there are some rows which require null entries to cater for display in Showcase.
UPDATE  summary4_New_sec
SET     bal_less1		= 0,
	num_less1		= 0,
	per_less1		= 0,
	per_less1_both		= 0,
	per_less1_all		= 0,
	per_less1_all_both	= 0,
	bal_0to1		= 0,		-- 0 to 1 month
	num_0to1		= 0,
	per_0to1		= 0,
	per_0to1_both		= 0,
	per_0to1_all		= 0,
	per_0to1_all_both	= 0,
	bal_1to2		= 0,
	num_1to2		= 0,
	per_1to2		= 0,
	per_1to2_both		= 0,
	per_1to2_all		= 0,
	per_1to2_all_both	= 0,
	bal_2to3		= 0,
	num_2to3		= 0,
	per_2to3		= 0,
	per_2to3_both		= 0,
	per_2to3_all		= 0,
	per_2to3_all_both	= 0,
	bal_3to4		= 0,
	num_3to4		= 0,
	per_3to4		= 0,
	per_3to4_both		= 0,
	per_3to4_all		= 0,
	per_3to4_all_both	= 0,
	bal_4to5		= 0,
	num_4to5		= 0,
	per_4to5		= 0,
	per_4to5_both		= 0,
	per_4to5_all		= 0,
	per_4to5_all_both	= 0,
	bal_5to6		= 0,
	num_5to6		= 0,
	per_5to6		= 0,
	per_5to6_both		= 0,
	per_5to6_all		= 0,
	per_5to6_all_both	= 0,
	bal_6to12		= 0,
	num_6to12		= 0,
	per_6to12		= 0,
	per_6to12_both		= 0,
	per_6to12_all		= 0,
	per_6to12_all_both	= 0,
	bal_12plus		= 0,
	num_12plus		= 0,
	per_12plus		= 0,
	per_12plus_both		= 0,
	per_12plus_all		= 0,
	per_12plus_all_both	= 0,
	other_total_bal		= 0		/* KEF 02/07/02 FR94058 */
WHERE   summary4_New_sec.acctlifeband IS NOT NULL;
--go


--UPDATE summary4_New_sec FROM summary4_New_sec_tmpgroup
--UPDATE TABLE FOR LESS in Advance
UPDATE	summary4_New_sec
SET	bal_less1 = balance,
	num_less1 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '000'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband;
--go

--UPDATE TABLE FOR LESS 1 MONTH ARREARS
UPDATE	summary4_New_sec
SET	bal_0to1 = balance,
	num_0to1 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '001-030'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband;
--go

--UPDATE TABLE FOR 1-2 MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_1to2 = balance,
	num_1to2 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '031-060'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband;
--go


--UPDATE TABLE FOR 2-3 MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_2to3 = balance,
	num_2to3 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '061-090'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband;
--go


--UPDATE TABLE FOR 3-4 MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_3to4 = balance,
	num_3to4 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '091-120'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband;
--go

--UPDATE TABLE FOR 4-5 MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_4to5 = balance,
	num_4to5 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '121-150'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband;
--go


--UPDATE TABLE FOR 5-6 MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_5to6 = balance,
	num_5to6 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '151-180'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband
--go


--UPDATE TABLE FOR 06-12 MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_6to12 = balance,
	num_6to12 = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '181-360'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband
--go


--UPDATE TABLE FOR 12+ MONTHS ARREARS
UPDATE	summary4_New_sec
SET	bal_12plus = balance,
	num_12plus = number
FROM	summary4_New_sec_tmpgroup
WHERE	summary4_New_sec_tmpgroup.daysarrears  = '360+'
AND	summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND	summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND	summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
AND	summary4_New_sec_tmpgroup.acctlifeband   = summary4_New_sec.acctlifeband
--go


--UPDATE TABLE FOR REMAINING ACCTTYPES
UPDATE  summary4_New_sec
SET     total_bal = balance,
        total_num = number,
	other_total_bal	= isnull(other_balance,0)	/* KEF 02/07/02 FR94058 */
FROM    summary4_New_sec_tmpgroup
WHERE   summary4_New_sec_tmpgroup.daysarrears IS NULL
AND     summary4_New_sec_tmpgroup.acctlifeband  IS NULL
AND     summary4_New_sec_tmpgroup.branchno       = summary4_New_sec.branchno
AND     summary4_New_sec_tmpgroup.accttypegroup  = summary4_New_sec.accttypegroup
AND     summary4_New_sec_tmpgroup.statuscodeband = summary4_New_sec.statuscodeband
--go


--TOTAL UP COLUMNS
UPDATE  summary4_New_sec
SET     total_bal = (bal_less1 + bal_0to1 + bal_1to2  + bal_2to3  + bal_3to4 +
                     bal_4to5  + bal_5to6  + bal_6to12 + bal_12plus),
        total_num = (num_less1 + num_0to1 + num_1to2  + num_2to3  + num_3to4 +
                     num_4to5  + num_5to6  + num_6to12 + num_12plus)
WHERE   acctlifeband IS NOT NULL 
and not(acctlifeband ='' and statuscodeband in ('6','7','8','9'))
--go


--PERCENTAGES

/* Accttypegroup in single branch level.  This is the percentage that the number makes of the 
** total number of accounts of the same account type, in the same branch. */
SELECT
        branchno,accttypegroup,SUM(total_bal) AS total			/* CJB 13/09/00 calc % of bal not count */
INTO
		summary4_New_sec_tmpsum
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
GROUP BY
        branchno,accttypegroup
--go

UPDATE	summary4_New_sec_tmpsum
SET	total = 1		/* To avoid division by zero */
WHERE	total = 0;
--go

UPDATE	summary4_New_sec
SET						/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1  	  = (convert (float,bal_less1)  / convert(float,total)) 	* 100,
		per_0to1   	  = (convert (float,bal_0to1)   / convert(float,total)) 	* 100,
        per_1to2   	  = (convert (float,bal_1to2)   / convert(float,total)) 	* 100,
        per_2to3   	  = (convert (float,bal_2to3)   / convert(float,total)) 	* 100,
        per_3to4   	  = (convert (float,bal_3to4)   / convert(float,total)) 	* 100,
        per_4to5   	  = (convert (float,bal_4to5)   / convert(float,total)) 	* 100,
        per_5to6   	  = (convert (float,bal_5to6)   / convert(float,total)) 	* 100,
        per_6to12  	  = (convert (float,bal_6to12)  / convert(float,total)) 	* 100,
        per_12plus 	  = (convert (float,bal_12plus) / convert(float,total)) 	* 100,
		total_per  	  = (convert (float,total_bal)  / convert(float,total)) 	* 100
FROM	summary4_New_sec_tmpsum
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND		summary4_New_sec.branchno      = summary4_New_sec_tmpsum.branchno
AND		summary4_New_sec.accttypegroup = summary4_New_sec_tmpsum.accttypegroup;
--go

UPDATE	summary4_New_sec
SET
	total_per  = abs((convert(float,total_bal)  / convert(float,total))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum
WHERE
        statuscodeband IN ('6','7','8','9')
AND	summary4_New_sec.branchno      = summary4_New_sec_tmpsum.branchno
AND	summary4_New_sec.accttypegroup = summary4_New_sec_tmpsum.accttypegroup;
--go


--Accttypegroups across all branches level: This is the percentage that the number 
--makes of the total number of accounts of the same account type, across all branches.

--First do HP accounts
SELECT
        SUM(total_bal) AS total_all_HP
INTO
summary4_New_sec_tmpsum_all_HP
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'HP'
--go

UPDATE	summary4_New_sec_tmpsum_all_HP
SET	total_all_HP = 1		/* To avoid division by zero */
WHERE	total_all_HP = 0;
--go

UPDATE	summary4_New_sec
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_HP))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_HP))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_HP))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_HP))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_HP))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_HP))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_HP))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_HP))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_HP))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_HP))	* 100
FROM	summary4_New_sec_tmpsum_all_HP
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'HP'
--go

UPDATE	summary4_New_sec
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_HP))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_all_HP
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'HP'
--go


--First do Loan accounts
SELECT
        SUM(total_bal) AS total_all_CLN
INTO
summary4_New_sec_tmpsum_all_CLN
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'CLN'
--go

UPDATE	summary4_New_sec_tmpsum_all_CLN
SET	total_all_CLN = 1		/* To avoid division by zero */
WHERE	total_all_CLN = 0;
--go

UPDATE	summary4_New_sec
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_CLN))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_CLN))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_CLN))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_CLN))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_CLN))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_CLN))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_CLN))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_CLN))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_CLN))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_CLN))	* 100
FROM	summary4_New_sec_tmpsum_all_CLN
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'CLN'
--go

UPDATE	summary4_New_sec
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_CLN))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_all_CLN
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'CLN'
--go

--Then do Singer accounts
SELECT
        SUM(total_bal) AS total_all_SGR
INTO
summary4_New_sec_tmpsum_all_SGR
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'SGR'
--go

UPDATE	summary4_New_sec_tmpsum_all_SGR
SET	total_all_SGR = 1		/* To avoid division by zero */
WHERE	total_all_SGR = 0;
--go

UPDATE	summary4_New_sec
SET					
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_SGR))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_SGR))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_SGR))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_SGR))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_SGR))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_SGR))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_SGR))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_SGR))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_SGR))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_SGR))	* 100
FROM	summary4_New_sec_tmpsum_all_SGR
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'SGR'
--go

UPDATE	summary4_New_sec
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_SGR))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_all_SGR
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'SGR'

--#19510 - Commented out the below
---- #13760 Then do Assist accounts
--SELECT
--        SUM(total_bal) AS total_all_AST
--INTO
--summary4_New_sec_tmpsum_all_AST
--FROM
--        summary4_New_sec
--WHERE
--        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
--AND	accttypegroup = 'AST'
----go

--UPDATE	summary4_New_sec_tmpsum_all_AST
--SET	total_all_AST = 1		/* To avoid division by zero */
--WHERE	total_all_AST = 0;
----go

--UPDATE	summary4_New_sec
--SET					
--			-- jec 19/02/08  change money to float for percentage calc
--		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_AST))	* 100,
--		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_AST))	* 100,
--        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_AST))	* 100,
--        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_AST))	* 100,
--        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_AST))	* 100,
--        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_AST))	* 100,
--        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_AST))	* 100,
--        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_AST))	* 100,
--        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_AST))	* 100,
--	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_AST))	* 100
--FROM	summary4_New_sec_tmpsum_all_AST
--WHERE
--        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
--AND	accttypegroup     = 'AST'
----go

--UPDATE	summary4_New_sec
--SET
--	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_AST))) * 100	-- jec 17/12/08  ABS
--FROM	summary4_New_sec_tmpsum_all_AST
--WHERE
--        statuscodeband IN ('6','7','8','9')
--AND	accttypegroup     = 'AST'

--Second do OPT accounts
SELECT
        SUM(total_bal) AS total_all_OPT
INTO
summary4_New_sec_tmpsum_all_OPT
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'OPT'
--go

UPDATE	summary4_New_sec_tmpsum_all_OPT
SET	total_all_OPT = 1		/* To avoid division by zero */
WHERE	total_all_OPT = 0
--go

UPDATE	summary4_New_sec
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_OPT))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_OPT))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_OPT))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_OPT))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_OPT))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_OPT))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_OPT))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_OPT))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_OPT))	* 100,
	total_per_all  = (convert(float,total_num)  / convert(float,total_all_OPT))	* 100
FROM	summary4_New_sec_tmpsum_all_OPT
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'OPT'
--go

UPDATE	summary4_New_sec
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_OPT))) * 100		-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_all_OPT
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'OPT'
--go



--Now Do SC Accounts
SELECT
        SUM(total_bal) AS total_all_StC
INTO
summary4_New_sec_tmpsum_all_StC
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9', 'StC')
AND	accttypegroup = 'SC'
--go

UPDATE	summary4_New_sec_tmpsum_all_StC
SET	total_all_StC = 1		/* To avoid division by zero */
WHERE	total_all_StC = 0;
--go

UPDATE	summary4_New_sec
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_StC))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_StC))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_StC))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_StC))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_StC))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_StC))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_StC))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_StC))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_StC))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_StC))	* 100
FROM	summary4_New_sec_tmpsum_all_StC
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'SC'
--go

UPDATE	summary4_New_sec
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_StC))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_all_StC
WHERE
        statuscodeband IN ('6','7','8','9', 'StC')
AND	accttypegroup     = 'SC'
--go




/* Now populate the 'both' columns.  This is the percentage that the number makes of the 
** total number of accounts of both account types, for the same branch. */
SELECT
	branchno,
        SUM(total_bal) AS total_both
INTO
	summary4_New_sec_tmpsum_both
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9', 'StC')
GROUP BY
	branchno;
--go

UPDATE	summary4_New_sec_tmpsum_both
SET	total_both = 1		/* To avoid division by zero */
WHERE	total_both = 0;
--go



UPDATE	summary4_New_sec
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_both  = (convert(float,bal_less1)  / convert(float,total_both))	* 100,
		per_0to1_both   = (convert(float,bal_0to1)   / convert(float,total_both))	* 100,
        per_1to2_both   = (convert(float,bal_1to2)   / convert(float,total_both))	* 100,
        per_2to3_both   = (convert(float,bal_2to3)   / convert(float,total_both))	* 100,
        per_3to4_both   = (convert(float,bal_3to4)   / convert(float,total_both))	* 100,
        per_4to5_both   = (convert(float,bal_4to5)   / convert(float,total_both))	* 100,
        per_5to6_both   = (convert(float,bal_5to6)   / convert(float,total_both))	* 100,
        per_6to12_both  = (convert(float,bal_6to12)  / convert(float,total_both))	* 100,
        per_12plus_both = (convert(float,bal_12plus) / convert(float,total_both))	* 100,
	total_per_both  = (convert(float,total_bal)  / convert(float,total_both))	* 100
FROM	summary4_New_sec_tmpsum_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	summary4_New_sec.branchno      = summary4_New_sec_tmpsum_both.branchno;
--go

UPDATE	summary4_New_sec
SET
	total_per_both  = abs((convert(float,total_bal)  / convert(float,total_both)))	* 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_both
WHERE
        statuscodeband IN ('6','7','8','9', 'StC')
AND	summary4_New_sec.branchno      = summary4_New_sec_tmpsum_both.branchno;
--go


--Now populate the 'all_both' columns.  This is the percentage that the
--number makes of the total number of accounts of both account types, across all branches.
SELECT
        SUM(total_bal) AS total_all_both
INTO
summary4_New_sec_tmpsum_all_both
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9', 'StC');
--go

UPDATE	summary4_New_sec_tmpsum_all_both
SET	total_all_both = 1		/* To avoid division by zero */
WHERE	total_all_both = 0
--go

UPDATE	summary4_New_sec
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all_both  = (convert(float,bal_less1)  / convert(float,total_all_both))	* 100,
		per_0to1_all_both   = (convert(float,bal_0to1)   / convert(float,total_all_both))	* 100,
        per_1to2_all_both   = (convert(float,bal_1to2)   / convert(float,total_all_both))	* 100,
        per_2to3_all_both   = (convert(float,bal_2to3)   / convert(float,total_all_both))	* 100,
        per_3to4_all_both   = (convert(float,bal_3to4)   / convert(float,total_all_both))	* 100,
        per_4to5_all_both   = (convert(float,bal_4to5)   / convert(float,total_all_both))	* 100,
        per_5to6_all_both   = (convert(float,bal_5to6)   / convert(float,total_all_both))	* 100,
        per_6to12_all_both  = (convert(float,bal_6to12)  / convert(float,total_all_both))	* 100,
        per_12plus_all_both = (convert(float,bal_12plus) / convert(float,total_all_both))	* 100,
	total_per_all_both  = (convert(float,total_bal)  / convert(float,total_all_both))	* 100
FROM	summary4_New_sec_tmpsum_all_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
--go

UPDATE	summary4_New_sec
SET
	total_per_all_both  = abs((convert(float,total_bal)  / convert(float,total_all_both))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_sec_tmpsum_all_both
WHERE
        statuscodeband IN ('6','7','8','9', 'StC')
--go



/* DROP TEMPORARY TABLES*/
DROP TABLE summary4_New_sec_tmpinitial
--go
DROP TABLE summary4_New_sec_tmpgroup
--go
DROP TABLE summary4_New_sec_tmpstatic
--go
DROP TABLE summary4_New_sec_tmpsum
--go
DROP TABLE summary4_New_sec_tmpsum_all_HP
--go
DROP TABLE summary4_New_sec_tmpsum_all_OPT
--go
DROP TABLE summary4_New_sec_tmpsum_both
--go
DROP TABLE summary4_New_sec_tmpsum_all_both
--go
DROP TABLE summary4_New_sec_tmpsum_all_Stc
DROP TABLE summary4_New_sec_tmpsum_all_CLN
DROP TABLE summary4_New_sec_tmpsum_all_SGR
--DROP TABLE summary4_New_sec_tmpsum_all_AST  --#19510

--MODIFY summary4_New_non TABLE TO TRUNCATED TO DELETE ALL ROWS
truncate table summary4_New_non
--go


--CREATE TEMPORARY TABLE summary4_New_non_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_New_non_tmpinitial(
	branchno	smallint,
	accttype	char(1),
	accttypegroup	char(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(7),
	daysarrears	int,
	balance		money,
	number		integer,
	other_balance	money
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_New_non_tmpinitial(
	branchno	smallint,
	accttype	char(1),
	accttypegroup	char(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(7),
	daysarrears	int,
	balance		money,
	number		integer,
	other_balance	money
	)
end
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS LESS THAN 2 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
AND	acctlife 	<=     31    /* this is 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS 2 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
AND	acctlife between 32 and  183    /* this is 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS 7 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
AND	acctlife between 366 and 730
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-2 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'1-2','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--- sc3

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS LESS THAN 2 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
AND	acctlife 	<=     31    /* this is 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS 2 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
AND	acctlife between 32 and  183    /* this is 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS 7 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
AND	acctlife between 366 and 730
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 3 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'3','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('3')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go



--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS LESS THAN 2 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	<= 	31
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 2 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	between	32 and 183
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	between  184 and 365
/** AND	acctlife 	<= 	 730         MAC 04/03/01 Should be 365 **/  
AND	outstbalcorr 	>	 0
AND	accttype 	<>	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	between	366 and 730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'4','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	730
AND	outstbalcorr	>	 0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS LESS THAN 2 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	<= 	31
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	=	 'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 2 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	between	32 and 183
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	=	 'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	between	184 and 365
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	between 366 and 730
AND	outstbalcorr	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'5','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go




--INSERT DATA FOR ACCOUNTS WITH STATUS CODE T WHERE THE ACCOUNT LIFE IS up to 1 MONTH
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup, 'StC','>00-01',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
AND	acctlife 	<=     31    /* this is 1 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS >1 TO 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup, 'StC','>01-06',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE	securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
AND	acctlife between 32 and 183   /* this is upto 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go

--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup, 'StC','>06-12',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife between 184 and 365   /* this is upto 12 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup, 'StC','>12-24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
AND	acctlife between 366 and 730  -- 12 to 24 months
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup, 'StC','>24',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('T')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go



--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 8 FOR ALL ACCOUNT TYPES
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'8','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'8'
AND	outstbalcorr 	> 	0
and	deliveryflag	= 	'Y'
and accttype!='S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--CJB 21/06/00 INSERT DATA FOR settled accounts with balances
--KEF FR119992 START
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number,other_balance)
SELECT	branchno,accttype,accttypegroup,'St',SUM(outstbalcorr),  /* KEF FR119992 removed abs(outstbalcorr) > 0.001 */
	COUNT(acctno),SUM(outstbal)	    /* KEF FR94058 Show balance of settled accs inc. interest and admin */
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND	outstbal <> 0
and accttype!='S'
and outstbal not between -0.00999999 and 0.009999999		-- #12563
GROUP BY
	branchno,accttype,accttypegroup
--go

--INSERT DATA FOR ACCOUNTS WITH AN ACCOUNT TYPE 'S' AND CURRENT STATUS NOT EQUAL TO 8
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Sp',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype = 'S'
--AND	deliveryflag	= 'Y'	/* CJB 14/09/00 - removed 'AND	outstbalcorr	> 0' */
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 6 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'6','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '6'
and outstbalcorr>0
and	deliveryflag	= 'Y'	/* cjb 13/09/00 removed "and	outstbalcorr	> 0" */
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 7 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'7','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '7'
and	deliveryflag	= 'Y'
and	outstbalcorr	> 0
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go


--Calculate Int and Adm for all unsettled accounts
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'AU',SUM(outstbal - outstbalcorr),0	
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus <> 'S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go

--Calculate Int and Adm for all settled accounts with non-zero balance
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'AS',SUM(outstbal - outstbalcorr),0	
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND     outstbal <> 0
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go

--Insert as one figure for summary4_New_non
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'AD',sum(balance),0	
FROM
	summary4_New_non_tmpinitial
WHERE statuscodeband IN ('AU','AS')
GROUP BY
	branchno,accttype,accttypegroup
--go
--KEF END


--CJB 07/06/00 INSERT DATA FOR value of accounts with no status code
--for non cash accounts 
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Sc',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	(currstatus 	in ('','U','0')
or	currstatus 	is null )
and	deliveryflag	= 'Y'
and	outstbalcorr 	> 	0
and	accttype <> 'S'	/* KEF 13/01/05 66582 Added so doesn't get counted twice */

and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WHERE THE DELIVERY FLAG IS  NOT EQUAL TO 'Y',
--THE CURRENT STATUS IS 1-5 OR 9, AND THE ACCOUNT TYPE NOT EQUAL TO 'S'
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Nd',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	deliveryflag 	<> 'Y'	
AND	currstatus	<> 'S'
and outstbal not between -0.00999999 and 0.009999999
and accttype!='S'
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-5 OR 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,balance,number)
SELECT	branchno,accttype,accttypegroup,'Cr',SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	outstbalcorr 	<= 	0
AND	deliveryflag 	= 	'Y'
AND	accttype	<> 	'S'	/* CJB 14/09/00 - exclude specials */
AND	currstatus	<> 	'S'
--AND	currstatus	<>	'6'	/* CJB 13/09/00 - exclude status 6 */
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_New_non_tmpinitial(
	branchno,accttype,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttype,accttypegroup,'9','',daysarrears,SUM(outstbalcorr),COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'9'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'
and outstbal not between -0.00999999 and 0.009999999
GROUP BY
	branchno,accttype,accttypegroup,daysarrears
--go
/* cjb 06/06/00 removed as covered by summary1 extract */

--OBTAIN UNSETTLED ACCOUNTS NOT IN SUMMARY1 TABLE I.E. THEY ARE MORE THAN 2 YEARS OLD AND STILL UNSETTLED
--THIS IS DONE BY CREATING A TEMP TABLE FIRST THEN UPDATING SUMMARY TABLE 4 LATER

	/*
	** UPDATE summary4_New_non_tmpinitial accttypegroup COLUMN
	**
	** Despite accounts already having
	** a value in the accttypegroup column, this value
	** needs to be generalised for summary4_New_non.
	**
	** Map the account types as follows:
	**
	** From the accttype table, map accounts with accttype
	** 'H' or 'B' (or genaccttypes corresponding to these)
	** to group 'HP'.
	** Map the others ('C' or 'S') to group 'OPT' i.e.
	** 'Option' accounts. 
	*/

UPDATE	summary4_New_non_tmpinitial
SET	accttypegroup = 'HP'
FROM	accttype
WHERE	(summary4_New_non_tmpinitial.accttype = accttype.accttype
OR	 summary4_New_non_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('H','B','D','E','F','G','R')  /* Mac 3/4/01 D,E,F,G ALSO valid HP accts */ /*RF change CR453 19/06/03 */
and accttypegroup != 'CLN'
and accttypegroup != 'SGR'		-- #11763
--and accttypegroup != 'AST'		--#19510 -- #13760

UPDATE	summary4_New_non_tmpinitial
SET	accttypegroup = 'OPT'
FROM	accttype
WHERE	(summary4_New_non_tmpinitial.accttype = accttype.accttype
OR	 summary4_New_non_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('C','S');

UPDATE	summary4_New_non_tmpinitial
SET	accttypegroup = 'SC'
FROM	accttype
WHERE	(summary4_New_non_tmpinitial.accttype = accttype.accttype
OR	 summary4_New_non_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('T');
--go


--CREATE TEMPORARY TABLE summary4_New_non_tmpgroup FOR GROUPING THE DATA IN summary_tmpinitial
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_New_non_tmpgroup(
	branchno	smallint,
	accttypegroup	varchar(3),
	statuscodeband	varchar(7),
	acctlifeband	varchar(7),
	daysarrears	varchar(7),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_New_non_tmpgroup(
	branchno	smallint,
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(7),
	acctlifeband	nvarchar(7),
	daysarrears	nvarchar(7),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	)
end
--go


--POPULATE summary4_New_non_tmpgroup FROM summary4_New_non_tmpinitial GROUP DATA FOR ACCOUNTS in Advance
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'000',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears <= 0	-- advance
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go

--POPULATE summary4_New_non_tmpgroup FROM summary4_New_non_tmpinitial GROUP DATA FOR ACCOUNTS LESS THAN 1 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'001-030',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears >0
and daysarrears !> 30	-- 1 month
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go

--GROUP DATA FOR ACCOUNTS WITH 1-2 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'031-060',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears > 30
AND	daysarrears !>  60
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 2-3 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'061-090',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears > 60
AND	daysarrears !>  90
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 3-4 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'091-120',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears > 90
AND	daysarrears !>  120
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go

--GROUP DATA FOR ACCOUNTS WITH 4-5 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'121-150',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears >= 121
AND	daysarrears !>  150
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband

--go

--GROUP DATA FOR ACCOUNTS WITH 5-6 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'151-180',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears > 150
AND	daysarrears !> 180
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 06-12 MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'181-360',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears > 180
AND	daysarrears !> 360
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH 12+ MONTHS ARREARS
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,acctlifeband,daysarrears,balance,number)
SELECT	branchno,accttypegroup,statuscodeband,acctlifeband,'360+',SUM(balance),SUM(number)
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears > 360
GROUP BY
	branchno,accttypegroup,statuscodeband,acctlifeband
--go


--GROUP DATA FOR ACCOUNTS WITH NULL MONTHS ARREARS AND NULL ACCOUNT LIFE BAND
INSERT INTO summary4_New_non_tmpgroup(
	branchno,accttypegroup,statuscodeband,balance,number,other_balance			/* KEF 02/06/02 FR94058 */
	)
SELECT	branchno,accttypegroup,statuscodeband,SUM(balance),SUM(number),SUM(other_balance)		/* KEF 02/06/02 FR94058 */
FROM
	summary4_New_non_tmpinitial
WHERE
	daysarrears IS NULL
AND	acctlifeband IS NULL
GROUP BY
	branchno,accttypegroup,statuscodeband
--go


--CREATE TEMPORARY TABLE summary4_New_non_tmpstatic FROM STATIC TEXT FILE tmpstatic.txt
--THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_New_non_tmpstatic(
	accttypegroup	varchar(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(7)
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_New_non_tmpstatic(
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(3),
	acctlifeband	nvarchar(7)
	)
end
--go


--INSERT INTO summary4_New_non BY JOINING WITH THE branch TABLE
--AND summary4_New_non_tmpstatic.  THE RESULT IS ALL POSSIBLE COMBINATIONS.
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','1-2','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','3','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','4','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','5','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','8','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Sp','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','6','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','7','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Nd','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Cr','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Ad','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','Sc','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','St','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('CLN','9','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-2','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','3','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','8','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sp','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','6','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','7','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Nd','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Cr','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Ad','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sc','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','St','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','9','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-2','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','3','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','8','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','6','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','7','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Nd','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Cr','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Ad','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sc','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','St','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','9','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC', 'StC','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC', 'StC','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC', 'StC','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC', 'StC','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC', 'StC','>24')
-- #11763 Singer Account
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','1-2','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','3','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','4','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>00-01')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>01-06')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>06-12')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>12-24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','5','>24')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','8','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Sp','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','6','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','7','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Nd','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Cr','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Ad','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','Sc','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','St','')
INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SGR','9','')

--#19510 - Commented out the below
---- #13760 Assist Account
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>00-01')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>01-06')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>06-12')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>12-24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','1-2','>24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>00-01')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>01-06')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>06-12')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>12-24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','3','>24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>00-01')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>01-06')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>06-12')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>12-24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','4','>24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>00-01')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>01-06')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>06-12')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>12-24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','5','>24')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','8','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Sp','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','6','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','7','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Nd','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Cr','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Ad','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','Sc','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','St','')
--INSERT INTO summary4_New_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('AST','9','')

INSERT INTO summary4_New_non(
	branchno,branchname,accttypegroup,statuscodeband,acctlifeband,  asatdate)
SELECT DISTINCT
	branchno,branchname,accttypegroup,statuscodeband,acctlifeband, getdate()
FROM
	branch,
	summary4_New_non_tmpstatic
--go


--Update summary4_New_non with default zeroes.  This cannot be done in the above step as 
--there are some rows which require null entries to cater for display in Showcase.
UPDATE  summary4_New_non
SET     bal_less1		= 0,
	num_less1		= 0,
	per_less1		= 0,
	per_less1_both		= 0,
	per_less1_all		= 0,
	per_less1_all_both	= 0,
	bal_0to1		= 0,
	num_0to1		= 0,
	per_0to1		= 0,
	per_0to1_both		= 0,
	per_0to1_all		= 0,
	per_0to1_all_both	= 0,
	bal_1to2		= 0,
	num_1to2		= 0,
	per_1to2		= 0,
	per_1to2_both		= 0,
	per_1to2_all		= 0,
	per_1to2_all_both	= 0,
	bal_2to3		= 0,
	num_2to3		= 0,
	per_2to3		= 0,
	per_2to3_both		= 0,
	per_2to3_all		= 0,
	per_2to3_all_both	= 0,
	bal_3to4		= 0,
	num_3to4		= 0,
	per_3to4		= 0,
	per_3to4_both		= 0,
	per_3to4_all		= 0,
	per_3to4_all_both	= 0,
	bal_4to5		= 0,
	num_4to5		= 0,
	per_4to5		= 0,
	per_4to5_both		= 0,
	per_4to5_all		= 0,
	per_4to5_all_both	= 0,
	bal_5to6		= 0,
	num_5to6		= 0,
	per_5to6		= 0,
	per_5to6_both		= 0,
	per_5to6_all		= 0,
	per_5to6_all_both	= 0,
	bal_6to12		= 0,
	num_6to12		= 0,
	per_6to12		= 0,
	per_6to12_both		= 0,
	per_6to12_all		= 0,
	per_6to12_all_both	= 0,
	bal_12plus		= 0,
	num_12plus		= 0,
	per_12plus		= 0,
	per_12plus_both		= 0,
	per_12plus_all		= 0,
	per_12plus_all_both	= 0,
	other_total_bal		= 0		/* KEF 02/07/02 FR94058 */
WHERE   summary4_New_non.acctlifeband IS NOT NULL;
--go

--UPDATE summary4_New_non FROM summary4_New_non_tmpgroup
--UPDATE TABLE FOR in Advance
UPDATE	summary4_New_non
SET	bal_less1 = balance,
	num_less1 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '000'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go

--UPDATE summary4_New_non FROM summary4_New_non_tmpgroup
--UPDATE TABLE FOR LESS THAN 1 MONTH ARREARS
UPDATE	summary4_New_non
SET	bal_0to1 = balance,
	num_0to1 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '001-030'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR 1-2 MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_1to2 = balance,
	num_1to2 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '031-060'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR 2-3 MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_2to3 = balance,
	num_2to3 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '061-090'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR 3-4 MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_3to4 = balance,
	num_3to4 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '091-120'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go

--UPDATE TABLE FOR 4-5 MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_4to5 = balance,
	num_4to5 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '121-150'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR 5-6 MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_5to6 = balance,
	num_5to6 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '151-180'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR 06-12 MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_6to12 = balance,
	num_6to12 = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '181-360'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR 12+ MONTHS ARREARS
UPDATE	summary4_New_non
SET	bal_12plus = balance,
	num_12plus = number
FROM	summary4_New_non_tmpgroup
WHERE	summary4_New_non_tmpgroup.daysarrears  = '360+'
AND	summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND	summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND	summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband
AND	summary4_New_non_tmpgroup.acctlifeband   = summary4_New_non.acctlifeband;
--go


--UPDATE TABLE FOR REMAINING ACCTTYPES
UPDATE  summary4_New_non
SET     total_bal = balance,
        total_num = number,
	other_total_bal	= isnull(other_balance,0)	/* KEF 02/07/02 FR94058 */
FROM    summary4_New_non_tmpgroup
WHERE   summary4_New_non_tmpgroup.daysarrears IS NULL
AND     summary4_New_non_tmpgroup.acctlifeband  IS NULL
AND     summary4_New_non_tmpgroup.branchno       = summary4_New_non.branchno
AND     summary4_New_non_tmpgroup.accttypegroup  = summary4_New_non.accttypegroup
AND     summary4_New_non_tmpgroup.statuscodeband = summary4_New_non.statuscodeband;
--go


--TOTAL UP COLUMNS
UPDATE  summary4_New_non
SET     total_bal = (bal_less1 + bal_0to1 + bal_1to2  + bal_2to3  + bal_3to4 +
                     bal_4to5  + bal_5to6  + bal_6to12 + bal_12plus),
        total_num = (num_less1 + num_0to1  + num_1to2  + num_2to3  + num_3to4 +
                     num_4to5  + num_5to6  + num_6to12 + num_12plus)
WHERE   acctlifeband IS NOT NULL
and not (acctlifeband ='' and statuscodeband not in ('6','7','8','9')) 
--go


--PERCENTAGES

/* Accttypegroup in single branch level.  This is the percentage that the number makes of the 
** total number of accounts of the same account type, in the same branch. */
SELECT
        branchno,
        accttypegroup,
        SUM(total_bal) AS total			/* CJB 13/09/00 calc % of bal not count */
INTO
		summary4_New_non_tmpsum
FROM
        summary4_New_non
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
GROUP BY
        branchno,
        accttypegroup
--go

UPDATE	summary4_New_non_tmpsum
SET	total = 1		/* To avoid division by zero */
WHERE	total = 0
--go

UPDATE	summary4_New_non
SET						/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1  	  = (convert (float,bal_less1)  / convert(float,total)) 	* 100,
		per_0to1   	  = (convert (float,bal_0to1)   / convert(float,total)) 	* 100,
        per_1to2   	  = (convert (float,bal_1to2)   / convert(float,total)) 	* 100,
        per_2to3   	  = (convert (float,bal_2to3)   / convert(float,total)) 	* 100,
        per_3to4   	  = (convert (float,bal_3to4)   / convert(float,total)) 	* 100,
        per_4to5   	  = (convert (float,bal_4to5)   / convert(float,total)) 	* 100,
        per_5to6   	  = (convert (float,bal_5to6)   / convert(float,total)) 	* 100,
        per_6to12  	  = (convert (float,bal_6to12)  / convert(float,total)) 	* 100,
        per_12plus 	  = (convert (float,bal_12plus) / convert(float,total)) 	* 100,
		total_per  	  = (convert (float,total_bal)  / convert(float,total)) 	* 100
FROM	summary4_New_non_tmpsum
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND		summary4_New_non.branchno      = summary4_New_non_tmpsum.branchno
AND		summary4_New_non.accttypegroup = summary4_New_non_tmpsum.accttypegroup;
--go

UPDATE	summary4_New_non
SET
	total_per  = abs((convert(float,total_bal)  / convert(float,total))) * 100		-- jec 17/12/08  ABS
FROM	summary4_New_non_tmpsum
WHERE
        statuscodeband IN ('6','7','8','9')
AND	summary4_New_non.branchno      = summary4_New_non_tmpsum.branchno
AND	summary4_New_non.accttypegroup = summary4_New_non_tmpsum.accttypegroup
--go


--Accttypegroups across all branches level: This is the percentage that the number 
--makes of the total number of accounts of the same account type, across all branches.

--First do HP accounts
SELECT
        SUM(total_bal) AS total_all_HP
INTO
summary4_New_non_tmpsum_all_HP
FROM
        summary4_New_non
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'HP'
--go

UPDATE	summary4_New_non_tmpsum_all_HP
SET	total_all_HP = 1		/* To avoid division by zero */
WHERE	total_all_HP = 0
--go

UPDATE	summary4_New_non
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_HP))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_HP))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_HP))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_HP))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_HP))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_HP))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_HP))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_HP))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_HP))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_HP))	* 100
FROM	summary4_New_non_tmpsum_all_HP
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'HP'
--go

UPDATE	summary4_New_non
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_HP))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_non_tmpsum_all_HP
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'HP'
--go


--First do Loan accounts
SELECT
        SUM(total_bal) AS total_all_CLN
INTO
summary4_New_Non_tmpsum_all_CLN
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'CLN'
--go

UPDATE	summary4_New_Non_tmpsum_all_CLN
SET	total_all_CLN = 1		/* To avoid division by zero */
WHERE	total_all_CLN = 0;
--go

UPDATE	summary4_New_sec
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_CLN))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_CLN))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_CLN))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_CLN))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_CLN))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_CLN))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_CLN))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_CLN))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_CLN))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_CLN))	* 100
FROM	summary4_New_Non_tmpsum_all_CLN
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'CLN'
--go

UPDATE	summary4_New_Non
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_CLN))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_Non_tmpsum_all_CLN
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'CLN'
--go

-- #11763 then do Singer accounts
SELECT
        SUM(total_bal) AS total_all_SGR
INTO
summary4_New_Non_tmpsum_all_SGR
FROM
        summary4_New_sec
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'SGR'
--go

UPDATE	summary4_New_Non_tmpsum_all_SGR
SET	total_all_SGR = 1		/* To avoid division by zero */
WHERE	total_all_SGR = 0;
--go

UPDATE	summary4_New_sec
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_SGR))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_SGR))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_SGR))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_SGR))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_SGR))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_SGR))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_SGR))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_SGR))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_SGR))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_SGR))	* 100
FROM	summary4_New_Non_tmpsum_all_SGR
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'SGR'
--go

UPDATE	summary4_New_Non
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_SGR))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_Non_tmpsum_all_SGR
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'SGR'

--#19510 - Commented out the below
---- #13760 then do Assist accounts
--SELECT
--        SUM(total_bal) AS total_all_AST
--INTO
--summary4_New_Non_tmpsum_all_AST
--FROM
--        summary4_New_sec
--WHERE
--        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
--AND	accttypegroup = 'AST'
--go

--UPDATE	summary4_New_Non_tmpsum_all_AST
--SET	total_all_AST = 1		/* To avoid division by zero */
--WHERE	total_all_AST = 0;
----go

--UPDATE	summary4_New_sec
--SET					/* CJB 13/09/00 calc % of bal not count */
--			-- jec 19/02/08  change money to float for percentage calc
--		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_AST))	* 100,
--		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_AST))	* 100,
--        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_AST))	* 100,
--        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_AST))	* 100,
--        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_AST))	* 100,
--        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_AST))	* 100,
--        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_AST))	* 100,
--        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_AST))	* 100,
--        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_AST))	* 100,
--	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_AST))	* 100
--FROM	summary4_New_Non_tmpsum_all_AST
--WHERE
--        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
--AND	accttypegroup     = 'AST'
----go

--UPDATE	summary4_New_Non
--SET
--	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_AST))) * 100	-- jec 17/12/08  ABS
--FROM	summary4_New_Non_tmpsum_all_AST
--WHERE
--        statuscodeband IN ('6','7','8','9')
--AND	accttypegroup     = 'AST'

--Second do OPT accounts
SELECT
        SUM(total_bal) AS total_all_OPT
INTO
summary4_New_non_tmpsum_all_OPT
FROM
        summary4_New_non
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9')
AND	accttypegroup = 'OPT'
--go

UPDATE	summary4_New_non_tmpsum_all_OPT
SET	total_all_OPT = 1		/* To avoid division by zero */
WHERE	total_all_OPT = 0
--go

UPDATE	summary4_New_non
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_OPT))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_OPT))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_OPT))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_OPT))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_OPT))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_OPT))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_OPT))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_OPT))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_OPT))	* 100,
	total_per_all  = (convert(float,total_num)  / convert(float,total_all_OPT))	* 100
FROM	summary4_New_non_tmpsum_all_OPT
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'OPT'
--go

UPDATE	summary4_New_non
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_OPT))) * 100		-- jec 17/12/08  ABS
FROM	summary4_New_non_tmpsum_all_OPT
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'OPT'
--go


--Now do StC accounts
SELECT
        SUM(total_bal) AS total_all_StC
INTO
summary4_New_non_tmpsum_all_StC
FROM
        summary4_New_non
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9', 'StC')
AND	accttypegroup = 'SC'
--go

UPDATE	summary4_New_non_tmpsum_all_StC
SET	total_all_StC = 1		/* To avoid division by zero */
WHERE	total_all_StC = 0
--go

UPDATE	summary4_New_non
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_StC))	* 100,
		per_0to1_all   = (convert(float,bal_0to1)   / convert(float,total_all_StC))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_StC))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_StC))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_StC))	* 100,
        per_4to5_all   = (convert(float,bal_4to5)   / convert(float,total_all_StC))	* 100,
        per_5to6_all   = (convert(float,bal_5to6)   / convert(float,total_all_StC))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_StC))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_StC))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_StC))	* 100
FROM	summary4_New_non_tmpsum_all_StC
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'SC'
--go

UPDATE	summary4_New_non
SET
	total_per_all  = abs((convert(float,total_bal)  / convert(float,total_all_StC))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_non_tmpsum_all_StC
WHERE
        statuscodeband IN ('6','7','8','9', 'StC')
AND	accttypegroup     = 'SC'
--go



/* Now populate the 'both' columns.  This is the percentage that the number makes of the 
** total number of accounts of both account types, for the same branch. */
SELECT
	branchno,
        SUM(total_bal) AS total_both
INTO
	summary4_New_non_tmpsum_both
FROM
        summary4_New_non
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9', 'StC')
GROUP BY
	branchno
--go

UPDATE	summary4_New_non_tmpsum_both
SET	total_both = 1		/* To avoid division by zero */
WHERE	total_both = 0
--go

UPDATE	summary4_New_non
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_both  = (convert(float,bal_less1)  / convert(float,total_both))	* 100,
		per_0to1_both   = (convert(float,bal_0to1)   / convert(float,total_both))	* 100,
        per_1to2_both   = (convert(float,bal_1to2)   / convert(float,total_both))	* 100,
        per_2to3_both   = (convert(float,bal_2to3)   / convert(float,total_both))	* 100,
        per_3to4_both   = (convert(float,bal_3to4)   / convert(float,total_both))	* 100,
        per_4to5_both   = (convert(float,bal_4to5)   / convert(float,total_both))	* 100,
        per_5to6_both   = (convert(float,bal_5to6)   / convert(float,total_both))	* 100,
        per_6to12_both  = (convert(float,bal_6to12)  / convert(float,total_both))	* 100,
        per_12plus_both = (convert(float,bal_12plus) / convert(float,total_both))	* 100,
	total_per_both  = (convert(float,total_bal)  / convert(float,total_both))	* 100
FROM	summary4_New_non_tmpsum_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	summary4_New_non.branchno      = summary4_New_non_tmpsum_both.branchno;
--go

UPDATE	summary4_New_non
SET
	total_per_both  = abs((convert(float,total_bal)  / convert(float,total_both))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_non_tmpsum_both
WHERE
        statuscodeband IN ('6','7','8','9', 'StC')
AND	summary4_New_non.branchno      = summary4_New_non_tmpsum_both.branchno;
--go


--Now populate the 'all_both' columns.  This is the percentage that the
--number makes of the total number of accounts of both account types, across all branches.
SELECT
        SUM(total_bal) AS total_all_both
INTO
summary4_New_non_tmpsum_all_both
FROM
        summary4_New_non
WHERE
        statuscodeband IN ('1-2','3','4','5','6','7','8','9', 'StC');
--go

UPDATE	summary4_New_non_tmpsum_all_both
SET	total_all_both = 1		/* To avoid division by zero */
WHERE	total_all_both = 0
--go

UPDATE	summary4_New_non
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all_both  = (convert(float,bal_less1)  / convert(float,total_all_both))	* 100,
		per_0to1_all_both   = (convert(float,bal_0to1)   / convert(float,total_all_both))	* 100,
        per_1to2_all_both   = (convert(float,bal_1to2)   / convert(float,total_all_both))	* 100,
        per_2to3_all_both   = (convert(float,bal_2to3)   / convert(float,total_all_both))	* 100,
        per_3to4_all_both   = (convert(float,bal_3to4)   / convert(float,total_all_both))	* 100,
        per_4to5_all_both   = (convert(float,bal_4to5)   / convert(float,total_all_both))	* 100,
        per_5to6_all_both   = (convert(float,bal_5to6)   / convert(float,total_all_both))	* 100,
        per_6to12_all_both  = (convert(float,bal_6to12)  / convert(float,total_all_both))	* 100,
        per_12plus_all_both = (convert(float,bal_12plus) / convert(float,total_all_both))	* 100,
	total_per_all_both  = (convert(float,total_bal)  / convert(float,total_all_both))	* 100
FROM	summary4_New_non_tmpsum_all_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %
--go

UPDATE	summary4_New_non
SET
	total_per_all_both  = abs((convert(float,total_bal)  / convert(float,total_all_both))) * 100	-- jec 17/12/08  ABS
FROM	summary4_New_non_tmpsum_all_both
WHERE
        statuscodeband IN ('6','7','8','9', 'StC')
--go



/* DROP TEMPORARY TABLES*/
DROP TABLE summary4_New_non_tmpinitial
--go
DROP TABLE summary4_New_non_tmpgroup
--go
DROP TABLE summary4_New_non_tmpstatic
--go
DROP TABLE summary4_New_non_tmpsum
--go
DROP TABLE summary4_New_non_tmpsum_all_HP
--go
DROP TABLE summary4_New_non_tmpsum_all_OPT
--go
DROP TABLE summary4_New_non_tmpsum_both
--go
DROP TABLE summary4_New_non_tmpsum_all_both
--go
DROP TABLE summary4_New_non_tmpsum_all_Stc
DROP TABLE summary4_New_non_tmpsum_all_CLN
DROP TABLE summary4_New_non_tmpsum_all_SGR
--DROP TABLE summary4_New_non_tmpsum_all_AST  --#19510 

set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
