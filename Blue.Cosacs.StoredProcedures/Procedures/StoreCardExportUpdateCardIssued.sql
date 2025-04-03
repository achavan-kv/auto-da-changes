SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[StoreCardExportUpdateCardIssued]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[StoreCardExportUpdateCardIssued]
GO

CREATE PROCEDURE StoreCardExportUpdateCardIssued
-- **********************************************************************
-- Title: StoreCardExportUpdateCardIssued.sql
-- Developer: Ilyas Parker
-- Date: 6/01/2011
-- Purpose: Called from StoreCardExport End Of Day. Updates the CardIssued 
--			column when exported to csv.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 6/01/11   IP  Created
-- 13/04/12  IP/JC #9908 - StoreCardPaymentDetails status not updated correctly	
-- 19/04/12  IP  #9924 - StatusCode 'A' was inserted twice for a card. Do not insert the StatusCode if the last one inserted
--				 is the same as the current StatusCode being inserted.
-- **********************************************************************
 @runNo int
AS 
    DECLARE @rundate DATETIME , @cardstatus VARCHAR(2)
    SELECT @rundate =MAX(DateStart) FROM interfacecontrol WHERE interface = 'STCARDEXPORT'
    IF EXISTS( SELECT * FROM CountryMaintenance WHERE codeNAME LIKE 'StoreCardActivate' AND value = 'True')
		SET @cardstatus = 'A' -- activate
	ELSE 
		SET @cardstatus = 'AA' -- awaiting activation
		

	DECLARE @branchno SMALLINT
	SELECT @branchno= hobranchno FROM country 
	INSERT INTO StoreCardStatus (
		CardNumber,
		DateChanged,
		StatusCode,
		EmpeeNo,
		BranchNo
	) 
	SELECT 	CardNumber,
		@rundate,
		@cardstatus, 
		-116,
		@BranchNo
	 FROM  View_StoreCardWithPayments V  
	INNER JOIN custacct ca on V.acctno = ca.acctno
		INNER JOIN customer c on ca.custid = c.custid
		INNER JOIN custaddress cad on c.custid = cad.custid
	WHERE ca.hldorjnt = 'H'
	AND V.ExportRunNo IS NULL
	AND c.CreditBlocked = 0
	AND cad.addtype = 'H'
	AND cad.datemoved is null
	AND c.StoreCardApproved = 1		
	AND v.Cancelled=0 
	AND NOT EXISTS (select * from StoreCardStatus s													--IP - 19/04/12 - #9924 
						where s.cardnumber = v.cardnumber
						and s.datechanged = (select max(s1.Datechanged) from StoreCardStatus s1
												where s1.cardnumber = s.cardnumber)
					    and s.statuscode = @cardstatus)	

    
	UPDATE StoreCard
	SET ExportRunNo = @runNo
	FROM StoreCard s
		INNER JOIN custacct ca on s.acctno = ca.acctno
		INNER JOIN customer c on ca.custid = c.custid
		INNER JOIN custaddress cad on c.custid = cad.custid
	WHERE ca.hldorjnt = 'H'
	AND s.ExportRunNo IS NULL
	AND c.CreditBlocked = 0
	AND cad.addtype = 'H'
	AND cad.datemoved is null
	AND c.StoreCardApproved = 1
   
   update StoreCardPaymentDetails 
   SET Status = @cardstatus,LastUpdatedBy=-116					--IP/JC - 13/04/12 - 	#9908	
   WHERE  EXISTS (SELECT * FROM StoreCard s 
				  WHERE s.AcctNo = storecardpaymentdetails.acctno AND ExportRunNo = @runno)
   AND  status = 'TBI'

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO 
