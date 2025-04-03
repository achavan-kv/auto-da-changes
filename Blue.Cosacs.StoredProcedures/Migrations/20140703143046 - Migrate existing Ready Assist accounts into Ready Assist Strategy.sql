-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

	--Previous arrears
	select 
		i.acctno, 
		coalesce(MAX(a.dateto), 
		(select dateadd(day, -1, min(datefrom))	from arrearsdaily x where x.acctno = i.acctno),
		getdate()) as dateenteredarrears
	into 
		#prevArrears
	from 
		instalplan i
	inner join 
		acct aa on i.acctno = aa.acctno
	inner join 
		ReadyAssistDetails ra on ra.AcctNo = i.acctno
	left join 
		ArrearsDaily a on a.acctno=i.acctno
		and a.dateto is not null
		and ((a.arrears < .2 * i.instalamount)	
				or (a.arrears =0))
	where 
		ra.Status = 'Active'						
	group by 
		i.acctno


	insert into CMStrategyAcct
	select 
		a.acctno, 
		'RDYAST', 
		getdate(), 
		NULL, 
		1, 
		getdate()
	from 
		acct a
	inner join 
		instalplan i on a.acctno = i.acctno
	inner join 
		ReadyAssistDetails ra on ra.AcctNo = i.acctno
	where 
	a.arrears >= .2 * i.instalamount and a.outstbal >=.2 * i.instalamount
	and exists(select * from ArrearsDaily d
			   where d.Acctno = i.acctno 
					 and d.datefrom = (select min(datefrom) 
										from ArrearsDaily ad, #prevArrears p
										where p.acctno = ad.Acctno
										and ad.Acctno = a.acctno
										and ad.datefrom >= p.dateenteredarrears)
												
					and datediff(hour, d.datefrom ,getdate()) /24.00 >= 1
				)
	and ra.Status = 'Active'
	and not exists(select * from CMStrategyAcct ca
	where 
		ca.acctno = a.acctno
	and ca.dateto is null)

          