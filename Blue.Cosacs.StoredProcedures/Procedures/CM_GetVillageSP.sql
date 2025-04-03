
--Script for Address Standardization CR2019 - 025

IF EXISTS (SELECT 'A' FROM sys.procedures where name = 'CM_GetVillageSP')
	DROP PROC [dbo].[CM_GetVillageSP]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sachin Wandhare
-- Create date: 06/03/2020
-- Description:	Get all Village/Town/City
-- History 
-- Version		Date		Modified By		Comment
-- 1.0			06/03/2020	Sachin Wandhare Address Standardization CR2019 - 025
-- =============================================

CREATE PROCEDURE [dbo].[CM_GetVillageSP] 
	@return int output
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @return = 0    --initialise return code

    SELECT Village FROM [dbo].[AddressMaster] WITH (NOLOCK) ORDER BY Village

    IF (@@error <> 0)
    BEGIN
        SET @return = @@error
    END

END