-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update warranty.warranty
set TypeCode = 'I'
from warranty.warranty w
inner join warranty.linkwarranty lw on w.id = lw.warrantyid
inner join warranty.linkproduct lp on lw.linkid = lp.linkid
where lp.refcode = 'ZZ'
and substring(w.number, len(w.number), len(w.number)) != 'M'