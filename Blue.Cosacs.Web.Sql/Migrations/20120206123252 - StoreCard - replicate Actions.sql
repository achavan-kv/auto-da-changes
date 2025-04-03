-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists( select * from CMStrategyActions where Strategy like 'SC%')
Begin
insert into CMStrategyActions (Strategy, ActionCode, Description, Cycletonextflag)

select 'SC'+s.Strategy,ActionCode,s.Description,Cycletonextflag
from CMStrategyActions s
where s.Strategy not in('INS','DFA','SER','NSL','REP')

End


	UPDATE code set Category ='SS1' where category='SS2'
	
	delete codecat where category='SS2'
	
if not exists( select * from CMActionRights where Strategy like 'SC%')
Begin

insert into CMActionRights (EmpeeNo, Strategy, Action, EmpeeType, Empeenochange, CycleToNextFlag, MinNotesLength)

select EmpeeNo,'SC'+s.Strategy,Action, EmpeeType, Empeenochange, CycleToNextFlag, MinNotesLength
from CMActionRights s
where s.Strategy not in('INS','DFA','SER','NSL','REP',' ')

End

if not exists( select * from CMWorkListRights where WorkList like 'SC%')
Begin

insert into CMWorkListRights (EmpeeNo, WorkList, EmpeeType, Empeenochange)

select EmpeeNo, 'SC'+WorkList, EmpeeType, Empeenochange
from CMWorkListRights s
where exists (select * from CMWorkList w where w.worklist='SC'+s.WorkList)

End

if not exists( select * from CMCondition where Condition='CreditAcct')
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
	/* Condition - varchar(12) */ 'CreditAcct',
	/* Description - varchar(128) */ 'Account is Credit account',
	/* QualifyingCode - varchar(256) */ '',
	/* OperandAllowable - char(1) */ 'N',
	/* Type - char(1) */ '',
	/* FalseStep - bit */ 0,
	/* AllowReuse - bit */ 0 ) 
end

if not exists(select * from CMStrategyCondition	where Strategy not like 'SC%' and SavedType='N' and Condition='CreditAcct')
BEGIN
	
	insert into dbo.CMStrategyCondition (Strategy,Condition,SavedType) 
	select  
		/* Strategy - char(3) */ s.Strategy,
		/* Condition - varchar(12) */ 'CreditAcct',	
		/* SavedType - char(1) */ 'N' 
	from CMstrategy s
	where Strategy not like 'SC%'
	
	
END
