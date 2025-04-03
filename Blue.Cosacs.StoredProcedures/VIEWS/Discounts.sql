
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[Discounts]'))
DROP VIEW [dbo].[Discounts]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Discounts]
AS SELECT
     d.AcctNo,
     d.AgrmtNo,
     --d.DiscountItemNo,
     S1.IUPC as DiscountItemNo,		-- RI jec 10/05/11
     s1.ItemDescr1 + ' - ' + s1.ItemDescr2 AS DiscountDescription,
     --ISNULL(d.ParentItemNo,'') AS ParentItemNo,
     ISNULL(s2.IUPC,'') AS ParentItemNo,	-- RI jec 10/05/11
     ISNULL(s2.ItemDescr1 + ' - ' + s2.ItemDescr2,'') AS ParentDescription,
     d.StockLocn,
     d.Amount,
     d.SalesPerson,
     u1.FirstName + ' ' + U1.LastName AS SalesPersonName,
     d.DateAuthorised,
     d.AuthorisedBy,
     u2.FirstName + ' ' + U2.LastName  AS AuthorisedByName
--FROM StockItem s1,
FROM StockInfo s1 
INNER JOIN StockQuantity sq on s1.ID=sq.id,DiscountsAuthorised d
LEFT OUTER JOIN StockInfo s2 ON    s2.ID = d.ParentItemID		-- RI jec 10/05/11
LEFT OUTER JOIN StockQuantity sq2 on d.ParentItemID=sq2.id AND sq2.StockLocn = d.StockLocn -- RI jec 10/05/11
INNER JOIN admin.[User] u1 ON u1.Id = d.SalesPerson
INNER JOIN Admin.[User] u2 ON u2.Id = d.AuthorisedBy
WHERE s1.ID = d.ItemID			-- RI jec 10/05/11
GO



