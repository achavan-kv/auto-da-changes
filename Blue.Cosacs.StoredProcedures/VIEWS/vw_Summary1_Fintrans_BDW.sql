if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_Fintrans_BDW') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_Fintrans_BDW
GO 
-- Bad Debt Writeoff transactions
create view dbo.vw_Summary1_Fintrans_BDW
as
	Select	acctno,	SUM(transvalue) AS BDW_Total
		From vw_Summary1_C_Fintrans
			Where transtypecode in ('BDW')
	
	Group by acctno

go