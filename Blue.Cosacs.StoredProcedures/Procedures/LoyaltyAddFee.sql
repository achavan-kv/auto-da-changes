
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltyAddFee'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltyAddFee
END
GO


CREATE PROCEDURE [dbo].[LoyaltyAddFee]
-- ======================================================      
-- Project      : CoSACS .NET      
-- File Name    : LoyaltyAddFee.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        :   
-- Author       : ??      
-- Date         : ??        
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 01/08/11  IP  RI System Integration
-- ======================================================
@acctno VARCHAR(12),
@membertype CHAR(1),
@custid VARCHAR(20),
@user INT,
@return INT OUTPUT 
AS 
BEGIN

DECLARE @itemtoadd VARCHAR(20),
		@branchno INT,
		@unitprice MONEY,
		@free bit,
		@itemID INTEGER																	--IP - 01/08/11 - RI

BEGIN TRAN

SET @branchno = (SELECT SUBSTRING(@acctno,1,3))


IF ((SELECT value FROM CountryMaintenance
WHERE codename = 'LoyaltyMembershipFee') = 'FALSE')
BEGIN
	IF EXISTS (SELECT * FROM code 
		WHERE category = 'HCM'
		AND reference = '1'
		AND code = @membertype)
		 BEGIN
	 			SELECT  @itemtoadd = code 
	 			FROM code 
	 			WHERE category = 'HCI'
	 			AND codedescript = 'Free member item'
	 			
	 			SET @itemID = isnull((select ID from StockInfo si where IUPC = @itemtoadd and repossesseditem = 0),0)	--IP - 01/08/11 - RI
	 			
	 			SET @free = 0
		 END 
	 ELSE
		 BEGIN
 				SELECT  @itemtoadd = code 
	 			FROM code 
	 			WHERE category = 'HCI'
	 			AND codedescript = 'HomeClub card fee item'
	 			
	 			SET @itemID = isnull((select ID from StockInfo si where IUPC = @itemtoadd and repossesseditem = 0),0)	--IP - 01/08/11 - RI
	 			
	 			SET @free = 1
		 END
	END
ELSE
BEGIN
	 SELECT  @itemtoadd = code 
	 			FROM code 
	 			WHERE category = 'HCI'
	 			AND codedescript = 'HomeClub card fee item' 
	 			
	 SET @itemID = isnull((select ID from StockInfo si where IUPC = @itemtoadd and repossesseditem = 0),0)	--IP - 01/08/11 - RI
END

	 
	 
	 SET @unitprice = (SELECT unitpricecash FROM stockitem
					--WHERE itemno = @itemtoadd
					WHERE ItemID = @itemID																	--IP - 01/08/11 - RI
					AND stocklocn = @branchno)
	 
IF (@unitprice IS NULL OR @itemtoadd IS NULL)	 
BEGIN
	RAISERROR('Loyalty club items are not setup on this database',16,1)
END

IF NOT EXISTS (SELECT * FROM custacct
               WHERE custid = @custid 
               AND acctno = @acctno)
BEGIN
	INSERT INTO custacct (
		origbr,
		custid,
		acctno,
		hldorjnt
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* custid - varchar(20) */ @custid ,
		/* acctno - char(12) */ @acctno,
		/* hldorjnt - char(1) */ 'H' ) 
		
INSERT INTO acct (
	origbr,
	acctno,
	accttype,
	dateacctopen,
	creditdays,
	agrmttotal,
	datelastpaid,
	as400bal,
	outstbal,
	arrears,
	highststatus,
	currstatus,
	histatusdays,
	branchno,
	paidpcent,
	termstype,
	repossarrears,
	repossvalue,
	dateintoarrears,
	lastupdatedby,
	Securitised,
	bdwbalance,
	bdwcharges,
	hasstocklineitems
) VALUES ( 
	/* origbr - smallint */ 0,
	/* acctno - char(12) */ @acctno,
	/* accttype - char(1) */ 'C',
	/* dateacctopen - datetime */ GETDATE(),
	/* creditdays - smallint */ 0,
	/* agrmttotal - money */ 0,
	/* datelastpaid - datetime */ '1900-01-01',
	/* as400bal - money */ 0,
	/* outstbal - money */ 0,
	/* arrears - money */ 0,
	/* highststatus - char(1) */ '1',
	/* currstatus - char(1) */ '1',
	/* histatusdays - smallint */ 0,
	/* branchno - smallint */ @branchno,
	/* paidpcent - smallint */ 0,
	/* termstype - nvarchar(2) */ N'',
	/* repossarrears - money */ 0,
	/* repossvalue - money */ 0,
	/* dateintoarrears - datetime */ '',
	/* lastupdatedby - int */ @user,
	/* Securitised - char(1) */ '',
	/* bdwbalance - money */ 0,
	/* bdwcharges - money */ 0,
	/* hasstocklineitems - bit */ 0 ) 
	
	INSERT INTO agreement (
		origbr,
		acctno,
		agrmtno,
		dateagrmt,
		empeenosale,
		datedepchqclr,
		holdmerch,
		holdprop,
		datedel,
		datenextdue,
		oldagrmtbal,
		cashprice,
		discount,
		pxallowed,
		servicechg,
		sdrychgtot,
		agrmttotal,
		deposit,
		codflag,
		soa,
		paymethod,
		unpaidflag,
		deliveryflag,
		fulldelflag,
		PaymentMethod,
		empeenoauth,
		dateauth,
		empeenochange,
		datechange,
		AdminFee,
		InsCharge,
		datefullydelivered,
		createdby,
		paymentcardline,
		paymentholidays,
		AgreementPrinted,
		TaxInvoicePrinted,
		WarrantyPrinted,
		source
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* acctno - char(12) */ @acctno,
		/* agrmtno - int */ 1,
		/* dateagrmt - datetime */ GETDATE(),
		/* empeenosale - int */ @user,
		/* datedepchqclr - datetime */ '1900-01-01',
		/* holdmerch - char(1) */ 'N',
		/* holdprop - char(1) */ 'Y', --
		/* datedel - datetime */ GETDATE(),
		/* datenextdue - datetime */ GETDATE(),
		/* oldagrmtbal - money */ 0,
		/* cashprice - money */ 0,
		/* discount - money */ 0,
		/* pxallowed - money */ 0,
		/* servicechg - money */ 0,
		/* sdrychgtot - money */ 0,
		/* agrmttotal - money */ 0,
		/* deposit - money */ 0,
		/* codflag - char(1) */ '',
		/* soa - varchar(4) */ '',
		/* paymethod - varchar(1) */ '',
		/* unpaidflag - varchar(1) */ 'y',
		/* deliveryflag - varchar(1) */ 'y',
		/* fulldelflag - varchar(1) */ 'y',
		/* PaymentMethod - char(1) */ '',
		/* empeenoauth - int */ CASE WHEN @free = 0 THEN @user ELSE 0 END,
		/* dateauth - smalldatetime */ CASE WHEN @free = 0 THEN GETDATE() ELSE '1900-01-01' END,
		/* empeenochange - int */ CASE WHEN @free = 0 THEN @user ELSE 0 END,
		/* datechange - smalldatetime */ '1900-01-01',
		/* AdminFee - money */ 0,
		/* InsCharge - money */ 0,
		/* datefullydelivered - datetime */ '1900-01-01',
		/* createdby - int */ @user,
		/* paymentcardline - smallint */ 0,
		/* paymentholidays - smallint */ 0,
		/* AgreementPrinted - char(1) */ 'Y',
		/* TaxInvoicePrinted - char(1) */ 'Y',
		/* WarrantyPrinted - char(1) */ 'Y',
		/* source - char(15) */ 'Loyalty' ) 
		
END
ELSE
BEGIN
	UPDATE agreement
	SET empeenoauth =  CASE WHEN @free = 0 THEN @user ELSE 0 END,
		dateauth =  CASE WHEN @free = 0 THEN GETDATE() ELSE '1900-01-01' END,
		empeenochange =  CASE WHEN @free = 0 THEN @user ELSE 0 END
	WHERE acctno = @acctno
END
	 

	UPDATE acct
	SET currstatus = 1
	WHERE acctno = @acctno
	AND currstatus = 'S'
	AND @unitprice > 0

 
IF NOT EXISTS (SELECT * FROM lineitem
		   WHERE acctno = @acctno
		   --AND itemno = @itemtoadd)
		   AND ItemID = @itemID)																--IP - 01/08/11 - RI
BEGIN
	INSERT INTO Lineitem
		(acctno,agrmtno,itemno,quantity,delqty,stocklocn,price,ordval,delnotebranch,qtydiff,
		--itemtype, taxamt,isKit,deliveryaddress, contractno)
		itemtype, taxamt,isKit,deliveryaddress, contractno, ItemID)								--IP - 01/08/11 - RI
		--SELECT @acctno, 1, @itemtoadd,1,1,@branchno,@unitprice,@unitprice,@branchno,'N',	
		SELECT @acctno, 1, '',1,1,@branchno,@unitprice,@unitprice,@branchno,'N',				--IP - 01/08/11 - RI
			   'N',0,0,'H','', @itemID															--IP - 01/08/11 - RI
	
END
ELSE
BEGIN
	UPDATE lineitem
	SET delqty = delqty +1,
		quantity = quantity + 1,
		ordval = @unitprice  * (quantity + 1)
	WHERE acctno = @acctno
	--AND itemno = @itemtoadd
	AND ItemID = @itemID																		--IP - 01/08/11 - RI
END

	
	INSERT INTO delivery
		(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn,
		quantity, retitemno, retstocklocn, retval, buffno, buffbranchno,
		datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker,
		--notifiedby, ftnotes)
		notifiedby, ftnotes, ItemID)															--IP - 01/08/11 - RI
	SELECT	@branchno,@acctno, 1, getdate(), 'D', @itemtoadd,@branchno, 
		1, '', '', '', hibuffno + 1,@branchno, 
		getdate(),@branchno,  hirefno + 1, @unitprice, 0, '', '',
		--@user, 'HC'
		@user, 'HC', @itemID																	--IP - 01/08/11 - RI
	FROM	Branch
	WHERE Branchno = @branchno
	
	UPDATE branch
	SET hibuffno = hibuffno + 1,
	    hirefno = hirefno +1
	WHERE branchno = @branchno
	
	UPDATE acct
	SET outstbal =  (SELECT SUM(transvalue) 
					 FROM fintrans 
					 WHERE fintrans.acctno = acct.acctno),
		agrmttotal = (SELECT SUM(ordval) FROM lineitem
		             WHERE lineitem.acctno = acct.acctno)
	WHERE acctno = @acctno
	
	UPDATE agreement
	SET agrmttotal = (SELECT SUM(ordval) FROM lineitem
		              WHERE lineitem.acctno = agreement.acctno)
	WHERE acctno = @acctno
	
	
	UPDATE loyalty 
	SET LoyaltyAcct = @acctno
	WHERE custid = @custid 
	
	UPDATE acct
	SET currstatus = 'S'
	WHERE acctno = @acctno
	AND (SELECT SUM(transvalue) FROM Fintrans
	     WHERE acctno = @acctno) = 0
		
		SET @return = 0
	
COMMIT 	
	
END

