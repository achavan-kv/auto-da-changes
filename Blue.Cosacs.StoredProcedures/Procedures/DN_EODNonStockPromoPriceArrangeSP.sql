SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODNonStockPromoPriceArrangeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODNonStockPromoPriceArrangeSP]
GO

CREATE PROCEDURE DN_EODNonStockPromoPriceArrangeSP
		@interface varchar(10),
		@runno int
        
AS

	DECLARE	@return int
    SET 	@return = 0			--initialise return code

	DECLARE @branch smallint
	DECLARE @itemno varchar(10)
	DECLARE @stocklocn smallint
	DECLARE @hporcash char(1)
	DECLARE @fromdate datetime
	DECLARE @todate datetime
	DECLARE @unitprice float
	DECLARE	@text varchar(200)
	DECLARE @datetest datetime

	SELECT	@branch = convert(smallint,value)
	FROM	countrymaintenance
	WHERE	codename = 'hobranchno'

    /* Instead of having a temp TABLE with 6 groups of price AND date info, we rearrange    */
    /* the data INTO one TABLE with many rows AND only 1 group of price AND date info.      */
    /* This makes it infinitely easier when populating/amending the promoprice TABLE.       */

    /* Weed out blank rows while we're at it */
    
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'temp_nonstockpromoload')
	BEGIN
		DROP TABLE temp_nonstockpromoload
		SET @return = @@error
	END	

    /* FR 992 */

    SELECT @datetest = CONVERT(datetime,'01/06/00');

	IF(@return = 0)
	BEGIN
	    IF(@datetest = '1-jun-2000')/* dmy format */
		BEGIN
	        UPDATE temp_rawnonstockpromoload SET
	        datefromhp1 = (LEFT(RIGHT(datefromhp1,4),2) + LEFT(datefromhp1,2) + RIGHT(datefromhp1,2)),
	        datetohp1 = (LEFT(RIGHT(datetohp1,4),2) + LEFT(datetohp1,2) + RIGHT(datetohp1,2)) ,
	        datefromhp2 = (LEFT(RIGHT(datefromhp2,4),2) + LEFT(datefromhp2,2) + RIGHT(datefromhp2,2)),
	        datetohp2 = (LEFT(RIGHT(datetohp2,4),2) + LEFT(datetohp2,2) + RIGHT(datetohp2,2)) ,
	        datefromhp3 = (LEFT(RIGHT(datefromhp3,4),2) + LEFT(datefromhp3,2) + RIGHT(datefromhp3,2)),
	        datetohp3 = (LEFT(RIGHT(datetohp3,4),2) + LEFT(datetohp3,2) + RIGHT(datetohp3,2))
		END
	
		SET @return = @@error
    END

	IF(@return = 0)
	BEGIN
	    SELECT	IDENTITY(INT,1,1) AS lineid,
				@branch as origbr,
	            itemno,
	            CONVERT(int,branchno) as stocklocn,
	            'H' as hporcash,
	            CONVERT(datetime,(LEFT(RIGHT(datefromhp1,4),2) ) + '-' + LEFT(datefromhp1,2) + '-' + RIGHT(datefromhp1,2)) as fromdate,
	            CONVERT(datetime,(LEFT(RIGHT(datetohp1,4),2)) + '-' + LEFT(datetohp1,2) + '-' + RIGHT(datetohp1,2)) as todate,
	            CONVERT(float,pricehp1) as unitprice,
	            CONVERT(smallint,(0)) as duprow
	    INTO	temp_nonstockpromoload
	    FROM  	temp_rawnonstockpromoload
	    WHERE 	datefromhp1 != '000000'
	    AND   	datetohp1 != '000000'
	    AND   	CONVERT(float,pricehp1) > 0
	                    
		SET @return = @@error
	END

	/*IF(@return = 0)
	BEGIN
    	ALTER TABLE temp_nonstockpromoload ADD tid integer IDENTITY
    
		SET @return = @@error
	END*/

	IF(@return = 0)
	BEGIN
	    INSERT INTO temp_nonstockpromoload (origbr, itemno, stocklocn, hporcash, fromdate, todate, unitprice, duprow)
	    SELECT  @branch,
	            itemno,
	            CONVERT(int,branchno),
	            'H',
	            CONVERT(datetime,LEFT(RIGHT(datefromhp2,4),2) + '-' + LEFT(datefromhp2,2) + '-' + RIGHT(datefromhp2,2)),
	            CONVERT(datetime,LEFT(RIGHT(datetohp2,4),2) + '-' + LEFT(datetohp2,2) + '-' +RIGHT(datetohp2,2)),
	            CONVERT(float,pricehp2),
	            0
	    FROM    temp_rawnonstockpromoload
	    WHERE   datefromhp2 != '000000'
	    AND     datetohp2 != '000000'
	    AND     CONVERT(float,pricehp2) > 0;       
	
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    INSERT INTO temp_nonstockpromoload (origbr, itemno, stocklocn, hporcash, fromdate, todate, unitprice, duprow)
	    SELECT  @branch,
	            itemno,
	            CONVERT(int,branchno),
	            'H',
	            CONVERT(datetime,LEFT(RIGHT(datefromhp3,4),2) + '-' + LEFT(datefromhp3,2) + '-' + RIGHT(datefromhp3,2)),
	            CONVERT(datetime,LEFT(RIGHT(datetohp3,4),2) + '-' + LEFT(datetohp3,2) + '-' + RIGHT(datetohp3,2)),
	            CONVERT(float,pricehp3),
	            0
	    FROM    temp_rawnonstockpromoload
	    WHERE   datefromhp3 != '000000'
	    AND     datetohp3 != '000000'
	    AND     CONVERT(float,pricehp3) > 0;       
	
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    INSERT INTO temp_nonstockpromoload (origbr, itemno, stocklocn, hporcash, fromdate, todate, unitprice, duprow)
	    SELECT  @branch,
	            itemno,
	            CONVERT(int,branchno),
	            'C',
	            CONVERT(datetime,LEFT(RIGHT(datefromcash1,4),2) + '-' + LEFT(datefromcash1,2) + '-' + RIGHT(datefromcash1,2)),
	            CONVERT(datetime,LEFT(RIGHT(datetocash1,4),2) + '-' + LEFT(datetocash1,2) + '-' + RIGHT(datetocash1,2)),
	            CONVERT(float,pricecash1),
	            0
	    FROM    temp_rawnonstockpromoload
	    WHERE   datefromcash1 != '000000'
	    AND     datetocash1 != '000000'
	    AND     CONVERT(float,pricecash1) > 0;     
	
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    INSERT INTO temp_nonstockpromoload (origbr, itemno, stocklocn, hporcash, fromdate, todate, unitprice, duprow)
	    SELECT  @branch,
	            itemno,
	            CONVERT(int,branchno),
	            'C',
	            CONVERT(datetime,LEFT(RIGHT(datefromcash2,4),2) + '-' + LEFT(datefromcash2,2) + '-' + RIGHT(datefromcash2,2)),
	            CONVERT(datetime,LEFT(RIGHT(datetocash2,4),2) + '-' + LEFT(datetocash2,2) + '-' + RIGHT(datetocash2,2)),
	            CONVERT(float,pricecash2),
	            0
	    FROM    temp_rawnonstockpromoload
	    WHERE   datefromcash2 != '000000'
	    AND     datetocash2 != '000000'
	    AND     CONVERT(float,pricecash2) > 0;     
	
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    INSERT INTO temp_nonstockpromoload (origbr, itemno, stocklocn, hporcash, fromdate, todate, unitprice, duprow)
	    SELECT  @branch,
	            itemno,
	            CONVERT(int,branchno),
	            'C',
	            CONVERT(datetime,LEFT(RIGHT(datefromcash3,4),2) + '-' + LEFT(datefromcash3,2) + '-' + RIGHT(datefromcash3,2)),
	            CONVERT(datetime,LEFT(RIGHT(datetocash3,4),2) + '-' + LEFT(datetocash3,2) + '-' + RIGHT(datetocash3,2)),
	            CONVERT(float,pricecash3),
	            0
	    FROM    temp_rawnonstockpromoload
	    WHERE   datefromcash3 != '000000'
	    AND     datetocash3 != '000000'
	    AND     CONVERT(float,pricecash3) > 0;     
	
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
    	/* Before MODIFYing, check for duplicates */
        UPDATE  temp_nonstockpromoload
        SET     duprow = 1
        WHERE   RTRIM(LTRIM(itemno)) + RTRIM(LTRIM(CONVERT(varchar,stocklocn))) + RTRIM(LTRIM(hporcash)) + RTRIM(LTRIM(CONVERT(varchar,fromdate)))
        IN (
            SELECT RTRIM (LTRIM(t1.itemno)) + RTRIM(LTRIM(CONVERT(varchar,t1.stocklocn))) + RTRIM(LTRIM(t1.hporcash)) +
                                                RTRIM(LTRIM(CONVERT(varchar,t1.fromdate)))
            FROM    temp_nonstockpromoload t1, temp_nonstockpromoload t2
            WHERE   t1.itemno = t2.itemno
            AND     t1.stocklocn = t2.stocklocn
            AND     t1.hporcash = t2.hporcash
            AND     t1.fromdate = t2.fromdate
            AND     t1.lineid != t2.lineid 
           )
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
		DECLARE duplicate_cursor CURSOR FOR 
	    SELECT  itemno, stocklocn, hporcash,
	            fromdate, todate, unitprice
	    FROM    temp_nonstockpromoload
	    WHERE   duprow = 1

		OPEN duplicate_cursor
		
		FETCH NEXT FROM duplicate_cursor 
		INTO	@itemno, @stocklocn,@hporcash,
				@fromdate, @todate, @unitprice
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
	        SET @text = @itemno + ',' + convert(varchar,@stocklocn) + ',' +
	                convert(varchar,@hporcash) + ',' + convert(varchar,@fromdate,106) + ',' + 
					convert(varchar,@todate,106)

	   		INSERT INTO Interfaceerror
			(
				interface, runno,errordate,
				severity,errortext
			)   
			VALUES
			(	
				@interface, @runno, GETDATE(),
				'W', 'Duplicate row: ' + @text
			)   

			FETCH NEXT FROM duplicate_cursor 
			INTO	@itemno, @stocklocn,@hporcash,
					@fromdate, @todate, @unitprice
		END

		CLOSE duplicate_cursor
		DEALLOCATE duplicate_cursor

		SET @return = @@error
	END
	
	IF(@return = 0)
	BEGIN
	    DELETE 
		FROM	temp_nonstockpromoload
	    WHERE 	duprow = 1;
	
		SET @return = @@error
	END
    
	IF(@return = 0)
	BEGIN
		CREATE CLUSTERED INDEX ixtemp_nonstockpromoload ON temp_nonstockpromoload (itemno, stocklocn, hporcash, fromdate)
    
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    /* As a fudge (we're currently on OpenROAD v3.5/02 which is non Y2K compliant) then */
	    /* add 100 years to the dates WHERE the year part is before 50. ie 1.1.01 gets SET  */
	    /* to 1.1.2001                                                                      */
	
	    /* DSR 3/6/2003 Changed for SQL Server */
	    UPDATE	temp_nonstockpromoload
	    SET     fromdate = DATEADD(Year, +100, fromdate)
	    WHERE   fromdate < CONVERT(SMALLDATETIME, '01-01-1950', 105)
	    
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    /* DSR 3/6/2003 Changed for SQL Server */
	    UPDATE  temp_nonstockpromoload
	    SET     todate = DATEADD(Year, +100, todate)
	    WHERE   todate < CONVERT(SMALLDATETIME, '01-01-1950', 105)
	    
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    /* DSR 3/6/2003 Found some dates > 2050 so added reverse */
	    UPDATE  temp_nonstockpromoload
	    SET     fromdate = DATEADD(Year, -100, fromdate)
	    WHERE   fromdate >= CONVERT(SMALLDATETIME, '01-01-2050', 105)
	    
		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
	    /* DSR 3/6/2003 Found some dates > 2050 so added reverse */
	    UPDATE  temp_nonstockpromoload
	    SET     todate = DATEADD(Year, -100, todate)
	    WHERE   todate >= CONVERT(SMALLDATETIME, '01-01-2050', 105)
	    
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
