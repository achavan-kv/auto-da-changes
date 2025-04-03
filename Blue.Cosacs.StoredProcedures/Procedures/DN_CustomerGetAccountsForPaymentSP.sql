-- PC 15/11/06 Modfied to pull in service request numbers for charge accounts related to payements
-- RD & DR 30/11/04 Modified to add segmentid column for Tallyman

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetAccountsForPaymentSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetAccountsForPaymentSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerGetAccountsForPaymentSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerGetAccountsForPaymentSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Customer Get Accounts For Payment 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the accounts for the payment screen.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/02/10  jec CR1072 Malaysia Merge
-- 21/07/11  ip  #4305 - Return the AccountType as 'SRC' for a Service Charge account.
-- 17/04/12  jec #9919 Show Minimum payment/monthly payment for Storecard  accounts
-- 22/01/14  ip #17083 Service Request charge account table changed from SR_ChargeAcct to ServiceChargeAcct
-- ================================================
	-- Add the parameters for the stored procedure here
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	;WITH SCStatement AS
(
	select p.acctno,case when MonthlyAmount>MinimumPayment then MonthlyAmount else MinimumPayment end as MinimumPayment
	from custacct ca LEFT OUTER JOIN StoreCardPaymentDetails p on ca.acctno = p.acctno
	where hldorjnt='H' and custid=@custid
	and p.acctno like '___9%'
)

	SELECT	'' as 'LockedBy',			--holds the user who has the account locked
 			ISNULL(FOA.empeeno,0) AS empeeno,
			ISNULL(U.FullName,'') AS EmployeeName,
			A.acctno as 'acctno',
			--AT.accttype as 'AccountType',
			CASE WHEN SCA.acctno IS NOT NULL THEN 'SRC'						--IP - 21/07/11 - #4305
				ELSE AT.accttype END as 'AccountType',
			A.agrmttotal as 'AgreementTotal',
			A.arrears as 'Arrears',
			ISNULL(A.outstbal,0) as 'Outstanding Bal',
			max(AG.DeliveryFlag) as DeliveryFlag,
			A.currstatus as 'Status',
			case when AT.accttype='T' then sc.MinimumPayment						-- #9919
				else ISNULL(IP.instalamount,0)  end as 'Instal Amount',
			ISNULL(IP.datefirst, '1/1/1900') as 'DateFirst',
			0 as 'DeliveredIndicator',		--delivered indicator
			0.0 as 'Rebate',			--rebate			these fields will be calculated in the business layer
		 	0.0 as 'CollectionFee',			-- charge to account
		 	0.0 as 'BailiffFee',			-- commission to bailiff
			0.0 as 'SettlementFigure',		--settlement figure
			0.0 as 'ToFollowAmount',		--To follow amount
			0 as 'debitaccount',		--bailcommnbas debit account flag will be added here
			0.0 as 'CalculatedFee',		--calculated fee may differ from actual fee
			0 as 'FreeInstalment',                     --flag to award a free instalment
			A.bdwbalance,
			A.bdwcharges,
            A.Securitised,
			max(AG.paymentholidays) as paymentholidays,
			min(AG.agrmtno) as agrmtno,
			isnull(IRH.paymentholidaymin, IP.instalno) as paymentholidaymin,
			convert (integer,max(AG.paymentcardline)) AS paymentcardline,
			A.dateacctopen,
			isnull(Segment_ID,0) as 'Segment_ID',
			Segment_Name,		
			A.TallymanAcct,
			dbo.fn_SRGetServiceRequestNo(SCA.ServiceRequestNo) [ServiceRequestNoStr],
			ISNULL(ST.Internal,'') [Internal]
	into #accts
	FROM		CustAcct CA, AcctType AT,  Agreement AG,  Acct A
	LEFT OUTER JOIN FollupAlloc FOA   ON FOA.AcctNo = A.Acctno
	AND		isnull(FOA.datealloc, '1/1/1900') != '1/1/1900'  	AND	isnull(FOA.datealloc, '1/1/1900') <= getdate()
	AND		(isnull(FOA.datedealloc, '1/1/1900') = '1/1/1900' 	OR 	isnull(FOA.datedealloc, '1/1/1900') >= getdate())
	LEFT OUTER JOIN Admin.[User] u ON u.id = foa.empeeno
	LEFT OUTER JOIN InstalPlan IP ON IP.AcctNo = A.AcctNo 
	LEFT OUTER JOIN intratehistory IRH ON A.termstype = IRH.termstype
	AND		A.dateacctopen >= IRH.datefrom
	AND		(A.dateacctopen <= IRH.dateto
	OR		IRH.dateto = CONVERT(DATETIME, '01-01-1900', 105))
	LEFT OUTER JOIN TM_Segments TMS on a.acctno = TMS.Account_Number		--CR1072
	LEFT OUTER JOIN ServiceChargeAcct SCA ON SCA.AcctNo = A.AcctNo AND A.AcctType = 'C'	--#17083
	LEFT OUTER JOIN SR_Allocation AL  ON SCA.ServiceRequestNo = AL.ServiceRequestNo AND DateAllocated <> '1900-01-01'
	LEFT OUTER JOIN SR_Technician ST  ON AL.TechnicianId = ST.TechnicianId
	LEFT OUTER JOIN SCStatement sc on a.acctno=sc.acctno					-- #9919
	WHERE	CA.CustId = @CustId
  	AND		A.AcctNo = CA.AcctNo
	AND		AG.AcctNo = A.AcctNo
    AND      CA.hldorjnt='H'
	AND		AT.GenAcctType = A.AcctType
	AND		(A.currstatus != 'S' OR ABS(a.bdwbalance) >= 0.01 OR ABS(a.bdwcharges) >= 0.01 )
 	AND		NOT EXISTS (SELECT CN.AcctNo FROM Cancellation CN WHERE CN.AcctNo = A.AcctNo)
	   GROUP BY 			A.acctno ,		AT.accttype ,A.agrmttotal ,A.arrears ,	A.outstbal ,A.currstatus ,ISNULL(IP.instalamount,0) , ISNULL(IP.datefirst, '1/1/1900') ,
		A.bdwbalance,	A.bdwcharges,     A.Securitised,ISNULL(FOA.empeeno,0),ISNULL(U.FullName,'') ,			
		isnull(IRH.paymentholidaymin, IP.instalno) ,	A.dateacctopen, Segment_ID, Segment_Name, a.TallyManAcct,	--CR1072
		SCA.ServiceRequestNo, ISNULL(ST.Internal,''),
		SCA.acctno,sc.MinimumPayment		-- #9919
 

 --RM changes for work list when not allocated based on last action
 
	 select isnull(max(datetrans), a.dateacctopen) as paymentdate, a.acctno
	 into #paymentdate
            from acct a
            inner join #accts s on s.acctno = a.acctno
            left outer join fintrans f
                  on a.acctno = f.acctno 
            where isnull(f.transtypecode, 'PAY') = 'PAY'
            group by a.dateacctopen, a.acctno
	
	update a
	set empeeno = ISNULL(b.empeeno,0),
		EmployeeName = ISNULL(u.Fullname,'')
    from #accts a
    inner join #paymentdate p on p.acctno = a.acctno
	inner join bailaction b on b.acctno = a.acctno
	inner join Admin.[User] u ON u.id = b.empeeno
    where	 isnull(a.empeeno, 0) = 0
			and dateadded > paymentdate
            AND b.dateadded = (SELECT MAX(bc.dateadded)
							from bailaction bc
							where bc.acctno = b.acctno
							and bc.dateadded > paymentdate)
 
	select *, CASE WHEN bail.PermissionId IS NULL THEN 0 ELSE 1 END AS isBailiff, CASE WHEN telephone.PermissionId IS NULL THEN 0 ELSE 1 END AS isTelephoneCaller
	from #accts a
	LEFT OUTER JOIN Admin.UserPermissionsView bail ON bail.UserId = a.empeeno AND bail.UserId = 381
	LEFT OUTER JOIN Admin.UserPermissionsView telephone ON telephone.UserId = a.empeeno AND telephone.UserId = 380

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End

