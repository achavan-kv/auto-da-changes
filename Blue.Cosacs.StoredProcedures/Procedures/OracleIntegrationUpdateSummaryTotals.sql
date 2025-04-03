-- Procedure to save totals interfaced to Oracle so can do reconciliation from the product interface.
-- 
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'OracleIntegrationUpdateSummaryTotals')
DROP PROCEDURE OracleIntegrationUpdateSummaryTotals
GO 
CREATE PROCEDURE OracleIntegrationUpdateSummaryTotals @runno INT 
AS 

--UAT(5.1) - 706 - NM
--SELECT COUNT(*),statusflag FROM #temp 
--GROUP BY statusflag 

DECLARE @taxtotal MONEY , @nonstocktotal MONEY ,@stocktotal MONEY 
SELECT @stocktotal = ISNULL(SUM(lineamount),0) FROM #temp 
JOIN StockInfo s ON s.itemno= #temp.itemno 
WHERE statusflag IN ('D' ,'R','P')
AND (s.itemtype = 'S' OR s.category IN (12,82) ) 
AND acctno NOT LIKE '___5%' -- exclude cash and go as cannot reconcile as these do not have correct date

SELECT @nonstocktotal = ISNULL(SUM(lineamount),0) FROM #temp 
JOIN StockInfo s ON s.itemno= #temp.itemno 
WHERE statusflag IN ('D' ,'R','P')
AND  (s.itemtype = 'N' AND s.category NOT IN (12,82) )  
--AND acctno NOT LIKE '___5%' --do not exclude cash and go as should be able to check if buffno in the agrmeent .... 
-- TODO ADD tax total.... 

DELETE FROM interfacevalue WHERE runno= @runno AND interface='OrInteg2'

INSERT INTO interfacevalue (
	interface,	runno,	counttype1,
	counttype2,	branchno,	accttype,
	countvalue,	value
) VALUES ( 'OrInteg2', @runno,'StockWarTotal',
'',0,'',
0,@stocktotal)

INSERT INTO interfacevalue (
	interface,	runno,	counttype1,
	counttype2,	branchno,	accttype,
	countvalue,	value
) VALUES ( 'OrInteg2', @runno,'NonStock',
'',0,'',
0,@nonstocktotal)

GO 


