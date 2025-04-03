-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO dbo.CountryMaintenance (
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
) select top 1
	/* CountryCode - char(1) */ CountryCode,
	/* ParameterCategory - varchar(4) */ '22',
	/* Name - varchar(50) */ 'Display FACT employee no',
	/* Value - varchar(1500) */ 'True',
	/* Type - varchar(10) */ 'checkbox',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(1500) */ 'If selected the FACT employee number will be displayed in staff maintenance',
	/* CodeName - varchar(30) */ 'showFactEmpNo' 
	FROM country
	
	--select * from countrymaintenance where name like '%consolidated%'