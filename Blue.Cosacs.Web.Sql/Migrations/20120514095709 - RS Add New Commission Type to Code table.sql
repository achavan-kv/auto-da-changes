-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


if not exists( select * from code where category= 'CTP' and code='PS')
BEGIN
	
	insert into dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		additional,
		Additional2
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'CTP',
		/* code - varchar(18) */ 'PS',
		/* codedescript - nvarchar(64) */ N'Product Sub-Class',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '0',
		/* additional - varchar(15) */ '',
		/* Additional2 - varchar(15) */ '' )
		
	insert into dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		additional,
		Additional2
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'CTP',
		/* code - varchar(18) */ 'SK',
		/* codedescript - nvarchar(64) */ N'Product SKU',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '0',
		/* additional - varchar(15) */ '',
		/* Additional2 - varchar(15) */ '' )
		
	UPDATE code
	set codedescript='Product Class' where category='CTP' and code='PL'
	
	
END