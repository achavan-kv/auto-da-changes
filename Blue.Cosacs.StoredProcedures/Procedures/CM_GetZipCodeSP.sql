--Script for Address Standardization CR2019 - 025

IF EXISTS (SELECT 'A' FROM sys.procedures where name = 'CM_GetZipCodeSP')
	DROP PROC [dbo].[CM_GetZipCodeSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sachin Wandhare
-- Create date: 06/03/2020
-- Description:	Get ZipCode for a given Region and Village
-- History 
-- Version		Date		Modified By		Comment
-- 1.0			06/03/2020	Sachin Wandhare	Standardization CR2019 - 025
-- =============================================
CREATE PROCEDURE [dbo].[CM_GetZipCodeSP] 
	@Region VARCHAR(50),
	@Village VARCHAR(50),
	@return int output
AS
BEGIN
    
	SET NOCOUNT ON;
	SET @return = 0    --initialise return code

    SELECT ZipCode 
	FROM [dbo].[AddressMaster] WITH (NOLOCK)
	WHERE Region = @Region AND Village = @Village
	
	IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END
END