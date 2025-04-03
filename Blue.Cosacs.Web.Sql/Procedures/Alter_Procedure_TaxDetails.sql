  IF EXISTS (SELECT * FROM SYS.OBJECTS  WHERE OBJECT_ID = OBJECT_ID(N'[dbo].[TaxDetails]') AND TYPE IN (N'P', N'PC'))
		 DROP PROCEDURE [dbo].[TaxDetails]
  GO 
--exec [TaxDetails] 'OB','2019-08-01','2019-08-30', '411'
CREATE PROCEDURE [dbo].[TaxDetails] 
--===================================================================================================
-- Author     :	<Zensar>
-- Create date: <08/10/2019>
--Version    :	<001> 
-- Description:	That Reeport is Tax Type Calcuation (OB,LUX,NonTaxable). Type OB Calcuation 6%, Type LUX calcuation 9%  NonTaxable calcuation 0.
				-- Exclude Account no ''422500001880' in this reports. ItemNo='RB' Consider on OB Type.custid='INTERCOBON' showing on Non taxable Type.
				-- Ticket No refernce Ticket no. - #6488088. 	
--===================================================================================================			 		
		@taxtype VARCHAR(10), 
		@DateFrom DATETIME,
		@DateTo DATETIME, 
		@Branch VARCHAR(6)
AS
BEGIN
SET NOCOUNT ON 
  DECLARE @totalsales_lux MONEY,
          @totalsalesorder_lux INT,
          @totalsales_ob MONEY,
          @totalsalesorder_ob INT,
          @totalsales_del MONEY,
          @totalsalesorder_del INT,
          @normtaxamt_LUX MONEY,
          @normtaxamt_OB MONEY,
          @TotalNonTaxable MONEY

		SELECT RANK() OVER(PARTITION BY AgreementInvNoVersion ORDER BY AgreementInvNoVersion) AS NoP,acctno,itemno,AgreementInvNoVersion,agrmtno,
			 stocklocn,quantity,Price INTO #Invoicedetails  FROM Invoicedetails  WHERE (stocklocn = @Branch OR @Branch = '0')
 
	    SELECT d.*,(SELECT TOP 1  l.AgreementInvNoVersion  FROM #Invoicedetails l WHERE d.acctno = l.acctno  AND  d.itemno=l.itemno AND d.quantity=l.quantity AND d.agrmtno=l.agrmtno ) 
		AS AgreementInvoiceNumber,t.Name,
			(SELECT TOP 1 dd.title + '. ' + dd.name +' ' + Dd.firstname   FROM custacct c , customer dd  WHERE c.custid=dd.custid AND  c.AcctNo = d.acctno) AS CutomerName,
			(SELECT TOP 1  l.TaxRate  FROM lineitem l WHERE d.acctno = l.acctno AND d.itemno = l.itemno AND d.agrmtno=l.agrmtno) AS TaxRate,	
			(SELECT MAX(d2.TaxAmtAfter)    FROM LineItemaudit d2 WHERE d2.acctno =  d.acctno AND d2.ItemID = d.ItemID AND d2.stocklocn = d.stocklocn AND d2.contractno = D.contractno
			AND d2.ParentItemID =D.ParentItemID AND d2.ValueAfter=d.transvalue AND d2.valueafter>0 ) AS taxamt 
		INTO #Delivery   FROM delivery d LEFT JOIN  merchANDising.product AS p  WITH(NOLOCK) ON d.itemno = p.sku 
			LEFT JOIN [MerchANDising].[TaxRate] t  WITH(NOLOCK) ON t.ProductId = P.id 
		WHERE d.datetrans    BETWEEN @DateFrom  AND @DateTo    AND D.Acctno<>'422500001880' 

		  SELECT  
			  CASE WHEN taxamt IS NULL AND  transvalue >0 THEN (SELECT TOP 1  l.TaxAmt  FROM lineitem l  WHERE t.acctno = l.acctno AND t.itemno = l.itemno AND t.agrmtno=l.agrmtno)
				   WHEN transvalue <0 THEN (( transvalue/ ((100 + 9.00) / 100) /quantity) * quantity) * 9.00 / 100   ELSE taxamt 
			  END as taxamt 
		  INTO #LUXSum from #Delivery t 
		  WHERE t.datetrans BETWEEN @DateFrom AND @DateTo  AND (t.stocklocn = @Branch  OR @Branch = '0')  AND taxrate IN (9,0.09)

		SELECT * INTO #OBTAXSUM FROM 
		(SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,taxrate,
			CASE WHEN (taxamt<>(( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100 AND taxamt<>0 ) THEN  (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100
			WHEN (itemno IN ('DEO','DFO','DEC','DFM','DES','DEP','DEM') AND quantity<0) THEN (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100
			ELSE taxamt END AS TaxAmt,  buffno,CutomerName
		FROM  (SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,taxrate,
			   CASE WHEN taxamt IS NULL AND  transvalue >0 THEN (SELECT TOP 1 (l.TaxAmt) FROM lineitem l  WHERE t.acctno = l.acctno AND t.itemno = l.itemno AND t.agrmtno=l.agrmtno)
			   WHEN   transvalue <0 then (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100  
			   ELSE(( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100
			   End as taxamt ,buffno,CutomerName			
		FROM #Delivery t  
			Where  taxrate IN (6,0.06)  AND datetrans BETWEEN @DateFrom AND @DateTo AND (t.stocklocn = @Branch  OR @Branch = '0') 
		UNION ALL 
		SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,ISNULL(taxrate,6) AS taxrate,
			CASE WHEN taxamt IS NULL AND  transvalue >0 THEN (SELECT TOP 1  l.TaxAmt  FROM lineitem l  WHERE t.acctno = l.acctno AND t.itemno = l.itemno AND t.agrmtno=l.agrmtno  )
			WHEN   transvalue <0 THEN (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100   ELSE isnull(taxamt,'0.00')  END AS taxamt ,buffno,CutomerName
		FROM #Delivery t  
			WHERE    ItemNo='RB'  AND datetrans BETWEEN @DateFrom AND @DateTo AND (t.stocklocn = @Branch  OR @Branch = '0') )a
				)b
			WHERE acctno NOT IN  (SELECT AcctNo FROM custacct WHERE custid='INTERCOBON')
		 	
	
   SELECT @totalsales_lux = SUM(transvalue) FROM #Delivery WHERE taxrate IN (9,0.09)  
   SELECT @totalsalesorder_lux = (SELECT COUNT(acctno) FROM #Delivery WHERE taxrate IN (9,0.09)  )
   SELECT @TotalNonTaxable = (SELECT SUM(Total) FROM ( 
   SELECT SUM(transvalue) AS Total from #Delivery d WHERE taxrate IS NULL  AND datetrans BETWEEN @DateFrom AND @DateTo  AND (stocklocn = @Branch OR @Branch = '0' AND ItemNo<>'RB'))a)
   SELECT @totalsales_del = (SELECT SUM(transvalue) FROM #Delivery d  WITH(NOLOCK)  WHERE datetrans BETWEEN @DateFrom AND @DateTo  AND (d.stocklocn = @Branch OR @Branch = '0'))
   SELECT @totalsalesorder_del = (SELECT COUNT(DISTINCT d.acctno) FROM #Delivery d WITH(NOLOCK) WHERE datetrans BETWEEN @DateFrom AND @DateTo AND (stocklocn = @Branch  OR @Branch = '0'))
   SELECT @totalsales_ob = @totalsales_del - @totalsales_lux-@TotalNonTaxable
   SELECT @totalsalesorder_ob = @totalsalesorder_del - @totalsalesorder_lux
   SELECT @normtaxamt_LUX = (SELECT SUM(taxamt)  FROM #LUXSum WITH(NOLOCK)) 
   SELECT @normtaxamt_OB = (select SUM(taxamt) FROM #OBTaxSUM  WITH(NOLOCK))
   

IF (@taxtype = 'LUX')
	SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,taxrate,
		    CASE WHEN taxamt IS NULL AND  transvalue >0 THEN 
			(SELECT TOP 1  l.TaxAmt  FROM lineitem l  WHERE t.acctno = l.acctno AND t.itemno = l.itemno AND t.agrmtno=l.agrmtno)
		    WHEN   transvalue <0 THEN (( transvalue/ ((100 + 9.00) / 100) /quantity) * quantity) * 9.00 / 100   ELSE ISNULL(taxamt,'0.00') 
		    END AS taxamt,buffno,CutomerName,
			@taxtype AS name, 0 AS TotalSalesOrder, @totalsales_lux AS Totalsales_lux, @totalsalesorder_lux AS Totalsalesorder_lux, @totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,  @normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable FROM #Delivery t 
			WHERE t.datetrans BETWEEN @DateFrom AND @DateTo  AND (t.stocklocn = @Branch  OR @Branch = '0')  AND taxrate IN (9,0.09)

IF (@taxtype = 'OB')
	  		SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,taxrate,name,Totalsales_lux,Totalsalesorder_lux,Totalsales_del,Totalsalesorder_del,
		    Totalsales_ob,Totalsalesorder_ob,Totalsales_del,Normtaxamt_OB,Nontaxable,Normtaxamt_LUX,
				CASE WHEN (taxamt<>(( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100 AND taxamt<>0 ) THEN  (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100
					 WHEN (itemno IN ('DEO','DFO','DEC','DFM','DES','DEP','DEM') AND quantity<0) THEN (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100
				Else taxamt END AS TaxAmt,  buffno,CutomerName
			FROM  (
				SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,taxrate,
					CASE WHEN taxamt IS NULL AND  transvalue >0 THEN (SELECT TOP 1 (l.TaxAmt) FROM lineitem l  WHERE t.acctno = l.acctno AND t.itemno = l.itemno AND t.agrmtno=l.agrmtno)
						 WHEN   transvalue <0 THEN (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100  
					ELSE(( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100
			END AS taxamt ,buffno,CutomerName,
				@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux,@totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
				@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
				@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable 
			FROM #Delivery t  
			WHERE  taxrate IN (6,0.06)     AND datetrans BETWEEN @DateFrom AND @DateTo AND (t.stocklocn = @Branch  OR @Branch = '0') 
			UNION ALL 
			SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,ISNULL(taxrate,6) AS taxrate,
					CASE WHEN taxamt IS NULL AND  transvalue >0 THEN (SELECT TOP 1  l.TaxAmt  FROM lineitem l  WHERE t.acctno = l.acctno AND t.itemno = l.itemno AND t.agrmtno=l.agrmtno  )
						 WHEN  transvalue <0 THEN (( transvalue/ ((100 + 6.00) / 100) /quantity) * quantity) * 6.00 / 100   ELSE ISNULL(taxamt,'0.00') 
					END AS taxamt ,buffno,CutomerName,
			@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux,@totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable
			FROM #Delivery t 
			WHERE    ItemNo='RB'  AND datetrans BETWEEN @DateFrom AND @DateTo AND (t.stocklocn = @Branch  OR @Branch = '0') 
			) a    WHERE acctno NOT IN  (SELECT AcctNo from custacct WHERE custid='INTERCOBON')
		
IF (@taxtype = 'NonTaxable')
	SELECT a.* FROM (
	SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,ISNULL(taxrate,0) AS TaxRate,ISNULL(taxamt,0) AS Taxamt,buffno,CutomerName,
			@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux,@totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable from #Delivery t where taxrate IS NULL  
			AND datetrans BETWEEN @DateFrom AND @DateTo AND (stocklocn = @Branch  OR @Branch = '0') AND ItemNo<>'RB') a
	UNION ALL			 	 
			SELECT acctno,delorcoll,AgreementInvoiceNumber,datedel,itemno,stocklocn,quantity,transvalue,ISNULL(taxrate,0) AS TaxRate,ISNULL(taxamt,0) AS Taxamt,buffno,CutomerName,
			@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux,@totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable 
	FROM #Delivery t WHERE T.acctno IN (SELECT AcctNo FROM custacct WHERE custid='INTERCOBON') 

DROP TABLE #Invoicedetails
DROP TABLE #Delivery
DROP TABLE  #OBTaxSUM
DROP TABLE #LUXSum
END 