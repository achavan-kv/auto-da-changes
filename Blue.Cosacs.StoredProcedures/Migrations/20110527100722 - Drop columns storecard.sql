IF EXISTS (SELECT *
		   FROM syscolumns 
		   WHERE name = 'ActivatedOn'
		   AND OBJECT_NAME(id) = 'storecard')
BEGIN
	ALTER TABLE StoreCard
	DROP COLUMN ActivatedOn
END
GO


IF EXISTS (SELECT *
		   FROM syscolumns 
		   WHERE name = 'SecurityAnswer'
		   AND OBJECT_NAME(id) = 'storecard')
BEGIN
ALTER TABLE StoreCard
DROP COLUMN SecurityAnswer
END
GO

IF EXISTS (SELECT *
		   FROM syscolumns 
		   WHERE name = 'SecurityQuestion'
		   AND OBJECT_NAME(id) = 'storecard')
BEGIN
ALTER TABLE StoreCard
DROP COLUMN SecurityQuestion
END