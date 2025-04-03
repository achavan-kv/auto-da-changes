SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_GetScheduledLoadsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetScheduledLoadsSP]
GO

CREATE PROCEDURE dbo.DN_GetScheduledLoadsSP
    @branchno           smallint,
    @dateFrom           datetime,
    @dateTo             datetime,
    @printed            smallint,
    @loadNo             smallint,
    @withschedules      smallint = 0,
    @return             int OUTPUT

AS

    SET @return = 0            --initialise return code

    CREATE TABLE #scheds
        (origbr         smallint,
         branchno       smallint,
         datedel        datetime,
         loadno         smallint,
         deliveryslot   smallint,
         truckid        varchar (26),
         drivername     varchar (50) NOT NULL DEFAULT '',
         carrierNumber  varchar (20),
         printed        smallint,
         piclListCount  int)	 -- 69184 SC 26-7-07

    CREATE TABLE #PicklistCount
        (branchno       smallint,
         datedel        datetime,
         loadno         smallint,
         piclListCount  int )	-- 69184 SC 26-7-07


    INSERT INTO #scheds
        (origbr ,
         branchno ,
         datedel ,
         loadno ,
         deliveryslot ,
         truckid ,
         printed,
         piclListCount)
    SELECT
        ts.origbr,
        ts.branchno,
        ts.datedel,
        ts.loadno,
        ts.deliveryslot,
        ts.truckid,
        ts.printed,
        0
    FROM    Transptsched ts
    WHERE   ts.branchno = @branchno
    AND     (  (@dateTo = '1900-01-01' AND @dateFrom = '1900-01-01' AND ts.dateDel >= getdate()-1)
            OR (ts.dateDel >= @dateFrom-1 AND ts.dateDel <= @dateTo+1)
            )
    AND     ((@printed = 1 AND ts.printed =1) OR (@printed = 0 AND ts.printed =0) OR @printed = -1)
    AND     (@loadno = -1 OR ts.loadno = @loadno)
    and     ts.deliverystatus not in ('D','C') -- delivered or cancelled
    and     ( exists (select * from deliveryload DL, schedule
                      WHERE  DL.BranchNo = ts.branchno
                      AND    DL.datedel = ts.datedel
                      AND    DL.loadno = ts.loadno
                      AND    DL.BuffBranchNo = (CASE WHEN ISNULL(schedule.retstocklocn,0) = 0 THEN schedule.stocklocn ELSE schedule.retstocklocn END)
                      AND    DL.BuffNo = schedule.BuffNo )
             OR @withschedules = 0 )
    -- if loading up from delivery schedule screen (@withschedules=0 )
    -- then only loads which have schedules assigned to them will appear


    -- Add the driver name where we can find it in Transport table
    UPDATE #scheds
    SET    DriverName = t.DriverName,
		   carrierNumber = IsNull(t.carrierNumber, '')
    FROM   Transport t
    WHERE  t.TruckId = #scheds.TruckId
    

    INSERT INTO #PicklistCount
        (branchno ,
         datedel ,
         loadno ,
         piclListCount)
    SELECT  sc.BranchNo, 
            sc.datedel, 
            sc.loadno, 
            MAX(ts.picklistnumber) AS picklist
    FROM    schedule ts, #scheds sc, deliveryload dl
    WHERE   sc.datedel = ts.datedelplan
    AND     sc.loadno  = ts.loadno
    AND     dl.datedel = sc.datedel
    AND     dl.loadno  = sc.loadno
    AND     dl.BranchNo  = sc.BranchNo
    GROUP BY ts.picklistnumber, sc.datedel, sc.loadno, sc.BranchNo


    UPDATE  #scheds
    SET     piclListCount = temp.picklist
    FROM    (SELECT MAX(p.piclListCount) as picklist, p.loadno, p.branchno, p.datedel
             FROM   #PicklistCount p, #scheds s
             WHERE  s.loadno = p.loadno
             AND    s.branchno = p.branchno
             AND    s.datedel = p.datedel
             GROUP BY p.loadno, p.branchno, p.datedel) as temp, #scheds s
    WHERE   s.datedel = temp.datedel
    AND     s.loadno = temp.loadno
    AND     s.branchno = temp.BranchNo

    SELECT  * from #scheds
    
    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

