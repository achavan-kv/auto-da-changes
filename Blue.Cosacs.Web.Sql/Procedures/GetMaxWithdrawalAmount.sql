

/****** Object:  StoredProcedure [dbo].[GetMaxWithdrawalAmount]    Script Date: 11/19/2018 2:01:54 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetMaxWithdrawalAmount'
   )
BEGIN
DROP PROCEDURE [dbo].[GetMaxWithdrawalAmount]
END
GO

/****** Object:  StoredProcedure [dbo].[GetMaxWithdrawalAmount]    Script Date: 11/19/2018 2:01:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*

--*********************************************************************** 
-- Script Name : GetMaxWithdrawalAmount.sql 
-- Created For  : Unipay (T) 
-- Created By   : Sagar Kute
-- Created On   : 13/07/2018 
--*********************************************************************** 
-- Change Control 
-- -------------- 
-- Date(DD/MM/YYYY)		Changed By(FName LName)		Description 
-- ------------------------------------------------------------------------------------------------------- 

--*********************************************************************************************************
DECLARE @Message varchar(MAX)
EXEC GetMaxWithdrawalAmount @CustId = '891216-0093', @Message = @Message OUT
SELECT @Message
*/
CREATE PROCEDURE [dbo].[GetMaxWithdrawalAmount]
	@CustId VARCHAR(20) = N''
	, @Message varchar(MAX) output
	, @Status varchar(5) output
AS
BEGIN
	--
	SET @Message = ''
	SET @Status = ''
	IF NOT EXISTS (select 1 from Customer where Custid=@CustId)
	BEGIN
		SET @Message = 'User not found'
		SET @Status = '404'
		RETURN		
	END
	ELSE IF NOT EXISTS (select 1 from custacct where custid=@CustId)
	BEGIN
		SET @Message = 'No accounts for user'
		SET @Status = '404'
		RETURN		
	END
	--ELSE IF EXISTS (SELECT 1 FROM customer cust INNER JOIN custacct ca ON ca.CustId = cust.CustId WHERE RFCreditLimit = 0.00 AND cust.CustId=@CustId)
	--BEGIN
	--	SET @Message = 'No transactions for user'
	--	RETURN		
	--END
	ELSE
	BEGIN
		SELECT 
			--ISNULL(ROUND(cust.AvailableSpend, 2), 0.00) AS CreditAvailable
			ISNULL(cust.AvailableSpend, 0.00) AS CreditAvailable
		FROM
			dbo.customer cust
		WHERE
			cust.custid = @CustId
		SET @Message = 'Max available credit limit found'
		SET @Status = '200'
	END
END



GO

