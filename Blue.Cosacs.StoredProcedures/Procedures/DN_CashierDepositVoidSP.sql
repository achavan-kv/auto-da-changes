SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select  * from dbo.sysobjects
           where   id = object_id('[dbo].[DN_CashierDepositVoidSP]')
           and     OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierDepositVoidSP]
GO


CREATE PROCEDURE dbo.DN_CashierDepositVoidSP
    @depositid int,
    @datevoided datetime,
    @voidedby int,
    @reverse bit,
    @return int OUTPUT

AS
    -- If this is a transfer from the safe to a bank that is being voided then
    -- there are actually two records involved. Therefore if the empeeno is -1
    -- we must also void the record with the same date but an opposite amount.

    SET         @return = 0            --initialise return code

    DECLARE     @datedeposit    datetime
    DECLARE     @empeeno        int
    DECLARE     @depositvalue   money

    SELECT  @datedeposit = datedeposit,
            @empeeno = empeeno,
            @depositvalue = depositvalue
    FROM    CashierDeposits
    WHERE   depositid = @depositid

    IF(@reverse = 0)
    BEGIN
        UPDATE  CashierDeposits
        SET     voided = 'Y',
                empeenovoided = @voidedby,
                datevoided = @datevoided
        WHERE   depositid = @depositid

        IF(@empeeno = -1)
        BEGIN
            UPDATE  CashierDeposits
            SET     voided = 'Y',
                    empeenovoided = @voidedby,
                    datevoided = @datevoided
            WHERE   datedeposit = @datedeposit
            AND     empeeno = -1
            AND     depositvalue = -@depositvalue
        END
    END
    ELSE
    BEGIN
        -- The Deposit being reversed has '1' in the 'IsReversed' column to
        -- indicate this deposit has been reversed. It cannot be reversed again.
        UPDATE CashierDeposits
        SET    IsReversed = 1
        WHERE  depositid = @depositid

        -- The reversal has '2' in the 'IsReversed' column to indicate that this
        -- is a reversal. A reversal can be reversed again (cancelled out) by the user.
        INSERT
        INTO    cashierdeposits
                (datedeposit, code, runno, empeeno, empeenoentered,
                cashiertotalid, empeenovoided, voided, datevoided,
                depositvalue, branchno, reference, paymethod, includeincashiertotals,
                IsReversed)
        SELECT  getdate(), code, 0, empeeno, empeenoentered,
                cashiertotalid, empeenovoided, voided, datevoided,
                -@depositvalue, branchno, reference, paymethod, includeincashiertotals,
                2
        FROM    cashierdeposits
        WHERE   depositid = @depositid


        IF(@empeeno = -1)
        BEGIN
            UPDATE CashierDeposits
            SET    IsReversed = 1
            WHERE  datedeposit = @datedeposit
            AND    empeeno = -1
            AND    depositvalue = @depositvalue

            INSERT
            INTO    cashierdeposits
                    (datedeposit, code, runno, empeeno, empeenoentered,
                    cashiertotalid, empeenovoided, voided, datevoided,
                    depositvalue, branchno, reference, paymethod, includeincashiertotals,
                    IsReversed)
            SELECT  getdate(), code, 0, empeeno, empeenoentered,
                    cashiertotalid, empeenovoided, voided, datevoided,
                    @depositvalue, branchno, reference, paymethod, includeincashiertotals,
                    2
            FROM    cashierdeposits
            WHERE   datedeposit = @datedeposit
            AND     empeeno = -1
            AND     depositvalue = -@depositvalue
        END
    END

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

