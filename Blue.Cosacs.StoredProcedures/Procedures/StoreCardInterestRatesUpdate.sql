IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME ='StoreCardInterestRatesUpdate')
DROP PROCEDURE StoreCardInterestRatesUpdate
GO
CREATE PROCEDURE StoreCardInterestRatesUpdate
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : StoreCardInterestRatesUpdate.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update StoreCardPaymentDetails Interest Rate
-- Author       : ??
-- Date         : ??
--
--
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/04/12  IP  #9894 - UAT143 - Interest Rate was incorrectly being updated to the incorret Interest Option and Rate 
--				 (applied the most recently added rate) not taking into consideration the rate option currently selected.
-- 22/05/12  IP  #10083 - The rate was not being selected correctly due to incorrect join.
-- 04/10/12  IP  #11401 - LW75479 - InterestRate was not being updated after change made in Store Card Interest Rate screen.
-- ================================================
@rundate DATETIME
AS 
DECLARE @previousrundate DATETIME
SELECT @previousrundate = ISNULL(MAX(DateFinish),'1-jan-1900')
FROM interfacecontrol 
WHERE interface = 'STINTEREST' AND result = 'A'
AND DateStart < @rundate AND runno >1
SELECT @previousrundate

UPDATE srd
SET InterestRate= R.PurchaseInterestRate, Rateid = rd.id 
FROM StoreCardPaymentDetails srd, 
	 proposal p , 
	 StoreCardRateAudit R, 
	 custacct ca ,
	 StoreCardRateDetails Rd
WHERE ca.acctno = srd.acctno 
AND rd.PurchaseInterestRate = r.PurchaseInterestRate
AND r.id = rd.parentid										--IP - 12/04/12 - #9894 - UAT143
AND ca.hldorjnt ='H'   
AND p.custid = ca.custid 
AND p.dateprop = (SELECT MAX(pp.dateprop) FROM proposal pp WHERE pp.custid = p.custid AND pp.acctno NOT LIKE '___9%' AND pp.points >0) --IP - 04/10/12 - #11401 
AND ISNULL(srd.RateFixed,0) = 0 
AND r.[$Action] = 'I' -- insert 
AND r.[$CreatedOn] = (SELECT MAX(ra.[$CreatedOn]) FROM StoreCardRateAudit ra WHERE ra.[$Action] ='I' 
						AND ((ISNULL(p.scorecard,'A') = 'A' AND p.points BETWEEN ra.AppScorefrom AND ra.AppScoreTo )
							OR  (ISNULL(p.scorecard,'A') = 'B' AND p.points BETWEEN ra.BehaveScoreFrom AND ra.BehaveScoreTo))
						AND ra.[$CreatedOn] > @previousrundate
						--AND ra.[Id] = srd.RateId			--IP - 12/04/12 - #9894 - UAT143
						AND ra.[id] = rd.ParentID			--IP - 22/05/12 - #10083 - UAT160
						AND rd.id = srd.RateID				--IP - 22/05/12 - #10083 - UAT160
					   )

--GO 
--BEGIN TRAN
--SELECT rateid,InterestRate FROM StoreCardPaymentDetails WHERE acctno= '700900000321' 
--EXEC StoreCardInterestRatesUpdate @rundate ='2011-09-19 15:29:04.683'

--SELECT rateid,InterestRate FROM StoreCardPaymentDetails WHERE acctno= '700900000321'
--ROLLBACK