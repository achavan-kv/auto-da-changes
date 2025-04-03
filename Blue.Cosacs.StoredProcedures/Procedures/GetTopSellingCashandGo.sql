IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'GetTopSellingCashandGo ')
DROP PROCEDURE GetTopSellingCashandGo 
GO 
CREATE PROCEDURE GetTopSellingCashandGo
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : GetTopSellingCashandGo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Top Selling Cash & Go Items
-- Date         : ??
--
-- This procedure will Return the 20 top selling Cash & Go Items.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/05/11 jec CR1212 RI Integration - use ItemID
-- ================================================
	-- Add parameters here
	@branchno SMALLINT , 
	@return INT OUTPUT
	
AS 

SET NOCOUNT ON 
SET @return = 0
DECLARE @currentdate DATETIME
SET @currentdate = GETDATE()
-- here we are getting the top 20 items sold in the cash and go screen. For performance reasons only going to refresh this once a day. 
-- also we are going to only once a day as maybe a bit slow... 

IF NOT EXISTS (SELECT * FROM CashandGoTopSellStock WHERE branchno= @branchno AND daterefresh > DATEADD(DAY,-1,@currentdate) )
BEGIN
	DELETE FROM CashandGoTopSellStock WHERE branchno = @branchno 
	DECLARE @startrunno INT , @endrunno INT , @CGacctno CHAR(12)
	SELECT @endrunno=MAX(runno) FROM interfacecontrol 
	WHERE  interface = 'COS fact'
	-- For performance reasons 
	
	SELECT @cgacctno =a.acctno  FROM custacct c
	join acct a  ON a.acctno= c.acctno 
	WHERE  c.custid = 'PAID & TAKEN'
	AND a.currstatus !='S' AND a.acctno LIKE CONVERT(VARCHAR,@branchno) + '%'

--	SELECT @cgacctno, @endrunno, @startrunno 
	SET @startrunno= @endrunno - 30 		
	
	INSERT INTO CashandGoTopSellStock   (Branchno,itemno, daterefresh, numbersold,itemID )	-- RI 11/05/11
	SELECT TOP 20 @branchno,s.itemno, @currentdate, COUNT(*),s.ID		-- RI 11/05/11
	FROM delivery d, StockInfo s 
	WHERE runno BETWEEN @startrunno AND @endrunno 	
	AND d.acctno = @CGacctno 
	--AND d.itemno= s.itemno 
	AND d.itemID= s.ID		-- RI 11/05/11
	AND s.itemtype = 'S'
	AND d.stocklocn  = @branchno 
	GROUP BY s.itemno,s.ID		-- RI 11/05/11
	order by COUNT(*) DESC 
END
	SELECT c.Branchno,s.IUPC as itemno,		-- RI 11/05/11 c.itemno, 
			c.numbersold,s.itemdescr1, s.itemdescr2, sq.qtyAvailable
	FROM CashandGoTopSellStock c
	JOIN StockInfo  s ON s.ID= c.itemID 
	LEFT JOIN StockQuantity SQ ON sq.ID= s.ID AND sq.stocklocn = c.branchno	-- RI 11/05/11
	WHERE branchno = @branchno 
	order by c.numbersold DESC 
	
	SET @return = @@ERROR
GO 

--EXEC GetTopSellingCashandGo @branchno =720, @return = 0
-- End End End End End End End End End End End End End End End End End End End End End End End End End