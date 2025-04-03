
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_MPRCalcSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_MPRCalcSP
END
GO

CREATE PROCEDURE DN_MPRCalcSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS dotNET ? 2004 Strategic Thought Ltd.
-- File Name    : DN_MPRCalcSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : MPR calculation for CoSACS
-- Author       : D Richardson
-- Date         : 26 April 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @AcctNo             char(12),
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
  set nocount on
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
      -- what we are doing here is to try and work out the actual instalment that the variable instalment
      -- kicks in at so that we can accurately determine the monthly interest rate
    declare @variableT table  (acctno char(12),
            instalorder	smallint,
            instalment	money,
            instalmentnumber	smallint,
            datefrom	datetime,
            servicecharge	money,
            dateto	datetime,
            startinstal smallint)
      insert into @variableT
        (acctno,instalorder,instalment,instalmentnumber,
           datefrom,servicecharge,dateto,startinstal)
        select  acctno,instalorder,instalment,instalmentnumber,
               datefrom,servicecharge,dateto,0
      from instalmentvariable where acctno = @acctno
      declare @counter int,@variable money
      set @counter = 2
      update @variableT set startinstal = instalorder + instalmentnumber where instalorder = 1 
      while 1 =1 
      begin
           update @variableT
           set startinstal = startinstal + v.instalorder * v.instalmentnumber
           from @variableT v
           where v.instalorder = (@counter -1) and  instalorder = @counter
          if @@rowcount= 0
           break

        set @counter = @counter + 1
     end
            -- now we are looping round to try and get the agreement total to zero after taking off instalments
            -- and adding on a rate of interest which we have initially guessed at.
            -- by reducing the amount and sign of the difference that we take off we should eventually get to zero.
            
            SET @Balance = @InitBalance
            SET @MPR = @CurMPR
            SET @CurInstalNo = 1
            WHILE @CurInstalNo <= @InstalNo
            BEGIN

                IF @CurInstalNo > @Deferred AND @CurInstalNo < @InstalNo
                    -- Normal payment with balance reducing, but interest applied
                  begin
                    select @variable =instalment from @variableT where startinstal >=@instalno
                    if @variable !=0 
                       set @instalamount = @variable
                    SET @Balance = @Balance - @InstalAmount + (@Balance * @CurMPR)
                    SET @Return = @@ERROR 
                    IF @Return != 0 
                    begin
                        RETURN @Return
                    end
                  end
                ELSE IF @CurInstalNo = @Deferred
                  begin
                    -- Account is deferred with balance increasing
                    SET @Balance = @Balance + (@Balance * @CurMPR)
                    SET @Return = @@ERROR 
                    IF @Return != 0 
                    begin
                        RETURN @Return
                    end
                   end
                ELSE IF @CurInstalNo = @InstalNo
                  begin
                    -- Final instalment
                    SET @Balance = @Balance - @FinalInstalAmount + (@Balance * @CurMPR)
                    SET @Return = @@ERROR 
                    IF @Return != 0 
                    begin
                        RETURN @Return
                    end
                  end
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
GO