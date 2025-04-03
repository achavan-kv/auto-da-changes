-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'StoreCard' and object_name(id) = 'CMStrategyVariables')
BEGIN
	ALTER TABLE CMStrategyVariables ADD StoreCard CHAR(1) null 
	ALTER TABLE CMStrategyVariables ADD CreditAcct CHAR(1) null 
	ALTER TABLE CMStrategyVariables ADD ArrearsPCSC CHAR(1) null
	ALTER TABLE CMStrategyVariables ADD BalSCard CHAR(1) null
	ALTER TABLE CMStrategyVariables ADD XPayMS CHAR(1) null
	ALTER TABLE CMStrategyVariables ADD MS1Pay CHAR(1) null

end 

IF NOT EXISTS(select * from syscolumns where name = 'StoreCard' and object_name(id) = 'CMStrategyVariablesEntry')
BEGIN
	ALTER TABLE CMStrategyVariablesEntry ADD StoreCard CHAR(1) null 
	ALTER TABLE CMStrategyVariablesEntry ADD CreditAcct CHAR(1) null 
	ALTER TABLE CMStrategyVariablesEntry ADD ArrearsPCSC CHAR(1) null
	ALTER TABLE CMStrategyVariablesEntry ADD BalSCard CHAR(1) null
	ALTER TABLE CMStrategyVariablesEntry ADD XPayMS CHAR(1) null
	ALTER TABLE CMStrategyVariablesEntry ADD MS1Pay CHAR(1) null

end 
IF NOT EXISTS(select * from syscolumns where name = 'StoreCard' and object_name(id) = 'CMStrategyVariablesExit')
BEGIN
	ALTER TABLE CMStrategyVariablesExit ADD StoreCard CHAR(1) null 
	ALTER TABLE CMStrategyVariablesExit ADD CreditAcct CHAR(1) null 
	ALTER TABLE CMStrategyVariablesExit ADD ArrearsPCSC CHAR(1) null
	ALTER TABLE CMStrategyVariablesExit ADD BalSCard CHAR(1) null
	ALTER TABLE CMStrategyVariablesExit ADD XPayMS CHAR(1) null
	ALTER TABLE CMStrategyVariablesExit ADD MS1Pay CHAR(1) null

end 
IF NOT EXISTS(select * from syscolumns where name = 'StoreCard' and object_name(id) = 'CMStrategyVariablesSteps')
BEGIN
	ALTER TABLE CMStrategyVariablesSteps ADD StoreCard CHAR(1) null 
	ALTER TABLE CMStrategyVariablesSteps ADD CreditAcct CHAR(1) null 
	ALTER TABLE CMStrategyVariablesSteps ADD ArrearsPCSC CHAR(1) null
	ALTER TABLE CMStrategyVariablesSteps ADD BalSCard CHAR(1) null
	ALTER TABLE CMStrategyVariablesSteps ADD XPayMS CHAR(1) null
	ALTER TABLE CMStrategyVariablesSteps ADD MS1Pay CHAR(1) null

end 

