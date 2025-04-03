IF EXISTS (SELECT name 
    FROM   sysobjects 
    WHERE  name = N'CustomerDeliveriesImport' 
    AND    type = 'P')
    DROP PROCEDURE CustomerDeliveriesImport
go
CREATE PROCEDURE CustomerDeliveriesImport
AS
--*****************************************************************************
-- begin get list of files for import
--*****************************************************************************
DECLARE @vchrFile				VARCHAR(1000),
	@bulkinsert					VARCHAR(1000),
	@fileListRow				VARCHAR(300),
	@fileWithPath				VARCHAR(1000),
	@new_file_name				VARCHAR(1000),
	@command					VARCHAR(2000),
	@runno						int,
	@DateBegin					DATETIME,
	@dosCommand					VARCHAR(8000),
	@err						INT,
	@DocumentType				VARCHAR(4),
	@CNNote						VARCHAR(10),
	@DeliveryLine				VARCHAR(6),
	@Material					VARCHAR(8),
	@ConfirmedPickedQuantity	VARCHAR(15),
	@UOM						VARCHAR(3),
	@PickingDate				VARCHAR(8),
	@DHLDNNumber				VARCHAR(10),
	@DateDel					DATETIME,
	@Quantity					FLOAT,
	@acctno						VARCHAR(12),
	@datefirst					DATETIME,
	@ScheduledQuantity			FLOAT

-- get all despatchnotes from directory
SELECT @vchrFile = [Value] FROM CountryMaintenance WHERE codename = '3PLFileDir'


-- Build the dos command to get a list of files  
select @dosCommand =  
    'insert into #tempFileList(fileListRow) ' +  
    'exec master.dbo.xp_cmdshell ''dir /b ' + @vchrFile + 'CM_OF_GI*.DAT '''  

-- Create a temporary table to store the file list  
create table #tempFileList (  
    fileListRow varchar(1000) null  
)
exec(@dosCommand) 
--*****************************************************************************
-- begin get list of files for import
--*****************************************************************************


-- now loop through each file
SELECT * FROM #tempfilelist

SET NOCOUNT ON
DECLARE c1 CURSOR READ_ONLY FOR SELECT fileListRow FROM #tempfilelist  
			
	OPEN c1  
	FETCH NEXT FROM c1 INTO @fileListRow
		
	WHILE @@FETCH_STATUS = 0  
	BEGIN
		IF not @fileListRow IS NULL AND @fileListRow != 'File Not Found'
		BEGIN
			SET @fileWithPath =  @vchrFile + @fileListRow 

			SELECT @RunNo = RunNo FROM interfacecontrol
				WHERE interface = 'LOGCDIMP'
			IF @RunNo IS NULL	
				SET @RunNo = 1
			ELSE
				SET @RunNo = @RunNo + 1

			SET @DateBegin = GETDATE()

			-- add an interfacecontrol record for each file
			INSERT INTO interfacecontrol
				(interface,runno,datestart,result, filename)
			VALUES
				('LOGCDIMP',@runno,@DateBegin,'F',@fileWithPath)

			-- transaction is per file, if a good file is processed then a bad file causes error, the good file does not need to be imprted again
			BEGIN TRANSACTION


			TRUNCATE TABLE CustomerDeliveryImp

			-- cannot put a variable as file path in bulk update so exec a string
			SET @bulkinsert = 'bulk insert dbo.CustomerDeliveryImp From ''' + @fileWithPath + ''' With ( 	FormatFile = ''' + @vchrFile + 'FormatFile.fmt'') '
			EXEC (@bulkinsert)

			--  isdate check to date column and 
			IF EXISTS(SELECT * FROM CustomerDeliveryImp where isdate(pickingdate) = 0)
			BEGIN
				ROLLBACK TRANSACTION
				INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
				VALUES('LOGCDIMP', @runno, @DateBegin,0,'non-date value IN PickingDate COLUMN file: ' + @fileWithPath)
				RETURN
			END
			


		


			DECLARE c2 CURSOR READ_ONLY FOR SELECT * FROM CustomerDeliveryImp
			OPEN c2  
			FETCH NEXT FROM c2 INTO @DocumentType, @CNNote, @DeliveryLine, @Material,
				@confirmedPickedQuantity, @UOM, @PickingDate,  @DHLDNNumber
		
			WHILE @@FETCH_STATUS = 0  
			BEGIN

				-----------------------------------------------------------------------------------
				-- Begin Processing OF Delivery 
				-----------------------------------------------------------------------------------
				/*
				1) I insert a record into the delivery table for the item
					NOTE: RunNo must be 0 when inserting to Delivery so that it will get added to Tallyman
				2) Update Customer Balances  - Acct. Outstanding balance, Agreement.date del, intalplan.datefirst
				3) If Quantity delivered is total of item schedule, delete record from schedule and insert row in schedule audit table.
				4) If Quantity is less, leave record in schedule table and reduse quantity in schedule table.
				*/
				
				SET @datedel = CONVERT(datetime, @PickingDate, 120)
				SET @Quantity = CAST(@ConfirmedPickedQuantity AS FLOAT)

				-- caluculate if full quantity delivered
				-- if so set transvalue to ordervalue fom lineitem
				-- else use unitprice * deliveredqunatity
				SELECT @ScheduledQuantity = quantity FROM schedule s
					WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material 

				SET @err = @@ERROR
					IF @err <> 0 
				BEGIN
					ROLLBACK TRANSACTION
					INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
						VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
					RETURN
				END

				DECLARE @transValue money
				DECLARE @price MONEY

				IF(@Quantity > 0)
				BEGIN
				----------------------------------------------------------------------------
				-- DELIVERY
				----------------------------------------------------------------------------
					IF(@ScheduledQuantity = @Quantity)
					BEGIN
						SELECT @transvalue = l.ordval
							FROM schedule s
								INNER JOIN LineItem l 
									ON s.acctno = l.acctno 
										AND s.itemno = l.itemno
										AND s.stocklocn = l.stocklocn
								WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material 	

						SET @err = @@ERROR
						IF @err <> 0 
						BEGIN
							ROLLBACK TRANSACTION
							INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
								VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
							RETURN
						END
					END
					else
					BEGIN
						SELECT @price = price
							FROM schedule s
								INNER JOIN LineItem l 
									ON s.acctno = l.acctno 
										AND s.itemno = l.itemno
										AND s.stocklocn = l.stocklocn
								WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material 
					
						SET @err = @@ERROR
						IF @err <> 0 
						BEGIN
							ROLLBACK TRANSACTION
							INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
								VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
							RETURN
						END
		
						SET @transValue = @price * @quantity

						SET @err = @@ERROR
						IF @err <> 0 
						BEGIN
							ROLLBACK TRANSACTION
							INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
								VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
							RETURN
						END
					END
				END
				ELSE
				BEGIN
					----------------------------------------------------------------------------
					-- COLLECTION
					----------------------------------------------------------------------------	
					SELECT @transvalue = d.transValue
						FROM schedule s
							INNER JOIN Delivery d
								ON s.acctno = d.acctno 
									AND s.itemno = d.itemno
									AND s.stocklocn = d.stocklocn
						WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material 
							AND d.delorcoll = 'D'
						 
						IF(@ScheduledQuantity <> @Quantity)
							SET @transvalue = @transvalue * (@Quantity / @ScheduledQuantity)
							
						-- set to a negative value
						SET @transValue = -@transValue
				END

				SELECT @acctno = s.acctno FROM schedule s
					WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material

				SET @err = @@ERROR
				IF @err <> 0 
				BEGIN
					ROLLBACK TRANSACTION
					INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
						VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
					RETURN
				END
				--1)
				INSERT INTO delivery
					(origbr, acctno, agrmtno, datedel, delorcoll, itemno, stocklocn,
					quantity, retitemno, retstocklocn, retval, buffno, buffbranchno,
					datetrans, branchno, transrefno, transvalue, runno, contractno, ReplacementMarker, notifiedby, ftnotes)
				/*SELECT origbr,acctno, agrmtno, @datedel, delorcoll, itemno, stocklocn,
					@Quantity, retitemno, retstocklocn, retVal,buffno,buffbranchno,
					@datedel, origbr, '',@transValue,0,'','',-88888,'DHL'*/
				SELECT stocklocn,acctno, agrmtno, @datedel, delorcoll, itemno, stocklocn,
					@Quantity, retitemno, retstocklocn, retVal,buffno,buffbranchno,
					@datedel, stocklocn, '',@transValue,0,'','',-88888,'DHL'
				FROM schedule s
					WHERE s.consignmentnoteno = @CNNote AND s.itemno = @Material

					SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END

				--2)
				-- TODO It may be better to do these after all lineitems for the account have been processed
				-- update Agreement.Datedel
				UPDATE Agreement SET datedel = @datedel WHERE acctno = @acctno

					SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END

				-- update datefist
				exec dbdatefirst @acctno, @datedel, @datefirst out
				update instalplan
					set [DATEFIRST] = @datefirst
					where acctno=@acctno

					SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END

				-- update oustanding balance
				UPDATE acct
					SET outstbal = (select sum(transvalue) from fintrans where acctno = @acctno)
					WHERE acctno = @acctno

					SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END

				--3)

				IF(@ScheduledQuantity = @Quantity)
				BEGIN
					-- correct quantity delivered
					-- insert into schedul audit
					-- delete from schedule
					-- does it do this via a trigger?
					INSERT INTO scheduleAudit (origbr,acctno,agrmtno, datedelplan,delorcoll,itemno,stocklocn, quantity, retstocklocn,retitemno,retval,vanno,buffbranchno,buffno,loadno,dateprinted,printedby,Picklistnumber, undeliveredflag,datepicklistprinted,picklistbranchnumber,runno,dhlpickingdate,dhldnno,consignmentnoteno,deliverylineno)
						SELECT origbr,acctno,agrmtno, datedelplan,delorcoll,itemno,stocklocn, quantity, retstocklocn,retitemno,retval,vanno,buffbranchno,buffno,loadno,dateprinted,printedby,Picklistnumber, undeliveredflag,datepicklistprinted,picklistbranchnumber,runno,dhlpickingdate,dhldnno,consignmentnoteno,deliverylineno 
						FROM schedule
						WHERE consignmentnoteno = @CNNote AND itemno = @Material 

					SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END	

					DELETE FROM schedule WHERE consignmentnoteno = @CNNote AND itemno = @Material

					SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END 
				END
				ELSE
				BEGIN
					--partial delivery
					--reduce quantity appropriatly
					UPDATE schedule  SET quantity = (quantity - @quantity)
						WHERE consignmentnoteno = @CNNote AND itemno = @Material 

				SET @err = @@ERROR
					IF @err <> 0 
					BEGIN
						ROLLBACK TRANSACTION
						INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
							VALUES('LOGCDIMP', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)) + ', file: ' + @fileWithPath)
						RETURN
					END 
				END	
			-----------------------------------------------------------------------------------
			-- End Processing OF Delivery 
			-----------------------------------------------------------------------------------

				FETCH NEXT FROM c2 INTO @DocumentType, @CNNote, @DeliveryLine, @Material,
					@confirmedPickedQuantity, @UOM, @PickingDate,  @DHLDNNumber
			END
			CLOSE c2
			DEALLOCATE c2


			-- rename file extension from dat to imp

			set @new_file_name = replace(@fileListRow, '.DAT', '.IMP')
			set @command = 'rename "' + @fileWithPath + '" "' + @new_file_name + '"'
			--print @command
			exec master.dbo.xp_cmdshell @command , NO_OUTPUT

			-- update interfacecontrol for 'P'
			update interfacecontrol
				SET datefinish = getdate() , result = 'P'
				where interface = 'LOGCDIMP' AND runno = @runno
			
			COMMIT transaction
		end
		FETCH NEXT FROM c1 INTO @fileListRow
	END 
 CLOSE c1
 DEALLOCATE c1