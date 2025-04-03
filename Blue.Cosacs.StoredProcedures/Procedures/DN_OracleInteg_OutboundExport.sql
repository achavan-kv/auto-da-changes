
	   
	IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'DN_OracleInteg_OutboundExport')
		DROP PROCEDURE DN_OracleInteg_OutboundExport
		GO 

	 CREATE  PROCEDURE [dbo].[DN_OracleInteg_OutboundExport] -- 1438,0
	 @runNo int,      
	  @return int output ,  
	  @logging smallint =0 
	  
	 AS  
	 
	 
	 set deadlock_priority high
	  set @return = 0  -- initialfroise return code   
	  SET NOCOUNT ON   -- slight performadnce benefit.--------------------------------------  
	 
	 
	  
	  IF @runNo > 0  
		BEGIN  
		  
			
			EXEC OracleUpdateOrderlines @runno = @runno     
		
			  UPDATE lineitem SET orderno =x.orderno , orderlineno = x.orderlineno    
			  FROM LineitemOracleExport x    
			  WHERE x.runno =@runno AND lineitem.acctno= x.acctno AND lineitem.contractno = x.contractno    
			  AND lineitem.itemno= x.itemno AND lineitem.stocklocn = x.stocklocn      
			  AND x.agrmtno= lineitem.agrmtno    
			  AND ISNULL(lineitem.orderno,-1) <=0 
			  
	-- cancellation after return or delivery
	delete from lineitemoracleexport 
	 where lineitemoracleexport.type in ('C') and  exists 
	 (select 'x' from LineitemOracleExport l
	where lineitemoracleexport.agrmtno=l.agrmtno
	and lineitemoracleexport.itemno=l.itemno
	and lineitemoracleexport.stocklocn=l.stocklocn
	and lineitemoracleexport.contractno=l.contractno
	and lineitemoracleexport.orderno=l.orderno
	and lineitemoracleexport.orderlineno=l.orderlineno
	and l.type in ('D') and quantity<0
	and l.runno<=@runno)
	and lineitemoracleexport.runno=@runno  
		END
	 
	    
	 DECLARE @datestart DATETIME ,@minrundate DATETIME  
	 SELECT @datestart = datestart FROM interfacecontrol   
	 WHERE interface = 'orinteg2'  
	 AND runno= @runno   
	  
	 SELECT @minrundate = MIN(datestart) FROM interfacecontrol   
	 WHERE interface = 'orinteg2'  
	   

	 -----------------------------------------------------------------------------------------------------  
	 
	  SELECT x.acctno,     x.agrmtno,     x.itemno,  
		  x.contractno,     x.quantity,     x.stocklocn,  
		  x.ordval,     [type],     x.runno,  
		  x.buffno,     x.orderno,     x.orderlineno,  
		  x.SerialNo, ISNULL(itemtype,'S') AS itemtype   
	  INTO #export       
	  FROM lineitemOracleExport X  , stockitem s
	  where s.itemno=X.itemno and s.stocklocn=X.stocklocn
	 

	 -----------------------------------------------------------------------------------------------------   
	 SELECT  
	 OE.RunNo, OE.AcctNo, OE.OrderNo,   
	  
	 (   
	  Select Max(DateStart) From InterfaceControl Where RunNo = @runNo and interface = 'OrInteg2'    
	 ) as InterfacedDate,   
	  
	 AG.DateAgrmt as OrderedDate,  
	  
	 Case  
	  When AC.AcctType = 'S' Then 'Special'  
	  When AC.AcctType = 'C' Then 'Cash'  
	  Else 'Credit'  
	 End as OrderType,  
	  
	 Case   
	  When OE.Type = 'O' or OE.Type = 'U' or OE.Type = 'C' Then 'Order'  
	  When OE.Type = 'D' and isnull(DEL.DelOrColl,'D') = 'D' Then 'Delivery' -- This value will be changed below  
	  When OE.Type = 'D' and DEL.DelOrColl = 'C' Then 'Return'  
	  When OE.Type = 'D' and DEL.DelOrColl = 'R' AND del.transvalue <=0 Then 'Repossession'   
	  When OE.Type = 'D' and DEL.DelOrColl = 'R' AND del.transvalue >0 Then 'RDelivery' -- redelivery after repo has delorcoll r and is delivery  but will change this later...
	  Else ''  
	 End as TranClass,  
	  
	 IP.InstalNo as PayTerm,   
	  
	 (     
	  Select IsNull(Code.CodeDescript, '') From Code   
	  Where Code.Category = 'FPM' and Code.Code = ( Select Convert(Varchar(10), MAX(IsNull(FinTrans.PayMethod,0))) From FinTrans  
				  Where FinTrans.TransTypeCode = 'PAY' and FinTrans.AcctNo = OE.AcctNo )         
	 ) as PayMethod,  
	  
	 AC.BranchNo, C_AC.CustId,   
	 CONVERT(VARCHAR(200),'') as BillToAddr,  
	 --IsNull((C_AD_H.CusAddr1 + ' ' + C_AD_H.CusAddr2 + ' ' + C_AD_H.CusAddr3 + ' ' + C_AD_H.CusPoCode), '') as BillToAddr,  
	 CONVERT(VARCHAR(200),'') as ShipToAddr,  
	 --IsNull((C_AD_D.CusAddr1 + ' ' + C_AD_D.CusAddr2 + ' ' + C_AD_D.CusAddr3 + ' ' + C_AD_D.CusPoCode), '') as ShipToAddr, -- for  performance  
	 isnull(emp.EmpeeNo,0) as SalesPersonId, isnull(EMP.FullName,'') as SalesPersonName,   
	 IsNull(AG.SOA, '') + '-' + IsNull(CODE.CodeDescript, '') as SalesChannel,  
	 OE.OrderLineNo as LineNumber, OE.ItemNo , SI.ItemDescr1 as ItemDesc, N'Unit' as UOM,   
	  
	 Case  
	  When SI.Category IN (Select DISTINCT reference From code Where category = 'dis') Then 'DISCOUNT'  
	  When OE.ItemNo = 'DT' Then 'DT'  
	  Else ''  
	 End as ItemType,  
	  
	 Case  
	  When (  OE.quantity !=0  )   
	   Then ROUND(OE.OrdVal/OE.Quantity, 2) -- for collectionn or order use unit value /quantity  
	   else 0 end as
	 UnitPrice,  
	  
	 OE.OrdVal  
	 as LineAmount,   
	  
	 N'Y' as TaxFlag, N'VAT 15%' as TaxCode, Convert(Float, 0.15) as TaxRate,  
	  
	 Case  
	  When OE.Type = 'U' Then 'U'   
	  When OE.Type = 'C' Then 'C' -- Cancelled  
	  When OE.Type = 'O' --and IsNull(SCH.LoadNo,0) = 0 
	  Then 'B' -- Booked  
	  --When OE.Type = 'O' and IsNull(SCH.LoadNo,0) != 0 Then 'S'  -- Scheduled Mauritius don't want this  
	  When OE.Type = 'D' and isnull(DEL.DelOrColl,'D') = 'D' Then 'D' -- This value will be changed below  
	  When OE.Type = 'D' and DEL.DelOrColl = 'R' AND ISNULL(SI.ITEMTYPE,'S') ='S' AND DEL.transvalue >0 Then 'D' -- Redelivery after repo  
	  When OE.Type = 'D' and DEL.DelOrColl = 'R' Then 'P' -- Repossession  
	  When OE.Type = 'D' and DEL.DelOrColl = 'C' Then 'R' -- Returned  
	  Else 'B'  
	 End as StatusFlag,  
	  
	 isnull(Case  
	  When OE.Type = 'D' and li.OrdVal >= 0 and DEL.TransValue <0 AND  -- so if non stock and collection e.g. warranty  
		 SI.itemtype !='S' Then ABS(DEL.Quantity) * -1   
	  --When OE.Type = 'D' and OE.OrdVal > 0 and  AA Discounts can and should have a negative price and positive quantity  
	  --   SI.Category IN (select distinct reference from code where category = 'dis') Then ABS(DEL.Quantity) * -1  
	  --When OE.Type = 'D' and OE.OrdVal > 0 Then DEL.Quantity Spec says want original order quantity and Maninder does too.  
	  When OE.Type = 'O' and LI.Quantity <= 0 and OE.Quantity > 0 Then OE.Quantity  
	  When LI.Quantity = 0 and LI.OrdVal > 0 Then 1   
	  Else LI.Quantity -- for deliveries and collections Oe.quantity will have been succcessfully updated.   
	 End,oe.quantity) as OrderedQty,   
	  
	 CNCL.Code as CancelReason, CL_R.CollectionReason as ReturnReason, ISNULL(Del.retitemno,'') AS ReturnItemno , 
	 ISNULL(Del.retstocklocn,'') AS retstocklocn,   
	 coalesce(OE.BuffNo,DEL.buffno,0) as DeliveryNumber, DEL.DateDel as DeliveredDate,  
	  
	 Case  
	  When DEL.Quantity = 0 AND DEL.TransValue > 0 Then 1   
	  Else DEL.Quantity  
	 End as DeliveredQty,  
	  
	 case when OE.itemno='DT' then '100' 
	  when isnull(del.retstocklocn,0)!=0 then del.retstocklocn else OE.stocklocn end as DeliveredFromLocn, LI.Notes as DeliveryComments,   
	  
	 Case  
	  When OE.Type = 'D' Then   
	   Case  
		When DEL.DelOrColl = 'R' or DEL.DelOrColl = 'C' Then NULL -- When Repossessed or Returned       
		When SCH_AUD.DateDelPlan >= SCH_AUD.DatePicklistPrinted and SCH_AUD.DateDelPlan >= DEL.DateTrans Then SCH_AUD.DateDelPlan  
		When SCH_AUD.DatePicklistPrinted >= SCH_AUD.DateDelPlan and SCH_AUD.DatePicklistPrinted >= DEL.DateTrans Then SCH_AUD.DatePicklistPrinted  
		When DEL.DateTrans >= SCH_AUD.DateDelPlan and DEL.DateTrans >= SCH_AUD.DatePicklistPrinted Then DEL.DateTrans  
		Else NULL  
	   End     
	  Else NULL  
	 End as DropOffTime,   
	  
	 Case  
	  When OE.Type = 'O' or OE.Type = 'U' Then   
	   Case  
		When SCH.DateDelPlan is not NULL and SCH.DatePicklistPrinted is not NULL Then  
		 Case  
		  When SCH.DateDelPlan > SCH.DatePicklistPrinted Then SCH.DateDelPlan  
		  Else SCH.DatePicklistPrinted   
		 End  
		When SCH.DateDelPlan is not NULL and SCH.DateDelPlan != '01/01/1900' Then SCH.DateDelPlan  
		When SCH.DatePicklistPrinted is not NULL and SCH.DatePicklistPrinted != '01/01/1900'   
		   Then SCH.DatePicklistPrinted  
		Else NULL   
	   End  
	  When OE.Type = 'D' Then   
	   Case  
		When SCH_AUD.DateDelPlan is not NULL and SCH_AUD.DatePicklistPrinted is not NULL Then  
		 Case  
		  When SCH_AUD.DateDelPlan > SCH_AUD.DatePicklistPrinted Then SCH_AUD.DateDelPlan  
		  Else SCH_AUD.DatePicklistPrinted   
		 End  
		When SCH_AUD.DateDelPlan is not NULL and SCH_AUD.DateDelPlan != '01/01/1900' Then SCH_AUD.DateDelPlan  
		When SCH_AUD.DatePicklistPrinted is not NULL and SCH_AUD.DatePicklistPrinted != '01/01/1900'   
		   Then SCH_AUD.DatePicklistPrinted  
		Else DEL.DateTrans   
	   End   
	  Else NULL  
	 End as PickUpTime,  
	  
	 TS.TruckId as FreightCarrier, Convert(Money, NULL) as FreightCharge,  
	 SCH.DateDelPlan as ScheduledDelDate , OE.BuffNo, OE.AgrmtNo, OE.StockLocn, del.datetrans AS NotifiedDelDate, CONVERT(VARCHAR(10),'') AS OriginalItemCode  
	  
	 INTO #TEMP  
	 FROM #export  OE  
	 INNER JOIN Acct AC on OE.AcctNo = AC.AcctNo  
	 LEFT JOIN Agreement AG on OE.AgrmtNo = AG.AgrmtNo and OE.AcctNo = AG.AcctNo  
	 LEFT JOIN Delivery DEL on OE.Type = 'D' and OE.ItemNo = DEL.ItemNo and 
				(OE.AcctNo = DEL.AcctNo or (del.agrmtno!=1 and del.transvalue=0)) and   
		OE.AgrmtNo = DEL.AgrmtNo and OE.StockLocn = DEL.StockLocn and OE.ContractNo = DEL.ContractNo   
		and OE.BuffNo = DEL.BuffNo  
	 LEFT JOIN InstalPlan IP on OE.AcctNo = IP.AcctNo  
	 LEFT JOIN CustAcct C_AC on OE.AcctNo = C_AC.AcctNo and C_AC.hldorjnt = 'H'  
	 LEFT JOIN Admin.[User] EMP on AG.EmpeeNoSale = EMP.Id  
	 LEFT JOIN StockItem SI on OE.ItemNo = SI.ItemNo and OE.StockLocn = SI.StockLocn  
	 LEFT JOIN LineItem LI on OE.ItemNo = LI.ItemNo and OE.AcctNo = LI.AcctNo and OE.AgrmtNo = LI.AgrmtNo   
		and OE.StockLocn = LI.StockLocn and OE.ContractNo = LI.ContractNo   
		and LI.ParentItemNo in ( Select TOP 1 LI_TEMP.ParentItemNo from LineItem LI_TEMP where   
			  LI_TEMP.ItemNo = OE.ItemNo and LI_TEMP.AcctNo = OE.AcctNo and LI_TEMP.AgrmtNo = OE.AgrmtNo   
			 and LI_TEMP.StockLocn = OE.StockLocn and LI_TEMP.ContractNo = OE.ContractNo )  
	 LEFT JOIN Schedule SCH on OE.ItemNo = SCH.ItemNo and OE.AcctNo = SCH.AcctNo and OE.AgrmtNo = SCH.AgrmtNo   
		and OE.StockLocn = SCH.StockLocn and OE.ContractNo = SCH.ContractNo and (OE.BuffNo = SCH.BuffNo or SCH.BuffNo = 0)  
		and SCH.BuffBranchNo in ( Select TOP 1 SCH_TEMP.BuffBranchNo from Schedule SCH_TEMP where   
			  SCH_TEMP.ItemNo = OE.ItemNo and SCH_TEMP.AcctNo = OE.AcctNo and SCH_TEMP.AgrmtNo = OE.AgrmtNo   
			 and SCH_TEMP.StockLocn = OE.StockLocn and SCH_TEMP.ContractNo = OE.ContractNo and SCH_TEMP.BuffNo = OE.BuffNo)  
	 LEFT JOIN ScheduleAudit SCH_AUD on OE.Type = 'D' and OE.ItemNo = SCH_AUD.ItemNo and OE.AcctNo = SCH_AUD.AcctNo and   
		OE.AgrmtNo = SCH_AUD.AgrmtNo and OE.StockLocn = SCH_AUD.StockLocn and OE.BuffNo = SCH_AUD.BuffNo -- if delivered buffno wont be 0         
	 LEFT JOIN Cancellation CNCL on OE.Acctno = CNCL.AcctNo and OE.AgrmtNo = CNCL.AgrmtNo   
		and CNCL.DateCancel in (Select MAX(CNCL_TEMP.DateCancel) from Cancellation CNCL_TEMP where CNCL_TEMP.AcctNo = OE.AcctNo and CNCL_TEMP.AgrmtNo = OE.AgrmtNo)  
	 LEFT JOIN CollectionReason CL_R on OE.AcctNo = CL_R.AcctNo and OE.ItemNo = CL_R.ItemNo and OE.StockLocn = CL_R.StockLocn  
		and CL_R.DateAuthorised = (Select MAX(CL_R_TEMP.DateAuthorised) from CollectionReason CL_R_TEMP where CL_R_TEMP.AcctNo = OE.AcctNo and CL_R_TEMP.ItemNo = OE.ItemNo and CL_R_TEMP.StockLocn = OE.StockLocn)  
	 LEFT JOIN TransptSched TS on ((OE.Type = 'O' or OE.Type = 'U') and SCH.BuffBranchNo = TS.BranchNo and SCH.DateDelPlan = TS.DateDel AND SCH.LoadNo = TS.LoadNo)  
		or (OE.Type = 'D' and SCH_AUD.BuffBranchNo = TS.BranchNo and SCH_AUD.DateDelPlan = TS.DateDel AND SCH_AUD.LoadNo = TS.LoadNo)  
	 LEFT JOIN CODE on AG.SOA = CODE.Code and CODE.Category = 'SOA'   
	 WHERE OE.RunNo = @runNo and OE.OrderNo is not NULL    
	 --AND oe.acctno = '754010472061'  
	 ORDER BY OE.AcctNo, OE.OrderNo, OE.OrderLineNo, OE.SerialNo  
	 -----------------------------------------------------------------------------------------------------  
	  
	 IF @logging = 1  
	 begin   
	  SELECT 'ss',* FROM #temp   
	 end   
	 CREATE INDEX ix_temphashoracleexp ON #temp (custid,acctno)   
	-- don't like 0 ordered quantity updates  


	 UPDATE #temp SET OrderedQty =1, UnitPrice=0, lineamount = 0    
	 FROM stockinfo s  
	 WHERE OrderedQty =0 AND StatusFlag = 'U' AND lineamount = 0    
	  AND s.itemno= #temp.itemno AND s.itemtype = 'N'  
	    
	  
	 UPDATE cu  
	 SET  ShipToAddr =IsNull((C_AD_D.CusAddr1 + ' ' + C_AD_D.CusAddr2 + ' ' + C_AD_D.CusAddr3 + ' ' + C_AD_D.CusPoCode), '')   
	 FROM #temp cu, CustAddress C_AD_D WHERE  CU.CustId = C_AD_D.CustId and C_AD_D.AddType like 'D%' and C_AD_D.DateMoved is null  
	  
	 UPDATE cu  
	 SET BillToAddr=IsNull((C_AD_H.CusAddr1 + ' ' + C_AD_H.CusAddr2 + ' ' + C_AD_H.CusAddr3 + ' ' + C_AD_H.CusPoCode), '')    
	 FROM  #temp cu,CustAddress C_AD_H   
	 WHERE  CU.CustId = C_AD_H.CustId and C_AD_H.AddType = 'H' and C_AD_H.DateMoved is null  
	  
	 -----------------------------------------------------------------------------------------------------  
	 -- Updating the values in the #TEMP table -----------------------------------------------------------  
	 -- Updating the correct TranClass for 'Delivery' for non-stocks   
	 -- reinstating so for all non-stocks  
	 -- Returns should be determined based on whether the original record was >0 or < 0 - if original > 0 then return is less than 0....    
	 BEGIN   
	 UPDATE #TEMP    
	 SET TranClass = 'Return',statusflag= 'R'  
	 FROM stockitem s  
	 WHERE TranClass IN  ('Delivery') AND lineamount <0     
	 AND EXISTS (SELECT * FROM LineItemAudit l WHERE    
	 l.acctno= #temp.acctno AND l.stocklocn= #temp.stocklocn AND l.valueafter >0 -- if order value was > 0 ...  
	 AND l.agrmtno=#temp.agrmtno AND s.itemno= l.itemno   
	 AND s.stocklocn = l.stocklocn  
	 AND l.Datechange = (SELECT MIN(datechange) FROM LineitemAudit d WHERE   
	  l.acctno= d.acctno AND l.itemno= d.itemno   
	  AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno  
	  AND l.contractno = d.contractno AND d.QuantityAfter <>0 /*AND d.Datechange >@minrundate*/   
	  AND d.valueafter <>0 ))  
	 AND s.itemtype !='S' AND s.itemno= #temp.itemno   
	   
	 UPDATE #TEMP  -- return of DT should always be return  
	 SET TranClass = 'Return',statusflag= 'R'  
	 WHERE TranClass IN  ('Delivery') AND lineamount <0   AND itemno= 'DT'  
	   
	 UPDATE #TEMP  -- return of discounts   
	 SET TranClass = 'Return',statusflag= 'R'  
	 FROM stockitem s  
	 WHERE TranClass IN  ('Delivery') AND lineamount >0     
	 AND EXISTS (SELECT * FROM LineItemAudit l WHERE    
	 l.acctno= #temp.acctno AND l.stocklocn= #temp.stocklocn AND l.valueafter <0 -- if order value was < 0 ...  
	 AND l.agrmtno=#temp.agrmtno AND s.itemno= l.itemno   
	 AND s.stocklocn = l.stocklocn  
	 AND l.Datechange = (SELECT MIN(datechange) FROM LineitemAudit d WHERE   
	  l.acctno= d.acctno AND l.itemno= d.itemno   
	  AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno  
	  AND l.contractno = d.contractno AND d.QuantityAfter <>0  /*AND d.Datechange >@minrundate*/ ))  
	    
	 AND s.itemtype !='S' AND s.itemno= #temp.itemno   
	 END   
	   
	 --AND  s.category IN (12,82) -- all non stocks now  
		-- ordered quantity should never be zero except for cancellations  
	 UPDATE #temp SET orderedqty = 1 WHERE orderedqty = 0 AND statusflag IN ('D','P','R','B')   
	   
	 -- non stock repos must be returns.   
	 UPDATE #temp SET  Tranclass='Return',statusflag = 'R'   
	 FROM StockInfo s   
	 WHERE s.itemno =#temp.itemno AND s.itemtype = 'N' AND s.category NOT IN (12,82)  
	 AND tranclass ='Repossession'   
	  
	 -- returns should always have a negative ordered quantity   
	 UPDATE #temp   
	 SET OrderedQty =-OrderedQty WHERE OrderedQty >0 AND StatusFlag = 'R'  
	 -- deliveries should always have a positive order quantity   
	 UPDATE #temp   
	 SET OrderedQty =-OrderedQty WHERE OrderedQty <0 AND StatusFlag = 'd'  
	  
	 UPDATE #temp SET UnitPrice = lineamount/orderedqty   
	 WHERE #temp.statusflag IN ('B','P','R') AND orderedqty <>0   
	  
	  
	 UPDATE #temp SET UnitPrice = lineamount/deliveredqty   
	 WHERE #temp.statusflag IN ('D') AND deliveredqty <>0   
	  
	     
	 UPDATE l  
	 SET DeliveryComments =  D.itemno + '^'  
	 FROM  #temp l ,delivery d   
	 WHERE statusflag IN ('P','R')    
	 AND l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn  
	 AND l.agrmtno = d.agrmtno  
	  
	 UPDATE #temp  -- remaining deliveries are orders   
	 SET Tranclass = 'Order'  
	 WHERE TranClass = 'Delivery'   
	 ALTER TABLE #temp ALTER COLUMN tranclass VARCHAR(14)  
	  
	  
	  -- all things service
	  
	 -- labour items
	 DECLARE @Courtspartid VARCHAR(10), @courtswarrantyid VARCHAR(10 )  
	 SELECT @Courtspartid=value FROM CountryMaintenance WHERE NAME LIKE 'Service Item Parts Courts%'  
	 
	 CREATE TABLE #nonparts (itemno VARCHAR(10) )   
	 INSERT INTO #nonparts ( itemno )   
	 SELECT value FROM CountryMaintenance WHERE NAME LIKE 'Service Item%'  
	 
	  
	  
	 UPDATE l SET orderedqty = 1   
	 FROM #temp l  
	 WHERE  EXISTS   
	 (SELECT * FROM delivery d   
	 WHERE l.acctno= d.acctno AND l.itemno= d.itemno   
		AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno  
		AND l.itemno IN (select * from #nonparts) AND d.quantity =1 )  
	    
	UPDATE l SET orderedqty = -1   
	 FROM #temp l  
	 WHERE  EXISTS   
	 (SELECT * FROM delivery d   
	 WHERE l.acctno= d.acctno AND l.itemno= d.itemno   
		AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno  
		AND l.itemno IN (select * from #nonparts) AND d.quantity =-1 )  
	  
	 UPDATE l SET deliveredqty = 1   
	 FROM #temp l  
	 WHERE  EXISTS   
	 (SELECT * FROM delivery d   
	 WHERE l.acctno= d.acctno AND l.itemno= d.itemno   
		AND l.stocklocn=  d.stocklocn AND l.agrmtno = d.agrmtno  
		AND l.itemno IN (select * from #nonparts) AND d.quantity =1) AND statusflag = 'd'  
	    
	 
	 UPDATE #temp  -- now updating for service....   
	 SET Tranclass = 'Service Order'  
	 WHERE EXISTS (SELECT * FROM CountryMaintenance c WHERE NAME LIKE '%service%' AND DATALENGTH(value) = 12 AND c.[value] = #temp.acctno)  
	 OR exists (SELECT * FROM code ca WHERE ca.category='SRSUPPLIER' AND ca.reference = #temp.acctno)  
	 or  EXISTS (SELECT * FROM sr_chargeAcct s WHERE s.acctno= #temp.AcctNo)  
	  
	 UPDATE #temp   
	 SET DeliveryComments =  CONVERT(VARCHAR,c.ServiceRequestNo)  
	 FROM SR_ChargeAcct c  
	 WHERE tranclass = 'Service Order'  
	 AND c.AcctNo = #temp.AcctNo  
	 and agrmtno= 1  
	 -- updating delivery comments with service request number  
	   
	 UPDATE #temp   
	 SET DeliveryComments = SUBSTRING(CONVERT(VARCHAR,agrmtno),4,10)   
	 WHERE tranclass = 'Service Order'  
	 AND AcctNo = #temp.AcctNo  
	 and agrmtno> 100  
	 --select 'ss',* from #temp   
	 
	   
	 -- try and work out the amount based on the price of the parts compared to the spare parts charge  
	  
	 IF @logging = 1  
	 begin   
	  SELECT 'dd1',* FROM #temp   
	 end   

	 -- set ordered date to delivered date for service order deliveries   
	  IF @logging = 1  
	 begin   
	  SELECT 'dd1e',* FROM #temp   
	 end
	 
	  -- set ordered date to delivered date for service orders  
	 UPDATE #temp SET ordereddate = isnull(delivereddate , GETDATE()),
	NotifiedDelDate = isnull(delivereddate , GETDATE())
	 WHERE TranClass ='Service Order'  
	 AND ISNULL (ordereddate,'1-jan-1900')= '1-jan-1900' AND statusflag = 'D'  

	 UPDATE #temp SET DeliveredDate = isnull(delivereddate , GETDATE())
	 WHERE ISNULL (DeliveredDate,'1-jan-1900')= '1-jan-1900' AND statusflag = 'D'  

	 
	  UPDATE #temp SET NotifiedDelDate = isnull(delivereddate , GETDATE())
	 WHERE ISNULL (NotifiedDelDate,'1-jan-1900')= '1-jan-1900' AND statusflag = 'D'  

	 UPDATE l   
	 SET ordereddate = isnull(x.delivereddate , GETDATE())  
	 FROM #temp l, #temp x    
	 WHERE l.TranClass ='Service Order'  
	 AND l.acctno= x.acctno AND l.itemno= x.itemno   
	  AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno  
	  AND ISNULL (l.ordereddate,'1-jan-1900')= '1-jan-1900'   
	  AND l.statusflag = 'B' AND x.statusflag = 'D'  
		 AND x.tranclass = l.tranclass   
	     

	 -- removing service orders where no delivery - will be inserting orders later
	  IF @logging = 1  
	 begin   
	  SELECT 'dd2',* FROM #temp   
	 end
	 
	 DELETE l 
	 FROM #temp l 
	 WHERE statusflag = 'B' AND tranclass = 'SERVICE Order'
	 AND NOT EXISTS (SELECT * FROM #temp x 
	 WHERE l.acctno= x.acctno AND l.itemno= x.itemno 
		   AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno
		   AND x.linenumber = l.linenumber AND x.statusflag = 'D'
		   ) 
		   
			IF @logging = 1  
	 begin   
	  SELECT 'dd2e',* FROM #temp   
	 end
	-- aligning service order with delivery.... 	   
	UPDATE l SET unitprice = x.unitprice 
	FROM #temp l, #temp X 
	WHERE l.acctno= x.acctno AND l.itemno= x.itemno 
		  AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno
		  AND l.orderno = x.orderno AND x.linenumber = l.linenumber
		  AND x.statusflag = 'D'
		  AND l.statusflag = 'B'
		  AND x.tranclass = l.tranclass 
		  AND x.tranclass = 'Service Order'
	 
	   
	 -- inserting missing orders for service orders -- especially where don't have orders for spare parts  
	 INSERT INTO LineitemOracleExport   
	 (  
	  acctno,  agrmtno,  itemno,  
	  contractno,  quantity,  stocklocn,  
	  ordval,  [type],  runno,  
	  buffno,  orderno,  orderlineno  
	 )   
	 SELECT acctno, agrmtno, itemno,   
	 deliverycomments, orderedqty, Stocklocn, -- not putting in contract number for delivery comments..   
	 lineamount,CASE statusflag  
	 WHEN  'B'  THEN 'O'  
	 else 'D'  
	 END , @runno,  
	 l.buffno, NULL,NULL   
	 FROM #temp l WHERE   
	 tranclass = 'Service Order'  
	 and statusflag IN ('b','D')  
	 AND NOT EXISTS   
	 (SELECT 'x' FROM lineitemOracleExport x   
	 WHERE l.acctno= x.acctno AND l.itemno= x.itemno   
		AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno  
		AND x.ordval = l.lineamount AND x.runno= @runno )  
	 AND runno= @runno       
	 
	 declare @partsmarkup float
	 select @partsmarkup= value from CountryMaintenance where Name = 'Service Parts Mark Up' 
	 if isnull(@partsmarkup,0)=0  set @partsmarkup=1
	 else set @partsmarkup = (@partsmarkup/100)+1
	 
	 UPDATE l SET unitprice=round(unitprice*@partsmarkup,0),
	 lineamount = round(unitprice*@partsmarkup,0)*OrderedQty
	FROM #temp l, SR_ChargeAcct
	WHERE l.deliverycomments=sr_chargeacct.ServiceRequestNo
			AND ChargeType='C'
		  AND l.statusflag in ( 'B')
		  AND l.tranclass = 'Service Order'
		  And l.itemno!='7L0001'
		  
	UPDATE l SET unitprice=round(unitprice*1.2,0),
	 lineamount =  round(unitprice*1.2,0)*OrderedQty
	FROM #temp l, SR_ChargeAcct
	WHERE l.deliverycomments=sr_chargeacct.ServiceRequestNo
			AND ChargeType='W'
		  AND l.statusflag in ( 'B')
		  AND l.tranclass = 'Service Order'
		  And l.itemno!='7L0001'
		 
	UPDATE l SET unitprice=round(unitprice*@partsmarkup,0),
	 lineamount = round(unitprice*@partsmarkup,0)*deliveredQty
	FROM #temp l, SR_ChargeAcct
	WHERE l.deliverycomments=sr_chargeacct.ServiceRequestNo
			AND ChargeType='C'
		  AND l.statusflag in ( 'D')
		  AND l.tranclass = 'Service Order'
		  And l.itemno!='7L0001'
		  
		 UPDATE l SET unitprice=round(unitprice*1.2,0),
	 lineamount =  round(unitprice*1.2,0)*deliveredqty
	FROM #temp l, SR_ChargeAcct
	WHERE l.deliverycomments=sr_chargeacct.ServiceRequestNo
			AND ChargeType='W'
		  AND l.statusflag in ( 'D')
		  AND l.tranclass = 'Service Order'
		  And l.itemno!='7L0001'
		  	  
	 
	 -- issue is that there may be multiple service lines so won't be able to distinguish....   
	 -- another issue is that on re-running would not get the same data ....   
	 EXEC OracleUpdateOrderlines @runno= @runNo, @service = 'Y' -- updating the order line number....   
	  
	 UPDATE x   
	 SET linenumber = l.orderlineno,   
	 orderno = l.orderno   
	 FROM #temp x , lineitemOracleExport l   
	 WHERE l.acctno= x.acctno AND l.itemno= x.itemno   
		AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno  
		AND X.deliveryComments = l.contractno   
	  AND x.tranclass = 'Service Order'   


	  IF @logging = 1  
	 begin   
	  SELECT 'xx1',* FROM #temp   
	 end  
	 -- remove charge to parts as now parts have value assigned  
	 DELETE FROM #temp WHERE itemno= @Courtspartid OR ISNULL(acctno,'') =''  
	 IF @logging = 1  
	 begin   
	  SELECT 'xx2',* FROM #temp   
	 end   
	 --SELECT * FROM SR_ChargeTo  
	 --SELECT * FROM SR_ChargeAcct WHERE acctno = '730406644760'  
	 --------------------------  
	 UPDATE #TEMP  
	 SET StatusFlag = Case  
		  When TranClass = 'Return' Then 'R'  
		  Else StatusFlag  
		  End  
	 WHERE StatusFlag = 'D'  
	  
	 --------------------------  
	 
	 --------------------------  
	 UPDATE #TEMP   -- Correcting the unitprice for DT and Discounts -- Discounts can be positive...   
	 SET UnitPrice = Case  
		  --When ItemType = 'DISCOUNT' Then -1 * ABS(UnitPrice)   
		  When ItemType = 'DT' Then ABS(UnitPrice)   
		  Else UnitPrice  
		 End   
	 WHERE ---ItemType = 'DISCOUNT' or   
	 ItemType = 'DT'  
	  
	 --------------------------  
	 UPDATE #TEMP  
	 SET DeliveredDate = NULL,  
	  DeliveryNumber = NULL,  
	  DeliveredQty = 0  
	 WHERE StatusFlag != 'D'  -- Delivery details must be provided only for Delivery   
	  
	 --------------------------  
	 UPDATE #TEMP  
	 SET ReturnReason = NULL  
	 WHERE StatusFlag != 'R' and ReturnReason is NOT NULL -- Return Reason must be provided only for Returns   
	  
	 --------------------------  
	 UPDATE #TEMP  
	 SET CancelReason = NULL  
	 WHERE StatusFlag != 'C' and CancelReason is NOT NULL -- Cancel Reason must be provided only for Cancels   
	  
	  
	   
	 -- setting ordered date where incorrect -- to start with taken from agreement.dateagrmt.   
	 BEGIN -- Mauritius want this to be the date the item added or if for a collection and re-delivery the date the item was added...   
	 -- firstly make sure that order date has to be less than delivery date.  
	 UPDATE #temp SET OrderedDate = ISNULL((SELECT  max(a.Datechange) FROM   
	 LineitemAudit a WHERE #temp.agrmtno=1 AND a.acctno =#temp.acctno AND a.Datechange < OrderedDate   
	 AND a.itemno=#temp.itemno AND a.QuantityAfter >0  
	 AND  a.datechange < ISNULL((SELECT MAX(datedel) FROM delivery d WHERE   
	 a.acctno= d.acctno AND a.itemno= d.itemno AND a.stocklocn=  d.stocklocn  
	 AND a.agrmtno = d.agrmtno ),GETDATE())  
	 AND  ordereddate > ISNULL((SELECT MAX(datedel) FROM delivery d WHERE   
	  a.acctno= d.acctno AND a.itemno= d.itemno AND a.stocklocn=  d.stocklocn  
	  AND a.agrmtno = d.agrmtno ),GETDATE())  
	 ),ordereddate)   
	 WHERE  acctno NOT LIKE '___5%'  
	  
	 -- now make sure that if collection then get the correct date -- this should be the   
	 UPDATE #temp SET OrderedDate = ISNULL((SELECT  max(a.Datechange) FROM   
	 LineitemAudit a WHERE #temp.agrmtno=1 AND a.acctno =#temp.acctno   
	 AND a.itemno=#temp.itemno AND a.QuantityAfter >0  
	 AND  a.datechange <   
	  ISNULL((SELECT MAX(datedel) FROM delivery d WHERE   
	  a.acctno= d.acctno AND (a.itemno= d.itemno OR a.itemno= d.retitemno) AND a.stocklocn=  d.stocklocn  
	  AND a.agrmtno = d.agrmtno AND d.delorcoll='C' ),GETDATE())  
	 ),ordereddate)   
	 WHERE  acctno NOT LIKE '___5%'  
	  
	 --- if no delivery then want earliest order date....   
	 UPDATE #temp SET OrderedDate = ISNULL((SELECT  max(a.Datechange) FROM   
	 LineitemAudit a WHERE #temp.agrmtno=1 AND a.acctno =#temp.acctno   
	 --AND a.Datechange < OrderedDate  removing as want latest date of revision  
	 AND a.agrmtno= 1   
	 AND a.itemno=#temp.itemno  --AND a.QuantityAfter >0  
	  AND NOT EXISTS (SELECT datedel FROM delivery d WHERE   
	 a.acctno= d.acctno AND a.itemno= d.itemno AND a.stocklocn=  d.stocklocn  
	 AND a.agrmtno = d.agrmtno )  
	 ),ordereddate)   
	 WHERE  acctno NOT LIKE '___5%'  
	  
	 -- if collection or redelivery  
	 UPDATE #temp SET OrderedDate = isnull((select MAX(d.datedel) FROM delivery d   
	 WHERE d.acctno= #temp.acctno AND d.agrmtno = #temp.agrmtno --AND d.delorcoll ='D'   
	 AND d.agrmtno =#temp.agrmtno  
	 AND d.agrmtno= #temp.agrmtno AND DATEDIFF(day,#temp.ordereddate,d.datedel) >4 ),ordereddate)  
	  
	 -- for cash and go agrmtno >1 use delivery date for delivery.              
	 UPDATE #temp SET OrderedDate = isnull((select MAX(d.datedel) FROM delivery d   
	 WHERE d.acctno= #temp.acctno AND d.agrmtno = #temp.agrmtno AND d.delorcoll ='D' AND d.agrmtno >1  
	 AND d.agrmtno= #temp.agrmtno),ordereddate)  
	  
	 -- order date cannot exceed delivered date...   
	 update x  set ordereddate= ISNULL((SELECT MIN(l.delivereddate)  
	 FROM #temp l  
	 where   
	 l.acctno= x.acctno AND l.itemno= x.itemno   
	 AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno  
	 AND l.delivereddate <x.ordereddate and l.delivereddate is not NULL),ordereddate)  
	 FROM #temp x  
	  
	 -- order date cannot exceed drop off time or pickup time  
	 update x  set ordereddate= ISNULL((SELECT MIN(l.DropOffTime)  
	 FROM #temp l  
	 where   
	 l.acctno= x.acctno AND l.itemno= x.itemno   
	 AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno  
	 AND l.DropOffTime <x.ordereddate and l.DropOffTime is not NULL),ordereddate)  
	 FROM #temp x  
	  
	 update x  set ordereddate= ISNULL((SELECT MIN(l.PickUpTime)  
	 FROM #temp l  
	 where   
	 l.acctno= x.acctno AND l.itemno= x.itemno   
	 AND l.stocklocn=  x.stocklocn AND l.agrmtno = x.agrmtno  
	 AND l.PickUpTime <x.ordereddate and l.PickUpTime is not NULL),ordereddate)  
	 FROM #temp x  
	  
	  
	 END   
	  
	 -----------------------------------------------------------------------------------------------------  
	  
	 -- remove where items are in the thing but shouldn't be...  
	 BEGIN   
	 CREATE TABLE #deltotsAll (acctno CHAR(12), agrmtno INT , itemno VARCHAR(10), quantity INT ,delvalue MONEY, ordvalue MONEY , delqty INT)   
	  
	 INSERT INTO #deltotsAll (acctno,agrmtno,itemno,quantity,ordvalue )  
	 SELECT acctno,agrmtno,itemno,SUM(quantity),SUM(ordval) FROM lineitem l WHERE EXISTS (   
	 SELECT * FROM #temp x WHERE   
	 l.acctno= x.acctno AND l.itemno= x.itemno   
	 AND l.agrmtno = x.agrmtno)  
	 GROUP BY acctno,agrmtno,itemno  
	  
	 UPDATE x  
	 SET delqty = (SELECT SUM(quantity)FROM  delivery l  
	 WHERE l.acctno= x.acctno AND l.itemno= x.itemno   
		AND l.agrmtno = x.agrmtno)  
	 FROM #deltotsAll x   
	  
	 UPDATE x  
	 SET delvalue = (SELECT SUM(transvalue) FROM  delivery l  
	 WHERE l.acctno= x.acctno AND l.itemno= x.itemno   
		AND l.agrmtno = x.agrmtno)  
	 FROM #deltotsAll x   
	  
	  IF @logging = 1  
	 begin   
	  SELECT 'xa1',* FROM #temp   
	 end   
	 -- Remove where no movement and no changes  
	 delete l FROM #temp l WHERE   statusflag ='U' and runno =@runNo   
	 AND NOT EXISTS (SELECT * FROM delivery d   
	 WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn  
		AND l.agrmtno = d.agrmtno AND d.datetrans > DATEADD (DAY,-7,@datestart) )  
	 AND NOT EXISTS (SELECT * FROM lineitemaudit x,StockInfo s WHERE   
	 l.acctno= x.acctno AND l.itemno= x.itemno   
	 AND l.stocklocn=  x.stocklocn and s.itemno= x.itemno   
	 and ((s.itemtype = 'N' and x.valuebefore !=x.valueafter)  
	 or (s.itemtype !='N' and x.quantitybefore !=x.quantityafter))  
	 --AND l.contractno = x.contractno   
	 AND l.agrmtno = x.agrmtno   
	 AND x.datechange >dateadd(DAY,-7,@datestart) )     
	 AND  EXISTS (SELECT * FROM #deltotsAll x   
	 WHERE l.acctno= x.acctno AND l.itemno= x.itemno AND l.lineamount=x.delvalue  
	  AND l.agrmtno = x.agrmtno)  
	 -- remove if update and value not equal to delivery total... . AAch -- don't need to do this any more....  
	   IF @logging = 1  
	 begin   
	  SELECT 'xa2',* FROM #temp   
	 end 
	 END   
	 
	 --RETURN   
		-- updating return item numbers and return locations.   
	 UPDATE #temp SET   
	 OriginalItemCode = itemno,  
	 itemno =  ReturnItemno ,
	 deliveredfromlocn=retstocklocn  
	 WHERE  ReturnItemno !='' AND Returnitemno !='0'  
	 AND (statusflag IN ('P','R')  OR tranclass ='Rdelivery')
	 
	  UPDATE l 
	  SET   
	 l.OriginalItemCode = t2.itemno,  
	 l.itemno =  t2.ReturnItemno, 
	 l.deliveredfromlocn = (case when t2.retstocklocn!=0 then t2.retstocklocn else t2.stocklocn end)
	 FROM #temp t2, #temp l
	 where t2.acctno=l.acctno
	 and t2.itemno=l.itemno
	 and t2.stocklocn=l.stocklocn
	 and t2.agrmtno=l.agrmtno
	 and t2.ReturnItemno !='' AND t2.Returnitemno !='0'  
	 AND (t2.statusflag IN ('D') OR t2.tranclass ='Rdelivery')
	 and l.statusflag='B'
	    
	 
	UPDATE #temp SET  tranclass = 'Delivery' WHERE TranClass= 'RDelivery'  
	  
	 -- update the tax   
	 DECLARE @taxrate FLOAT  
	 SET @taxrate = 15.0 -- hard coded currently which is bad, but Mauritius are currently not storing the taxrate in their database  
	 -- do totals before tax taken off   
	 EXEC OracleIntegrationUpdateSummaryTotals @runno =@runNo  
	  
	  
	 UPDATE #temp   
	 set TaxRate = 0.0,  
	 TaxFlag = 'N',  
	 TaxCode = ''  
	 WHERE EXISTS (SELECT * FROM stockitem s WHERE s.itemno= #temp.itemno AND s.StockLocn = #temp.stocklocn  
	 AND unitpricehp = 0  
	 AND category not in (14, 84, 36, 37, 38, 46, 47, 48, 86, 87, 88) )  
	 -----------------------------------------------------------------------------------------------------  
	 -- remove old records from > 2 months but not during busy periods -- so after 5 pm.  
	 
	   
	   
	  
	  
	 DELETE FROM OracleExportHist WHERE runno =@runno  
	 DELETE FROM #temp WHERE itemno= 'rb' -- no rebates required....   
	  
	 delete from #temp where runno in (1,2,3) and statusflag = 'U'  
	 
	   

	 delete l -- remove where order and not exists an order for a stockitem for the same agreement  
	 from #temp l where l.runno in (1,2,3) and l.statusflag = 'O'  
	 and not exists (SELECT * FROM #temp x   
	 JOIN StockInfo s ON s.itemno = x.itemno   
	 WHERE s.itemtype !='N' AND l.acctno= x.acctno   
	  AND l.agrmtno = x.agrmtno)  
	  
		 IF @logging = 1  
	 begin   
	  SELECT 'xc1',* FROM #temp   
	 end 
	 
	---------------------------------------------------------------------------
	 -- temp fix -- no order lines for these records
	 delete from #temp where isnull(linenumber,0)=0
	 
	 INSERT INTO LineitemOracleExport (		acctno,		agrmtno,		itemno,		contractno,		quantity,		stocklocn,
		ordval,		[type],		runno,		buffno,		orderno,		orderlineno	)  	
	 select acctno, agrmtno, itemno,contractno,quantity,stocklocn,ordval,'O',0,0,orderno, null
	 from lineitemoracleexport where runno>1383
	 and isnull(orderlineno,0)=0

	 
	 update LineitemOracleExport 
	 set runno=0 where runno>1383
	 and isnull(orderlineno,0)=0
	 
	 -- remove duplicate bookings
	 delete from #temp 
	 where exists (select 'x' from LineitemOracleExport l
	where #temp.agrmtno=l.agrmtno
	and #temp.itemno=l.itemno
	and #temp.stocklocn=l.stocklocn
	and #temp.orderno=l.orderno
	and #temp.linenumber=l.orderlineno
	and l.type in ('B','C')
	and l.runno<@runno)
	and #temp.runno=@runno
	and #temp.statusflag='B'
	
	 update #temp set orderedqty=1, lineamount=ABS(lineamount) where itemno='DT' and statusflag='B'
	 
	 update #temp set orderedqty=-1 where itemno='DT' and lineamount<0 and statusflag!='B'
	
	  
	 -- issue with agreement no 1 on deliveries not getting delqty
	 UPDATE #temp   
	 SET Deliveredqty = orderedqty   
	 WHERE tranclass = 'Service Order'  
	 AND isnull(deliveredqty,0)=0 AND statusflag = 'd'
	 
	 update #temp
	 set buffno=ISNULL((select MAX(buffno) from delivery
	 where agrmtno=#temp.agrmtno
	 and itemno=#temp.itemno
	 and stocklocn=#temp.stocklocn
	 and agrmtno!=1
	 and datetrans>dateadd(day,-2,getdate())),0)
	 
	 update #temp
	 set orderedqty=lineamount/unitprice
	 where orderedqty=0 and unitprice!=0 and lineamount!=0
	 
	 update #temp 
	 set orderedqty=case when orderedqty>0 then orderedqty*-1 else orderedqty end, 
	 unitprice=ABS(unitprice) where tranclass='Repossession'
	
	 
	---------------------------------------------------------------------------
	 INSERT  INTO OracleExportHist (  
	  RunNo, AcctNo, OrderNo, InterfacedDate,  
	  OrderedDate, OrderType, TranClass, PayTerm,  
	  PayMethod, BranchNo, CustId, BillToAddr,  
	  ShipToAddr, SalesPersonId, SalesPersonName, SalesChannel,  
	  LineNumber, ItemNo, ItemDesc, UOM,  
	  ItemType, UnitPrice, LineAmount, TaxFlag,  
	  TaxCode, TaxRate, StatusFlag, OrderedQty,  
	  CancelReason, ReturnReason, ReturnItemno, DeliveryNumber,  
	  DeliveredDate, DeliveredQty, DeliveredFromLocn, DeliveryComments,  
	  DropOffTime, PickUpTime, FreightCarrier, FreightCharge,  
	  ScheduledDelDate, BuffNo, AgrmtNo, StockLocn,  
	  OriginalItemCode,NotifiedDelDate  
	 )  SELECT   
	  RunNo, AcctNo, OrderNo, InterfacedDate,  
	  OrderedDate, OrderType, TranClass, PayTerm,  
	  PayMethod, BranchNo, CustId, BillToAddr,  
	  ShipToAddr, SalesPersonId, SalesPersonName, SalesChannel,  
	  LineNumber, ItemNo, ItemDesc, UOM,  
	  ItemType, UnitPrice, LineAmount, TaxFlag,  
	  TaxCode, TaxRate, StatusFlag, OrderedQty,  
	  CancelReason, ReturnReason, ReturnItemno, DeliveryNumber,  
	  DeliveredDate, DeliveredQty, DeliveredFromLocn, DeliveryComments,  
	  DropOffTime, PickUpTime, FreightCarrier, FreightCharge,  
	  ScheduledDelDate, BuffNo, AgrmtNo, StockLocn,  
	  OriginalItemCode,NotifiedDelDate  
	  FROM #temp   
	 
	 
	  
	 SELECT DISTINCT OrderNo  
	 FROM #export   
	 WHERE RunNo = @runNo and OrderNo is not NULL  and isnull(orderlineno  ,0)!=0
	 AND exists (SELECT * FROM #TEMP T WHERE T.ORDERNO=#export.orderno)  
	 ORDER BY OrderNo  
	  
	  
	  
	 SELECT * FROM #temp -- where tranclass like 'serv%'
	 where ISNULL(linenumber,0)!=0 
	  
	  SET @return = @@error  
	  
	 IF DATEPART(hour, GETDATE() ) > '17'  
	 BEGIN 
		insert into OracleExportHisttemp
		select *
		FROM OracleExportHist   
		WHERE RunNo>(select MAX(runno) from OracleExportHisttemp)
	 
		
	  
	  DELETE FROM OracleExportHist   
	  WHERE EXISTS (SELECT * FROM interfacecontrol i WHERE interface = 'OrInteg2' AND datestart < DATEADD(DAY,-60, GETDATE())  
	  AND i.runno= OracleExportHist.runno)  
	 END   

	GO
