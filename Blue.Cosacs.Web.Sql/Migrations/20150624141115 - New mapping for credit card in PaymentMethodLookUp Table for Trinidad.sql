-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF  EXISTS (SELECT 1 FROM Country WHERE countrycode = 'T')
BEGIN
	 IF NOT EXISTS(SELECT 1 FROM  [dbo].[PaymentMethodLookUp] WHERE POSPayMethodId = 10) 
	 BEGIN
		 INSERT INTO [dbo].[PaymentMethodLookUp] (CountryCode, WinCosacsPayMethodId, POSPayMethodId)
		 VALUES ('T' ,3,10 )
	 END
 END
GO


