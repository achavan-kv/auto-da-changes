SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary4SP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary4SP
END
GO

CREATE PROCEDURE dbo.Summary4SP

/*
** Author	: M. S. Davies, M. A. King (Strategic Thought)
** Date		: 16-Nov-1999
** Version	: 1.0
** Name		: Summary Table 4
**
** MSD  03/12/99 Updates following review by Courts
** MJKC 15/05/00 Removed sign from calculation of monthsarrears
** CJB 	06/06/00 Add extra join to agreement table
**               Remove processing on > 2 yr accounts as these are now on summary 1	
** CJB	07/06/00 Separate Admin Charges out in totals. Add separate total for accounts with no status code
** CJB	08/06/00 Add country code to every row - for central reporting And copy out to manrep.ing
** CJB 	13/06/00 Check for nulls in delivery flag
** CJB 	21/06/00 Sort out totals to ensure it reflect DB total
** CJB	27/06/00 don't count accounts with admin fees
** CJB	13/09/00 Amend status 6 to match as400. Change Pct to be pct of balance not pct of number of accts
** CJB	14/09/00 Don't put specials in Cr total
** AMR  24/10/00 STL- MIGRATE TO SQL SERVER
** MAC  03/04/01 Quite a few changes to correct incorrect totals 
** KEF  01/07/02 FR94058 New line in report 11 to show balance of settled accounts including the interest 
**               and admin
** KEF  21/01/03 FR121052 - Re-tune indexes: Removed index ix_summary4
**               Created new indexes ix_summary41 and ix_summary42
** KEF  10/12/02 FR119992 Changed the AD calculation to: INT & ADM on all non-settled accounts 
**               (Status <> S) plus INT & ADM on all settled accounts where the balance (incl. int/adm) 
**               is not zero (outstbal <> 0) 
**               Changed the ST figure where clause from: abs(outstbalcorr) > 0.001
**                                                    to: outstbal <> 0
** KEF  27/05/03 Change float->money for money type columns and varchar->nvarchar for all varchar columns.
** KEF  16/06/03 Removed countrycode column as not used.
** KEF  19/06/03 CR453 Change for Ready Finance accounts, include them with HP totals.
** KEF  25/06/03 Added if statement so nvarchar and nchar changes only affect Thailand
** AA   11/03/04 adding asatdate to table to help with extracts to Courts UK
** KEF  15/04/04 CR597 Reports need to be split between securitised and non-securitised accounts
** KEF  13/01/05 66582 Added and accttype <> 's' to 'SC' figure as was counting this value twice
** Jec  19/02/08 Improve percentage calculation & correct rounding to bring total % closer to 100%
*/
			@return int
as 

set @return=0
set implicit_transactions off
--go

--if not exists (select * from information_schema.columns where table_name = 'summary4_sec' and column_name ='asatdate')
--alter table summary4_sec add asatdate datetime
--go

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpinitial'
               AND TABLE_SCHEMA = 'dbo')
	BEGIN
		DROP TABLE summary4_non_tmpinitial;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpgroup'
               AND TABLE_SCHEMA = 'dbo')

	BEGIN
		DROP TABLE summary4_non_tmpgroup;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpstatic'
               AND TABLE_SCHEMA = 'dbo')
			   BEGIN
		DROP TABLE summary4_non_tmpstatic;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpsum'
               AND TABLE_SCHEMA = 'dbo')
			   BEGIN
		DROP TABLE summary4_non_tmpsum;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpsum_all_HP'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
		DROP TABLE summary4_non_tmpsum_all_HP;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpsum_all_OPT'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
		DROP TABLE summary4_non_tmpsum_all_OPT;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpsum_both'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
		DROP TABLE summary4_non_tmpsum_both;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_non_tmpsum_all_both'
               AND TABLE_SCHEMA = 'dbo')


BEGIN
		DROP TABLE summary4_non_tmpsum_all_both;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpinitial'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
		DROP TABLE summary4_sec_tmpinitial;
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpgroup'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpgroup;	
	END


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpstatic'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpstatic;	
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpsum'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpsum;	
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpsum_all_HP'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpsum_all_HP;	
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpsum_all_OPT'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpsum_all_OPT;	
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpsum_both'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpsum_both;	
	END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'summary4_sec_tmpsum_all_both'
               AND TABLE_SCHEMA = 'dbo')

BEGIN
	DROP TABLE summary4_sec_tmpsum_all_both;	
	END
	
truncate table summary4_sec;
--go

--CREATE TEMPORARY TABLE summary4_sec_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_sec_tmpinitial(
	branchno	smallint,
	accttype	char(1),
	accttypegroup	char(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(5),
	monthsarrears	smallint,
	balance		money,
	number		integer,
	other_balance	money
	);
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_sec_tmpinitial(
	branchno	smallint,
	accttype	nchar(1),
	accttypegroup	nchar(3),
	statuscodeband	nvarchar(3),
	acctlifeband	nvarchar(5),
	monthsarrears	smallint,
	balance		money,
	number		integer,
	other_balance	money
	);
end
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'00-06',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE	securitised = 'Y'			/* KEF 15/04/04 CR597 */
and		outstbal not between -0.00999 and 0.009999
AND	currstatus 	IN 	('1','2','3')
AND	acctlife 	<=     183    /* this is 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999

GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'06-12',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife 	>      183  
AND     acctlife        <=     365
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'12-24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
AND	acctlife 	>  	365
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	acctlife 	<= 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'> 24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'00-06',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	<= 	183
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'06-12',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	 183
/** AND	acctlife 	<= 	 730         MAC 04/03/01 Should be 365 **/  
AND	acctlife 	<= 	 365
AND	outstbalcorr 	>	 0
AND	accttype 	<>	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'12-24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	365
AND	acctlife 	<= 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'> 24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	730
AND	outstbalcorr	>	 0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'00-06',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	<= 	183
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	=	 'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'06-12',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	>  	183
AND	acctlife 	<= 	365
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'12-24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	>  	365
AND	acctlife 	<= 	730
AND	outstbalcorr	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'> 24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 8 FOR ALL ACCOUNT TYPES
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'8',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'8'
AND	outstbalcorr 	> 	0
and	deliveryflag	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--CJB 21/06/00 INSERT DATA FOR settled accounts with balances
--KEF FR119992 START
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number,
	other_balance
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'St',
	SUM(outstbalcorr),  /* KEF FR119992 removed abs(outstbalcorr) > 0.001 */
	COUNT(acctno),
	SUM(outstbal)	    /* KEF FR94058 Show balance of settled accs inc. interest and admin */
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND	outstbal <> 0
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go
--KEF FR119992 END



--INSERT DATA FOR ACCOUNTS WITH AN ACCOUNT TYPE 'S' AND CURRENT STATUS NOT EQUAL TO 8
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Sp',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype = 'S'
--AND	deliveryflag	= 'Y'	/* CJB 14/09/00 - removed 'AND	outstbalcorr	> 0' */
--AND	currstatus	<> 'S'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 6 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'6',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '6'
and	deliveryflag	= 'Y'
and		outstbal not between -0.00999 and 0.009999	/* cjb 13/09/00 removed "and	outstbalcorr	> 0" */
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 7 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'7',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '7'
and	deliveryflag	= 'Y'
and	outstbalcorr	> 0
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go

--CJB 07/06/00 INSERT DATA FOR value of admin charges interest and feed 
--sum of difference between osbalance and osbalcorr 



--Calculate Int and Adm for all unsettled accounts
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AU',
	SUM(outstbal - outstbalcorr),
	0	
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus <> 'S'
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go

--Calculate Int and Adm for all settled accounts with non-zero balance
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AS',
	SUM(outstbal - outstbalcorr),
	0	
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND     outstbal <> 0
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go

--Insert as one figure for summary4_sec
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AD',
	sum(balance),
	0	
FROM
	summary4_sec_tmpinitial
WHERE   statuscodeband IN ('AU','AS')
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go
--KEF END


--CJB 07/06/00 INSERT DATA FOR value of accounts with no status code
--for non cash accounts 
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Sc',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	(currstatus 	in ('','U','0')
or	currstatus 	is null )
and	deliveryflag	= 'Y'
and	outstbalcorr 	> 	0
and		outstbal not between -0.00999 and 0.009999
and	accttype <> 'S'	/* KEF 13/01/05 66582 Added so doesn't get counted twice */
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WHERE THE DELIVERY FLAG IS  NOT EQUAL TO 'Y',
--THE CURRENT STATUS IS 1-5 OR 9, AND THE ACCOUNT TYPE NOT EQUAL TO 'S'
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Nd',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	deliveryflag 	<> 'Y'	
AND	currstatus	<> 'S'
and		outstbal not between -0.00999 and 0.009999
and accttype!='S'
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-5 OR 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Cr',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	outstbalcorr 	<= 	0
AND	deliveryflag 	= 	'Y'
AND	accttype	<> 	'S'	/* CJB 14/09/00 - exclude specials */
AND	currstatus	<> 	'S'
AND	currstatus	<>	'6'	/* CJB 13/09/00 - exclude status 6 */
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_sec_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
)
SELECT	branchno,
	accttype,
	accttypegroup,
	'9',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised = 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'9'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go
/* cjb 06/06/00 removed as covered by summary1 extract */

--OBTAIN UNSETTLED ACCOUNTS NOT IN SUMMARY1 TABLE I.E. THEY ARE MORE THAN 2 YEARS OLD AND STILL UNSETTLED
--THIS IS DONE BY CREATING A TEMP TABLE FIRST THEN UPDATING SUMMARY TABLE 4 LATER

	/*
	** UPDATE summary4_sec_tmpinitial accttypegroup COLUMN
	**
	** Despite accounts already having
	** a value in the accttypegroup column, this value
	** needs to be generalised for summary4_sec.
	**
	** Map the account types as follows:
	**
	** From the accttype table, map accounts with accttype
	** 'H' or 'B' (or genaccttypes corresponding to these)
	** to group 'HP'.
	** Map the others ('C' or 'S') to group 'OPT' i.e.
	** 'Option' accounts. 
	*/

UPDATE	summary4_sec_tmpinitial
SET	accttypegroup = 'HP'
FROM	accttype
WHERE	(summary4_sec_tmpinitial.accttype = accttype.accttype
OR	 summary4_sec_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('H','B','D','E','F','G','R');  /* Mac 3/4/01 D,E,F,G ALSO valid HP accts */ /*RF change CR453 19/06/03 */
--go

UPDATE	summary4_sec_tmpinitial
SET	accttypegroup = 'OPT'
FROM	accttype
WHERE	(summary4_sec_tmpinitial.accttype = accttype.accttype
OR	 summary4_sec_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('C','S');
--go


--CREATE TEMPORARY TABLE summary4_sec_tmpgroup FOR GROUPING THE DATA IN summary_tmpinitial
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_sec_tmpgroup(
	branchno	smallint,
	accttypegroup	varchar(3),
	statuscodeband	varchar(7),
	acctlifeband	varchar(5),
	monthsarrears	varchar(5),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	);
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_sec_tmpgroup(
	branchno	smallint,
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(7),
	acctlifeband	nvarchar(5),
	monthsarrears	nvarchar(5),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	);
end
--go


--POPULATE summary4_sec_tmpgroup FROM summary4_sec_tmpinitial GROUP DATA FOR ACCOUNTS LESS THAN 1 MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'< 1',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears !> 1
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 1-2 MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'1-2',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears > 1
AND	monthsarrears !>  2
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 2-3 MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'2-3',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears > 2
AND	monthsarrears !>  3
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 3-4 MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'3-4',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears > 3
AND	monthsarrears !>  4
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 4-6 MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'4-6',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears > 4
AND	monthsarrears !> 6
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 06-12 MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'06-12',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears > 6
AND	monthsarrears !> 12
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 12+ MONTHS ARREARS
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'12+',
	SUM(balance),
	SUM(number)
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears > 12
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH NULL MONTHS ARREARS AND NULL ACCOUNT LIFE BAND
INSERT INTO summary4_sec_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	balance,
	number,
	other_balance			/* KEF 02/06/02 FR94058 */
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	SUM(balance),
	SUM(number),
	SUM(other_balance)		/* KEF 02/06/02 FR94058 */
FROM
	summary4_sec_tmpinitial
WHERE
	monthsarrears IS NULL
AND	acctlifeband IS NULL
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband;
--go


--CREATE TEMPORARY TABLE summary4_sec_tmpstatic FROM STATIC TEXT FILE tmpstatic.txt
--THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_sec_tmpstatic(
	accttypegroup	varchar(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(5)
	);
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_sec_tmpstatic(
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(3),
	acctlifeband	nvarchar(5)
	);
end
--go


--INSERT INTO summary4_sec BY JOINING WITH THE branch TABLE
--AND summary4_sec_tmpstatic.  THE RESULT IS ALL POSSIBLE COMBINATIONS.
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','00-06');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','06-12');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','12-24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','> 24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','00-06');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','06-12');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','12-24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','> 24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','00-06');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','06-12');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','12-24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','> 24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','8','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sp','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','6','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','7','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Nd','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Cr','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Ad','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sc','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','St','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','9','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','00-06');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','06-12');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','12-24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','> 24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','00-06');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','06-12');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','12-24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','> 24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','00-06');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','06-12');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','12-24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','> 24');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','8','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','6','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','7','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Nd','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Cr','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Ad','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sc','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','St','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','9','');
INSERT INTO summary4_sec_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('SC','StC','');


INSERT INTO summary4_sec(
	branchno,
	branchname,
	accttypegroup,
	statuscodeband,
	acctlifeband,
   asatdate
	)
SELECT DISTINCT
	branchno,
	branchname,
	accttypegroup,
	statuscodeband,
	acctlifeband,
   getdate()
FROM
	branch,
	summary4_sec_tmpstatic;
--go


--Update summary4_sec with default zeroes.  This cannot be done in the above step as 
--there are some rows which require null entries to cater for display in Showcase.
UPDATE  summary4_sec
SET     bal_less1		= 0,
	num_less1		= 0,
	per_less1		= 0,
	per_less1_both		= 0,
	per_less1_all		= 0,
	per_less1_all_both	= 0,
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
	bal_4to6		= 0,
	num_4to6		= 0,
	per_4to6		= 0,
	per_4to6_both		= 0,
	per_4to6_all		= 0,
	per_4to6_all_both	= 0,
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
WHERE   summary4_sec.acctlifeband IS NOT NULL;
--go


--UPDATE summary4_sec FROM summary4_sec_tmpgroup
--UPDATE TABLE FOR LESS THAN 1 MONTH ARREARS
UPDATE	summary4_sec
SET	bal_less1 = balance,
	num_less1 = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '< 1'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR 1-2 MONTHS ARREARS
UPDATE	summary4_sec
SET	bal_1to2 = balance,
	num_1to2 = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '1-2'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR 2-3 MONTHS ARREARS
UPDATE	summary4_sec
SET	bal_2to3 = balance,
	num_2to3 = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '2-3'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR 3-4 MONTHS ARREARS
UPDATE	summary4_sec
SET	bal_3to4 = balance,
	num_3to4 = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '3-4'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR 4-6 MONTHS ARREARS
UPDATE	summary4_sec
SET	bal_4to6 = balance,
	num_4to6 = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '4-6'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR 06-12 MONTHS ARREARS
UPDATE	summary4_sec
SET	bal_6to12 = balance,
	num_6to12 = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '06-12'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR 12+ MONTHS ARREARS
UPDATE	summary4_sec
SET	bal_12plus = balance,
	num_12plus = number
FROM	summary4_sec_tmpgroup
WHERE	summary4_sec_tmpgroup.monthsarrears  = '12+'
AND	summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND	summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND	summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband
AND	summary4_sec_tmpgroup.acctlifeband   = summary4_sec.acctlifeband;
--go


--UPDATE TABLE FOR REMAINING ACCTTYPES
UPDATE  summary4_sec
SET     total_bal = balance,
        total_num = number,
	other_total_bal	= isnull(other_balance,0)	/* KEF 02/07/02 FR94058 */
FROM    summary4_sec_tmpgroup
WHERE   summary4_sec_tmpgroup.monthsarrears IS NULL
AND     summary4_sec_tmpgroup.acctlifeband  IS NULL
AND     summary4_sec_tmpgroup.branchno       = summary4_sec.branchno
AND     summary4_sec_tmpgroup.accttypegroup  = summary4_sec.accttypegroup
AND     summary4_sec_tmpgroup.statuscodeband = summary4_sec.statuscodeband;
--go


--TOTAL UP COLUMNS
UPDATE  summary4_sec
SET     total_bal = (bal_less1 + bal_1to2  + bal_2to3  + bal_3to4 +
                     bal_4to6  + bal_6to12 + bal_12plus),
        total_num = (num_less1 + num_1to2  + num_2to3  + num_3to4 +
                     num_4to6  + num_6to12 + num_12plus)
WHERE   acctlifeband IS NOT NULL 
AND      acctlifeband <> '';
--go


--PERCENTAGES

/* Accttypegroup in single branch level.  This is the percentage that the number makes of the 
** total number of accounts of the same account type, in the same branch. */
SELECT
        branchno,
        accttypegroup,
        SUM(total_bal) AS total			/* CJB 13/09/00 calc % of bal not count */
INTO
		summary4_sec_tmpsum
FROM
        summary4_sec
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
GROUP BY
        branchno,
        accttypegroup;
--go

UPDATE	summary4_sec_tmpsum
SET	total = 1		/* To avoid division by zero */
WHERE	total = 0;
--go

UPDATE	summary4_sec
SET						/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1  	  = (convert (float,bal_less1)  / convert(float,total)) 	* 100,
        per_1to2   	  = (convert (float,bal_1to2)   / convert(float,total)) 	* 100,
        per_2to3   	  = (convert (float,bal_2to3)   / convert(float,total)) 	* 100,
        per_3to4   	  = (convert (float,bal_3to4)   / convert(float,total)) 	* 100,
        per_4to6   	  = (convert (float,bal_4to6)   / convert(float,total)) 	* 100,
        per_6to12  	  = (convert (float,bal_6to12)  / convert(float,total)) 	* 100,
        per_12plus 	  = (convert (float,bal_12plus) / convert(float,total)) 	* 100,
		total_per  	  = (convert (float,total_bal)  / convert(float,total)) 	* 100
FROM	summary4_sec_tmpsum
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND		summary4_sec.branchno      = summary4_sec_tmpsum.branchno
AND		summary4_sec.accttypegroup = summary4_sec_tmpsum.accttypegroup;
--go

UPDATE	summary4_sec
SET
	total_per  = (convert(float,total_bal)  / convert(float,total)) * 100
FROM	summary4_sec_tmpsum
WHERE
        statuscodeband IN ('6','7','8','9')
AND	summary4_sec.branchno      = summary4_sec_tmpsum.branchno
AND	summary4_sec.accttypegroup = summary4_sec_tmpsum.accttypegroup;
--go


--Accttypegroups across all branches level: This is the percentage that the number 
--makes of the total number of accounts of the same account type, across all branches.

--First do HP accounts
SELECT
        SUM(total_bal) AS total_all_HP
INTO
summary4_sec_tmpsum_all_HP
FROM
        summary4_sec
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
AND	accttypegroup = 'HP';
--go

UPDATE	summary4_sec_tmpsum_all_HP
SET	total_all_HP = 1		/* To avoid division by zero */
WHERE	total_all_HP = 0;
--go

UPDATE	summary4_sec
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_HP))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_HP))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_HP))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_HP))	* 100,
        per_4to6_all   = (convert(float,bal_4to6)   / convert(float,total_all_HP))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_HP))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_HP))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_HP))	* 100
FROM	summary4_sec_tmpsum_all_HP
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'HP';
--go

UPDATE	summary4_sec
SET
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_HP)) * 100
FROM	summary4_sec_tmpsum_all_HP
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'HP';
--go


--Second do OPT accounts
SELECT
        SUM(total_bal) AS total_all_OPT
INTO
summary4_sec_tmpsum_all_OPT
FROM
        summary4_sec
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
AND	accttypegroup = 'OPT';
--go

UPDATE	summary4_sec_tmpsum_all_OPT
SET	total_all_OPT = 1		/* To avoid division by zero */
WHERE	total_all_OPT = 0;
--go

UPDATE	summary4_sec
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_OPT))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_OPT))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_OPT))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_OPT))	* 100,
        per_4to6_all   = (convert(float,bal_4to6)   / convert(float,total_all_OPT))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_OPT))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_OPT))	* 100,
	total_per_all  = (convert(float,total_num)  / convert(float,total_all_OPT))	* 100
FROM	summary4_sec_tmpsum_all_OPT
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'OPT';
--go

UPDATE	summary4_sec
SET
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_OPT)) * 100
FROM	summary4_sec_tmpsum_all_OPT
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'OPT';
--go


/* Now populate the 'both' columns.  This is the percentage that the number makes of the 
** total number of accounts of both account types, for the same branch. */
SELECT
	branchno,
        SUM(total_bal) AS total_both
INTO
	summary4_sec_tmpsum_both
FROM
        summary4_sec
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
GROUP BY
	branchno;
--go

UPDATE	summary4_sec_tmpsum_both
SET	total_both = 1		/* To avoid division by zero */
WHERE	total_both = 0;
--go

UPDATE	summary4_sec
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_both  = (convert(float,bal_less1)  / convert(float,total_both))	* 100,
        per_1to2_both   = (convert(float,bal_1to2)   / convert(float,total_both))	* 100,
        per_2to3_both   = (convert(float,bal_2to3)   / convert(float,total_both))	* 100,
        per_3to4_both   = (convert(float,bal_3to4)   / convert(float,total_both))	* 100,
        per_4to6_both   = (convert(float,bal_4to6)   / convert(float,total_both))	* 100,
        per_6to12_both  = (convert(float,bal_6to12)  / convert(float,total_both))	* 100,
        per_12plus_both = (convert(float,bal_12plus) / convert(float,total_both))	* 100,
	total_per_both  = (convert(float,total_bal)  / convert(float,total_both))	* 100
FROM	summary4_sec_tmpsum_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	summary4_sec.branchno      = summary4_sec_tmpsum_both.branchno;
--go

UPDATE	summary4_sec
SET
	total_per_both  = (convert(float,total_bal)  / convert(float,total_both))	* 100
FROM	summary4_sec_tmpsum_both
WHERE
        statuscodeband IN ('6','7','8','9')
AND	summary4_sec.branchno      = summary4_sec_tmpsum_both.branchno;
--go


--Now populate the 'all_both' columns.  This is the percentage that the
--number makes of the total number of accounts of both account types, across all branches.
SELECT
        SUM(total_bal) AS total_all_both
INTO
summary4_sec_tmpsum_all_both
FROM
        summary4_sec
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9');
--go

UPDATE	summary4_sec_tmpsum_all_both
SET	total_all_both = 1		/* To avoid division by zero */
WHERE	total_all_both = 0;
--go

UPDATE	summary4_sec
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all_both  = (convert(float,bal_less1)  / convert(float,total_all_both))	* 100,
        per_1to2_all_both   = (convert(float,bal_1to2)   / convert(float,total_all_both))	* 100,
        per_2to3_all_both   = (convert(float,bal_2to3)   / convert(float,total_all_both))	* 100,
        per_3to4_all_both   = (convert(float,bal_3to4)   / convert(float,total_all_both))	* 100,
        per_4to6_all_both   = (convert(float,bal_4to6)   / convert(float,total_all_both))	* 100,
        per_6to12_all_both  = (convert(float,bal_6to12)  / convert(float,total_all_both))	* 100,
        per_12plus_all_both = (convert(float,bal_12plus) / convert(float,total_all_both))	* 100,
	total_per_all_both  = (convert(float,total_bal)  / convert(float,total_all_both))	* 100
FROM	summary4_sec_tmpsum_all_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
--go

UPDATE	summary4_sec
SET
	total_per_all_both  = (convert(float,total_bal)  / convert(float,total_all_both))	* 100
FROM	summary4_sec_tmpsum_all_both
WHERE
        statuscodeband IN ('6','7','8','9');
--go


/* INDEXES */
/* FR121052 21/01/03 KEF Added new indexes */
--DECLARE @bErrors as bit
--BEGIN TRANSACTION
--SET @bErrors = 0

--if exists (select * from sysindexes where name = 'ix_summary4_sec1')
--DROP INDEX [dbo].[summary4_sec].[ix_summary4_sec1]

--CREATE NONCLUSTERED INDEX [ix_summary4_sec1] ON [dbo].[summary4_sec] ([statuscodeband], [branchno], [accttypegroup], [total_bal] )
--IF( @@error <> 0 ) SET @bErrors = 1

--if exists (select * from sysindexes where name = 'ix_summary4_sec2')
--DROP INDEX [dbo].[summary4_sec].[ix_summary4_sec2]

--CREATE NONCLUSTERED INDEX [ix_summary4_sec2] ON [dbo].[summary4_sec] ([acctlifeband] )
--IF( @@error <> 0 ) SET @bErrors = 1

--IF( @bErrors = 0 )
--  COMMIT TRANSACTION
--ELSE
--  ROLLBACK TRANSACTION


/* DROP TEMPORARY TABLES*/
DROP TABLE summary4_sec_tmpinitial;
--go
DROP TABLE summary4_sec_tmpgroup;
--go
DROP TABLE summary4_sec_tmpstatic;
--go
DROP TABLE summary4_sec_tmpsum;
--go
DROP TABLE summary4_sec_tmpsum_all_HP;
--go
DROP TABLE summary4_sec_tmpsum_all_OPT;
--go
DROP TABLE summary4_sec_tmpsum_both;
--go
DROP TABLE summary4_sec_tmpsum_all_both;
--go


/***** summary4_non *****/
--if not exists (select * from information_schema.columns where table_name = 'summary4_non' and column_name ='asatdate')
--alter table summary4_non add asatdate datetime
--go

--MODIFY summary4_non TABLE TO TRUNCATED TO DELETE ALL ROWS
truncate table summary4_non;
--go

--CREATE TEMPORARY TABLE summary4_non_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_non_tmpinitial(
	branchno	smallint,
	accttype	char(1),
	accttypegroup	char(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(5),
	monthsarrears	smallint,
	balance		money,
	number		integer,
	other_balance	money
	);
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_non_tmpinitial(
	branchno	smallint,
	accttype	nchar(1),
	accttypegroup	nchar(3),
	statuscodeband	nvarchar(3),
	acctlifeband	nvarchar(5),
	monthsarrears	smallint,
	balance		money,
	number		integer,
	other_balance	money
	);
end
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'00-06',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
AND	acctlife 	<=     183    /* this is 6 months */	
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'06-12',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
/*** AND	acctlife 	between 183 and 365   Change to below Mac 03/04/01 ***/
AND	acctlife 	>      183  
AND     acctlife        <=     365
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
and		outstbal not between -0.00999 and 0.009999
AND	deliveryflag 	= 	'Y'
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'12-24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
AND	acctlife 	>  	365
/** AND	acctlife 	<= 	720     MAC 04/03/01 Should be 730 **/ 
AND	acctlife 	<= 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-3 WHERE THE ACCOUNT LIFE > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'1-3',
	'> 24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	IN 	('1','2','3')
/** AND	acctlife 	> 	720          MAC 04/03/01 Should be 730 **/  
AND	acctlife 	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'00-06',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	<= 	183
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'06-12',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	 183
/** AND	acctlife 	<= 	 730         MAC 04/03/01 Should be 365 **/  
AND	acctlife 	<= 	 365
AND	outstbalcorr 	>	 0
AND	accttype 	<>	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'12-24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	365
AND	acctlife 	<= 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 4 WHERE THE ACCOUNT LIFE IS > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'4',
	'> 24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'4'
AND	acctlife 	>  	730
AND	outstbalcorr	>	 0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS LESS THAN 6 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'00-06',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	<= 	183
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	=	 'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 6 TO 12 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'06-12',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	>  	183
AND	acctlife 	<= 	365
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS 12 TO 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'12-24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife 	>  	365
AND	acctlife 	<= 	730
AND	outstbalcorr	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 5 WHERE THE ACCOUNT LIFE IS > 24 MONTHS
--AND ACCOUNT TYPE IS NOT 'S' FOR SPECIAL AND DELIVERY FLAG IS 'Y'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'5',
	'> 24',
	monthsarrears,
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'5'
AND	acctlife	> 	730
AND	outstbalcorr 	> 	0
AND	accttype 	<> 	'S'
AND	deliveryflag 	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup,
	monthsarrears;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 8 FOR ALL ACCOUNT TYPES
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'8',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'8'
AND	outstbalcorr 	> 	0
and	deliveryflag	= 	'Y'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--CJB 21/06/00 INSERT DATA FOR settled accounts with balances
--KEF FR119992 START
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number,
	other_balance
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'St',
	SUM(outstbalcorr),  /* KEF FR119992 removed abs(outstbalcorr) > 0.001 */
	COUNT(acctno),
	SUM(outstbal)	    /* KEF FR94058 Show balance of settled accs inc. interest and admin */
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND	outstbal <> 0
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go
--KEF FR119992 END



--INSERT DATA FOR ACCOUNTS WITH AN ACCOUNT TYPE 'S' AND CURRENT STATUS NOT EQUAL TO 8
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Sp',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype = 'S'
--AND	deliveryflag	= 'Y'	/* CJB 14/09/00 - removed 'AND	outstbalcorr	> 0' */
--AND	currstatus	<> 'S'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 6 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'6',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '6'
and	deliveryflag	= 'Y'	/* cjb 13/09/00 removed "and	outstbalcorr	> 0" */
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 7 AND ACCOUNT TYPE NOT EQUAL TO 'S'
--NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'7',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	accttype 	<> 'S'
AND	currstatus 	= '7'
and	deliveryflag	= 'Y'
and	outstbalcorr	> 0
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go

--CJB 07/06/00 INSERT DATA FOR value of admin charges interest and feed 
--sum of difference between osbalance and osbalcorr 


--KEF FR119992 START
--Remove old AD code
/*
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AO',
	SUM(outstbal - outstbalcorr),
	0
FROM
	summary1
GROUP BY
	branchno,
	accttype,
	accttypegroup;
*/

--Calculate Int and Adm for all unsettled accounts
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AU',
	SUM(outstbal - outstbalcorr),
	0	
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus <> 'S'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go

--Calculate Int and Adm for all settled accounts with non-zero balance
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AS',
	SUM(outstbal - outstbalcorr),
	0	
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus = 'S'
AND     outstbal <> 0
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go

--Insert as one figure for summary4_non
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'AD',
	sum(balance),
	0	
FROM
	summary4_non_tmpinitial
WHERE statuscodeband IN ('AU','AS')
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go
--KEF END


--CJB 07/06/00 INSERT DATA FOR value of accounts with no status code
--for non cash accounts 
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Sc',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'			/* KEF 15/04/04 CR597 */
AND	(currstatus 	in ('','U','0')
or	currstatus 	is null )
and	deliveryflag	= 'Y'
and	outstbalcorr 	> 	0
and	accttype <> 'S'	/* KEF 13/01/05 66582 Added so doesn't get counted twice */
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WHERE THE DELIVERY FLAG IS  NOT EQUAL TO 'Y',
--THE CURRENT STATUS IS 1-5 OR 9, AND THE ACCOUNT TYPE NOT EQUAL TO 'S'
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Nd',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	deliveryflag 	<> 'Y'	
AND	currstatus	<> 'S'
and		outstbal not between -0.00999 and 0.009999
and accttype!='S'
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 1-5 OR 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
	)
SELECT	branchno,
	accttype,
	accttypegroup,
	'Cr',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	outstbalcorr 	<= 	0
AND	deliveryflag 	= 	'Y'
AND	accttype	<> 	'S'	/* CJB 14/09/00 - exclude specials */
AND	currstatus	<> 	'S'
AND	currstatus	<>	'6'	/* CJB 13/09/00 - exclude status 6 */
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go


--INSERT DATA FOR ACCOUNTS WITH STATUS CODE 9 AND OUTSTANDING BALANCE IS LESS THAN OR EQUAL TO ZERO
--AND ACCOUNT TYPE IS NOT 'S' AND DELIVERY FLAG EQUALS 'Y' NOT BROKEN DOWN INTO ACCOUNT LIFE OR MONTHS IN ARREARS
INSERT INTO summary4_non_tmpinitial(
	branchno,
	accttype,
	accttypegroup,
	statuscodeband,
	balance,
	number
)
SELECT	branchno,
	accttype,
	accttypegroup,
	'9',
	SUM(outstbalcorr),
	COUNT(acctno)
FROM
	summary1
WHERE securitised <> 'Y'		/* KEF 15/04/04 CR597 */
AND	currstatus 	= 	'9'
AND	outstbalcorr 	> 	0
AND	deliveryflag 	= 	'Y'
AND	accttype 	<> 	'S'
and		outstbal not between -0.00999 and 0.009999
GROUP BY
	branchno,
	accttype,
	accttypegroup;
--go
/* cjb 06/06/00 removed as covered by summary1 extract */

--OBTAIN UNSETTLED ACCOUNTS NOT IN SUMMARY1 TABLE I.E. THEY ARE MORE THAN 2 YEARS OLD AND STILL UNSETTLED
--THIS IS DONE BY CREATING A TEMP TABLE FIRST THEN UPDATING SUMMARY TABLE 4 LATER

	/*
	** UPDATE summary4_non_tmpinitial accttypegroup COLUMN
	**
	** Despite accounts already having
	** a value in the accttypegroup column, this value
	** needs to be generalised for summary4_non.
	**
	** Map the account types as follows:
	**
	** From the accttype table, map accounts with accttype
	** 'H' or 'B' (or genaccttypes corresponding to these)
	** to group 'HP'.
	** Map the others ('C' or 'S') to group 'OPT' i.e.
	** 'Option' accounts. 
	*/

UPDATE	summary4_non_tmpinitial
SET	accttypegroup = 'HP'
FROM	accttype
WHERE	(summary4_non_tmpinitial.accttype = accttype.accttype
OR	 summary4_non_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('H','B','D','E','F','G','R');  /* Mac 3/4/01 D,E,F,G ALSO valid HP accts */ /*RF change CR453 19/06/03 */
--go

UPDATE	summary4_non_tmpinitial
SET	accttypegroup = 'OPT'
FROM	accttype
WHERE	(summary4_non_tmpinitial.accttype = accttype.accttype
OR	 summary4_non_tmpinitial.accttype = accttype.genaccttype)
AND	 accttype.accttype in ('C','S');
--go


--CREATE TEMPORARY TABLE summary4_non_tmpgroup FOR GROUPING THE DATA IN summary_tmpinitial
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_non_tmpgroup(
	branchno	smallint,
	accttypegroup	varchar(3),
	statuscodeband	varchar(7),
	acctlifeband	varchar(5),
	monthsarrears	varchar(5),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	);
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_non_tmpgroup(
	branchno	smallint,
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(7),
	acctlifeband	nvarchar(5),
	monthsarrears	nvarchar(5),
	balance		money,
	number		integer,
	other_balance	money		/* KEF 02/06/02 FR94058 */
	);
end
--go


--POPULATE summary4_non_tmpgroup FROM summary4_non_tmpinitial GROUP DATA FOR ACCOUNTS LESS THAN 1 MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'< 1',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears !> 1
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 1-2 MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'1-2',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears > 1
AND	monthsarrears !>  2
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 2-3 MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'2-3',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears > 2
AND	monthsarrears !>  3
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 3-4 MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'3-4',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears > 3
AND	monthsarrears !>  4
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 4-6 MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'4-6',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears > 4
AND	monthsarrears !> 6
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 06-12 MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'06-12',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears > 6
AND	monthsarrears !> 12
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH 12+ MONTHS ARREARS
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	monthsarrears,
	balance,
	number
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband,
	'12+',
	SUM(balance),
	SUM(number)
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears > 12
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband,
	acctlifeband;
--go


--GROUP DATA FOR ACCOUNTS WITH NULL MONTHS ARREARS AND NULL ACCOUNT LIFE BAND
INSERT INTO summary4_non_tmpgroup(
	branchno,
	accttypegroup,
	statuscodeband,
	balance,
	number,
	other_balance			/* KEF 02/06/02 FR94058 */
	)
SELECT	branchno,
	accttypegroup,
	statuscodeband,
	SUM(balance),
	SUM(number),
	SUM(other_balance)		/* KEF 02/06/02 FR94058 */
FROM
	summary4_non_tmpinitial
WHERE
	monthsarrears IS NULL
AND	acctlifeband IS NULL
GROUP BY
	branchno,
	accttypegroup,
	statuscodeband;
--go


--CREATE TEMPORARY TABLE summary4_non_tmpstatic FROM STATIC TEXT FILE tmpstatic.txt
--THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4_non_tmpstatic(
	accttypegroup	varchar(3),
	statuscodeband	varchar(3),
	acctlifeband	varchar(5)
	);
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4_non_tmpstatic(
	accttypegroup	nvarchar(3),
	statuscodeband	nvarchar(3),
	acctlifeband	nvarchar(5)
	);
end
--go

-- bulk insert summary4_non_tmpstatic
-- from 'd:\cosprog\eod\summary\tmpstatic.txt' with 
-- (datafiletype = 'char', fieldterminator=',')
-- go


--INSERT INTO summary4_non BY JOINING WITH THE branch TABLE
--AND summary4_non_tmpstatic.  THE RESULT IS ALL POSSIBLE COMBINATIONS.
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','00-06');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','06-12');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','12-24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','1-3','> 24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','00-06');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','06-12');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','12-24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','4','> 24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','00-06');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','06-12');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','12-24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','5','> 24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','8','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sp','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','6','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','7','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Nd','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Cr','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Ad','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','Sc','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','St','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('HP','9','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','00-06');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','06-12');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','12-24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','1-3','> 24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','00-06');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','06-12');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','12-24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','4','> 24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','00-06');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','06-12');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','12-24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','5','> 24');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','8','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','6','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','7','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Nd','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Cr','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Ad','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sc','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','Sp','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','St','');
INSERT INTO summary4_non_tmpstatic  (accttypegroup,statuscodeband,acctlifeband) values ('OPT','9','');


INSERT INTO summary4_non(
	branchno,
	branchname,
	accttypegroup,
	statuscodeband,
	acctlifeband,
   asatdate
	)
SELECT DISTINCT
	branchno,
	branchname,
	accttypegroup,
	statuscodeband,
	acctlifeband,
   getdate()
FROM
	branch,
	summary4_non_tmpstatic;
--go


--Update summary4_non with default zeroes.  This cannot be done in the above step as 
--there are some rows which require null entries to cater for display in Showcase.
UPDATE  summary4_non
SET     bal_less1		= 0,
	num_less1		= 0,
	per_less1		= 0,
	per_less1_both		= 0,
	per_less1_all		= 0,
	per_less1_all_both	= 0,
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
	bal_4to6		= 0,
	num_4to6		= 0,
	per_4to6		= 0,
	per_4to6_both		= 0,
	per_4to6_all		= 0,
	per_4to6_all_both	= 0,
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
WHERE   summary4_non.acctlifeband IS NOT NULL;
--go


--UPDATE summary4_non FROM summary4_non_tmpgroup
--UPDATE TABLE FOR LESS THAN 1 MONTH ARREARS
UPDATE	summary4_non
SET	bal_less1 = balance,
	num_less1 = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '< 1'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR 1-2 MONTHS ARREARS
UPDATE	summary4_non
SET	bal_1to2 = balance,
	num_1to2 = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '1-2'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR 2-3 MONTHS ARREARS
UPDATE	summary4_non
SET	bal_2to3 = balance,
	num_2to3 = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '2-3'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR 3-4 MONTHS ARREARS
UPDATE	summary4_non
SET	bal_3to4 = balance,
	num_3to4 = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '3-4'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR 4-6 MONTHS ARREARS
UPDATE	summary4_non
SET	bal_4to6 = balance,
	num_4to6 = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '4-6'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR 06-12 MONTHS ARREARS
UPDATE	summary4_non
SET	bal_6to12 = balance,
	num_6to12 = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '06-12'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR 12+ MONTHS ARREARS
UPDATE	summary4_non
SET	bal_12plus = balance,
	num_12plus = number
FROM	summary4_non_tmpgroup
WHERE	summary4_non_tmpgroup.monthsarrears  = '12+'
AND	summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND	summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND	summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband
AND	summary4_non_tmpgroup.acctlifeband   = summary4_non.acctlifeband;
--go


--UPDATE TABLE FOR REMAINING ACCTTYPES
UPDATE  summary4_non
SET     total_bal = balance,
        total_num = number,
	other_total_bal	= isnull(other_balance,0)	/* KEF 02/07/02 FR94058 */
FROM    summary4_non_tmpgroup
WHERE   summary4_non_tmpgroup.monthsarrears IS NULL
AND     summary4_non_tmpgroup.acctlifeband  IS NULL
AND     summary4_non_tmpgroup.branchno       = summary4_non.branchno
AND     summary4_non_tmpgroup.accttypegroup  = summary4_non.accttypegroup
AND     summary4_non_tmpgroup.statuscodeband = summary4_non.statuscodeband;
--go


--TOTAL UP COLUMNS
UPDATE  summary4_non
SET     total_bal = (bal_less1 + bal_1to2  + bal_2to3  + bal_3to4 +
                     bal_4to6  + bal_6to12 + bal_12plus),
        total_num = (num_less1 + num_1to2  + num_2to3  + num_3to4 +
                     num_4to6  + num_6to12 + num_12plus)
WHERE   acctlifeband IS NOT NULL 
AND      acctlifeband <> '';
--go


--PERCENTAGES

/* Accttypegroup in single branch level.  This is the percentage that the number makes of the 
** total number of accounts of the same account type, in the same branch. */
SELECT
        branchno,
        accttypegroup,
        SUM(total_bal) AS total			/* CJB 13/09/00 calc % of bal not count */
INTO
		summary4_non_tmpsum
FROM
        summary4_non
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
GROUP BY
        branchno,
        accttypegroup;
--go

UPDATE	summary4_non_tmpsum
SET	total = 1		/* To avoid division by zero */
WHERE	total = 0;
--go

UPDATE	summary4_non
SET						/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1  	  = (convert (float,bal_less1)  / convert(float,total)) 	* 100,
        per_1to2   	  = (convert (float,bal_1to2)   / convert(float,total)) 	* 100,
        per_2to3   	  = (convert (float,bal_2to3)   / convert(float,total)) 	* 100,
        per_3to4   	  = (convert (float,bal_3to4)   / convert(float,total)) 	* 100,
        per_4to6   	  = (convert (float,bal_4to6)   / convert(float,total)) 	* 100,
        per_6to12  	  = (convert (float,bal_6to12)  / convert(float,total)) 	* 100,
        per_12plus 	  = (convert (float,bal_12plus) / convert(float,total)) 	* 100,
		total_per  	  = (convert (float,total_bal)  / convert(float,total)) 	* 100
FROM	summary4_non_tmpsum
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND		summary4_non.branchno      = summary4_non_tmpsum.branchno
AND		summary4_non.accttypegroup = summary4_non_tmpsum.accttypegroup;
--go

UPDATE	summary4_non
SET
	total_per  = (convert(float,total_bal)  / convert(float,total)) * 100
FROM	summary4_non_tmpsum
WHERE
        statuscodeband IN ('6','7','8','9')
AND	summary4_non.branchno      = summary4_non_tmpsum.branchno
AND	summary4_non.accttypegroup = summary4_non_tmpsum.accttypegroup;
--go


--Accttypegroups across all branches level: This is the percentage that the number 
--makes of the total number of accounts of the same account type, across all branches.

--First do HP accounts
SELECT
        SUM(total_bal) AS total_all_HP
INTO
summary4_non_tmpsum_all_HP
FROM
        summary4_non
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
AND	accttypegroup = 'HP';
--go

UPDATE	summary4_non_tmpsum_all_HP
SET	total_all_HP = 1		/* To avoid division by zero */
WHERE	total_all_HP = 0;
--go

UPDATE	summary4_non
SET					/* CJB 13/09/00 calc % of bal not count */
			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_HP))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_HP))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_HP))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_HP))	* 100,
        per_4to6_all   = (convert(float,bal_4to6)   / convert(float,total_all_HP))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_HP))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_HP))	* 100,
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_HP))	* 100
FROM	summary4_non_tmpsum_all_HP
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'HP';
--go

UPDATE	summary4_non
SET
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_HP)) * 100
FROM	summary4_non_tmpsum_all_HP
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'HP';
--go


--Second do OPT accounts
SELECT
        SUM(total_bal) AS total_all_OPT
INTO
summary4_non_tmpsum_all_OPT
FROM
        summary4_non
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
AND	accttypegroup = 'OPT';
--go

UPDATE	summary4_non_tmpsum_all_OPT
SET	total_all_OPT = 1		/* To avoid division by zero */
WHERE	total_all_OPT = 0;
--go

UPDATE	summary4_non
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all  = (convert(float,bal_less1)  / convert(float,total_all_OPT))	* 100,
        per_1to2_all   = (convert(float,bal_1to2)   / convert(float,total_all_OPT))	* 100,
        per_2to3_all   = (convert(float,bal_2to3)   / convert(float,total_all_OPT))	* 100,
        per_3to4_all   = (convert(float,bal_3to4)   / convert(float,total_all_OPT))	* 100,
        per_4to6_all   = (convert(float,bal_4to6)   / convert(float,total_all_OPT))	* 100,
        per_6to12_all  = (convert(float,bal_6to12)  / convert(float,total_all_OPT))	* 100,
        per_12plus_all = (convert(float,bal_12plus) / convert(float,total_all_OPT))	* 100,
	total_per_all  = (convert(float,total_num)  / convert(float,total_all_OPT))	* 100
FROM	summary4_non_tmpsum_all_OPT
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	accttypegroup     = 'OPT';
--go

UPDATE	summary4_non
SET
	total_per_all  = (convert(float,total_bal)  / convert(float,total_all_OPT)) * 100
FROM	summary4_non_tmpsum_all_OPT
WHERE
        statuscodeband IN ('6','7','8','9')
AND	accttypegroup     = 'OPT';
--go


/* Now populate the 'both' columns.  This is the percentage that the number makes of the 
** total number of accounts of both account types, for the same branch. */
SELECT
	branchno,
        SUM(total_bal) AS total_both
INTO
	summary4_non_tmpsum_both
FROM
        summary4_non
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9')
GROUP BY
	branchno;
--go

UPDATE	summary4_non_tmpsum_both
SET	total_both = 1		/* To avoid division by zero */
WHERE	total_both = 0;
--go

UPDATE	summary4_non
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_both  = (convert(float,bal_less1)  / convert(float,total_both))	* 100,
        per_1to2_both   = (convert(float,bal_1to2)   / convert(float,total_both))	* 100,
        per_2to3_both   = (convert(float,bal_2to3)   / convert(float,total_both))	* 100,
        per_3to4_both   = (convert(float,bal_3to4)   / convert(float,total_both))	* 100,
        per_4to6_both   = (convert(float,bal_4to6)   / convert(float,total_both))	* 100,
        per_6to12_both  = (convert(float,bal_6to12)  / convert(float,total_both))	* 100,
        per_12plus_both = (convert(float,bal_12plus) / convert(float,total_both))	* 100,
	total_per_both  = (convert(float,total_bal)  / convert(float,total_both))	* 100
FROM	summary4_non_tmpsum_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %om t
AND	summary4_non.branchno      = summary4_non_tmpsum_both.branchno;
--go

UPDATE	summary4_non
SET
	total_per_both  = (convert(float,total_bal)  / convert(float,total_both))	* 100
FROM	summary4_non_tmpsum_both
WHERE
        statuscodeband IN ('6','7','8','9')
AND	summary4_non.branchno      = summary4_non_tmpsum_both.branchno;
--go


--Now populate the 'all_both' columns.  This is the percentage that the
--number makes of the total number of accounts of both account types, across all branches.
SELECT
        SUM(total_bal) AS total_all_both
INTO
summary4_non_tmpsum_all_both
FROM
        summary4_non
WHERE
        statuscodeband IN ('1-3','4','5','6','7','8','9');
--go

UPDATE	summary4_non_tmpsum_all_both
SET	total_all_both = 1		/* To avoid division by zero */
WHERE	total_all_both = 0;
--go

UPDATE	summary4_non
SET			-- jec 19/02/08  change money to float for percentage calc
		per_less1_all_both  = (convert(float,bal_less1)  / convert(float,total_all_both))	* 100,
        per_1to2_all_both   = (convert(float,bal_1to2)   / convert(float,total_all_both))	* 100,
        per_2to3_all_both   = (convert(float,bal_2to3)   / convert(float,total_all_both))	* 100,
        per_3to4_all_both   = (convert(float,bal_3to4)   / convert(float,total_all_both))	* 100,
        per_4to6_all_both   = (convert(float,bal_4to6)   / convert(float,total_all_both))	* 100,
        per_6to12_all_both  = (convert(float,bal_6to12)  / convert(float,total_all_both))	* 100,
        per_12plus_all_both = (convert(float,bal_12plus) / convert(float,total_all_both))	* 100,
	total_per_all_both  = (convert(float,total_bal)  / convert(float,total_all_both))	* 100
FROM	summary4_non_tmpsum_all_both
WHERE
        acctlifeband IS NOT NULL and acctlifeband!=' '	-- jec 19/02/08 excl % from total %
--go

UPDATE	summary4_non
SET
	total_per_all_both  = (convert(float,total_bal)  / convert(float,total_all_both))	* 100
FROM	summary4_non_tmpsum_all_both
WHERE
        statuscodeband IN ('6','7','8','9');
--go


/* INDEXES */
/* FR121052 21/01/03 KEF Added new indexes */
--DECLARE @bErrors as bit
--BEGIN TRANSACTION
--SET @bErrors = 0

--if exists (select * from sysindexes where name = 'ix_summary4_non1')
--DROP INDEX [dbo].[summary4_non].[ix_summary4_non1]

--CREATE NONCLUSTERED INDEX [ix_summary4_non1] ON [dbo].[summary4_non] ([statuscodeband], [branchno], [accttypegroup], [total_bal] )
--IF( @@error <> 0 ) SET @bErrors = 1

--if exists (select * from sysindexes where name = 'ix_summary4_non2')
--DROP INDEX [dbo].[summary4_non].[ix_summary4_non2]

--CREATE NONCLUSTERED INDEX [ix_summary4_non2] ON [dbo].[summary4_non] ([acctlifeband] )
--IF( @@error <> 0 ) SET @bErrors = 1

--IF( @bErrors = 0 )
--  COMMIT TRANSACTION
--ELSE
--  ROLLBACK TRANSACTION


/* DROP TEMPORARY TABLES*/
DROP TABLE summary4_non_tmpinitial;
--go
DROP TABLE summary4_non_tmpgroup;
--go
DROP TABLE summary4_non_tmpstatic;
--go
DROP TABLE summary4_non_tmpsum;
--go
DROP TABLE summary4_non_tmpsum_all_HP;
--go
DROP TABLE summary4_non_tmpsum_all_OPT;
--go
DROP TABLE summary4_non_tmpsum_both;
--go
DROP TABLE summary4_non_tmpsum_all_both;
--go

set @return=@@ERROR

GO