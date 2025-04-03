-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists (select * from dbo.CMCondition where Condition='DaysDue')
insert into CMCondition (Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
Values ('DaysDue','Account is less than or greater than X days after StoreCard due date','','1','',1,0)

if not exists (select * from dbo.CMCondition where Condition='MS1Pay')
insert into CMCondition (Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
Values ('MS1Pay','Customer has missed first StoreCard payment date','','N','N',0,0)


delete CMStrategyCondition where Strategy='SCMR' and Condition ='BALEXCHARGES'

if not exists(select * from CMStrategyCondition where Strategy='SCMR' and Condition='BalSCard')
Begin
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCMR','BalSCard','<','75',null,null,null,null,null,null,null,'N')
	
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCMR','BalSCard','>=','35',null,null,null,null,null,null,null,'N')
end
	
UPDATE CMCondition 
	set AllowReuse=1,[Type]='N'
where Condition='BalSCard'

if exists (select * from CMStrategyCondition where Strategy='SCMR' and ISNULL(Step,0)=10)
Begin
	delete CMStrategyCondition where Strategy='SCMR' and (ISNULL(Step,0)=8 or ISNULL(Step,0)=9)
	UPDATE CMStrategyCondition 
		set Step=8
	where Strategy='SCMR' and ISNULL(Step,0)=10
End

if not exists(select * from CMStrategyCondition where Strategy='SCNS' and Condition='MS1Pay')
Begin
	delete CMStrategyCondition where Strategy='SCNS' and Condition in('MS1INS','DELTHRESH')
	
	UPDATE CMStrategyCondition 
		set Operator1='7',Operand='>'
	where Strategy='SCNS' and Condition='DAYARRS'
	
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCNS','MS1Pay',null,null,null,null,null,null,null,null,null,'N')
End




