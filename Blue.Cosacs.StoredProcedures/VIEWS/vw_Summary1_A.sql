if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_A') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_A
GO 
-- Delamount
create view dbo.vw_Summary1_A
as
	Select f.acctno,isnull(sum(f.transvalue),0) as DelAmount
	From fintrans f
	Where f.transtypecode in ('DEL','ADD','GRT','REP','RDL','RPO','CLD')					-- #10138
	Group by f.acctno
-- 1m 23s Jamaica
go