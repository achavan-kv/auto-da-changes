if exists (select * from dbo.sysobjects where id = object_id('[dbo].[BrokerPopulateCreditAccts]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[BrokerPopulateCreditAccts] 
GO
/****** Object:  StoredProcedure [dbo].[BrokerExtractSP]    Script Date: 11/29/2018 12:44:20 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
-- ========================================================================
-- Version:		<006> 
-- ========================================================================
CREATE PROCEDURE [dbo].[BrokerPopulateCreditAccts] @mindelrunno INT, @maxdelRunno INT, @decimals INT, @runno INT , @query BIT 
as 
-- this procedure populates a #credit table for service charge/ insurance/admin calculation
declare @status INT, 
		@insuranceitemno VARCHAR(10),
		@adminitemno VARCHAR(10),
		@minDate DATETIME,
		@maxDate DATETIME

SELECT @insuranceitemno= Value FROM CountryMaintenance WHERE NAME LIKE 'Insurance charge Item Number'
SELECT @adminitemno=Value FROM CountryMaintenance WHERE codename LIKE 'adminitemno'
SELECT @minDate = DateStart FROM InterfaceControl WHERE RunNo = (SELECT MAX(RunNo) FROM InterfaceControl WHERE RunNo < @mindelrunno AND Interface = 'COS FACT' AND Result = 'P') AND Interface = 'COS FACT'
SELECT @maxDate = DateStart FROM InterfaceControl WHERE RunNo = @maxdelrunno AND Interface = 'COS FACT'

DECLARE @countrycode VARCHAR(1) -----Added by Charudatt

SELECT @countrycode = countrycode FROM country
-- temporary table to hold agreement data
IF NOT EXISTS ( SELECT * FROM tempdb..sysobjects WHERE NAME = '##creditaccts' )
	CREATE TABLE ##creditaccts
	(acctno CHAR(12) NOT NULL ,
	agrmttotal MONEY,
	agrmtyears float,				--#13478				
	servicecharge MONEY,
	insurance MONEY,
	termstype CHAR(4),
	dateacctopen DATETIME,
	administration MONEY,
	inspcent FLOAT,
	adminpcent FLOAT,
	insincluded SMALLINT,
	reducedservice MONEY,
	LastServiceCharge MONEY,		--#13662
	DTWeight FLOAT,					--#13662
	InsuranceWeight FLOAT,			--#13662
	AgreementDateChange DATETIME	--#18537
	)
TRUNCATE TABLE ##creditaccts 

select 
    d.acctno, 
	sum( ROUND(((d.transvalue) / (( 100 + ISNULL(N.taxrate,0))/100) ), @decimals)) as transvalue, 	
	max(d.datetrans) as datetrans
INTO #DTDeliveries
from delivery d INNER JOIN Nonstocks.Nonstock N ON d.itemno= N.SKU 
where d.runno BETWEEN @minDelRunno AND @maxDelRunno 
	AND d.itemno = 'dt'
    and d.BrokerExRunNo = ISNULL(@runno, 0)
GROUP BY d.acctno
--HAVING SUM(d.transvalue) != 0
	
--Lazy way of creating a temp table
SELECT TOP 0 *
INTO #agreementAudit 
FROM agreementAudit

--Select the first change in the agreement total for all accounts considered
INSERT INTO #agreementAudit
SELECT * FROM agreementAudit aa
WHERE aa.acctno IN (SELECT acctno FROM #DTDeliveries)
	AND aa.datechange = (SELECT MIN(datechange) FROM agreementAudit WHERE acctno = aa.acctno AND datechange >= @minDate)

--Update the new values based on the latest agreement change within this run
UPDATE aa
SET aa.NewAgreementTotal = a.NewAgreementTotal,
	aa.NewServiceCharge = a.NewServiceCharge,
	aa.Newdeposit = a.Newdeposit,
	aa.NewTermsType = a.NewTermsType,
	aa.NewCODflag = a.NewCODflag,
	aa.empeenochange = a.empeenochange,
	aa.datechange = a.datechange
FROM #agreementAudit aa
INNER JOIN agreementAudit a
ON aa.acctno = a.acctno
WHERE a.datechange = (SELECT MAX(datechange) FROM agreementAudit WHERE acctno = a.acctno AND datechange <= @maxDate)
	
--Add row to main table
	INSERT INTO ##creditaccts (
		acctno,
		agrmttotal,
		agrmtyears,					
		servicecharge,
		insurance,					
		termstype,
		dateacctopen,
		administration,				
		inspcent ,					
		adminpcent ,				
		insincluded ,				
		reducedservice,				
		LastServiceCharge,			
		DTWeight,					
		InsuranceWeight,			
		AgreementDateChange			
	) 
	SELECT 
		a.acctno,
		case when transvalue >= 0 then aa.NewAgreementTotal else aa.OldAgreementTotal end, 
		round(cast(case when transvalue >= 0 then ia.Newinstalno else ia.Oldinstalno end / 12.0  as float),2), 
		case 
            when transvalue >= 0 then aa.NewServiceCharge 
            else ISNULL(NULLIF(aa.OldServiceCharge, 0), aa.NewServiceCharge)
        end,
		0,
		case when transvalue >= 0 then aa.NewTermsType else aa.OldTermsType end,
		a.dateacctopen,
		0,0,0,0,0,
		SUM(ROUND(d.transvalue, @decimals)),
		0,0,
		aa.DateChange				
	FROM agreement g
	JOIN acct a ON g.acctno = a.acctno
    JOIN #DTDeliveries d on d.acctno = a.acctno
	JOIN agreementAudit aa on aa.acctno = d.acctno
	JOIN instalplanAudit ia on ia.acctno = d.acctno           
	WHERE a.acctno LIKE '___0%'
	AND aa.datechange = (select MAX(datechange) from agreementAudit b where b.acctno = d.acctno and b.datechange < d.datetrans)
	AND ia.datechange = (select MAX(datechange) from instalplanaudit c where c.acctno = d.acctno and c.datechange < d.datetrans)
    	AND NOT EXISTS(select 1 from ReadyAssistDetails ra  where ra.acctno = g.acctno) 
	AND d.transvalue != 0 ----Added by Charudatt on 24/10/2018
	GROUP BY 
		a.acctno,
		case when transvalue >= 0 then aa.NewAgreementTotal else aa.OldAgreementTotal end,
		case 
            when transvalue >= 0 then aa.NewServiceCharge 
            else ISNULL(NULLIF(aa.OldServiceCharge, 0), aa.NewServiceCharge)
        end, 
		case when transvalue >= 0 then aa.NewTermsType else aa.OldTermsType end,
		a.dateacctopen, 
		case when transvalue >= 0 then ia.Newinstalno else ia.Oldinstalno end,
		aa.datechange
    --HAVING SUM(d.transvalue) != 0



--Inurance and admin charge details
UPDATE ##creditaccts 
SET inspcent = i.inspcent,
adminpcent = i.AdminPcent,
insincluded = i.InsIncluded 
FROM intratehistory i 
WHERE dateacctopen >= i.datefrom 
	AND (dateacctopen <=i.dateto OR i.dateto= '1-jan-1900') 
	AND i.termstype = ##creditaccts.termstype

-- take out insurance and admin percent from deferred terms 

update ##creditaccts set DTWeight = LastServiceCharge / servicecharge  Where servicecharge !=0		

UPDATE ##creditaccts SET insurance = round((agrmttotal) * agrmtyears * (inspcent / 100), @decimals)	
WHERE insincluded = 1 AND (ISNULL(inspcent,0) + ISNULL(adminpcent,0)) > 0 

UPDATE ##creditaccts SET insurance = 0 WHERE insurance IS NULL 

UPDATE ##creditaccts SET administration =0 WHERE administration IS NULL


update ##creditaccts set InsuranceWeight = round(insurance * DTWeight, @decimals)

--Select delivery of Insurance relating to the agreementaudit.DateChange
IF @countrycode = 'J' 
	BEGIN
		UPDATE ##creditaccts SET InsuranceWeight =  
		ISNULL((SELECT SUM(d.transvalue) FROM delivery d WHERE d.runno BETWEEN @minDelRunno AND @maxDelRunno   
		AND d.itemno =@insuranceitemno AND d.acctno=##creditaccts.acctno AND d.BrokerExRunNo = @runno ),0)
	END
ELSE
	BEGIN
		UPDATE ##creditaccts 
		SET InsuranceWeight = ISNULL(InsuranceWeight, 0) + ISNULL((SELECT ROUND(SUM((d.transvalue) / (( 100 + ISNULL(N.taxrate,0))/100) ), @decimals) 
														   FROM delivery d INNER JOIN Nonstocks.Nonstock N ON d.itemno= N.SKU 
														   WHERE d.runno BETWEEN @minDelRunno AND @maxDelRunno 
 															  AND d.itemno =@insuranceitemno 
															  AND d.acctno=##creditaccts.acctno   ),  0)
	END
		
		UPDATE ##creditaccts 
		SET administration = ISNULL(administration,0) + ISNULL((SELECT ROUND(SUM((d.transvalue) / (( 100 + ISNULL(N.taxrate,0))/100) ), @decimals) 
														FROM delivery d INNER JOIN Nonstocks.Nonstock N ON d.itemno= N.SKU 
														WHERE d.runno BETWEEN @minDelRunno AND @maxDelRunno 
 															AND d.itemno =@adminitemno 
															AND d.acctno=##creditaccts.acctno AND d.BrokerExRunNo = @runno),0)

UPDATE ##creditaccts 
SET reducedservice = round(LastServiceCharge - InsuranceWeight, @decimals)	
WHERE (ISNULL(inspcent,0) + ISNULL(adminpcent,0)) > 0 
	AND insincluded = 1

UPDATE ##creditaccts 
SET reducedservice = LastServiceCharge
WHERE (ISNULL(inspcent,0) + ISNULL(adminpcent,0)) = 0 
	OR insincluded = 0
-- its called from the query routine so want to populate the totals for the query screen. 
IF @query = 1
	EXEC BrokerQueryPopulateServiceInsurance @mindelrunno =@mindelrunno , @maxdelrunno =@maxdelrunno,  @runno =@runno
 

 --SELECT * FROM ##creditaccts

 
DROP TABLE #DTDeliveries
DROP TABLE #agreementAudit

