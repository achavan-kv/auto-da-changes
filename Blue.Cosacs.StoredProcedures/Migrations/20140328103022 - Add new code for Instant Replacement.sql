-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from code where category ='COL' and code='I')
insert into code (origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
values(0,'COL','I','Instant Replacement','L',0,0,null,null)

