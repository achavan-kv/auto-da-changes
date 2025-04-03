-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from Config.Setting where id = 'NonCourtsDealerName')
BEGIN
	insert into config.setting
	select 'Blue.Config', 'NonCourtsDealerName', null, null, null, null, CountryMaintenance.Value, null, null
	from CountryMaintenance
	where CodeName = 'NonCourtsDealerName'
END