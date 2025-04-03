SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

IF EXISTS (SELECT * FROM sysobjects
           WHERE xtype = 'p'
           AND NAME = 'DeliveriesAndNotificationImport_File')
BEGIN
	DROP PROCEDURE DeliveriesAndNotificationImport_File 
END
GO

CREATE PROCEDURE [dbo].[DeliveriesAndNotificationImport_File]
-- =============================================
-- Author:		??
-- Create date: ??
-- Description:	Deliveries and Notification Import_File
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/02/10  jec CR1072 Malaysia merge
-- 04/03/10  jec CR1072 OrigBuffno
-- 31/03/10  jec UAT51 Include Rejections in ScheduleAudit table
-- 16/04/10  ip  UAT92 Partial shipments
-- 14/05/10  jec UAT146 POD files will only have positive quantities.
-- 04/06/10  ip  UAT262 UAT5.2.1.0 Log - When processing partial deliveries, where a rejection is processed, should use origbuffno
-- ============================================= 
@fileWithPath  VARCHAR(1000),
@vchrFile      VARCHAR(1000),
@fileListRow   VARCHAR(300),
@first BIT,
@nextsequence INT,
@lastsequence INT

AS

		DECLARE  
				 @runno INT,
			     @DateBegin DATETIME,
			     @bulkinsert    VARCHAR(1000),
			     @new_file_name VARCHAR(1000),
			     @command VARCHAR(1000),
			     @SchedBuffNo INT,	  -- UAT51 jec 31/03/10
			      
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
                @err INT 
                
		

IF NOT @fileListRow IS NULL
AND @fileListRow != 'File Not Found'
BEGIN
        SET @fileWithPath = @vchrFile + @fileListRow
        
        SELECT @RunNo     = RunNo
        FROM   interfacecontrol
        WHERE  interface = 'LOGIMPORT'
      
        IF @RunNo IS NULL
        OR @RunNo  = ''
			SET @RunNo = 1
        ELSE
			SET @RunNo     = @RunNo + 1
        
        SET @DateBegin = GETDATE()
        -- add an interfacecontrol record for each file
        INSERT
        INTO   interfacecontrol
               (
                      interface,
                      runno,
                      datestart,
                      result,
                      filename
               )
               VALUES
               (
                      'LOGIMPORT',
                      @runno,
                      @DateBegin,
                      'F',
                      @fileWithPath
               )
    
        -- CR 953 Check that each file is the next in sequence. If not then terminate and log an error.
        IF @first <> 1
        BEGIN
                SET @nextSequence =
                (SELECT SUBSTRING(@fileListRow,LEN(@fileListRow) - 8,5)
                )
                SET @lastSequence =
                (SELECT  TOP 1 SUBSTRING([fileName],LEN([fileName]) - 8,5)
                         FROM     interfacecontrol
                         WHERE    interface           = 'LOGIMPORT'
                              AND RIGHT([fileName],3) = 'DAT'
                         ORDER BY datestart DESC
                )
                IF @nextSequence <> @lastSequence + 1
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
                                      'File name does not contain the next sequence number. file: ' + @fileWithPath
                               )
                               RAISERROR
                               (
       'File name does not contain the next sequence number',
                                      16,1
                               )
                        RETURN
                END
                SET @first = 0
        END
      
        -- transaction is per file, if a good file is processed then a bad file causes error, the good file does not need to be imprted again
        TRUNCATE TABLE CustomerDeliveriesImp
        -- cannot put a variable as file path in bulk update so exec a string
        -- CR 953 commas to be used as separators
        SET @bulkinsert = 'bulk insert CustomerDeliveriesImp From ''' + @fileWithPath --+ ''' With (  FormatFile = ''' + @vchrFile + 'FormatFile2.fmt'') '
                                                                      + ''' With(FIELDTERMINATOR = '','',ROWTERMINATOR = ''\n'' )'
        --PRINT @bulkinsert
        EXEC(@bulkinsert)
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
                              'Bulk insert has failed. Check POD file for errors. Error number: ' + CAST(@err AS VARCHAR(10)) + '. File: ' + @fileWithPath
                       )
                RETURN
        END
       
        --  isdate check for pickingdate
        IF EXISTS
        (SELECT *
                FROM    CustomerDeliveriesImp
                WHERE   isdate(LEFT(pickingdate,8)) = 0
        )
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
                              'Non-date value IN PickingDate COLUMN. File: ' + @fileWithPath
                       )
                       RAISERROR
                       (
                              'Non-date value IN PickingDate',
                              16,1
                       )
                RETURN
        END
        --  isdate check for datedel
        IF EXISTS
        (SELECT *
                FROM    CustomerDeliveriesImp
                WHERE   isdate(LEFT(datedel,8)) = 0
        )
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
                              'Non-date value IN datedel COLUMN. File: ' + @fileWithPath
                       )
                       RAISERROR
                       (
                              'Non-date value IN datedel',
                              16,1
                       )
                RETURN
        END
        --  isdate check for datetrans
   
        IF EXISTS
        (SELECT *
                FROM    CustomerDeliveriesImp
                WHERE   isdate(LEFT(datetrans,8)) = 0
        )
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
                              'Non-date value IN datetrans COLUMN. File: ' + @fileWithPath
                       )
                       RAISERROR
                       (
                              'Non-date value IN datetrans',
                              16,1
                       )
                RETURN
        END
        
        
       -- Check for items not in schedule.   
       INSERT
       INTO   interfaceerror
       (
			interface,
			runno,
			errordate,
			severity,
			errortext
        )
         SELECT 'LOGIMPORT',
                 @runno,
                 @DateBegin,
                 'W',
                 'Imported item not found! ' + acctno + ', itemno ' + itemno + ', buffbranch ' + CONVERT(VARCHAR,buffbranchno) +  ', buffno ' + CONVERT(VARCHAR,buffno) + ' DHLStatus ' + [Status]  
         FROM CustomerDeliveriesImp I 
         WHERE NOT EXISTS (SELECT *
                           FROM schedule S 
                           WHERE I.acctno = S.acctno
						   AND I.delorcoll = S.delorcoll
						   AND I.itemno = S.itemno
						   AND I.stocklocn = S.stocklocn
						   --AND I.buffno = S.buffno
						   AND I.buffno = S.OrigBuffno		-- partial deliveries use Orig Buffno
				           AND I.buffbranchno = S.buffbranchno)


        
        DECLARE c2 CURSOR READ_ONLY FOR
        SELECT *
        FROM   CustomerDeliveriesImp OPEN c2
                FETCH NEXT
                FROM  c2
                INTO  @DHLacctno,--@DHLagrmtno,
                      @DHLdatedel,
                      @DHLdelorcoll,
                      @DHLDeliveryLine,
                      @DHLitemno,
                      @DHLstocklocn,
                      @DHLquantity,
                      @DHLretitemno,
                      @DHLretstocklocn,
                      @DHLretval,
                      @DHLbuffno,
                      @DHLbuffbranchno,
                      @DHLdatetrans,
                      @DHLbranchno,  --@DHLtransrefno,
                      @DHLtransvalue,--@DHLrunno,@DHLcontractno,@DHLreplacementMarker,
                      @DHLNotifiedBy,
                      @DHLftnotes,
                      @DHLStatus,
                      @DHLPickQuantity,
                      @DHLpickingDate,
                      @DHLDNNo
                SELECT @err        = @@error
        WHILE (@@FETCH_STATUS !=-1)
            BEGIN
               
				if (@@FETCH_STATUS !=-2)
				begin
                IF @DHLStatus = 'DEL'
                BEGIN

					IF ISNULL(@DHLDNNo,0) != 0
					BEGIN
							EXEC DeliveriesAndNotificationImport_Delivery	@DHLacctno = @DHLacctno,
																				@DHLdatedel =@DHLdatedel,
																				@DHLdelorcoll = @DHLdelorcoll,   
																				@DHLDeliveryLine = @DHLDeliveryLine, 
																				@DHLitemno = @DHLitemno       ,
																				@DHLstocklocn = @DHLstocklocn,    
																				@DHLquantity = @DHLquantity     ,
																				@DHLretitemno = @DHLretitemno    ,
																				@DHLretstocklocn = @DHLretstocklocn ,
																				@DHLretval = @DHLretval  ,     
																				@DHLbuffno = @DHLbuffno       ,
																				@DHLbuffbranchno = @DHLbuffbranchno ,
																				@DHLdatetrans = @DHLdatetrans    ,
																				@DHLbranchno = 	@DHLbranchno     ,
																				@DHLtransvalue = @DHLtransvalue   ,
																				@DHLNotifiedBy = @DHLNotifiedBy   ,
																				@DHLftnotes = @DHLftnotes      ,
																				@DHLStatus = @DHLStatus       ,
																				@DHLPickQuantity = @DHLPickQuantity ,
																				@DHLpickingDate = @DHLpickingDate  ,
																				@DHLDNNo = @DHLDNNo  ,
																				@fileWithPath = @fileWithPath,
																				@datebegin = @datebegin,
																			@runno = @runno
					END
					ELSE
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
										  'W',  
										   'Delivery without shipping:'+ @dhlacctno + ' itemno ' + @DHLitemno + '  file: ' + @fileWithPath  
								   )  
					END
				END
     
                
                IF @dhlstatus = 'SHP'
                BEGIN
                BEGIN TRY
                        PRINT 'notify despatch'
                        -- save original quantity
                        declare @schedQty INT
						select @schedQty=(select quantity from dbo.schedule
						  WHERE  acctno         = @DHLacctno
                           AND itemno         = @DHLitemno
                           AND delorcoll      = @DHLdelorcoll
                           AND buffbranchno = @DHLbuffbranchno
                           AND stocklocn = @DHLstocklocn
                           --AND buffno = @DHLbuffno)
                           AND OrigBuffno = @DHLbuffno -- partial deliveries use Orig Buffno
                           AND DHLPickingDate IS null) -- IP - 16/04/10 - UAT(92) UAT5.2
                        
                        if @DHLdelorcoll = 'C' and @DHLquantity>0   -- UAT146 jec 14/05/10
						begin  
							set @DHLquantity=@DHLquantity*-1  
						end  
                           
                        UPDATE SCHEDULE
                        SET    DHLPickingDate = CONVERT(DATETIME,CONVERT(CHAR(8),@DHLpickingDate)),
                               DHLDNNo        = @DHLDNNo,
                               RunnoImport		 = @Runno,
                               --ShipQty		= ShipQty+@DHLquantity		--02/03/10  jec
                               quantity		= @DHLquantity,		-- update to qty shipped
                               retval		= ROUND(retval * ABS((CAST(@DHLquantity AS INT) *1.00)/@schedQty *1.00),0)--IP/JC - 16/04/10 - UAT(90) UAT5.2 Update retval for collection
                        WHERE  acctno         = @DHLacctno
                           AND itemno         = @DHLitemno
                           AND delorcoll      = @DHLdelorcoll
                           AND buffbranchno = @DHLbuffbranchno
                           AND stocklocn = @DHLstocklocn
                           --AND buffno = @DHLbuffno --AND
                           AND OrigBuffno = @DHLbuffno		-- partial deliveries use Orig Buffno
                           AND DHLPickingDate IS NULL -- IP - 16/04/10 - UAT(92) UAT5.2
                     
                     
                     -- Partial shipment      
                     if @schedQty!= @DHLquantity
                     BEGIN				 	
					        
                        declare @newBuffno int   
                        select @newBuffno = cast(hibuffno+1 as varchar) 
						FROM branch 
						where branchno=@dhlbuffbranchno

                        insert into schedule (
						origbr,acctno,agrmtno,datedelplan,delorcoll,itemno,stocklocn,quantity,retstocklocn,
						retitemno,retval,vanno,buffbranchno,buffno,loadno,dateprinted,printedby,
						Picklistnumber,undeliveredflag,datePicklistPrinted,picklistbranchnumber,
						contractno,runNo,DHLPickingDate,consignmentNoteNo,deliveryLineNo,DHLDNNo,
						transchedno,transchednobranch,datetranschednoprinted,CreatedBy,DateCreated,
						GRTnotes,Sequence,OrigBuffNo	--,OrigQty,ShipQty
						) 
						--Select origbr,acctno,agrmtno,datedelplan,'D',itemno,stocklocn,@schedQty-@DHLquantity,retstocklocn,
						Select origbr,acctno,agrmtno,datedelplan,@DHLdelorcoll,itemno,stocklocn,@schedQty-@DHLquantity,retstocklocn, --IP - 16/04/10 - UAT(90) UAT5.2 Using @DHLdelorcoll
							retitemno,ROUND(retval * ABS(((@schedQty-CAST(@DHLquantity AS INT)) *1.00)/quantity *1.00),0),'DHL',buffbranchno,@newBuffno,loadno,dateprinted,printedby, --IP/JC - 16/04/10 - UAT(90) UAT5.2 - Update retval for collection
							Picklistnumber,undeliveredflag,null,picklistbranchnumber,
							contractno,runNo,null,consignmentNoteNo,deliveryLineNo,null,
							transchedno,transchednobranch,datetranschednoprinted,CreatedBy,DateCreated,
							GRTnotes,Sequence+1,@DHLbuffno	--,OrigQty,ShipQty
						from schedule 
						WHERE  acctno         = @DHLacctno
                           AND itemno         = @DHLitemno
                           AND delorcoll      = @DHLdelorcoll
                           AND buffbranchno = @DHLbuffbranchno
                           AND stocklocn = @DHLstocklocn
                           --AND buffno = @DHLbuffno
                           AND OrigBuffno = @DHLbuffno		-- partial deliveries use Orig Buffno
                           AND DHLDNNo = @DHLDNNo -- IP - 16/04/10 - UAT(92) UAT5.2
						
						UPDATE branch 
							SET hibuffno=hibuffno+1 
							WHERE branchno=substring(@dhlbuffbranchno,1,3)
			
					END  	   
                END TRY--end shp
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
                                      'W',  
                                      'Error in notify: '+ @dhlacctno + ' itemno ' + @DHLitemno + ': '+  CAST(ERROR_NUMBER() as varchar(10)) +':'+ ERROR_MESSAGE() + ', file: ' + @fileWithPath  
                               )  
                END CATCH 
                END
                
                
                IF @dhlstatus = 'REJ'
                BEGIN
                BEGIN TRY
                        PRINT 'reject by customer' -- todo
                        
                        SELECT @SchedBuffNo=BuffNo		-- get actual Buffno -- UAT51 jec 31/03/10
						FROM   SCHEDULE s
						WHERE  s.acctno    = @DHLacctno  
						   AND s.itemno    = @DHLitemno  
						   AND s.delorcoll = @DHLdelorcoll  
						   and s.stocklocn = @DHLstocklocn  
						   and s.buffbranchno=@DHLbuffbranchno
						   and s.dhlDNNO =@DHLDNNo
						   and s.OrigBuffNo=@DHLbuffno		-- DHL use Original BuffNo
						   
                        INSERT
                        INTO   ScheduleRemoval
                               (
                                      AcctNo,
                                      AgrmtNo,
                                      ItemNo,
                                      StockLocn,
                                      Quantity,
                                      Price,
                                      DeliveryArea,
                                      BuffNo,
                                      LoadNo,
                                      DateRemoved,
                                      RemovedBy
                               )
                        SELECT distinct s.acctno,
                               s.agrmtno,
                               s.itemno,
                               s.stocklocn,
                               s.quantity,
                               ordval,
                               '',
                               buffno,
                               loadno,
                               GETDATE(),
                               CAST(@DHLNotifiedBy AS INT)
                        FROM   SCHEDULE s
                               JOIN lineitem l
                               ON     l.acctno    = s.acctno
                                  AND l.itemno    = s.itemno
                                  AND l.stocklocn = s.stocklocn
                                  and l.delnotebranch=s.buffbranchno
                                  and s.buffbranchno	  = @DHLbuffbranchno
                        WHERE  s.acctno           = @DHLacctno
                           AND s.itemno           = @DHLitemno
                           AND delorcoll          = @DHLdelorcoll 
                           and s.stocklocn		  = @DHLstocklocn
                           and s.buffbranchno	  = @DHLbuffbranchno
                           and s.buffno			  = @SchedBuffNo		-- UAT51 jec 31/03/10  --IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log - changed from @DHLbuffno to @SchedBuffNo
                           and s.OrigBuffno		  = @DHLbuffno		--IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
                           
                        -- UAT51 jec 31/03/10 
                        INSERT  
                        INTO   scheduleaudit  
                               (origbr,acctno,agrmtno,datedelplan,delorcoll,itemno,stocklocn,quantity,retstocklocn,  
                                retitemno,retval,vanno,buffbranchno,buffno,loadno,dateprinted,printedby,picklistnumber,  
                                undeliveredflag,datepicklistprinted,picklistbranchnumber,runno,DHLPickingDate,dhldnno,  
                                consignmentnoteno,deliverylineno,OrigBuffNo,RunnoImport,Sequence  
                               )  
                        SELECT origbr,acctno,agrmtno,datedelplan,delorcoll,itemno,stocklocn,quantity,retstocklocn,  
                                retitemno,retval,vanno,buffbranchno,buffno,loadno,dateprinted,printedby,picklistnumber,  
                                undeliveredflag,datepicklistprinted,picklistbranchnumber,runno,DHLPickingDate,dhldnno,  
                                consignmentnoteno,deliverylineno,OrigBuffNo,RunnoImport,Sequence 
                        
                        FROM   SCHEDULE s  
                        WHERE  s.acctno    = @DHLacctno  
                           AND s.itemno    = @DHLitemno  
                           AND s.delorcoll = @DHLdelorcoll  
                           and s.stocklocn=@DHLstocklocn  
                           and s.buffbranchno=@DHLbuffbranchno
                           and s.BuffNo=@SchedBuffNo		                            
                           
                         --AND
                        --agrmtno  = @DHLagrmtno
                        DELETE
                        FROM   SCHEDULE
                        WHERE  acctno    = @DHLacctno
                           AND itemno    = @DHLitemno
                           AND delorcoll = @DHLdelorcoll
                           and stocklocn		  = @DHLstocklocn --AND
                        --agrmtno  = @DHLagrmtno
                           and buffbranchno	= @DHLbuffbranchno	-- UAT51 jec 31/03/10
                          -- and buffno		= @DHLbuffno		-- UAT51 jec 31/03/10
                           and buffno		= @SchedBuffNo		--IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
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
                                      'W',  
                                      'Error in rejection '+ @dhlacctno + ' itemno ' + @DHLitemno + ': '+  CAST(ERROR_NUMBER() as varchar(10)) +':'+ ERROR_MESSAGE() + ', file: ' + @fileWithPath  
                               )  
                END CATCH 
		END--rej


				

				
END--if fetchstatus
FETCH NEXT
                FROM  c2
                INTO  @DHLacctno,--@DHLagrmtno,
                      @DHLdatedel,
                      @DHLdelorcoll,
                      @DHLDeliveryLine,
                      @DHLitemno,
                      @DHLstocklocn,
                      @DHLquantity,
                      @DHLretitemno,
                      @DHLretstocklocn,
                      @DHLretval,
                      @DHLbuffno,
                      @DHLbuffbranchno,
                      @DHLdatetrans,
                      @DHLbranchno,  --@DHLtransrefno,
                      @DHLtransvalue,--@DHLrunno,@DHLcontractno,@DHLreplacementMarker,
                      @DHLNotifiedBy,
                      @DHLftnotes,
                      @DHLStatus,
                      @DHLPickQuantity,
                      @DHLpickingDate,
                      @DHLDNNo
                SELECT @err        = @@error
                
                
        END --WHILE
        CLOSE c2
        DEALLOCATE c2
        
        
        -- rename file extension from dat to imp
        SET @new_file_name = REPLACE(@fileListRow,'.DAT','.IMP')
        SET @command       = 'rename "' + @fileWithPath + '" "' + @new_file_name + '"'
        --print @command
        EXEC master.dbo.xp_cmdshell @command,
                NO_OUTPUT
        -- update interfacecontrol for 'P'
        UPDATE interfacecontrol
        SET    datefinish = GETDATE(),
               result     = 'P'
        WHERE  interface  = 'LOGIMPORT'
           AND runno      = @runno

END

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End