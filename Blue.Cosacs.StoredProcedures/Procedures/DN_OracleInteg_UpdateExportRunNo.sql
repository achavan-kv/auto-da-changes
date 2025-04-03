If not exists (select 'x' from sys.tables where name='OracleExportHisttemp')
	select *
	into OracleExportHisttemp
	FROM OracleExportHist  

	GO 	
if EXISTS (SELECT * FROM sysobjects WHERE NAME ='DN_OracleInteg_UpdateExportRunNo')
	drop procedure [dbo].[DN_OracleInteg_UpdateExportRunNo] 
GO 
create PROCEDURE [dbo].[DN_OracleInteg_UpdateExportRunNo] --2000,0
		@newRunNo		int,
		@return         int OUTPUT    

	AS
		SET @return = 0
		SET NOCOUNT ON 	
		SET DEADLOCK_PRIORITY HIGH

		exec OracleInsertIntoLiExport	
	-- missing bookings for identical replacement	
	INSERT INTO LineitemOracleExport (		acctno,		agrmtno,		itemno,		contractno,		quantity,		stocklocn,
		ordval,		[type],		runno,		buffno,		orderno,		orderlineno	)  	
select schedule.acctno,schedule.agrmtno,schedule.itemno, schedule.contractno,max(schedule.quantity), schedule.stocklocn,
max(price) ,'O',0,isnull(orderno,0),0 , null
from schedule , stockitem, lineitem
where schedule.itemno=stockitem.itemno
and schedule.stocklocn=stockitem.stocklocn
and stockitem.itemtype='S'
and schedule.itemno=lineitem.itemno
and schedule.stocklocn=lineitem.stocklocn
and schedule.acctno=lineitem.acctno
and schedule.agrmtno=lineitem.agrmtno
and lineitem.quantity!=0
and not exists
(select 'x' from LineitemOracleExport l 
where l.acctno=schedule.acctno
and l.agrmtno=schedule.agrmtno
and l.itemno=schedule.itemno
and l.type='O'
and not exists (select 'x' from LineitemOracleExport l2 
where l.acctno=l2.acctno
and l.orderlineno=l2.orderlineno
and l.agrmtno=l2.agrmtno
and l.itemno=l2.itemno
and l2.type='D' )
)
group by schedule.acctno,schedule.itemno, schedule.stocklocn ,schedule.agrmtno,schedule.contractno, OrderNo
having sum(schedule.quantity)=0
and max(schedule.quantity)>0


	update lineitemoracleexport
	set acctno=isnull((select top 1 acctno from SR_ChargeAcct
						where substring(cast(agrmtno as varchar),4,10)=ServiceRequestNo),acctno),
	ordval = ISNULL((select UnitPrice*Quantity  from SR_PartListResolved
						where substring(cast(agrmtno as varchar),4,10)=ServiceRequestNo
						and lineitemoracleexport.itemno=SR_PartListResolved.PartNo),ordval),
	quantity = ISNULL((select quantity  from SR_PartListResolved
						where substring(cast(agrmtno as varchar),4,10)=ServiceRequestNo
						and lineitemoracleexport.itemno=SR_PartListResolved.partno),quantity)
	where  lineitemoracleexport.acctno='730500010630'  and type='D' and runno=0
			----------------------------------------------------------------------------------------
		UPDATE InterfaceControl
		SET DateStart = GetDate()
		WHERE  Interface = 'OrInteg2' and RunNo = @newRunNo 
		----------------------------------------------------------------------------------------
		
		
		----------------------------------------------------------------------------------------
		-- for performance reasons
		declare @rowcount int 
		SELECT *
		INTO #TempOracleExport
		FROM LineItemOracleExport
		WHERE RunNo = 0	--and
		--	  OrderNo is not NULL
		ORDER BY serialno,AcctNo, OrderNo, OrderLineNo
		set @rowcount =@@rowcount 
		
		----------------------------------------------------------------------------------------
			
		SET IDENTITY_INSERt #TempOracleExport OFF 
		select * 
		FROM lineitemOracleExport l WHERE runno = 0 AND TYPE !='D'
		AND EXISTS (SELECT * FROM  #TempOracleExport x
		WHERE l.acctno= x.acctno AND l.itemno= x.itemno 
			  AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno
			  AND l.contractno = x.contractno )
			  
		SET IDENTITY_INSERt #TempOracleExport OFF 
	 	
		----------------------------------------------------------------------------------------
		
		-- Update the New Run number in the lineitemOraclExport Table
		BEGIN
		
		UPDATE x 
		SET runno = @newrunno 
		FROM lineitemOracleExport x,  #TempOracleExport l 
		WHERE l.acctno= x.acctno AND l.itemno= x.itemno 
			  AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno
			  AND l.contractno = x.contractno AND ISNULL(l.orderno,0) = ISNULL(X.orderno,0) AND x.runno  =l.runno 
			  AND x.SerialNo = l.SerialNo
			
		END
		----------------------------------------------------------------------------------------
	 	
	    
		-- Updating FinTransOracleExport with the new RunnNo & ReceiptNo------------------------
		----------------------------------------------------------------------------------------
		-- again for performance reasons only getting 10,000 records at a time. 
		SELECT  * INTO #fx FROM FinTransOracleExport f
		WHERE  f.runno= 0
		ORDER BY f.datetrans ASC
		
		UPDATE
		x
		SET RunNo = @newRunNo
		from FinTransOracleExport x
		WHERE RunNo = 0 AND EXISTS 
		(SELECT  * FROM #fx f
		WHERE f.acctno= x.acctno AND f.transrefno= x.transrefno 
		AND f.datetrans= x.datetrans AND f.branchno= x.branchno) 
		   
		----------------------------------------------------------------------------------------
		DECLARE @newReceiptNo bigint, @acctNo_2 char(12), @branchNo smallint, @transRefNo int, 
				@dateTrans datetime, @oracleReceiptNo  varchar(19)
		SET @newReceiptNo = (Select HiExtReceiptNo From Country) + 1
		
		
		 CREATE TABLE #Foe (seedint INT IDENTITY(1,1), acctno CHAR(12), branchno SMALLINT,transrefno INT ,datetrans DATETIME,oraclereceiptno VARCHAR(19))     
	  
	    
		 INSERT INTO #Foe    
		 SELECT AcctNo, BranchNo, TransRefNo, DateTrans, OracleReceiptNo   
		 FROM FinTransOracleExport F     
		 WHERE RunNo = @newRunNo   
		 AND OracleReceiptNo IS NULL    
		 ORDER BY DateTrans    
		     
		
		UPDATE f
		 SET OracleReceiptNo = LEFT(CAST(t.branchNo as varchar(5)),3) + CAST(seedint + @newReceiptNo  as varchar(13))      
		FROM FinTransOracleExport f ,#Foe t 
		WHERE t.OracleReceiptNo is NULL and t.AcctNo = f.acctNo and t.BranchNo = f.branchNo and 
				t.TransRefNo = f.transRefNo and t.DateTrans = f.dateTrans 
		
	    
		SELECT @newReceiptNo = MAX( seedint )  FROM  #Foe	

		-- Update HiExtReceiptNo in Country table ----------------------------------------------
		IF @newReceiptNo IS NOT NULL 
			UPDATE Country SET HiExtReceiptNo = @NewReceiptNo	 
	   
		----------------------------------------------------------------------------------------
	        
		SET @return = @@error
		

SET QUOTED_IDENTIFIER OFF 
SET ANSI_NULLS ON 
GO 