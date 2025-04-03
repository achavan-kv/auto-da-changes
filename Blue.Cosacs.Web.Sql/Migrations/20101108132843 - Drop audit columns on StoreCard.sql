ALTER TABLE dbo.StoreCardRate
	DROP COLUMN [$DeletedBy], [$DeletedOn], [$CreatedBy], [$CreatedOn], [$LastUpdatedBy], [$LastUpdatedOn]
GO

ALTER TABLE dbo.StoreCard
	DROP COLUMN [$DeletedBy], [$DeletedOn], [$CreatedBy], [$CreatedOn], [$LastUpdatedBy], [$LastUpdatedOn]
GO
