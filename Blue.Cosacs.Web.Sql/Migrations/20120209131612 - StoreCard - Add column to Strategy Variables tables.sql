-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'DaysDue' and object_name(id) = 'CMStrategyVariables')
BEGIN
	ALTER TABLE CMStrategyVariables ADD DaysDue CHAR(1) null 

end 

IF NOT EXISTS(select * from syscolumns where name = 'DaysDue' and object_name(id) = 'CMStrategyVariablesEntry')
BEGIN
	ALTER TABLE CMStrategyVariablesEntry ADD DaysDue CHAR(1) null 

end 
IF NOT EXISTS(select * from syscolumns where name = 'DaysDue' and object_name(id) = 'CMStrategyVariablesExit')
BEGIN
	ALTER TABLE CMStrategyVariablesExit ADD DaysDue CHAR(1) null 

end 
IF NOT EXISTS(select * from syscolumns where name = 'DaysDue' and object_name(id) = 'CMStrategyVariablesSteps')
BEGIN
	ALTER TABLE CMStrategyVariablesSteps ADD DaysDue CHAR(1) null 

end 

