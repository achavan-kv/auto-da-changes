IF EXISTS (
		SELECT 1
		FROM sys.procedures WITH (NOLOCK)
		WHERE NAME = 'DeliverNonStocks'
			AND type = 'P'
		)
	DROP PROCEDURE [dbo].[DeliverNonStocks]
GO

-- ===================================================================================
-- Procedure Name:	[dbo].[DeliverNonStocks]
-- Author:			Ashwini Akula
-- Create date:		03/02/2019
-- Description:		Procedure used to deliver non stock items for only sale of 'non stock' 
					-- (i.e. during sale no other types items selected) 
					-- Initially there was non stock sale allowed with stock items. 
					-- This change is for CR where we should be able to sell Non stocks only 
					-- by checking the NS checkbox on sales screen
-- ===================================================================================
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeliverNonStocks] 
	@acctno VARCHAR(12)
	,@user VARCHAR(10)
	,@return INT OUT
AS
BEGIN
	DECLARE @stocklocn INT
		,@price MONEY
		,@ordval MONEY
		,@itemid INT
		,@parentitemid INT
		,@itemno VARCHAR(18)
		,@parentitemno VARCHAR(18)
		,@agrmtno INT
		,@itemtype VARCHAR(2)
		,@quantity INT

	-- Step 1: Create Temp table and insert details to take items of nonstock account.
	CREATE TABLE #lineitemNonStock (
		ID INT PRIMARY KEY IDENTITY
		,stocklocn INT
		,price MONEY
		,ordval MONEY
		,itemid INT
		,parentitemid INT
		,itemno VARCHAR(18)
		,parentitemno VARCHAR(18)
		,agrmtno INT
		,itemtype VARCHAR(2)
		,quantity INT
		)
			
	INSERT INTO #lineitemNonStock (
		stocklocn
		,price
		,ordval
		,itemid
		,parentitemid
		,itemno
		,parentitemno
		,agrmtno
		,itemtype
		,quantity
		)
	SELECT l.stocklocn
		,l.price
		,l.ordval
		,l.itemid
		,l.parentitemid
		,l.itemno
		,l.parentitemno
		,l.agrmtno
		,l.itemtype
		,l.quantity
	FROM lineitem l
		 INNER JOIN NonstockAccounts n ON l.acctno = n.acctno
	WHERE l.acctno = @acctno
		AND l.itemtype = 'N'
		AND l.quantity <> 0
		AND l.contractno = ''

	--*************************************************************************************************************	
	DECLARE @count INT
		,@id INT

	SET @return = 0

	SELECT	@count = count(id)
	FROM	dbo.lineitem
	WHERE	acctno = @acctno
			AND itemtype = 'N'

	SET @id = 1

	--loop through all the items of the temp table to insert into delivery table
	WHILE (@id <= @count)
	BEGIN
		SELECT @stocklocn = stocklocn
			,@price = price
			,@ordval = ordval
			,@itemid = itemid
			,@parentitemid = parentitemid
			,@itemno = itemno
			,@parentitemno = parentitemno
			,@agrmtno = agrmtno
			,@itemtype = itemtype
			,@quantity = quantity
		FROM #lineitemNonStock
		WHERE id = @id

		INSERT INTO delivery (
			origbr
			,acctno
			,agrmtno
			,datedel
			,delorcoll
			,itemno
			,stocklocn
			,quantity
			,retstocklocn
			,retval
			,buffno
			,buffbranchno
			,datetrans
			,branchno
			,transrefno
			,transvalue
			,runno
			,contractno
			,ReplacementMarker
			,notifiedby
			,ftnotes
			,parentitemno
			,itemid
			,parentitemid
			)
		SELECT 0
			,@acctno
			,@agrmtno
			,GETDATE()
			,'D'
			,i.iupc
			,@stocklocn
			,@quantity
			,@stocklocn
			,@ordval
			,hibuffno + 1
			,@stocklocn
			,GETDATE()
			,@stocklocn
			,hirefno + 1
			,@ordval
			,0
			,''
			,''
			,@user
			,'DND2'
			,isnull(s.IUPC, '')
			,@itemid
			,@parentitemid
		FROM branch
		INNER JOIN stockinfo i ON i.id = @itemid
		LEFT OUTER JOIN stockinfo s ON s.id = @parentitemid
		WHERE branchno = @stocklocn

		--update branch table to increment the value of buffno and transrefno
		UPDATE branch
		SET hibuffno = hibuffno + 1
			,hirefno = hirefno + 1
		WHERE branchno = @stocklocn

		--update lineitem for delqty after delivery of item
		UPDATE lineitem
		SET delqty = (
				SELECT sum(quantity)
				FROM delivery
				WHERE acctno = @acctno
					AND itemno = @itemno
				)
		WHERE acctno = @acctno
			AND itemno = @itemno

		SET @id = @id + 1
	END

	DROP TABLE #lineitemNonStock
END