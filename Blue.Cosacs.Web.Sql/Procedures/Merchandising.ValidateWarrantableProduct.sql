IF EXISTS (SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[Merchandising].[ValidateWarrantableProduct]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE [Merchandising].[ValidateWarrantableProduct]
END

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================
-- Procedure Name	: [Merchandising].[ValidateWarrantableProduct]
-- Author			: Ritesh J
-- Date Creation	: 23 Jan 2020
-- Description		: Check if SKU is present in [Warranty].[LinkProduct] table.
--					  If product SKU is present in [Warranty].[LinkProduct] table then it is not allow 
--					  for adding item in nonwarrantable item maintaince.
-- ================================================
CREATE PROCEDURE [Merchandising].[ValidateWarrantableProduct] 
	@SKU VARCHAR(20) NULL
AS
BEGIN	
	 IF EXISTS (SELECT 1 FROM [Warranty].[LinkProduct] WHERE [ItemNumber] = @SKU)
	 BEGIN
		SELECT CAST(0 AS BIT)
	 END
	ELSE
	 BEGIN
	   SELECT CAST(1 AS BIT)
	 END	 
END
