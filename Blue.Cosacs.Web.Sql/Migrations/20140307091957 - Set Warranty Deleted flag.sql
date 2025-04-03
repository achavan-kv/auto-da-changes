-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update warranty.Warranty
set Deleted = 1
from warranty.Warranty w inner join stockitem s on w.number=s.itemno
where free!=1
and s.deleted='Y'

update warranty.Warranty
set Deleted = 1
from warranty.Warranty w inner join stockitem s on replace(w.number,'M','')=s.itemno
where free=1
and s.deleted='Y'
