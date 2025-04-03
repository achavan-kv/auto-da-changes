SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'CurrentStockRetailPriceView'
           AND xtype = 'V')
BEGIN 
DROP VIEW [Merchandising].[CurrentStockRetailPriceView]
END
GO 

-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Merchandising].[CurrentStockRetailPriceView]
AS

SELECT * FROM [Merchandising].[CurrentStockRetailPriceView1]


GO


