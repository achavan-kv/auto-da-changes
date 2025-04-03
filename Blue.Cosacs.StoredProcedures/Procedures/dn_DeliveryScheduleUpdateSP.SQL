
if  exists (SELECT * FROM sysobjects  WHERE name =  'dn_DeliveryScheduleUpdateSP' )
drop procedure dn_DeliveryScheduleUpdateSP
go

create procedure dbo.dn_DeliveryScheduleUpdateSP

    @loadno              int, --delivery load number entered
    @buffno              int, -- either this for dn number
    @filter              int, -- Stock Location OR a Pick List Number
    @branchno            int, -- current branch
    @picklistnumber      int, --or picklist number (0 if doing buffno)
    @datedel             datetime,
    @return              int OUTPUT

AS
    set @return =@@error

    declare @isScheduled int
    set @isScheduled = -1

    if @picklistnumber = 0 or @picklistnumber is null
    BEGIN
        select  @isScheduled = COUNT(*)
        from    schedule sc
        where   sc.loadno > 0 AND
                @filter = (CASE WHEN ISNULL(sc.retstocklocn,0) = 0 THEN sc.stocklocn ELSE sc.retstocklocn END)
                AND  sc.buffNo = @buffno
			
    END
    ELSE
    BEGIN
        select  @isScheduled = COUNT(*)
        from    schedule sc
        where   sc.loadno > 0 AND
                sc.picklistbranchnumber = @filter AND
                sc.picklistnumber =  @picklistnumber
    END

    IF @isScheduled = 0
    BEGIN
	-- RD 13/09/06 UAT issue 147 Modified to add buffno as getting duplicate key error for identical replacement
        create table #scheds(   acctno char(12),
                                agrmtno int,
                                itemId int,
                                stocklocn smallint,
								buffno int)		

        if @picklistnumber = 0 or @picklistnumber is null
        begin
            INSERT INTO #scheds
                (acctno, agrmtno, itemId, stocklocn, buffno)
            SELECT
                L.AcctNo, L.AgrmtNo,L.ItemID,L.StockLocn, S.buffno
            FROM
            LINEITEM L, SCHEDULE S
            WHERE   L.AcctNo        = S.Acctno AND
                    L.AgrmtNo       = S.AgrmtNo AND
                    L.ItemID        = S.ItemID AND
                    --L.delnotebranch  = S.buffBranchNo AND
                    L.StockLocn     = S.StockLocn AND
                    @filter = (CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END)
                    AND S.BuffNo    = @BuffNo AND
                    L.Iskit         = 0

            set @return =@@error
        end
        else
        begin
            INSERT INTO #scheds
                (acctno, agrmtno, itemId, stocklocn, buffno)
            SELECT
                L.AcctNo, L.AgrmtNo,L.ItemId,L.StockLocn, S.buffno
            FROM
            LINEITEM L, SCHEDULE S
            WHERE   L.AcctNo        = S.Acctno AND
                    L.AgrmtNo       = S.AgrmtNo AND
                    L.ItemId        = S.ItemId AND
                    --L.stocklocn     = S.buffBranchNo AND
                    S.picklistbranchnumber = @filter AND
                    S.Picklistnumber = @Picklistnumber AND
                    L.Iskit         = 0

            set @return =@@error
        end

        IF @return = 0
        begin
            UPDATE lineitem
            SET   dateplANDel=@datedel
            FROM  #SCHEDS S, lineitem L
            WHERE   L.AcctNo        = S.Acctno AND
                    L.AgrmtNo       = S.AgrmtNo AND
                    L.ItemId        = S.ItemId AND
                    L.StockLocn     = S.StockLocn

            set @return =@@error
        end


        IF @return = 0 and (@picklistnumber = 0 or @picklistnumber is null)
        begin
            UPDATE schedule
            SET     datedelplan = @datedel,
                    LoadNo  = @LoadNo
            WHERE   @filter = (CASE WHEN ISNULL(retstocklocn,0) = 0 THEN stocklocn ELSE retstocklocn END)
            AND     buffno    = @BuffNo

            set @return = @@error
        end
        else
        IF @return = 0
        begin
            UPDATE  schedule 
            SET     datedelplan = @datedel,
                    LoadNo  = @LoadNo
            from    schedule sc1
            WHERE   sc1.picklistbranchnumber = @filter
            AND     Picklistnumber           = @Picklistnumber
        end


        INSERT into deliveryload
            (origbr, branchno, datedel, loadno, buffbranchno, buffno)
        SELECT DISTINCT origbr, @branchno, @datedel, loadno,
                        (CASE WHEN ISNULL(s1.retstocklocn,0) = 0 THEN s1.stocklocn ELSE s1.retstocklocn END),
						s1.buffno
        from schedule s1 ,#scheds s2
        where   s1.AcctNo        = s2.Acctno AND
                s1.AgrmtNo       = s2.AgrmtNo AND
                s1.ItemId        = s2.ItemId AND
                s1.StockLocn     = s2.StockLocn AND
		s1.Buffno	 = s2.Buffno

        set @return = @@error
    END

GO

