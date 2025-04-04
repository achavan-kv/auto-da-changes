if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_SecretTokenDetails]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_SecretTokenDetails]
END
GO

Create PROCEDURE [dbo].[VE_SecretTokenDetails]
	@Token NVARCHAR(MAX) = NULL
	,@Expiration DATETIME = NULL
AS
BEGIN
	IF(ISNULL(@Token, '' ) = '' AND ISNULL(@Expiration, '') = '')
	BEGIN
		SELECT TOP 1 token FROM dbo.VE_SecretToken WHERE expiration > DATEADD(HOUR,1,GETUTCDATE()) ORDER BY id DESC
	END
	ELSE
	BEGIN
		INSERT INTO
			dbo.VE_SecretToken
			(
				token
				,expiration
			)
		VALUES
			(
				@Token
				,@Expiration
			)
	END
END