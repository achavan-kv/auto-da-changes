
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryIRSearchSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryIRSearchSP]
GO


CREATE procedure DN_DeliveryIRSearchSP
-- ================================================      
-- Project      : CoSACS .NET      
-- File Name    : DN_DeliveryIRSearchSP.sql      
-- File Type    : MSSQL Server Stored Procedure Script      
-- Title        :   
-- Author       : ??      
-- Date         : ??        
--       
-- Change Control      
-- --------------      
-- Date      By  Description      
-- ----      --  -----------      
-- 28/07/11  IP  RI - #4429 - RI System Integration
-- 01/08/11  IP  RI - #4445 - Return the IUPC and Courts Code for the item and warranty. The Courts Code being displayed in the warranty reporting screen
--				 is dependent upon a Country Parameter.
-- =================================================================================  
		    @acctno varchar (12),
		    @custid varchar (20),
			@buffno int,
		    @datefrom datetime,
		    @dateto datetime,
			@type char(1),
		    @return int out
AS

	SET ROWCOUNT 250

	SET @return = 0
	
	SET @datefrom = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @datefrom, 105), 105)
	SET @dateto = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @dateto, 105), 105)

    
	CREATE TABLE	#delivery
	(
		acctno      varchar(12),
		agrmtno     int,
		--itemno      varchar(8),
		itemno      varchar(18),						--IP - 28/07/11 - RI - #4429
		stocklocn   smallint,
		quantity    float,
		stockprice  money,
		category    varchar(10),
		contractno  varchar(10),
		datetrans   DATETIME,
		ItemID		integer								--IP - 28/07/11 - RI - #4429 
	)
	
	IF (@type = 'S')
	BEGIN
		INSERT 
		INTO	#delivery
		SELECT 	d.acctno, 
				d.agrmtno, 
				--d.itemno, 
				isnull(s.iupc,'') as itemno,		   --IP - 28/07/11 - RI - #4429
				d.stocklocn, 
				d.quantity,
				s.unitpricecash,
				s.category,
				d.contractno,
				d.datetrans, 
				d.ItemID								--IP - 28/07/11 - RI - #4429
		FROM	delivery d, stockitem s 
		--WHERE	d.itemno = s.itemno
		WHERE	d.ItemID = s.ID							--IP - 28/07/11 - RI - #4429
		AND		d.stocklocn = s.stocklocn
		AND		(d.agrmtno = @buffno OR @buffno = 0)
		AND 	CONVERT(DATETIME, CONVERT(VARCHAR(10), d.datetrans, 105), 105) BETWEEN @datefrom AND @dateto
		AND		d.agrmtno != 1
		AND		s.refcode = 'ZZ'

		SET @return =@@error        
	END
	ELSE 
	BEGIN
		INSERT 
		INTO	#delivery
		SELECT 	d.acctno, 
				d.agrmtno, 
				--d.itemno,				
				isnull(s.iupc,'') as itemno,			--IP - 28/07/11 - RI - #4429	
				d.stocklocn, 
				d.quantity,
				s.unitpricecash,
				s.category,
				d.contractno,
				d.datetrans,
				d.ItemID								--IP - 28/07/11 - RI - #4429	
		FROM	delivery d, acct a, custacct c, stockitem s 
		WHERE	a.acctno like @acctno
		AND		c.custid like @custid
		AND     a.accttype = @type
		AND		a.acctno = c.acctno
		AND		c.hldorjnt = 'H'
		AND		d.acctno = a.acctno	
		--AND		d.itemno = s.itemno
		AND		d.ItemID = s.ID							--IP - 28/07/11 - RI - #4429
		AND		d.stocklocn = s.stocklocn
		--AND 	CONVERT(DATETIME, CONVERT(VARCHAR(10), d.datetrans, 105), 105) BETWEEN @datefrom AND @dateto			-- #17290
		AND		s.refcode = 'ZZ'
	END

	SELECT 	acctno, 
			agrmtno, 
			itemno, 
			stocklocn, 
			sum(quantity) as quantity,
			ItemID												--IP - 28/07/11 - RI - #4429
	INTO	#delaccts
	FROM	#delivery 
	GROUP BY acctno, agrmtno, itemno, stocklocn,ItemID			--IP - 28/07/11 - RI - #4429
	HAVING sum(quantity) = 0

	DELETE
	FROM	#delivery
	WHERE EXISTS( SELECT 1
				  FROM   #delaccts d
				  WHERE  #delivery.acctno = d.acctno
			      AND    #delivery.agrmtno = d.agrmtno
			      --AND    #delivery.itemno = d.itemno
			      AND    #delivery.ItemID = d.ItemID			--IP - 28/07/11 - RI - #4429
			      AND    #delivery.stocklocn = d.stocklocn)
					
		--CREATE CLUSTERED INDEX ix_cangdelivery ON #delivery (acctno, agrmtno, itemno,stocklocn)
	CREATE CLUSTERED INDEX ix_cangdelivery ON #delivery (acctno, agrmtno, ItemID,stocklocn)			--IP - 28/07/11 - RI - #4429
		
	SELECT 	DISTINCT
			d.acctno,
			isnull(s.itemno,'') as 'CourtsCode',			--IP - 01/08/11 - RI - #4445
			d.itemno,
			s.itemdescr1 + ' ' + s.itemdescr2 as itemdescr1,
			d.stocklocn,
			l2.quantity as quantity,
			l.price,
			l.price as transvalue,
			d.agrmtno as buffno,
			--l2.itemno as warrantyno,
			isnull(sw.itemno,'') as 'WarrantyCourtsCode',	--IP - 01/08/11 - RI - #4445	
			isnull(sw.iupc,'') as warrantyno,		--IP - 28/07/11 - RI - #4429
			d.datetrans,
			s.taxrate,
			l2.contractno,
			d.category,
			convert(money, 0) as discount,
			a.empeenosale,
			c.custid,
			d.ItemID,								--IP - 28/07/11 - RI - #4429
			l2.ItemID as WarrantyID					--IP - 28/07/11 - RI - #4429
	INTO	#temp
	FROM 	#delivery d 
			INNER JOIN lineitem l ON   l.acctno = d.acctno 
								  AND  l.agrmtno = d.agrmtno 
								  --AND  l.itemno = d.itemno 
								  AND  l.ItemID = d.ItemID				--IP - 28/07/11 - RI - #4429 
								  AND  l.stocklocn = d.stocklocn
			INNER JOIN agreement a ON  d.acctno = a.acctno
								   AND d.agrmtno = a.agrmtno
			INNER JOIN custacct c ON   d.acctno = c.acctno
								   AND c.hldorjnt = 'H'
			INNER JOIN lineitem l2 ON  l.acctno = l2.acctno
								   AND l.agrmtno = l2.agrmtno
								   AND l.stocklocn = l2.parentlocation
								   --AND l.itemno = l2.parentitemno
								   AND l.ItemID = l2.ParentItemID		--IP - 28/07/11 - RI - #4429
								   AND l2.contractno != '' 
			LEFT JOIN stockinfo sw ON l2.ItemID = sw.ID					--IP - 28/07/11 - RI - #4429	-- get the iupc of the warranty	
			--INNER JOIN stockitem s ON  l.itemno = s.itemno
			INNER JOIN stockitem s ON  l.ItemID = s.ID					--IP - 28/07/11 - RI - #4429
								   AND l.stocklocn = s.stocklocn
			--INNER JOIN warrantyband w ON  w.waritemno = l2.itemno
	------		INNER JOIN warrantyband w ON  w.ItemID = l2.ItemID			--IP - 28/07/11 - RI - #4429
	------WHERE   DATEADD(YEAR, w.warrantylength, d.datetrans) > GETDATE()
			inner join warranty.warrantySale ws on ws.CustomerAccount=@acctno and ws.ItemId=d.itemid and ws.WarrantyContractNo=l2.contractno		-- #17290
			where dateadd(month,ws.WarrantyLength,ws.EffectiveDate)>=getdate() and ws.EffectiveDate<=getdate() and ws.Status='Active'

	UPDATE	#temp
	SET		discount = ISNULL(abs(l.ordval), 0)
	FROM	lineitem l, stockitem s
	WHERE	#temp.acctno = l.acctno
	AND		#temp.buffno = l.agrmtno
	AND		#temp.itemid = l.parentitemid
	--AND		l.itemno = s.itemno
	AND		l.ItemID = s.ID					--IP - 28/07/11 - RI - #4429
	AND 	l.stocklocn = s.stocklocn
	--AND		s.category IN(36, 37, 38, 46, 47, 48, 86, 87, 88)
	AND		s.category in (select code from code where category = 'PCDIS') --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties

	SELECT	DISTINCT
			MAX(datetrans) as datetrans,
	        acctno,
	        custid,
			warrantyno,
			WarrantyCourtsCode as 'Warranty Courts Code',	--IP - 01/08/11 - RI - #4445
			contractno,
			itemno,
			CourtsCode as 'Courts Code',					--IP - 01/08/11 - RI - #4445
			itemdescr1,
			buffno,
			stocklocn,
			quantity,
			price, 
			transvalue - discount as transvalue,
			discount,
			taxrate, 
			empeenosale,
			ItemID,									--IP - 28/07/11 - RI - #4429
			WarrantyID								--IP - 28/07/11 - RI - #4429
	FROM	#temp
    GROUP BY acctno, warrantyno, WarrantyCourtsCode,contractno, itemno,CourtsCode, itemdescr1, buffno, stocklocn, quantity, price,	--IP - 01/08/11 - RI - #4445		
             transvalue, discount, taxrate, empeenosale, custid,ItemID, WarrantyID						--IP - 28/07/11

		
	SET ROWCOUNT 0

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



