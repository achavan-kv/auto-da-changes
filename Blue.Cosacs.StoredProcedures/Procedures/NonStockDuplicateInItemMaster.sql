IF EXISTS (SELECT 1 FROM sys.procedures WHERE name= 'NonStockDuplicateInItemMaster')
	DROP PROC NonStockDuplicateInItemMaster
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rahul Dubey, Zensar
-- Create date: 04/12/2019
-- Description:	Duplicate SKU - Nonstock and Item Master create same SKU code
-- =============================================
CREATE PROCEDURE NonStockDuplicateInItemMaster 
	-- Add the parameters for the stored procedure here
	@SKU VARCHAR(18)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(*) FROM Merchandising.Product WHERE SKU = @SKU
END
GO
