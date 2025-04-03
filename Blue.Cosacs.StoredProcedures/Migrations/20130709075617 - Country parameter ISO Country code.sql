-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from countrymaintenance where codename = 'ISOCountryCode')
BEGIN
	INSERT INTO CountryMaintenance(CountryCode, ParameterCategory, Name, Value, [Type], [Precision], OptionCategory, OptionListName, [Description], CodeName)
	SELECT c.countrycode, '01', 'ISO Country Code', ISOCountryCode, 'text', 0, '', '','This is the ISO Country Code', 'ISOCountryCode'
	FROM Country c
END


if not exists( select * from Code where Code='ECOMMERCE' and CATEGORY='EDC')
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
	/* code - varchar(12) */ 'ECOMMERCE',
	/* codedescript - nvarchar(64) */ N'Online Product Export',
	/* statusflag - char(1) */ 'L',
	/* sortorder - smallint */ 25,
	/* reference - varchar(12) */ '0',
	/* additional - varchar(15) */ '0' )	-- CanReRun

End
