
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'LoyaltySave'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE LoyaltySave
END
GO



CREATE PROCEDURE LoyaltySave
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : LoyaltySave.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ?
-- Date         : ?

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/07/11 ip  RI - System Integration
-- ================================================
@MemberNo VARCHAR(16),
@Custid VARCHAR(20),
@StartDate DATETIME,
@Enddate DATETIME,
@MemberType CHAR(1),
@StatusAcct INT,
@StatusVoucher INT,
@cancel CHAR(1),
@user INT,
@return INT OUTPUT
AS
BEGIN

DECLARE @outtext VARCHAR(200)

SET @Outtext = ''

IF EXISTS (SELECT * FROM Loyalty 
           WHERE MemberNo = @Memberno
           AND Custid != @custid)
BEGIN 
  SET @outtext = 'Membership linked to other customer. No changes saved.'
END

IF EXISTS (SELECT * FROM Loyalty 
           WHERE MemberNo = @Memberno
           AND statusacct IN (2,3))
BEGIN 
  SET @outtext = 'Membership account exists already. Please select another membership account'
END

IF EXISTS (SELECT * FROM Loyalty 
           WHERE MemberNo = @Memberno
           AND statusacct IN (2,3))
BEGIN 
  SET @outtext = 'Membership account exists already. Please select another membership account'
END


IF EXISTS (SELECT * FROM custacct
           INNER JOIN acct ON acct.acctno = custacct.acctno
           INNER JOIN lineitem ON lineitem.acctno = custacct.acctno
           INNER JOIN loyalty ON loyalty.custid = custacct.custid
           INNER JOIN stockinfo ON stockinfo.ID = lineitem.ItemID				--IP - 27/07/11 - RI
           WHERE lineitem.delqty < lineitem.quantity
           --AND itemno IN (SELECT code FROM code
           AND stockinfo.iupc IN (SELECT code FROM code							--IP - 27/07/11 - RI
                            WHERE category = 'HCI')
           AND acct.dateacctopen BETWEEN DATEADD(hour,-2,loyalty.startdate) AND loyalty.startdate -- Will occour before start of membership.
           and custacct.custid = @custid)
           AND (@statusacct = 2)
           AND (SELECT SUM(transvalue) FROM fintrans
                INNER JOIN Loyalty L ON fintrans.acctno = L.LoyaltyAcct
			    AND custid= @custid) > 0
BEGIN
	 SET @outtext = 'Please remove Home Club items from predelivery accounts before account payment or membership cancellation.'
END
           
IF EXISTS (SELECT * FROM custacct
           INNER JOIN acct ON acct.acctno = custacct.acctno
           INNER JOIN lineitem ON lineitem.acctno = custacct.acctno
           INNER JOIN loyalty ON loyalty.custid = custacct.custid
           INNER JOIN stockinfo ON stockinfo.ID = lineitem.ItemID				--IP - 27/07/11 - RI
           WHERE lineitem.delqty = lineitem.quantity
           --AND itemno IN (SELECT code FROM code
           AND stockinfo.iupc IN (SELECT code FROM code							--IP - 27/07/11 - RI
                            WHERE category = 'HCI'
							     AND reference = 0)
           AND acct.dateacctopen BETWEEN DATEADD(hour,-2,loyalty.startdate) AND loyalty.startdate -- Will occour before start of membership.
           and custacct.custid = @custid
           AND (SELECT COUNT(transvalue) FROM fintrans f
		       WHERE f.acctno = acct.acctno
		       AND transtypecode = 'PAY') = 0
           AND @statusacct = 2
           AND (SELECT SUM(transvalue) FROM fintrans f2
                INNER JOIN Loyalty L ON f2.acctno = L.LoyaltyAcct
			    AND custid= @custid) > 0)
		  
BEGIN
	 SET @outtext = 'You cannot cancel membership if unpaid homeclub items have been delivered.'
END
     

IF @Outtext = ''
BEGIN
	IF EXISTS (SELECT * FROM Loyalty 
			   WHERE MemberNo = @Memberno) -- Check if exists and if so update.
		BEGIN
		
			IF EXISTS (SELECT * FROM Loyalty
					   WHERE memberno = @memberno
				       AND statusacct IN (4,1)) AND @StatusAcct = 2 -- if unpaid or free cancelled delete item.
			BEGIN
					EXEC LoyaltyRemoveFee @custid = @custid, @user = @user 
			END
			
			UPDATE Loyalty
			SET StartDate = @StartDate,
				Enddate = CASE WHEN @StatusAcct = 2 THEN GETDATE() ELSE @Enddate END,
				MemberType = @MemberType,
				StatusAcct = @StatusAcct,
				StatusVoucher = @StatusVoucher,
				empeeno = @user
			WHERE MemberNo = @Memberno	
			
			
		END
		ELSE
		BEGIN
			INSERT INTO Loyalty 
			(
				MemberNo,
				Custid,
				StartDate,
				Enddate,
				MemberType,
				StatusAcct,
				StatusVoucher,
				empeeno
			) VALUES ( 
				/* MemberNo - CHAR(16) */ @MemberNo,
				/* Custid - VARCHAR(20) */ @Custid,
				/* StartDate - DATETIME */ @StartDate,
				/* Enddate - DATETIME */ @Enddate,
				/* MemberType - CHAR(1) */ @MemberType,
				/* MemStatus - INT */ @StatusAcct,
									  @StatusVoucher,
									  @user)				  
		END
						  
	IF EXISTS (SELECT * FROM LoyaltyRejections
			   WHERE custid = @custid
			   AND Rejections > 0 )
	BEGIN
		UPDATE LoyaltyRejections
		SET Rejections = 0
		WHERE Custid = @custid
	END

	IF (ASCII(@cancel) != 0) 	
	BEGIN
		INSERT INTO LoyaltyCancellation (
			MemberNo,
			ReasonCode
		) VALUES ( 
			/* MemberNo - char(16) */ @MemberNo,
			/* ReasonCode - char(1) */ @cancel ) 
			
	END
END

SET @RETURN = 0

SELECT @outtext


END

GO

	