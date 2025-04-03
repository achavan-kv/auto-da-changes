SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountGetDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetDetailsSP]
GO

--CR906 rdb 12/09/07 changed proc to join  TermsTypeTable and select isLoan
--UAT 286 Include ChargeAcctNo field to determine whether or not the account is a Service charge-to account

CREATE PROCEDURE  [dbo].[DN_AccountGetDetailsSP]
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Returns Account details
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/02/10  jec CR1072 Malaysia merge 
-- 19/04/10  ip  UAT(58) - Display account type as 'HCC' for Home Club Account
-- 21/09/10  ip  UAT5.2 Log - UAT(1118) - For special accounts created for Cash & Go warranties
--				 the agreement number is > 1, but previously agreement number was hardcoded to pass in 1
--				 therefore set agreement number to actual agreement number.
-- 26/03/12  ip  #8842 - LW73943 - Merged from current
-- 22/01/14  ip  #17083 Service Request charge account table changed from SR_ChargeAcct to ServiceChargeAcct. Also changed
--				 reference from SR_ServiceRequest to use new table Service.Request
-- =============================================
   			@acctNo varchar(50),
   			@agrmtno int,
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code
 	
 	DECLARE @archived int
 	
 	SELECT	@archived = ISNULL(COUNT(*), 0)
 	FROM	acct_archive
 	WHERE	acctno = @acctNo
 	
 	SELECT @agrmtno =MIN(agrmtno) FROM agreement WHERE acctno = @acctNo --IP - 21/09/10 - UAT5.2 Log - UAT(1118)
 
	SELECT  DISTINCT
			A.AcctNo AS "Account Number",
			--D.AcctType  AS "Account Type",
			CASE WHEN l.LoyaltyAcct IS NOT NULL THEN 'HCC' 
			     WHEN SR.AcctNo IS NOT NULL THEN 'SRC' 
			     ELSE RTRIM(d.accttype) END AS "Account Type", --IP - 19/04/10 - UAT(58) UAT5.2 -- AA trimming
			A.DateAcctOpen AS "Account Opened",
			A.CreditDays AS "Credit Days",
			A.AgrmtTotal AS "Agreement Total",
			A.DateLastPaid AS "Date Last Paid",
			A.AS400Bal AS "AS400 Balance",
			A.OutstBal AS "Outstanding Balance",
			A.Arrears AS "Arrears",
			A.HighstStatus AS "Highest Status",
			A.CurrStatus AS "Current Status",
			A.HiStatusDays AS "High Status Days",
			B.rfcreditlimit AS "creditlimit",
			ISNULL(B.StoreCardLimit,0) AS "StoreCardLimit",
			ISNULL(B.StoreCardAvailable,0) AS "StoreCardAvailable",
			B.CustId AS "Customer ID",
			B.OtherId AS "Other ID",
			B.BranchNoHdle AS "Branch Number Handle",
			A.BranchNo AS "Branch Number",
			Case when AvailableSpend >0 then AvailableSpend  else 0 end AS "AvailableSpend",
			B.Name AS "Last Name",
			B.FirstName AS "First Name",
			B.Alias AS "Alias",
			B.AddrSort AS "Address Sort",
			B.NameSort AS "Name Sort",
			B.DateBorn AS "DateBorn",
			B.Ethnicity AS "Ethnicity",
			B.title AS "Title",
			B.sex AS "Sex",
			C.HldorJnt AS "Holder or Joint",
			Ag.servicechg,
			Ag.datedel,
			A.bdwbalance,
			A.bdwcharges,
			@archived as archived,
			T.IsLoan,
			ISNULL(SR.AcctNo,'') AS ChargeAcctNo,  
			Segment_Name  ,													
			SSR.State,		--#17083															--#8842 - LW73943 - Merged from current
			SSR.Id as ServiceRequestNo,		--#19773	--#17083														    --#8842 - LW73943 - Merged from current
			a.termstype,						-- 15186 
			A.isAmortized       -- Amortized CL
	FROM
			ACCT A --, CUSTOMER B, CUSTACCT C, ACCTTYPE D, agreement Ag
			INNER JOIN	ACCTTYPE D 
			ON		A.accttype = D.genaccttype
			INNER JOIN	CUSTACCT C
			ON		A.acctno = C.acctno
			INNER JOIN	CUSTOMER B
			ON		B.custid = C.custid
			LEFT JOIN	agreement Ag
			ON		Ag.acctno = A.acctno
			AND		Ag.agrmtno = @agrmtno
			LEFT OUTER JOIN TermsTypeTable T
			ON A.TermsType = T.TermsType
			left outer join TM_Segments s on a.acctno = s.account_number	--CR1072
			LEFT OUTER JOIN loyalty l	--IP - 19/04/10 - UAT(58) UAT5.2
			ON a.acctno = l.LoyaltyAcct
			LEFT OUTER JOIN ServiceChargeAcct SR ON a.acctno = sr.AcctNo  --#17083
			LEFT OUTER JOIN Service.Request SSR ON SSR.Id = SR.servicerequestno --#17083	--#8842 - LW73943 - Merged from current
	WHERE 	A.acctno = @acctNo 

 
 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END
GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End


