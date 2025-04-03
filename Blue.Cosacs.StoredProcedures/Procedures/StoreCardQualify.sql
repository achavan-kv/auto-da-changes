--*************************************************
-- Script Name : 4761758_StoreCardQualify.sql
-- Created For	: Barbados
-- Created By	: Manoj Harne
-- Created On	: 26/02/2018
--*************************************************
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
/* 2/26/2018 9:18:52  #4761758 - Received large data in storecard  added the filtered for duration month */
-- **********************************************************************

-- Modified On	Modified By	Comment
-- DATE   Modify_By    Remark 
--*************************************************
/****** Object:  StoredProcedure [dbo].[StoreCardQualify]  Script Date: 26/02/2018 9:18:52 AM ******/
/******  ******/
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[StoreCardQualify]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[StoreCardQualify]
GO

CREATE PROCEDURE [dbo].[StoreCardQualify]
@custid VARCHAR(20), 
@points INT , 
@scorecard CHAR(1),
@StoreCardApproved BIT OUT, 
@rundate Datetime
-- **********************************************************************
AS 
SET NOCOUNT ON 
SET ARITHABORT ON  

Declare @minRFSpendLimit money,
		@minRFAvailSpend money,
		@issueCard BIT,	--IP - 05/05/11
		@storecardpcent FLOAT,
		@durationmonth int

			/* #4761758 - Received large data in storecard  added the filtered for duration month */
		select @durationmonth = value from countrymaintenance where codename = 'addtoterm' /* #4774465 -- 15/02/2018   */

--Select Country Parameter values to be used below to determine if a customer qualifies based on the spend limit criteria
select @minRFSpendLimit = value from countrymaintenance where codename = 'MinStoreCardLimit'
select @minRFAvailSpend = value from countrymaintenance where codename = 'StoreCardMinRFAvail'

SELECT @storecardpcent = CONVERT(FLOAT,value)  
FROM CountryMaintenance 
WHERE CodeName='StorecardPercent'
	
--select @issueCard = value from countrymaintenance where codename = 'StoreCardIssueCardPreAppr'	--IP - 05/05/11
set @StoreCardApproved = 0

IF @points = 0 AND @custid !=''  -- if blank points retrieve from database 
	SELECT @points = points FROM proposal 
	WHERE acctno NOT LIKE '___9%'
	AND points >0 -- actually scored.. 
	AND custid = @custid
	order by dateprop DESC 


--IP - 04/05/11 - Check if customer has already been store card approved. If so then do not process.
	if(@custid!='')
	begin

		if (select creditblocked from Customer where custid = @custid) = 1
		BEGIN
			set @StoreCardApproved = 0
			return
		END

		select @StoreCardApproved = storecardapproved from Customer where custid = @custid
		
		if(@StoreCardApproved = 1) 
			return
	end
	
--Select 
	CREATE TABLE #CustomersPreApproved 
	(
		BranchNo smallint,
		CustID varchar(20),
		CustomerName varchar(100),
		RFCreditLimit money,
		RFAvailableSpend money,
		AcctNo varchar(12),
		Arrears money,
		CurrStatus char(1),
		ScoreCardType char(1),
		Points int,
		DateAcctOpen datetime,
		InstalAmount money,
		CurrentMthsArrs float,
		DateLast datetime,
		DateSettled datetime,
		MthsAcctHistFrom datetime,
		MthsAcctHistTo datetime,
		PrevMthsArrs float,
		PrevMthsArrsStatus FLOAT, 
		PreApproved int,
		CardNumber bigint,
		StoreCardAcctNo varchar(12)
	)
	
	CREATE CLUSTERED INDEX ix_customers3w34rsdcustid ON #CustomersPreApproved(custid,acctno) 
	
	--First select all customers that have not been preapproved
	INSERT into #CustomersPreApproved
	select null,
		   c.custid,
		   c.title + ' ' + c.firstname + ' ' + c.name,
		   c.RFCreditLimit,
		   c.AvailableSpend,
		   a.acctno,
		   a.arrears,
		   a.currstatus,
		   case when @scorecard !='' then @scorecard else c.scorecardtype end,	--IP - 04/05/11 - select based on running from EOD or for individual customer
		   case when @points != 0 then @points else 0 end, --IP - 04/05/11 - select based on running from EOD or for individual customer,
		   a.dateacctopen,
		   i.instalamount,
		   a.monthsinarrears,
		   i.datelast,
		   null,
		   null, 
		   null,
		   null,
		   NULL,
		   0,
		   null,
		   null
	FROM customer c 
	inner join custacct ca on c.custid = ca.custid
	inner join acct a on ca.acctno = a.acctno
	inner join proposal p on a.acctno = p.acctno
	inner join instalplan i on a.acctno = i.acctno
	where c.creditblocked = 0
	and ca.hldorjnt = 'H' 
	and ( isnull(c.storecardapproved,0) = 0	--IP - 06/05/11 - #3595
	OR EXISTS  (SELECT * 
					FROM custacct ca3
					INNER JOIN storecardpaymentdetails scpd ON ca3.acctno = scpd.acctno 
					WHERE isnull(nextContactDate,getdate()) < @rundate -- so if next contact date null dont contact
					AND ca3.custid = c.custid )) 
	and a.accttype in ('O', 'R')
	and exists (select * 
				from custacct ca2 
				inner join acct a2 on ca2.acctno = a2.acctno	--RF account must exist
				and a2.accttype = 'R'
				and ca2.custid = ca.custid)
	and not exists (select * from cancellation can	--IP - 04/05/11 - Exclude cancelled accounts from being considered.
					where can.acctno = a.acctno)
	and ((@custid!='' and StoreCardApproved = 0 and c.custid = @custid) 
		  or @custid = '') --IP - 04/05/11 - selection based on running from End Of Day for all customers or an individual customer
	AND NOT EXISTS (SELECT * 
					FROM custacct ca3
					INNER JOIN storecardpaymentdetails scpd ON ca3.acctno = scpd.acctno 
					--WHERE isnull(nextContactDate,getdate()) >= @rundate -- so if next contact date null dont contact
					WHERE isnull(nextContactDate,'1900-01-01') >= @rundate -- so if next contact date null dont contact		#11206 - LW75408
					AND ca3.custid = c.custid ) --AND b.currstatus !='S') -- exclude customers with existing StoreCard Accounts
    AND NOT EXISTS (SELECT 1 
					FROM VIEW_StoreCardStatusLatest stc
					INNER JOIN custacct ca4 on ca4.acctno = stc.acctno
					WHERE hldorjnt = 'H' AND stc.StatusCode ='A' -- so if customer has any cards which are active don't approve
					AND ca4.custid = c.custid)
	AND c.RFCreditLimit > 0 
    -- may already have settled accounts.... 
	
	 
	--Update the Branch to be the branch number of the customers latest RF account. Rules will be based on this branch.
	UPDATE C
	SET BranchNo = left(a.acctno, 3)
	from  #CustomersPreApproved c
	INNER JOIN custacct ca on ca.custid = c.custid
	INNER JOIN acct a on ca.acctno = a.acctno
	Where a.accttype = 'R'
	AND a.dateacctopen = (SELECT MAX(a1.dateacctopen) 
						  FROM custacct ca1
						  inner join acct a1 on ca1.acctno = a1.acctno
						  and a1.accttype = 'R' AND ca1.hldorjnt = 'H'
						  and ca1.custid = ca.custid)

	-- remove those customers whos latest account not in qualifcation branch 					 
	DELETE FROM #CustomersPreApproved WHERE NOT EXISTS (SELECT * FROM StoreCardBranchQualRules r, CUSTACCT CA WHERE R.branchno= 	
	CONVERT(INT,LEFT(CA.ACCTNO,3)) AND CA.custid = #CustomersPreApproved.CustID)
	--Select the settled date for settled accounts
	UPDATE #CustomersPreApproved
	SET DateSettled = (select max(s.datestatchge) from [status] s
						where s.acctno = #CustomersPreApproved.acctno)
	WHERE #CustomersPreApproved.currstatus = 'S'

	--Update the datefrom and dateto to work out the account history months	 
	UPDATE #CustomersPreApproved
	SET MthsAcctHistFrom = case when c.dateacctopen > dateadd(m, -s.MinMthsAcctHistY, @rundate) then c.dateacctopen
								else dateadd(m, -s.MinMthsAcctHistY, @rundate) end,
		MthsAcctHistTo = case when c.DateSettled is not null then c.DateSettled
						 when c.DateLast < @rundate then c.DateLast 
						 else @rundate end
	FROM #CustomersPreApproved c inner join storecardbranchqualrules s on c.BranchNo = s.branchno
	WHERE s.MinMthsAcctHistY is not null --IP - 05/05/11
	
	--Set previous months in arrears
	UPDATE #CustomersPreApproved
	SET PrevMthsArrs = isnull((select max(ad.arrears)/c.instalamount from arrearsdaily ad
							where ad.acctno = c.acctno
							and (ad.datefrom > dateadd(m, -ISNULL(s.MaxPrevMthsInArrsY,999), @rundate) or ad.dateto is null) and c.instalamount !=0),0)
	FROM #CustomersPreApproved c inner join storecardbranchqualrules s on c.BranchNo = s.branchno
	
	-- using status for older previous months arrears
	UPDATE #CustomersPreApproved
	SET PrevMthsArrsStatus = isnull((select CONVERT(FLOAT,max(ad.statuscode-1)) 
							
							FROM status ad
							where ad.acctno = c.acctno AND ad.statuscode IN ('2','3','4','5')
							and (ad.datestatchge> dateadd(m, -ISNULL(s.MaxPrevMthsInArrsY,999), @rundate) 
							and c.instalamount !=0)),PrevMthsArrs)
	FROM #CustomersPreApproved c inner join storecardbranchqualrules s on c.BranchNo = s.branchno
	
	UPDATE #CustomersPreApproved SET PrevMthsArrs = PrevMthsArrsStatus 
	WHERE PrevMthsArrsStatus > PrevMthsArrs
	
	--Select the points from the latest proposal
	UPDATE #CustomersPreApproved
	SET points = p.points
	FROM #CustomersPreApproved c inner join proposal p on c.custid = p.custid
	WHERE p.dateprop >= (select max(dateprop)from proposal p1
							where p1.custid = p.custid and p1.points <> 0)
	AND @custid = ''	--IP - 04/05/11 - only set the points when processing from End Of Day

	--IP - 05/05/11 - Added OR conditions to ignore rules that have not been set.
	UPDATE #CustomersPreApproved
	SET PreApproved = 1
	FROM #CustomersPreApproved c, storecardbranchqualrules s 
	WHERE c.BranchNo = s.branchno
	AND ((isnull(c.ScoreCardType,'')='A' and ((c.points >= s.MinApplicationScore AND s.MinApplicationScore is NOT NULL) or s.MinApplicationScore is NULL))
		OR (ISNULL(c.ScoreCardType,'') = 'B' and ((c.points >= s.MinBehaviouralScore AND s.MinBehaviouralScore is NOT NULL) or s.MinBehaviouralScore is NULL))
	    ) 
	AND ((datediff(m,c.MthsAcctHistFrom, c.MthsAcctHistTo) >= s.MinMthsAcctHistX AND s.MinMthsAcctHistX is NOT NULL)or s.MinMthsAcctHistX is NULL or s.MinMthsAcctHistX = 0)  
	--IP - 04/05/11 - Added OR condition //IP - 13/05/11 - ignore if MinMthsAcctHistX = 0
	AND c.RFAvailableSpend >= @minRFAvailSpend 
	AND c.RFCreditLimit >= @minRFSpendLimit
	AND ((c.RFAvailableSpend / c.RFCreditLimit >= s.PcentInitRFLimit / 100)
	     OR s.PcentInitRFLimit IS NULL)
	 
	--Update PreApproved = 0 if any active credit accounts belonging to a customer has current month in arrears > 
	--than that specified in the rule.

	UPDATE #CustomersPreApproved
	SET PreApproved = 0
	FROM #CustomersPreApproved c
	WHERE EXISTS(select * from #CustomersPreApproved c2 inner join storecardbranchqualrules s on c2.BranchNo = s.branchno
					where c2.custid = c.custid
					and c2.CurrentMthsArrs > s.MaxCurrMthsInArrs and s.MaxCurrMthsInArrs is NOT NULL --IP - 13/05/11 - removed check on c2.CurrentMthsArrs >=0
					and c2.CurrStatus!='S')



	--Update PreApproved = 0 if any credit accounts previous arrears exceed the criteria specified in the rule.
	UPDATE #CustomersPreApproved
	SET PreApproved = 0
	FROM #CustomersPreApproved c
	WHERE EXISTS(select * from #CustomersPreApproved c2 inner join storecardbranchqualrules s on c2.BranchNo = s.branchno
					where c2.PrevMthsArrs > s.MaxPrevMthsInArrsX AND s.MaxPrevMthsInArrsX is NOT NULL
					and c2.custid = c.custid) 
	create table #maxCusts (custid VARCHAR(20))

	CREATE CLUSTERED INDEX ix_maxcust3343s ON  #maxCusts(custid)

	-- limit to maximum per branch
	declare @MaxNoCustForApproval INT , @branchno INT  
	declare acct_cursor CURSOR FAST_FORWARD READ_ONLY FOR
	SELECT ISNULL(MaxNoCustForApproval,999999), BranchNo 
	FROM StoreCardBranchQualRules
	OPEN acct_cursor
	FETCH NEXT FROM acct_cursor INTO @MaxNoCustForApproval,@branchno
	WHILE @@FETCH_STATUS = 0
	BEGIN

		INSERT INTO #maxCusts (
			custid
		) 
		SELECT TOP (@MaxNoCustForApproval) custid FROM 
		#CustomersPreApproved c WHERE PreApproved  >0
		AND branchno= @branchno 
		GROUP BY custid 
		order by custid 
	FETCH NEXT FROM acct_cursor INTO @MaxNoCustForApproval,@branchno


	END

	CLOSE acct_cursor
	DEALLOCATE acct_cursor

	DELETE FROM #CustomersPreApproved 
	WHERE NOT EXISTS (SELECT * FROM #maxCusts m WHERE m.custid  = #CustomersPreApproved.custid)
	
	delete from #CustomersPreApproved  where isnull(preapproved,0)=0
	
	DECLARE @runno INT SELECT @runno= MAX(runno) 
	FROM interfacecontrol WHERE Interface='STCARDQUAL'
		INSERT INTO dbo.letter (
		runno,		acctno,		dateacctlttr,
		datedue,		lettercode,		addtovalue,
		ExcelGen
	) 
	SELECT DISTINCT @runno,p.acctno, @rundate, 
	@rundate,'STQ', 0,
	0
	FROM customer c 
	JOIN dbo.proposal p ON p.custid = c.custid 
	WHERE p.dateprop >= (SELECT MAX(DATEPROP) FROM dbo.proposal PA
	WHERE pa.custid = p.custid AND pa.points <>0 )
--	AND ISNULL(c.storecardapproved,0) =0 Generating more letters for those being recontacted... 
	AND EXISTS(SELECT preapproved FROM   #CustomersPreApproved cp1
	WHERE cp1.custid = c.custid AND preapproved=1)
	--Update the customer table to mark the customer as StoreCardApproved				

	

	UPDATE customer SET StoreCardLimit = RFCreditLimit * @storecardpcent / 100,
	StoreCardAvailable = RFCreditLimit * @storecardpcent / 100,			-- #11601 
	StoreCardApproved = 1,
		SCardApprovedDate = @rundate
	FROM StoreCardBranchQualRules Q, proposal P 
	WHERE q.BranchNo = CONVERT(int,LEFT(p.acctno,3))
	AND p.custid = customer.custid 
	AND p.dateprop = (SELECT MAX(pd.dateprop) FROM proposal pd
	WHERE pd.custid = p.custid --AND p.acctno LIKE CONVERT(VARCHAR,q.BranchNo) +'0%' 
	)
	--AND ISNULL(customer.StoreCardApproved,0) =0 Removing to allow recontact.....
	AND EXISTS
	(select cp1.PreApproved from #CustomersPreApproved cp1
	WHERE  cp1.custid = customer.custid AND cp1.PreApproved >0)
	
	SELECT @StoreCardApproved = StoreCardApproved FROM customer 
	WHERE custid = @custid
	and @custid!=''	--IP - 04/05/11 - Only select if processing for an indivdual customer

-- Now mark those customers as unpreapproved and those storecards status as suspended where expiry...
declare @expirymonths int , @expireddate DATETIME 

SELECT @expirymonths= CONVERT(INT,Value) FROM CountryMaintenance WHERE CodeName = 'SCardPreExpiryMonths'
IF @expirymonths > 0
BEGIN
	SET @expireddate= DATEADD(MONTH,-@expirymonths,GETDATE())

	UPDATE customer 
	SET StoreCardApproved = 0,
		StoreCardAvailable = 0,
		StoreCardLimit = 0 ,
		SCardApprovedDate =NULL 
	WHERE StoreCardApproved = 1 
	AND SCardApprovedDate < dateadd(day,-1,@expireddate)
	AND NOT EXISTS (SELECT * FROM custacct ca , storecardpaymentdetails s, acct a  
					WHERE ca.acctno = s.AcctNo 
					AND ca.custid = customer.custid 
					AND a.acctno = ca.acctno
					AND ca.hldorjnt = 'H' AND (a.outstbal <>0  OR s.nextcontactdate > @rundate))
	AND (NOT EXISTS (SELECT * FROM StoreCardStatus ss
					 INNER JOIN StoreCard ON ss.CardNumber  = StoreCard.CardNumber
					 INNER JOIN custacct ca2 ON StoreCard.AcctNo = ca2.acctno
					 WHERE ca2.custid = customer.custid AND ss.statusCode ='A'))
	
	UPDATE StoreCardPaymentDetails SET Status = 'S'
	FROM CUSTOMER C,custacct ca WHERE SCardApprovedDate < @expireddate
	AND StoreCardApproved = 0  AND status !='S' and c.custid = ca.custid
	and ca.hldorjnt='H' and StoreCardPaymentDetails.acctno= ca.acctno
	
	UPDATE StoreCardPaymentDetails SET NextContactDate = NULL  
	FROM CUSTOMER C,custacct ca WHERE NextContactDate < @rundate 
	AND StoreCardApproved = 1  and c.custid = ca.custid
	and ca.hldorjnt='H' and StoreCardPaymentDetails.acctno= ca.acctno
	 
END 
DECLARE @expmonths int
SELECT @expmonths = CONVERT(INT,value) FROM CountryMaintenance WHERE CodeName LIKE 'SCardPreExpiryMonths'
TRUNCATE TABLE StoreCardLastAppSuccess
INSERT INTO StoreCardLastAppSuccess (
	custid,	Title,	Firstname, NAME,
	cusaddr1,	cusaddr2,	cusaddr3,	cuspocode,
	branchname,	branchaddr1,	branchaddr2,	branchaddr3,
	StoreCardLimit,	ApprovalDate,	OfferExpiryDate,	runno,
	HomePhone,	MobilePhone,	branchno, 
	CreditScore , MonthsCreditHistory , ArrearsInst , -- extra columns #3863
	MaxArrearsInstEver ,rflimit -- extra columns #3863
) 
SELECT  ca.custid,c.title ,c.FirstName,c.[Name],
cd.cusaddr1,cd.cusaddr2,cd.cusaddr3,cd.cuspocode,
MAX(b.branchname), MAX(b.branchaddr1),MAX(b.branchaddr2),MAX(b.branchaddr3), 
c.StoreCardLimit,@rundate , DATEADD(MONTH,@expmonths,@rundate) AS OfferExpiryDate,runno,
MAX(t.telno) AS HomePhone, MAX(m.telno) AS MobilePhone ,MAX(b.branchno),
MAX(pa.Points) ,MAX(DATEDIFF(month,pa.MthsAcctHistFrom ,pa.MthsAcctHistTo)),
MAX(pa.CurrentMthsArrs) ,MAX(pa.PrevMthsArrs),
c.rfcreditlimit
FROM letter l JOIN custacct ca ON l.acctno = ca.acctno
JOIN customer c ON ca.custid = c.custid
JOIN #CustomersPreApproved pa ON pa.CustID = c.custid
JOIN custaddress cd ON c.custid = cd.custid
LEFT JOIN custtel t ON c.custid = t.custid AND t.datediscon IS NULL  AND t.tellocn='H'
LEFT JOIN custtel m ON t.custid = m.custid AND m.datediscon IS NULL   AND m.tellocn='M'
JOIN branch b ON b.branchno= CONVERT(SMALLINT,LEFT(ca.acctno,3))
										/* #4761758 - Received large data in storecard  added the filtered for duration month */
WHERE ca.hldorjnt = 'H' AND cd.addtype = 'H' AND cd.datemoved IS NULL AND l.dateacctlttr =@rundate 
GROUP BY ca.custid,c.title ,c.FirstName,c.[Name],
cd.cusaddr1,cd.cusaddr2,cd.cusaddr3,cd.cuspocode,
c.StoreCardLimit,runno,c.rfcreditlimit

UPDATE StoreCardLastAppSuccess SET   ArrearsInst= 0 WHERE  ArrearsInst<0 
UPDATE StoreCardLastAppSuccess SET   MaxArrearsInstEver= 0 WHERE  MaxArrearsInstEver<0 

SELECT custid, Title, FirstName, [Name], cusaddr1, cusaddr2, cusaddr3, cuspocode, 
branchname, branchaddr1, branchaddr2, branchaddr3, StoreCardLimit, ApprovalDate AS OfferApprovalDate, 
OfferExpiryDate, runno, HomePhone, MobilePhone, branchno,
CreditScore , MonthsCreditHistory , ArrearsInst , MaxArrearsInstEver ,RFlimit -- extra columns #3863
FROM StoreCardLastAppSuccess

GO