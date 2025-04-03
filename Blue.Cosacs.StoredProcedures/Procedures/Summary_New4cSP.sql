SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].Summary_New4cSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE Summary_New4cSP
END
GO

CREATE PROCEDURE dbo.Summary_New4cSP
-- =============================================
-- Author:		John Croft
--			(Based on summary4c_sql.sql)
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

set implicit_transactions off
--go

/***** summary4c_New_sec *****/

--MODIFY summary4c_New_sec TABLE TO TRUNCATED TO DELETE ALL ROWS
truncate table summary4c_New_sec;
--go


--CREATE TEMPORARY TABLE summary4c_New_sec_tmpinitial DATA WILL BE INITIALLY INSERTED INTO THIS TABLE
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4c_New_sec_tmpinitial(
	branchno	 smallint,
	currstatus	 varchar(1),
	accttypegroup	 char(3),
	acctlife	 integer,
	monthsarrearsnew varchar(14),
	balance		 money,
	number		 integer
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4c_New_sec_tmpinitial(
	branchno	smallint,
	currstatus	nvarchar(1),
	accttypegroup	nchar(3),
	acctlife	integer,
	monthsarrearsnew nvarchar(14),
	balance		money,
	number		integer
	)
end
--go


--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS IN ADVANCE
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'In Adv',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew <= 0
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 1 MONTH
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'1 Mth',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 1
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go


--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 2 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'2 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 2
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go


--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 3 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'3 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 3
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 4 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'4 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 4
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 5 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'5 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 5
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 6 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'6 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 6
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 7 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'7 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 7
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 8 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'8 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 8
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 9 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'9 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 9
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 10 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'10 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 10
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 11 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'11 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 11
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS 12 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'12 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew = 12
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT DATA FOR ALL ACCOUNTS WHERE MONTHS ARREARS IS >12 MONTHS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'>12 Mths',SUM(outstbalcorrnew),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	monthsarrearsnew > 12
AND	(currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
	OR statuscodeband in ('StC'))
GROUP BY
	branchno,currstatus,acctlife
--go

--INSERT AD DATA FOR ALL ACCOUNTS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'AD',SUM(outstbal - outstbalcorrnew - bdw),	--want INT, ADM, DDF and FEE in this value
	COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
GROUP BY
	branchno,currstatus,acctlife
--go


--INSERT AD DATA FOR ALL ACCOUNTS
INSERT INTO summary4c_New_sec_tmpinitial(
	branchno,currstatus,acctlife,monthsarrearsnew,balance,number)
SELECT	branchno,currstatus,acctlife,'WO',SUM(bdw),COUNT(acctno)
FROM
	summary1
WHERE
	securitised = 'Y'
AND	currstatus 	IN 	('1','2','3','4','5','6','7','8','9')
GROUP BY
	branchno,currstatus,acctlife
--go


--CREATE TEMPORARY TABLE summary4c_New_sec_tmpgroup FOR GROUPING THE DATA IN summary_tmpinitial
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4c_New_sec_tmpgroup(
	branchno	smallint,
	acctlifeband	varchar(7),
	monthsarrearsnew	varchar(14),
	balance		money,
	number		integer
	)
end
--go

if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4c_New_sec_tmpgroup(
	branchno	smallint,
	acctlifeband	nvarchar(7),
	monthsarrearsnew	nvarchar(14),
	balance		money,
	number		integer
	)
end
--go


--POPULATE summary4c_New_sec_tmpgroup FROM summary4c_New_sec_tmpinitial
--!!!!!!!!!!!!!!!!!!!!!!!!!!!
--!!!!!!!!!!!!!!!!!!!!!!!!!!!  
-- Why different grouping ?????
--
--GROUP DATA FOR ACCOUNTS LESS THAN 4 months old in status code 1-5, 8 and 9
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'00-03',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('1','2','3','4','5','6','7','8','9', 'T')
AND	acctlife 	<	122	/* Acctlife less than 4 months */
GROUP BY
	branchno,monthsarrearsnew
--go

--GROUP DATA FOR ACCOUNTS LESS THAN 7 months old in status code 1-5, 8 and 9
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'04-06',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('1','2','3','4','5','6','7','8','9', 'T')
AND	acctlife 	>=	122	/* This is 4 months */
AND	acctlife 	<	213     /* Acctlife less than 7 months */
GROUP BY
	branchno,monthsarrearsnew
--go

--GROUP DATA FOR ACCOUNTS LESS THAN 13 months old in status code 1-5, 8 and 9
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'07-12',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('1','2','3','4','5','6','7','8','9', 'T')
AND	acctlife 	>=	213	/* This is 7 months */
AND	acctlife 	<	396	/* Acctlife less than 13 months */
GROUP BY
	branchno,monthsarrearsnew
--go

--GROUP DATA FOR ACCOUNTS LESS THAN 24 months old in status code 1-5, 8 and 9
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'13-24',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('1','2','3','4','5','6','7','8','9', 'T')
AND	acctlife 	>=	396	/* This is 13 months */
AND	acctlife 	<	731	/* Acctlife less than 24 months */
GROUP BY
	branchno,monthsarrearsnew
--go

--GROUP DATA FOR ACCOUNTS GREATER THAN 24 months old in status code 1-5, 8 and 9
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'>24',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('1','2','3','4','5','6','7','8','9', 'T')
AND	acctlife 	>=	731	/* Acctlife greater than 24 months */
GROUP BY
	branchno,monthsarrearsnew
--go

--GROUP DATA FOR ACCOUNTS in Sc6
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'Sc6',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('6')
GROUP BY
	branchno,monthsarrearsnew
--go

--GROUP DATA FOR ACCOUNTS in Sc7
INSERT INTO summary4c_New_sec_tmpgroup(
	branchno,acctlifeband,monthsarrearsnew,balance,number)
SELECT	branchno,'Sc7',monthsarrearsnew,SUM(balance),SUM(number)
FROM
	summary4c_New_sec_tmpinitial
WHERE
	currstatus 	IN 	('7')
GROUP BY
	branchno,monthsarrearsnew
--go



--CREATE TEMPORARY TABLE summary4c_New_sec_tmpstatic
--THIS TABLE WILL BE USED IN A JOIN TO ENSURE ALL POSSIBLE COMBINATIONS ARE DISPLAYED IN THE REPORT
if (select countrycode from country) <> 'H'
begin
    CREATE TABLE summary4c_New_sec_tmpstatic(
	sequence		smallint, 
	monthsarrearsnew	varchar(14)
	)
end
--go
if (select countrycode from country) = 'H'
begin
    CREATE TABLE summary4c_New_sec_tmpstatic(
	sequence		smallint, 
	monthsarrearsnew	nvarchar(14)
	)
end
--go

INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (1,'In Adv');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (1,'1 Mth');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (2,'2 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (3,'3 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (4,'4 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (5,'5 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (6,'6 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (7,'7 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (8,'8 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (9,'9 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (10,'10 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (11,'11 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (12,'12 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (13,'>12 Mths');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (14,'AD');
INSERT INTO summary4c_New_sec_tmpstatic (sequence, monthsarrearsnew) values (15,'WO');


--INSERT INTO summary4c_New_sec BY JOINING WITH THE branch TABLE AND summary4c_New_sec_tmpstatic.
INSERT INTO summary4c_New_sec(
	sequence,branchno,branchname,accttypegroup,monthsarrearsnew,asatdate,
	bal_0to3,num_0to3,per_0to3,per_0to3_both,per_0to3_all,per_0to3_all_both,
	bal_4to6,num_4to6,per_4to6,per_4to6_both,per_4to6_all,per_4to6_all_both,
	bal_7to12,num_7to12,per_7to12,per_7to12_both,per_7to12_all,per_7to12_all_both,
	bal_13to24,num_13to24,per_13to24,per_13to24_both,per_13to24_all,per_13to24_all_both,
	bal_24plus,num_24plus,per_24plus,per_24plus_both,per_24plus_all,per_24plus_all_both,
	bal_Sc6,num_Sc6,per_Sc6,per_Sc6_both,per_Sc6_all,per_Sc6_all_both,
	bal_Sc7,num_Sc7,per_Sc7,per_Sc7_both,per_Sc7_all,per_Sc7_all_both,
	total_bal,total_num,total_per,total_per_both,total_per_all,total_per_all_both,
	baldue12mths,baldueafter12mths,other_total_bal
	)
SELECT DISTINCT
	sequence,branchno,branchname,'HP',monthsarrearsnew,getdate(),
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0,0,0,0,
	0,0,0
	FROM
	branch,
	summary4c_New_sec_tmpstatic;
--go


--UPDATE summary4c_New_sec FROM summary4c_New_sec_tmpgroup

--UPDATE TABLE FOR ACCTLIFE 00-03
UPDATE	summary4c_New_sec
SET	bal_0to3 = balance,
	num_0to3 = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband	= '00-03'
AND	summary4c_New_sec_tmpgroup.branchno		= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--UPDATE TABLE FOR ACCTLIFE 03-06
UPDATE	summary4c_New_sec
SET	bal_4to6 = balance,
	num_4to6 = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband  	= '04-06'
AND	summary4c_New_sec_tmpgroup.branchno      	= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--UPDATE TABLE FOR ACCTLIFE 06-12
UPDATE	summary4c_New_sec
SET	bal_7to12 = balance,
	num_7to12 = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband  	= '07-12'
AND	summary4c_New_sec_tmpgroup.branchno       	= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--UPDATE TABLE FOR ACCTLIFE 13-24
UPDATE	summary4c_New_sec
SET	bal_13to24 = balance,
	num_13to24 = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband  	= '13-24'
AND	summary4c_New_sec_tmpgroup.branchno       	= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--UPDATE TABLE FOR ACCTLIFE >24
UPDATE	summary4c_New_sec
SET	bal_24plus = balance,
	num_24plus = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband  	= '>24'
AND	summary4c_New_sec_tmpgroup.branchno       	= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--UPDATE TABLE FOR Status code 6
UPDATE	summary4c_New_sec
SET	bal_Sc6 = balance,
	num_Sc6 = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband  	= 'Sc6'
AND	summary4c_New_sec_tmpgroup.branchno       	= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--UPDATE TABLE FOR Status code 7
UPDATE	summary4c_New_sec
SET	bal_Sc7 = balance,
	num_Sc7 = number
FROM	summary4c_New_sec_tmpgroup
WHERE	summary4c_New_sec_tmpgroup.acctlifeband  	= 'Sc7'
AND	summary4c_New_sec_tmpgroup.branchno       	= summary4c_New_sec.branchno
AND	summary4c_New_sec_tmpgroup.monthsarrearsnew = summary4c_New_sec.monthsarrearsnew;
--go

--TOTAL UP COLUMNS
UPDATE  summary4c_New_sec
SET     total_bal = isnull((bal_0to3 + bal_4to6 + bal_7to12 + bal_13to24 + bal_24plus  +
                     bal_Sc6  + bal_Sc7),0),
        total_num = isnull((num_0to3 + num_4to6 + num_7to12 + num_13to24 + num_24plus  +
                     num_Sc6  + num_Sc7),0)
--go



/* DROP TEMPORARY TABLES*/
DROP TABLE summary4c_New_sec_tmpinitial;
--go
DROP TABLE summary4c_New_sec_tmpgroup;
--go
DROP TABLE summary4c_New_sec_tmpstatic;
--go

set @return=@@ERROR

GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End

