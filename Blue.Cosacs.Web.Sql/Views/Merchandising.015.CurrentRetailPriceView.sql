SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'CurrentRetailPriceView'
           AND xtype = 'V')
BEGIN 
DROP VIEW [Merchandising].[CurrentRetailPriceView]
END
GO 
-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Merchandising].[CurrentRetailPriceView]
AS
  SELECT * FROM [Warranty].CurrentRetailPriceView1  With(NoLock)
    
GO







