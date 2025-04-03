-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DECLARE @SettingsNamespace VARCHAR(30)='Blue.Cosacs.Payments'
DECLARE @PaymentCardTypeID VARCHAR(30)='PaymentCardType'

IF NOT EXISTS(select * from Config.Setting where id = @PaymentCardTypeID)
BEGIN
    DECLARE @PaymentCardValuetext VARCHAR(256) = 
    (
	    SELECT STUFF((
            SELECT DISTINCT '' + codedescript + CHAR(10) 
            FROM code 
            WHERE category = 'CCT' AND
            codedescript <> ''
             FOR XML PATH('')), 1, 0, '')
    )

	INSERT INTO Config.Setting
		([NameSpace], id, ValueText)
	VALUES
		(
            @SettingsNamespace,
            @PaymentCardTypeID,
            CAST(@PaymentCardValuetext AS NTEXT)
         )
END


