SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODProductImportSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODProductImportSP]
GO

CREATE PROCEDURE DN_EODProductImportSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_EODProductImportSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Product file Import
-- Author       : ??
-- Date         : ??
--
-- This procedure will Import the Product details from FACT2000 or other external source.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/12/10 jec  CR1094 - If NonStock items are maintained in Cosacs - do not import.
-- 30/08/11 ip   RI - #4690 - Error when running Product File Import.
-- 02/02/12 jec #9568 LW74644 - B'dos - Growth of the Live DB
-- 04/05/12 ip  #10085 - LW74963 - The SKU, IUPC and CLASS were not being updated on the
--				 StockInfo table for the items that were being imported.
-- 22/05/12 jec #10153 LW74096 - CoSacs to RI Export -SAR (81) Sparepart flag not set
-- 28/09/12 ip   #10393 - LW74980 - Update StockInfo.VendorEan with the Barcode from the BarCodeItem table.
--				This will be used when searching on barcode in the Cash & Go, Stock Enquiry By Product and Location.
-- 16/01/13 ip  #11134 - LW75362 - Merged from CoSACS 6.5
-- ================================================
	-- Add the parameters for the stored procedure here  
        @return int OUTPUT  
  
AS  
    set nocount on  
    SET  @return = 0   --initialise return code  
 
 DECLARE @affinityipt float  
 DECLARE @hasbarcodes varchar(10)
 Declare @NonStockMaintainable BIT		--CR1094 jec  
  
  
 SELECT @affinityipt = convert(float,value)  
 FROM countrymaintenance  
 WHERE codename = 'affinityipt'  
  
 SELECT @hasbarcodes = value  
 FROM countrymaintenance  
 WHERE codename = 'hasbarcodes'
 -- Maintain NonStocks ??
 select @NonStockMaintainable=value 
 FROM countrymaintenance  
 WHERE codename = 'MaintainNonStocks'
  
 SET @return = @@error  
     
 IF(@return = 0)  
 BEGIN  
     UPDATE temp_prodload  
     SET unitpricehp = ROUND(ISNULL(convert(float,vunitpricehp)/100.00,0),2),  
        unitpricecash = ROUND(ISNULL(convert(float,vunitpricecash)/100.00,0),2),  
        unitpricedutyfree = ROUND(ISNULL(convert(float,vdutyfreeprice)/100.00,0),2),  
        Costprice = ROUND(ISNULL(convert(float,vCostprice)/100.00,0),2),
        taxrate = ROUND(ISNULL(convert(float,vtaxrate)*100.00,0),2)  
   
  SET @return = @@error  
 END  
    
 IF(@return = 0)  
 BEGIN  
  /* FACT sends warrantable info as Y or N. CoSACS uses 1 or 0 */  
     UPDATE temp_prodload  
     SET  warrantable = 1  
     WHERE fwarrantable = 'Y'  
  
  SET @return = @@error  
 END  
  

   
 IF(@return = 0)  
 BEGIN  
     UPDATE temp_prodload  
     SET     prodtype = fprodtype  
  SET @return = @@error  
   

 END  
      
 /* Use the warehouseno to deterMINe stocklocn */  
    /* Use MAX branchno in case warehouse has more than 1 branch */  
      
 IF(@return = 0)  
 BEGIN  
  IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables  
       WHERE  Table_Name = 'temp_branch')  
  BEGIN  
   DROP TABLE temp_branch  
  END      
  
  SET @return = @@error  
 END  
  
 IF(@return = 0)  
 BEGIN  
     SELECT branchno as branchno, branchno as warehouseno  
     INTO    temp_branch  
     FROM    branch  
     
   
     UPDATE temp_prodload  
     SET     branchno = temp_branch.branchno  
     FROM    temp_branch  
     WHERE   temp_prodload.warehouseno = temp_branch.warehouseno  
  
  SET @return = @@error  
 END  
  
 IF(@return = 0)  
 BEGIN  
     /* FR656 - remove all rows with a branchno of zero */  
     /* (they are linked to a non-existant warehouse)   */  
     DELETE   
  FROM temp_prodload   
  WHERE  branchno = 0  
       
  SET @return = @@error  
 END  
  
 IF(@return = 0)  
 BEGIN  
     if not exists (select * from sys.indexes where name = 'ixtemp_prodload')
     CREATE CLUSTERED INDEX ixtemp_prodload on temp_prodload (itemno, branchno)  
  SET @return = @@error  
 END  
  

 
 IF(@return = 0)  
 BEGIN  
     /* Now process the stockitems that were loaded INTO the stockitem table */  
       
     /* Firstly, check for DELETED items. */  
     /* IF item is a kit AND is DELETEd, then DELETE the entry FROM the kitproduct AND */  
     /* the stockitem table. NB This is deleting the definition of the kit AND not the */  
     /* individual stockitems that form the kit.                                       */  
      --Livewire Issue 69293 : Kit product items that have been imported need to remain in the database if they are later deleted from FACT  
         --A soft delete will be implemented instead of the hard delete so that items that have been just imported are seen as active  
                                        
     UPDATE  kitproduct  
        SET deleted = 1  
     WHERE   itemno IN  
       (SELECT itemno  
           FROM   temp_prodload  
           WHERE  itemdescr1 = 'DELETED'  
           AND    itemdescr2 = 'KIT')  
   
  SET @return = @@error  
 END  
  

  
 IF(@return = 0)  
 BEGIN  
     /* Flag those rows linked to at least one lineitem row.            */  
     /* IF they are it means that they shouldn't be physically DELETEd. */  
  UPDATE temp_prodload  
     SET     lirowsexist = 1  
     FROM    lineitem  
     WHERE   temp_prodload.itemno = lineitem.itemno  
     AND     temp_prodload.branchno = lineitem.stocklocn  
   
  SET @return = @@error  
 END  
      
 IF(@return = 0)  
 BEGIN  
     /* Now DELETE DELETED rows that are not associated with any lineitem rows */  
     DELETE   
  FROM stockquantity   
     WHERE   exists 
            (SELECT itemno   
             FROM    temp_prodload  
             WHERE   itemdescr1 = 'DELETED'
             and itemno= stockquantity.itemno and 
             branchno=stockquantity.stocklocn   /* NB DELETED KITS have already been DELETEd above */  
             AND     lirowsexist = 0)  
   
  SET @return = @@error  
 END  
     
 IF(@return = 0)  
 BEGIN  
     /* For any DELETED rows that were not physically DELETEd FROM the stockitem table */  
     /* above, we must reSET the details (price etc) in the stockitem table.           */  
     UPDATE stockinfo 
     SET     suppliercode   = '',  
       itemdescr1     = temp_prodload.itemdescr1,  
       itemdescr2     = '',  
       category       = convert(smallint,temp_prodload.category),  
       supplier       = ISNULL(temp_prodload.supplier,''),  
       prodstatus     = 'D',  
       warrantable    = temp_prodload.warrantable,  
       itemtype       = temp_prodload.prodtype, 
       warrantyrenewalflag        = temp_prodload.warrantyrenewalflag,  
       leadtime       = ISNULL(temp_prodload.leadtime,60),        
       assemblyrequired = temp_prodload.assemblyrequired  ,
       taxrate = temp_prodload.taxrate,
       sku			  = ltrim(rtrim(temp_prodload.itemno)),						--IP - 04/05/12 - #10085 - LW74963
       iupc			  = ltrim(rtrim(temp_prodload.itemno)),						--IP - 04/05/12 - #10085 - LW74963												
       class		  = temp_prodload.class,			
       subclass		  = temp_prodload.subclass				
     
     FROM    temp_prodload  
     WHERE   stockinfo.itemno = temp_prodload.itemno  
     AND     temp_prodload.itemdescr1   = 'DELETED'  
     AND     temp_prodload.lirowsexist  = 1  
     
     update stockprice 
     set   CreditPrice    = 0,  
       CashPrice  = 0
       --taxrate        = temp_prodload.taxrate 
       from temp_prodload
     WHERE   stockprice.itemno = temp_prodload.itemno  
     AND     stockprice.branchno = temp_prodload.branchno  
     AND     temp_prodload.itemdescr1   = 'DELETED'  
     AND     temp_prodload.lirowsexist  = 1 
        
        
                                                                  
  SET @return = @@error  
 END  
  
 IF(@return = 0)  
 BEGIN  
     /******* MARK DELETED ROWS AS PROCESSED **********/  
     UPDATE temp_prodload  
     SET     rowprocessed = 1  
     WHERE   itemdescr1 = 'DELETED';  
   
  SET @return = @@error  
 END  
  
 IF(@return = 0)  
 BEGIN  
     /* UPDATE the stockitem table, AND flag those rows in the temp table that */  
     /* WERE UPDATEd. Any that weren't can then be INSERTed.                   */  
     UPDATE stockinfo 
     SET     suppliercode       = temp_prodload.suppliercode,  
       itemdescr1         = temp_prodload.Itemdescr1,  
       itemdescr2         = temp_prodload.Itemdescr2,  
       category           = convert(smallint,temp_prodload.category),  
       supplier           = ISNULL(temp_prodload.supplier,''),  
       prodstatus         = ISNULL(temp_prodload.prodstatus,''),  
       warrantable        = temp_prodload.Warrantable,  
       itemtype           = ISNULL(temp_prodload.prodtype,''),  
       warrantyrenewalflag = temp_prodload.warrantyrenewalflag,  
       leadtime           = ISNULL(temp_prodload.leadtime,60),  
       assemblyrequired   = temp_prodload.assemblyrequired  ,
       taxrate			  = temp_prodload.taxrate,
       sku				  = ltrim(rtrim(temp_prodload.itemno)),						--IP - 04/05/12 - #10085 - LW74963	
       iupc				  = ltrim(rtrim(temp_prodload.itemno)),						--IP - 04/05/12 - #10085 - LW74963											
       class		  = temp_prodload.class,			
       subclass		  = temp_prodload.subclass	
     FROM    temp_prodload, branch  
     WHERE   stockinfo.itemno = temp_prodload.itemno  
     AND     temp_prodload.rowprocessed  = 0  
   
   --Only update if item has changed	jec 23/12/10
	and(stockinfo.suppliercode       != temp_prodload.suppliercode  
       or stockinfo.itemdescr1         != temp_prodload.Itemdescr1  
       or stockinfo.itemdescr2         != temp_prodload.Itemdescr2  
       or stockinfo.category           != convert(smallint,temp_prodload.category) 
       or stockinfo.supplier           != ISNULL(temp_prodload.supplier,'')  
       or stockinfo.prodstatus         != ISNULL(temp_prodload.prodstatus,'') 
       or stockinfo.warrantable        != temp_prodload.Warrantable  
       or stockinfo.itemtype           != ISNULL(temp_prodload.prodtype,'')  
       or stockinfo.warrantyrenewalflag != temp_prodload.warrantyrenewalflag  
       or stockinfo.leadtime           != ISNULL(temp_prodload.leadtime,60)  
       or stockinfo.assemblyrequired != temp_prodload.assemblyrequired  
       or stockinfo.taxrate != temp_prodload.taxrate
       or stockinfo.class		  != temp_prodload.class			
       or stockinfo.subclass		  != temp_prodload.subclass)
   
     
        UPDATE stockprice 
	SET Creditprice        = CAST(temp_prodload.UnitPriceHP as MONEY),			-- #9568
       CostPrice        = CAST(temp_prodload.CostPrice as MONEY),  
       CashPrice      = CAST(temp_prodload.UnitPriceCash as MONEY),  
       DutyFreePrice  = case when temp_prodload.unitpricedutyfree=0 then CAST(temp_prodload.unitpricecash as MONEY) else CAST(temp_prodload.unitpricedutyfree as MONEY) end
       
       FROM    temp_prodload, branch  
     WHERE   stockprice.itemno = temp_prodload.itemno  
     AND     stockprice.branchno = branch.branchno 
     AND    stockprice.branchno = temp_prodload.warehouseno
     AND     temp_prodload.rowprocessed  = 0  
   
   --Only update if price has changed	jec 23/12/10
   and (stockprice.Creditprice        != CAST(temp_prodload.UnitPriceHP as MONEY)	-- #9568
       or stockprice.CostPrice        != CAST(temp_prodload.CostPrice as MONEY)  
       or stockprice.CashPrice      != CAST(temp_prodload.UnitPriceCash as MONEY)  
       or stockprice.DutyFreePrice  != case when temp_prodload.unitpricedutyfree=0 then CAST(temp_prodload.unitpricecash as MONEY) else CAST(temp_prodload.unitpricedutyfree as MONEY) end)  
 
       IF(@return = 0)  
 BEGIN  
	 UPDATE StockPrice			-- #9568
		SET     dutyfreeprice = case when dutyfreeprice=0 then temp_prodload.unitpricecash else dutyfreeprice end
     FROM    temp_prodload  
     WHERE   StockPrice.itemno = temp_prodload.itemno  
     AND     StockPrice.branchno = temp_prodload.branchno  
    -- AND     StockPrice.DutyFreePrice = 0  
     AND     temp_prodload.rowprocessed = 0
     -- #9568  Only update if price 
     and (stockprice.DutyFreePrice != case when temp_prodload.unitpricedutyfree=0 then CAST(temp_prodload.unitpricecash as MONEY) else CAST(temp_prodload.unitpricedutyfree as MONEY) end
			) 
           
  SET @return = @@error  
 END 
       
    UPDATE stockquantity 
    SET stock = 0 ,  
        deleted = ISNULL(temp_prodload.deleted, '')
    FROM temp_prodload, branch  
    WHERE stockquantity.itemno = temp_prodload.itemno  
        AND stockquantity.stocklocn = branch.branchno  
        AND temp_prodload.rowprocessed  = 0  
   AND  branch.branchno = temp_prodload.warehouseno  

    UPDATE s 
    SET s.stock = 0, 
        s.deleted = 'Y'
    FROM stockquantity s
    WHERE NOT EXISTS (SELECT 'a' FROM temp_prodload WHERE itemno = s.itemno AND branchno = s.stocklocn)

	select branchno, * from temp_prodload
         
    SET @return = @@error  

END  
  
 IF(@return = 0)  
 BEGIN  
     UPDATE temp_prodload  
     SET     rowprocessed = 1  
     FROM    stockitem  
     WHERE   stockitem.itemno = temp_prodload.itemno  
     AND     stockitem.stocklocn = temp_prodload.branchno  
               
  SET @return = @@error  
 END  
  
 IF(@return = 0)  
 BEGIN  
    
    declare @maxid int

    select @maxid = ISNULL(Max(id), 0) from stockinfo
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
         sku,													--IP - 04/05/12 - #10085 - LW74963	
         iupc,													--IP - 04/05/12 - #10085 - LW74963	
         class,													--IP - 04/05/12 - #10085 - LW74963	
         subclass
     )  
     SELECT  
          ROW_NUMBER() over (order by itemno) + @maxid,
          ltrim(rtrim(itemno)),  
          MAX(suppliercode),  
          MAX(itemdescr1),  
          MAX(itemdescr2),
          convert(smallint,MAX(category)), 
          MAX(supplier),  
          MAX(prodstatus),  
          MAX(warrantable),  
          MAX(prodtype),
          MAX(warrantyrenewalflag),  
          ISNULL(MAX(leadtime),999),  
          MAX(assemblyrequired ) ,
          MAX(taxrate ),
          ltrim(rtrim(itemno)),												--IP - 04/05/12 - #10085 - LW74963	
          ltrim(rtrim(itemno)),												--IP - 04/05/12 - #10085 - LW74963	
          max(class),
          max(subclass)	
          
     FROM temp_prodload  
     WHERE   rowprocessed = 0  
     AND     branchno > 0
     AND NOT EXISTS (SELECT * FROM StockInfo s WHERE s.itemno = temp_prodload.itemno )
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
		 ID																			--IP - 30/08/11 
     )  
     --SELECT itemno,  
     SELECT t.itemno,																--IP - 30/08/11 
       branchno,  
          unitpriceHP,  
          unitpricecash,  
         
       unitpricedutyfree,  
         CostPrice,
         refcode,
         si.ID																		--IP - 30/08/11  
     FROM temp_prodload  t INNER JOIN stockinfo si on t.itemno = si.itemno			--IP - 30/08/11
     WHERE   rowprocessed = 0  
     AND     branchno > 0  
     AND NOT EXISTS (SELECT * FROM stockprice s WHERE s.itemno= t.itemno AND s.branchno = t.branchno )
     AND NOT EXISTS (SELECT * FROM stockprice s WHERE s.ID = si.ID AND s.branchno = t.branchno ) --IP - 16/01/13
    
        
     insert into stockquantity
     (itemno, stocklocn, stock, qtyavailable, stockonorder,stockdamage, leadtime,LastOperationSource,deleted,ID )	--IP - 30/08/11   
           -- SELECT itemno,  
            SELECT temp_prodload.itemno,																			--IP - 30/08/11 
       branchno,  
          0,0,0,0,0,'','N', StockInfo.ID																			--IP - 30/08/11
     FROM temp_prodload INNER JOIN StockInfo on temp_prodload.itemno = StockInfo.itemno								--IP - 30/08/11
     WHERE   rowprocessed = 0  
     AND     branchno > 0
     AND NOT EXISTS (SELECT * from stockquantity where stockquantity.itemno = temp_prodload.itemno
					AND stockquantity.stocklocn = temp_prodload.branchno)  
     and not exists (select * from StockQuantity q
	                where q.id = stockinfo.ID and q.stocklocn = temp_prodload.branchno)
  SET @return = @@error  
 END  
  
  --insert item taxrate
  	UPDATE code
	SET codedescript = (SELECT MAX(t.taxrate) 
					   FROM temp_prodload t
					   WHERE t.itemno = p.itemno
					   GROUP BY t.itemno)
	FROM code c
	INNER JOIN temp_prodload p on p.itemno = c.code
	WHERE c.category = 'TXR'

    insert into code (origbr,category,code,codedescript
    ,statusflag,sortorder,reference,additional,Additional2)
    
    select distinct 0, 'TXR', itemno, taxrate
    , 'L', 0, 0, '', null 
    from temp_prodload p
    where not exists (select 1 from code c
    where c.category = 'TXR' and c.code = p.itemno)

     

  -- #10153 Now update Sparepart flag for items in category 20
	UPDATE StockInfo
		set SparePart=1
	Where category=20 and SparePart=0
 
  --What's the point of this code??? - removed 23/12/10 
  --re-instated, set leadtime for kit to max leadtime of components.
 IF(@return = 0)  
 BEGIN  
      update stockinfo   
            set leadtime = isnull(
			(
				select max(c.leadtime)   
				from   stockinfo c,kitproduct k   
                where k.itemno=stockinfo.itemno  
					and k.componentno = c.itemno AND k.deleted = 0
			),leadtime)   
     END																					--IP - 30/08/11    
  
 IF(@return = 0)  
 BEGIN  
     /* UPDATE the barcodeitem table WHERE hasbarcodes flag is SET to true, AND flag those */  
     /* rows in the temp table that were UPDATEd. Any that weren't can then be INSERTed.   */  
     IF(@hasbarcodes = 'True')  
  BEGIN  
   UPDATE  temp_prodload  
         SET     rowprocessed = 0;  
       
   SET @return = @@error  
    
   IF(@return = 0)  
   BEGIN  
       UPDATE temp_prodload  
       SET     rowprocessed = 1  
       WHERE   itemdescr1 = 'DELETED';  
    
    SET @return = @@error  
   END  
    
   IF(@return = 0)  
   BEGIN  
       UPDATE  barcodeitem  
       SET  BarCodeNo = temp_prodload.BarCode  
       FROM    temp_prodload  
       WHERE   barcodeitem.itemno = temp_prodload.itemno  
    AND     LEN(barcode) > 1  
       AND     temp_prodload.rowprocessed  = 0  
              
    SET @return = @@error  
   END  
    
   IF(@return = 0)  
   BEGIN  
       UPDATE temp_prodload  
       SET     rowprocessed = 1  
       FROM    barcodeitem  
       WHERE   barcodeitem.itemno = temp_prodload.itemno  
                 
    SET @return = @@error  
   END  
   
   IF(@return = 0)  
   BEGIN  
       INSERT INTO barcodeitem    
       (  
        itemno,  
           barcodeno  
       )  
       SELECT DISTINCT ltrim(rtrim(itemno)), barcode  
       FROM temp_prodload  
       WHERE   rowprocessed = 0  
    AND  LEN(barcode) > 1;  
    
    SET @return = @@error  
   END 
   
   --IP - 28/09/12 - #10393 - LW74980	
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
	--WarehouseRegion=b.warehouseregion ,
	leadtime = t.leadtime
	FROM SupplierItem s, temp_prodload t
	WHERE  
	t.itemno= s.Itemno
	AND t.supplier = s.Supplier

	INSERT INTO SupplierItem 
	(
	Itemno,	SupplierProductCode,	Supplier,
	Leadtime,	CostPrice,	WarehouseRegion
	) 
	SELECT Itemno,		suppliercode,		supplier,
	leadtime	,		MAX(Costprice),		b.warehouseregion 	
	FROM temp_prodload  t, branch b 
	WHERE t.warehouseno = b.warehouseno
	AND NOT EXISTS (SELECT * FROM SupplierItem s WHERE s.Itemno= t.itemno AND s.Supplier = t.supplier)
	GROUP BY Itemno,suppliercode,supplier, leadtime,b.warehouseregion 	
	

	UPDATE Supplier
	SET suppliername = t.SupplierName
	FROM temp_prodload t 
	WHERE t.supplier = Supplier.supplier 
	AND t.SupplierName !=Supplier.suppliername 
	
	
	INSERT INTO Supplier (Supplier,SupplierName)
	SELECT supplier,MAX(SupplierName) FROM temp_prodload t
	WHERE  NOT EXISTS (SELECT * FROM Supplier s WHERE s.supplier = t.supplier) 
	GROUP BY supplier
    
	if @NonStockMaintainable=1
	BEGIN
		EXEC NonStockUpdateTaxRate
	END

GO 
    
    SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
GO
-- End End End End End End End End End End End End End End End End End End End End End End End End End 
