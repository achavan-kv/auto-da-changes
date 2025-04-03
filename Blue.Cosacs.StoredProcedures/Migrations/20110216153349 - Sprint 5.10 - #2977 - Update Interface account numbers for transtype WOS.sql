-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists(select * from transtype where transtypecode = 'WOS' and [description] = 'Writeoff Service')
UPDATE transtype
SET interfaceaccount = 1301,
	interfacebalancing = '9020C'
WHERE transtypecode = 'WOS' and [description] = 'Writeoff Service' 