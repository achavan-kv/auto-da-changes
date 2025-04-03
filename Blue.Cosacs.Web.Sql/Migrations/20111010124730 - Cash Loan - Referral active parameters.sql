-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmcat INT

select @parmcat=code from code where category='CMC' and codedescript='Cash Loans' 

if not exists (select * from CountryMaintenance where codename='CL_ReferralMonths')
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
	/* Name - varchar(50) */ 'Referral History (Months)',
	/* Value - varchar(200) */ '0',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'Number of months back to check if account has been referred. If account has been referred within ''x'' months, it will not qualify for Cash Loan',
	/* CodeName - varchar(30) */ 'CL_ReferralMonths'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_ReferralArrears')
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
	/* Name - varchar(50) */ 'Referral Msg - Account in Arrears',
	/* Value - varchar(200) */ 'false',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If true and the customer has arrears on an account at the time of applying for Cash Loan a message box is displayed',
	/* CodeName - varchar(30) */ 'CL_ReferralArrears'
	From Country 
End


if not exists (select * from CountryMaintenance where codename='CL_ReferralRescored')
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
	/* Name - varchar(50) */ 'Referral Msg - Rescored',
	/* Value - varchar(200) */ 'false',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If true and the customer’s application has been re-scored, do not refer if rescored value is higher than country parameter score for cash loan qualification. If the customer’s rescore triggers a decrease in limit which then makes the loan amount greater than the % qualification or the spend is now less than the loan value a message box is displayed. ',
	/* CodeName - varchar(30) */ 'CL_ReferralRescored'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_ReferralStatus')
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
	/* Name - varchar(50) */ 'Referral Msg - High Status',
	/* Value - varchar(200) */ 'false',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If true and an account status code went over the maximum status code but has now returned to acceptable status code a message box is displayed',
	/* CodeName - varchar(30) */ 'CL_ReferralStatus'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_ReferralResidence')
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
	/* Name - varchar(50) */ 'Referral Msg - Residence changed',
	/* Value - varchar(200) */ 'false',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If true and the application has been updated and the customer’s home address has been changed a message box is displayed',
	/* CodeName - varchar(30) */ 'CL_ReferralResidence'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_ReferralEmployment')
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
	/* Name - varchar(50) */ 'Referral Msg - Employment changed',
	/* Value - varchar(200) */ 'false',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If true and the application has been updated and the customer’s work address has been changed a message box is displayed',
	/* CodeName - varchar(30) */ 'CL_ReferralEmployment'
	From Country 
End

if not exists (select * from CountryMaintenance where codename='CL_ReferralPercentage')
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
	/* Name - varchar(50) */ 'Referral Msg - Percentage',
	/* Value - varchar(200) */ 'false',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(500) */ 'If true and the customer’s available spend is greater than the maximum loan amount but the percentage is less than Loan RF% available amount a message box is displayed',
	/* CodeName - varchar(30) */ 'CL_ReferralPercentage'
	From Country 
End


if not exists (select * from Code where category='SOA' and Code ='CL')
Begin
	insert into dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		additional
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'SOA',
		/* code - varchar(18) */ 'CL',
		/* codedescript - nvarchar(64) */ N'Cash Loan Letter',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '0',
		/* additional - varchar(15) */ '' ) 
end

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='CashLoan' AND column_name = 'EmpeenoAccept')
BEGIN
 ALTER TABLE CashLoan ADD [EmpeenoAccept] int
END
GO 


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='CashLoan' AND column_name = 'EmpeenoDisburse')
BEGIN
 ALTER TABLE CashLoan ADD [EmpeenoDisburse] int
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='Customer' AND column_name = 'CashLoanBlocked')
BEGIN
 ALTER TABLE Customer ADD [CashLoanBlocked] tinyint default 0
END
GO
