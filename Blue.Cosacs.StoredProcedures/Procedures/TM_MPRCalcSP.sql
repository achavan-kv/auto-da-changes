
GO
/****** Object:  StoredProcedure [dbo].[TM_MPRCalcSP]    Script Date: 03/06/2008 09:32:14 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[TM_MPRCalcSP]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TM_MPRCalcSP]
GO
/****** Object:  StoredProcedure [dbo].[TM_MPRCalcSP]    Script Date: 03/06/2008 09:32:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[TM_MPRCalcSP]

--------------------------------------------------------------------------------
--
-- Project      : CoSACS dotNET © 2004 Strategic Thought Ltd.
-- File Name    : TM_MPRCalcSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : MPR calculation for CoSACS to Tallyman interface
-- Author       : D Richardson
-- Date         : 19 Nov 2004
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/02/10 jec Malaysia 4.3 merge.
--------------------------------------------------------------------------------

    -- Parameters
    @AcctNo             CHAR(12),
    @MPR                FLOAT       OUTPUT,
    @Return             INTEGER     OUTPUT

AS DECLARE
    -- Local variables
    @CurMPR             FLOAT,
    @InitBalance        FLOAT,
    @Balance            FLOAT,
    @InstalAmount       MONEY,
    @FinalInstalAmount  MONEY,
    @InstalNo           SMALLINT,
    @Deferred           SMALLINT,
    @CurInstalNo        SMALLINT,
    @MaxLoops           INTEGER,
    @Diff               FLOAT

BEGIN

--print 'AcctNo = ' + CONVERT(VARCHAR,@AcctNo)

    SELECT  @CurMPR = CASE WHEN AgrmtTotal - ISNULL(Deposit,0) > 0
                           THEN ISNULL(ServiceChg/((AgrmtTotal - ISNULL(Deposit,0))/100),0)
                           ELSE 0
                      END,
            @InitBalance = ISNULL(AgrmtTotal,0) - ISNULL(ServiceChg,0) - ISNULL(Deposit,0)
    FROM    Agreement
    WHERE   AcctNo = @AcctNo

    SET @Return = @@ERROR IF @Return != 0 RETURN @Return

    SELECT  @InstalAmount       = ISNULL(InstalAmount,0),
            @FinalInstalAmount  = ISNULL(FinInstalAmt,0),
            @InstalNo           = ISNULL(InstalNo,0),
            @Deferred           = ISNULL(MthsIntFree,0)
    FROM    InstalPlan
    WHERE   AcctNo = @AcctNo

    SET @Return = @@ERROR IF @Return != 0 RETURN @Return

    SET @MPR = 0

    IF  @InstalAmount >= 0.01
    AND @InstalNo > 0
    AND @CurMPR > 0
    AND @InitBalance >= 0.01
    BEGIN

        SET @Balance = @InitBalance
        SET @CurMPR = @CurMPR / @InstalNo / 100
        SET @MaxLoops = 0
        SET @Diff = 0.001
        WHILE ABS(@Balance) >= 0.001 AND @MaxLoops < 500
        BEGIN

--print 'MPR = ' + CONVERT(VARCHAR,@CurMPR)
--print 'Diff = ' + CONVERT(VARCHAR,@Diff)

            SET @Balance = @InitBalance
            SET @MPR = @CurMPR
            SET @CurInstalNo = 1
            WHILE @CurInstalNo <= @InstalNo
            BEGIN

                IF @CurInstalNo > @Deferred AND @CurInstalNo < @InstalNo
                    -- Normal payment with balance reducing, but interest applied
                    SET @Balance = @Balance - @InstalAmount + (@Balance * @CurMPR)
                    SET @Return = @@ERROR IF @Return != 0 RETURN @Return
                ELSE IF @CurInstalNo = @Deferred
                    -- Account is deferred with balance increasing
                    SET @Balance = @Balance + (@Balance * @CurMPR)
                    SET @Return = @@ERROR IF @Return != 0 RETURN @Return
                ELSE IF @CurInstalNo = @InstalNo
                    -- Final instalment
                    SET @Balance = @Balance - @FinalInstalAmount + (@Balance * @CurMPR)
                    SET @Return = @@ERROR IF @Return != 0 RETURN @Return

                SET @CurInstalNo = @CurInstalNo + 1
            END

--print 'Bal = ' + CONVERT(VARCHAR,@balance)

            IF (@Balance < 0 AND @Diff < 0) OR (@Balance > 0 AND @Diff > 0)
                -- MPR too high or too low
                SET @Diff = -@Diff / 2

            SET @CurMPR = @CurMPR + @Diff
            SET @MaxLoops = @MaxLoops + 1

        END
    END

    SET @Return = 0
    RETURN @Return
END

go
-- End End End End End End End End End End End End End End End End End End End End End End
