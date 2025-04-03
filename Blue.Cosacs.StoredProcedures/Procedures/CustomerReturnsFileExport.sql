IF EXISTS (SELECT name 
    FROM   sysobjects 
    WHERE  name = N'CustomerReturnsFileExport' 
    AND    type = 'P')
    DROP PROCEDURE CustomerReturnsFileExport
go
CREATE PROCEDURE CustomerReturnsFileExport
	@warehouseBranchNo INT 
AS


DECLARE 
	@vchrFile VARCHAR(1000),
	@vchrFileID INT ,
	@RC INT,
	@FS INT,
	@vchrLine varchar(8000), -- holds the line that we are about to print
	@RunNo INT, 
	@DateBegin DATETIME,
	@dateStr VARCHAR(15),
	@Plant						SMALLINT,
	@CNNote						Varchar(10),
	@DistCentreStore			SMALLINT,
	@CustomerPO					CHAR(12),
	@ScheduleShipDate			DATETIME,
	@CustomerName1				VARCHAR(60),
	@CustomerName2				varchar(60),
	@AddressLine1				VARCHAR(50),
	@AddressLine2				VARCHAR(50),
	@AddressLine3				VARCHAR(50),
	@PostalCode					VARCHAR(10),
	@AddressInstructions		NVARCHAR(2000),
	@HomePhoneNumber			VARCHAR(20),
	@MobilePhoneNumber			VARCHAR(20),
	@DeliveryLine				INT,
	@material					VARCHAR(8),
	@Quantity					FLOAT,
	@HigherlevlItem				VARCHAR(8),
	@ItemDeliveryInstruction	VARCHAR(200),
	@err						int

DECLARE @count INT
-- insert deliveryLineNo
SELECT acctno,itemno,IDENTITY(INT,1,1) AS id 
	INTO #schedule
FROM schedule
	WHERE buffbranchno =@warehouseBranchNo AND 
			delorcoll = 'C' AND 
			runNo IS NULL
		ORDER BY acctno,itemno

SET @count = @@ROWCOUNT

IF(@count > 0)
begin

-- if an export did not complete we will not update the runno, so delete these records
DELETE FROM scheduleexport WHERE runNo IS null

SELECT @RunNo = RunNo FROM interfacecontrol
	WHERE interface = 'LOGEXPCR'
IF @RunNo IS NULL	
	SET @RunNo = 1
ELSE
	SET @RunNo = @RunNo + 1

SET @DateBegin = GETDATE()

	--initialize file
	SELECT @vchrFile = [Value] FROM CountryMaintenance WHERE codename = '3PLFileDir'
	
	-- todo check for final '\' in directory in @vchrFile, append if neccesary
	
	--filename format CM_IF_CR.YYYYMMSSMNHR.DAT
	
	set @dateStr = CONVERT(CHAR(8), @DateBegin, 112) + REPLACE(CONVERT(CHAR(8), @DateBegin, 108), ':', '') 
	
	SET @vchrFile = @vchrFile + 'CM_IF_CR.' + @dateStr + '.DAT'


INSERT INTO interfacecontrol
	(interface,runno,datestart,result, filename)
VALUES
	('LOGEXPCR',@runno,@DateBegin,'F',@vchrFile)

BEGIN TRANSACTION


-- set the consignmentNoteNo
UPDATE schedule
	-- this would not be unique
	--SET ConsignmentNoteNo = CAST(ISNULL(buffbranchno,'') AS VARCHAR(3))  + CAST(ISNULL(buffno,'') AS VARCHAR(7))
	SET ConsignmentNoteNo = CAST(ISNULL(stocklocn,'') AS VARCHAR(3))  + CAST(ISNULL(buffno,'') AS VARCHAR(7))
	WHERE buffbranchno =@warehouseBranchNo AND 
			delorcoll = 'C' AND 
			runNo IS NULL AND 
			ConsignmentNoteNo IS NULL

SET @err = @@ERROR
IF @err <> 0 
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPCR', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))
	RETURN
END


UPDATE s
	SET s.DeliveryLineNo = st.ID 
	FROM schedule s
		INNER JOIN #schedule st
			ON s.acctno = st.acctno AND
				s.itemno = st.itemno

SET @err = @@ERROR
IF @err <> 0 
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPCR', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))
	RETURN
END

--insert valuese into exportTable so they can be retrieved later and dont lock Schedule table

--delivery order file
INSERT INTO scheduleExport
SELECT	a.branchno AS Plant,
		ConsignmentNoteNo AS CNNote,
		l.stocklocn AS DistCentreStore,
		s.acctno AS CustomerPO,
		l.dateplandel AS ScheduleShipDate,
		-- new line 
		SUBSTRING(c.NAME,0,35) AS CustomerName1,
		SUBSTRING(c.NAME,35,35) AS CustomerName2,
		cad.cusaddr1 AS AddressLine1,
		cad.cusaddr2 AS AddressLine2,
		cad.cusaddr3 AS AddressLine3,
		cad.cuspocode AS PostalCode,
		cad.Notes AS AddressInstructions,
		ct.telno AS HomePhoneNumber,
		cm.telno AS MobilePhoneNumber,
		-- new line
		s.DeliveryLineNo as DeliveryLine, 
		s.itemno AS material,
		s.quantity AS Quantity,
		s.itemno AS HigherLevItem,
		-- new line
		l.Notes AS ItemDeliveryInstruction,
		NULL,
		null
	
	FROM schedule s
		INNER JOIN acct a
			ON s.acctno = a.acctno
		INNER JOIN lineitem l
			ON s.acctno = l.acctno AND s.itemno = l.itemno
		INNER JOIN custacct ca ON
			s.acctno = ca.acctno
		INNER JOIN customer c ON
			ca.custid = c.custid
		left JOIN custaddress cad
			ON ca.custid = cad.custid AND cad.addtype= l.deliveryAddress AND cad.datemoved IS NULL
		left JOIN custtel ct
			ON ca.custid = ct.custid AND ct.tellocn = 'H' AND ct.datediscon IS NULL
		left JOIN custtel cm
			ON ca.custid = cm.custid AND cm.tellocn = 'M' AND cm.datediscon IS NULL
		INNER JOIN #schedule st
			ON s.acctno = st.acctno AND	s.itemno = st.itemno
	WHERE s.buffbranchno =@warehouseBranchNo AND s.delorcoll = 'C'

SET @err = @@ERROR
IF @err <> 0 
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPCR', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))
	RETURN
END

-- =================================================================
-- open the output file
-- =================================================================
EXEC @RC = sp_OACreate 'Scripting.FileSystemObject', @FS OUT

IF @RC <> 0
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Creating the file system object')
	RETURN
END

-- Opens the file specified by the @vchrFile input parameter
EXEC @RC = sp_OAMethod @FS , 'OpenTextFile' , @vchrFileID OUT , @vchrFile , 8 , 1

-- Prints error if non 0 return code during sp_OAMethod OpenTextFile execution 
IF @RC <> 0
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Opening the specified text file')
	RETURN
END
-- =================================================================
-- end open the output file
-- =================================================================
	
	DECLARE  @prevCustomerPO CHAR(12)
	SET @prevCustomerPO = ''

	SET NOCOUNT ON
	DECLARE c1 CURSOR READ_ONLY FOR 
		SELECT Plant, CNNote, DistCentreStore, CustomerPO, ScheduleShipDate, 
			CustomerName1, CustomerName2, AddressLine1, AddressLine2,
			AddressLine3, PostalCode, AddressInstructions, HomePhoneNumber, MobilePhoneNumber,
			DeliveryLine, material, Quantity, HigherLevItem, ItemDeliveryInstruction 
			FROM scheduleExport  
			WHERE RunNo IS null
	OPEN c1  
	FETCH NEXT FROM c1 INTO @Plant, @CNNote, @DistCentreStore, @CustomerPO, @ScheduleShipDate,
		@CustomerName1, @CustomerName2, @AddressLine1, @AddressLine2, @AddressLine3,
		@PostalCode, @AddressInstructions, @HomePhoneNumber, @MobilePhoneNumber,
		@DeliveryLine, @material, @Quantity, @HigherlevlItem, @ItemDeliveryInstruction
		
	WHILE @@FETCH_STATUS = 0  
	BEGIN
		IF @customerPO != @prevCustomerPO
		BEGIN

			set @vchrLine = 'RH1' --TransactionType
				+ CAST(ISNULL(@Plant,'') AS CHAR(4))
				+ CAST(ISNULL(@CNNote,'') AS CHAR(10)) -- Return Delivery Note?
				+ CAST(ISNULL(@CustomerPO,'') AS CHAR(20))
				+ REPLACE(CONVERT(CHAR(10), @ScheduleShipDate, 103), '/', '') -- Return Date?
				+ '           ' -- OrderType,
				+ CAST(ISNULL(@DistCentreStore,'') AS CHAR(18)) -- Store Zone /Advertising Date (This is store where stock located not store where order was made)
			EXEC @RC = sp_OAMethod @vchrFileID, 'WriteLine', Null , @vchrLine
			IF @RC <> 0 PRINT 'Error:  Writing string data to file'

			set @vchrLine = 'RH2' -- TransactionType,
				+ CAST(ISNULL(@CNNote,'') AS CHAR(10))
				+ '           ' -- ShipToCustomer,
				+ CAST(ISNULL(@CustomerName1,'') AS CHAR(35))
				+ CAST(ISNULL(@CustomerName2,'') AS CHAR(35))
				+ '                                           ' -- CustomerName3
				+ CAST(ISNULL(@AddressLine1,'') AS CHAR(35))
				+ CAST(ISNULL(@AddressLine2,'') AS CHAR(35))
				+ CAST(ISNULL(@AddressLine3,'') AS CHAR(35))
				+ '   ' -- state,
				+ CAST(ISNULL(@PostalCode,'') AS CHAR(10))
				+ '                                           ' -- District
				+ 'MY ' -- CountryKey,
				--+ CAST(ISNULL(@AddressInstructions,'') AS CHAR(200))
				--+ CAST(ISNULL(@HomePhoneNumber,'') AS CHAR(15))
				--+ CAST(ISNULL(@MobilePhoneNumber,'') AS CHAR(15))
				+ '                         ' -- DockNumber,
				--+ '      ' -- DockTime1,
				--+ '      ' -- DockTime2,
				--+ '      ' -- DockTime3,
				--+ '      ' -- DockTime4,
			EXEC @RC = sp_OAMethod @vchrFileID, 'WriteLine', Null , @vchrLine
			IF @RC <> 0
			BEGIN
				ROLLBACK TRANSACTION
				INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
					VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Writing string data to file')
				RETURN
			END


			set @vchrLine = 'RH3' -- TransactionType,
				+ CAST(ISNULL(@CNNote,'') AS CHAR(10))
				+ '          ' -- Dock Location
				+ CAST(ISNULL(@AddressInstructions,'') AS CHAR(280))
			EXEC @RC = sp_OAMethod @vchrFileID, 'WriteLine', Null , @vchrLine
			IF @RC <> 0
			BEGIN
				ROLLBACK TRANSACTION
				INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
					VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Writing string data to file')
				RETURN
			END

			SET @prevCustomerPO = @customerPO
		END

		-- this section repeats
		set @vchrLine = 'RT1' -- TransactionType,
			+ CAST(ISNULL(@CNNote,'') AS CHAR(10))
			+ CAST(ISNULL(@DeliveryLine,'') AS CHAR(6))
			+ CAST(ISNULL(@material,'') AS CHAR(18))
			+ CAST(ISNULL(@Quantity,'') AS CHAR(15))
			+ '   ' -- UOM,
			+ CAST(ISNULL(@DistCentreStore,'') AS CHAR(4)) -- Plant,
			+ '    ' -- StorageLocation,
			+ CAST(ISNULL(@CustomerPO,'') AS CHAR(20))
			+ '          ' -- UnitsPerBox,
			+ '          ' -- BoxPerPallet,
			--+ ' ' -- DeliveryLineType,
			--+ CAST(ISNULL(@HigherlevlItem,'') AS CHAR(6))
		EXEC @RC = sp_OAMethod @vchrFileID, 'WriteLine', Null , @vchrLine
		IF @RC <> 0
			BEGIN
				ROLLBACK TRANSACTION
				INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
					VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Writing string data to file')
				RETURN
			END

		set @vchrLine = 'RT2' -- TransactionType,
			+ CAST(ISNULL(@CNNote,'') AS CHAR(10))
			+ CAST(ISNULL(@DeliveryLine,'') AS CHAR(6))
			+ CAST(ISNULL(@material,'') AS CHAR(18))
			+ CAST(ISNULL(@ItemDeliveryInstruction,'') AS CHAR(280))
		EXEC @RC = sp_OAMethod @vchrFileID, 'WriteLine', Null , @vchrLine
		IF @RC <> 0
			BEGIN
				ROLLBACK TRANSACTION
				INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
					VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Writing string data to file')
				RETURN
			END
		

		FETCH NEXT FROM c1 INTO @Plant, @CNNote, @DistCentreStore, @CustomerPO, @ScheduleShipDate,
		@CustomerName1, @CustomerName2, @AddressLine1, @AddressLine2, @AddressLine3,
		@PostalCode, @AddressInstructions, @HomePhoneNumber, @MobilePhoneNumber,
		@DeliveryLine, @material, @Quantity, @HigherlevlItem, @ItemDeliveryInstruction
	END 
 CLOSE c1
 DEALLOCATE c1





UPDATE s
	SET s.RunNo = @runNo, s.VanNo = 'DHL'
	FROM schedule s
		INNER JOIN #schedule st
			ON s.acctno = st.acctno AND
				s.itemno = st.itemno
SET @err = @@ERROR
IF @err <> 0 
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPCR', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))
	RETURN
END


UPDATE s
	SET s.RunNo = @runNo--, s.VanNo = 'DHL'
	FROM scheduleExport s
		INNER JOIN #schedule st
			ON s.customerpo = st.acctno AND
				s.material = st.itemno

SET @err = @@ERROR
IF @err <> 0 
BEGIN
	ROLLBACK TRANSACTION
	INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)
		VALUES('LOGEXPCR', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))
	RETURN
END


update interfacecontrol
	SET datefinish = getdate() , result = 'P'
	where interface = 'LOGEXPCR' AND runno =@runno


COMMIT TRANSACTION

EXECUTE @RC = sp_OADestroy @vchrFileID
EXECUTE @RC = sp_OADestroy @FS
end