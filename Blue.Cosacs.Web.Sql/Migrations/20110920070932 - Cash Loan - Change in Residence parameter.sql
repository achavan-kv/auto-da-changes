-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmcat INT

select @parmcat=code from code where category='CMC' and codedescript='Cash Loans' 

if not exists (select * from CountryMaintenance where codename='CL_AddressMonths')
Begin
insert into dbo.CountryMaintenance (
	CountryCode,
	ParameterCategory,
	[Name],
	Value,
	[Type],
	[Precision],
	OptionCategory,
	OptionListName,
	Description,
	CodeName
) select 
	/* CountryCode - char(1) */ CountryCode,
	/* ParameterCategory - varchar(4) */ @parmcat,
	/* Name - varchar(50) */ 'Months since Address changed',
	/* Value - varchar(200) */ '12',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If ''Changes in Residence require manual approval'' parameter is true, Customers whose address has changed within the last X months AND has changed since they last had an account DA''d will not qualify',
	/* CodeName - varchar(30) */ 'CL_AddressMonths'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_EmployMonths')
Begin
insert into dbo.CountryMaintenance (
	CountryCode,
	ParameterCategory,
	[Name],
	Value,
	[Type],
	[Precision],
	OptionCategory,
	OptionListName,
	Description,
	CodeName
) select 
	/* CountryCode - char(1) */ CountryCode,
	/* ParameterCategory - varchar(4) */ @parmcat,
	/* Name - varchar(50) */ 'Months since Employment changed',
	/* Value - varchar(200) */ '12',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If ''Changes in Employment require manual approval'' parameter is true, Customers whose employment has changed within the last X months AND has changed since they last had an account DA''d will not qualify',
	/* CodeName - varchar(30) */ 'CL_EmployMonths'
	From Country 
End