IF OBJECT_ID('[Stock].[StockItemView]', 'V') IS NOT NULL
	DROP VIEW [Stock].[StockItemView]
GO

CREATE VIEW [Stock].[StockItemView]
AS

select a.Id,a.DepartmentCode,a.Department,a.origbr,a.StoreType,a.itemno,a.suppliercode,a.itemdescr1,
a.itemdescr2,a.unitpricehp,a.unitpricecash, a.taxRate,a.category,a.CategoryName,
a.supplier,
    a.prodstatus,
    a.warrantable,
    a.itemtype,
    a.unitpricedutyfree,
    a.refcode,
    a.warrantyrenewalflag,
    a.assemblyrequired,
    a.CostPrice,
    a.suppliername,
    a.DateActivated,
    a.IUPC,
    a.ColourName,
    a.ColourCode,
    a.VendorStyle,
    a.VendorLongStyle,
    a.ItemID,
    a.SKU,
    a.VendorEAN,
    a.RepossessedItem,
    a.VendorWarranty,
    a.SparePart,
    a.Class, 
    a.ClassName,
    a.SubClass,
    a.Brand,
    a.ItemPOSDescr,
   CAST(ISNULL(pr.unitprice, 0) AS money) AS UnitPromoPrice from (
SELECT
    ROW_NUMBER() OVER(ORDER BY i.ID, b.branchno, i.category) AS Id,
    ISNULL(ph.DivisionCode, '') AS DepartmentCode, --Division
    ISNULL(ph.DivisionName, '') AS Department,--Division
    b.branchno AS origbr,  
    b.StoreType,
    i.itemno,  
    i.suppliercode,
    i.itemdescr1,  
    i.itemdescr2, 
    i.unitpricehp, 
    i.unitpricecash,  
    i.taxrate * 100 AS taxRate,
    Convert(smallint, ISNULL(i.category, 0)) AS category, 
    ISNULL(ph.DepartmentName, '') AS CategoryName, --Department
    i.supplier,
    i.prodstatus,
    i.warrantable,
    i.ProdType AS itemtype,
    i.DutyFreePrice AS unitpricedutyfree,
    i.refcode,
    i.warrantyrenewalflag,
    i.assemblyrequired,
    i.CostPrice,
    i.Supplier AS suppliername,
    p.CreatedDate AS DateActivated,
    s.IUPC,
    s.ColourName,
    s.ColourCode,
    s.VendorStyle,
    s.VendorLongStyle,
    s.ID AS ItemID,
    s.SKU,
    i.barcode AS VendorEAN,
    s.RepossessedItem,
    s.VendorWarranty,
    s.SparePart,
    ISNULL(i.Class, '') AS Class, 
    ISNULL(ph.ClassName, '') AS ClassName,
    i.SubClass,
    brand.brandName AS Brand,
    s.ItemPOSDescr,
	i.WarehouseNo
FROM Merchandising.ProductExportView i
inner join stockinfo s 
    ON i.itemno = s.itemno
INNER JOIN Branch b 
    ON b.branchno = i.warehouseno
INNER JOIN merchandising.Product p 
    ON p.Id = i.productId
LEFT JOIN merchandising.Brand brand 
    ON brand.Id = p.BrandId
INNER join merchandising.ProductHierarchySummaryView ph 
    ON i.ProductId = ph.productId 
 ) a LEFT JOIN dbo.promoprice pr 
    ON a.itemno = pr.itemno 
    AND a.warehouseno = pr.origbr 
    AND GETDATE() >= pr.fromdate 
    AND pr.todate > GETDATE()

GO