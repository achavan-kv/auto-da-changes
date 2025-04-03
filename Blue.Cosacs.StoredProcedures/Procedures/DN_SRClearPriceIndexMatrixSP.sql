SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRClearPriceIndexMatrixSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRClearPriceIndexMatrixSP]
GO


CREATE PROCEDURE dbo.DN_SRClearPriceIndexMatrixSP
    @Return                 INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    TRUNCATE TABLE SR_PriceIndexMatrix

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

