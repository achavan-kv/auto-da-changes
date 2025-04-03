-- Script Comment : Update Setting
-- Script Name : 5230974_Belize_Update_View_PriceCalcView.sql
-- Created For	: Belize
-- Created By	: Nilesh
-- Created On	: 7/31/2018
-- Modified On	Modified By	Comment

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'PriceCalcView'
           AND xtype = 'V')
BEGIN 
DROP VIEW [Warranty].[PriceCalcView]
END
GO 
-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Warranty].[PriceCalcView]
AS
  SELECT * FROM [Warranty].PriceCalcView11  
    

GO


