SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[DN_AccountGetControlNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetControlNoSP]
GO


CREATE PROCEDURE dbo.DN_AccountGetControlNoSP
                    @countryCode varchar(2),
                    @branchNo smallint,
                    @accountType varchar(3),
                    @return int OUTPUT

AS

    SET @return = 0

    -- First lock the required account type.
    -- Ready Finance and HP accounts share the same account
    -- number control, so lock them together.

    IF @accountType IN ('G','F','B','D','E','H','R')
    BEGIN
        UPDATE  acctnoctrl
        SET     hiallowed = hiallowed
        WHERE   branchno = @branchNo
        AND     acctcat IN ('G','F','B','D','E','H','R')

       SELECT  MAX(a.hiallocated) as hiAllocated,
                MIN(a.hiallowed) as hiAllowed,
                t.acctind
        FROM    acctnoctrl a,
                accttype t
        WHERE   a.branchno = @branchNo
        AND     a.acctcat IN ('G','F','B','D','E','H','R')
        AND     t.countrycode = @countryCode
        AND     t.accttype = @accountType
        GROUP BY t.acctind
    END
    ELSE
    BEGIN
        UPDATE  acctnoctrl
        SET     hiallowed = hiallowed
        WHERE   branchno = @branchNo
        AND     acctcat = @accountType

        SELECT  a.hiallocated,
                a.hiallowed,
                t.acctind
        FROM    acctnoctrl a,
                accttype t
        WHERE   a.branchno = @branchNo
        AND     a.acctcat = @accountType
        AND     t.countrycode = @countryCode
        AND     t.accttype = @accountType
    END

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
