-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMStrategy]') AND name = N'pk_CMStrategystrategy')
ALTER TABLE [dbo].[CMStrategy] DROP CONSTRAINT [pk_CMStrategystrategy]
GO

alter TABLE CMStrategy alter column Strategy VARCHAR(7) not null
go

ALTER TABLE [dbo].[CMStrategy] ADD  CONSTRAINT [pk_CMStrategystrategy] PRIMARY KEY CLUSTERED 
(
	[Strategy] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

if not exists( select * from CMStrategy where Strategy like 'SC%')
Begin
insert into CMStrategy (Strategy, Description, IsActive, ReadOnly, Manual)

select 'SC'+s.Strategy,'StoreCard '+s.Description,s.IsActive, s.ReadOnly, s.Manual
from CMStrategy s
where s.Strategy not in('INS','DFA','SER','NSL','REP')

End

alter TABLE CMStrategyCondition alter column Strategy VARCHAR(7) not null
go

if not exists( select * from CMStrategyCondition where Strategy like 'SC%')
Begin
insert into dbo.CMStrategyCondition (
	Strategy,
	Condition,
	Operand,
	Operator1,
	Operator2,
	OrClause,
	NextStepTrue,
	ActionCode,
	StepActiontype,
	Step,
	NextStepFalse,
	SavedType
)

select  'SC'+c.Strategy,Condition,Operand,Operator1,Operator2,OrClause,NextStepTrue,
			case 
				when ISNULL(c.SavedType,'')='X' or c.StepActiontype in('X','P') then 'SC'+c.ActionCode
				else c.ActionCode end ,
			StepActiontype,Step,NextStepFalse,SavedType

from CMStrategyCondition c
where c.Strategy not in('INS','DFA','SER','NSL','REP')
and exists (select * from CMStrategy s where c.Strategy=s.Strategy)

end

if not exists( select * from CMCondition where Condition='StoreCard')
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
	/* Condition - varchar(12) */ 'StoreCard',
	/* Description - varchar(128) */ 'Account is Store card account',
	/* QualifyingCode - varchar(256) */ '',
	/* OperandAllowable - char(1) */ 'N',
	/* Type - char(1) */ '',
	/* FalseStep - bit */ 0,
	/* AllowReuse - bit */ 0 ) 
end
	
if not exists(select * from CMStrategyCondition	where Strategy like 'SC%' and SavedType='N' and Condition='StoreCard')
BEGIN
	
	insert into dbo.CMStrategyCondition (Strategy,Condition,SavedType) 
	select  
		/* Strategy - char(3) */ s.Strategy,
		/* Condition - varchar(12) */ 'StoreCard',	
		/* SavedType - char(1) */ 'N' 
	from CMstrategy s
	where Strategy like 'SC%'
	
	
END

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMStrategyActions]') AND name = N'PK_CMStrategyActions')
ALTER TABLE [dbo].[CMStrategyActions] DROP CONSTRAINT [PK_CMStrategyActions]
GO

alter TABLE CMStrategyActions alter column Strategy VARCHAR(7) not null
go

ALTER TABLE [dbo].[CMStrategyActions] ADD  CONSTRAINT [PK_CMStrategyActions] PRIMARY KEY CLUSTERED 
(
	[Strategy] ASC,
	[ActionCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMStrategyAcct]') AND name = N'pk_CMStrategyAcct_acctno')
ALTER TABLE [dbo].[CMStrategyAcct] DROP CONSTRAINT [pk_CMStrategyAcct_acctno]
GO

alter TABLE CMStrategyAcct alter column Strategy VARCHAR(7) not null
go

ALTER TABLE [dbo].[CMStrategyAcct] ADD  CONSTRAINT [pk_CMStrategyAcct_acctno] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[strategy] ASC,
	[datefrom] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

alter TABLE CMStrategySOD alter column Strategy VARCHAR(7) not null
go

