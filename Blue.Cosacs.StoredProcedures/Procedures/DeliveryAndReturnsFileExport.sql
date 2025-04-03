IF EXISTS (SELECT * FROM sysobjects 
           WHERE NAME = 'DeliveryAndReturnsFileExport'
           AND xtype = 'P')
BEGIN
	DROP PROCEDURE DeliveryAndReturnsFileExport
END
GO


CREATE PROCEDURE DeliveryAndReturnsFileExport 
-- =============================================
-- Author:		??
-- Create date: ??
-- Description: Delivery And Returns File Export
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/03/10  jec LW72237 - Remove comma's from GRT notes
-- 16/03/10  jec LW?? - Remove Newline char from GRT notes
-- 30/03/10  jec UAT44 Previous exported lines re-exported on Revision of item
-- 01/04/10  jec UAT59 Line number displayed Incorrectly
-- =============================================        
AS      
      
      
DECLARE        
 @vchrFile     VARCHAR(1000),      
 @vchrFileID     INT ,      
 @RC       INT,      
 @FS       INT,      
 @vchrLine     varchar(8000), -- holds the line that we are about to print      
 @RunNo      INT,       
 @DateBegin     DATETIME,      
 @dateStr     VARCHAR(15),      
 @acctno      CHAR(12),      
 @agrmtno     int,      
 @datedelplan    Datetime,       
 @delorcoll     CHAR(1),      
 @itemno      VARCHAR(8),      
 @stocklocn     smallint,       
 @quantity     float,      
 @retstocklocn    smallint,       
 @retitemno     VARCHAR(8),       
 @retval      float,       
 @vanno      VARCHAR(8),       
 @buffbranchno    smallint,      
 @buffno      int,       
 @loadno      smallint,      
 @dateprinted    datetime,       
 @printedby     int,       
 @picklistnumber    int,       
 @undeliveredflag   CHAR(1),       
 @contractno     VARCHAR(10),      
 @transactiontype   VARCHAR(3),       
 @custid      VARCHAR(20),       
 @customername1    VARCHAR(35),      
 @customername2    VARCHAR(35),       
 @addressline1    VARCHAR(50),      
 @addressline2    VARCHAR(50),      
 @addressline3    VARCHAR(50),      
 @postalcode     VARCHAR(10),      
 @country     VARCHAR(3),      
 @addressinstructions  NVARCHAR(1000),      
 @homePhone                  VARCHAR(28),      
 @workPhone                  VARCHAR(28),      
 @mobile                     VARCHAR(28),      
 @itemdeliveryinstruction VARCHAR(200),      
 @deliverylineno    INT,      
 @count      INT,      
 @err      int      

-- to remove Newline chars
declare @NewLine char(2)			
set @NewLine=char(13)+char(10)
   
-- if an export did not complete we will not update the runno, so delete these records      
-- insert deliveryLineNo      
      
PRINT 'Get schedule items to export'      
SELECT acctno,itemno,IDENTITY(INT,1,1) AS id , delorcoll, stocklocn, buffno     
 INTO #schedule      
FROM schedule      
  INNER JOIN branch      
   ON schedule.buffbranchno = branch.branchno      
 WHERE --buffbranchno =@warehouseBranchNo AND       
   ((runNo IS NULL and delorcoll<>'X')  
   OR (delorcoll = 'X' and runNo is not null) )
   --AND branch.dotnetforwarehouse = 'Y'     --IP - 26/02/10 - CR1072 - Malaysia 3PL for Version 5.2 - commented out as now using ThirdPartyWarehouse
   AND branch.ThirdPartyWarehouse= 'Y'  
   and loadno=0    
  ORDER BY delorcoll,acctno,itemno      
      
   
SET @count = @@ROWCOUNT 
DELETE FROM schedule FROM schedule s        
 WHERE s.delorcoll = 'X' and runno is null     
           
      
IF(@count > 0)      
begin      
    
DELETE FROM scheduledexport WHERE runNo IS NULL      
      
-- CR953 Remove data from scheduledexport that is more than 40 days old      
      
--DELETE FROM scheduledexport WHERE  DATEDIFF(DAY,exportdate,GETDATE()) > 40      
      
SELECT @RunNo = RunNo FROM interfacecontrol      
 WHERE interface = 'LOGEXPORT'      
       
IF @RunNo IS NULL OR @RunNo = ''       
 SET @RunNo = 1      
ELSE      
 SET @RunNo = @RunNo + 1      
      
SET @DateBegin = GETDATE()      
      
 --initialize file      
 SELECT @vchrFile = [Value] FROM CountryMaintenance WHERE codename = '3PLFileDir'      
 -- UAT 4 check for final '\' in directory in @vchrFile, append if neccesary      
 IF RIGHT(@vchrFile,1) <> '\'      
 BEGIN      
  SET @vchrFile = @vchrFile + '\'      
 END      
       
 --filename format SCHYYYYMMSSMNHR.DAT      
 -- CR953 file name is to be of the format SCHYYYYMMDDSSSSS.DAT where SSSSS is the run number starting with 00001, and YYYYMMDD is the date      
 DECLARE @sequence VARCHAR(5)      
       
    IF EXISTS(SELECT * FROM interfacecontrol WHERE interface = 'LOGEXPORT' AND RIGHT([fileName],3) = 'DAT' AND substring([fileName],LEN([fileName]) - 19,3) = 'SCH')      
    BEGIN      
  SET @sequence = (SELECT TOP 1 ISNULL(SUBSTRING([fileName],LEN([fileName]) - 8,5),'00001')       
        FROM interfacecontrol WHERE interface = 'LOGEXPORT' AND RIGHT([fileName],3) = 'DAT' AND SUBSTRING([fileName],LEN([fileName]) - 19,3) = 'SCH' ORDER BY runno DESC)      
        DECLARE @int INT      
        SET @int = CAST(@sequence AS INT)      
        SET @int = @int + 1      
        SET @sequence = RIGHT('00000' + CAST (@int AS VARCHAR(5)),5)       
 END          
    ELSE      
    BEGIN      
  SET @sequence = '00001'      
 END      
          
    set @dateStr = CONVERT(CHAR(8), @DateBegin, 112) + @sequence -- + REPLACE(CONVERT(CHAR(8), @DateBegin, 108), ':', '')       
       
 SET @vchrFile = @vchrFile + 'SCH' + @dateStr + '.DAT'      
      
      
INSERT INTO interfacecontrol      
 (interface,runno,datestart,result, filename)      
VALUES      
 ('LOGEXPORT',@runno,@DateBegin,'F',@vchrFile)      
      
      
BEGIN TRANSACTION      
      
      
-- set the consignmentNoteNo      
-- CR 953 If the item has been removed then a cancellation record needs to be sent to DHL      
-- UAT 6 datepicklistprinted should be updated to date and time of export.      
UPDATE s      
 SET s.DeliveryLineNo = st.ID,      
  s.ConsignmentNoteNo = CAST(ISNULL(buffbranchno,'') AS VARCHAR(3)) + CAST(ISNULL(s.buffno,'') AS VARCHAR(7)),      
  s.VanNo = 'DHL',      
  s.datepicklistprinted = @DateBegin   
 FROM  schedule s      
  INNER JOIN #schedule st      
   ON st.acctno = s.acctno AND      
    st.itemno = s.itemno AND      
    st.delorcoll = s.delorcoll
    and st.stocklocn=s.stocklocn       -- UAT59 jec 01/04/10
 WHERE st.delorcoll <> 'X'      
       
      
UPDATE s      
 SET s.DeliveryLineNo = st.ID,      
  s.ConsignmentNoteNo = CAST(ISNULL(buffbranchno,'') AS VARCHAR(3)) + CAST(ISNULL(s.buffno,'') AS VARCHAR(7)),      
  s.datepicklistprinted = @DateBegin     
 FROM  schedule s      
  INNER JOIN #schedule st      
   ON st.acctno = s.acctno AND      
    st.itemno = s.itemno AND      
    st.delorcoll = s.delorcoll
    and st.stocklocn=s.stocklocn      -- UAT59 jec 01/04/10 
 WHERE st.delorcoll = 'X'      
       
SET @err = @@ERROR      
IF @err <> 0       
BEGIN      
 ROLLBACK TRANSACTION      
 INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)      
  VALUES('LOGEXPORT', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))      
 RETURN      
END      
      
      
      
--insert valuese into exportTable so they can be retrieved later and dont lock Schedule table      
--delivery order file      
INSERT INTO scheduledExport      
    ([filename],      
     exportdate,      
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
  contractno,       
  TransactionType,  -- TODO 'DEL','CAN','GRT'      
  custid,        
  CustomerName1,      
  CustomerName2,      
  AddressLine1,      
  AddressLine2,      
  AddressLine3,      
  PostalCode,      
  country ,      
  AddressInstructions,      
  HomePhone,      
  WorkPhone,      
  Mobile,      
  ItemDeliveryInstruction,      
  DeliveryLineNo)      
SELECT distinct  'SCH' + @dateStr + '.DAT',      
        @DateBegin,      
        REPLACE(s.acctno,',',''),      
  REPLACE(s.agrmtno,',',''),      
  case when datedelplan='01-jan-1900' then getdate() else REPLACE(datedelplan,',','') end,      
  REPLACE(s.delorcoll,',',''),      
  REPLACE(s.itemno,',',''),      
  REPLACE(s.stocklocn,',',''),      
  REPLACE(s.quantity,',',''),      
  REPLACE(retstocklocn,',',''),      
  REPLACE(retitemno,',',''),      
  CASE WHEN ISNULL(retval,0) = 0 AND price = 0 then REPLACE(0,',','') 
       WHEN ISNULL(retval,0) = 0 AND s.quantity<0 then REPLACE((s.quantity)*price*-1,',','') 
       WHEN ISNULL(s.retval,0) > 0 THEN s.retval 
       ELSE REPLACE((s.quantity)*price,',','') END,    
  REPLACE(vanno,',',''),      
  REPLACE(buffbranchno,',',''),      
  REPLACE(s.buffno,',',''),      
  REPLACE(loadno,',',''),      
  REPLACE(dateprinted,',',''),      
  REPLACE(printedby,',',''),      
  REPLACE(picklistnumber,',',''),      
  REPLACE(undeliveredflag,',',''),      
  REPLACE(s.contractno,',',''),      
  TransactionType = CASE s.delorcoll      
   WHEN 'D' THEN 'DEL'      
   WHEN 'C' THEN 'GRT'      
   ELSE 'CAN'      
        END,      
  REPLACE(c.custid,',',''),        
  SUBSTRING(REPLACE(c.NAME,',',''),0,35) AS CustomerName1,      
  case when SUBSTRING(REPLACE(c.NAME,',',''),35,35) ='' then '-' else SUBSTRING(REPLACE(c.NAME,',',''),35,35) end AS CustomerName2,      
  REPLACE(cad.cusaddr1,',','') AS AddressLine1,      
  REPLACE(cad.cusaddr2,',','') AS AddressLine2,      
  REPLACE(cad.cusaddr3,',','') AS AddressLine3,      
  REPLACE(cad.cuspocode,',','') AS PostalCode,      
  'MAL' AS country ,
  -- max 300 for address notes and GRT notes   CR1040 jec 14/10/09
  case
  when LEN(cad.notes) + LEN(ISNULL(s.GRTnotes,''))<300 then      
		REPLACE(REPLACE(REPLACE(REPLACE(cad.Notes, CHAR(10) + CHAR(13), ' '),CHAR(10), ' '), CHAR(13), ' '),',','') + ' ' 
		+ ISNULL(REPLACE(REPLACE(s.GRTnotes,',',' '),@NewLine,' '),'')	-- LW72237 jec 09/03/10
  else SUBSTRING(REPLACE(REPLACE(REPLACE(REPLACE(cad.Notes, CHAR(10) + CHAR(13), ' '),CHAR(10), ' '), CHAR(13), ' '),',',''),1,300-(LEN(ISNULL(s.GRTnotes,''))+1)) 
		+ ' ' + (REPLACE(REPLACE(s.GRTnotes,',',' '),@NewLine,' '))	-- LW72237 jec 09/03/10 
  End  AS AddressInstructions,      
  REPLACE(LTRIM(RTRIM(ct1.DialCode)),',','') + REPLACE(LTRIM(ct1.TelNo),',','') AS HomePhone,      
        case when ((replace (isnull(ct1.telno,''),' ',''))+(replace (isnull(ct2.telno,''),' ',''))+(replace (isnull(ct3.telno,''),' ','')))=''      
  then REPLACE(LTRIM(RTRIM(br.telno)),'-','')        
  else REPLACE(LTRIM(RTRIM(ct2.DialCode)),',','') + REPLACE(LTRIM(ct2.TelNo),',','') end AS WorkPhone,      
        REPLACE(LTRIM(RTRIM(ct3.DialCode)),',','') + REPLACE(LTRIM(ct3.TelNo),',','') AS Mobile,      
  REPLACE(REPLACE(REPLACE(REPLACE(l.Notes, CHAR(10) + CHAR(13), ' '),CHAR(10), ' '), CHAR(13), ' '),',','') AS ItemDeliveryInstruction,      
  REPLACE(DeliveryLineNo,',','')      
 FROM schedule s      
 INNER JOIN #schedule st      
   ON s.acctno = st.acctno AND s.itemno = st.itemno AND s.delorcoll = st.delorcoll and s.stocklocn=st.stocklocn 
		and s.buffno=st.buffno	-- UAT44 jec      
 INNER JOIN lineitem l      
   ON s.acctno = l.acctno AND s.itemno = l.itemno and s.stocklocn=l.stocklocn      
 INNER JOIN stockitem stock       
   ON stock.itemno = l.itemno AND l.stocklocn = stock.stocklocn AND stock.itemtype <> 'N' AND stock.category NOT IN (12,82)      
 INNER JOIN custacct ca      
      ON s.acctno = ca.acctno and ca.hldorjnt='H'      
 INNER JOIN customer c       
      ON ca.custid = c.custid and ca.hldorjnt='H'      
 left JOIN custaddress cad      
   ON c.custid = cad.custid AND cad.addtype= l.deliveryAddress AND cad.datemoved IS NULL      
 LEFT OUTER JOIN CustTel ct1      
            ON   ct1.CustId = c.custId AND ct1.TelLocn = 'H' AND ISNULL(ct1.DateDiscon,'') = ''      
    LEFT OUTER JOIN CustTel ct2      
            ON   ct2.CustId = c.custId AND ct2.TelLocn = 'W' AND ISNULL(ct2.DateDiscon,'') = ''      
    LEFT OUTER JOIN CustTel ct3      
            ON   ct3.CustId = c.custId AND ct3.TelLocn = 'M' AND ISNULL(ct3.DateDiscon,'') = ''      
 left outer join branch br      
   ON substring(s.acctno,1,3)=branchno
where not (s.delorcoll='X' and isnull(runno,0)=0   )   
      
SET @err = @@ERROR      
IF @err <> 0       
BEGIN      
 ROLLBACK TRANSACTION      
 INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)      
  VALUES('LOGEXPORT', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))      
 RETURN      
END      
     
UPDATE scheduledExport SET datedelplan =GETDATE() WHERE datedelplan ='1-jan-1900'      
      
      
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
      
      
 SET NOCOUNT ON      
 DECLARE c1 CURSOR READ_ONLY FOR       
  SELECT acctno,agrmtno, datedelplan, delorcoll,itemno,stocklocn,quantity,      
   retstocklocn, retitemno, retval, vanno, buffbranchno,buffno, loadno,      
   dateprinted, printedby, picklistnumber, undeliveredflag, contractno,      
   transactiontype, custid, customername1,customername2, addressline1,      
   addressline2,addressline3,postalcode,country,addressinstructions,      
   HomePhone,WorkPhone,Mobile,itemdeliveryinstruction,deliverylineno      
      
   FROM scheduledExport        
   WHERE RunNo IS null      
   ORDER BY deliverylineno      
 OPEN c1        
 FETCH NEXT FROM c1 INTO @acctno,@agrmtno, @datedelplan, @delorcoll,@itemno,@stocklocn,@quantity,      
   @retstocklocn, @retitemno, @retval, @vanno, @buffbranchno,@buffno, @loadno,      
   @dateprinted, @printedby, @picklistnumber, @undeliveredflag, @contractno,      
   @transactiontype, @custid, @customername1,@customername2, @addressline1,      
   @addressline2,@addressline3,@postalcode,@country,@addressinstructions,      
   @homePhone,@workPhone,@mobile,@itemdeliveryinstruction,@deliverylineno      
        
 WHILE @@FETCH_STATUS = 0        
 BEGIN      
      
if @addressline1 is null or @addressline1=''       
set @addressline1='please contact the branch manager'      
      
if @postalcode is null or @postalcode=''       
set @postalcode='00000'      
      
   set @vchrLine =   CAST(ISNULL(@acctno,'') AS CHAR(12))      
       --+ CAST(ISNULL(@Agrmtno,'') AS CHAR(6))      
       +',' 
       +  ISNULL(CONVERT(CHAR(8), @datedelplan, 112), '        ')      
       +','      
       + CAST(ISNULL(@delorcoll,'') AS CHAR(1))      
       +','      
       + CAST(ISNULL(@itemno,'') AS CHAR(8))      
       +','      
       + CAST(ISNULL(@stocklocn,'') AS CHAR(3))      
       +','      
       + CAST(ISNULL(@quantity,'') AS CHAR(8))      
       +','      
       + CAST(ISNULL(@retitemno,'') AS CHAR(8))      
       +','          
       + CAST(ISNULL(@retstocklocn,'') AS CHAR(3))      
       +','      
       + CAST(ISNULL(@retval,'') AS CHAR(8))      
       +','      
       + CAST(ISNULL(@vanno,'') AS CHAR(3))      
       +','      
       + CAST(ISNULL(@buffbranchno,'') AS CHAR(3))      
       +','      
       + CAST(ISNULL(@buffno,'') AS CHAR(10))      
       +','      
       + CAST(ISNULL(@loadno,'') AS CHAR(3))      
       +','      
       + ISNULL(CONVERT(CHAR(8), @dateprinted, 112), '        ')      
       +','      
       + CAST(ISNULL(@printedby,'') AS CHAR(6))      
       +','      
       + CAST(ISNULL(@picklistnumber,'') AS CHAR(6))      
       +','      
       + CAST(ISNULL(@undeliveredflag,'') AS CHAR(1))      
       +','      
--       + CAST(ISNULL(@contractno,'') AS CHAR(10))      
--       +','      
       + CAST(ISNULL(@transactiontype,'') AS CHAR(3))      
       +','      
       + CAST(ISNULL(@custid,'') AS CHAR(20))      
       +','      
       + CAST(ISNULL(@customername1,'') AS CHAR(60))      
       +','      
       + CAST(ISNULL(@customername2,'') AS CHAR(30))      
       +','      
       + CAST(ISNULL(@addressline1,'please contact the branch manager') AS CHAR(50))      
       +','      
       + CAST(ISNULL(@addressline2,'') AS CHAR(50))      
       +','      
       + CAST(ISNULL(@addressline3,'') AS CHAR(50))      
       +','      
       + CAST(ISNULL(@postalcode,'00000') AS CHAR(10))      
       +','      
       + CAST(ISNULL(@country,'') AS CHAR(3))      
       +','      
       + CAST(ISNULL(@addressinstructions,'') AS CHAR(300))      
       +','      
       + CAST(ISNULL(@homePhone,'') AS CHAR(28))      
       +','      
       + CAST(ISNULL(@workPhone,'') AS CHAR(28))      
       +','      
       + CAST(ISNULL(@mobile,'') AS CHAR(28))      
       +','      
       + CAST(ISNULL(@itemdeliveryinstruction,'') AS CHAR(200))      
       +','      
       + CAST(ISNULL(@deliverylineno,'') AS CHAR(4))      
      
      
   EXEC @RC = sp_OAMethod @vchrFileID, 'WriteLine', Null , @vchrLine      
      
   IF @RC <> 0      
   BEGIN      
    ROLLBACK TRANSACTION      
    INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)      
     VALUES('LOGEXPORT', @runno, @DateBegin,0,'Error:  Writing string data to file')      
    RETURN      
   END      
      
        
      
   FETCH NEXT FROM c1 INTO @acctno,@agrmtno, @datedelplan, @delorcoll,@itemno,@stocklocn,@quantity,      
    @retstocklocn, @retitemno, @retval, @vanno, @buffbranchno,@buffno, @loadno,      
    @dateprinted, @printedby, @picklistnumber, @undeliveredflag, @contractno,      
    @transactiontype, @custid, @customername1,@customername2, @addressline1,      
    @addressline2,@addressline3,@postalcode,@country,@addressinstructions,      
    @homePhone,@workPhone,@mobile,@itemdeliveryinstruction,@deliverylineno      
 END       
 CLOSE c1      
 DEALLOCATE c1      
      
PRINT 'update schedule'      
      
UPDATE s      
 SET s.RunNo = @runNo      
 FROM schedule s      
  INNER JOIN #schedule st      
   ON s.acctno = st.acctno AND      
    s.itemno = st.itemno      
    and s.stocklocn=st.stocklocn      
 WHERE s.delorcoll <> 'X' 
	and s.runno is null			-- UAT44 jec 30/03/10    
       
INSERT INTO ScheduleAudit (      
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
 Picklistnumber,      
 undeliveredflag,      
 datePicklistprinted,      
 picklistbranchNumber,      
 runNo,      
 DHLPickingDate,      
 DHLDNNo,      
 consignmentNoteNo,      
 deliveryLineNo      
) SELECT distinct origbr,s.acctno,agrmtno,datedelplan,s.delorcoll,s.itemno,s.stocklocn,quantity,retstocklocn,retitemno,retval,vanno,buffbranchno,s.buffno,      
loadno,dateprinted,printedby,Picklistnumber,undeliveredflag,datePicklistprinted,picklistbranchNumber,runNo,DHLPickingDate,DHLDNNo,consignmentNoteNo,deliveryLineNo      
FROM  schedule s      
  LEFT OUTER JOIN #schedule st      
   ON s.acctno = st.acctno AND      
    s.itemno = st.itemno AND      
     s.stocklocn=st.stocklocn and      
    s.delorcoll = st.delorcoll      
WHERE s.delorcoll = 'X'      
and not exists  
(select 'x' from ScheduleAudit x  
where x.acctno = s.acctno AND      
    x.itemno = s.itemno AND      
     x.stocklocn=s.stocklocn and      
    x.delorcoll = s.delorcoll)    
 
SET @err = @@ERROR      
IF @err <> 0       
BEGIN      
 ROLLBACK TRANSACTION      
 INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)      
  VALUES('LOGEXPORT', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))      
 RETURN      
END      
      
      
print 'update schedulexport'      
UPDATE se      
 SET se.RunNo = @runNo      
 FROM scheduledExport se      
  INNER JOIN #schedule st      
   ON se.acctno = st.acctno AND      
    se.itemno = st.itemno      
	and se.stocklocn=st.stocklocn      
 WHERE se.runno is null			-- UAT44 jec 30/03/10 - dont update previous rows
      
SET @err = @@ERROR      
IF @err <> 0       
BEGIN      
 ROLLBACK TRANSACTION      
 INSERT INTO interfaceerror(interface, runno,errordate,severity, errortext)      
  VALUES('LOGEXPORT', @runno, @DateBegin,0,'error number: ' + CAST(@err AS VARCHAR(10)))      
 RETURN      
END      
      
      
PRINT 'update interfacecontrol'      
update interfacecontrol      
 SET datefinish = getdate() , result = 'P'      
 where interface = 'LOGEXPORT' AND runno =@runno      


DELETE FROM schedule FROM schedule s        
 WHERE s.delorcoll = 'X' and runno is not null     
       
COMMIT TRANSACTION      
          
           
EXECUTE @RC = sp_OADestroy @vchrFileID      
EXECUTE @RC = sp_OADestroy @FS      
      
end      
      
-- End End End End End End End End End End End End End End End End End End End End End End End End End End