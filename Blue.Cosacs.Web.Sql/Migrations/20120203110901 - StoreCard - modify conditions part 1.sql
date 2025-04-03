-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists (select * from dbo.CMCondition where Condition='ArrearsPCSC')
insert into CMCondition (Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
Values ('ArrearsPCSC','Arrears are less than or greater than X% of StoreCard minimum payment','','1','',1,1)

UPDATE CMStrategyCondition
	set Condition='ArrearsPCSC'
where Strategy='SCNON' and Condition='ArrearsPC'

UPDATE CMStrategyCondition
	set Operator1='7'
where Strategy='SCNON' and Condition='DAYARRS'


if not exists (select * from dbo.CMCondition where Condition='BalSCard')
	insert into CMCondition (Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
	Values ('BalSCard','Balance Outstanding is less than or greater than X% of StoreCard limit','','1','',0,0)

if not exists(select * from CMStrategyCondition where Strategy='SCLR' and Condition='BalSCard')
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCLR','BalSCard','<','35',null,null,null,null,null,null,null,'N')

if exists (select * from CMStrategyCondition where Strategy='SCLR' and ISNULL(Step,0)=10)
Begin
	delete CMStrategyCondition where Strategy='SCLR' and (ISNULL(Step,0)=8 or ISNULL(Step,0)=9)
	UPDATE CMStrategyCondition 
		set Step=8
	where Strategy='SCLR' and ISNULL(Step,0)=10
End

if not exists (select * from dbo.CMCondition where Condition='XPayMS')
	insert into CMCondition (Condition, Description, QualifyingCode, OperandAllowable, Type, FalseStep, AllowReuse)
	Values ('XPayMS','Customer has missed X payments within Y months of first StoreCard transaction','','2','N',0,0)


if not exists (select * from CMStrategyCondition where Strategy='SCED' and Condition='XPayMS')
Begin
	delete CMStrategyCondition where Strategy='SCED' and Condition in('XINSMS','DELTHRESH')
	UPDATE CMStrategyCondition 
		set Operator1='7'
	where Strategy='SCED' and Condition='DAYARRS'
	
	UPDATE CMStrategyCondition 
		set Operator1='45'
	where Strategy='SCED' and Condition='DAYX' and Step=7
	
End

if not exists(select * from CMStrategyCondition where Strategy='SCED' and Condition='XPayMS')
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCED','XPayMS','<','1','6',null,null,null,null,null,null,'N')
	
if not exists (select * from CMStrategyCondition where Strategy='SCHR' and Condition='XPayMS')
Begin
	delete CMStrategyCondition where Strategy='SCHR' and Condition in('XINSMS','MaxItemVal')
		
	UPDATE CMStrategyCondition 
		set Condition='BalSCard',Operator1='75'
	where Strategy='SCHR' and Condition='BALEXCHARGES' 
	
End

if not exists(select * from CMStrategyCondition where Strategy='SCHR' and Condition='XPayMS')
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCHR','XPayMS','<','1','6',null,null,null,null,null,null,'N')
	
if exists (select * from CMStrategyCondition where Strategy='SCHR' and ISNULL(Step,0)=10)
Begin
	delete CMStrategyCondition where Strategy='SCHR' and (ISNULL(Step,0)=8 or ISNULL(Step,0)=9)
	UPDATE CMStrategyCondition 
		set Step=8
	where Strategy='SCHR' and ISNULL(Step,0)=10
End

if not exists (select * from CMStrategyCondition where Strategy='SCRA' and Condition='XPayMS')
Begin
	delete CMStrategyCondition where Strategy='SCRA' and Condition in('XINSMS','ArrearsPC')
		
	UPDATE CMStrategyCondition 
		set Operator1='2'
	where Strategy='SCRA' and Condition='RECARRS' 
	
End

if not exists(select * from CMStrategyCondition where Strategy='SCRA' and Condition='XPayMS')
	insert into CMStrategyCondition (Strategy, Condition, Operand, Operator1, Operator2, OrClause, NextStepTrue, 
					ActionCode, StepActiontype, Step, NextStepFalse, SavedType)
	Values ('SCRA','XPayMS','<','1','6',null,null,null,null,null,null,'N')
	
if exists (select * from CMStrategyCondition where Strategy='SCRA' and ISNULL(Operator1,'0')='60')
BEGIN
	
	UPDATE CMStrategyCondition 
		set Operator1='7'
	where Strategy='SCRA' and Operator1='14'
	UPDATE CMStrategyCondition 
		set Operator1='21'
	where Strategy='SCRA' and Operator1='30'
	UPDATE CMStrategyCondition 
		set Operator1='35'
	where Strategy='SCRA' and Operator1='45'
	UPDATE CMStrategyCondition 
		set Operator1='45'
	where Strategy='SCRA' and Operator1='60'
	
END

