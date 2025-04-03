-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from code where category = 'EDC' and code = 'STCARDEXPORT')
INSERT INTO Code (origbr,
				  category,
				  code,
				  codedescript,
				  statusflag,
				  sortorder,
				  reference,
				  additional)
SELECT	0,	
		'EDC',
		'STCARDEXPORT',
		'Store Card Export',
		'L',
		13,
		0,
		NULL