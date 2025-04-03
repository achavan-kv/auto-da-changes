SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'RP3CurrentWeeklyMerchandisingView'
           AND xtype = 'V')
BEGIN 
DROP VIEW [Merchandising].[CurrentStockPriceByLocationView]
END
GO 

-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE VIEW [Merchandising].[CurrentStockPriceByLocationView]
AS

SELECT * from [Merchandising].[CurrentStockPriceByLocationView1] WITH(NoLock)

GO