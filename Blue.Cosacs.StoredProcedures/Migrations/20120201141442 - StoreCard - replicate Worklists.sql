-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMWorkList]') AND name = N'pk_WorkList')
--ALTER TABLE [dbo].[CMWorkList] DROP CONSTRAINT [pk_WorkList]
--GO

alter TABLE CMWorkList alter column [Description] VARCHAR(40) not null
go

--ALTER TABLE [dbo].[CMWorkList] ADD  CONSTRAINT [pk_WorkList] PRIMARY KEY CLUSTERED 
--(
--	[WorkList] ASC,
--	[EmpeeType] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
--GO


if not exists (select * from CMWorkList where worklist like 'SC%')
Begin
insert into CMWorkList (WorkList,Description,EmpeeType) 

select 'SC'+w.Worklist,'StoreCard '+ w.Description,w.EmpeeType
from CMWorkList w 
where Worklist not in('WI1','WRE1','WRE2','WS1')

End

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMWorklistsAcct]') AND name = N'pk_CMWorklistsAcct_acctno')
ALTER TABLE [dbo].[CMWorklistsAcct] DROP CONSTRAINT [pk_CMWorklistsAcct_acctno]
GO
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CMWorklistsAcct]') AND name = N'ix_CMWorklistAcct_datefrom')
DROP INDEX [ix_CMWorklistAcct_datefrom] ON [dbo].[CMWorklistsAcct] WITH ( ONLINE = OFF )
GO

alter TABLE CMWorklistsAcct alter column Strategy VARCHAR(7) not null
alter TABLE CMWorklistsAcct alter column WorkList VARCHAR(10) not null

ALTER TABLE [dbo].[CMWorklistsAcct] ADD  CONSTRAINT [pk_CMWorklistsAcct_acctno] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[Strategy] ASC,
	[Datefrom] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ix_CMWorklistAcct_datefrom] ON [dbo].[CMWorklistsAcct] 
(
	[Datefrom] ASC
)
INCLUDE ( [Worklist]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

alter TABLE CMWorklistSOD alter column WorkList VARCHAR(10) not null
go

UPDATE CMstrategyCondition  
set Actioncode=rtrim(w.worklist)+' '+w.Description
from CMstrategyCondition c, cmWorklist w
where c.StepActiontype='W' and SUBSTRING(c.actioncode,1,8)=SUBSTRING(rtrim(w.worklist)+' '+w.Description,1,8)
and c.Actioncode!=rtrim(w.worklist)+' '+w.Description

select worklist,worklist + ' '+ description as description, REPLACE(worklist,'SC','') + REPLACE(description,'Storecard','') as oldAction 
into #temp
from CMWorkList where Worklist like 'SC%'

UPDATE CMStrategyCondition
set Actioncode= t.description 
from CMStrategyCondition c , #temp t
where c.Strategy like 'SC%' and c.ActionCode=t.oldAction and StepActiontype='W'

