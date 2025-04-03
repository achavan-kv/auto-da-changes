-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmcategory INT

select @parmcategory= code from code c where c.category='CMC' and codedescript='Interface'

insert into CountryMaintenance (CountryCode,ParameterCategory,[Name],Value,[Type],[Precision],
				OptionCategory,OptionListName,[Description],CodeName) 
select CountryCode,	@parmcategory,'Maintain Non Stock Items in Cosacs','False','checkbox',0,
				'','','If true, Non Stock items are maintained in CoSACS and are not imported via the Product File Import EOD process. If false, Non Stock items will be imported via the Product File Import EOD process.','MaintainNonStocks'
From country			
				 
