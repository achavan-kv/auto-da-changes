IF NOT OBJECT_ID('Communication.GetSmsNotSent') IS NULL
	DROP PROCEDURE Communication.GetSmsNotSent
GO

CREATE PROCEDURE Communication.GetSmsNotSent
	@ExportedOn DateTime
AS
	IF OBJECT_ID('tempdb..#SmsNotSent','U') IS NOT NULL 
		DROP TABLE #SmsNotSent

	SELECT 
		PhoneNumber, Body, CustomerId
	INTO #SmsNotSent
	FROM 
		Communication.SmsToSend s
	WHERE
		ExportedOn IS NULL

	UPDATE Communication.SmsToSend
	SET ExportedOn = @ExportedOn
	WHERE ExportedOn IS NULL

	SELECT 
		PhoneNumber, Body, CustomerId
	FROM 
		#SmsNotSent

	DROP TABLE #SmsNotSent