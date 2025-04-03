IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='StoreCardRateLoadAll' )
DROP PROCEDURE StoreCardRateLoadAll
GO
CREATE PROCEDURE StoreCardRateLoadAll AS 

SELECT Id,
	   [$Version] AS [Version],
	   [$IsDeleted] AS [IsDeleted],
	   [NAME],
	   ratefixed
	    FROM dbo.StoreCardRate 
GO 

