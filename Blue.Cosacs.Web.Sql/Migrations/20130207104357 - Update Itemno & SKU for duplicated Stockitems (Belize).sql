-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE stockinfo 
	set itemno=iupc
where RepossessedItem=0
and (IUPC!=itemno  and  iupc = itemno+'Dup')
and iupc like '%Dup'

UPDATE stockinfo 
	set sku=iupc
where RepossessedItem=0
and (IUPC!=itemno  and  iupc = sku+'Dup')
and iupc like '%Dup'


UPDATE stockinfo 
	set itemno=iupc
where RepossessedItem=1
and (IUPC!=itemno  and  iupc = itemno+'Dup')
and iupc like '%Dup'
