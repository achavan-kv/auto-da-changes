-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM code WHERE category = 'ICF' AND CODE = 'REF')
INSERT INTO code (
	origbr,	category,	code,	codedescript,
	statusflag,	sortorder,	reference,	additional
) VALUES ( 
	0,	 'ICF', 'REF', N'Account was Referred',
	'L', 0, '0', null ) 

