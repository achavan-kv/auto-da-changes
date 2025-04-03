-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE dbo.CMStrategyCondition
set OrClause=null
where Strategy like '%wo' and SavedType='N' and OrClause is not null and Condition='Woff'

UPDATE CMStrategyCondition
set OrClause=null
where Strategy like 'SCMR' and SavedType='N' and OrClause is not null and Condition='SCORE'

UPDATE dbo.CMStrategyCondition
set Operand='>='
where Strategy like '%RA' and SavedType='N'  and Condition='RECARRS' and Operand!='>='

--delete CMStrategyCondition where strategy ='SCHR' and  condition='MthArrMths' and SavedType='N'

UPDATE CMStrategyCondition
set Operator2=6
where Strategy='SCHR' and Condition='RETCHQ'

UPDATE dbo.CMStrategyCondition
set condition='XpayMS'
where strategy ='SCLR' and  condition='XINSMS'

UPDATE dbo.CMStrategyCondition
set condition='XpayMS'
where strategy ='SCMR' and  condition='XINSMS'

UPDATE CMStrategyCondition
set Operator1=45
where strategy ='SCNS' and  Operator1=60


if exists( select * from CMStrategyCondition where Strategy='SCPWO' and Condition='REPO')
BEGIN
	delete CMStrategyCondition where Strategy='SCPWO' and Step in(2,3,4)
	UPDATE CMStrategyCondition
		set Step=2,NextStepTrue=3
	where Strategy='SCPWO' and Step=5
	
	UPDATE CMStrategyCondition
		set Step=3
	where Strategy='SCPWO' and Step=6
END

