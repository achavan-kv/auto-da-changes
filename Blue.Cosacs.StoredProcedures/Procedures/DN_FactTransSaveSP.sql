SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FactTransSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FactTransSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_FactTransSaveSP
			@acctno varchar(12),
			@agrmtno int,
			@buffno int,
			@itemno varchar(18),
			@stocklocn smallint,
			@tccode varchar(2),
			@trantype varchar(3),
			@trandate datetime,
			@quantity float,
			@price float,
			@taxamt float,
			@value float,
			@return int OUTPUT

AS

	--ip - 16/01/13 - #11534 - LW75555 - Merged from CoSACS 6.5
	--This needs to change in .Net so that the itemID is passed to the SP
	DECLARE @itemID INT
	
	SELECT @itemID = ISNULL(ID, 0) 
	FROM dbo.StockInfo 
	WHERE itemno = @itemno
	
	--This needs to change in .Net so that the itemID is passed to the SP
	IF NOT EXISTS
	(SELECT 'a' FROM dbo.lineitem 
	 WHERE acctno = @acctno
		AND agrmtno = @agrmtno
		AND stocklocn = @stocklocn
		AND ItemID = @itemID
	)
		SET @itemID = 0
	
	-- #16381 removed
	--if (@quantity=0) --IP - 18/02/10 - CR1072 - LW 70615 - General Fixes from 4.3 - Merge
	--begin
	--	set @return=1
	--	return
	--end

	SET 	@return = 0			--initialise return code

	IF(@trantype = '01')
	BEGIN
		DELETE
		FROM		facttrans
		WHERE	acctno	= @acctno
		and		trantype =  '01'
		--and		itemno =  @itemno
		AND ItemID = @itemID			--ip - 16/01/13
		and		stocklocn =  @stocklocn
		and		buffno =  @buffno
		and		TCCode = @tccode;
	END

	INSERT INTO facttrans  
	(
		OrigBr,
		acctno,
		AgrmtNo,
		BuffNo,
		ItemNo,
		StockLocn,
		TCCode,
		trantype,
		trandate,
		quantity,
		price,
		taxamt,
		value,
		ItemID		--ip - 16/01/13
	)
	--values --IP - 18/02/10 - CR1072 - LW 70615 - General Fixes from 4.3 - Merge - Commented out
	--(
	select
		0,
		@acctno,
		@agrmtno,
		@buffno,
		@itemno,
		@stocklocn,
		@tccode,
		@trantype,
		@trandate,
		@quantity,
		@price,
		@taxamt,
		@value,
		@itemID		--ip - 16/01/13
	--IP - 18/02/10 - CR1072 - LW 70615 - General Fixes from 4.3 - Merge
	where not exists
	(select 'x' from facttrans f
	where f.acctno=@acctno
	and f.agrmtno=@agrmtno
	and f.buffno=@buffno 
	--and f.itemno=@itemno
	AND f.ItemID = @itemID		--ip - 16/01/13
	and f.stocklocn=@stocklocn
	and f.tccode=@tccode 
	and f.trantype=	@trantype
	and f.trandate=	@trandate)
		--)

	SET @return = @@error
GO

    
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

