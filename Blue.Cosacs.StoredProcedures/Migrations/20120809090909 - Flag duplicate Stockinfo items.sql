-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE stockinfo 
	set iupc=iupc+'Dup'
from stockinfo d
where RepossessedItem=0
and exists(select * from stockinfo i where i.RepossessedItem=0 and i.iupc=d.iupc and d.id>i.id and IUPC!='')


UPDATE stockinfo 
	set iupc=iupc+'Dup'
from stockinfo d
where RepossessedItem=1
and exists(select * from stockinfo i where i.RepossessedItem=1 and i.iupc=d.iupc and d.id>i.id and IUPC!='')

