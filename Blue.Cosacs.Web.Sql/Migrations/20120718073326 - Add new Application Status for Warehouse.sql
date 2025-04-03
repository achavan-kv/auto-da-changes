-- transaction: true 
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from code where category = 'APS' and code = 'SCD')
BEGIN
	insert into code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, additional2)
	select null, 'APS', 'SCD', 'Scheduled', 'L', 0, 0, null, null
END

IF NOT EXISTS(select * from code where category = 'APS' and code = 'FLD')
BEGIN
	insert into code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, additional2)
	select null, 'APS', 'FLD', 'Failed Deliver', 'L', 0, 0, null, null
END

