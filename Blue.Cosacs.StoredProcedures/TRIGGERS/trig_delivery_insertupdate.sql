IF EXISTS (SELECT * FROM sysobjects 
           WHERE name = 'trig_delivery_insertupdate')
	DROP TRIGGER trig_delivery_insertupdate



GO
    
    
CREATE TRIGGER trig_delivery_insertupdate    
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : trig_delivery_insertupdate.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        : 
-- Author       : ??      
-- Date         : ??      
--        
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 13/07/11  IP  RI - #4266 - When setting contractno to '' need to check category not in code category WAR.
--				 Previously just checked not in (12, 82)
-- 28/07/11  IP  RI - #4424 - Missing bracket
-- 11/10/11  jec CR1232 #3291 new disbursement screen - print receipt button
-- 10/03/15  IP  Cash Loan Disbursement - prevent writing fintrans record for CLD. Now
--               done in c# code
-- =================================================================================     
on [dbo].[delivery]    
FOR INSERT    
AS       
    
BEGIN    
DECLARE @error SQLText    
-- DECLARE @acctno char(12) ,@itemno varchar (10),@stocklocn smallint,@error varchar (256),    
--@category integer, @delivery_count smallint,@order_count integer,@transvalue money ,@contractno varchar(12),    
--@buffno integer,@buffbranchno smallint,@newcontractno varchar(12),@delorcoll char(1),@retitemno varchar (10),     
--@runno integer,@itemtype varchar(1),@olditemno varchar(10), @agrmtno int    

--IP - 13/04/10 - UAT(70) UAT5.2  - Legacy Cash & Go Return
-- jec 09/05/11 - CR1212 RI Integration - use ItemID and increase Itemno columns to varchar18 
--IP - 30/06/11 - CR1212 - RI - #3987 - RI System Integration changes   
DECLARE @LegacyCashGoRet BIT

 If EXISTS(SELECT * FROM INSERTED i
						WHERE i.acctno LIKE '___5%'
						AND i.agrmtno = 1
						AND i.quantity < 0)
BEGIN
		SET @LegacyCashGoRet = 1
END
ELSE
BEGIN
		SET @LegacyCashGoRet = 0
END
 
--Only perform the below checks if this is not a Legacy Cash & Go Return.
IF (@LegacyCashGoRet = 0)
BEGIN   
	 IF EXISTS (SELECT *     
		 FROM INSERTED i   
			    WHERE EXISTS (SELECT 1 FROM stockinfo si  --IP - 30/06/11 - CR1212 - RI - #3987 - Replaces the below code  
							 WHERE category IN (select distinct code from code where category = 'WAR')    
							 AND i.ItemID = si.ID)   
			   --WHERE EXISTS (SELECT 1 FROM stockitem s    
						--	 WHERE category IN (select distinct code from code where category = 'WAR')    
						--	 AND i.itemno = s.itemno)    
			   AND contractno = ''    
			   -- again check only for inserts    
			   --AND NOT EXISTS (SELECT * FROM deleted d WHERE i.acctno= d.acctno AND i.itemno= d.itemno 
			   AND NOT EXISTS (SELECT * FROM deleted d WHERE i.acctno= d.acctno AND i.ItemID= d.ItemID	--IP - 30/06/11 - CR1212 - RI - #3987     
				  AND i.stocklocn=  d.stocklocn AND i.agrmtno = d.agrmtno    
				  AND i.contractno = i.contractno )                              )    
	BEGIN    
	 SET @error ='Contract number not set for warranty. Contact BBS support! -Trig_delivery_insertupdate'    
	 RAISERROR(@error, 16, 1)     
	 ROLLBACK    
	END    
	    
	IF EXISTS (SELECT *     
		 FROM INSERTED i INNER JOIN StockInfo s on i.itemid=s.id   -- RI jec 06/06/11
			   WHERE NOT EXISTS (SELECT *     
								 FROM lineitem l  
								 WHERE l.acctno = i.acctno    
								 AND l.agrmtno = i.agrmtno    
								 --AND l.itemno = i.itemno
								 AND l.itemID = i.itemID     
								 AND l.stocklocn = i.stocklocn    
								 AND l.contractno = i.contractno)    
			   --AND itemno not in ( 'RB' ,'ADDDR','ADDCR','REFINCR','REFINDR')
			   AND IUPC not in ( 'RB' ,'ADDDR','ADDCR','REFINCR','REFINDR')		-- RI jec 06/06/11
	   AND NOT EXISTS (SELECT * FROM deleted d WHERE i.acctno= d.acctno AND i.itemID= d.itemID --i.itemno= d.itemno     
				  AND i.stocklocn=  d.stocklocn AND i.agrmtno = d.agrmtno    
				  AND i.contractno = i.contractno )                                  
			   ) -- only for inserts    
	BEGIN    
	 SET @error ='Delivered item not in lineitem table. Contact BBS support! -Trig_delivery_insertupdate'    
	 RAISERROR(@error, 16, 1)
	 ROLLBACK    
	END 
   
END    
    
--AA - removing as this seems to be a new check and cannot see reason for it   
IF EXISTS (SELECT *						--IP - 30/06/11 - CR1212 - RI - #3987 - Replaces below code
     FROM INSERTED i     
           WHERE delorcoll IN ('C','R')    
           AND (RetItemID = 0 OR ISNULL(retstocklocn,0) = 0 OR retval IS NULL))
 
--IF EXISTS (SELECT *     
--     FROM INSERTED i     
--           WHERE delorcoll IN ('C','R')    
--           AND (ISNULL(retitemno,'') = '' OR ISNULL(retstocklocn,0) = 0 OR retval IS NULL    
--           OR retitemno IS NULL OR retstocklocn IS NULL)         
-- )
    
BEGIN  
  
 IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'Delivery_Error_log')
 BEGIN
	SELECT * 
	INTO Delivery_Error_log 
	FROM INSERTED
 END  
 ELSE
 BEGIN
	INSERT INTO Delivery_Error_log
	(origbr, acctno, agrmtno, datedel, 
	delorcoll, itemno, stocklocn, quantity, 
	retitemno, retstocklocn, retval, buffno, 
	buffbranchno, datetrans, branchno, transrefno, 
	transvalue, runno, contractno, ReplacementMarker, 
	NotifiedBy, ftnotes, InvoiceLineNo, ExtInvoice,parentItemno,ItemID,ParentItemID,RetItemID)		-- RI
	SELECT origbr, acctno, agrmtno, datedel, 
	--delorcoll, itemno, stocklocn, quantity,
	delorcoll, '', stocklocn, quantity, 
	retitemno, retstocklocn, retval, buffno, 
	buffbranchno, datetrans, branchno, transrefno, 
	transvalue, runno, contractno, ReplacementMarker,
	--NotifiedBy, ftnotes, InvoiceLineNo, ExtInvoice,ParentItemno 
	NotifiedBy, ftnotes, InvoiceLineNo, ExtInvoice,'',ItemID,ParentItemID,RetItemID			-- RI
	FROM inserted 
	
 END
 
 UPDATE d
 --SET retitemno = CASE WHEN ISNULL(d.retitemno,'') = '' THEN d.itemno ELSE d.retitemno END,
  SET RetItemID = CASE WHEN ISNULL(d.RetItemID,0) = 0 THEN d.itemID ELSE d.RetItemID END,		-- RI
	 retstocklocn = CASE WHEN ISNULL(d.retstocklocn,'') = '' THEN d.stocklocn ELSE d.retstocklocn END,
	 retval = CASE WHEN d.retval IS NULL THEN d.transvalue ELSE d.retval END
 FROM delivery d
 INNER JOIN INSERTED i ON d.acctno = i.acctno   
  WHERE d.agrmtno = i.agrmtno    
  --AND d.itemno = i.itemno 
  AND d.itemID = i.itemID					-- RI   
  AND d.stocklocn = i.stocklocn    
  AND d.contractno = i.contractno
END    
 
--IP - 30/06/11 - CR1212 - RI - #3987 - Replaces the below code
UPDATE delivery     
set contractno=''     
FROM delivery d inner join INSERTED i on d.acctno = i.acctno
and d.agrmtno = i.agrmtno  
inner join StockInfo si on si.ID = d.ItemID
where  d.itemID =i.itemID				-- RI        
and d.buffno =i.buffno     
and d.buffbranchno =i.buffbranchno     
and d.contractno=i.contractno    
AND i.contractno != ''    
AND si.iupc not like '19%'     
and si.iupc not like 'XW%'     
and i.acctno not like '___5%'     
--AND EXISTS (SELECT * FROM StockInfo s    
--            WHERE s.ID = i.ItemID    
--            AND s.category NOT IN ('12','82','11','50','51','52','53','54','55','56','57','58','59'))  
AND EXISTS (SELECT * FROM StockInfo s    
            WHERE s.ID = i.ItemID    
            AND s.category NOT IN ('11','50','51','52','53','54','55','56','57','58','59')
            AND s.category NOT IN (select code from code where category = 'WAR')             --IP - 13/07/11 - RI - #4266 
            AND s.itemno NOT IN (select code from code where category = 'RDYAST'))   
       
--UPDATE delivery     
--set contractno=''     
--FROM delivery d, INSERTED i    
--where d.acctno =i.acctno     
----and d.itemno =i.itemno 
--and d.itemID =i.itemID				-- RI    
--and d.agrmtno = i.agrmtno     
--and d.buffno =i.buffno     
--and d.buffbranchno =i.buffbranchno     
--and d.contractno=i.contractno    
--AND i.contractno != ''    
--AND i.itemno not like '19%'     
--and i.itemno not like 'XW%'     
--and i.acctno not like '___5%'     
--AND EXISTS (SELECT * FROM StockInfo s    
--            WHERE s.itemno = i.itemno    
--            AND s.category NOT IN ('12','82','11','50','51','52','53','54','55','56','57','58','59'))    
     
            
 update fintrans     
 set transvalue = transvalue + isnull ((select sum(i.transvalue) from     
 inserted i INNER JOIN StockInfo s on i.itemid=s.id		-- RI  jec 06/06/11
 where fintrans.acctno =i.acctno and fintrans.datetrans =i.datedel and     
 fintrans.branchno =i.branchno and fintrans.transrefno =i.transrefno and isnull(i.ftnotes,'') !='ORLD' and isnull(i.ftnotes,'') not like 'XX%'     
 --and itemno not in ('ADDDR','ADDCR')), transvalue)
 and IUPC not in ('ADDDR','ADDCR')), transvalue)    -- RI  jec 06/06/11
 where exists (select * from inserted i where fintrans.acctno =i.acctno and fintrans.datetrans =i.datedel and     
 fintrans.branchno =i.branchno and fintrans.transrefno =i.transrefno)    
    
 -- Table Variable     
 declare @delivery table  (    
  acctno char (12) NOT NULL ,    
    transtypecode varchar(3),    
  agrmtno int NOT NULL ,    
  datedel datetime NOT NULL ,    
  delorcoll char (1) NOT NULL ,    
  itemno varchar (18) NOT NULL ,    
  stocklocn smallint NOT NULL ,    
  quantity float NOT NULL ,    
  retitemno varchar (8) NULL ,    
  retstocklocn smallint NULL ,    
  retval float NULL ,    
  buffno int NOT NULL ,    
  buffbranchno smallint NOT NULL ,    
  datetrans datetime NULL ,    
  branchno smallint NULL ,    
  transrefno int NULL ,    
  transvalue money NULL ,    
  runno int NOT NULL ,    
  contractno varchar (10) NOT NULL ,    
  ReplacementMarker varchar (5) NULL ,    
  NotifiedBy int NOT NULL ,    
  ParentItemNo VARCHAR(18) NOT NULL,
    ftnotes varchar(4) not null,
  ItemID INT, ParentItemID Int				-- RI
    --primary key  (acctno, agrmtno, itemno, stocklocn, buffno, contractno) )
    primary key  (acctno, agrmtno, itemID, stocklocn, buffno, contractno, ParentItemID) ) --IP - 30/06/11 - CR1212 - RI - #3987 - added ParentItemID   -- RI
    
    insert into @delivery    
    (acctno,agrmtno,datedel,    
    delorcoll,itemno,stocklocn,    
    quantity,retitemno,retstocklocn,    
    retval,buffno,buffbranchno,    
    datetrans,branchno,transrefno,    
    transvalue,runno,contractno,    
    ReplacementMarker,NotifiedBy,ftnotes,    
    transtypecode, parentitemno,
    ItemID, ParentItemID						-- RI 
     )    
    select acctno,agrmtno,datedel,    
    --delorcoll,itemno,stocklocn,
    delorcoll,s.IUPC,stocklocn,					-- RI   
    quantity,retitemno,retstocklocn,    
    retval,buffno,buffbranchno,    
    datetrans,branchno,transrefno,    
    transvalue,runno,contractno,    
    ReplacementMarker,NotifiedBy,isnull(ftnotes,'dtrg'),    
    'DEL' ,ParentItemNo,
    ItemID, ParentItemID 				-- RI
    from inserted i 
		INNER JOIN stockinfo s on i.itemID=s.ID
    where not exists (    
   select * from fintrans where fintrans.acctno =i.acctno and fintrans.datetrans =i.datedel and     
   fintrans.branchno =i.branchno and fintrans.transrefno =i.transrefno)    
    and isnull(i.ftnotes,'') !='ORLD' and isnull(i.ftnotes,'') !='XX'	-- and itemno not in ('ADDDR','ADDCR')
		and s.IUPC not in ('ADDDR','ADDCR')			-- RI 
        
   /* Need to determine transtypecode before posting - need to do big testplan on this.*/    
  update @delivery set transtypecode = 'REB',datedel =getdate() where itemno ='RB'    
    
  update @delivery set transtypecode = 'GRT' where quantity <0     
         
  update @delivery set transtypecode = 'ADD' where itemno in ('ADDCR','ADDDR')   
  
  update @delivery set transtypecode = 'RFN' where itemno in ('REFINDR','REFINCR')	--CR976  Refinance Arrangements  jec
  update @delivery set transtypecode = 'RFD' where itemno = 'REFINCR' and transvalue>0 	--CR976  Refinance Deposit  jec 
         
  update @delivery set transtypecode = 'REP' where quantity <0 AND delorcoll ='R'    
    
  update @delivery set transtypecode = 'RDL' where quantity >0 AND delorcoll ='R'
  
  update @delivery set transtypecode = 'CLD' where itemno = 'LOAN' and transvalue>0 	--CR1232 Cash Loan  jec
      
  
-- Installation charges for spare parts
	UPDATE d  
	SET transtypecode = 'INE'
	FROM @delivery d
	WHERE d.acctno = (SELECT valuestring FROM Config.Setting
                      WHERE id = 'InstallElectrical')

	UPDATE @delivery 
	SET transtypecode = 'INF'
	FROM @delivery d
	WHERE d.acctno = (SELECT valuestring FROM Config.Setting
                     WHERE id = 'InstallFurniture')
  
    
  if exists(select * from @delivery)    
  BEGIN
  	UPDATE f SET transvalue = f.transvalue + d.transvalue
  	FROM @delivery d ,fintrans f
  	WHERE d.acctno= f.acctno 
  	AND d.transrefno= f.transrefno AND d.datetrans =f.datetrans 
  	AND d.branchno= f.branchno 
  	
   insert into fintrans    
   (origbr,branchno,acctno,transrefno,    
    empeeno,transupdated,transprinted,    
    transvalue,bankcode,bankacctno,chequeno,    
    ftnotes,paymethod,runno,source,transtypecode,    
    datetrans, agrmtno)    
    select    
    branchno,branchno,acctno,transrefno,    
    notifiedby ,'N','N',    
    sum(transvalue),'','','',    
    isnull (ftnotes,'TDEL'),
    0,
    0,'COSACS',min(transtypecode),    
    min(datetrans), agrmtno		-- CR1232 jec need time on transaction
    from @delivery d 
    WHERE NOT EXISTS (SELECT * FROM fintrans f WHERE f.acctno= d.acctno AND f.transrefno=d.transrefno
    AND f.datetrans = d.datetrans AND f.branchno= d.branchno)
    AND transtypecode != 'CLD'  --Cash Loan Disbursement handled below
    group by branchno,acctno,transrefno,notifiedby,ftnotes, agrmtno  

    --Cash Loan Disbursement (CLD)
    insert into 
        fintrans  (origbr,branchno,acctno,transrefno,    
                    empeeno,transupdated,transprinted,    
                    transvalue,bankcode,bankacctno,chequeno,    
                    ftnotes,paymethod,runno,source,transtypecode,    
                    datetrans, agrmtno)  
    select    
        branchno,branchno,d.acctno,transrefno,    
        notifiedby ,'N','N',    
        sum(transvalue),cld.Bank,cld.BankAcctNo,cld.ChequeCardNo,    
        isnull (ftnotes,'TDEL'),
        cld.DisbursementType,		-- CR1232 Paymethod = 1 for cash loan disbursement
        0,'COSACS',min(transtypecode),    
        min(datetrans), d.agrmtno		-- CR1232 jec need time on transaction
    from 
        @delivery d 
    inner join 
        CashLoanDisbursement cld on d.acctno = cld.acctno
        and d.agrmtno = cld.agrmtno
    where 
        NOT EXISTS (SELECT * FROM fintrans f WHERE f.acctno= d.acctno AND f.transrefno=d.transrefno
        AND f.datetrans = d.datetrans AND f.branchno= d.branchno)
        AND transtypecode = 'CLD'
    group by branchno, d.acctno, transrefno, notifiedby, ftnotes, d.agrmtno, cld.DisbursementType, cld.Bank, cld.BankAcctNo, cld.ChequeCardNo   
  END
    
       -- Show RFD transaction as Credit and Debit    -- CR976 Refinanced Arrangements  
    if exists(select * from @delivery where transtypecode='RFD')
	begin  
	update @delivery set transvalue=transvalue*-1			-- show as a credit in Fintrans
				 where itemno = 'REFINCR' and transtypecode = 'RFD' and transvalue>0 	--CR976  Refinance Deposit  jec
  	insert into fintrans
	 (origbr,branchno,acctno,transrefno,
	  empeeno,transupdated,transprinted,
	  transvalue,bankcode,bankacctno,chequeno,
	  ftnotes,paymethod,runno,source,transtypecode,
	  datetrans, agrmtno)
	  select
	  branchno,branchno,acctno,transrefno,
	  notifiedby ,'N','N',
	  sum(transvalue),'','','',
	  isnull (ftnotes,'TDEL'),0,0,'COSACS',min(transtypecode),
	  min(datetrans), agrmtno		-- CR1232 jec need time on transaction
	  from @delivery
	  group by branchno,acctno,transrefno,notifiedby,ftnotes, agrmtno
	End
    
 IF EXISTS (SELECT * FROM CountryMaintenance WHERE codeNAME = 'OracleLineExport' AND value IN ('F','P','L'))    
 BEGIN -- populate the export table for interface to Oracle....     
    -- AA Mauritius found part delivery -- really don't think we need to do an update when inserting into delivery table??    
    -- maybe should remove???    
 UPDATE l    
  set    
  quantity = l.quantity + i.quantity,    
  ordval = l.ordval  + i.transvalue     
  FROM LineitemOracleExport l ,inserted i --, lineitem li    
  WHERE l.acctno= i.acctno AND l.agrmtno=i.agrmtno AND l.itemno = i.itemno    
  AND l.stocklocn = i.stocklocn AND l.contractno = i.contractno AND l.runno= 0 AND l.type = 'D'    
  --AND li.acctno = i.acctno AND li.itemno = i.itemno AND li.stocklocn = i.stocklocn AND li.agrmtno = i.agrmtno    
  AND l.buffno= i.buffno     
  --AND li.contractno = l.contractno     
   --if updating the existing buffno --- this looks ok.... */    
  -- remove any existing order -- actually leave for now... 
  
-- removed block commented out code from here for readability jec02/02/11 
  
  -- check if previous collection or repo - if so we need to send another order..    
      
  select i.*, l.orderlineno,  l.orderno    
  into #inserted     
  FROM inserted i    
  LEFT OUTER JOIN lineitem l ON     
  --l.acctno = i.acctno AND l.itemno = i.itemno AND l.stocklocn = i.stocklocn AND l.agrmtno = i.agrmtno    
  l.acctno = i.acctno AND l.ItemID = i.ItemID AND l.stocklocn = i.stocklocn AND l.agrmtno = i.agrmtno  --IP - 30/06/11 - CR1212 - RI - #3987  
  AND l.contractno = i.contractno     
      
      
 -- Setting orderline number to -1 for items that have no orders in lineitemorexport table     
 -- removed block commented out code from here for readability jec02/02/11    
      
      
  BEGIN    
   INSERT INTO LineitemOracleExport (    
   acctno,  agrmtno,    
   itemno,  contractno,    
   quantity, stocklocn,    
   ordval,  [type],    
   runno,  buffno,    
   orderlineno,  orderno)     
   SELECT i.acctno, i.agrmtno,     
   i.itemno, i.contractno,    
   i.quantity, i.stocklocn,    
   i.transvalue , 'O',    
   0,  0,    
   orderlineno,  orderno -- the -1 is for a different line number.....     
   FROM #inserted i    
   --join StockInfo S ON s.itemno= i.itemno    
   join StockInfo S ON s.ID= i.ItemID				--IP - 30/06/11 - CR1212 - RI - #3987
   --WHERE i.itemno= s.itemno AND (s.itemtype = 'S' OR s.category IN (select distinct code from code where category = 'WAR') )  
   WHERE i.ItemID= s.ID AND (s.itemtype = 'S' OR s.category IN (select distinct code from code where category = 'WAR') ) --IP - 30/06/11 - CR1212 - RI - #3987   
   and orderlineno = -1     
       
   --AND NOT EXISTS (SELECT * FROM lineitemOracleExport X WHERE  X.ACCTNO= I.ACCTNO AND X.ITEMNO= I.ITEMNO AND x.stocklocn = i.stocklocn     
   --AND x.contractno= i.contractno AND i.agrmtno = i.agrmtno and x.type = 'O')     
  END    
      
  -- insert delivery records where not already in the table for runno of zero    
  INSERT INTO LineitemOracleExport (    
   acctno,  agrmtno,    
   itemno,  contractno,    
   quantity,      
   stocklocn,    
   ordval,  [type],    
   runno,  buffno,    
   orderlineno,  orderno)     
  SELECT i.acctno,  i.agrmtno,    
   i.itemno,  i.contractno,    
   --CASE WHEN i.quantity >0 THEN ISNULL(l.quantity,1) --if delivery then use order quantity    
   --ELSE     
    i.quantity  -- else use delivered quantity for collections and reposessions    
    --RM 04-01-2010 always should use delivered quantity    
   --END     
    , i.stocklocn,     
   i.transvalue,  'D',-- Delivery    
   0,  i.buffno,    
   i.orderlineno,  i.orderno    
   FROM #inserted i    
   LEFT JOIN lineitem l    
   --ON l.acctno = i.acctno AND l.itemno = i.itemno AND l.stocklocn = i.stocklocn AND l.agrmtno = i.agrmtno  
   ON l.acctno = i.acctno AND l.ItemID = i.ItemID AND l.stocklocn = i.stocklocn AND l.agrmtno = i.agrmtno  --IP - 30/06/11 - CR1212 - RI - #3987  
   AND l.contractno = i.contractno    
   WHERE NOT EXISTS (SELECT * FROM LineitemOracleExport li WHERE     
   li.acctno= i.acctno AND li.agrmtno=i.agrmtno AND li.itemno = i.itemno    
   AND li.stocklocn = i.stocklocn AND li.contractno = i.contractno AND li.runno= 0 and type ='D'     
   AND li.buffno= i.buffno     
   AND li.ordval = i.transvalue     
    )     
       
   -- remove existing orders    
  END    
    
 END    
    
