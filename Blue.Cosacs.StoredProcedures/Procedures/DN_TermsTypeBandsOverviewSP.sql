SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeBandsOverviewSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeBandsOverviewSP]
GO



CREATE PROCEDURE dbo.DN_TermsTypeBandsOverviewSP
    @Return         INTEGER OUTPUT

AS

    SET NOCOUNT ON
    SET @Return = 0

    SELECT TermsType, Description, Band, ServPcent ServiceCharge, InsPcent, InsIncluded
    FROM   TermsTypeAllBands
    ORDER BY TermsType, Band

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

