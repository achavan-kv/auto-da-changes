SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[RIProductLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RIProductLoadSP]
GO

CREATE PROCEDURE RIProductLoadSP
-- ================================================  
-- Project      : CoSACS .NET  
-- File Name    : RIProductLoadSP.prc  
-- File Type    : MSSQL Server Stored Procedure Script  
-- Title        : RI Interface - Product Data Load  
-- Date         : 16 March 2010  
--  
-- This procedure will load the Product Data from the RI interface file.  
--  
-- Change Control  
-- --------------  
-- Date      By  Description  
-- ----      --  -----------  
-- 28/04/11 jec  Update StockInfo for existing Cosacs ItemNo  
-- 07/06/11 jec  CR1212 #3814 Insert blank itemno into stockinfo for New products (no match on import file with itemno)
-- 27/06/11 jec  #4112 Warranty RefCode/Length not supplied
--               #4116 Tax rate not imported
-- 29/07/11 jec  #4436 fix Warrantable flag & Dummy Stock Quantities.
-- 21/09/11 jec  #8204 Import repossession warranties
-- 07/10/11 ip   #8365 - LW74068 - Warranties were being imported as ItemType 'S'. Now check the category against the 
--				 WAR (Warranty) category. If it exists, set the ItemType to 'N'
-- 21/11/11 jec  #8678 Product Codes showing as Deleted
-- 12/12/11 ip   #8872 RI to CoSACS Interface Import failed. Update of StockQuantity.Deleted to 'N', did not cater where
--				 branch is 'C' or 'N'.
-- 12/12/11 ip	 #8770 LW74355 Warranty Flag issue. Previously incorrectly checking for 'W' from RI. Should be 'Y' (merged from 6.2)
-- 15/12/11 jec  #8678 LW74286 - Product Codes showing as Deleted
-- ================================================  
 -- Add the parameters for the stored procedure here  
  @interface varchar(10),  
  @runno int,  
  @rerun BIT,  
  @repo BIT,  
  @return int OUTPUT  
AS 
  BEGIN   
	  SET  @return = 0   --initialise return code
	  
	  DECLARE @repoText VARCHAR(7) = ''
	  declare @taxrate float
	
	  Select @taxrate= CAST(value as float) from CountryMaintenance where CodeName ='Taxrate'
	  
      IF( @repo = 1 ) 
        SET @repoText = 'REPO - ' 
      
      SELECT TOP 1 * 
      INTO #RawProductLoad 
      FROM  RItemp_RawProductLoad 

      TRUNCATE TABLE #RawProductLoad 

      CREATE CLUSTERED INDEX [ix_RawProductload_itemiupc] 
        ON [dbo].[#RawProductLoad] ([itemiupc]) 

      IF @repo = 0 -- regular stock     
        INSERT INTO #RawProductLoad 
        SELECT * FROM RITemp_RawProductLoad 
      ELSE 
        INSERT INTO #RawProductLoad 
        SELECT * FROM RITemp_RawProductLoadRepo       

      -- Update existing products - Cosacs ItemNo     
      UPDATE StockInfo 
      SET    itemdescr1 = @repoText + Isnull(p.itemdescr1, ''), 
             itemdescr2 = Isnull(p.itemdescr2, ''), 
             category = p.category, 
             supplier = suppliername, 
             prodstatus = p.prodstatus, 
             suppliercode = p.suppliercode, 
             warrantable = CASE 
                             WHEN p.warrantable = 'Y' THEN 1			--IP - 12/12/11 - #8770 - LW74355 
                             --WHEN p.warrantable = 'W' THEN 1 
                             ELSE 0 
                           END, 
             --itemtype = ISNULL(p.itemtype,'N'),
               itemtype = CASE																					--IP - 07/10/11 - RI - #8365 - LW74068
						 WHEN p.category in (select code from code where category = 'WAR') THEN 'N'
						 ELSE ISNULL(p.itemtype,'N')
						END,
             --WarrantyRenewalFlag=,     
             leadtime = ISNULL(p.leadtime,0), 
             assemblyrequired = p.assemblyreq, 
             iupc = p.itemiupc, 
             sku = p.sku, 
             class = p.class, 
             subclass = p.subclass, 
             colourname = p.colourname, 
             colourcode = p.colourcode, 
             qtyboxes = boxes, 
             warrantylength = 0, 
             vendorwarranty = fywperiod, 
             brand = brandname, 
             vendorstyle = p.vendorstyle, 
             vendorlongstyle = p.vendorstylelong, 
             vendorean = p.vendorupc, 
             warrantyrenewalflag = ISNULL(p.warrantyrenewable, 'N'), 
             itemposdescr = p.itemposdescr 
      FROM   StockInfo s 
      INNER JOIN #RawProductLoad p ON s.itemno = p.itemno AND s.repossesseditem = @repo 
      WHERE  Isnull(p.itemno, '') != '' -- jec 17/05/11     
      
      
      -- Update existing products - Non Cosacs ItemNo -- jec 28/04/11      
      UPDATE StockInfo 
      SET    itemdescr1 = @repoText + Isnull(p.itemdescr1, ''), 
             itemdescr2 = Isnull(p.itemdescr2, ''), 
             category = p.category, 
             supplier = suppliername, 
             prodstatus = p.prodstatus, 
             suppliercode = p.suppliercode, 
             warrantable = CASE 
                             WHEN p.warrantable = 'Y' THEN 1 
                             ELSE 0 
                           END, 
             --itemtype = ISNULL(p.itemtype,'N'),--WarrantyRenewalFlag=,     
             itemtype = CASE																					--IP - 07/10/11 - RI - #8365 - LW74068
						 WHEN p.category in (select code from code where category = 'WAR') THEN 'N'
						 ELSE ISNULL(p.itemtype,'N')
						END,
             leadtime = ISNULL(p.leadtime,0), 
             assemblyrequired = p.assemblyreq, 
             iupc = p.itemiupc, 
             sku = p.sku, 
             class = p.class, 
             subclass = p.subclass, 
             colourname = p.colourname, 
             colourcode = p.colourcode, 
             qtyboxes = boxes, 
             warrantylength = 0, 
             vendorwarranty = fywperiod, 
             brand = brandname, 
             vendorstyle = p.vendorstyle, 
             vendorlongstyle = p.vendorstylelong, 
             vendorean = p.vendorupc, 
             warrantyrenewalflag = Isnull(p.warrantyrenewable, 'N'), 
             itemposdescr = p.itemposdescr 
      FROM   StockInfo s 
      INNER JOIN #RawProductLoad p ON s.iupc = p.itemiupc AND repossesseditem = @repo 
      WHERE  Isnull(p.itemno, '') = '' -- jec 17/05/11  
         
      declare @maxid int

        select @maxid = Max(id) from stockinfo
        where id < 100000
          -- Insert new products     
      INSERT INTO StockInfo 
                  (
                   id,
                   itemno, 
                   itemdescr1, 
                   itemdescr2, 
                   category, 
                   supplier, 
                   prodstatus, 
                   suppliercode, 
                   warrantable, 
                   itemtype, 
                   warrantyrenewalflag, 
                   leadtime, 
                   assemblyrequired, 
                   taxrate, 
                   sku, 
                   iupc, 
                   class, 
                   subclass, 
                   colourname, 
                   colourcode, 
                   qtyboxes, 
                   warrantylength, 
                   vendorwarranty, 
                   brand, 
                   vendorstyle, 
                   vendorlongstyle, 
                   vendorean, 
                   repossesseditem, 
                   itemposdescr) 
      SELECT DISTINCT ROW_NUMBER() OVER (order by itemno) + @maxid,
                      Isnull(itemno, ''), 
                      @repoText + Isnull(itemdescr1, ''), 
                      Isnull(itemdescr2, ''), 
                      category, 
                      suppliername, 
                      prodstatus, 
                      suppliercode, 
                      CASE 
                        WHEN warrantable = 'Y' THEN 1				
                        ELSE 0 
                      END, 
                      --ISNULL(itemtype,'N'), 
                      CASE																					--IP - 07/10/11 - RI - #8365 - LW74068
						 WHEN category in (select code from code where category = 'WAR') THEN 'N'
						 ELSE ISNULL(itemtype,'N')
					  END,
                      Isnull(p.warrantyrenewable, 'N'), 
                      ISNULL(p.leadtime,0), 
                      assemblyreq, 
                      0, 
                      sku, 
                      itemiupc, 
                      class, 
                      subclass, 
                      colourname, 
                      colourcode, 
                      boxes, 
                      0, 
                      fywperiod, 
                      brandname, 
                      vendorstyle, 
                      vendorstylelong, 
                      vendorupc, 
                      @repo, 
                      itemposdescr 
      FROM   #RawProductLoad p 
      WHERE  NOT EXISTS (SELECT 1 
                         FROM   StockInfo s 
                         WHERE  s.iupc = p.itemiupc AND repossesseditem = @repo) 
             --AND (@repo = 0 OR p.category NOT IN (SELECT code FROM dbo.code WHERE category = 'WAR'))	-- 21/09/11
             
   
      -- Update existing Prices - specific Branch or All Branches     
      UPDATE StockPrice 
      SET    costprice = l.costprice, 
             cashprice = l.cashprice, 
             creditprice = l.creditprice, 
             dutyfreeprice = l.dutyfreeprice, 
             refcode = l.refcode 
      FROM   StockInfo i 
      INNER JOIN StockPrice p ON i.id = p.id 
      INNER JOIN #RawProductLoad l ON i.iupc = l.itemiupc AND i.RepossessedItem = @repo, 
      branch b 
      WHERE  ( (l.branchno NOT IN ( '000', 'C', 'N' ) AND p.branchno = l.branchno AND p.branchno = b.branchno) 
                OR 
               ( l.branchno = '000' AND p.branchno = b.branchno ) ) 
      
	  
      -- Update existing Prices - specific Store Type only     
      UPDATE StockPrice 
      SET    costprice = l.costprice, 
             cashprice = l.cashprice, 
             creditprice = l.creditprice, 
             dutyfreeprice = l.dutyfreeprice, 
             refcode = l.refcode 
      FROM   StockInfo i 
      INNER JOIN StockPrice p ON i.id = p.id 
      INNER JOIN #RawProductLoad l ON i.iupc = l.itemiupc AND i.RepossessedItem = @repo,  
      branch b 
      WHERE  l.branchno IN ( 'C', 'N' ) AND 
				b.storetype = l.branchno AND 
				p.branchno = b.branchno

      -- Insert New Prices - specific Branch or All Branches     
      INSERT INTO StockPrice 
                  (itemno, 
                   branchno, 
                   creditprice, 
                   cashprice, 
                   dutyfreeprice, 
                   costprice, 
                   refcode, 
                   dateactivated, 
                   id) 
      SELECT DISTINCT i.itemno, 
                      CASE 
                        WHEN l.branchno NOT IN ( '000', 'C', 'N' ) THEN l.branchno 
                        WHEN l.branchno IN ( '000', 'C', 'N' ) THEN b.branchno 
                      END, 
                      l.creditprice, 
                      l.cashprice, 
                      l.dutyfreeprice, 
                      l.costprice, 
                      l.refcode, 
                      NULL, 
                      i.id 
      FROM   StockInfo i 
      INNER JOIN #RawProductLoad l ON i.iupc = l.itemiupc AND i.repossesseditem = @repo, 
      branch b 
      WHERE  (
				( l.branchno NOT IN ( '000', 'C', 'N' ) AND l.branchno = b.branchno ) -- Specific branch     
			    OR 
			    l.branchno = '000' -- All branches
              )
              AND NOT EXISTS(SELECT 1 
                             FROM   StockPrice p
                             WHERE  i.id = p.id AND p.branchno = b.branchno)  

      
      -- Insert New Prices - Specific Store Type only     
      INSERT INTO StockPrice 
                  (itemno, 
                   branchno, 
                   creditprice, 
                   cashprice, 
                   dutyfreeprice, 
                   costprice, 
                   refcode, 
                   dateactivated, 
                   id) 
      SELECT DISTINCT i.itemno,   
                      CASE 
                        WHEN l.branchno NOT IN ( '000', 'C', 'N' ) THEN l.branchno 
                        WHEN l.branchno IN ( '000', 'C', 'N' ) THEN b.branchno 
                      END, 
                      l.creditprice, 
                      l.cashprice, 
                      l.dutyfreeprice, 
                      l.costprice, 
                      l.refcode, 
                      NULL, 
                      i.id 
      FROM   StockInfo i 
      INNER JOIN #RawProductLoad l ON i.iupc = l.itemiupc AND i.repossesseditem = @repo, 
      branch b
      WHERE  l.branchno IN ( 'C', 'N' ) 
			 AND b.storetype = l.branchno 
             AND NOT EXISTS(SELECT 1 
                            FROM StockPrice p 
                            WHERE i.id = p.id AND p.branchno = b.branchno)
                       
      -- Update Taxrate
      UPDATE StockInfo
	  set taxrate=case 
			when t.SpecialRate is not null then t.SpecialRate		-- Special Rate
			when s.itemtype='N' and CAST(s.category as VARCHAR(5)) in(select code from code c where c.category in('PCDIS','FGC','WAR')) then @taxrate -- discount/Warranty/Free Gift
			when s.itemtype='N' and not exists(select * from StockPrice p where s.id=p.id and p.creditprice>0) then 0  -- Non stock & zero price			
			else @taxrate											-- Standard Rate
			end
	  from StockInfo s LEFT outer JOIN TaxItem t on s.id=t.ItemId
	  Where s.Taxrate!=case
			when t.SpecialRate is not null then t.SpecialRate 
			when s.itemtype='N' and CAST(s.category as VARCHAR(5)) in(select code from code c where c.category in('PCDIS','FGC','WAR')) then @taxrate
			when s.itemtype='N' and not exists(select * from StockPrice p where s.id=p.id and p.creditprice>0) then 0			
			else @taxrate
			end

	-- Add Dummy Stock Quantities for all Branches	
	insert into StockQuantity (itemno,stocklocn,qtyAvailable,stock,stockonorder,stockdamage,leadtime,dateupdated,deleted,LastOperationSource,ID)
	SELECT Distinct i.itemno,b.branchno,0,0,0,0,i.leadtime,GETDATE(),'N','',i.ID   
	FROM   #RawProductLoad l INNER JOIN StockInfo i ON i.iupc = l.itemiupc AND i.repossesseditem = @repo, 
			branch b
    WHERE not exists(select * from StockQuantity q where q.id=i.id and q.StockLocn=b.BranchNo) 
    
 --   -- Undelete products that had previously been deleted	#8678
 --   UPDATE StockQuantity
	--	set deleted='N'
	--from #RawProductLoad l INNER JOIN StockInfo i ON i.iupc = l.itemiupc AND i.repossesseditem = @repo
	--						INNER JOIN StockQuantity q on i.ID=q.ID	and l.branchno=q.stocklocn
 --   where l.ProdStatus='L'
 
	--IP - 12/12/11 - #8872 - Update for courts/non courts branches
	UPDATE StockQuantity
	set deleted='N'
	from #RawProductLoad l INNER JOIN StockInfo i ON i.iupc = l.itemiupc AND i.repossesseditem = @repo
							INNER JOIN StockQuantity q on i.ID=q.ID,
							branch b
	where l.branchno in('C', 'N')and b.storetype = l.branchno and b.branchno = q.stocklocn  
    and l.ProdStatus='L'
    -- Set product to deleted		#8678
    UPDATE StockQuantity
	set deleted='Y'
	from #RawProductLoad l INNER JOIN StockInfo i ON i.iupc = l.itemiupc AND i.repossesseditem = @repo
							INNER JOIN StockQuantity q on i.ID=q.ID,
							branch b
	where l.branchno in('C', 'N')and b.storetype = l.branchno and b.branchno = q.stocklocn  
    and l.ProdStatus='E'
    
    --Update for specific branch
    UPDATE StockQuantity
	set deleted='N'
	from #RawProductLoad l INNER JOIN StockInfo i ON i.iupc = l.itemiupc AND i.repossesseditem = @repo
							INNER JOIN StockQuantity q on i.ID=q.ID	
    where l.ProdStatus='L'
    and l.branchno not in ('C', 'N')
    and (l.branchno=q.stocklocn or l.branchno='000')
    
    -- Set product to deleted		#8678
    UPDATE StockQuantity
	set deleted='Y'
	from #RawProductLoad l INNER JOIN StockInfo i ON i.iupc = l.itemiupc AND i.repossesseditem = @repo
							INNER JOIN StockQuantity q on i.ID=q.ID	
    where l.ProdStatus='E'
    and l.branchno not in ('C', 'N')
    and (l.branchno=q.stocklocn or l.branchno='000')
    
    
	-- Warranty 
      IF(@repo = 0)
      BEGIN	
		  SELECT 
				W.ID AS ItemId, 
				ISNULL(L.RefCode,' ') as RefCode,		-- must not be null
				ISNULL(W.VendorWarranty,0) AS WarrantyLength,	-- must not be null
				CONVERT(SMALLINT, ISNULL(MIN(S.VendorWarranty), 0)) AS VendorWarranty
		  INTO #WarrantyImport
		  FROM #RawProductLoad L
		  INNER JOIN StockInfo W ON L.ItemIUPC = W.IUPC AND W.RepossessedItem = @repo	
		  INNER JOIN Code C ON C.Category = 'WAR' AND CONVERT(VARCHAR, W.Category) = C.Code
		  LEFT JOIN StockPrice P ON L.RefCode = P.RefCode
		  LEFT JOIN StockInfo S ON P.ID = S.ID 
                  AND  S.ItemType = 'S' -- S for StockItem
                  AND S.RepossessedItem = 0  -- Regular Item
		  --LEFT JOIN dbo.StockItem S ON L.RefCode = S.RefCode 
                  --AND S.ItemType = 'S'  -- S for StockItem 
                  --AND S.RepossessedItem = 0  -- Regular Item
		  GROUP BY W.ID, L.RefCode, W.VendorWarranty   
	  
	        
		  UPDATE WarrantyBand 
		  SET    minprice = 0,
				 maxprice = 9999999.99,
				 warrantylength = WI.WarrantyLength, 
				 firstYearWarPeriod = WI.VendorWarranty
		  FROM   WarrantyBand WB
		  INNER JOIN #WarrantyImport WI ON WB.ItemID = WI.ItemId AND WB.Refcode = WI.RefCode      
	     

		  INSERT INTO WarrantyBand 
					  (waritemno, 
					   refcode, 
					   minprice, 
					   maxprice, 
					   warrantylength, 
					   firstyearwarperiod, 
					   itemid) 
		  SELECT S.IUPC, 
				 WI.RefCode, 
				 0, 
				 9999999.99, 
				 WI.WarrantyLength, 
				 WI.VendorWarranty, 
				 WI.ItemId 
		  FROM #WarrantyImport WI
		  INNER JOIN StockInfo S ON S.ID = WI.ItemId
		  AND NOT EXISTS(SELECT 1 
						 FROM WarrantyBand WB 
						 WHERE WB.ItemID = WI.ItemId AND WB.Refcode = WI.RefCode)
		  And WI.RefCode !=' '				-- Must have a ref code
		  And WI.WarrantyLength !=0
	 
		  -- Issue Warning - RefCode
		  INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
		  Select @interface, @runno, getdate(),'W', 
				'ABC file: RefCode not supplied for Warranty Item ' + s.IUPC   
		  From #WarrantyImport WI INNER JOIN StockInfo s on WI.ItemId=s.ID and RepossessedItem=@repo
		  Where WI.RefCode=' '
		  -- Issue Warning - WarrantyLength
		  INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
		  Select @interface, @runno, getdate(),'W', 
				'ABC file: WarrantyLength not supplied for Warranty Item ' + s.IUPC   
		  From #WarrantyImport WI INNER JOIN StockInfo s on WI.ItemId=s.ID and RepossessedItem=@repo
		  Where WI.WarrantyLength=0
		  
		  -- Update WarrantyCode table (Also in RIStockQuantityLoadSP)     
		  -- Cost Price may only be entered in the StockQuantityLoad file     
		  UPDATE WarrantyCodes 
		  SET    itemdescr1 = s.itemdescr1, 
				 itemdescr2 = s.itemdescr2, 
				 costprice = p.costprice 
		  FROM   WarrantyCodes w 
		  INNER JOIN StockInfo s ON w.itemid = s.id
		  INNER JOIN dbo.code C ON C.category = 'WAR' AND CONVERT(VARCHAR, s.category) = C.code
		  INNER JOIN StockPrice p ON w.itemid = p.id 
		  INNER JOIN #RawProductLoad l ON l.itemiupc = s.iupc AND s.RepossessedItem = @repo

		  INSERT INTO WarrantyCodes 
					  (warrantyno, 
					   itemdescr1, 
					   itemdescr2, 
					   costprice, 
					   itemid) 
		  SELECT itemiupc, 
				 l.itemdescr1, 
				 l.itemdescr2, 
				 MAX(p.costprice), 
				 s.id 
		  FROM   #RawProductLoad l 
		  INNER JOIN StockInfo s ON itemiupc = s.iupc AND s.RepossessedItem = @repo	
		  INNER JOIN StockPrice p ON s.id = p.id 
		  WHERE CONVERT(VARCHAR, l.category) IN (SELECT code FROM dbo.code WHERE category ='WAR') 
				AND NOT EXISTS(SELECT * 
								FROM   WarrantyCodes w 
								WHERE  s.id = w.itemid) 
		  GROUP  BY itemiupc, 
					l.itemdescr1, 
					l.itemdescr2, 
					s.id 
		END
  END
 Go
  
 -- End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
