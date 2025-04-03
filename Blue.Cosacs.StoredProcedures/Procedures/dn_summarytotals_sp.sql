SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (SELECT * FROM dbo.sysobjects
WHERE id = object_id('[dbo].[dn_summarytotals_sp]') AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_summarytotals_sp]
GO

CREATE PROCEDURE dn_summarytotals_sp
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_summarytotals_sp.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Summary Totals
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the Update Control summary totals to be displayed in the 
-- Summary Update Control Report - Total screen.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/10/087  jec UAT543 Only retrieve one row for Settled Accounts.
-- 17/02/12   IP  #9423 - CR8262 - Include StoreCard account type totals
-- ================================================

@runno      int,            -- batch/run number must be supplied
@branchno   smallint = 0,   -- supply branch number 0 for all.
@type       varchar(12),    -- 'INTEREST' 'FINANCE' 'OPEN'
@return     int = 0 OUTPUT

AS

SET @return = 0
SET NOCOUNT ON
declare @minbranchno    smallint
declare @maxbranchno    smallint
declare @rowCounter     int
declare @prev_runno     int

if @branchno = 0
begin
    set @minbranchno = -999
    set @maxbranchno = 1000
end
else
begin
    set @minbranchno = @branchno
    set @maxbranchno = @branchno
end

create table #summary
    (description    varchar(100),
     hpvalue        money,
     cashvalue      money,
     specialvalue   money,
     storecardvalue	money,									--IP - 17/02/12 - #9423 -CR8262
     totalvalue     money,
     orderby        smallint,
     source         varchar(12))


IF @TYPE = 'FINANCE'
BEGIN

    --
    -- Opening Balance (excludes unsettled interest) = FACT2000
    -- SECOND line of report retrieved first
    --
    INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Opening Balance (excludes unsettled interest) = FACT2000',ISNULL(sum(value),0),0,0,0,20,'' --counttype2
    (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)									--IP - 17/02/12 - #9423 - CR8262			
    SELECT 'Opening Balance (excludes unsettled interest) = FACT2000',ISNULL(sum(value),0),0,0,0,0,20,'' --counttype2			--IP - 17/02/12 - #9423 - CR8262
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'BALANCE B/F'
    AND branchno between @minbranchno AND @maxbranchno
    --GROUP BY counttype2

    update #summary set cashvalue =
        ISNULL((SELECT isnull(sum(value),0)   FROM interfacevalue i
                WHERE i.interface = 'UPDSMRY'
                AND i.runno = @runno
                AND i.accttype = 'CASH'
                AND i.branchno between @minbranchno AND @maxbranchno
                AND i.counttype1 = 'BALANCE B/F'
                AND #summary.source = counttype2
                AND #summary.description = 'Opening Balance (excludes unsettled interest) = FACT2000'),0)

    update #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue i
                WHERE i.interface = 'UPDSMRY'
                AND i.runno = @runno
                AND i.accttype = 'SPECIAL'
                AND i.branchno between @minbranchno AND @maxbranchno
                AND i.counttype1 = 'BALANCE B/F'
                AND #summary.source = counttype2
                AND #summary.description = 'Opening Balance (excludes unsettled interest) = FACT2000'),0)

	--IP - 17/02/12 - #9423 - CR8262
    update #summary set storecardvalue =
    ISNULL((SELECT sum(value)   FROM interfacevalue i
            WHERE i.interface = 'UPDSMRY'
            AND i.runno = @runno
            AND i.accttype = 'STORECARD'
            AND i.branchno between @minbranchno AND @maxbranchno
            AND i.counttype1 = 'BALANCE B/F'
            AND #summary.source = counttype2
            AND #summary.description = 'Opening Balance (excludes unsettled interest) = FACT2000'),0)
    --
    -- Opening Balance (includes unsettled interest) = Report 11
    -- FIRST line of report calculated from second line
    --
    -- UAT 28 Add the unsettled interest for the previous run to give
    -- the Report 11 opening balance
    SELECT @prev_runno = max(runno)
    FROM   interfacecontrol
    WHERE  interface = 'UPDSMRY'
    AND    result = 'P'
    AND    runno < @runno

    IF ISNULL(@prev_runno,0) > 0
    BEGIN
        INSERT INTO #summary
        --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
        --SELECT 'Opening Balance (includes unsettled interest) = Report 11', hpvalue, cashvalue, specialvalue, totalvalue, 10, source
            (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)													--IP - 17/02/12 - #9423 - CR8262
        SELECT 'Opening Balance (includes unsettled interest) = Report 11', hpvalue, cashvalue, specialvalue,storecardvalue, totalvalue, 10, source		--IP - 17/02/12 - #9423 - CR8262
        FROM   #summary
        WHERE  description = 'Opening Balance (excludes unsettled interest) = FACT2000'

        UPDATE #summary set hpvalue = hpvalue +
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'HP'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance (includes unsettled interest) = Report 11'

        UPDATE #summary set cashvalue = cashvalue +
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'CASH'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance (includes unsettled interest) = Report 11'

        UPDATE #summary set specialvalue = specialvalue +
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'SPECIAL'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance (includes unsettled interest) = Report 11'
        
         --IP - 17/02/12 - #9423 - CR8262
         UPDATE #summary set storecardvalue = storecardvalue +
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'STORECARD'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance (includes unsettled interest) = Report 11'
    END


    --
    -- Deliveries
    ----
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES ('Deliveries',0,0,0,0,30,'COSACS')
    --INSERT INTO #summary
    --   (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES ('Deliveries',0,0,0,0,30,'COASTER')
    
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)					--IP - 17/02/12 - #9423 - CR8262
    VALUES ('Deliveries',0,0,0,0,0,30,'COSACS')
    INSERT INTO #summary
       (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)					--IP - 17/02/12 - #9423 - CR8262			
    VALUES ('Deliveries',0,0,0,0,0,30,'COASTER')

    UPDATE #summary set hpvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'HP'
                AND counttype1 = 'DELIVERY'
                AND branchno between @minbranchno AND @maxbranchno
                AND #summary.source = counttype2), 0)
    WHERE #summary.description = 'Deliveries'

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'DELIVERY'
                AND #summary.source = counttype2), 0)
    WHERE #summary.description = 'Deliveries'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'DELIVERY'
                AND #summary.source = counttype2), 0)
    WHERE #summary.description = 'Deliveries'

	--IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'DELIVERY'
                AND #summary.source = counttype2), 0)
    WHERE #summary.description = 'Deliveries'
    
    --
    -- Receipts
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES ('Receipts',0,0,0,0,40,'COSACS')
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES ('Receipts',0,0,0,0,40,'COASTER')
    
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue,storecardvalue,totalvalue,orderby, source)					--IP - 17/02/12 - #9423 - CR8262
    VALUES ('Receipts',0,0,0,0,0,40,'COSACS')
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)					--IP - 17/02/12 - #9423 - CR8262
    VALUES ('Receipts',0,0,0,0,0,40,'COASTER')

    UPDATE #summary set hpvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'HP'
                AND counttype1 = 'RECEIPT'
                AND branchno between @minbranchno AND @maxbranchno
                AND #summary.source = counttype2), 0)
    WHERE #summary.description = 'Receipts'

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'RECEIPT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Receipts'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'RECEIPT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Receipts'

	--IP - 17/02/12 - #9423 - CR8262
	UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'RECEIPT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Receipts'
    
    --
    -- Adjustments
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES ('Adjustments',0,0,0,0,50,'COSACS')
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES ('Adjustments',0,0,0,0,50,'COASTER')
    
    
    INSERT INTO #summary
          (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)				--IP - 17/02/12 - #9423 - CR8262
    VALUES ('Adjustments',0,0,0,0,0,50,'COSACS')																	--IP - 17/02/12 - #9423 - CR8262
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)					--IP - 17/02/12 - #9423 - CR8262
    VALUES ('Adjustments',0,0,0,0,0,50,'COASTER')																	--IP - 17/02/12 - #9423 - CR8262
    

    UPDATE #summary set hpvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'HP'
                AND counttype1 = 'ADJUSTMENT'
                AND branchno between @minbranchno AND @maxbranchno
                AND #summary.source = counttype2), 0)
    WHERE #summary.description = 'Adjustments'

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ADJUSTMENT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Adjustments'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ADJUSTMENT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Adjustments'

	--IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ADJUSTMENT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Adjustments'

    --
    -- Transferred to Settled Interest
    --
    -- Make sure at least one row appears for 'COSACS' source even if it is zero

    --INSERT INTO #summary
    --  (description ,  hpvalue ,cashvalue , specialvalue , totalvalue,orderby, source)
    --  values ('Transferred to Settled Interest',0,0,0,0 ,60,'COSACS')
    
      --IP - 17/02/12 - #9423 - CR8262
      INSERT INTO #summary
       (description ,  hpvalue ,cashvalue , specialvalue , storecardvalue, totalvalue,orderby, source)					
      values ('Transferred to Settled Interest',0,0,0,0,0 ,60,'COSACS')	

    UPDATE #summary set hpvalue = ISNULL((SELECT sum(value)   FROM interfacevalue 
        WHERE interface ='UPDSMRY' 
        AND runno = @runno
        AND accttype ='HP' 
        AND branchno between @minbranchno AND @maxbranchno   
        AND counttype1  ='INTONSETT'
--      AND #summary.source =counttype2
        ),0)
    WHERE	#summary.description = 'Transferred to Settled Interest'


/*KEF 68001 - commented out as doing above insert and update now to enusre there's always a row inserted
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'Transferred to Settled Interest',ISNULL(sum(value),0),0,0,0,60,'COSACS'
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'INTONSETT'
    AND counttype2 = 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno

    -- Add other rows for other sources
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'Transferred to Settled Interest',sum(value),0,0,0,60,counttype2
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'INTONSETT'
    AND counttype2 != 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno
    GROUP BY counttype2
*/
    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'Transferred to Settled Interest'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
    WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'Transferred to Settled Interest'

	--IP - 17/02/12 - #9423 - CR8262
	UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
    WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'Transferred to Settled Interest'
    
    --
    -- Closing Balance (including settled) = FACT2000
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Closing Balance (including settled) = FACT2000',ISNULL(sum(value),0),0,0,0,95, '' --counttype2
    
     --IP - 17/02/12 - #9423 - CR8262
    INSERT INTO #summary
       (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)							
    SELECT 'Closing Balance (including settled) = FACT2000',ISNULL(sum(value),0),0,0,0,0,95, '' --counttype2	
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'BALANCE C/F'
    AND branchno between @minbranchno AND @maxbranchno
    --GROUP BY counttype2

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'BALANCE C/F'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Closing Balance (including settled) = FACT2000'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'BALANCE C/F'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Closing Balance (including settled) = FACT2000'

	--IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'BALANCE C/F'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Closing Balance (including settled) = FACT2000'
    
    -- UAT 28 Include Unsettled Interest on Financial Statement as well
    --
    -- Interest on unsettled accounts (carried forward)
    --
--KEF 68001 changed to ensure always insert a blank row then update
    --INSERT INTO #summary
    --  (description ,  hpvalue ,cashvalue , specialvalue , totalvalue,orderby, source)
    --  values ('Interest on unsettled accounts (carried forward)',0,0,0,0 ,80,'')
    
    --IP - 17/02/12 - #9423 - CR8262
     INSERT INTO #summary
     (description ,  hpvalue ,cashvalue , specialvalue , storecardvalue, totalvalue,orderby, source)					
      values ('Interest on unsettled accounts (carried forward)',0,0,0,0,0 ,80,'')	
    
    UPDATE #summary set hpvalue = ISNULL((SELECT sum(value)   FROM interfacevalue 
      WHERE interface ='UPDSMRY' 
      AND runno = @runno
      AND accttype ='HP' 
      AND branchno between @minbranchno AND @maxbranchno   
      AND counttype1  ='TOTUNINT'
    --  AND #summary.source =counttype2
    ),0)
      WHERE	#summary.description = 'Interest on unsettled accounts (carried forward)'

/*    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'Interest on unsettled accounts (carried forward)',ISNULL(sum(value),0),0,0,0,80,'COSACS' --counttype2
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'TOTUNINT'
    AND branchno between @minbranchno AND @maxbranchno
    --  GROUP BY counttype2
*/
    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'TOTUNINT'),0)
    WHERE #summary.description = 'Interest on unsettled accounts (carried forward)'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'TOTUNINT'),0)
    WHERE #summary.description = 'Interest on unsettled accounts (carried forward)'

	--IP - 17/02/12 - #9423 - CR8262
	UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'TOTUNINT'),0)
    WHERE #summary.description = 'Interest on unsettled accounts (carried forward)'
    --
    -- Closing Balance = Report 11
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Closing Balance = Report 11', s1.hpvalue, s1.cashvalue, s1.specialvalue, 0, 90,''
    
     --IP - 17/02/12 - #9423 - CR8262
     INSERT INTO #summary
         (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)				
    SELECT 'Closing Balance = Report 11', s1.hpvalue, s1.cashvalue, s1.specialvalue,s1.storecardvalue, 0, 90,''		
    FROM   #summary s1
    WHERE  s1.description = 'Closing Balance (including settled) = FACT2000'

    UPDATE #summary set hpvalue = hpvalue +
        ISNULL((SELECT sum(s2.hpvalue)
                FROM   #summary s2
                WHERE  s2.description = 'Interest on unsettled accounts (carried forward)'),0)
    WHERE  #summary.description = 'Closing Balance = Report 11'

    UPDATE #summary set cashvalue = cashvalue +
        ISNULL((SELECT sum(s2.cashvalue)
                FROM   #summary s2
                WHERE  s2.description = 'Interest on unsettled accounts (carried forward)'),0)
    WHERE  #summary.description = 'Closing Balance = Report 11'


    UPDATE #summary set specialvalue = specialvalue +
        ISNULL((SELECT sum(s2.specialvalue)
                FROM   #summary s2
                WHERE  s2.description = 'Interest on unsettled accounts (carried forward)'),0)
    WHERE  #summary.description = 'Closing Balance = Report 11'
    
     --IP - 17/02/12 - #9423 - CR8262 
     UPDATE #summary set storecardvalue = storecardvalue +
        ISNULL((SELECT sum(s2.storecardvalue)
                FROM   #summary s2
                WHERE  s2.description = 'Interest on unsettled accounts (carried forward)'),0)
    WHERE  #summary.description = 'Closing Balance = Report 11'

END


-- NOW DOING opened accounts
IF @TYPE = 'OPEN'
BEGIN

    --
    -- Opening Balance
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Opening Balance',ISNULL(sum(countvalue),0),0,0,0,10,''
    
    --IP - 17/02/12 - #9342 - CR8262
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue,totalvalue,orderby, source)
    SELECT 'Opening Balance',ISNULL(sum(countvalue),0),0,0,0,0,10,''
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'ACCOUNTS B/F'
    AND branchno between @minbranchno AND @maxbranchno

    update #summary set cashvalue =
        ISNULL((SELECT isnull(sum(countvalue),0)   FROM interfacevalue i
                WHERE i.interface = 'UPDSMRY'
                AND i.runno = @runno
                AND #summary.description = 'Opening Balance'
                AND i.accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND i.counttype1 = 'ACCOUNTS B/F'
                AND #summary.source = counttype2),0)
    where #summary.description = 'Opening Balance'

    update #summary set specialvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND description = 'Opening Balance'
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ACCOUNTS B/F'
                AND #summary.source = counttype2),0)
    where #summary.description = 'Opening Balance'

	--IP - 17/02/12 - #9423 - CR8262
	update #summary set storecardvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND description = 'Opening Balance'
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ACCOUNTS B/F'
                AND #summary.source = counttype2),0)
    where #summary.description = 'Opening Balance'
    
    --
    -- New Accounts
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --values ('New Accounts',0,0,0,0,20,'COSACS')

	--IP - 17/02/12 - #9423 - CR8262
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)
    values ('New Accounts',0,0,0,0,0,20,'COSACS')
    
    update #summary set HPvalue =
        ISNULL((SELECT isnull(sum(countvalue),0)   FROM interfacevalue i
                WHERE i.interface = 'UPDSMRY'
                AND i.runno = @runno
                AND #summary.description = 'New Accounts'
                AND i.accttype = 'HP'
                AND branchno between @minbranchno AND @maxbranchno
                AND i.counttype1 = 'NEWACCT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'New Accounts'

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'NEWACCT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'New Accounts'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
              AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'NEWACCT'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'New Accounts'

	--IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
    ISNULL((SELECT sum(countvalue)   FROM interfacevalue
            WHERE interface = 'UPDSMRY'
          AND runno = @runno
            AND accttype = 'STORECARD'
            AND branchno between @minbranchno AND @maxbranchno
            AND counttype1 = 'NEWACCT'
            AND #summary.source = counttype2),0)
	WHERE #summary.description = 'New Accounts'

    --
    -- Reopened Accounts
    --
    -- Make sure at least one row appears for 'COSACS' source even if it is zero
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Reopened Accounts',ISNULL(sum(countvalue),0),0,0,0,30,'COSACS'
    
      --IP - 17/02/12 - #9423 - CR8262
       INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue,totalvalue,orderby, source)
    SELECT 'Reopened Accounts',ISNULL(sum(countvalue),0),0,0,0,0,30,'COSACS'
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'REOPENED'
    AND counttype2 = 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno

    -- Add other rows for other sources
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Reopened Accounts',sum(countvalue),0,0,0,30,counttype2
    
        --IP - #9423 - CR8262
      INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)
    SELECT 'Reopened Accounts',sum(countvalue),0,0,0,0,30,counttype2
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'REOPENED'
    AND counttype2 != 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno
    GROUP BY counttype2

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'REOPENED'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Reopened Accounts'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'REOPENED'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Reopened Accounts'

	--IP - 17/02/12 - #9423 - CR8262
	UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'REOPENED'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Reopened Accounts'
    --
    -- Settled Accounts
  
    -- Add other rows for other sources
    -- AA 69586 Inserting blank row for branch where there are no credit accounts being settled.
    -- also source does not matter for settled accounts I believe so update does not include this
    --INSERT INTO #summary
    --(description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT Distinct 'Settled Accounts',0,0,0,0,40,''		-- UAT543 jec 06/10/08
    
     --IP - 17/02/12 - #9423 - CR8262
     INSERT INTO #summary
    (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)
    SELECT Distinct 'Settled Accounts',0,0,0,0,0,40,''		-- UAT543 jec 06/10/08
    FROM branch
    WHERE branchno between @minbranchno AND @maxbranchno
    
    UPDATE #summary set hpvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'HP'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTLED'),0)
    WHERE #summary.description = 'Settled Accounts'

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTLED'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Settled Accounts'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTLED'
     AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Settled Accounts'

	--IP - 17/02/12 - #9423 - CR8262
	UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTLED'
     AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Settled Accounts'
    
    --
    -- Closing Balance
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Closing Balance',ISNULL(sum(countvalue),0),0,0,0,50,''
    
        --IP - 17/02/12 - #9423 - CR8262
      INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue,totalvalue,orderby, source)
    SELECT 'Closing Balance',ISNULL(sum(countvalue),0),0,0,0,0,50,''
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'ACCOUNTS C/F'
    AND branchno between @minbranchno AND @maxbranchno

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ACCOUNTS C/F'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Closing Balance'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ACCOUNTS C/F'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Closing Balance'
    
        --IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(countvalue)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'ACCOUNTS C/F'
                AND #summary.source = counttype2),0)
    WHERE #summary.description = 'Closing Balance'

END


IF @type = 'INTEREST'
BEGIN
	
	--IP - 23/04/08 - UAT(315) - commented out the below that selects the 
	--'No of accounts' row as this is not required to be displayed beneath the
	-- 'Interest' tab.

--    --
--    -- No of accounts
--    --
--    -- Make sure at least one row appears for 'COSACS' source even if it is zero
--    INSERT INTO #summary
--        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
--    SELECT 'No of accounts',ISNULL(sum(CountValue),0),0,0,0,10,'COSACS'
--    FROM interfacevalue
--    WHERE interface = 'UPDSMRY'
--    AND runno = @runno
--    AND accttype = 'HP'
--    AND counttype1 = 'SETTWITHINT'
--    AND counttype2 = 'COSACS'
--    AND branchno between @minbranchno AND @maxbranchno
--
--    -- Add other rows for other sources
--    INSERT INTO #summary
--        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
--    SELECT 'No of accounts',sum(COUNTvalue),0,0,0,10,counttype2
--    FROM interfacevalue
--    WHERE interface = 'UPDSMRY'
--    AND runno = @runno
--    AND accttype = 'HP'
--    AND counttype1 = 'SETTWITHINT'
--    AND counttype2 != 'COSACS'
--    AND branchno between @minbranchno AND @maxbranchno
--    GROUP BY counttype2
--
--    UPDATE #summary set cashvalue =
--        ISNULL((SELECT sum(COUNTvalue)   FROM interfacevalue
--                WHERE interface = 'UPDSMRY'
--                AND runno = @runno
--                AND accttype = 'CASH'
--                AND branchno between @minbranchno AND @maxbranchno
--                AND counttype1 = 'SETTWITHINT'
--                AND #summary.source = counttype2),0)
--    WHERE #summary.description = 'No of accounts'
--
--    UPDATE #summary set specialvalue =
--        ISNULL((SELECT sum(COUNTvalue)   FROM interfacevalue
--                WHERE interface = 'UPDSMRY'
--                AND runno = @runno
--                AND accttype = 'SPECIAL'
--                AND branchno between @minbranchno AND @maxbranchno
--                AND counttype1 = 'SETTWITHINT'
--                AND #summary.source = counttype2),0)
--    WHERE #summary.description = 'No of accounts'


    --
    -- Opening Balance unsettled (carried forward)
    --
    -- UAT 28 Opening Balance is the unsettled interest for the previous run
    SELECT @prev_runno = max(runno)
    FROM   interfacecontrol
    WHERE  interface = 'UPDSMRY'
    AND    result = 'P'
    AND    runno < @runno

    IF ISNULL(@prev_runno,0) > 0
    BEGIN
        --INSERT INTO #summary
        --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
        --SELECT 'Opening Balance unsettled (carried forward)',ISNULL(sum(value),0),0,0,0,20,'COSACS' --counttype2
        
           
        --IP - 17/02/12 - #9423 - CR8262
            INSERT INTO #summary
            (description,  hpvalue,cashvalue, specialvalue,storecardvalue, totalvalue,orderby, source)
        SELECT 'Opening Balance unsettled (carried forward)',ISNULL(sum(value),0),0,0,0,0,20,'COSACS' --counttype2
        FROM interfacevalue
        WHERE interface = 'UPDSMRY'
        AND runno = @prev_runno
        AND accttype = 'HP'
        AND counttype1 = 'TOTUNINT'
        AND branchno between @minbranchno AND @maxbranchno
        --  GROUP BY counttype2

        UPDATE #summary set cashvalue =
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'CASH'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance unsettled (carried forward)'

        UPDATE #summary set specialvalue =
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'SPECIAL'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance unsettled (carried forward)'
        
         --IP - 17/02/12 - #9423 - CR8262
         UPDATE #summary set storecardvalue =
            ISNULL((SELECT sum(value)   FROM interfacevalue
                    WHERE interface = 'UPDSMRY'
                    AND runno = @prev_runno
                    AND accttype = 'STORECARD'
                    AND branchno between @minbranchno AND @maxbranchno
                    AND counttype1 = 'TOTUNINT'),0)
        WHERE #summary.description = 'Opening Balance unsettled (carried forward)'
    END


    --
    -- Transferred to Settled Interest
    --
    -- Make sure at least one row appears for 'COSACS' source even if it is zero

    --INSERT INTO #summary
    --  (description ,  hpvalue ,cashvalue , specialvalue , totalvalue,orderby, source)
    --  values ('Transferred to Settled Interest',0,0,0,0 ,30,'COSACS')
    
      --IP - 17/02/12 - #9423 - CR8262
      INSERT INTO #summary
      (description ,  hpvalue ,cashvalue , specialvalue , storecardvalue, totalvalue,orderby, source)
      values ('Transferred to Settled Interest',0,0,0,0,0 ,30,'COSACS')

    UPDATE #summary set hpvalue = ISNULL((SELECT sum(value)   FROM interfacevalue 
        WHERE interface ='UPDSMRY' 
        AND runno = @runno
        AND accttype ='HP' 
        AND branchno between @minbranchno AND @maxbranchno   
        AND counttype1  ='INTONSETT'
--      AND #summary.source =counttype2
        ),0)
    WHERE	#summary.description = 'Transferred to Settled Interest'

/*KEF 68001 - commented out as doing above insert and update now to enusre there's always a row inserted
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'Transferred to Settled Interest',ISNULL(sum(value),0),0,0,0,30,'COSACS'
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'INTONSETT'
    AND counttype2 = 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno

    -- Add other rows for other sources
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'Transferred to Settled Interest',sum(value),0,0,0,30,counttype2
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'INTONSETT'
    AND counttype2 != 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno
    GROUP BY counttype2
*/
    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'Transferred to Settled Interest'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'Transferred to Settled Interest'

	--IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'Transferred to Settled Interest'

    --
    -- New Unsettled Interest
    --
    -- Make sure at least one row appears for 'COSACS' source even if it is zero

    --INSERT INTO #summary
    --  (description ,  hpvalue ,cashvalue , specialvalue , totalvalue,orderby, source)
    --  values ('New Unsettled Interest',0,0,0,0 ,70,'COSACS')

	  --IP - 17/02/12 - #9423 - CR8262
      INSERT INTO #summary
      (description ,  hpvalue ,cashvalue , specialvalue ,storecardvalue, totalvalue,orderby, source)
      values ('New Unsettled Interest',0,0,0,0,0 ,70,'COSACS')
      
    UPDATE #summary set hpvalue = ISNULL((SELECT sum(value)   FROM interfacevalue 
        WHERE interface ='UPDSMRY' 
        AND runno = @runno
        AND accttype ='HP' 
        AND branchno between @minbranchno AND @maxbranchno   
        AND counttype1  ='INTONUNSETT'
--      AND #summary.source =counttype2
        ),0)
    WHERE	#summary.description = 'New Unsettled Interest'

/*KEF 68001 - commented out as doing above insert and update now to enusre there's always a row inserted
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'New Unsettled Interest',ISNULL(sum(value),0),0,0,0,40,'COSACS'
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'INTONUNSETT'
    AND counttype2 = 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno

    -- Add other rows for other sources
    INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    SELECT 'New Unsettled Interest',sum(value),0,0,0,40,counttype2
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'INTONUNSETT'
    AND counttype2 != 'COSACS'
    AND branchno between @minbranchno AND @maxbranchno
    GROUP BY counttype2
*/
    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONUNSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'New Unsettled Interest'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONUNSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'New Unsettled Interest'


		--IP - 17/02/12 - #9423 - CR8262
	UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'INTONUNSETT'
--                AND #summary.source = counttype2
    ),0)
    WHERE #summary.description = 'New Unsettled Interest'
    
--KEF UAT 51 (4.1.0.0) Add new line to show Int and adm added since last run on accounts settled since last run
    --
    -- Reversals of unsettled interest
    --
    -- Make sure at least one row appears for 'COSACS' source even if it is zero

    --INSERT INTO #summary
    --  (description ,  hpvalue ,cashvalue , specialvalue , totalvalue,orderby, source)
    --  values ('Reversals of unsettled interest',0,0,0,0 ,65,'COSACS')
    
     --IP - 17/02/12 - #9423 - CR8262
      INSERT INTO #summary
      (description ,  hpvalue ,cashvalue , specialvalue ,storecardvalue, totalvalue,orderby, source)
      values ('Reversals of unsettled interest',0,0,0,0,0 ,65,'COSACS')

    UPDATE #summary set hpvalue = ISNULL((SELECT sum(value)   FROM interfacevalue 
        WHERE interface ='UPDSMRY' 
        AND runno = @runno
        AND accttype ='HP' 
        AND branchno between @minbranchno AND @maxbranchno   
        AND counttype1  ='SETTMOVEMENT'
        ),0)
    WHERE	#summary.description = 'Reversals of unsettled interest'

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTMOVEMENT'
    ),0)
    WHERE #summary.description = 'Reversals of unsettled interest'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTMOVEMENT'
    ),0)
    WHERE #summary.description = 'Reversals of unsettled interest'
    
     --IP - 17/02/12 - #9423 - CR8262
     UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'SETTMOVEMENT'
    ),0)
    WHERE #summary.description = 'Reversals of unsettled interest'
--UAT 51 KEF end

    --
    -- Closing Balance of unearned interest
    --
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --SELECT 'Closing Balance of unearned interest',ISNULL(sum(value),0),0,0,0,80,'COSACS' --counttype2
    
       --IP - 17/02/12 - #9423 - CR8262
     INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)
    SELECT 'Closing Balance of unearned interest',ISNULL(sum(value),0),0,0,0,0,80,'COSACS' --counttype2
    FROM interfacevalue
    WHERE interface = 'UPDSMRY'
    AND runno = @runno
    AND accttype = 'HP'
    AND counttype1 = 'TOTUNINT'
    AND branchno between @minbranchno AND @maxbranchno
    --  GROUP BY counttype2

    UPDATE #summary set cashvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'CASH'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'TOTUNINT'),0)
    WHERE #summary.description = 'Closing Balance of unearned interest'

    UPDATE #summary set specialvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'SPECIAL'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'TOTUNINT'),0)
    WHERE #summary.description = 'Closing Balance of unearned interest'
    
        --IP - 17/02/12 - #9423 - CR8262
    UPDATE #summary set storecardvalue =
        ISNULL((SELECT sum(value)   FROM interfacevalue
                WHERE interface = 'UPDSMRY'
                AND runno = @runno
                AND accttype = 'STORECARD'
                AND branchno between @minbranchno AND @maxbranchno
                AND counttype1 = 'TOTUNINT'),0)
    WHERE #summary.description = 'Closing Balance of unearned interest'

END


--update #summary set totalvalue = isnull (HPvalue,0) + isnull (cashvalue,0) + isnull (specialvalue,0)

--IP - 17/02/12 - #9423 - CR8262
update #summary set totalvalue = isnull (HPvalue,0) + isnull (cashvalue,0) + isnull (specialvalue,0) + isnull(storecardvalue,0)

select @rowCounter = count(*) from #summary
if @rowCounter = 0
BEGIN
    --INSERT INTO #summary
    --    (description,  hpvalue,cashvalue, specialvalue, totalvalue,orderby, source)
    --VALUES  ('No Data Found',0,0,0,0,0,'')
    
      --IP - 17/02/12 - #9423 - CR8262
      INSERT INTO #summary
        (description,  hpvalue,cashvalue, specialvalue, storecardvalue, totalvalue,orderby, source)
    VALUES  ('No Data Found',0,0,0,0,0,0,'')
    
END

--SELECT description, hpvalue, cashvalue, specialvalue, totalvalue as total, source
SELECT description, hpvalue, cashvalue, specialvalue, storecardvalue, totalvalue as total, source			--IP - 17/02/12 - #9423 - CR8262
FROM   #summary
ORDER BY orderby, source


GO


-- for testing
/*
exec dn_summarytotals_sp
@runno =513, -- batch/run number must be supplied
@branchno  =0, -- supply branch number 0 for all.
@type= 'INTEREST', -- 'INTEREST' 'FINANCE' 'OPEN'
@RETURN =0

select distinct (counttype1) from
interfacevalue
where runno = 1348

select top 10 * from interfacevalue
where counttype2 IN('TOTUNINT','INTONUNSETT','INTONSETT','SETTWITHINT')
*/


