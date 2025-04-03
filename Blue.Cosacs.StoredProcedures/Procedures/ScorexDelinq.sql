-- Author; John Croft  
-- Date; March 2003

-- Version: 	2.0	October 2003
-- Version:     2.1     September 2005
-- Version:     3.0     March 2006
/*
   This script will generate the Delinquency data to be sent to Scorex 
   The format of the Delinquency table matches that from the AS400
   The Delinquency table is exported using BCP to ScorexDLQ.csv

   Uses view DLQ_MthArrs 
	 	 

	View to calculate the correct "months in arrears" for Delinquency extract
*/

-- Amendments
-- Version 2.1  -- Delinquency codes for settled a/cs in status 6,7,8
-- Version 3.0  John Croft 29/03/06  Mods for .Net EOD CR781, error checking 
-- John Croft 12/10/07 69317 Scorecard review issues
-- IP - 23/09/09 - 71155 - Application and Delinquency duplicate records.
-- jec  25/07/11 CR1254 RI Integration
-- IP   14/06/13 - #13918 - Settled account was not being extracted
-- IP   14/06/13 - #13919 - Commented out code that set PTPTaken to Y incorrectly for any action taken.
-- IP   17/06/13 - #13921 - Customer Good Bad flag incorrect. Duplicate rows for account were inserted.
--				   CustGoodBadFlag was null for accounts with Status U, 0. Now set to 'N'
-- IP   19/06/13 - #14017 - Duplicate row for account
-- =====================================================================================================================
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DLQ_MthsArrs]') 
            and OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.DLQ_MthsArrs

go

create view dbo.DLQ_MthsArrs 
as select acctno,monthsinarrears=case
	when arrears=0 then 0
	when instalamount>0 then arrears/instalamount
	when instalamount=0 then 0
	end
		
from summary1
		
--where  currstatus<>'S'
--and accttype not in('C','S')
where accttype not in('C','S')

go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[ScorexDelinq]') 
            and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure ScorexDelinq
Go

Create Procedure ScorexDelinq

        @return int OUTPUT

as 

SET NOCOUNT OFF 


/*Allow up to 10 ten days after end of current month to do previous month*/
DECLARE @currentmonth SMALLDATETIME
SET @currentmonth = DATEADD(DAY,-10,GETDATE())
SET @currentmonth= DATEADD(mm, DATEDIFF(mm,0, @currentmonth ), 0)





DECLARE @statement sqltext , @rowcount INT 
--SET @statement = 'ALTER DATABASE ' + DB_NAME() + ' SET recovery SIMPLE '
--EXEC SP_EXECUTESQL @statement

DECLARE @status INTEGER, @rundate dateTIME 
SET @rundate = GETDATE()
SET 	@return = 0			--initialise return code
SET 	@status = 0

/* Create Delinquency table */
IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_name = 'Delinquency')

create table .dbo.Delinquency
(
acctno char(12),
dlq_code smallint,
dlq_balos decimal(11,2),
dlq_arrears decimal(11,2),
dlq_servicechg decimal(11,2),
dlq_interest decimal(11,2),
dlq_charges decimal(11,2),
countrycode char(1)
)
else
    truncate table Delinquency  -- drop table Delinquency

-- go

-- Active accounts - currstatus <> 'S'  
DECLARE @globdelpcent FLOAT 
SELECT @globdelpcent= globdelpcent FROM country

Insert into Delinquency (acctno,dlq_code,dlq_balos,dlq_arrears,
dlq_servicechg,dlq_interest,dlq_charges,countrycode,DelAddress
,countpcent,custid,dateacctopen,currstatus,Currentmonth)

select DISTINCT s.acctno,dlqcode=CASE --IP - 23/09/09 - Livewire (71155) - added DISTINCT
	when agrmttotal=0 and propresult='X' then ' '	-- Rejected		jec 12/10/07
	when datedel1 is null or datedel1='1900-01-01' then 4
	when currstatus in('6','8') then 16
	when currstatus='7' then 15
	when monthsinarrears>12 then 14
	when monthsinarrears>6 then 13
	when monthsinarrears>5 then 12
	when monthsinarrears>4 then 11
	when monthsinarrears>3 then 10
	when monthsinarrears>2 then 9
	when monthsinarrears>1 then 8
	when monthsinarrears>0.01 then 7
	when monthsinarrears>=0 and outstbal>0 then 6
	when monthsinarrears<0 then 5
	when outstbal=0 then 3
	when outstbal<0 then 5
	end,
	 s.outstbal,s.arrears,s.servicechg,0,0,countrycode ,'H' ,
	 @globdelpcent,	ca.custid,dateacctopen,currstatus,@Currentmonth								--#13921
	
from summary1 s inner join custacct ca on s.acctno = ca.acctno and ca.hldorjnt = 'H'			--#13921
LEFT outer join proposal p on s.acctno=p.acctno and p.custid = ca.custid, DLQ_MthsArrs d		--#13921
-- not special or cash accounts
where accttype not in('S','C') and currstatus<>'S' and s.acctno=d.acctno
--and dateacctopen>dateadd(d,-1,cast(dateprop as datetime))	-- select correct proposal if more than 1  --jec 12/10/07		--#13921

SET @status = @@error


/*
 Create setlstatus table to hold date of last status code change before account settled 
 Only where date of last change within last 3 months 
*/

drop table setlstatus

IF EXISTS (SELECT * 
           FROM INFORMATION_SCHEMA.TABLES 
           WHERE TABLE_NAME='setldates')
BEGIN
	drop table setldates			--#13918
END

select s.acctno, max(datestatchge) dt
	into setlstatus 
	from status s 
	where statuscode not in('S')	-- --('U','S','0') jec 20/12/07
 group by s.acctno --#13918 --having datediff(month,max(datestatchge),getdate())<=3 
 order by s.acctno
PRINT CONVERT(VARCHAR,@@ROWCOUNT ) + ' settled accounts inserted' 

--#13918 - select the settled date for accounts settled within the last 3 months
select s.acctno, s.datestatchge
into setldates
from status s inner join setlstatus ss on s.acctno = ss.acctno
where s.statuscode = 'S'
and s.datestatchge = (select max(s1.datestatchge) from status s1
						where s1.acctno = s.acctno)
and getdate() <= dateadd(month, 3, s.datestatchge)

-- settled accounts  - currstatus = 'S' 
Insert into Delinquency (acctno,dlq_code,dlq_balos,dlq_arrears,
						dlq_servicechg,dlq_interest,dlq_charges,countrycode,countpcent,
						custid,dateacctopen,currstatus,currentmonth )

select distinct s.acctno,dlqcode=case
	when statuscode in ('1','0','U') and propresult='X' then ' '	-- Rejected		jec 12/10/07
	when statuscode ='1' and agrmttotal=0 then'4'		-- Cancelled	jec 12/10/07
	when statuscode in('1','2','3','9') then 1
	when statuscode in('4','5') then 2
	when statuscode in('7') then 15    	-- v2.1
	when statuscode in('6','8') then 16	-- v2.1
	else 0
	end,
	s.outstbal,s.arrears,s.servicechg,0,0,country.countrycode ,globdelpcent,
	ca.custid,s.dateacctopen,s.currstatus,@currentmonth											--#13921
from summary1 s inner join custacct ca on s.acctno = ca.acctno and ca.hldorjnt = 'H'			--#13921
 LEFT outer join proposal p on s.acctno=p.acctno and p.custid = ca.custid						--#13921
,setlstatus,status,country, setldates					--#13918
where accttype not in('S','C') and currstatus='S' and
 s.acctno=setlstatus.acctno and status.acctno=setlstatus.acctno and status.datestatchge=dt 
	and status.statuscode not in('S','U') --#14017	--('U','S','0')	jec 20/12/07
	and s.acctno = setldates.acctno   --#13918
--and s.dateacctopen>dateadd(d,-1,cast(dateprop as datetime))	-- select correct proposal if more than 1  --jec 12/10/07		--#13921

SELect @status = @@ERROR, @rowcount = @@ROWCOUNT

PRINT CONVERT(VARCHAR,@rowcount) + ' Number of Accounts Inserted' 

UPDATE Delinquency SET CustWorstCurrentStatus = 
(SELECT MAX(a.currstatus ) FROM acct a , custacct ca 
 WHERE a.acctno= ca.acctno AND ca.hldorjnt='H'
 AND ca.custid = Delinquency.custid AND a.currstatus NOT IN ('U','S','O')
AND ISNUMERIC( a.currstatus) = 1 )

UPDATE Delinquency SET WorstArrearsStatus = 
(SELECT MAX(a.statuscode) FROM status a --, custacct ca 
 WHERE a.acctno= Delinquency.acctno AND  ISNUMERIC(a.statuscode) = 1  )
 



UPDATE d 
SET rfcreditlimit = c.rfcreditlimit ,
Creditblocked= c.creditblocked
FROM delinquency d, customer c 
WHERE d.custid= c.custid 

UPDATE d 
SET OriginalBalance = ISNULL(g.agrmttotal,0)- ISNULL(g.deposit,0),
DeliveryDate = g.datedel   ,
Dateintoarrears = a.dateintoarrears,
instalment = i.instalamount,
ageofaccount = DATEDIFF(day,DATEFIRST ,@rundate)/30.43 ,
InstalNo = i.instalno,
Accttype = a.accttype
FROM delinquency d 
join agreement g ON d.acctno = g.acctno 
JOIN acct a ON g.acctno = a.acctno
LEFT JOIN intratehistory ir ON a.termstype = ir.
	termstype  AND  a.dateacctopen > ir.datefrom 
	AND ( a.dateacctopen < ir.dateto 
	OR ISNULL(ir.dateto,'1-jan-1900')='1-jan-1900')
JOIN instalplan i ON i.acctno= a.acctno 
WHERE G.datedel IS NOT NULL 
SET @status = @@ERROR

IF @status =0
BEGIN
	PRINT 'updating financials '
	-- update payment and int adm charges
	exec ScoreExtractUpdateFinancials @RUNDATE = @rundate
END
 

IF @status =0
BEGIN
	PRINT 'update product categories'
	-- update product cats 1 - 3 
	exec ScoreExtractUpdateProductCategories 
END

IF @status =0
BEGIN
	-- update delinquency details columns numacctsarrears through to worstcurrentstatusChangelast12Months
	Print 'updating with Behavioural information'
	EXEC ScorexDelinqUpdateBehaviouralDetails	
END

-- #13919 - Correct update below
-- ptp in the last month
--UPDATE delinquency SET ptpTaken = 'Y'
--WHERE EXISTS (SELECT * FROM bailaction f 
--WHERE f.acctno= delinquency.acctno AND f.dateadded > DATEADD(MONTH,-1,@rundate))

UPDATE delinquency 
SET Dateallocatedtocollector = (SELECT MIN(datealloc) FROM follupalloc f
							    INNER JOIN Admin.[User] u ON f.empeeno = u.Id
								WHERE f.acctno = delinquency.acctno 
								AND u.id = f.empeeno 
								AND Admin.CheckPermission(u.id,381) = 1)

UPDATE Delinquency 
SET DelAddress = LEFT(l.deliveryaddress,1)
FROM lineitem l WHERE l.acctno = Delinquency.acctno
AND ( l.deliveryaddress LIKE 'W' OR l.deliveryaddress LIKE 'D%') 


UPDATE Delinquency SET Warrantypurchased ='Y'
WHERE  EXISTS (SELECT * FROM lineitem l, StockInfo s
WHERE l.acctno = delinquency.acctno 
--AND l.itemno = s.itemno
AND l.ItemId = s.ID			-- RI 
AND s.category IN (12,82) /*warranty categories*/ )
-- promise to pay
UPDATE Delinquency SET PTPTaken = 'Y'
WHERE  EXISTS (SELECT * FROM bailaction b 
WHERE delinquency.acctno= b.acctno AND b.code = 'PTP'
AND b.dateadded > DATEADD(MONTH,-1,@rundate) )

/*Now we are updating the good bad flag */
BEGIN 
	--UPDATE Delinquency 
	--SET CustGoodBadFlag= 'SG' -- settled Good
	--WHERE EXISTS 
	--(SELECT * FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid AND ca.hldorjnt = 'H'
	--AND a.currstatus ='S' AND a.highststatus IN ('2','1'))

	--UPDATE Delinquency 
	--SET CustGoodBadFlag= 'G' -- current good
	--WHERE EXISTS 
	--(SELECT * FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
	--AND a.currstatus IN ('1','2'))

	--UPDATE Delinquency 
	--SET CustGoodBadFlag= 'N'  -- current neutral
	--WHERE EXISTS 
	--(SELECT * FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
	--AND a.currstatus IN ('3','4'))


	--UPDATE Delinquency 
	--SET CustGoodBadFlag= 'SB' -- settled bad
	--WHERE EXISTS 
	--(SELECT * FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
	--AND a.currstatus ='S' AND a.highststatus IN ('5','6','7','8'))

	--UPDATE Delinquency 
	--SET CustGoodBadFlag= 'B'  -- Bad.
	--WHERE EXISTS 
	--(SELECT * FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
	--AND a.currstatus IN ('5','6','7','8'))

	--UPDATE Delinquency
	--SET CustGoodBadFlag = 
	--	CASE 
	--		WHEN a.currstatus IN ('5','6','7','8') THEN 'B'
	--		WHEN a.currstatus ='S' AND a.highststatus IN ('5','6','7','8') THEN 'SB' 
	--		WHEN a.currstatus IN ('3','4') THEN 'N'
	--		WHEN a.currstatus IN ('1','2') THEN 'G'
	--		WHEN a.currstatus ='S' AND a.highststatus IN ('2','1') THEN 'SG' 	
	--	 END
	--FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid AND ca.hldorjnt = 'H'


	UPDATE Delinquency
	SET CustGoodBadFlag = 
		CASE 
			WHEN EXISTS (SELECT * FROM custacct ca, acct a 
							WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
							AND ca.hldorjnt = 'H'
							AND a.currstatus IN ('5','6','7','8')) THEN 'B'
			WHEN EXISTS(SELECT * FROM custacct ca, acct a 
							WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
							AND ca.hldorjnt = 'H'
							AND a.currstatus ='S' AND a.highststatus IN ('5','6','7','8')) THEN 'SB'
			WHEN EXISTS(SELECT * FROM custacct ca, acct a 
							WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
							AND ca.hldorjnt = 'H'
							AND a.currstatus IN ('3','4')) THEN 'N'
			WHEN EXISTS(SELECT * FROM custacct ca, acct a 
							WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
							AND ca.hldorjnt = 'H'
							AND a.currstatus IN ('1','2')) THEN 'G'
			WHEN EXISTS(SELECT * FROM custacct ca, acct a 
							WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
							AND ca.hldorjnt = 'H'
							AND a.currstatus ='S' AND a.highststatus IN ('2','1')) THEN 'SG'
			ELSE	'N'		-- ones that have not started yet (status - U, 0)
		 END
	--FROM custacct ca, acct a 
	--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid AND ca.hldorjnt = 'H'

END 

--UPDATE Delinquency 
--SET CustGoodBadFlag= 'N' WHERE EXISTS 
--(SELECT * FROM custacct ca, acct a 
--WHERE a.acctno = ca.acctno AND delinquency.custid = ca.custid
--AND a.currstatus IN ('3','4'))

UPDATE Delinquency SET Interestrate =i.intrate
  FROM intratehistory i ,acct a 
  WHERE  i.datefrom < a.DateAcctOpen 
  AND (i.dateto > a.DateAcctOpen OR ISNULL(i.dateto,'1-jan-1900') = '1-Jan-1900')
  AND  a.acctno = Delinquency.acctno
  AND i.termstype = a.termstype 


--SET @statement = 'ALTER DATABASE ' + DB_NAME() + ' SET recovery FULL '  - Doesn't like this being called... 
--EXEC SP_EXECUTESQL @statement

--IF @status != 0
--	BEGIN
--		SET @return = @@error
--	END


Go

-- the original embedded export process is now a separate procedure - "ScorexDLQexport"
/*
 set command string and execute BCP utility 
 export delinquency table to ScorexDLQ.csv


declare @path varchar(200)
set @path = '"C:\MSSQL7\Binn\bcp" ' + 'cosacs..delinquency' + ' out ' +
 'd:\users\default\ScorexDLQ.csv ' + '-c -t, -q -T'

exec master.dbo.xp_cmdshell @path

*/
-- End End End End End End End End End End End End End End End 


GO 
