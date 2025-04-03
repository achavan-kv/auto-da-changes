-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists(select * from code where category = 'ICF' and code = 'INS')
update code 
set code = 'INST'
where category = 'ICF' 
and code = 'INS'