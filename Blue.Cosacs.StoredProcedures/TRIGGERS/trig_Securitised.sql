
IF EXISTS(SELECT * FROM sysobjects WHERE type='TR'
	  	AND name = 'trig_Securitised')

DROP TRIGGER trig_Securitised
GO


CREATE TRIGGER trig_Securitised
ON dbo.acct
FOR update
AS
   	DECLARE	@acctno char(12), 
			@error varchar (128),
			@old_sec char(1),
			@new_sec char(1)

	SELECT	@new_sec = securitised,
			@acctno = acctno
	FROM   	inserted 
	     
	SELECT	@old_sec = securitised
	FROM   	deleted
	
	IF(@old_sec = 'Y' AND @new_sec != 'Y')
	BEGIN
		UPDATE	acct
		SET		securitised = 'Y'
		WHERE	acctno = @acctno

		SET @error = 'Account has been flagged for securitisation, cannot remove flag. ' + @acctno
	 	RAISERROR (@error, 16, 1)
	END
GO