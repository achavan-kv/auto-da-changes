if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_E_Fintrans') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_E_Fintrans
GO 
-- Interest charges transactions
create view dbo.vw_Summary1_E_Fintrans
as
	Select	acctno,	SUM(transvalue) AS intcharges
		From vw_Summary1_C_Fintrans
			Where transtypecode in ('INT', 'ADM','DDF')
	
	Group by acctno

go