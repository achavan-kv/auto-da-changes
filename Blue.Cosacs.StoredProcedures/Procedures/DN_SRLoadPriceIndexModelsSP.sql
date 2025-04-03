SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRLoadPriceIndexModelsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRLoadPriceIndexModelsSP]
GO


CREATE PROCEDURE dbo.DN_SRLoadPriceIndexModelsSP
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Load the list of Models within Suppliers from the Price Index Matrix
    SELECT DISTINCT Year, Supplier, Product
    FROM SR_PriceIndexMatrix
    ORDER BY Year, Supplier, Product

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
