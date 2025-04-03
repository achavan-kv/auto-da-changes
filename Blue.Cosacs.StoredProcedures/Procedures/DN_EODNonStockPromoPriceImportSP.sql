SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODNonStockPromoPriceImportSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODNonStockPromoPriceImportSP]
GO

CREATE PROCEDURE DN_EODNonStockPromoPriceImportSP
		@interface varchar(10),
		@runno int,
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code

	DECLARE @branch smallint

	SELECT	@branch = convert(smallint,value)
	FROM	countrymaintenance
	WHERE	codename = 'hobranchno'

    /* Use the warehouseno to determine stocklocn */
    /* Use MAX branchno in case warehouse has more than 1 branch */
    
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_branch')
	BEGIN
		DROP TABLE temp_branch
	END	

	SET @return = @@error

	IF(@return = 0)
	BEGIN
	    SELECT	MAX(branchno) as branchno, 
				warehouseno
	    INTO 	temp_branch
	    FROM  	branch
	    GROUP BY warehouseno
	
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	   	UPDATE	temp_rawnonstockpromoload
	   	SET     branchno = temp_branch.branchno
	   	FROM    temp_branch
	   	WHERE   temp_rawnonstockpromoload.warehouseno = temp_branch.warehouseno
	   
		SET @return = @@error
	END

    /* Now copy/INSERT the data to a temporary table based upon promoprice table */
    EXECUTE DN_EODNonStockPromoPriceArrangeSP @interface = @interface, 
									  @runno = @runno 	
        
    /* DSR 2/6/2003 HEAT 127495: Call SProc to remove duplicate or overlapping promo records */
    EXECUTE SP_NonStockPromoPriceLoad

  
    /***************************************************************************/
    /***************** Now add the new promotional details *********************/
    /***************************************************************************/
    
    /* delete any existing row first (shouldn't be any but...) */
    
    /* DSR 3/6/2003 Changed for SQL Server - did still get duplicate key */
	DELETE 
	FROM	PromoPrice
    WHERE 	EXISTS (SELECT ItemNo FROM temp_NonStockPromoLoad tl
    				WHERE  tl.ItemNo    = PromoPrice.ItemNo
    				AND    tl.StockLocn = PromoPrice.StockLocn
    				AND    tl.HPorCash  = PromoPrice.HPorCash
    				AND    tl.FROMDate  = PromoPrice.FROMDate)
                        
	SET @return = @@error

	IF(@return = 0)
	BEGIN
	    INSERT INTO promoprice
	    (
	        origbr, 
	        itemno,
	        stocklocn,
	        hporcash,
	        FROMdate,
	        todate,
	        unitprice,
			itemid
	    )
	    SELECT	@branch,
	        	temp_nonstockpromoload.itemno,
	        	stocklocn,
	        	hporcash,
	        	FROMdate,
	        	todate,
	        	convert(float,unitprice)/100,
				isnull(stockinfo.id, 0)
	    FROM 	temp_nonstockpromoload
		left outer join stockinfo on temp_nonstockpromoload.itemno = stockinfo.itemno;
	            
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off

