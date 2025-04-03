-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

If not exists(select * from code where code='ADI' and category='APS')
Insert into dbo.code (
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
	/* category - varchar(12) */ 'APS',
	/* code - varchar(12) */ 'ADI',
	/* codedescript - nvarchar(64) */ N'Awaiting D.A. Instant Credit',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 0,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ null ) 
