SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_GetDelfaultCommissionBasisSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetDelfaultCommissionBasisSP]
GO

CREATE PROCEDURE dbo.DN_GetDelfaultCommissionBasisSP
    @empeetype varchar (4),
    @return int OUTPUT

AS

    SET     @return = 0            --initialise return code

    SELECT  countrycode,
            statuscode,
            collecttype,
            collectionpercent,
            commnpercent,
            reppercent,
            allocpercent,
            reposspercent,
            minvalue,
            maxvalue,
            debitaccount,
            empeetype
    FROM    commnbasis
    WHERE   empeetype = @empeetype


    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO