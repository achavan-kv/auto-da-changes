-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmCat INT

select @parmCat = (select Code from Code where category='CMC' and Codedescript='Cashier')

If not exists (select * from CountryMaintenance where codename='MaxShortageDaily')
BEGIN
	Insert into dbo.CountryMaintenance (
		CountryCode,
		ParameterCategory,
		[Name],
		Value,
		[Type],
		[Precision],
		OptionCategory,
		OptionListName,
		Description,
		CodeName)
	Select CountryCode,
		/* ParameterCategory - varchar(4) */ @parmCat,
		/* Name - varchar(50) */ 'Max Shortage - Daily',
		/* Value - varchar(1500) */ '0',
		/* Type - varchar(10) */ 'numeric',
		/* Precision - int */ 2,
		/* OptionCategory - varchar(4) */ '',
		/* OptionListName - varchar(50) */ '',
		/* Description - varchar(1500) */ 'Maximum Daily shortage allowed per employee in Cashier Totals Reconcilliation',
		/* CodeName - varchar(30) */ 'MaxShortageDaily' 
	From Country 
END

If not exists (select * from CountryMaintenance where codename='MaxShortageWeekly')
BEGIN
	Insert into dbo.CountryMaintenance (
		CountryCode,
		ParameterCategory,
		[Name],
		Value,
		[Type],
		[Precision],
		OptionCategory,
		OptionListName,
		Description,
		CodeName)
	Select CountryCode,
		/* ParameterCategory - varchar(4) */ @parmCat,
		/* Name - varchar(50) */ 'Max Shortage - Weekly',
		/* Value - varchar(1500) */ '0',
		/* Type - varchar(10) */ 'numeric',
		/* Precision - int */ 2,
		/* OptionCategory - varchar(4) */ '',
		/* OptionListName - varchar(50) */ '',
		/* Description - varchar(1500) */ 'Maximum Weekly (rolling 7 days) shortage allowed per employee in Cashier Totals Reconcilliation',
		/* CodeName - varchar(30) */ 'MaxShortageWeekly'
	From Country   
END

If not exists (select * from CountryMaintenance where codename='MaxShortageMonthly')
BEGIN
	Insert into dbo.CountryMaintenance (
		CountryCode,
		ParameterCategory,
		[Name],
		Value,
		[Type],
		[Precision],
		OptionCategory,
		OptionListName,
		Description,
		CodeName)
	Select CountryCode,
		/* ParameterCategory - varchar(4) */ @parmCat,
		/* Name - varchar(50) */ 'Max Shortage - Monthly',
		/* Value - varchar(1500) */ '0',
		/* Type - varchar(10) */ 'numeric',
		/* Precision - int */ 2,
		/* OptionCategory - varchar(4) */ '',
		/* OptionListName - varchar(50) */ '',
		/* Description - varchar(1500) */ 'Maximum Monthly (rolling 30 days) shortage allowed per employee in Cashier Totals Reconcilliation',
		/* CodeName - varchar(30) */ 'MaxShortageMonthly'
	From Country   
END

If not exists (select * from CountryMaintenance where codename='MaxShortageYearly')
BEGIN
	Insert into dbo.CountryMaintenance (
		CountryCode,
		ParameterCategory,
		[Name],
		Value,
		[Type],
		[Precision],
		OptionCategory,
		OptionListName,
		Description,
		CodeName)
	Select CountryCode,
		/* ParameterCategory - varchar(4) */ @parmCat,
		/* Name - varchar(50) */ 'Max Shortage - Yearly',
		/* Value - varchar(1500) */ '0',
		/* Type - varchar(10) */ 'numeric',
		/* Precision - int */ 2,
		/* OptionCategory - varchar(4) */ '',
		/* OptionListName - varchar(50) */ '',
		/* Description - varchar(1500) */ 'Maximum Yearly (rolling 365 days) shortage allowed per employee in Cashier Totals Reconcilliation',
		/* CodeName - varchar(30) */ 'MaxShortageYearly' 
	From Country  
END
