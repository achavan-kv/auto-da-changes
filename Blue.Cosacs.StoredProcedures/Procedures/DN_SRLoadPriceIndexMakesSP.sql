SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRLoadPriceIndexMakesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRLoadPriceIndexMakesSP]
GO


CREATE PROCEDURE dbo.DN_SRLoadPriceIndexMakesSP
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Load the list of Suppliers from the Price Index Matrix
    
	SELECT DISTINCT Year, Supplier
    FROM SR_PriceIndexMatrix
    ORDER BY Year, Supplier
	
	/*
	SELECT Code, Codedescript 
	FROM Code 
	WHERE Category = 'SRSUPPLIER'
   
	*/
	SET @Return = @@error
    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


