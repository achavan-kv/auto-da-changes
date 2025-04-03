IF OBJECT_ID('dbo.DN_EODStockQtyImportSP') IS NOT NULL
	DROP PROCEDURE dbo.DN_EODStockQtyImportSP
GO

CREATE PROCEDURE DN_EODStockQtyImportSP
	@interface varchar(10),
	@runno int,
    @return	int OUTPUT
AS
    SET @return = 0
	/* Overwrite the stock attribute in the stockitem table with the value read in FROM	*/
	/* the FACT stock quantity file.													*/

	/* Use the warehouseno to determine stocklocn */
	/* Use max branchno in case warehouse has more than 1 branch */

	UPDATE temp_stockload
	SET	branchno = temp_branch.branchno
	FROM	
	(
		SELECT MAX(branchno) as branchno, warehouseno
		FROM branch
		GROUP BY warehouseno
	) AS temp_branch
	WHERE temp_stockload.warehouseno = temp_branch.warehouseno

	UPDATE temp_stockload
	SET	planneddate = CONVERT(datetime,
						(LEFT(RIGHT(stocklastplanneddate, 6), 2) + '-' 
					  	+ LEFT(stocklastplanneddate, 2) + '-' 
					  	+ RIGHT(stocklastplanneddate, 2))
					  ,10)
	WHERE stocklastplanneddate != '00000000'

	UPDATE stockquantity
	SET qtyavailable = ISNULL(CONVERT(float, temp_stockload.stockfactavailable / 100), 0),
	    stock = CONVERT(float, temp_stockload.stockactual / 100),
	    stockonorder = CONVERT(float, temp_stockload.stockonorder / 100),
	    stockdamage = CONVERT(float, temp_stockload.stockdamage / 100),
	    dateupdated = temp_stockload.planneddate
	FROM temp_stockload 
	INNER JOIN temp_prodload p 
		ON LTRIM(RTRIM(temp_stockload.itemno)) = LTRIM(RTRIM(p.itemno))
	WHERE 
		LTRIM(RTRIM(stockquantity.itemno)) = LTRIM(RTRIM(temp_stockload.itemno))
		AND	stockquantity.stocklocn = temp_stockload.warehouseno
		AND 
		(
			stockquantity.qtyavailable != CONVERT(float, temp_stockload.stockfactavailable / 100) 
			OR stockquantity.stock != CONVERT(float, temp_stockload.stockactual / 100) 
			OR stockquantity.stockonorder != CONVERT(float, temp_stockload.stockonorder / 100) 
			OR stockquantity.stockdamage != CONVERT(float, temp_stockload.stockdamage / 100) 
			OR stockquantity.dateupdated != temp_stockload.planneddate
		)

	UPDATE stockquantity
	SET qtyavailable = 0,
		stock = 0,
		--stockactual = 0,
		stockonorder = 0,
		stockdamage = 0
	WHERE NOT EXISTS (SELECT 1
	    			  FROM temp_stockload
	                  WHERE stockquantity.itemno = temp_stockload.itemno
	                      AND stockquantity.stocklocn = temp_stockload.warehouseno)

	UPDATE temp_stockload
	SET stockprocessed = 1
	FROM stockitem
	WHERE stockitem.itemno = temp_stockload.itemno
		AND stockitem.stocklocn = temp_stockload.warehouseno
	
	/* Now lets report as an error all those rows not updated */
	INSERT INTO Interfaceerror
		(interface, 
		runno, 
		errordate, 
		severity,
		errortext)
	SELECT	
		@interface AS interface, 
		@runno AS runno,
		GETDATE() AS errordate,
		'W' AS severity,
		warehouseno  + ',' + itemno + ' not found in CoSACS. Record ignored.'
	FROM temp_stockload
	WHERE stockprocessed = 0

	SET @return = @@error
GO