
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM dbo.SYSOBJECTS 
WHERE id = object_id('[dbo].[StoreCardSelectCardsToExport]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[StoreCardSelectCardsToExport]
GO

CREATE PROCEDURE StoreCardSelectCardsToExport
-- **********************************************************************
-- Title: StoreCardSelectCardsToExport.sql
-- Developer: Ilyas Parker
-- Date: 6/01/2011
-- Purpose: Called from StoreCardExport End Of Day. Select Store Cards
--			that will be exported to a csv file.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 6/01/11   IP  Created
-- 12/12/11  IP  #8864 - UAT67 - Remove Title from Cardholder name
-- 20/12/11  jec #8864 Re-instate abve & remove title from Card Name
-- 13/04/12  IP  #9903 - Use Status from StoreCardPaymentDetaills 
-- 22/05/13  IP/JC #12584 - Cancellation of Store Card - Duplicate Card Issued
-- 28/05/13  IP #13460 - Cancellation of Storecard - Duplicate Card Issued - Exclude Cancelled Cards
-- 10/09/13  IP #14230 - Additional card not selected for export
-- **********************************************************************

AS

	DECLARE @runno INT ,@expirymonths SMALLINT
	SELECT @runno= MAX( i.runno )
	FROM  interfacecontrol i
	WHERE interface = 'STCARDEXPORT'
	
	SELECT @expirymonths =CONVERT(smallint,VALUE ) FROM CountryMaintenance WHERE codename  = 'SCardPreExpiryMonths'
	
	IF @expirymonths = 0
		SET @expirymonths =120 -- ten years

	SELECT c.custid as CustID, 
		   c.title as Title,					-- #8864 jec 20/12/11 re-instate
		   c.firstname as FirstName,
		   c.name as LastName,
		   cad.cusaddr1 as CusAddr1,
		   cad.cusaddr2 as CusAddr2,
		   cad.cusaddr3 as CusAddr3,
		   cad.cuspocode as CusPocode,
		   branchname AS Branch,
		   branchaddr1 AS BranchAddress1,
		   branchaddr2 AS BranchAddress2,
		   branchaddr3 AS BranchAddress3 ,
		   c.StoreCardLimit, 
		   s.CardNumber,							--#12584
		   REPLACE(s.CardName,c.title,'') as CardName,	-- #8864 jec 20/12/11
		   s.IssueYear,
		   s.IssueMonth,
		   s.ExpirationYear, 
		   s.ExpirationMonth, 
		   s.AcctNo,
		   @runno as ExportRunNo,
		   DATEADD(MONTH,@expirymonths ,C.ScardApprovedDate) AS OfferExpiryDate,
		   s.Source
	FROM StoreCard s
	INNER JOIN storecardpaymentdetails sp on s.acctno = sp.acctno							--IP - 13/04/12 - #9903
	INNER JOIN custacct ca on s.acctno = ca.acctno
	INNER JOIN customer c on c.custid = ca.custid
	INNER JOIN custaddress cad on cad.custid = c.custid
	INNER JOIN branch b ON b.branchno= LEFT(CONVERT(VARCHAR,s.acctno),3)
	INNER JOIN customer sc on s.custid=sc.custid			-- #8864 jec 20/12/11
    --INNER JOIN StoreCard MainStoreCard ON ca.custid = MainStoreCard.CustID				--#12584
	JOIN StoreCardStatus ss ON ss.CardNumber = s.CardNumber
	WHERE ca.hldorjnt = 'H'
	AND ss.DateChanged = (SELECT MAX(sc.DateChanged) FROM StoreCardStatus sc WHERE 
	sc.CardNumber= ss.CardNumber)
	AND s.ExportRunNo IS NULL
	--AND ss.StatusCode='TBI'
	AND ss.StatusCode != 'C'				--#13460
	AND (sp.[Status] = 'TBI' OR SS.StatusCode = 'TBI') --#14230	--IP - 13/04/12 - #9903
	AND c.CreditBlocked = 0
	AND cad.addtype = 'H'
	AND cad.datemoved is null
	AND c.StoreCardApproved = 1
   

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
