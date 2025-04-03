IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'ScoreExtractUpdateProductCategories')
DROP PROCEDURE ScoreExtractUpdateProductCategories
GO 
CREATE PROCEDURE ScoreExtractUpdateProductCategories 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : ScoreExtractUpdateProductCategories.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : ScoreExtractUpdateProductCategories
-- Author       : Alex Ayscough
-- Date         : 08 December 2010
--
-- This procedure will update product categories for Delinquency data
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/07/11  jec CR1212 RI Integration
-- ================================================
	-- Add the parameters for the stored procedure here
AS 
SET NOCOUNT ON 
CREATE TABLE #productcategories 
(acctno CHAR(12) NOT NULL PRIMARY KEY , pc1 VARCHAR(4), pc2 VARCHAR(4), pc3 VARCHAR(4),
it1 VARCHAR(18),it2 VARCHAR(18),it3 VARCHAR(18))		-- RI

INSERT INTO #productcategories (
	acctno)
SELECT acctno FROM delinquency 
GROUP BY acctno 

UPDATE #productcategories
SET it1 = 
ISNULL(
( SELECT MAX(s.IUPC)					-- RI MAX(l.itemno)
FROM lineitem l, stockinfo s
WHERE  s.ID = l.ItemId AND			 -- RI s.itemno = l.itemno AND 
l.acctno= #productcategories.acctno
AND l.ordval = (SELECT MAX(la.ordval) FROM lineitem la, StockInfo sa		-- RI
	WHERE l.acctno= la.acctno AND l.agrmtno= la.agrmtno  and la.ItemId=sa.ID AND sa.IUPC !='DT')		-- RI
),'')

UPDATE p
SET pc1 = s.category 
FROM #productcategories p, stockinfo s 
WHERE p.it1= s.IUPC			-- RI s.itemno 

UPDATE #productcategories
SET it2 = 
ISNULL(
( SELECT MAX(s.IUPC)					-- RI MAX(l.itemno)
FROM lineitem l, stockinfo s
WHERE  s.ID = l.ItemId AND			 -- RI s.itemno = l.itemno AND 
l.acctno= #productcategories.acctno
AND l.ordval = (SELECT MAX(la.ordval) FROM lineitem la, StockInfo sa		-- RI 
	WHERE l.acctno= la.acctno AND l.agrmtno= la.agrmtno and la.ItemId=sa.ID AND sa.IUPC !='DT'			-- RI
	--AND la.itemno != #productcategories.it1 )
	AND sa.IUPC != #productcategories.it1 )			-- RI
),'')

UPDATE p
SET pc2 = s.category 
FROM #productcategories p, stockinfo s 
WHERE p.it2= s.IUPC			-- RI s.itemno 

UPDATE #productcategories
SET it3 = 
ISNULL(
( SELECT MAX(s.IUPC)					-- RI MAX(l.itemno)
FROM lineitem l, stockinfo s
WHERE  s.ID = l.ItemId AND			 -- RI s.itemno = l.itemno AND
l.acctno= #productcategories.acctno
AND l.ordval = (SELECT MAX(la.ordval) FROM lineitem la, StockInfo sa		-- RI 
	WHERE l.acctno= la.acctno AND l.agrmtno= la.agrmtno and la.ItemId=sa.ID AND sa.IUPC !='DT'			-- RI
	--AND la.itemno NOT IN  (#productcategories.it1,#productcategories.it2)  )
	AND sa.IUPC NOT IN  (#productcategories.it1,#productcategories.it2)  )			-- RI
),'')

UPDATE p
SET pc3 = s.category 
FROM #productcategories p, stockinfo s 
WHERE p.it3= s.IUPC			-- RI s.itemno  


UPDATE Delinquency SET Productcategory1 = pc1 ,
Productcategory2 = pc2,
Productcategory3 =pc3 
FROM #productcategories p 
WHERE p.acctno = Delinquency.acctno

DROP TABLE #productcategories
GO 

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End
