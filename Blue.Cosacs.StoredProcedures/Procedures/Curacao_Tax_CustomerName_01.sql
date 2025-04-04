  IF EXISTS (SELECT * FROM sys.objects  WHERE object_id = OBJECT_ID(N'[dbo].[TaxDetails]') AND type IN (N'P', N'PC'))
  DROP PROCEDURE [dbo].[TaxDetails]
GO 
--exec [TaxDetails] 'OB','2019-09-25','2019-09-27', '422'
create PROCEDURE [dbo].[TaxDetails] @taxtype varchar(10), @DateFrom datetime, @DateTo datetime, @Branch varchar(6)
AS
BEGIN

  DECLARE @totalsales_lux money,
          @totalsalesorder_lux int,
          @totalsales_ob money,
          @totalsalesorder_ob int,
          @totalsales_del money,
          @totalsalesorder_del int,
          @normtaxamt_LUX money,
          @normtaxamt_OB money,
          @TotalNonTaxable money

  SELECT  d.*,p.id AS ProductId, l.taxamt,l.taxrate  INTO #temp1
		FROM delivery AS d with(Nolock) 
		JOIN merchandising.product AS p  with(Nolock) ON d.itemno = p.sku  JOIN lineitem l with(Nolock)
		ON d.acctno = l.acctno  AND d.itemno = l.itemno   
  WHERE d.datetrans BETWEEN @DateFrom  AND @DateTo and l.ordval<>0 
		AND (d.stocklocn = @Branch  OR @Branch = '0') 

  SELECT t1.*,t.Name INTO #temp2
		FROM #temp1 t1  INNER JOIN [Merchandising].[TaxRate] t  with(Nolock) ON t.ProductId = t1.ProductId
  WHERE t1.datetrans BETWEEN @DateFrom AND @DateTo
		AND (t1.stocklocn = @Branch OR @Branch = '0') AND t.[NAME] != 'OB'

select rank() OVER(Partition by AgreementInvNoVersion ORDER BY AgreementInvNoVersion) as NoP,acctno,itemno,AgreementInvNoVersion,agrmtno,
stocklocn,quantity,Price into #Invoicedetails  from Invoicedetails where (stocklocn = @Branch OR @Branch = '0')


  select Distinct acctno,  transvalue into #Lux from  #temp2 where datetrans BETWEEN  @DateFrom AND @DateTo and Name='LUX' and taxamt > 0 

  SELECT @totalsales_lux = sum(transvalue) from #Lux  
  print @totalsales_lux

  SELECT @totalsalesorder_lux = (SELECT COUNT(DISTINCT d.transvalue) FROM delivery d with(Nolock) JOIN lineitem o  ON d.acctno = o.acctno   
		AND d.itemno = o.itemno JOIN merchandising.product AS p   with(Nolock) ON d.itemno = p.sku   
		INNER JOIN [Merchandising].[TaxRate] t  with(Nolock)  ON t.ProductId = p.Id 
  WHERE d.datetrans BETWEEN @DateFrom AND @DateTo   AND o.taxamt > 0 AND t.Name = 'LUX')

  select distinct AcctNo,ItemNo,transvalue into #Nontaxable from #temp2 where datetrans BETWEEN @DateFrom AND @DateTo and taxamt = 0 and taxrate=0

SELECT @TotalNonTaxable = (SELECT SUM(t2.transvalue) FROM #Nontaxable t2 )
PRINT @TotalNonTaxable
SELECT @totalsales_del = (SELECT SUM(transvalue) FROM delivery d  with(Nolock)  WHERE datetrans BETWEEN @DateFrom AND @DateTo  AND (d.stocklocn = @Branch OR @Branch = '0'))
SELECT @totalsalesorder_del = (SELECT COUNT(DISTINCT d.acctno) FROM delivery d with(Nolock) WHERE datetrans BETWEEN @DateFrom AND @DateTo AND (d.stocklocn = @Branch  OR @Branch = '0'))
SELECT  @totalsales_ob = @totalsales_del - @totalsales_lux-@TotalNonTaxable
SELECT @totalsalesorder_ob = @totalsalesorder_del - @totalsalesorder_lux
SELECT  @normtaxamt_LUX = (SELECT DISTINCT(Rate)  FROM [Merchandising].[TaxRate]  with(Nolock) WHERE Name = 'LUX')  * (SELECT  @totalsales_lux)
SELECT  @normtaxamt_OB = (SELECT DISTINCT (Rate)  FROM [Merchandising].[TaxRate] with(Nolock) WHERE Name = 'OB')  * (SELECT   @totalsales_Ob)
print @totalsales_ob


IF (@taxtype = 'LUX')
Select a.acctno,delorcoll,(Select Top 1  l.AgreementInvNoVersion  From #Invoicedetails l Where a.acctno = l.acctno  AND  a.itemno=l.itemno and a.quantity=l.quantity and a.agrmtno=l.agrmtno ) 
as AgreementInvoiceNumber,a.datedel,a.itemno,a.stocklocn,a.quantity,a.transvalue,a.taxrate,a.taxamt,buffno,
(select top 1 d.title + '. ' + d.name +' ' + D.firstname   from custacct c , customer d  where c.custid=d.custid and  c.AcctNo = a.acctno) as CutomerName,
			@taxtype AS name, 0 AS TotalSalesOrder, @totalsales_lux AS Totalsales_lux, @totalsalesorder_lux AS Totalsalesorder_lux, @totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,  @normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable
 From ( SELECT Distinct t2.acctno,  agrmtno,buffno,
		(Select Top 1  l.AgreementInvNoVersion  From Invoicedetails l Where t2.acctno = l.acctno  AND  t2.itemno=l.itemno and t2.transvalue=l.price and  t2.datetrans>'2019-05-14' ) As agg
		,t2.datedel,t2.delorcoll, t2.itemno, t2.stocklocn, t2.quantity, t2.transvalue,t2.taxrate,
CASE  WHEN   (Select Top 1  l.taxamt  From lineitem l  Where t2.acctno = l.acctno and t2.itemno = l.itemno and t2.agrmtno=l.agrmtno)  = 0
THEN 0 ELSE   (( t2.transvalue/ ((100 + 9.00) / 100) /t2.quantity) *t2.quantity) * 9.00 / 100    END AS taxamt 			
FROM #temp2 t2  
	 WHERE t2.datetrans BETWEEN @DateFrom AND @DateTo
	 AND (t2.stocklocn = @Branch  OR @Branch = '0')  and   t2.Name='LUX' and t2.taxamt > 0  ) a   

IF (@taxtype = 'OB')
Select  a.AcctNo,delorcoll, buffno,
(Select Top 1  l.AgreementInvNoVersion  From #Invoicedetails l Where a.acctno = l.acctno  AND  a.itemno=l.itemno and a.quantity=l.quantity and a.agrmtno=l.agrmtno ) 
as AgreementInvoiceNumber,A.datedel,a.itemno,a.stocklocn,a.quantity,a.transvalue,A.TaxRate,a.taxamt, 
(select top 1 d.title + '. ' + d.name +' ' + D.firstname   from custacct c , customer d  where c.custid=d.custid and  c.AcctNo = a.acctno) as CutomerName,
			@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux,@totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable 
From (SELECT  d.acctno, d.datedel, d.delorcoll, d.itemno, d.stocklocn, d.quantity,d.transvalue,agrmtno,buffno,
	(Select Top 1  l.AgreementInvNoVersion  From Invoicedetails l Where d.acctno = l.acctno  AND  d.itemno=l.itemno and d.transvalue=l.price  and  d.datetrans>'2019-05-14')   As AGG,
isnull((Select Top 1  l.taxamt  From lineitem l Where d.acctno = l.acctno and d.itemno = l.itemno and d.agrmtno=l.agrmtno),6) As TaxRate,			 
	 CASE  WHEN   (Select top 1  l.tAXRATE  from lineitem l  where d.acctno = l.acctno and d.itemno = l.itemno and d.agrmtno=l.agrmtno)  = 0
	 THEN 0 ELSE   (( d.transvalue/ ((100 + 6.00) / 100) /d.quantity) * d.quantity) * 6.00 / 100    END AS taxamt 			  
FROM delivery d  With(Nolock)
WHERE  d.datetrans    BETWEEN @DateFrom AND   @DateTo   and d.transvalue<>0    
 AND d.acctno + d.itemno not in(SELECT acctno+itemno FROM #temp2)) a where  a.acctno + a.itemno not in(SELECT acctno+itemno FROM #temp2)
	  AND (a.stocklocn = @Branch OR @Branch = '0')   and TaxRate>0
 
			    
IF (@taxtype = 'NonTaxable')
Select	a.acctno,delorcoll,buffno,(Select Top 1  l.AgreementInvNoVersion  From #Invoicedetails l Where a.acctno = l.acctno  AND  a.itemno=l.itemno and a.quantity=l.quantity and a.agrmtno=l.agrmtno ) 
as AgreementInvoiceNumber,a.datedel,a.itemno,a.stocklocn,a.quantity,a.transvalue,a.taxrate,a.taxamt,
(select top 1 d.title + '. ' + d.name +' ' + D.firstname   from custacct c , customer d  where c.custid=d.custid and  c.AcctNo = a.acctno) as CutomerName,
		@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux, @totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
		@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
		@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable
From (SELECT DISTINCT  t2.acctno,agrmtno,buffno, (Select Top 1  l.AgreementInvNoVersion From Invoicedetails l Where t2.acctno = l.acctno  AND  t2.itemno=l.itemno and t2.transvalue=l.price and  t2.datetrans>'2019-05-14' )   As AGG,
      t2.datedel,t2.delorcoll,t2.itemno,t2.stocklocn,t2.quantity,t2.transvalue,'0' as taxrate,'0' AS taxamt
FROM #temp2 t2 
WHERE t2.datetrans BETWEEN @DateFrom AND @DateTo  AND (t2.stocklocn = @Branch OR @Branch = '0') AND t2.taxamt = 0   ) a 
union all
Select  a.AcctNo,delorcoll, buffno,
(Select Top 1  l.AgreementInvNoVersion  From #Invoicedetails l Where a.acctno = l.acctno  AND  a.itemno=l.itemno and a.quantity=l.quantity and a.agrmtno=l.agrmtno ) 
as AgreementInvoiceNumber,A.datedel,a.itemno,a.stocklocn,a.quantity,a.transvalue,A.TaxRate,a.taxamt, 
(select top 1 d.title + '. ' + d.name +' ' + D.firstname   from custacct c , customer d  where c.custid=d.custid and  c.AcctNo = a.acctno) as CutomerName,
			@taxtype AS name,0 AS TotalSalesOrder,@totalsales_lux AS Totalsales_lux,@totalsalesorder_lux AS Totalsalesorder_lux,@totalsales_del AS Totalsales_del,
			@totalsalesorder_del AS Totalsalesorder_del,@totalsales_ob AS Totalsales_ob,@totalsalesorder_ob AS Totalsalesorder_ob,@normtaxamt_LUX AS Normtaxamt_LUX,
			@normtaxamt_OB AS Normtaxamt_OB,@TotalNonTaxable AS Nontaxable 
From (SELECT  d.acctno, d.datedel, d.delorcoll, d.itemno, d.stocklocn, d.quantity,d.transvalue,agrmtno,buffno,
	(Select Top 1  l.AgreementInvNoVersion  From Invoicedetails l Where d.acctno = l.acctno  AND  d.itemno=l.itemno and d.transvalue=l.price  and  d.datetrans>'2019-05-14')   As AGG,
isnull((Select Top 1  l.TaxRate  From lineitem l Where d.acctno = l.acctno and d.itemno = l.itemno and d.agrmtno=l.agrmtno),6) As TaxRate,'0' AS taxamt 			 
	 --CASE  WHEN   (Select top 1  l.tAXRATE  from lineitem l  where d.acctno = l.acctno and d.itemno = l.itemno and d.agrmtno=l.agrmtno)  = 0
	 --THEN 0 ELSE   (( d.transvalue/ ((100 + 6.00) / 100) /d.quantity) * d.quantity) * 6.00 / 100    END AS taxamt 			  
FROM delivery d  With(Nolock)
WHERE  d.datetrans    BETWEEN @DateFrom AND   @DateTo   and d.transvalue<>0  AND d.acctno + d.itemno not in(SELECT acctno+itemno FROM #temp2)) a where  a.acctno + a.itemno not in(SELECT acctno+itemno FROM #temp2)
	   AND (a.stocklocn = @Branch OR @Branch = '0')   

DROP TABLE #temp1
DROP TABLE #temp2 
drop table #Lux
drop table #Invoicedetails
END



