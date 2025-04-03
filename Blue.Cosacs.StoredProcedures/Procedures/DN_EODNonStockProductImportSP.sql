
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_EODNonStockProductImportSP]') 
	AND type IN (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_EODNonStockProductImportSP]

GO

CREATE PROCEDURE [dbo].[DN_EODNonStockProductImportSP]
        @return int OUTPUT  
-- ========================================================================
-- Version:		<002> 
-- Change Control
-- --------------
-- Date      By     Description
-- ----      --     -----------
-- 30/07/20  Zensar Optimization changes : Changed Select * to Top 1 'a' in Exists and Non Exists Statement
--										   Unqualified Joins are changed to ANSI Joins.
-- ========================================================================
AS  
	SET NOCOUNT ON  
	SET @return = 0   --initialise return code  
   
	DECLARE @taxrate float  
	DECLARE @affinityipt float  
	DECLARE @hasbarcodes varchar(10)
	Declare @NonStockMaintainable BIT		--CR1094 jec  
  
	SELECT @taxrate = CONVERT(FLOAT,value)  
	FROM countrymaintenance  
	WHERE codename = 'taxrate'  
  
	SELECT @affinityipt = CONVERT(FLOAT,value)  
	FROM countrymaintenance  
	WHERE codename = 'affinityipt'  
  
	SELECT @hasbarcodes = value  
	FROM countrymaintenance  
	WHERE codename = 'hasbarcodes'
	-- Maintain NonStocks ??
	select @NonStockMaintainable=value 
	FROM countrymaintenance  
	WHERE codename = 'MaintainNonStocks'
  
 SET @return = @@ERROR
     
 IF(@return = 0)  
 BEGIN  
     UPDATE temp_nonstockprodload  
     SET     unitpricehp = ROUND(ISNULL(convert(float,vunitpricehp)/100.00,0),2),  
       unitpricecash = ROUND(ISNULL(convert(float,vunitpricecash)/100.00,0),2),  
       unitpricedutyfree = ROUND(ISNULL(convert(float,vdutyfreeprice)/100.00,0),2),  
   Costprice = ROUND(ISNULL(convert(float,vCostprice)/100.00,0),2),
   TaxRate = ROUND(ISNULL(convert(float,VTaxRate),0),2)     
   
  SET @return = @@ERROR
 END  
    
 IF(@return = 0)  
 BEGIN  
  /* FACT sends warrantable info as Y or N. CoSACS uses 1 or 0 */  
     UPDATE temp_nonstockprodload  
     SET  warrantable = 1  
     WHERE fwarrantable = 'Y'  
  
  SET @return = @@ERROR
 END  
   
 IF(@return = 0)  
 BEGIN  
     UPDATE temp_nonstockprodload  
     SET     prodtype = 'S'  
     WHERE   fprodtype in ('03','07') /* FR 897 */  
  SET @return = @@ERROR
   
     UPDATE temp_nonstockprodload  
     SET     prodtype = 'N'  
     WHERE   fprodtype in ('02','06') /* FR 897 */  
  
  SET @return = @@ERROR
 END  
      
 /* Use the warehouseno to deterMINe stocklocn */  
    /* Use MAX branchno in case warehouse has more than 1 branch */  
      
 IF(@return = 0)  
 BEGIN  
  IF EXISTS (SELECT TOP 1 Table_Name FROM INFORMATION_SCHEMA.tables  
       WHERE  Table_Name = 'temp_branch')  
  BEGIN  
   DROP TABLE temp_branch  
  END      
  
  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
     SELECT MAX(branchno) as branchno, warehouseno  
     INTO    temp_branch  
     FROM    branch  
     GROUP BY warehouseno  
   
     UPDATE temp_nonstockprodload  
     SET     branchno = temp_branch.branchno  
     FROM    temp_branch  
     WHERE   temp_nonstockprodload.warehouseno = temp_branch.warehouseno  
  
  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
     /* FR656 - remove all rows with a branchno of zero */  
     /* (they are linked to a non-existant warehouse)   */  
     DELETE   
  FROM temp_nonstockprodload   
  WHERE  branchno = 0  
       
  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
     IF NOT EXISTS (SELECT TOP 1 'a' FROM sys.indexes WHERE name = 'ixtemp_nonstockprodload')
     CREATE CLUSTERED INDEX ixtemp_nonstockprodload on temp_nonstockprodload (itemno, branchno)  
  SET @return = @@ERROR
 END   
  
 IF(@return = 0)  
 BEGIN  
     /* Flag those rows linked to at least one lineitem row.            */  
     /* IF they are it means that they shouldn't be physically DELETEd. */  
  UPDATE temp_nonstockprodload  
     SET     lirowsexist = 1  
     FROM    lineitem  
     WHERE   temp_nonstockprodload.itemno = lineitem.itemno  
     AND     temp_nonstockprodload.branchno = lineitem.stocklocn  
   
  SET @return = @@ERROR
 END  
      
 IF(@return = 0)  
 BEGIN  
     /* Now DELETE DELETED rows that are not associated with any lineitem rows */  
     DELETE   
  FROM stockquantity   
     WHERE   EXISTS 
            (SELECT TOP 1 'a'    
             FROM    temp_nonstockprodload  
             WHERE   itemdescr1 = 'DELETED'
             and itemno= stockquantity.itemno and 
             branchno=stockquantity.stocklocn   /* NB DELETED KITS have already been DELETEd above */  
             AND     lirowsexist = 0)  
   
  SET @return = @@ERROR
 END  
     
 IF(@return = 0)  
 BEGIN  
     /* For any DELETED rows that were not physically DELETEd FROM the stockitem table */  
     /* above, we must reSET the details (price etc) in the stockitem table.           */  
     UPDATE stockinfo 
     SET     suppliercode   = '',  
       itemdescr1     = temp_nonstockprodload.itemdescr1,  
       itemdescr2     = '',  
       category       = convert(smallint, isnull(class.LegacyCode, 0)),  
       supplier       = ISNULL(temp_nonstockprodload.supplier,''),  
       prodstatus     = 'D',  
       warrantable    = temp_nonstockprodload.warrantable,  
       itemtype       = temp_nonstockprodload.prodtype, 
       warrantyrenewalflag        = temp_nonstockprodload.warrantyrenewalflag,  
       leadtime       = ISNULL(temp_nonstockprodload.leadtime,60),        
       assemblyrequired = temp_nonstockprodload.assemblyrequired  ,
       taxrate = temp_nonstockprodload.taxrate,
       sku			  = ltrim(rtrim(temp_nonstockprodload.itemno)),	
       iupc			  = ltrim(rtrim(temp_nonstockprodload.itemno)),	
       class		  = substring(temp_nonstockprodload.itemno, 1, 3)
     FROM    
		temp_nonstockprodload 
		LEFT JOIN Merchandising.HierarchyTag hProdCat 
			on hProdCat.Id = temp_nonstockprodload.category
		LEFT JOIN Merchandising.ClassMapping class
			on hProdCat.Code = class.ClassCode
     WHERE   
		stockinfo.itemno = temp_nonstockprodload.itemno  
		AND temp_nonstockprodload.itemdescr1   = 'DELETED'  
		AND temp_nonstockprodload.lirowsexist  = 1  
     
     update stockprice 
     set   CreditPrice    = 0,  
       CashPrice  = 0
     from temp_nonstockprodload
     WHERE   stockprice.itemno = temp_nonstockprodload.itemno  
     AND     stockprice.branchno = temp_nonstockprodload.branchno  
     AND     temp_nonstockprodload.itemdescr1   = 'DELETED'  
     AND     temp_nonstockprodload.lirowsexist  = 1 

  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
     /******* MARK DELETED ROWS AS PROCESSED **********/  
     UPDATE temp_nonstockprodload  
     SET     rowprocessed = 1  
     WHERE   itemdescr1 = 'DELETED';  
   
  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
     /* UPDATE the stockitem table, AND flag those rows in the temp table that */  
     /* WERE UPDATEd. Any that weren't can then be INSERTed.                   */  
     UPDATE stockinfo 
     SET     suppliercode       = temp_nonstockprodload.suppliercode,  
       itemdescr1         = temp_nonstockprodload.Itemdescr1,  
       itemdescr2         = temp_nonstockprodload.Itemdescr2,  
       category           = convert(smallint, isnull(dmProdCat.LegacyCode, 0)),  
       supplier           = ISNULL(temp_nonstockprodload.supplier,''),  
       prodstatus         = ISNULL(temp_nonstockprodload.prodstatus,''),  
       warrantable        = temp_nonstockprodload.Warrantable,  
       itemtype           = ISNULL(temp_nonstockprodload.prodtype,''),  
       warrantyrenewalflag = temp_nonstockprodload.warrantyrenewalflag,  
       leadtime           = ISNULL(temp_nonstockprodload.leadtime,60),  
       assemblyrequired   = temp_nonstockprodload.assemblyrequired  ,
       taxrate			  = temp_nonstockprodload.taxrate,
       sku				  = ltrim(rtrim(temp_nonstockprodload.itemno)),						--IP - 04/05/12 - #10085 - LW74963	
       iupc				  = ltrim(rtrim(temp_nonstockprodload.itemno)),						--IP - 04/05/12 - #10085 - LW74963											
       class			  = substring(temp_nonstockprodload.itemno,1,3)			--IP - 04/05/12 - #10085 - LW74963	
     FROM    temp_nonstockprodload
     LEFT JOIN 
        Merchandising.HierarchyTag hProdCat on hProdCat.Id = temp_nonstockprodload.category
     LEFT JOIN
        Merchandising.ClassMapping dmProdCat on hProdCat.Code = dmProdCat.ClassCode, branch  
     WHERE   stockinfo.itemno = temp_nonstockprodload.itemno  
     AND     temp_nonstockprodload.rowprocessed  = 0  
   AND  branch.warehouseno = temp_nonstockprodload.warehouseno
   --Only update if item has changed	jec 23/12/10
	and(stockinfo.suppliercode          != temp_nonstockprodload.suppliercode  
       or stockinfo.itemdescr1          != temp_nonstockprodload.Itemdescr1  
       or stockinfo.itemdescr2          != temp_nonstockprodload.Itemdescr2  
       or stockinfo.category            != convert(smallint,temp_nonstockprodload.category) 
       or stockinfo.supplier            != ISNULL(temp_nonstockprodload.supplier,'')  
       or stockinfo.prodstatus          != ISNULL(temp_nonstockprodload.prodstatus,'') 
       or stockinfo.warrantable         != temp_nonstockprodload.Warrantable  
       or stockinfo.itemtype            != ISNULL(temp_nonstockprodload.prodtype,'')  
       or stockinfo.warrantyrenewalflag != temp_nonstockprodload.warrantyrenewalflag  
       or stockinfo.leadtime            != ISNULL(temp_nonstockprodload.leadtime,60)  
       or stockinfo.assemblyrequired    != temp_nonstockprodload.assemblyrequired  
       or stockinfo.taxrate             != temp_nonstockprodload.taxrate)
   
     
    UPDATE stockprice 
	SET Creditprice   = CAST(temp_nonstockprodload.UnitPriceHP as MONEY),
       CostPrice      = CAST(temp_nonstockprodload.CostPrice as MONEY),  
       CashPrice      = CAST(temp_nonstockprodload.UnitPriceCash as MONEY),  
       DutyFreePrice  = case when temp_nonstockprodload.unitpricedutyfree=0 then CAST(temp_nonstockprodload.unitpricecash as MONEY) else CAST(temp_nonstockprodload.unitpricedutyfree as MONEY) end
     FROM    temp_nonstockprodload, branch  
     WHERE   stockprice.itemno = temp_nonstockprodload.itemno  
     AND     stockprice.branchno = branch.branchno  
     AND     temp_nonstockprodload.rowprocessed  = 0  
     AND     branch.warehouseno = temp_nonstockprodload.warehouseno
   and (stockprice.Creditprice      != CAST(temp_nonstockprodload.UnitPriceHP as MONEY)	
       or stockprice.CostPrice      != CAST(temp_nonstockprodload.CostPrice as MONEY)  
       or stockprice.CashPrice      != CAST(temp_nonstockprodload.UnitPriceCash as MONEY)  
       or stockprice.DutyFreePrice  != case when temp_nonstockprodload.unitpricedutyfree=0 then CAST(temp_nonstockprodload.unitpricecash as MONEY) else CAST(temp_nonstockprodload.unitpricedutyfree as MONEY) end)  
 
       IF(@return = 0)  
 BEGIN  
	 UPDATE StockPrice			-- #9568
		SET     dutyfreeprice = case when dutyfreeprice=0 then temp_nonstockprodload.unitpricecash else dutyfreeprice end ,
			refcode = temp_nonstockprodload.refcode
     FROM    temp_nonstockprodload  
     WHERE   StockPrice.itemno = temp_nonstockprodload.itemno  
     AND     StockPrice.branchno = temp_nonstockprodload.branchno  
     AND     temp_nonstockprodload.rowprocessed = 0
     and (stockprice.DutyFreePrice != case when temp_nonstockprodload.unitpricedutyfree=0 then CAST(temp_nonstockprodload.unitpricecash as MONEY) else CAST(temp_nonstockprodload.unitpricedutyfree as MONEY) end
			or stockprice.refcode !=temp_nonstockprodload.refcode
			) 
           
  SET @return = @@ERROR
 END 
       
  UPDATE stockquantity 
     SET    stock              = 0 ,  
            deleted        = ISNULL(temp_nonstockprodload.deleted,'')
     FROM    temp_nonstockprodload
	 INNER JOIN branch  ON branch.warehouseno = temp_nonstockprodload.warehouseno  
     WHERE   stockquantity.itemno = temp_nonstockprodload.itemno  
     AND     stockquantity.stocklocn = branch.branchno  
     AND     temp_nonstockprodload.rowprocessed  = 0  
   --AND  branch.warehouseno = temp_nonstockprodload.warehouseno  
        
      
  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
     UPDATE temp_nonstockprodload  
     SET     rowprocessed = 1  
     FROM    stockitem  
     WHERE   stockitem.itemno = temp_nonstockprodload.itemno  
     AND     stockitem.stocklocn = temp_nonstockprodload.branchno  
               
  SET @return = @@ERROR
 END  
  
 IF(@return = 0)  
 BEGIN  
    
    declare @maxid int

    select @maxid = Max(id) FROM stockinfo
    where id < 100000



     /* Can now INSERT all those which didn't match on a stockitem row */  
     INSERT INTO stockinfo  
     (  
         Id,
         itemno,   
         suppliercode,  
         itemdescr1,  
         itemdescr2, 
         category,
         supplier,  
         prodstatus,  
         warrantable,  
         itemtype,  
         warrantyrenewalflag,  
         leadtime,  
         assemblyrequired,
         taxrate,
         sku,													
         iupc,													
         class													
     )  
     SELECT  
          ROW_NUMBER() over (order by itemno) + @maxid,
          ltrim(rtrim(itemno)),  
          MAX(suppliercode),  
          MAX(itemdescr1),  
          MAX(itemdescr2),
          convert(smallint,MAX(isnull(dmProdCat.LegacyCode,0))), 
          MAX(supplier),  
          MAX(prodstatus),  
          MAX(warrantable),  
          MAX(prodtype),
          MAX(warrantyrenewalflag),  
          ISNULL(MAX(leadtime),999),  
          MAX(assemblyrequired ) ,
          MAX(taxrate ),
          ltrim(rtrim(itemno)),									
          ltrim(rtrim(itemno)),									
          substring(temp_nonstockprodload.itemno,1,3)			
          
     FROM temp_nonstockprodload  
     LEFT JOIN 
        Merchandising.HierarchyTag hProdCat on hProdCat.Id = temp_nonstockprodload.category
     LEFT JOIN
        Merchandising.ClassMapping dmProdCat on hProdCat.Code = dmProdCat.ClassCode

     WHERE   
		rowprocessed = 0  
		AND branchno > 0
		AND NOT EXISTS (SELECT TOP 1 'a' FROM StockInfo s WHERE s.itemno = temp_nonstockprodload.itemno )
     GROUP BY itemno
     
		INSERT INTO stockprice  
		(  
			itemno,  
			branchno,  
			CreditPrice,  
			CashPrice,  
			DutyFreePrice,   
			CostPrice ,
			refcode,
			ID
		)       
		SELECT	itemno,	
				branchno,  
				unitpriceHP,  
				unitpricecash,           
				unitpricedutyfree,  
				CostPrice,
				refcode,
				ID	
		FROM	(	SELECT DISTINCT t.itemno,	
									branchno,  
									unitpriceHP,  
									unitpricecash,         
									unitpricedutyfree,  
									CostPrice,
									refcode,
									si.ID,
									ROW_NUMBER() OVER (PARTITION BY t.itemno, branchno ORDER BY t.itemno) AS RNo
					FROM			temp_nonstockprodload  t 
									INNER JOIN stockinfo si on t.itemno = si.itemno			
					WHERE			rowprocessed = 0  
									AND branchno > 0  
									AND NOT EXISTS (SELECT TOP 1 'a' FROM stockprice s WHERE s.itemno= t.itemno AND s.branchno = t.branchno )
									AND NOT EXISTS (SELECT TOP 1 'a' FROM stockprice s WHERE s.ID = si.ID AND s.branchno = t.branchno )
				) X
		WHERE	X.RNo = 1
        
     INSERT INTO stockquantity
		(itemno, stocklocn, stock, qtyavailable, stockonorder,stockdamage, leadtime,LastOperationSource,deleted,ID )	
     SELECT distinct temp_nonstockprodload.itemno,																	
       branchno,  
          0,0,0,0,0,'','N', StockInfo.ID																			
     FROM temp_nonstockprodload INNER JOIN StockInfo ON temp_nonstockprodload.itemno = StockInfo.itemno				
     WHERE   rowprocessed = 0  
     AND     branchno > 0
     AND NOT EXISTS (SELECT TOP 1 'a' FROM stockquantity WHERE stockquantity.itemno = temp_nonstockprodload.itemno
					AND stockquantity.stocklocn = temp_nonstockprodload.branchno)  
     AND NOT EXISTS (SELECT TOP 1 'a' FROM StockQuantity q
	                WHERE q.id = stockinfo.ID AND q.stocklocn = temp_nonstockprodload.branchno)
  SET @return = @@ERROR
 END  
  
  -- #10153 Now update Sparepart flag for items in category 20
	UPDATE StockInfo
		set SparePart=1
	Where category=20 and SparePart=0
  
 truncate table warrantycodes  
    insert into warrantycodes  
    (warrantyno,  
     itemdescr1,  
     itemdescr2,  
     costprice,
      ItemID)																								
 select   
	itemno ,  
	 max(itemdescr1),  
	 max(itemdescr2) ,  
	 max(costprice),
	 ItemID																										
 from stockitem where category in (select distinct code from code where category = 'WAR')  
 group by itemno, ItemID																				
  
 IF(@return = 0)  
 BEGIN  
     /* UPDATE the barcodeitem table WHERE hasbarcodes flag is SET to true, AND flag those */  
     /* rows in the temp table that were UPDATEd. Any that weren't can then be INSERTed.   */  
     IF(@hasbarcodes = 'True')  
  BEGIN  
   UPDATE  temp_nonstockprodload  
         SET     rowprocessed = 0;  
       
   SET @return = @@ERROR
    
   IF(@return = 0)  
   BEGIN  
       UPDATE temp_nonstockprodload  
       SET     rowprocessed = 1  
       WHERE   itemdescr1 = 'DELETED';  
    
    SET @return = @@ERROR
   END  
    
   IF(@return = 0)  
   BEGIN  
       UPDATE  barcodeitem  
       SET  BarCodeNo = temp_nonstockprodload.BarCode  
       FROM    temp_nonstockprodload  
       WHERE   barcodeitem.itemno = temp_nonstockprodload.itemno  
    AND     LEN(barcode) > 1  
       AND     temp_nonstockprodload.rowprocessed  = 0  
              
    SET @return = @@ERROR
   END  
    
   IF(@return = 0)  
   BEGIN  
       UPDATE temp_nonstockprodload  
       SET     rowprocessed = 1  
       FROM    barcodeitem  
       WHERE   barcodeitem.itemno = temp_nonstockprodload.itemno  
                 
    SET @return = @@ERROR
   END  
   
   IF(@return = 0)  
   BEGIN  
       INSERT INTO barcodeitem    
		(itemno, barcodeno)  
       SELECT DISTINCT ltrim(rtrim(itemno)), barcode  
       FROM temp_nonstockprodload  
       WHERE   rowprocessed = 0  
    AND  LEN(barcode) > 1;  
    
    SET @return = @@ERROR
   END 
   
   IF(@return = 0)
   BEGIN
		UPDATE stockinfo
		SET VendorEAN = dbo.BarCodeItem.barcodeno
		FROM dbo.BarCodeItem 
		WHERE stockinfo.itemno = dbo.BarCodeItem.itemno
   END
  END  
    END 
    
    
	UPDATE s 
	SET CostPrice =t.costprice,
	SupplierProductCode = t.suppliercode,
	Supplier = t.supplier,
	leadtime = t.leadtime
	FROM SupplierItem s
	INNER JOIN temp_nonstockprodload t ON t.itemno= s.Itemno
		AND t.supplier = s.Supplier
	--WHERE  
	--t.itemno= s.Itemno
	--AND t.supplier = s.Supplier

	INSERT INTO SupplierItem 
	(
	Itemno,	SupplierProductCode,	Supplier,
	Leadtime,	CostPrice,	WarehouseRegion
	) 
	SELECT Itemno,		suppliercode,		supplier,
	leadtime	,		MAX(Costprice),		b.warehouseregion 	
	FROM temp_nonstockprodload  t
	INNER JOIN branch b ON t.warehouseno = b.warehouseno
	WHERE --t.warehouseno = b.warehouseno AND 
	NOT EXISTS (SELECT TOP 1 'a' FROM SupplierItem s WHERE s.Itemno= t.itemno AND s.Supplier = t.supplier)
	GROUP BY Itemno,suppliercode,supplier, leadtime,b.warehouseregion 	

	UPDATE Supplier
	SET suppliername = t.SupplierName
	FROM temp_nonstockprodload t 
	WHERE t.supplier = Supplier.supplier 
	AND t.SupplierName !=Supplier.suppliername 
	
	INSERT INTO Supplier (Supplier,SupplierName)
	SELECT supplier,MAX(SupplierName) FROM temp_nonstockprodload t
	WHERE  NOT EXISTS (SELECT TOP 1 'a' FROM Supplier s WHERE s.supplier = t.supplier) 
	GROUP BY supplier

GO


