SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_SRDeletePartListSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SRDeletePartListSP]
GO


CREATE PROCEDURE dbo.DN_SRDeletePartListSP
    @ServiceRequestNo   INTEGER,
    @Return             INTEGER OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    -- Delete the part list for this SR
    DELETE FROM SR_PartListResolved
    WHERE  ServiceRequestNo = @ServiceRequestNo


    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

