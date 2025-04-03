-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from [sets] where tname='fascia')
BEGIN
	
insert into dbo.sets (
	setname,
	empeeno,
	tname,
	dateamend,
	columntype,
	setdescript
) VALUES ( 
	/* setname - nvarchar(32) */ N'C',
	/* empeeno - int */ 0,
	/* tname - varchar(24) */ 'fascia',
	/* dateamend - datetime */ '2012-7-27 8:29:4.473',
	/* columntype - char(1) */ 'V',
	/* setdescript - nvarchar(80) */ N'Courts Store' ) 
	
insert into dbo.sets (
	setname,
	empeeno,
	tname,
	dateamend,
	columntype,
	setdescript
) VALUES ( 
	/* setname - nvarchar(32) */ N'N',
	/* empeeno - int */ 0,
	/* tname - varchar(24) */ 'fascia',
	/* dateamend - datetime */ '2012-7-27 8:29:4.473',
	/* columntype - char(1) */ 'V',
	/* setdescript - nvarchar(80) */ N'Non Courts Store' ) 
	
END
