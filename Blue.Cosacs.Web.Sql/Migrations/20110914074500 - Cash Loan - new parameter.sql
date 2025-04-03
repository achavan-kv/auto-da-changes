-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmcat INT

select @parmcat=code from code where category='CMC' and codedescript='Cash Loans' 

if not exists (select * from CountryMaintenance where codename='CL_MaxPctRFavail')
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
	/* Name - varchar(50) */ 'Maximum %RF Spend Limit Allocated to Cash Loans',
	/* Value - varchar(200) */ '0',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'This is the percentage of the available spend that can be allocated to cash Loans',
	/* CodeName - varchar(30) */ 'CL_MaxPctRFavail' 
	From Country
End

if not exists (select * from CountryMaintenance where codename='CL_MinLoanAmount')
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
	/* Name - varchar(50) */ 'Minimum Loan Amount',
	/* Value - varchar(200) */ '0',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'Minimum Loan amount available. This is a set value across all accounts and will not be based on the % of the remaining available credit.',
	/* CodeName - varchar(30) */ 'CL_MinLoanAmount'
	From Country 
End

UPDATE CountryMaintenance 
	set Description='Maximum Loan amount available - no loan is available with default 0. This is a set value across all accounts and will not be based on the % of the remaining available credit.'
where codename='CL_MaxLoanAmount'

