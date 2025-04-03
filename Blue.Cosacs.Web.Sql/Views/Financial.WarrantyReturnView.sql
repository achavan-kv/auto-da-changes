IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Financial].[WarrantyReturnView]'))
	DROP VIEW [Financial].[WarrantyReturnView]

/****** Object:  View [Financial].[WarrantyReturnView]    Script Date: 10/30/2018 10:13:26 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Financial].[WarrantyReturnView]
AS
	SELECT * FROM [Financial].[WarrantyReturnView_Temp] WITH (NoLocK)


GO