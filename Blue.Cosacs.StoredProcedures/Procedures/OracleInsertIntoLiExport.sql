	if EXISTS (SELECT * FROM sysobjects WHERE NAME ='OracleInsertIntoLiExport')
	drop procedure OracleInsertIntoLiExport
	go 
	CREATE PROCEDURE OracleInsertIntoLiExport
	AS 


	begin tran serializable
	if exists(select 'x' from sys.tables where name='tempstatustype_process')
	drop table tempstatustype_process
	Select * into tempstatustype_process from tempstatustype order by id
	truncate table tempstatustype
	commit;

	CREATE CLUSTERED INDEX ix_tempstatustype_processsfdfd ON tempstatustype_process (acctno,itemno,agrmtno, stocklocn,contractno )          
	-- alter table tempstatustype_process add ID int identity 

	-- get correct orderlineno and orderno for all records 
	/*****SL: (the orderlineno passed into trigger is the first one for item not current one - this needs to be fixed and then this can be removed*/
	update tempstatustype_process
	set acctno=isnull((select top 1 acctno from SR_ChargeAcct
						where substring(cast(agrmtno as varchar),4,10)=ServiceRequestNo),acctno),
	ordval = ISNULL((select top 1 UnitPrice*Quantity  from SR_PartListResolved
						where substring(cast(agrmtno as varchar),4,10)=ServiceRequestNo
						and tempstatustype_process.itemno=SR_PartListResolved.PartNo),ordval),
	quantity = ISNULL((select top 1 quantity  from SR_PartListResolved
						where substring(cast(agrmtno as varchar),4,10)=ServiceRequestNo
						and tempstatustype_process.itemno=SR_PartListResolved.partno and Quantity>0),1)
	where  tempstatustype_process.acctno='730500010630'


		
	 update tempstatustype_process
	 set orderlineno=isnull((select max(orderlineno)
	 from lineitemoracleexport x 
	 where x.acctno=tempstatustype_process.acctno 
	 AND x.contractno= tempstatustype_process.contractno 
	 AND x.agrmtno= tempstatustype_process.agrmtno        
	 AND x.itemno = tempstatustype_process.itemno 
	 AND x.stocklocn = tempstatustype_process.stocklocn),orderlineno)
	 
	 update tempstatustype_process
	 set orderno=isnull((select max(orderno)
	 from lineitemoracleexport x 
	 where x.acctno=tempstatustype_process.acctno 
	 AND x.agrmtno= tempstatustype_process.agrmtno),orderno)
	 
	-- get current statustype from lineitem export
	update tempstatustype_process
	set currenttype=isnull((select max(X.type) from lineitemOracleExport X Where x.acctno =i.acctno AND x.agrmtno= i.agrmtno          
		AND x.stocklocn = i.stocklocn AND x.contractno = i.contractno  AND i.itemno = x.itemno AND x.runno= 0           
		AND x.TYPE !='D'),'Z')        
	from tempstatustype_process i

	 DECLARE @lastexportdate DATETIME        
	 SELECT @lastexportdate = MAX(datefinish) FROM interfacecontrol WHERE interface = 'OrInteg2'         
		 DECLARE @minrundate DATETIME             
	SELECT @minrundate = ISNULL(MIN(datestart),GETDATE()) FROM interfacecontrol WHERE interface = 'OrInteg2'           
	 
	 
	-- if multiple lines for same record - only need latest  
	DELETE FROM tempstatustype_process    
	WHERE EXISTS (    
	 SELECT * FROM tempstatustype_process b    
	 WHERE tempstatustype_process.acctno = b.acctno    
	  AND tempstatustype_process.agrmtno = b.agrmtno    
	  AND tempstatustype_process.itemno = b.itemno    
	  AND tempstatustype_process.stocklocn = b.stocklocn    
	  AND tempstatustype_process.contractno = b.contractno    
	  AND tempstatustype_process.id < b.id  
		AND  b.newstatustype =tempstatustype_process.newstatustype
	 )    

	-- is previously delivered
	UPDATE t  
	 SET PrevDel = 'Y'
	 FROM tempstatustype_process  t        
	 WHERE ( EXISTS (SELECT 'x' FROM lineitemOracleExport  d WHERE         
					d.acctno= t.acctno AND 
					d.itemno= t.itemno AND 
					d.contractno= t.contractno AND 
					d.agrmtno= t.agrmtno AND 
					d.stocklocn = t.stocklocn         
					AND d.runno >0 AND d.TYPE= 'D' 
					AND ISNULL(d.orderlineno,0) = ISNULL(t.orderlineno,0) )         
		OR EXISTS  (SELECT * FROM delivery d WHERE         
					d.acctno= t.acctno AND 
					d.itemno= t.itemno AND 
					d.contractno= t.contractno AND 
					d.agrmtno= t.agrmtno AND 
					d.stocklocn = t.stocklocn AND 
					d.datetrans <@minrundate and delorcoll='D') )         
	 AND t.acctno NOT LIKE '___5%'      
	         
	 UPDATE t  -- previously delivered  and interfaced       
	 SET PrevDelVal = ISNULL((SELECT SUM(d.transvalue ) FROM delivery d , lineitemOracleExport l
	 WHERE         
	 d.acctno= t.acctno AND d.itemno= t.itemno AND d.contractno= t.contractno         
	 AND d.agrmtno= t.agrmtno AND d.stocklocn = t.stocklocn         
	 AND l.acctno= d.acctno AND l.itemno= d.itemno  AND l.TYPE = 'D'        
	 AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno        
	 AND l.contractno = d.contractno AND ISNULL(l.orderlineno,0)= ISNULL(t.orderlineno,0)        
	 AND d.buffno = l.buffno  ) ,0)         
	FROM  tempstatustype_process  t
	                     
	 UPDATE t  -- previously delivered prior to first export        
	 SET PrevDelVal =PrevDelVal  + ISNULL((SELECT SUM(transvalue ) 
	 FROM delivery d WHERE         
	 d.acctno= t.acctno AND d.itemno= t.itemno AND d.contractno= t.contractno AND d.agrmtno= t.agrmtno AND d.stocklocn = t.stocklocn         
	 AND ( d.datetrans <@minrundate) ),0)        
	 FROM  tempstatustype_process  t        
	 WHERE  t.acctno NOT LIKE '___5%'          
	            
	 UPDATE t  -- previously delivered        
	 SET PrevDelQ = ISNULL((SELECT SUM(d.quantity ) FROM delivery d , lineitemOracleExport l         
	 WHERE         
	 d.acctno= t.acctno AND d.itemno= t.itemno AND d.contractno= t.contractno         
	 AND d.agrmtno= t.agrmtno AND d.stocklocn = t.stocklocn         
	 AND l.acctno= d.acctno AND l.itemno= d.itemno AND l.TYPE = 'D'        
	 AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno        
	 AND l.contractno = d.contractno AND ISNULL(l.orderlineno,0)= ISNULL(t.orderlineno,0)        
	 AND d.buffno = l.buffno ) ,0)        
	 FROM  tempstatustype_process  t        
	 WHERE  t.acctno NOT LIKE '___5%'        
	        
	 UPDATE t  -- previously delivered        
	 SET PrevDelQ =PrevDelQ + ISNULL((SELECT SUM(quantity ) 
	 FROM delivery d WHERE         
	 d.acctno= t.acctno
	  AND d.itemno= t.itemno 
	  AND d.contractno= t.contractno 
	  AND d.agrmtno= t.agrmtno AND d.stocklocn = t.stocklocn         
	 AND ( d.datetrans <@minrundate) ),0)        
	 FROM  tempstatustype_process  t        
	 WHERE  t.acctno NOT LIKE '___5%'        
	        



	 -- delivery waiting to be processed but no return
	 UPDATE t SET CURRENTDel=1        
	 FROM tempstatustype_process  t         
	  WHERE EXISTS  (SELECT x.quantity FROM lineitemOracleExport x         
	 WHERE x.acctno=t.acctno AND x.contractno= t.contractno AND x.agrmtno= t.agrmtno        
	 AND x.itemno = t.itemno AND x.stocklocn = t.stocklocn AND x.runno= 0 AND x.TYPE= 'D'         
	 AND ISNULL(x.orderlineno,0) = ISNULL(t.orderlineno,0) )        
	 AND NOT EXISTS   (SELECT x.quantity*-1 FROM lineitemOracleExport x         
	 WHERE x.acctno=t.acctno AND x.contractno= t.contractno AND x.agrmtno= t.agrmtno        
	 AND x.itemno = t.itemno AND x.stocklocn = t.stocklocn AND x.runno= 0 AND x.TYPE= 'D'         
	 AND ISNULL(x.orderlineno,0) = ISNULL(t.orderlineno,0) ) 
	 

	-- delete phantom collections from revise -- not sure what causes these in trigger
	delete from tempstatustype_process
	where quantity=0  and (exists (select 'x' from lineitem x
	WHERE x.acctno=tempstatustype_process.acctno
	 AND x.contractno= tempstatustype_process.contractno 
	 AND x.agrmtno= tempstatustype_process.agrmtno        
	 AND x.itemno = tempstatustype_process.itemno 
	 AND x.stocklocn = tempstatustype_process.stocklocn 
	 and quantity!=0 ) or currentdel=1 or prevdel='Y')

	 
	-- if delivered and collected - or revised to a greater amount set lineno to 0 as we need a new booking for this
	 update t
	 set orderlineno=0, 
	 newstatustype='O'
	 from tempstatustype_process t
	 WHERE ( EXISTS (SELECT 'x' FROM lineitemOracleExport  d WHERE   -- previous return      
					d.acctno= t.acctno AND 
					d.itemno= t.itemno AND 
					d.contractno= t.contractno AND 
					d.agrmtno= t.agrmtno AND 
					d.stocklocn = t.stocklocn         
					AND d.TYPE= 'D' and d.quantity<0
					AND ISNULL(d.orderlineno,0) = ISNULL(t.orderlineno,0) )         
		OR EXISTS  (SELECT * FROM delivery dx WHERE         
					dx.acctno= t.acctno AND 
					dx.itemno= t.itemno AND 
					dx.contractno= t.contractno AND 
					dx.agrmtno= t.agrmtno AND 
					dx.stocklocn = t.stocklocn AND 
					dx.datetrans <@minrundate and delorcoll='C')
		OR EXISTS (SELECT 'x' FROM lineitemOracleExport  d1 WHERE         --previous cancellation
					d1.acctno= t.acctno AND 
					d1.itemno= t.itemno AND 
					d1.contractno= t.contractno AND 
					d1.agrmtno= t.agrmtno AND 
					d1.stocklocn = t.stocklocn         
					 AND d1.TYPE= 'C'
					AND ISNULL(d1.orderlineno,0) = ISNULL(t.orderlineno,0)) 
		OR NOT EXISTS (SELECT 'x' FROM lineitemOracleExport  d2 WHERE   --no oder      
					d2.acctno= t.acctno AND 
					d2.itemno= t.itemno AND 
					d2.contractno= t.contractno AND 
					d2.agrmtno= t.agrmtno AND 
					d2.stocklocn = t.stocklocn         
					 AND d2.TYPE= 'O'
					AND ISNULL(d2.orderlineno,0) = ISNULL(t.orderlineno,0)) 
		OR PrevDelQ>quantity 
		OR (PrevDelVal>ordval  AND ORDVAL>0)--DISCOUNTS        
	 AND t.acctno NOT LIKE '___5%')   
	 
	          
	 UPDATE t -- previously interface order value        
	 SET PrevIntval = ISNULL(x.ordval,0),        
	 PrevIntQ = ISNULL(X.quantity ,0)        
	 FROM tempstatustype_process  t,lineitemOracleExport X         
	 WHERE x.acctno= t.acctno AND x.itemno= t.itemno AND x.stocklocn = t.stocklocn         
	 AND x.agrmtno= t.agrmtno AND x.contractno  = t.contractno AND x.runno>0          
	 --AND ISNULL(x.orderlineno,0) = ISNULL(t.orderlineno,0)         
	 AND x.TYPE  IN ( 'O','U','C')        
	 AND runno= (SELECT MAX(runno) FROM lineitemOracleExport Xp    --in last interface     
		WHERE xp.acctno= x.acctno AND xp.agrmtno= x.agrmtno AND xp.itemno= x.itemno        
		AND xp.contractno= x.contractno AND xp.stocklocn= x.stocklocn        
		AND ISNULL(xp.orderlineno,0) = ISNULL(x.orderlineno,0)         
		AND xp.TYPE in ('O','U','C')   )         
	 AND NOT EXISTS (SELECT 'x' FROM lineitemOracleExport l    -- and not cancelled in lineitemexport  
		 WHERE l.acctno= x.acctno AND l.itemno= x.itemno         
		 AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno        
		 AND l.contractno = x.contractno     
		 AND ISNULL(l.orderlineno,0) = ISNULL(x.orderlineno,0)         
		 AND l.TYPE = 'C' AND l.runno >0 ) -- so exclude cancellations         
	AND NOT  (((SELECT ISNULL(SUM(l2.quantity),0) FROM lineitemoracleexport l2    
				   WHERE l2.acctno= x.acctno     
				   AND l2.itemno= x.itemno         
		AND l2.stocklocn=  x.stocklocn     
		AND l2.agrmtno = x.agrmtno        
		AND l2.contractno = x.contractno     
		AND ISNULL(l2.orderlineno,0) = ISNULL(x.orderlineno,0)    
		AND quantity > 0    
		AND l2.type = 'D')     = (SELECT ISNULL(SUM(l3.quantity),0) * -1 FROM lineitemoracleexport l3    
				WHERE l3.acctno= x.acctno     
				AND l3.itemno= x.itemno         
				AND l3.stocklocn=  x.stocklocn     
				AND l3.agrmtno = x.agrmtno        
				AND l3.contractno = x.contractno     
				AND ISNULL(l3.orderlineno,0) = ISNULL(x.orderlineno,0)    
				AND l3.quantity < 0    
				AND l3.[type] = 'D')) AND EXISTS (SELECT 1 FROM lineitemoracleexport l4    
						WHERE l4.acctno= x.acctno     
					  AND l4.itemno= x.itemno         
					  AND l4.stocklocn=  x.stocklocn     
					  AND l4.agrmtno = x.agrmtno        
					  AND l4.contractno = x.contractno     
					  AND ISNULL(l4.orderlineno,0) = ISNULL(x.orderlineno,0)    
					  AND l4.[type] = 'D')) -- Check for delivered = cancelled where delivered exists.    
	 
	 UPDATE t -- previously interface order value        
	 SET t.PrevIntval = ISNULL(x.ordval,t.previntval),        
	 t.PrevIntQ = ISNULL(X.quantity ,t.PrevIntQ)        
	 FROM tempstatustype_process  t,lineitemOracleExport X         
	 WHERE x.acctno= t.acctno AND x.itemno= t.itemno AND x.stocklocn = t.stocklocn         
	 AND x.agrmtno= t.agrmtno AND x.contractno  = t.contractno     
	 --AND ISNULL(x.orderlineno,0) = ISNULL(t.orderlineno,0)         
	 --AND x.id<t.id
	 and x.runno >0
	   
	 -- update if booking previously interfaced and not delivered
	  UPDATE t         
	 SET newstatustype = 'U'        
	 FROM tempstatustype_process  t        
	 WHERE  
	 ISNULL(PrevDel,'N') != 'Y' 
	 AND exists (select 'x' from lineitemoracleexport d where
					d.acctno= t.acctno AND 
					d.itemno= t.itemno AND 
					d.contractno= t.contractno AND 
					d.agrmtno= t.agrmtno AND 
					d.stocklocn = t.stocklocn         
					AND d.runno >0 AND d.TYPE= 'B' 
					AND ISNULL(d.orderlineno,0) = ISNULL(t.orderlineno,0) )
 
	and not exists  (select 'x' from lineitemoracleexport d where
					d.acctno= t.acctno AND 
					d.itemno= t.itemno AND 
					d.contractno= t.contractno AND 
					d.agrmtno= t.agrmtno AND 
					d.stocklocn = t.stocklocn         
					AND d.runno >0 AND d.TYPE= 'C' 
					AND ISNULL(d.orderlineno,0) = ISNULL(t.orderlineno,0) 
	 	) 
	  --Updates should now only be sent prior to delivery but also for changes to price even if stockitem        
	 UPDATE tst         
	 SET newstatustype = 'U'        
	 FROM tempstatustype_process  tst        
	 WHERE  
	 ISNULL(PrevDel,'N') !='Y' 
	 AND (( PrevIntVal !=ordval AND ordval<>0 AND PrevIntQ<>0)        
	 OR (  PrevIntQ != quantity AND PrevIntQ >0 AND quantity >0 ) )        
	           
	 UPDATE tst         
	 SET newstatustype = 'U'        
	 FROM tempstatustype_process  tst        
	 WHERE CURRENTDel=1 AND Itemtype = 'N' AND category NOT IN (12,82) -- exclude warranties        
	 AND ABS(ordval) != ISNULL((SELECT (MAX(ABS(ordval))) FROM lineitemOracleExport x         
	 WHERE x.acctno = tst.acctno AND x.itemno= tst.itemno AND x.contractno= tst.contractno         
	 AND x.TYPE = 'd' AND x.runno=0 AND x.stocklocn= tst.stocklocn AND x.agrmtno=tst.agrmtno ),0)        
	 AND PrevDel !='Y'  
	 
	  -- for redelivery after repo
	 UPDATE tst         
	 SET newstatustype = 'O'        
	 FROM tempstatustype_process  tst        
	 WHERE exists  (SELECT sum(quantity)
	  FROM delivery x         
	 WHERE x.acctno = tst.acctno
	  AND x.itemno= tst.itemno 
	  AND x.contractno= tst.contractno         
	 AND delorcoll='R'   AND tst.agrmtno=x.agrmtno 
	 having sum(quantity)=0 ) 
	 and newstatustype='-'
	 and prevDel='Y'
	 
	 UPDATE tst         
	 SET newstatustype = 'C'        
	 FROM tempstatustype_process  tst        
	 WHERE quantity=0 and ordval=0
	  
	             
	 -- removing this record as order will already have been sent         
	 delete from tempstatustype_process  WHERE CURRENTDel=1 AND NewStatusType='O' 
	  AND  CATEGORY NOT IN (12,82)        
	          
	 -- if collection of non-stock item for cash and go         
	 IF EXISTS (SELECT * FROM tempstatustype_process  WHERE acctno LIKE '___5%')        
	 BEGIN        
	  UPDATE  tst         
	  SET newstatustype = 'U'        
	  FROM tempstatustype_process  tst        
	  WHERE EXISTS
	   (SELECT * FROM delivery d WHERE d.acctno= tst.acctno AND 
	   d.agrmtno= tst.agrmtno AND d.itemno= tst.itemno         
	  AND d.stocklocn = tst.stocklocn AND d.datetrans <  @minrundate )         
	  AND itemtype = 'N' AND category NOT IN (12,82)        
	 END        
	        
	 -- now need to remove if details are unchanged from previously delivered        
	 UPDATE tst         
	 SET deleteflag ='A' -- delete both this and any records in the table...         
	 FROM tempstatustype_process  tst        
	 WHERE         
	 (itemtype = 'N'  AND  PrevDelVal =ordval AND prevdelVal <>0 AND PrevDel ='Y'  ) -- if previously delivery         
	 OR (itemtype ='S' AND Prevdelq = quantity AND PrevdelQ >0 )        
	 AND PrevDel ='Y'   and orderlineno!=0         
	 
	  -- new order if partially delivered then revised
	 update tempstatustype_process
	 set quantity=case when quantity-previntq=0 then 1 else quantity-previntq end ,
	 ordval=ordval-previntval, orderlineno=0
	 where previntq=prevdelq
	 and previntval=prevdelval
	 and ordval>previntval
	          
	-- SET @testing= 0        
	         
	 UPDATE tst         
	 SET deleteflag ='A' -- delete both this and any records in the table...         
	 FROM tempstatustype_process  tst        
	 WHERE -- previously delivered stockitem and update or cancel record.         
	  itemtype ='S' AND PrevDel ='Y' AND newstatustype IN ('U','C') AND CURRENTDel !=1        
	 
	 -- cancellation where booked line not interfaced - delete this and all records in table       
	 UPDATE tst         
	 SET deleteflag ='A' -- delete both this and any records in the table...
	  FROM tempstatustype_process  tst      
	 where  newstatustype IN ('C')  and orderlineno=0 
	  
	         
	 UPDATE tst         
	 SET deleteflag ='A' -- delete both this and any records in the table...         
	 FROM tempstatustype_process  tst        
	 WHERE -- previously delivered nonstockitem and update or cancel record.         
	  itemtype ='N' AND PrevDel = 'Y' AND NewStatusType = 'O'  and prevdelval=ordval      
	        
	         
	 -- cancellation quantity ordval = 0 for non stock or quantity = 0 for stock test case 9 and 10         
	 UPDATE tempstatustype_process  SET NewStatusType ='C'         
	 WHERE  (quantity = 0   AND PrevDel !='Y' AND PrevIntQ <>0  AND CURRENTDel !=1  )            
	   
	          
	 -- update if quantity different or if non stock order value different.         
	         
	 declare @count int         
	-- set to cancellation if previously >0 and not delivered        
	 UPDATE tempstatustype_process  SET NewStatusType ='C'        
	 WHERE ((PrevIntQ > 0 AND quantity =0 AND itemtype='S')         
	 OR (PrevIntVal >0 AND ordval=0 AND itemtype ='N' AND CURRENTDel !=1))        
	 AND PrevDel !='Y'        
	         
	  --SET @TESTING = 1        
	               
	  -- do we need to remove previous records -- but don't if delivery        
	        
	 UPDATE tempstatustype_process  SET deleteflag='Y'        
	 WHERE  ( ISNULL(Currenttype,'') ='C' AND NewStatusType  IN('O','U') AND CURRENTDel<1)        
	 OR ( ISNULL(Currenttype,'') ='O'  AND NewStatusType ='U' AND CURRENTDel <1 )        
	         
	   ---        
	 --if stockitem and revision is less than previously delivered then don't need an update record as collection is going through        
	 DELETE FROM tempstatustype_process  WHERE itemtype ='S'        
	 AND PrevDel = 'Y' AND quantity < PrevIntQ        
	 -- the do want a new order if we have a previous cancellation and then another delivery - that is done in the delivery...         
	   
	            
	 -- remove where the previosly interfaced details have not changed at all.         
	 DELETE X     with (rowlock)   
	 FROM lineitemOracleExport x         
	 WHERE EXISTS (SELECT * FROM         
	 tempstatustype_process  t WHERE         
	 t.ordval = t.previntval         
	 and t.quantity = t.previntq        
	 and t.acctno= x.acctno and t.agrmtno= x.agrmtno        
	 and t.itemno= x.itemno and t.contractno= x.contractno        
	 AND t.stocklocn   = x.stocklocn AND t.currentdel=0 AND ISNULL(x.orderlineno,0) = ISNULL(t.orderlineno,0)    )        
	 AND x.TYPE !='D' AND x.runno=0         
	         
	        
	 DELETE x    with (rowlock)        
	 FROM lineitemOracleExport x        
	 WHERE EXISTS (SELECT * FROM tempstatustype_process  t         
	 WHERE         
	  t.acctno= x.acctno and t.agrmtno= x.agrmtno        
	 and t.itemno= x.itemno and t.contractno= x.contractno        
	 AND t.stocklocn   = x.stocklocn AND t.deleteflag in ('Y','A')  )        
	 AND x.TYPE !='D' AND x.runno=0         
	 -- don't remove if a new order for a delivery after collection         
	 AND NOT (ISNULL( x.orderlineno ,0) !=-1 AND TYPE = 'O')        
	         
	         
	 DELETE FROM  tempstatustype_process   WHERE         
	 (ordval = previntval         
	 and quantity = previntq and orderlineno!=0)        
	  OR deleteflag ='A'        
	          
	 SET @count = @@ROWCOUNT          
	      
	        
	 DELETE FROM tempstatustype_process  WHERE NewStatusType='-'        
	 
	 -----------------------------------        
	 --DE - DUPLICATE    
	 -----------------------------------    
	     
	   
	    
	DELETE FROM tempstatustype_process    
	WHERE EXISTS (    
	 SELECT * FROM tempstatustype_process b    
	 WHERE tempstatustype_process .acctno = b.acctno    
	  AND tempstatustype_process .agrmtno = b.agrmtno    
	  AND tempstatustype_process .itemno = b.itemno    
	  AND tempstatustype_process .stocklocn = b.stocklocn     
	  AND tempstatustype_process .contractno = b.contractno    
	  AND tempstatustype_process .[currenttype] = b.[currenttype]    
	  AND tempstatustype_process .[NewStatusType] = b.[NewStatusType]    
	  AND tempstatustype_process .id < b.id    
	 )    
	     
	 ----------------------------------    
	 --INSERT INTO EXPORT TABLE    
	 ----------------------------------         
	  
	  update tempstatustype_process
	  set quantity=previntq, ordval=previntval
		WHERE NewStatusType='C'     
	                
	             
	 INSERT INTO LineitemOracleExport (        
	  acctno,  agrmtno,  itemno,        
	  contractno,  quantity,  stocklocn,        
	  ordval,  [type],  runno,        
	  buffno,  orderno,  orderlineno        
	 )         
	 SELECT   distinct      
	  acctno,  agrmtno,  itemno,        
	  contractno,  quantity,  stocklocn,        
	  ordval,  NewStatusType,  0,        
	  0,  orderno,  orderlineno        
	 FROM tempstatustype_process X         
	 where NOT EXISTS         
	 (SELECT 'x' FROM LineitemOracleExport OE         
	 WHERE oe.acctno = x.acctno AND OE.itemno = x.itemno        
	 AND oe.stocklocn = x.stocklocn AND oe.agrmtno = x.agrmtno        
	 AND oe.contractno = x.contractno AND oe.[type] = x.[NewStatusType] 
	 and( (runno=0 and x.[NewStatusType] ='U') or x.[NewStatusType] !='U')
	 and oe.orderno=x.orderno
	 and oe.orderlineno=x.orderlineno)
	 
	 
	GO 
