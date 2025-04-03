IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRWarrantyReturnValuesView]'))
	DROP VIEW [Merchandising].[WTRWarrantyReturnValuesView]

/****** Object:  View [Merchandising].[WTRWarrantyReturnValuesView]    Script Date: 2018-10-30 21:03:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRWarrantyReturnValuesView]
AS
    SELECT * FROM Merchandising.WTRWarrantyReturnValuesView_Temp
GO