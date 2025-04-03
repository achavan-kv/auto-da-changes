
INSERT into CountryMaintenance	(CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
Values ((select top 1 Countrycode from country) ,'15','Enable InvoiceTotal in Guilder','False','checkbox',0,'','','If true, the tax invoicetotal will be displayed in guilder','InvoiceGuilder'); 
