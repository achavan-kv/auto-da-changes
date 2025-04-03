
--Script for Address Standardization CR2019 - 025


IF EXISTS (SELECT 'A' FROM sys.procedures where name = 'CM_GetRegionSP')
	DROP PROC [dbo].[CM_GetRegionSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Sachin Wandhare
-- Create date: 06/03/2020
-- Description:	Get Regions for a given Village/Town/City
-- History 
-- Version		Date		Modified By		Comment
-- 1.0			06/03/2020	Sachin Wandhare Address Standardization CR2019 - 025
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetRegionSP] 
	@Village VARCHAR(50),
	@return int output
AS
BEGIN
    
	SET NOCOUNT ON;
	SET @return = 0    --initialise return code

    SELECT DISTINCT Region 
	FROM [dbo].[AddressMaster] WITH (NOLOCK)
	WHERE Village = @Village
	ORDER BY Region

	IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END
END