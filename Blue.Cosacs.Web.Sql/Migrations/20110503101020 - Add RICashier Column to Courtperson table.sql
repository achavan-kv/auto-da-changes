-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RICashierCode'
               AND OBJECT_NAME(id) = 'CourtsPerson')
BEGIN
  ALTER TABLE CourtsPerson ADD RICashierCode INT 
END

go

 create TABLE #empeeno (id INT IDENTITY,empeeno INT,numbr INT,dateagrmt datetime)
 
 insert into #empeeno
 select distinct createdby,COUNT(*),MAX(dateagrmt)
 from courtsperson c INNER JOIN agreement ag on c.empeeno=ag.createdby 
 where empeetype!='Z'
 group by createdby
 having LEN(createdby)!=3 and MAX(dateagrmt)>='2010-01-01'
 order by MAX(dateagrmt)
 
UPDATE CourtsPerson
	set RICashierCode=id	
from courtsperson c INNER JOIN #empeeno t on c.empeeno=t.empeeno


	

