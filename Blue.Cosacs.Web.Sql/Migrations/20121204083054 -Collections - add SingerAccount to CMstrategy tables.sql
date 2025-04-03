-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'SingerAcct' and object_name(id) = 'CMStrategyVariables')
BEGIN
	ALTER TABLE CMStrategyVariables ADD SingerAcct CHAR(1) null 

end 

IF NOT EXISTS(select * from syscolumns where name = 'SingerAcct' and object_name(id) = 'CMStrategyVariablesEntry')
BEGIN
	ALTER TABLE CMStrategyVariablesEntry ADD SingerAcct CHAR(1) null 

end 
IF NOT EXISTS(select * from syscolumns where name = 'SingerAcct' and object_name(id) = 'CMStrategyVariablesExit')
BEGIN
	ALTER TABLE CMStrategyVariablesExit ADD SingerAcct CHAR(1) null 

end 
IF NOT EXISTS(select * from syscolumns where name = 'SingerAcct' and object_name(id) = 'CMStrategyVariablesSteps')
BEGIN
	ALTER TABLE CMStrategyVariablesSteps ADD SingerAcct CHAR(1) null 

end 




if not exists( select * from CMCondition where Condition='SingerAcct')
Begin
Insert into dbo.CMCondition (
	Condition,
	Description,
	QualifyingCode,
	OperandAllowable,
	[Type],
	FalseStep,
	AllowReuse
) VALUES ( 
	/* Condition - varchar(12) */ 'SingerAcct',
	/* Description - varchar(128) */ 'Account is Singer account',
	/* QualifyingCode - varchar(256) */ '',
	/* OperandAllowable - char(1) */ 'N',
	/* Type - char(1) */ 'N',
	/* FalseStep - bit */ 0,
	/* AllowReuse - bit */ 0 ) 
end

