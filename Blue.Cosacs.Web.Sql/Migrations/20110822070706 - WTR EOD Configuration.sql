-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists( select * from Code where Code='WKLYTRAD' and CATEGORY='EDC')
Begin
insert into dbo.code (
	origbr,
	category,
	code,
	codedescript,
	statusflag,
	sortorder,
	reference,
	additional
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'EDC',
	/* code - varchar(12) */ 'WKLYTRAD',
	/* codedescript - nvarchar(64) */ N'Weekly Trading Report',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 8,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ '0' )	

End