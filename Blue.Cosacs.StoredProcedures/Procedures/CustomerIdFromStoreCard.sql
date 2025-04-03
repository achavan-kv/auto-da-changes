
IF EXISTS (SELECT * FROM sysobjects
               WHERE NAME = 'CustomerIdFromStoreCard'
               AND xtype = 'P')
BEGIN
	DROP PROCEDURE CustomerIdFromStoreCard
END
GO


