-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists (select * from dbo.CMCondition where Condition='MthArrMthsSC')
insert into CMCondition (Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
Values ('MthArrMthsSC','Account has been at least X months payments in arrears in last Y months of StoreCard payments','','2','N',0,0)

UPDATE CMStrategyCondition
	set Condition='MthArrMthsSC'
where Strategy='SCHR' and Condition='MthArrMths'

UPDATE CMStrategyCondition
	set Operand='>'
where Strategy='SCHR' and Condition='XPayMS'


IF NOT EXISTS(select * from syscolumns where name = 'MthArrMthsSC' and object_name(id) = 'CMStrategyVariables')
BEGIN
	ALTER TABLE CMStrategyVariables ADD MthArrMthsSC CHAR(1) null
end 

IF NOT EXISTS(select * from syscolumns where name = 'MthArrMthsSC' and object_name(id) = 'CMStrategyVariablesEntry')
BEGIN
	ALTER TABLE CMStrategyVariablesEntry ADD MthArrMthsSC CHAR(1) null 
end 
IF NOT EXISTS(select * from syscolumns where name = 'MthArrMthsSC' and object_name(id) = 'CMStrategyVariablesExit')
BEGIN
	ALTER TABLE CMStrategyVariablesExit ADD MthArrMthsSC CHAR(1) null

end 
IF NOT EXISTS(select * from syscolumns where name = 'MthArrMthsSC' and object_name(id) = 'CMStrategyVariablesSteps')
BEGIN
	ALTER TABLE CMStrategyVariablesSteps ADD MthArrMthsSC CHAR(1) null

end 

