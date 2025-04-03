-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Multiple WOF strategies
select * into #CMStrategyAcct_WOF
from CMStrategyAcct a where strategy='WOF' and dateto is null
	and datefrom != (select MAX(datefrom) from CMStrategyAcct a2 where a.acctno=a2.acctno and strategy='WOF' and dateto is null)


UPDATE CMStrategyAcct set dateto=a.datefrom
from cmstrategyacct a INNER JOIN #CMStrategyAcct_WOF a2 on a.acctno=a2.acctno and a2.datefrom =a.datefrom and a.strategy='WOF'

-- Multiple PWO strategies
select * into #CMStrategyAcct_PWO
from CMStrategyAcct a where strategy='PWO' and dateto is null
	and datefrom != (select MAX(datefrom) from CMStrategyAcct a2 where a.acctno=a2.acctno and strategy='PWO' and dateto is null)

UPDATE CMStrategyAcct set dateto=a.datefrom
from cmstrategyacct a INNER JOIN #CMStrategyAcct_PWO a2 on a.acctno=a2.acctno and a2.datefrom =a.datefrom and a.strategy='PWO'


