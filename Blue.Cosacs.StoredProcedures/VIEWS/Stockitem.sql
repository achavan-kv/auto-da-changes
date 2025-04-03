-- **********************************************************************
-- Title: stockitem.sql
-- Developer: ?
-- Date: ?
-- Purpose: StockItem view

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/09/11  IP  RI - #8104 - UAT40 - Populate stocklastplanneddate as previously 
--				 always displayed null.
-- 19/09/11  IP  RI - #8218 - CR8201 - Agreement printout - description needs to be: descr+brand+vendor style long.
--				 Including Brand in view.
-- **********************************************************************

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[stockitem]'))
DROP VIEW [dbo].[stockitem]
GO

CREATE VIEW [dbo].[stockitem] AS   
SELECT i.ID,	-- RI primarykey jec 13/04/11
	p.branchno AS origbr,  
    i.itemno,    q.stocklocn,    i.suppliercode,    i.itemdescr1,  
    i.itemdescr2,    p.CreditPrice AS unitpricehp,    p.CashPrice AS unitpricecash,  
    i.taxrate,    q.stock,    q.stock AS stockactual,  
    q.stockonorder,    q.stockdamage, min(po.expectedreceiptdate) AS stocklastplanneddate, --NULL AS stocklastplanneddate,  
    q.qtyAvailable AS stockfactavailable,    i.category,    i.supplier,  
    i.prodstatus,    i.warrantable,    i.itemtype,  
    p.DutyFreePrice AS unitpricedutyfree,    P.refcode, --refocde and deleted come from stockinfo  
    i.warrantyrenewalflag,    q.leadtime,  
    i.assemblyrequired,    q.deleted,   
    p.CostPrice,    i.Supplier AS suppliername ,
    p.DateActivated,
    i.IUPC,	-- RI jec 20/04/11 - more columns to add?
    i.ColourName,
    i.ColourCode,		--IP - 27/07/11 - RI
    I.VendorStyle,
    i.VendorLongStyle,
    i.ID as ItemID,
    i.SKu as SKU,
    i.VendorEAN,
    i.RepossessedItem,
    i.VendorWarranty,
    i.SparePart,		-- RI jec 22/06/11
    i.Class,			--IP - 27/07/11 - RI - #4415
    i.SubClass,			--IP - 27/07/11 - RI - #4415
    i.Brand,				--IP - 19/09/11 - RI - #8218 - CR8201 
	i.WarrantyLength,
	i.WarrantyType	--#15639
    
    FROM StockInfo i INNER JOIN StockPrice p ON i.id = p.id 
    INNER JOIN StockQuantity Q ON p.id= q.id AND p.branchno= q.stocklocn
    LEFT JOIN PurchaseOrderOutstanding po on i.id = po.ItemID and po.stocklocn = q.stocklocn
    
    GROUP BY i.ID, p.branchno, i.itemno, q.stocklocn, i.suppliercode, i.itemdescr1, i.itemdescr2, p.CreditPrice, p.CashPrice,
    i.taxrate, q.stock, q.stock, q.stockonorder, q.stockdamage, q.qtyAvailable, i.category, i.supplier, i.prodstatus, i.warrantable, i.itemtype,
    p.DutyFreePrice, P.refcode,i.warrantyrenewalflag, q.leadtime, i.assemblyrequired, q.deleted, p.CostPrice, i.Supplier, p.DateActivated, i.IUPC,
    i.ColourName, i.ColourCode, I.VendorStyle, i.VendorLongStyle,  i.ID, i.SKu, i.VendorEAN,i.RepossessedItem, i.VendorWarranty, i.SparePart,  i.Class,  i.SubClass,  i.Brand, --IP - 19/09/11 - RI - #8218 - CR8201
	i.WarrantyLength,			--#15639	
	i.WarrantyType			--#15639

GO