SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DeliveriesAndNotificationImport_Delivery]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DeliveriesAndNotificationImport_Delivery]
go
CREATE PROCEDURE [dbo].[DeliveriesAndNotificationImport_Delivery]
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Deliveries and Notification Import_delivery
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/02/10  jec CR1072 Malaysia merge
-- 29/03/10  jec UAT35 DHL Interface Date show incorrectly after delivery import
--				 UAT36 Warranty not showing in Scheduling Delivery History
-- 07/04/10  jec UAT63 Datetrans/datedel is displaying the Incorrect date
-- 01/07/10  jec UAT283 Only deliver non stock items when parent item delivered. 
-- =============================================  
 
    @DHLacctno       VARCHAR(12),  
                @DHLdatedel      VARCHAR(14),  
                @DHLdelorcoll    VARCHAR(1),  
                @DHLDeliveryLine VARCHAR(6),  
                @DHLitemno       VARCHAR(8),  
                @DHLstocklocn    VARCHAR(3),  
                @DHLquantity     VARCHAR(12),  
                @DHLretitemno    VARCHAR(8),  
                @DHLretstocklocn VARCHAR(3),  
                @DHLretval       VARCHAR(12),  
                @DHLbuffno       VARCHAR(6),  
                @DHLbuffbranchno VARCHAR(3),  
                @DHLdatetrans    VARCHAR(14),  
                @DHLbranchno     VARCHAR(3),  
                @DHLtransvalue   VARCHAR(12),  
                @DHLNotifiedBy   VARCHAR(6),  
                @DHLftnotes      VARCHAR(4),  
                @DHLStatus       VARCHAR(3),  
                @DHLPickQuantity VARCHAR(12),  
                @DHLpickingDate  VARCHAR(14),  
                @DHLDNNo         VARCHAR(8),  
                @filewithpath    VARCHAR(1000),  
                @datebegin       DATETIME,  
                @runno          INT
                  
                  
 AS      
        DECLARE @datedel           AS DATETIME,  
                @quantity          AS FLOAT,  
                @scheduledquantity AS FLOAT,  
                @datefirst AS DATETIME,
                @SchedBuffNo INT,	  -- CR1072 jec 05/03/10
                @err       AS INT
          
        PRINT 'processing delivery'  
                -----------------------------------------------------------------------------------  
                -- Begin Prcessing OF Delivery  
                -----------------------------------------------------------------------------------  
                /*  
                1) I insert a record into the delivery table for the item  
                NOTE: RunNo must be 0 when inserting to Delivery so that it will get added to Tallyman  
                2) Update Customer Balances  - Acct. Outstanding balance, Agreement.date del, intalplan.datefirst  
                3) If Quantity delivered is total of item schedule, delete record from schedule and insert row in schedule audit table.  
                4) If Quantity is less, leave record in schedule table and reduse quantity in schedule table.  
                */  
        if @DHLdelorcoll = 'C' and @DHLquantity>0   
        begin  
			set @DHLquantity=@DHLquantity*-1  
        end  
          
        SET @datedel  = CONVERT(DATETIME,CONVERT(CHAR(8),@DHLdatedel))	-- UAT63 jec 07/04/10 
        SET @Quantity = CAST(@DHLquantity AS FLOAT)  
        -- caluculate if full quantity delivered  
        -- if so set transvalue to ordervalue fom lineitem  
        -- else use unitprice * deliveredqunatity  

        IF EXISTS (SELECT * FROM   SCHEDULE s  
                   WHERE  s.acctno    = @DHLacctno  
                   AND s.itemno    = @DHLitemno  
                   AND s.delorcoll = @DHLdelorcoll  
                   and s.stocklocn = @DHLstocklocn  
                   and s.buffbranchno=@DHLbuffbranchno
                   and s.dhlDNNO =@DHLDNNo
                   and s.OrigBuffNo=@DHLbuffno)
        BEGIN
            SELECT @ScheduledQuantity = quantity,
				    @SchedBuffNo=BuffNo			-- get actual Buffno		-- CR1072 jec 05/03/10
            FROM   SCHEDULE s  
                   --WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material  
            WHERE  s.acctno    = @DHLacctno  
               AND s.itemno    = @DHLitemno  
               AND s.delorcoll = @DHLdelorcoll  
               and s.stocklocn = @DHLstocklocn  
               and s.buffbranchno=@DHLbuffbranchno
               and s.dhlDNNO =@DHLDNNo
               and s.OrigBuffNo=@DHLbuffno		-- DHL use Original BuffNo 
         END
         ELSE
         BEGIN
             INSERT INTO interfaceerror  
                (  
                        interface,  
                        runno,  
                        errordate,  
                        severity,  
                        errortext  
                )  
                VALUES  
                (  
                        'LOGIMPORT',  
                        @runno,  
                        @DateBegin,  
                        0,  
                        'Schedule record does not exist. Acctno: ' + @DHLacctno + ',item: ' + @DHLitemno + ',stocklocn: ' + @DHLstocklocn + ',buffbr: ' + @DHLbuffbranchno + ',dhldnno: ' + @DHLDNNo + ',dhlbuffno: ' + @DHLbuffno + ',delorcol: ' + @DHLdelorcoll + ', file: ' + @fileWithPath  
                )  
                RETURN
         END    
          
        SET @err = @@ERROR  
        IF @err <> 0  
        BEGIN  
INSERT  
                        INTO   interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: '+ @dhlacctno+':' + CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )  
                RETURN  
        END  
        --1)  
        BEGIN TRY  
        
				if exists (SELECT 'x' FROM delivery 
					       WHERE acctno=      @DHLacctno  
						   AND itemno           = @DHLitemno  
						   AND delorcoll        = @DHLdelorcoll
						   AND stocklocn=@DHLstocklocn
						   AND buffbranchno=@dhlbuffbranchno
						   --AND buffno=@dhlbuffno)
						   AND buffno=@SchedBuffNo)		-- CR1072 jec 05/03/10
                BEGIN
                 select @dhlbuffno = cast(hibuffno+1 as varchar) 
                 FROM branch 
                 where branchno=@dhlbuffbranchno
                 
                 UPDATE schedule 
                 SET buffno=@dhlbuffno
                 WHERE  acctno=      @DHLacctno  
                 AND itemno           = @DHLitemno  
                 AND delorcoll        = @DHLdelorcoll
                 AND stocklocn=@DHLstocklocn
                 AND buffbranchno=@dhlbuffbranchno
                   --and buffno=@dhlbuffno-1
                 UPDATE branch 
                 SET hibuffno=hibuffno+1 
                 WHERE branchno=substring(@dhlbuffbranchno,1,3)
                END
                                  
				
                INSERT  
                INTO   delivery  
                       (  
                              origbr,  
                              acctno,  
                              agrmtno,  
                              datedel,  
                              delorcoll,  
                              itemno,  
                              stocklocn,  
                              quantity,  
                              retitemno,  
                              retstocklocn,  
                              retval,  
                              buffno,  
                              buffbranchno,  
                              datetrans,  
                              branchno,  
                              transrefno,  
                              transvalue,  
                              runno,  
                              contractno,  
                              replacementmarker,  
                              notifiedby,  
                              ftnotes,
                              ParentItemNo		--UAT283 01/07/10  
                       )  
                SELECT s.stocklocn,  
                       s.acctno,  
                       s.agrmtno,  
                       @datedel,  
                       delorcoll,  
                       s.itemno,  
                       s.stocklocn,  
                       @Quantity,  
                       retitemno,  
                       retstocklocn,  
                       retval,  
                       @SchedBuffNo,		-- @dhlbuffno,  -- CR1072 jec 05/03/10
                       buffbranchno,  
                       GETDATE(),		--@datedel,  -- UAT63 jec 07/04/10
                       s.stocklocn,  
                       B.hirefno + 1,  
                       case when @dhldelorcoll='C' then retval*-1 else @quantity*price end,  --IP - 16/04/10 - UAT(90) UAT9.2 transvalue must be negative for collection.
                       0,  
                       '',  
                       '',  
                       CAST(@DHLNotifiedBy AS INT),  
                       'DHL',
                       l.ParentItemNo		--UAT283 01/07/10  
                       FROM   SCHEDULE s  
                       INNER JOIN branch B ON B.branchno = S.stocklocn 
                       JOIN lineitem l  
                       ON     l.acctno    = s.acctno  
                          AND l.itemno    = s.itemno  
                          AND l.stocklocn = s.stocklocn  
                   WHERE  s.acctno           = @DHLacctno  
                   AND s.itemno           = @DHLitemno  
                   AND s.delorcoll        = @DHLdelorcoll --AND 
				   and s.buffbranchno=@dhlbuffbranchno
				   --and s.buffno=@dhlbuffno	-- need to change 
				   and s.BuffNo=@SchedBuffNo		-- CR1072 jec 05/03/10 
					 
					UPDATE branch 
					SET hirefno = hirefno + 1
					WHERE EXISTS (SELECT * FROM lineitem L
								  INNER JOIN Schedule S ON S.stocklocn = L.stocklocn
								  WHERE S.buffbranchno = 801
								  AND s.acctno           = @DHLacctno  
								  AND s.itemno           = @DHLitemno  
								  AND L.stocklocn = branch.branchno
								  AND s.buffno = @DHLBuffno)				
					
                --s.agrmtno = @DHLagrmtno  
        END TRY  
        BEGIN CATCH  
			INSERT INTO interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: '+ @dhlacctno+':' + CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )   
      END CATCH  
        -- sl - deliver warranty  
        
        -- must be reason not using same buffno exported right?
        BEGIN TRY  
                INSERT  
                INTO   delivery  
                       (  
                              origbr,  
                              acctno,  
                              agrmtno,  
                              datedel,  
                              delorcoll,  
                              itemno,  
                              stocklocn,  
                              quantity,  
                              retitemno,  
                              retstocklocn,  
                              retval,  
                              buffno,  
                              buffbranchno,  
                              datetrans,  
                              branchno,  
                              transrefno,  
                              transvalue,  
                              runno,  
                              contractno,  
							  replacementmarker,  
                              notifiedby,  
                              ftnotes,
                              ParentItemNo		--UAT36 jec 29/03/10  
                       )  
                SELECT distinct l.stocklocn,  
                       l.acctno,  
                       l.agrmtno,  
                       @datedel,  
                       'D',  
                       l.itemno,  
                       l.stocklocn,  
                       l.quantity,  
                       NULL,  
                       NULL,  
                       NULL,  
                       b.hibuffno + 1,  
                       l.delnotebranch,  
                       @datedel,  
                       l.stocklocn,  
                       b.hirefno + 1,  
                       price,  
                       0,  
                       l.contractno,  
                       '',  
                       CAST(@DHLNotifiedBy AS INT),  
                       'DHL',
                       l.ParentItemNo		--UAT36 jec 29/03/10    
                FROM   lineitem l  
                       INNER JOIN branch b  
                       ON     l.delnotebranch = b.branchno  
                WHERE  l.acctno               = @DHLacctno  
                   AND l.parentitemno         = @DHLitemno  
                   AND l.parentlocation       = @DHLstocklocn
                   and l.itemtype='N'		-- non stock item UAT283 01/07/10
                   AND @DHLdelorcoll          = 'D' 
                   AND l.quantity !=  isnull((select sum(quantity) from delivery d
											   where d.acctno               = l.acctno 
											   AND d.itemno         = l.itemno  
											   and d.stocklocn=l.stocklocn
											   and d.buffbranchno=l.delnotebranch											   
											   and d.contractno=l.contractno),0)
                  
                                     
                UPDATE branch  
                SET    hibuffno = hibuffno + 1,  
                       hirefno = hirefno + 1
                WHERE EXISTS (SELECT * FROM lineitem L
								  INNER JOIN Schedule S ON S.stocklocn = L.stocklocn
								  WHERE S.buffbranchno = @DHLbuffbranchno
								  AND s.acctno           = @DHLacctno  
								  AND s.itemno           = @DHLitemno  
								  AND L.stocklocn = branch.branchno
								  AND s.buffno = @DHLBuffno)
           
                UPDATE lineitem  
                SET    delqty                = quantity,  
                       qtydiff               = 'N'  
                WHERE  lineitem.acctno       = @DHLacctno  
                   AND lineitem.parentitemno = @DHLitemno  
                   AND @DHLdelorcoll         = 'D'  
        END TRY  
        BEGIN CATCH  
                INSERT INTO interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: ' + @dhlacctno+':'+ CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )  
        END CATCH  
        --2)  
        -- TODO It may be better to do these after all lineitems for the account have been processed  
        -- update Agreement.Datedel  
        BEGIN TRY  
               
         DECLARE @arrears MONEY,  
                 @return INT  
                  
         BEGIN  
         EXEC DbArrearsCalc  
   @AcctNo = @DHLacctno, --  char(12)  
   @CountPcent = 75, --  float  
   @NoDates = 0, --  smallint  
   @Arrears = @Arrears,  
   @Return = @Return  
        END  
        END TRY  
        BEGIN CATCH  
                INSERT  
                        INTO   interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: ' + @dhlacctno+':'+ CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )    
        END CATCH  
        -- update datefist  
        --EXEC dbdatefirst @dhlacctno,  
  --    @datedel,  
        --        @datefirst OUT  
        --BEGIN TRY  
        --        UPDATE instalplan  
        --        SET    [datefirst] = @datefirst  
        --        WHERE  acctno      = @dhlacctno  
        --END TRY  
        --BEGIN CATCH  
        --        INSERT  
        --        INTO   interfaceerror  
        --               (  
        --                      interface,  
        --                      runno,  
        --                      errordate,  
        --                      severity,  
        --                      errortext  
        --               )  
        --               VALUES  
        --               (  
        --                      'LOGIMPORT',  
        --                      @runno,  
        --                      @DateBegin,  
        --                      0,  
        --                      'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath  
        --               )  
        --END CATCH  
        -- update oustanding balance  
          
        BEGIN TRY  
                UPDATE acct  
                SET    outstbal =  
                       (SELECT SUM(transvalue)  
                       FROM    fintrans  
                       WHERE   acctno = @dhlacctno  
                       )  
                WHERE  acctno = @dhlacctno  
        END TRY  
        BEGIN CATCH  
                INSERT  
                        INTO   interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: '+ @dhlacctno+':' + CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )   
        END CATCH  
        --3)  
        BEGIN TRY  
                        INSERT  
                        INTO   scheduleaudit  
                               (  
                                      origbr,  
                                      acctno,  
                                      agrmtno,  
                                      datedelplan,  
									  delorcoll,  
                                      itemno,  
                                      stocklocn,  
                                      quantity,  
                                      retstocklocn,  
                                      retitemno,  
                                      retval,  
                                      vanno,  
                                      buffbranchno,  
                                      buffno,  
                                      loadno,  
                                      dateprinted,  
                                      printedby,  
                                      picklistnumber,  
                                      undeliveredflag,  
                                      datepicklistprinted,  
                                      picklistbranchnumber,  
                                      runno,  
                                      DHLPickingDate,  
                                      dhldnno,  
                                      consignmentnoteno,  
                                      deliverylineno,
                                      OrigBuffNo,		-- CR1072 jec 05/03/10
                                      RunnoImport,
                                      Sequence  
                               )  
                        SELECT origbr,  
                               acctno,  
                               agrmtno,  
                               datedelplan,  
							   delorcoll,  
							   itemno,  
                               stocklocn,  
                               @quantity,  
                               retstocklocn,  
                               retitemno,  
                               retval,  
                               vanno,  
                               buffbranchno,  
                               buffno,  
                               loadno,  
                               dateprinted,  
                               printedby,  
                               picklistnumber,  
                               undeliveredflag,  
                               datepicklistprinted,  
                               picklistbranchnumber,  
                               --@runno,
                               Runno,			-- UAT35 jec 29/03/10
                               CONVERT(datetime ,substring(@DHLpickingDate,1,8)),  
                               @dhldnno,  
                               consignmentnoteno,  
                               deliverylineno,
                               OrigBuffNo,		-- CR1072 jec 05/03/10
                               --RunnoImport,
                               @runno,			-- UAT35 jec 29/03/10
                               Sequence  
                        FROM   SCHEDULE s  
                        WHERE  s.acctno    = @DHLacctno  
                           AND s.itemno    = @DHLitemno  
                           AND s.delorcoll = @DHLdelorcoll  
                           and s.stocklocn=@DHLstocklocn  
                           and s.buffbranchno=@DHLbuffbranchno  
                           --and s.buffno=@DHLbuffno  
                           and s.BuffNo=@SchedBuffNo		-- CR1072 jec 05/03/10                             
                             
                END TRY  
                BEGIN CATCH  
                        INSERT  
                        INTO   interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: '+ @dhlacctno+':' + CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )  
                END CATCH  
                
        IF((@ScheduledQuantity = @Quantity ) and exists
        (select 'x' from delivery WHERE  acctno    = @DHLacctno  
						AND itemno    = @DHLitemno  
						AND delorcoll = @DHLdelorcoll  
                           and stocklocn=@DHLstocklocn  
                           and buffbranchno=@DHLbuffbranchno  
                           --and buffno=@DHLbuffno ))
                           and BuffNo=@SchedBuffNo))		-- CR1072 jec 05/03/10    
        BEGIN  
                -- correct quantity delivered  
                -- insert into schedul audit  
                -- delete from schedule  
                -- does it do this via a trigger?  
                BEGIN TRY  
                        DELETE  
                        FROM   SCHEDULE  
                        WHERE  acctno    = @DHLacctno  
						AND itemno    = @DHLitemno  
						AND delorcoll = @DHLdelorcoll  
                        AND stocklocn=@DHLstocklocn  
                        AND buffbranchno=@DHLbuffbranchno  
                        --AND buffno=@DHLbuffno 
                        and BuffNo=@SchedBuffNo		-- CR1072 jec 05/03/10    
                        --agrmtno = @DHLagrmtno  
                END TRY  
                BEGIN CATCH  
                    INSERT  
                        INTO   interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: '+ @dhlacctno+':' + CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )  
                END CATCH  
        END  
        ELSE  
        BEGIN  
                --partial delivery  
                --reduce quantity appropriatly  
                BEGIN TRY  
                        UPDATE SCHEDULE  
                        SET    quantity  =(quantity - @quantity)  
						WHERE  acctno    = @DHLacctno  
						AND itemno    = @DHLitemno  
                        AND delorcoll = @DHLdelorcoll --AND
                       -- AND buffno = @DHLbuffno
                        and BuffNo=@SchedBuffNo		-- CR1072 jec 05/03/10  
                        AND buffbranchno = @DHLbuffbranchno  
                        AND stocklocn = @DHLstocklocn
                        --agrmtno = @DHLagrmtno  
                END TRY  
                BEGIN CATCH  
                    INSERT INTO interfaceerror  
                               (  
                                      interface,  
                                      runno,  
                                      errordate,  
                                      severity,  
                                      errortext  
                               )  
                               VALUES  
                               (  
                                      'LOGIMPORT',  
                                      @runno,  
                                      @DateBegin,  
                                      0,  
                                      'error number: '+ @dhlacctno+':' + CAST(ERROR_NUMBER() as varchar(10)) +':'+ERROR_MESSAGE()+ ', file: ' + @fileWithPath  
                               )  
                END CATCH  
        END  
        
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End