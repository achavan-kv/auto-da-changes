-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #13720 - CR12949

IF NOT EXISTS(select * from CMStrategy where Strategy = 'RDYAST')
BEGIN
	INSERT INTO CMStrategy(Strategy, Description, IsActive, ReadOnly, Manual)
	VALUES('RDYAST', 'Ready Assist', 1, 0, 0)
END

IF NOT EXISTS(select * from CMCondition where condition = 'READYASSIST')
BEGIN
	INSERT INTO CMCondition(Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
	VALUES ('READYASSIST', 'Account has Ready Assist products', '','N', 'N', 0, 0)
END

IF NOT EXISTS(select * from CMStrategyCondition where strategy = 'RDYAST' and Condition = 'READYASSIST')
BEGIN
	INSERT INTO CMStrategyCondition(Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	VALUES ('RDYAST', 'READYASSIST', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'N')
END


IF NOT EXISTS(select * from code where category = 'SS1' and code = 'RDYAST')
BEGIN
	INSERT INTO Code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
	VALUES(0, 'SS1', 'RDYAST', 'Ready Assist', 'L', 0, 0, NULL, NULL)
END

IF NOT EXISTS(select * from syscolumns where name = 'ReadyAssist' and object_name(id) = 'CMStrategyVariablesEntry')
BEGIN
	ALTER TABLE CMStrategyVariablesEntry ADD ReadyAssist CHAR(1) null 
END 

IF NOT EXISTS(select * from syscolumns where name = 'ReadyAssist' and object_name(id) = 'CMStrategyVariablesExit')
BEGIN
	ALTER TABLE CMStrategyVariablesExit ADD ReadyAssist CHAR(1) null 
END

IF NOT EXISTS(select * from syscolumns where name = 'ReadyAssist' and object_name(id) = 'CMStrategyVariablesSteps')
BEGIN
	ALTER TABLE CMStrategyVariablesSteps ADD ReadyAssist CHAR(1) null 
END 


