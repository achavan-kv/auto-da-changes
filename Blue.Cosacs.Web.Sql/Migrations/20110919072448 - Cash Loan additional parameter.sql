-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parmcat INT

select @parmcat=code from code where category='CMC' and codedescript='Cash Loans' 

if not exists (select * from code where category='CLAT')
BEGIN
	insert into dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		Additional
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'CLAT',
		/* code - varchar(12) */ 'B',
		/* codedescript - nvarchar(64) */ N'Both',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '',
		null )
	
	insert into dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		Additional
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'CLAT',
		/* code - varchar(12) */ 'R',
		/* codedescript - nvarchar(64) */ N'RF',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '',
		null ) 
		
	insert into dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		Additional
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'CLAT',
		/* code - varchar(12) */ 'HP',
		/* codedescript - nvarchar(64) */ N'HP',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '',
		null )
		
	insert into dbo.codecat (
		origbr,
		category,
		catdescript,
		codelgth,
		forcenum,
		forcenumdesc,
		usermaint
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'CLAT',
		/* catdescript - nvarchar(64) */ N'CL_AccountType',
		/* codelgth - int */ 4,
		/* forcenum - char(1) */ 'N',
		/* forcenumdesc - char(1) */ 'N',
		/* usermaint - char(1) */ 'N' ) 
END

if not exists (select * from CountryMaintenance where codename='CL_AccountType')
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
	/* Name - varchar(50) */ 'Credit Account types to qualify for Cash Loan',
	/* Value - varchar(200) */ 'Both',
	/* Type - varchar(10) */ 'dropdown',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ 'CLAT',
	/* OptionListName - varchar(50) */ 'CashLoanAccountTypes',
	/* Description - varchar(500) */ 'Credit Account types to qualify for Cash Loan. RF, HP or Both',
	/* CodeName - varchar(30) */ 'CL_AccountType'
	From Country 
End

drop TABLE dbo.InstantCreditApprovalChecks
go

create table InstantCreditApprovalChecks
		(
		Hcustid				varchar(20),
		Hscore				smallint,
		Hdatechange			datetime,
		HdateSettCred		datetime,
		HdateSettcash 		datetime,
		HexistAccLen		smallint,
		HhighSettStatus		char(1),
		HspouseBDWlegal		char(1),
		HReferred			char(1),
		HmaxArrearsLevel	decimal(9,2),
		HRFpcAvail			int,			-- CR906  --RM - 06.05.2010 CR1082 has error if percentage too large to fit in small int
		HRFpcCashLoan		INT,			-- CR1232
		HAcctType			char(2),		-- CR1232
		HCustAddDateChange	datetime,
		HEmploymentDateChange datetime,
		Jcustid				varchar(20),
		Jscore				smallint,
		Jdatechange			datetime,
		JdateSettCred		datetime,
		JdateSettcash		datetime,
		JexistAccLen		smallint,
		JhighSettStatus		char(1),
		JspouseBDWlegal		char(1),
		JReferred			char(1),
		JmaxArrearsLevel	decimal(9,2),
		JRFpcAvail			int,			-- CR906  --RM - 06.05.2010 CR1082 has error if percentage too large to fit in small int
		JRFpcCashLoan		INT,			-- CR1232
		JAcctType			char(2),		-- CR1232
		JCustAddDateChange	datetime,
		JEmploymentDateChange datetime,
		InstantCredit		char(1),
		LoanQualified		char(1),			-- CR906
		PreapprovalDate		datetime			-- CR983
	)
