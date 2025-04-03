if exists(select * FROM dbo.sysobjects 
               WHERE ID = object_id('dbo.vw_Summary1_D_Fintrans') AND OBJECTPROPERTY(id, 'IsView') = 1)
	drop view dbo.vw_Summary1_D_Fintrans
GO 
-- Payment transactions
create view dbo.vw_Summary1_D_Fintrans
as
	Select f.acctno,sum(f.transvalue) as payments	--f.datetrans,f.transtypecode,f.transvalue
		From fintrans f, temp_Summary1_MR s1
			Where f.acctno = s1.acctno
				AND f.transtypecode in ('PAY','COR','XFR','REF','RET','DDE','DDN','DDR')
		Group by f.acctno

go
