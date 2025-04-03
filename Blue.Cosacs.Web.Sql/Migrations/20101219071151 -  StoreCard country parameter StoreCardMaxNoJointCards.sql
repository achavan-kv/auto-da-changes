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
) select 
	/* CountryCode - char(1) */ CountryCode,
	/* ParameterCategory - varchar(4) */ '33',
	/* Name - varchar(50) */ 'Max Number of Joint Cards',
	/* Value - varchar(1500) */ '1',
	/* Type - varchar(10) */ 'numeric',
	/* Precision - int */ 0,
	/* OptionCategory - varchar(4) */ '',
	/* OptionListName - varchar(50) */ '',
	/* Description - varchar(1500) */ 'Maximum number of Joint Cards for store card',
	/* CodeName - varchar(30) */ 'StoreCardMaxNoJointCards' 
	FROM country
	
