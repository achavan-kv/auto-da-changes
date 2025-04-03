-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Declare @ParmCat varchar(30)

select @ParmCat = code from code c where category='CMC' and codedescript='Instant Replacement'


if not exists (select * from CountryMaintenance where codename='MinFreeMonthIR')
begin 
	insert into CountryMaintenance (CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	Select c.countrycode,@ParmCat,'Min Free Months on Replacements (%)',50,'numeric',0,'','','The minimum number of months (as a percentage of the original item’s free/manufacturer warranty length) that a replacement item on an IR replacement can receive','MinFreeMonthIR'
	from country c
	
end

if not exists (select * from CountryMaintenance where codename='DelayNewIRW')
begin 
	insert into CountryMaintenance (CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	Select c.countrycode,@ParmCat,'Delay New IRW (True / False)','False','checkbox',0,'','','If True, when an IRW replacement item receives complimentary free/manufacturer warranty months, then the start date of any purchased IRW on the replacement item is delayed until the end of the free months','DelayNewIRW'
	from country c	
end

if not exists (select * from CountryMaintenance where codename='CreditMinPrice')
begin 
	insert into CountryMaintenance (CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	Select c.countrycode,@ParmCat,'Minimum price for credit terms',0,'numeric',0,'','','The minimum price for which an account can be sold on credit terms','CreditMinPrice'
	from country c

end

