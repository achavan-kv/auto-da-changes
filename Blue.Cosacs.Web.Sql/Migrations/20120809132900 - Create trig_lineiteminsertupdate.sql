-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #13576


IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'trig_lineiteminsertupdate')
DROP TRIGGER  trig_lineiteminsertupdate
GO 

create trigger [dbo].[trig_lineiteminsertupdate]     
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : trig_lineiteminsertupdate.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/05/11  IP RI Integration changes - CR1212 - #3668 - Use ItemID when joining
-- 25/05/12  IP #10225 - Warehouse & Deliveries Integration - Specify columns as IDENTITY column now
--				added to LineItem table.
-- ================================================ 
	on [dbo].[lineitem]      
	for update, INSERT,DELETE   as      
	SET NOCOUNT ON       
	   
	DECLARE @acctno varchar (12), @itemno varchar (12), @testing SMALLINT           
	          
	-- creating export table to reduce locking          
	--SELECT  'tetst' AS [fff],* FROM @ExportTable          
	          
	if @itemno = 'refin'           
	begin          
	   update agreement set deliveryflag ='Y' where acctno = @acctno          
	END          
	          
	DECLARE @minrundate DATETIME             
	SELECT @minrundate = ISNULL(MIN(datestart),GETDATE()) FROM interfacecontrol WHERE interface = 'OrInteg2'           
	        
	-- because inserted and deleted don't have indexes on - in order to improve performance putting these into a new          
	-- #tables. Mauritius actually hung when 18,000 rows updated on lineitem table.           
	--SELECT TOP 0 * INTO #insert FROM inserted  --IP - 25/05/12 - #10225 - Replaced with the below
	SELECT TOP 0 origbr, acctno, agrmtno, itemno, itemsupptext, quantity, delqty, stocklocn, price, ordval, datereqdel, 
				timereqdel, dateplandel, delnotebranch, qtydiff, itemtype, notes, taxamt, isKit, deliveryaddress, parentitemno, parentlocation, 
				contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired, damaged, OrderNo, Orderlineno, 
				PrintOrder, taxrate, ItemID, ParentItemID, SalesBrnNo,Express	-- #10229
	INTO #insert FROM inserted            
	          
	CREATE CLUSTERED INDEX ix_insert_lineitem2324 ON #insert (acctno,itemno,agrmtno, stocklocn,contractno )          
	          
	CREATE INDEX ix_insertStock ON #insert(itemno,stocklocn, itemtype , qtydiff)          
	          
	SELECT TOP 0 * INTO #delete FROM deleted          
	CREATE CLUSTERED INDEX ix_delete_lineitem2324 ON #delete (acctno,itemno,agrmtno, stocklocn,contractno )          
	          
	--INSERT INTO #insert SELECT * FROM inserted    --IP - 25/05/12 - #10225 - Replaced with the below  
	INSERT INTO #insert SELECT origbr, acctno, agrmtno, itemno, itemsupptext, quantity, delqty, stocklocn, price, ordval, datereqdel,
				 timereqdel, dateplandel, delnotebranch, qtydiff, itemtype, notes, taxamt, isKit, deliveryaddress, parentitemno, parentlocation,
				 contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired, damaged, OrderNo, Orderlineno, 
				 PrintOrder, taxrate, ItemID, ParentItemID, SalesBrnNo,Express	-- #10229
	FROM inserted           
	          
	--INSERT INTO #delete SELECT * FROM deleted   --IP - 25/05/12 - #10225 - Replaced with the below
	INSERT INTO #delete SELECT origbr, acctno, agrmtno, itemno, itemsupptext, quantity, delqty, stocklocn, price, ordval, datereqdel, 
				timereqdel, dateplandel, delnotebranch, qtydiff, itemtype, notes, taxamt, isKit, deliveryaddress, parentitemno, parentlocation, 
				contractno, expectedreturndate, deliveryprocess, deliveryarea, DeliveryPrinted, assemblyrequired, damaged, OrderNo, Orderlineno, 
				PrintOrder, taxrate, ItemID, ParentItemID, SalesBrnNo, Express	-- #10229
	FROM deleted          
	        
	IF EXISTS (select *           
		   from #insert i, #delete d          
		   where i.acctno = d.acctno           
		   and i.qtydiff ='N'           
		   and d.qtydiff !='N'           
		   --and d.itemno = i.itemno  
		   and d.ItemID = i.ItemID					--IP - 19/05/11 - CR1212 - #3668		         
		   and d.stocklocn = i.stocklocn          
		   and d.contractno = i.contractno          
		   AND d.agrmtno= i.agrmtno       )          
	BEGIN          
	 delete           
	 from lineitemosdelnotes           
	 where exists (select *           
		 from #insert i, #delete d          
		 where i.acctno =d.acctno           
		 and i.qtydiff ='N'           
		 and d.qtydiff !='N'           
		 --and d.itemno =i.itemno 
		 and d.ItemID = i.ItemID					--IP - 19/05/11 - CR1212 - #3668       
		 and d.stocklocn =i.stocklocn          
		 and d.contractno =i.contractno          
		 and d.acctno =lineitemosdelnotes.acctno          
		 and d.agrmtno=lineitemosdelnotes.agrmtno          
		 --and d.itemno =lineitemosdelnotes.itemno  
		 and d.ItemID = lineitemosdelnotes.ItemID	--IP - 19/05/11 - CR1212 - #3668  
		 and d.stocklocn =lineitemosdelnotes.stocklocn          
		 and d.contractno =lineitemosdelnotes.contractno)          
	END           
	-- reduce locking by only inserting/updating if not in the table....          
	if exists (select *           
		from #insert i ,stockitem s          
		where i.qtydiff !='N'           
		--AND i.itemno = s.itemno    
		AND i.ItemID = s.ItemID						--IP - 19/05/11 - CR1212 - #3668    
		AND i.stocklocn = s.stocklocn          
		AND i.itemtype !='N'          
		AND not exists (select *           
		   from lineitemosdelnotes l          
						 where l.acctno = i.acctno           
		   --and l.itemno = i.itemno    
		     and l.ItemID = i.ItemID				--IP - 19/05/11 - CR1212 - #3668       
			 and l.stocklocn = i.stocklocn          
			 and l.delnotebranch=i.delnotebranch))          
	 begin         
	              
	 insert into lineitemosdelnotes(acctno, itemno, stocklocn, contractno,agrmtno,delnotebranch, ItemID)	  --IP - 19/05/11 - CR1212 - #3668          
	 select a.acctno, '', lineitem.stocklocn, lineitem.contractno,          
	  lineitem.agrmtno,lineitem.delnotebranch, lineitem.ItemID												  --IP - 19/05/11 - CR1212 - #3668    
	 from  #insert lineitem , acct a, stockitem           
	 where  lineitem.qtydiff ='Y'          
	 and  a.acctno =lineitem.acctno           
	 --and  stockitem.itemno = lineitem.itemno
	 and  stockitem.ItemID = lineitem.ItemID				--IP - 19/05/11 - CR1212 - #3668  									           
	 and  stockitem.stocklocn = lineitem.stocklocn          
	 and  stockitem.itemtype !='N'           
	 and  a.currstatus !='S'          
	 and  lineitem.quantity > 0          
	 and  not exists (select *           
		   from  custacct           
		   where custacct.hldorjnt= 'H'          
		   and   custacct.acctno =lineitem.acctno           
		   and custacct.custid like 'PAID & TAKEN%')          
	 and  not exists (select *           
	from lineitemosdelnotes l          
						 where l.acctno = A.acctno           
		  -- and l.itemno = lineitem.itemno    
		     and l.ItemID = lineitem.ItemID					--IP - 19/05/11 - CR1212 - #3668     
			 and l.stocklocn = lineitem.stocklocn          
			 and l.delnotebranch=lineitem.delnotebranch) -- RD/AA Added this to resolve issue 67726          
	end  
	        
	IF EXISTS (SELECT * FROM CountryMaintenance WHERE codeNAME = 'OracleLineExport' AND value IN ('F','P','L'))          
	BEGIN -- populate the export table for interface to Oracle....           
	 -- U where quantity changed but not 0           
		-- C where quantity 0 from positive value.... and don't have a delivery for that item          
		-- SET @TESTING = 1          
	              
	              
	              
		IF @testing =1           
		BEGIN          
	  PRINT ' updating oracle export figures '          
	 END           
	 -- so get the details into table so we can update the newstatus type -- will only work for updates and inserts          


	 INSERT INTO TempStatusType           
		SELECT 
		I.AcctNo, 
		I.ItemNo, 
		I.AgrmtNo,
		I.StockLocn, 
		I.ContractNo,          
		Case  When-- new insert with quantity >0 -- was cancelled and now being re-ordered 
				 (D.Quantity is NULL and ISNULL(I.Quantity,0) >0 )  OR (d.quantity = 0 AND i.quantity >0) Then 'O'          
			  When -- quantity or value has been amended so update
				 (I.Quantity is not NULL and (I.Quantity <> D.Quantity OR i.ordval<>d.ordval) and I.Quantity <> 0 AND i.ordval!=0) 
				 then 'U'
			  When -- Cancelled
				  I.Quantity = 0 and ISNULL(D.Quantity,0)>=0 Then 'C'  		
   			  Else '-' --Default value for unknown type          
		End as NewStatusType ,          
		i.quantity,
		i.ordval ,
		CONVERT(VARCHAR(1),'') AS PrevInttoOr, 
		CONVERT(CHAR(1),'N') AS PrevDel,           
		ISNULL(s.itemtype,'S') AS Itemtype, 
		CONVERT(MONEY,0) AS PrevIntVal, 
		CONVERT(FLOAT,0) AS PrevIntQ ,           
		CONVERT(CHAR(1),'') AS deleteflag, 
		'Z' AS Currenttype,          
		CONVERT(CHAR(1),'') AS Del2bX,
		ISNULL(d.quantity,0) AS DELETEdQty ,           
		isnull(d.ordval,0) AS deleteval,
		i.orderno,
		i.orderlineno, 
		CONVERT(MONEY,0) AS PrevDelVal ,          
		CONVERT(FLOAT,0) AS PrevDelQ , 
		CONVERT(FLOAT,0) AS CURRENTDel , 
		ISNULL(s.category ,0) AS category                  
	 FROM #insert I          
	 --LEFT JOIN StockInfo s ON s.itemno= i.itemno 
	 LEFT JOIN StockInfo s ON s.ID = i.ItemID				--IP - 19/05/11 - CR1212 - #3668       
	 LEFT JOIN #delete D ON I.AcctNo = D.AcctNo 
			--and I.ItemNo = D.ItemNo   
			and I.ItemID = D.ItemID							--IP - 19/05/11 - CR1212 - #3668   
			and I.AgrmtNo = D.AgrmtNo and I.StockLocn = D.StockLocn and          
			I.ContractNo = D.ContractNo  
			--and i.itemno!='RB'
			and s.itemno!='RB'       -- RI jec 06/06/11   
	    
	     
	            
	 INSERT INTO TempStatusType           
	 (acctno,itemno,agrmtno,stocklocn,contractno,NewStatusType,quantity,          
	 ordval,deleteval,DELETEdQty,itemtype,deleteflag,          
	 Del2bX,PrevIntQ,PrevIntVal,Currenttype,orderno,orderlineno,          
	 PrevDelVal , PrevDelQ, CURRENTDel, category)                    
	 SELECT  
		d.AcctNo, 
		d.ItemNo, 
		d.AgrmtNo, 
		d.StockLocn, 
		d.ContractNo,
		'C',0,0 ,          
		d.ordval ,
		d.quantity , 
		ISNULL(s.itemtype,'S'),'',          
	 '',0,0,'',orderno,orderlineno,0,0,0 ,0                
	 FROM #delete d           
	 --LEFT JOIN stockinfo s ON s.itemno= d.itemno    
	 LEFT JOIN stockinfo s ON s.ID = d.ItemID				--IP - 19/05/11 - CR1212 - #3668  
	 WHERE NOT EXISTS (SELECT * FROM #insert i           
						WHERE i.acctno= d.acctno 
						--AND i.itemno= d.itemno 
						AND i.ItemId = d.ItemID				--IP - 19/05/11 - CR1212 - #3668  
						AND i.agrmtno = d.agrmtno           
						AND i.stocklocn = d.stocklocn 
						AND isnull(i.contractno,'')= isnull(d.contractno,'')) 
	--and d.itemno!='RB'
	and s.itemno!='RB'		-- RI jec 06/06/11         
	           
	end

	go









