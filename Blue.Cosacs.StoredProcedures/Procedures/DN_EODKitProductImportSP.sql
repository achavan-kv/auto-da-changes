if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EODKitProductImportSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EODKitProductImportSP]
GO
--exec DN_EODKitProductImportSP 'FACTCOS','2464',0
CREATE PROCEDURE DN_EODKitProductImportSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_EODKitProductImportSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Kit Product Import
-- Author       : ?
-- Date         : ?
--
-- This procedure will import kit products
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/10/11  IP  #8481 - FACT Import EOD Failed with primary key error. Added ItemID, and ComponentID when inserting into KitProduct table.
-- ================================================
		@interface varchar(10),
		@runno int,
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code

	DECLARE @branch smallint
	DECLARE @itemno varchar(10) 
	DECLARE	@componentno varchar(10)					

	SELECT	@branch = convert(smallint,value)
	FROM	countrymaintenance
	WHERE	codename = 'hobranchno'

	/*
	** check that the items are on the stock file - if not then kit is not processed
	*/
	UPDATE	temp_kitload
	SET		itexists = 1
	FROM	stockitem
	WHERE 	temp_kitload.itemno = stockitem.itemno

	SET @return = @@error
	
	IF(@return = 0)
	BEGIN
		/*
		** check that the components are on the stock file - if not then kit is not processed
		*/
		UPDATE	temp_kitload
	    SET     cpexists = 1
	    FROM    stockitem
	    WHERE  	temp_kitload.componentno = stockitem.itemno
		
		SET @return = @@error
	END
	
	--Livewire Issue 68901 : Components are not being removed fromn the database when they have been removed from the import file
	--First remove all items from the kitproduct table which are contained in the import file
    --Livewire Issue 69293 : Kit product items that have been imported need to remain in the database if they are later deleted from FACT
    --A soft delete will be implemented instead of the hard delete so that items that have been just imported are seen as active - JH 12/10/2007


    IF(@return = 0)
	BEGIN
		DELETE
	 	FROM	kitproduct
		WHERE	LTRIM(RTRIM(itemno)) in
		(
			SELECT	LTRIM(RTRIM(itemno))
			FROM	temp_kitload
			WHERE	convert(float,componentqty) = 0
		)
	
		SET @return = @@error
	END
	
		IF(@return = 0)
		BEGIN
			UPDATE	kitproduct
		    SET     origbr = @branch,
		    		itemno = temp_kitload.itemno,
		    		componentno = temp_kitload.componentno,
		    		componentqty = convert(float,temp_kitload.componentqty)/100,
                    deleted = 0,
                    ItemID = si.ID,																								-- 19/10/11  IP  #8481
                    ComponentID = sc.ID																							-- 19/10/11  IP  #8481
		    FROM    temp_kitload INNER JOIN kitproduct ON dbo.temp_kitload.componentno = dbo.kitproduct.componentno AND dbo.temp_kitload.itemno = dbo.kitproduct.itemno
		    INNER JOIN StockInfo si ON temp_kitload.itemno = si.itemno and si.RepossessedItem = 0								-- 19/10/11  IP  #8481
		    INNER JOIN StockInfo sc ON temp_kitload.componentno = sc.itemno and sc.RepossessedItem = 0							-- 19/10/11  IP  #8481
		    WHERE 	kitproduct.origbr = @branch
		    AND     convert(float,temp_kitload.componentqty)/100 > 0
		    AND     temp_kitload.itexists = 1
		    AND     temp_kitload.cpexists = 1
		
			SET @return = @@error
		END


	IF(@return = 0)
	BEGIN
		/* Flag row to say it has been UPDATEd. Then, INSERT all those not flagged. */
				UPDATE	temp_kitload
			    SET     rowprocessed = 1
			    FROM    kitproduct
			    WHERE	kitproduct.itemno = temp_kitload.itemno
			    AND     kitproduct.componentno = temp_kitload.componentno
			    /*AND     kitproduct.origbr = @branch duplicate key on insert - key is itemno and component no so updates should be 
				by the key*/
			    AND     convert(float,temp_kitload.componentqty)/100 > 0
			    AND     temp_kitload.itexists = 1
			    AND     temp_kitload.cpexists = 1

		INSERT into kitproduct
		(
				origbr,
				itemno,
				componentno,
				componentqty,
                deleted,
                ItemID,																			-- 19/10/11  IP  #8481
                ComponentID																		-- 19/10/11  IP  #8481
		)
		SELECT
				@branch,
				temp_kitload.itemno,
				componentno,
				convert(float,componentqty)/100,
                0,
                si.ID,
                sc.ID
		FROM	temp_kitload  INNER JOIN StockInfo si ON temp_kitload.itemno = si.itemno and si.RepossessedItem = 0							-- 19/10/11  IP  #8481
							  INNER JOIN StockInfo sc ON temp_kitload.componentno = sc.itemno and sc.RepossessedItem = 0					-- 19/10/11  IP  #8481
		WHERE	convert(float,componentqty)	> 0
		AND		rowprocessed 	= 0
		AND		itexists		= 1
		AND		cpexists		= 1
		
		SET @return = @@error
	END

    --Now set the soft delete for items that are not present in the import file
        IF(@return = 0)
		BEGIN
			UPDATE	kitproduct
		    SET     deleted = 1
		    --FROM    temp_kitload INNER JOIN kitproduct ON dbo.temp_kitload.componentno = dbo.kitproduct.componentno AND dbo.temp_kitload.itemno = dbo.kitproduct.itemno
		    WHERE 	kitproduct.itemno NOT IN 
            ( SELECT	itemno
			  FROM	temp_kitload) OR kitproduct.componentno NOT IN ( SELECT	componentno
			  FROM	temp_kitload WHERE itemno = kitproduct.itemno)
		
			SET @return = @@error
		END

	IF(@return = 0)
	BEGIN
		/* Now lets report as an error all those kit products not in stockitem */
		DECLARE kit_cursor CURSOR FOR 
		SELECT	itemno,
				componentno
		FROM	temp_kitload
		WHERE	itexists = 0

		OPEN kit_cursor
		
		FETCH NEXT FROM kit_cursor 
		INTO @itemno, @componentno
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
	   		INSERT INTO Interfaceerror
			(
				interface, runno,errordate,
				severity,errortext
			)   
			VALUES
			(	
				@interface, @runno, getdate(),
				'W', @itemno + ',' + @componentno + ' - Kit not found in CoSACS stock.'
			)   

			FETCH NEXT FROM kit_cursor 
			INTO @itemno, @componentno
		END

		CLOSE kit_cursor
		DEALLOCATE kit_cursor

		SET @return = @@error
	END

	IF(@return = 0)
	BEGIN
		/* Now lets report as an error all those kit products not in stockitem */
		DECLARE component_cursor CURSOR FOR 
		SELECT	itemno,
				componentno
		FROM	temp_kitload
		WHERE	cpexists = 0

		OPEN component_cursor
		
		FETCH NEXT FROM component_cursor 
		INTO @itemno, @componentno
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
	   		INSERT INTO Interfaceerror
			(
				interface, runno,errordate,
				severity,errortext
			)   
			VALUES
			(	
				@interface, @runno, getdate(),
				'W', @itemno + ',' + @componentno + ' - Kit Component not found in CoSACS stock.'
			)   

			FETCH NEXT FROM component_cursor 
			INTO @itemno, @componentno
		END

		CLOSE component_cursor
		DEALLOCATE component_cursor

		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS OFF
GO 
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
