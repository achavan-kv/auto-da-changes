IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[TestGetRandPickingView]'))
	DROP VIEW [dbo].TestGetRandPickingView
GO

CREATE VIEW dbo.TestGetRandPickingView
AS
SELECT TOP 1 id
FROM warehouse.picking
ORDER BY NEWID()
GO


