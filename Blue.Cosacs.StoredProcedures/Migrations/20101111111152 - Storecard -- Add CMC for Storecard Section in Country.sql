-- put your SQL code here
IF NOT EXISTS (SELECT * FROM code WHERE category = 'cmc' AND codedescript LIKE 'StoreCard')
INSERT INTO dbo.code (
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
	/* category - varchar(12) */ 'CMC',
	/* code - varchar(12) */ '33',
	/* codedescript - nvarchar(64) */ N'StoreCard',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 0,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ NULL ) 

