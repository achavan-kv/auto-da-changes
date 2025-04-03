
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'EMA_UpdateContractNotificationStatus')
BEGIN
DROP PROCEDURE [dbo].[EMA_UpdateContractNotificationStatus]
END
GO


CREATE PROCEDURE [dbo].[EMA_UpdateContractNotificationStatus]
	 @ContractNotificationStatus XML 
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT	     
AS  

BEGIN  

	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0	

	DECLARE 
	@Mode VARCHAR(10),
	@Flag int=0,

	@AccountNo VARCHAR(MAX) = N'',
	@CustomerId VARCHAR(MAX) = N'',
	@Status VARCHAR(MAX) = N''
	
	IF( @Flag = 0)
	BEGIN TRANSACTION	

	IF OBJECT_ID('tempdb..#temp') IS NOT NULL
	DROP TABLE #temp

	SELECT 
		  t.c.value('AccountNumber[1]' ,'[varchar](20)') AS AccountNo
		, t.c.value('Custid[1]' ,'[varchar](20)') AS CustomerId
		, t.c.value('status[1]' ,'[varchar](MAX)') AS Status
	INTO #temp
	FROM @ContractNotificationStatus.nodes('/ContractNotificationStatus/MailNotificationstatus/MailNotificationstatusList') T(c)
	
	IF(@Flag=0)  
	BEGIN
		UPDATE CustTPContract SET UpdateDate=getdate(),
		isTPContractSend=(	SELECT CASE WHEN Status='true' THEN '1'
								 WHEN Status='false' THEN null END)
		FROM #temp T0 INNER JOIN CustTPContract T1 ON T0.CustomerId=T1.custId AND T0.AccountNo=T1.acctNo	
	END

	print @@ERROR
	IF (@@ERROR != 0)
		BEGIN
			ROLLBACK
			print'Transaction rolled back'
			SET @StatusCode = 404;		
			SET @Message='Unable to update Email status';
		END
	ELSE
		BEGIN	

			print CONVERT(VARCHAR(3), @StatusCode)
			COMMIT
			SET @StatusCode = 200;
			print'Transaction committed'
			SET @Message='Email Notification updated successfully';
			PRINT @Message
		END
	PRINT @Message
END
GO

