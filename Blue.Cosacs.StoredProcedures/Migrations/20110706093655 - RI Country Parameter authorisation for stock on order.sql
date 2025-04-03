-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from countrymaintenance where codename = 'StockOnOrderAuth')
BEGIN
	insert into countrymaintenance (CountryCode, 
									ParameterCategory, 
									Name, 
									Value, 
									Type, 
									Precision, 
									OptionCategory, 
									OptionListName,
									Description, 
									CodeName)
	select
		  countrycode, 
		 '13',
		 'Password for stock on order required',
		 'True',
		 'checkbox',
		  0,
		  '',
		  '',
		  'If checked prevent selling of stock on order products by unauthorised users.',
		  'StockOnOrderAuth'	  
	from country
		 
END

