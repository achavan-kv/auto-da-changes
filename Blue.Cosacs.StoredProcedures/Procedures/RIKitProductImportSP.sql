if exists (select * from dbo.sysobjects where id = object_id('[dbo].[RIKitProductImportSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RIKitProductImportSP]
GO

CREATE PROCEDURE RIKitProductImportSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIKitProductImportSP.prc  (based on DN_EODKitProductImportSP)
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Kit Product Import
-- Date         : 15 March 2010
--
-- This procedure will import the Kit products from into the KitProduct table.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/08/11	 IP  #4621 - Run the import for regular and repo items
-- ================================================
	-- Add the parameters for the stored procedure here
		@interface varchar(10),
		@runno int,
		@rerun BIT,
		@repo BIT,															--IP - 26/08/11 - RI 
        @return int OUTPUT

AS

    SET 	@return = 0			--initialise return code
    
	IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.tables
			   WHERE  Table_Name = 'RItemp_Kitload')
	BEGIN
		Truncate TABLE RItemp_Kitload
	END	
	
	Insert into RItemp_Kitload (KitItemIUPC, ComponentIUPC, ComponentQty, ComponentPrice, 
				DeletedFlag, ItExists, CpExists, RowProcessed,ItemID,ComponentID)
	SELECT DISTINCT KitItemIUPC, ComponentIUPC,ComponentQty,ComponentPrice,
					DeletedFlag,convert(smallint,0) as itexists,convert(smallint,0),convert(smallint,0),0,0
    FROM  	RItemp_RawKitload
    
    -- remove Raw data
    Truncate TABLE RItemp_RawKitload

	DECLARE @branch smallint
	DECLARE @itemno varchar(10) 
	DECLARE	@componentno varchar(10)

	SELECT	@branch = convert(smallint,value)
	FROM	countrymaintenance
	WHERE	codename = 'hobranchno'

	/*
	** check that the items are on the stock file - if not then kit is not processed
	*/
	UPDATE	RItemp_Kitload
	SET		ItExists = 1,ItemID=stockinfo.ID
	FROM	stockinfo
	WHERE 	RItemp_Kitload.KitItemIUPC = stockinfo.IUPC
	AND		RepossessedItem = @repo											--IP - 26/08/11 - RI - #4621
		

	SET @return = @@ERROR
		
	IF(@return = 0)
	BEGIN
		/*
		** check that the components are on the stock file - if not then kit is not processed
		*/
		UPDATE	RItemp_Kitload
	    SET     CpExists = 1,ComponentID=stockinfo.ID
	    FROM    stockinfo
	    WHERE  	RItemp_Kitload.ComponentIUPC = stockinfo.IUPC
	    AND		RepossessedItem = @repo										--IP - 26/08/11 - RI - #4621
		
		SET @return = @@error
	END	
	
	-- This may not be required??
    IF(@return = 0)
	BEGIN
		DELETE kp
	 	FROM	KitProduct kp INNER JOIN RItemp_Kitload kl on kp.itemID=kl.ItemID and kp.ComponentID=kl.ComponentID 
	 	Where kl.componentqty=0
		--WHERE	LTRIM(RTRIM(itemno)) + LTRIM(RTRIM(componentno)) in
		--(
		--	SELECT	LTRIM(RTRIM(itemno)) + LTRIM(RTRIM(componentno))
		--	FROM	RItemp_Kitload
		--	WHERE	convert(float,componentqty) = 0
		--)
	
		SET @return = @@error
	END
	
		IF(@return = 0)
		BEGIN
			UPDATE	kitproduct
		    SET     origbr = @branch,
		    		--itemno = kl.KitItemIUPC,
		    		--componentno = kl.ComponentIUPC,
		    		itemno = '',
		    		componentno = '',
		    		componentqty = convert(float,kl.componentqty),
                    deleted = case when DeletedFlag='N' then 0 else 1 end,
                    ItemID=s.ID,ComponentID=sc.ID
		    FROM    RItemp_Kitload kl INNER JOIN StockInfo s on kl.ItemID = s.ID
						INNER JOIN StockInfo sc on kl.ComponentID = sc.ID
						INNER JOIN kitproduct kp ON sc.ID = kp.ComponentID 
								AND s.ID = kp.itemID
					--INNER JOIN kitproduct ON kl.ComponentIUPC = Kitproduct.ComponentNo 
					--			AND kl.KitItemIUPC = kitproduct.itemno
		    WHERE 	kp.origbr = @branch
		    --AND     convert(float,kl.componentqty) > 0
		    AND     kl.ItExists = 1
		    AND     kl.CpExists = 1
		
			SET @return = @@error
		END


	IF(@return = 0)
	BEGIN
		/* Flag row to say it has been UPDATEd. Then, INSERT all those not flagged. */
		UPDATE	RItemp_Kitload
	    SET     RowProcessed = 1
	    FROM    kitproduct kp INNER JOIN StockInfo s on kp.ItemID=s.ID 
				INNER JOIN StockInfo sc on kp.ComponentID=sc.ID 
	    --WHERE	kitproduct.itemno = RItemp_Kitload.KitItemIUPC
	    --AND     kitproduct.componentno = RItemp_Kitload.ComponentIUPC
	    WHERE	s.ID = RItemp_Kitload.ItemID
	    AND     sc.ID = RItemp_Kitload.ComponentID			 
	    AND     convert(float,RItemp_Kitload.componentqty) > 0
	    AND     RItemp_Kitload.ItExists = 1
	    AND     RItemp_Kitload.CpExists = 1

		INSERT into KitProduct
		(
				origbr,itemno,componentno,componentqty,deleted,ComponentPrice,ItemID,ComponentID
		)
		SELECT @branch,'','',convert(float,ComponentQty),0,ComponentPrice,s.ID,sc.ID
		FROM	RItemp_Kitload kl INNER JOIN StockInfo s on kl.ItemID = s.ID
						INNER JOIN StockInfo sc on kl.ComponentID = sc.ID 
		WHERE	convert(float,componentqty)	> 0
		AND		RowProcessed 	= 0
		AND		ItExists		= 1
		AND		CpExists		= 1
		
		SET @return = @@error
	END

    --Now set the soft delete for items that are flagged as deleted in the import file
        IF(@return = 0)
		BEGIN
			UPDATE	kitproduct
		    SET     deleted = 1
		   -- WHERE 	kitproduct.itemno NOT IN 
     --       ( SELECT	itemno
			  --FROM	RItemp_Kitload) OR kitproduct.componentno NOT IN ( SELECT	componentno
			  --FROM	RItemp_Kitload WHERE itemno = kitproduct.itemno)
			from kitproduct kp INNER JOIN StockInfo s on kp.ItemID = s.ID
						INNER JOIN StockInfo sc on kp.ComponentID = sc.ID 
						INNER JOIN RItemp_Kitload kl on s.ID=kl.ItemID
								and sc.ID=kl.ComponentID
			Where kl.DeletedFlag='Y'
			SET @return = @@error
		END

	IF(@return = 0)
	BEGIN
		/* Now lets report as an error all those kit products not in stockitem */
		DECLARE kit_cursor CURSOR FOR 
		SELECT	KitItemIUPC,ComponentIUPC
		FROM	RItemp_Kitload
		WHERE	ItExists = 0

		OPEN kit_cursor
		
		FETCH NEXT FROM kit_cursor 
		INTO @itemno, @componentno
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
	   		INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
			VALUES (@interface, @runno, getdate(),'W', 
			@itemno + ',' + @componentno + ' - Kit not found in CoSACS stock.')   

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
		SELECT	KitItemIUPC,ComponentIUPC
		FROM	RItemp_Kitload
		WHERE	CpExists = 0

		OPEN component_cursor
		
		FETCH NEXT FROM component_cursor 
		INTO @itemno, @componentno
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
	   		INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
			VALUES(@interface, @runno, getdate(),'W', 
			@itemno + ',' + @componentno + ' - Kit Component not found in CoSACS stock.')   

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

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End


