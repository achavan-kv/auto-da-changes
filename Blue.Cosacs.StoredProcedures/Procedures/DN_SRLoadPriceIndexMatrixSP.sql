SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRLoadPriceIndexMatrixSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRLoadPriceIndexMatrixSP]
GO


CREATE PROCEDURE dbo.DN_SRLoadPriceIndexMatrixSP
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_SRLoadPriceIndexMatrixSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Loads Price Index Matrix.
-- Author       : ??
-- Date         : ??
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/11/10  jec CR1030 Set values for Labour and Additional Costs for Blank Entry
************************************************************************************************************/
--Parameters
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Load the list of Suppliers and Products
    SELECT  Supplier,
            Product,
            Year,
            CONVERT(VARCHAR,Year*12-11) + ' - ' + CONVERT(VARCHAR,Year*12) AS Month,
            PartType,
            PartPercent,
            PartLimit,
            LabourPercent,
            LabourLimit,
            AdditionalPercent,
            AdditionalLimit
    FROM SR_PriceIndexMatrix
    -- And include a blank entry
    UNION
    SELECT  '', '', 0, '', '', 0, 0, 0, 99, 0, 99		--CR1030 
    ORDER BY Supplier, Product, Year, PartType

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End End