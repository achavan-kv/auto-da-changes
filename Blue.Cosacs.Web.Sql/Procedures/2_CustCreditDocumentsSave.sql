
IF OBJECT_ID('[dbo].[CustCreditDocumentsSave]') IS NOT NULL
	DROP PROCEDURE [dbo].[CustCreditDocumentsSave]
GO

CREATE PROCEDURE [dbo].[CustCreditDocumentsSave]
	 @CustId			NVARCHAR(30)
	,@FolderPath		NVARCHAR(300)
	,@FileName			NVARCHAR(50)
	,@AccountNumber		NVARCHAR(50)
	,@IsThirdParty		BIT = 0
	,@Message VARCHAR(MAX) OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	
	SET @Message = ''
	IF(@CustId <> '' AND @CustId IS NOT NULL)
	BEGIN
		IF(NOT EXISTS(SELECT 1 FROM [dbo].[customer] cust WHERE (LTRIM(RTRIM(cust.CustId)) = LTRIM(RTRIM(@CustId)))))
		BEGIN
			SET @Message = 'No user found';
		END
		ELSE IF NOT EXISTS (select 1 from custacct where custid=@CustId AND acctno=@AccountNumber)
		BEGIN
			SET @Message = 'No accounts for user'
			RETURN		
		END
		ELSE
		BEGIN	
			INSERT INTO [dbo].[CustCreditDocuments]
			   (
					[CustId]
				   ,[FolderPath]
				   ,[FileName]
				   ,[AccountNumber]
				   ,[CreatedOn]
			   )
			VALUES
			   (
					@CustId,
					@FolderPath,
					@FileName,
					@AccountNumber,
					GETDATE()
			   )
		
			--IF(LTRIM(RTRIM(@Message)) = '')
			SET @Message = 'File uploaded successfully.';
			
			IF(@IsThirdParty = 0)
				exec InsertSignatureStatus @CustId, @AccountNumber
			--ELSE
			--	SET @Message += 'File uploaded successfully.';
		END
	END
END
