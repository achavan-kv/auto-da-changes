-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF  EXISTS (SELECT 1 FROM Country WHERE countrycode = 'T')
BEGIN
 
 IF NOT EXISTS(SELECT 1 FROM  [dbo].[PaymentMethodLookUp] WHERE POSPayMethodId = 1) 
 BEGIN
	INSERT INTO [dbo].[PaymentMethodLookUp] (CountryCode, WinCosacsPayMethodId, POSPayMethodId)
	VALUES ('T' ,1,1 )
END

IF NOT EXISTS(SELECT 1 FROM  [dbo].[PaymentMethodLookUp] WHERE POSPayMethodId = 4) 
 BEGIN
	 INSERT INTO [dbo].[PaymentMethodLookUp] (CountryCode, WinCosacsPayMethodId, POSPayMethodId)
	 VALUES ('T' ,10,4 )
 END

 IF NOT EXISTS(SELECT 1 FROM  [dbo].[PaymentMethodLookUp] WHERE POSPayMethodId = 5) 
 BEGIN
	 INSERT INTO [dbo].[PaymentMethodLookUp] (CountryCode, WinCosacsPayMethodId, POSPayMethodId)
	 VALUES ('T' ,2,5 )
 END

 IF NOT EXISTS(SELECT 1 FROM  [dbo].[PaymentMethodLookUp] WHERE POSPayMethodId = 6) 
 BEGIN
	 INSERT INTO [dbo].[PaymentMethodLookUp] (CountryCode, WinCosacsPayMethodId, POSPayMethodId)
	 VALUES ('T' ,4,6 )
 END
 END
GO
