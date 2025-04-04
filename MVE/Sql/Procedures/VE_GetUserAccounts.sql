if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_GetUserAccounts]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_GetUserAccounts]
END
GO

Create PROCEDURE [dbo].[VE_GetUserAccounts]
	  @CustId VARCHAR(20) = N''
	, @Message varchar(MAX) output
	, @Status varchar(5) output
AS
BEGIN	
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
	ELSE
	BEGIN
		SELECT 
		ISNULL(RFCreditLimit, 0.00) AS CreditLimit,
		ISNULL(cust.AvailableSpend, 0.00) AS CreditAvailable
		FROM
			dbo.customer cust
		WHERE
			cust.custid = @CustId
		SET @Message = 'Customer Credit Details found'
		SET @Status = '200'
	END
END