SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[DN_AccountUpdateControlNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountUpdateControlNoSP]
GO

CREATE PROCEDURE dbo.DN_AccountUpdateControlNoSP
                    @branchNo smallint,
                    @accountType varchar(1),
                    @hiAllocated int,
                    @hiAllowed int,
                    @return int OUTPUT

AS
    DECLARE @accountType1 VARCHAR (1)

    -- Ready Finance and HP accounts share the same account
    -- number control, so update them together.

    SET @return = 0

    IF @accountType IN ('G','F','B','D','E','H','R')
    BEGIN
        UPDATE  acctnoctrl
        SET     hiallocated = @hiAllocated
        WHERE   branchno = @branchNo
        AND     acctcat IN ('G','F','B','D','E','H','R')
        AND     @hiAllocated <= hiAllowed
    END
    ELSE
    BEGIN
        UPDATE  acctnoctrl
        SET     hiallocated = @hiAllocated
        WHERE   branchno = @branchNo
        AND     acctcat = @accountType
        AND     @hiAllocated <= hiAllowed
    END

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
