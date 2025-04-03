ALTER TABLE StoreCard
DROP COLUMN Activated, Deactivated,LostorStolen
GO
if not exists (select * from information_schema.columns where column_name='ActivatedOn' and table_name  ='StoreCard')
ALTER TABLE StoreCard
ADD ActivatedOn DATETIME NULL,
	LostorStolenOn DATETIME NULL
GO
