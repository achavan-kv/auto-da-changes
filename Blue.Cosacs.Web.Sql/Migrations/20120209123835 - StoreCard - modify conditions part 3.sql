-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists (select * from CMStrategyCondition where Strategy='SCNS' and ISNULL(Step,0)=9 and Operator1='60')
Begin
	
	UPDATE CMStrategyCondition 
		set Operator1='44'
	where Strategy='SCNS' and ISNULL(Step,0)=7
	
	UPDATE CMStrategyCondition 
		set Operator1='45'
	where Strategy='SCNS' and ISNULL(Step,0)=9
End

