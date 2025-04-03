IF EXISTS (
		SELECT 1
		FROM SYSOBJECTS
		WHERE id = object_id(N'[Merchandising].[GetProductMessagesByProductIds]')
			AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
BEGIN
	DROP PROCEDURE [Merchandising].[GetProductMessagesByProductIds]
END
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE Merchandising.GetProductMessagesByProductIds
	--===============================================================================================
	-- Version:		<000> 
	--===============================================================================================
	--================================================================================
	-- Project            : Blue.Cosacs.Web
	-- Name               : -
	-- Author             : Ritesh Joge
	-- Create Date	      : 25 Aug 2020
	-- Description        : 
	--			Gets product info having product ids equals to inputted product ids. Convert: ProductMessageView to SP
	-- Change Control
	-- --------------
	-- Date           By        Description
	-- ----           --        -----------
	-- 25/08/20       RJ	Gets product info having product ids equals to inputted product ids. Convert: ProductMessageView to SP
	--================================================================================
	@ProductIds dbo.IntTVP READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT p.Id
		,p.ProductType AS Type
		,ph.LegacyCode AS DepartmentCode
	FROM Merchandising.Product p
	LEFT JOIN Merchandising.ProductHierarchySummaryView ph
		ON ph.ProductId = p.Id
			AND ph.DepartmentCode IS NOT NULL
	WHERE p.Id IN (
			SELECT [Id]
			FROM @ProductIds
			)
END
GO

