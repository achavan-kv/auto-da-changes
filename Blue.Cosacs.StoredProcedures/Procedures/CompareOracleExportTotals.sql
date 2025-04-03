IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CompareOracleExportTotals')
		DROP PROCEDURE CompareOracleExportTotals 
GO 
-- CompareOracleExportTotals is a procedure for Mauritius so that they can reconclie totals exported to Oracle to make sure that none are missed out
-- Deliveries for stocks and awarranties are treated different from non-stocks

CREATE PROCEDURE CompareOracleExportTotals
 @startdate DATETIME, @enddate DATETIME 
AS 

set @enddate= DATEADD(hour,23,@enddate)
set @enddate= DATEADD(minute,59,@enddate)
IF DATEADD(DAY,30,@startdate) <getdate() 
BEGIN
	PRINT ' Data already removed - please TRY running ON a backup'
	RETURN 
END

DECLARE @startrunno int, @endrunno INT 

SELECT @startrunno= MIN(runno) FROM interfacecontrol WHERE interface = 'updsmry'
AND datestart > @startdate 

SELECT @endrunno= MAX(runno)  FROM interfacecontrol WHERE interface = 'updsmry'
AND datestart > DATEADD(hour,23,@enddate)

--SET @startrunno = 0 SET @endrunno =0 -- make sure comment out...

SELECT 'Deliveries NOT Exported' AS DeliveriesNotExported,* FROM delivery d
WHERE  datetrans BETWEEN @startdate AND @enddate
AND NOT EXISTS (SELECT * FROM OracleExportHist l 
WHERE l.acctno= d.acctno AND (l.itemno= d.itemno OR l.itemno= d.retitemno ) -- AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND l.runno BETWEEN @startrunno AND @endrunno   AND statusflag IN ('R','U','D','P') )
	AND itemno !='RB'  

SELECT SUM(transvalue ) AS value,acctno , itemno,agrmtno ,stocklocn
INTO #deltots 
FROM  delivery l
WHERE EXISTS (SELECT * FROM OracleExportHist t WHERE t.acctno = l.acctno 
AND runno BETWEEN @startrunno AND @endrunno) 
AND datetrans < @enddate  AND l.itemno !='rb'
GROUP BY acctno ,itemno,agrmtno ,stocklocn 

SELECT 'Incorrect U record FOR non-stock',
* FROM OracleExportHist l 
JOIN stockinfo s ON l.itemno= s.itemno 
WHERE EXISTS 
(SELECT * FROM delivery d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datetrans < @startdate ) 
AND s.itemtype = 'N' AND s.category NOT IN (12,82)
AND  EXISTS (SELECT * FROM #deltots d WHERE d.acctno= l.acctno AND d.value != lineamount 
AND d.itemno= l.itemno AND d.stocklocn =l.stocklocn AND d.agrmtno= l.agrmtno) 
AND l.statusflag = 'U' AND l.runno BETWEEN @startrunno AND @endrunno  
  
SELECT 'Incorrect Update2 FOR non-stock order',l.* FROM OracleExportHist l 
JOIN stockinfo s ON l.itemno= s.itemno 
WHERE NOT EXISTS 
(SELECT * FROM delivery d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datetrans < @startdate ) 
	  AND EXISTS 
(SELECT * FROM LineitemAudit d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datechange  < @startdate ) 
AND s.itemtype = 'N' AND s.category NOT IN (12,82)
AND  EXISTS (SELECT * FROM lineitem d WHERE d.acctno= l.acctno AND d.ordval != l.lineamount
AND d.itemno= l.itemno AND d.stocklocn =l.stocklocn AND d.agrmtno= l.agrmtno) 
AND l.statusflag = 'U' AND l.runno BETWEEN @startrunno AND @endrunno 
AND l.itemno !='rb'
	
SELECT 'Missing UPDATE for non stock',*  FROM delivery d 
	JOIN stockinfo s ON d.itemno= s.itemno 
WHERE 
	  s.itemtype = 'N' AND s.category NOT IN (12,82) AND d.datetrans BETWEEN @startdate AND @enddate
	  AND EXISTS 
(SELECT * FROM delivery l 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND l.datetrans < @startdate 
	  AND NOT EXISTS (SELECT * FROM OracleExportHist x WHERE l.acctno= x.acctno AND l.itemno= x.itemno AND l.stocklocn=  x.stocklocn
	  AND l.agrmtno = x.agrmtno AND x.runno BETWEEN @startrunno AND @endrunno AND x.statusflag IN ('d','u') ) )
AND D.itemno !='rb'	  
SELECT value AS 'InterfacedStocksWarranties',SUM(transvalue ) AS deliveryTotalforStocks
FROM interfacevalue i, delivery d ,stockitem s 
WHERE interface = 'orinteg2' AND counttype1= 'StockWarTotal'
AND i.runno BETWEEN @startrunno AND @endrunno
AND d.datetrans BETWEEN @startdate AND @enddate
AND s.itemno= d.itemno AND s.stocklocn = d.stocklocn  
AND ( s.itemtype IN ('S','') OR s.category IN (12,82) )
GROUP BY value 

SELECT 'Incorrect Order Value FOR non-stock order -last run only',l.* FROM OracleExportHist l 
JOIN stockinfo s ON l.itemno= s.itemno 
WHERE NOT EXISTS 
(SELECT * FROM delivery d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datetrans < @enddate ) 
	  AND NOT EXISTS 
(SELECT * FROM LineitemAudit d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datechange  BETWEEN @startdate AND @enddate) 
AND  EXISTS (SELECT * FROM lineitem d WHERE d.acctno= l.acctno AND d.ordval != l.lineamount
AND d.itemno= l.itemno AND d.stocklocn =l.stocklocn AND d.agrmtno= l.agrmtno) 
AND l.statusflag = 'O' AND l.runno BETWEEN @startrunno AND @endrunno 
AND @endrunno = (SELECT MAX(runno) FROM interfacecontrol WHERE interface = 'orinteg2')

SELECT 'Missing Export Orders',l.* FROM lineitem l
left JOIN stockinfo s ON l.itemno= s.itemno 
WHERE NOT EXISTS 
(SELECT * FROM delivery d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datetrans < @enddate ) 
	  AND  EXISTS 
(SELECT * FROM LineitemAudit d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datechange  BETWEEN @startdate AND @enddate) 
and not  exists 	 -- exclude earlier orders which would have gone through as an update 
(SELECT * FROM LineitemAudit d 
WHERE l.acctno= d.acctno AND l.itemno= d.itemno AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND d.datechange  < @startdate ) 
AND  NOT EXISTS (SELECT * FROM OracleExportHist d WHERE d.acctno= l.acctno 
AND d.itemno= l.itemno AND d.stocklocn =l.stocklocn AND d.agrmtno= l.agrmtno
AND  d.statusflag = 'B' AND d.runno BETWEEN @startrunno AND @endrunno and d.lineamount = l.ordval) 
AND l.itemno !='rb' and not (isnull(s.itemtype,'S') = 'S' and l.quantity =0)
and not (s.itemtype = 'N' and l.ordval=0)

SELECT SUM(lineamount) AS TotalsExported,statusflag FROM  OracleExportHist
WHERE runno BETWEEN @startrunno AND @endrunno 
GROUP BY statusflag 

-- now checking to see whether quantities of stockitems match

SELECT 'Incorrect Delivery quantity' AS IncorrectDeliveryQuantity,* FROM delivery d,StockInfo s 
WHERE  datetrans BETWEEN @startdate AND @enddate
AND EXISTS (SELECT * FROM OracleExportHist l 
WHERE l.acctno= d.acctno AND (l.itemno= d.itemno OR l.itemno= d.retitemno ) -- AND l.stocklocn=  d.stocklocn
	  AND l.agrmtno = d.agrmtno AND l.runno BETWEEN @startrunno AND @endrunno  
	   AND statusflag IN ('R','U','D','P') 
	  AND d.quantity != l.orderedqty AND d.buffno= l.buffno  )
	AND d.itemno !='RB'  and d.itemno=s.itemno and (s.itemtype ='S' or s.category in (12,82) )
GO 