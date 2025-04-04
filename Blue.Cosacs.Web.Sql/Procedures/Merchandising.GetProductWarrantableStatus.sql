IF EXISTS (SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[Merchandising].[GetProductWarrantableStatus]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
    DROP PROCEDURE [Merchandising].[GetProductWarrantableStatus]
END

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ================================================
-- Procedure Name	: [Merchandising].[ValidProductForNonWarrantable]
-- Author			: Ritesh J
-- Date Creation	: 23 Jan 2020
-- Description		: Get product warrantable status from [Merchandising].[Product]. 
--					  This status is used to sell the product warranty in windows and web POS
-- ================================================
CREATE PROCEDURE [Merchandising].[GetProductWarrantableStatus]  
	@SKU VARCHAR(20) NULL
AS
BEGIN
	
	DECLARE @return BIT = 1;
		
	SELECT	@return = ISNULL([Warrantable],0) 
	FROM	[Merchandising].[Product] WITH(NOLOCK) 
	WHERE	[SKU] = @SKU
	
	SELECT @return
END
