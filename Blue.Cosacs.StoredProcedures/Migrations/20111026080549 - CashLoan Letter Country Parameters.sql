-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmcat INT

select @parmcat=code from code where category='CMC' and codedescript='Cash Loans' 

if not exists (select * from CountryMaintenance where codename='CL_CashLoanLetterPrevSettMths')
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
	/* Name - varchar(50) */ 'Months since most recent settled Cash Loan',
	/* Value - varchar(200) */ '2',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'Number of months a customers most recent Cash Loan account would need to have been settled in order for a Cash Loan letter to be sent to a Customer that does not have a current Cash Loan and has re-qualified',
	/* CodeName - varchar(30) */ 'CL_CashLoanLetterPrevSettMths'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_PercentagePaid')
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
	/* Name - varchar(50) */ 'Percentage paid of a Cash Loan',
	/* Value - varchar(200) */ '100',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'The percentage of a current Cash Loan that is paid to enable letters to be sent. ',
	/* CodeName - varchar(30) */ 'CL_PercentagePaid'
	From Country 
End


--Insert new letter codes into LT1 Categories

if not exists(select * from code where category = 'LT1' and code = 'LoanS')
begin
	insert into code (origbr, category, code, codedescript, statusflag, sortorder, reference, additional)
	select 0, 'LT1', 'LoanS', 'Cash Loan Settled', 'L', 0, 0, null
end

if not exists(select * from code where category = 'LT1' and code = 'LoanP')
begin
	insert into code (origbr, category, code, codedescript, statusflag, sortorder, reference, additional)
	select 0, 'LT1', 'LoanP', 'Cash Loan Paid', 'L', 0, 0, null
end

