SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetDetailsSP]
GO

-- EXEC DN_FintransGetDetailsSP '720500013510',0
--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_FintransGetDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Financial Transactions for Account Details Screen
-- Author       : 
-- Date         : 
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/12/02  RD  Added description for Payment Method to be displayed under 
--		 Transaction Tab
-- 14/02/03  AA  loading up if paymethod is null
-- 31/08/05  RD  Modified to display chequno for BDW Recovery account
-- 27/04/06  JEC Return transactions in ascending date order  (68158)
-- 09/11/07  JH  Service Request number and labour or parts to be returned for the internal account
-- 03/02/10  IP  CR1072 - 3.1.8 - Return and display the StaffType on Account Details - Transactions tab.
-- 17/02/10  jec CR1072 - Malaysia Merge LW69792.
-- 16/02/12  IP  #9632 - CR1234 - Transfer Reference to be displayed in Account Details.
-- 14/08/12  jec Use new Admin table
-- 22/01/14  IP  #17083 - Internal Service Request charges now go to Oracle. Commented out code.
-- 11/08/14  IP  #17824 - Duplicate transactions shown in Account Details - Transaction tab
--------------------------------------------------------------------------------


CREATE PROCEDURE  dbo.DN_FintransGetDetailsSP
   			@acctno varchar(50),
   			@return int OUTPUT
 
AS

    	DECLARE @Custid VARCHAR(20)

    	SELECT @CustId = custid FROM CustAcct WHERE acctno = @acctno AND hldorjnt = 'H'

	--IP - 03/02/10 - CR1072 - 3.1.8 - Added stafftype
    DECLARE @AcctTrans TABLE(acctno CHAR(12),transrefno INT,datetrans DATETIME,transtypecode VARCHAR(3),transvalue MONEY,PayMethod VARCHAR(64),EmployeeName VARCHAR(101), --stafftype VARCHAR(64), 
			empeeno INT,ftnotes VARCHAR(16),transprinted CHAR(1),reference VARCHAR(20),chequeno VARCHAR(16),transferref varchar(60)) --IP - 16/02/12 - #9632 - CR1234
 	SET  @return = 0   --initialise return code

	/*	get a list of all gift vouchers which have been redeemed against this account so that
		they can be matched to the transaction records in the subsequent query */
	SELECT	transrefnoredeemed,
			reference 
	INTO	#temp
	FROM	GiftVoucherRedeemed
	WHERE	acctnoredeemed = @acctno


    INSERT INTO @AcctTrans
	SELECT 		A.acctno,
			A.transrefno,
			A.datetrans,
			A.transtypecode,
			round(A.transvalue,2) as transvalue,
			isnull(C.codedescript,'Not Known')  as 'PayMethod',
			--B.empeename,
			--ISNULL(c1.codedescript, 'Not Known') AS 'stafftype', --IP - 03/02/10 - CR1072 - 3.1.8
			U.FullName AS EmployeeName,
			--'' AS 'stafftype',
			A.empeeno,
			A.ftnotes,
            A.transprinted,
			isnull(T.reference, '') as reference,
			A.chequeno,
			CASE WHEN a.transtypecode in ('SHO', 'OVE') then convert(varchar,isnull(cb.cashiertotalid,''))	--IP - 16/02/12 - #9632 - CR1234
				 ELSE isnull(fx.acctname,'') END	 
	
	FROM fintrans A
	--INNER JOIN agreement AG ON A.acctno = AG.acctno
	LEFT OUTER JOIN agreement AG ON A.acctno = AG.acctno		-- CR1072 jec
	LEFT OUTER JOIN Code C ON C.code = isnull(convert(varchar,A.PayMethod),'')
	AND	C.category = 'FPM'
	LEFT OUTER JOIN #temp T ON A.transrefno = T.transrefnoredeemed
	LEFT OUTER JOIN Admin.[User] u ON A.empeeno = u.Id 
	--LEFT OUTER JOIN courtsperson B ON A.empeeno =  B.empeeno
	--LEFT OUTER JOIN code c1 ON b.empeetype = c1.code --IP - 03/02/10 - CR1072 - 3.1.8
	--AND c1.category = 'ET1' --IP - 03/02/10 - CR1072 - 3.1.8
	LEFT OUTER JOIN CashierTotalsBreakDown CB on A.id = CB.FintransId							--IP - 16/02/12 - #9632 - CR1234
	LEFT OUTER JOIN FinXfr FX on A.acctno = FX.acctno and A.transrefno = FX.transrefno			--IP - 16/02/12 - #9632 - CR1234
					and a.chequeno = fx.acctnoxfr												-- #17824
	WHERE A.acctno = @acctno
	--AND		(convert(varchar,A.PayMethod) = C.code or A.Paymethod is null)
    --AND	C.category = 'FPM'
    AND ag.agrmtno = (SELECT MAX(g.agrmtno) FROM agreement g WHERE g.acctno = ag.acctno )
    Order by A.datetrans    -- 68158 jec
	

	IF @CustId like 'BDW%'
  	BEGIN
		UPDATE	@AcctTrans
		SET	ChequeNo = fa.LinkedAcctno
		FROM    FintransAccount fa INNER JOIN @AcctTrans AT ON AT.acctno = fa.acctno AND AT.transrefno = fa.transrefno AND AT.datetrans = fa.datetrans
		WHERE  	AT.acctno = @acctno
		AND	AT.transtypecode in ('DPY', 'RET')
	END

	--#17083
--    IF @acctno = (SELECT LTRIM(RTRIM(Value)) FROM CountryMaintenance WHERE CodeName = 'ServiceInternal')
--BEGIN
--	UPDATE	@AcctTrans
--    SET ftnotes = CONVERT(VARCHAR(16),sr.ServiceBranchNo) + CONVERT(VARCHAR(16),sc.ServiceRequestNo),reference = delivery.itemno
--    FROM SR_ChargeAcct sc INNER JOIN @AcctTrans AT ON sc.AcctNo = AT.AcctNo 
--    INNER JOIN SR_ChargeTo sct ON sc.ServiceRequestNo = sct.ServiceRequestNo AND sct.Internal = at.transvalue
--    INNER JOIN SR_ServiceRequest sr ON sr.ServiceRequestNo = sc.ServiceRequestNo
--    INNER JOIN delivery ON delivery.acctno = AT.acctno AND delivery.transrefno = AT.transrefno AND delivery.branchno = sr.ServiceBranchNo AND delivery.datedel = AT.datetrans
--END

	-- Load Financial Transactions    
    	SELECT * FROM   @AcctTrans

	SELECT 	sum(transvalue) as 'Total Admin Fees'
	FROM 	fintrans
	WHERE 	acctno = @acctno
	AND 	transtypecode = 'ADM'

	SELECT 	sum(transvalue) as 'Total Interest'
	FROM 	fintrans
	WHERE 	acctno = @acctno
	AND 	transtypecode = 'INT'

 
 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End